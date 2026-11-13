using System.Security.Cryptography;
using System.Text;
using F = FusionMod.FusionSaveFormat;

namespace FusionMod;

// Board-container storage only. No profile writer, native serializer, gameplay
// transition or fallback truncating overwrite is reachable from this class.
internal sealed class FusionSaveStore
{
    internal enum Outcome { Committed, Failed, RecoveryRequired }
    internal sealed record Result(Outcome Outcome, F.Package Preserved, string? Error = null);
    private readonly F.Owner owner;
    private readonly string target, directory;
    private readonly long quota;
    private readonly Action<string>? checkpoint;
    private bool entered;
    private bool compacting;
    private F.Package[] operationPackages = Array.Empty<F.Package>();
    private const int MaximumMetadataFiles = 16384;
    private const int CompactionReserve = 2 * 1024 * 1024;
    internal FusionSaveStore(F.Owner owner, string targetPath, string privateRoot,
        long quotaBytes = 1024L * 1024 * 1024, Action<string>? checkpoint = null)
    {
        owner.Validate();
        if (!Path.IsPathFullyQualified(targetPath) || !Path.IsPathFullyQualified(privateRoot) || quotaBytes <= 0)
            throw new ArgumentException("Fusion storage paths must be absolute and quota must be positive.");
        this.owner = owner; target = Path.GetFullPath(targetPath); quota = quotaBytes; this.checkpoint = checkpoint;
        byte[] identity = Encoding.UTF8.GetBytes(owner.Profile.Length + ":" + owner.Profile + owner.FileName.Length + ":" + owner.FileName);
        directory = Path.Combine(Path.GetFullPath(privateRoot), F.Digest(identity));
        if (string.Equals(target, directory, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Fusion recovery storage must be separate from the native target.");
    }

    internal string DirectoryPath => directory;
    private string ObjectPath(string digest, string suffix)
    {
        if (digest.Length != 64 || digest.Any(c => !(c >= '0' && c <= '9' || c >= 'A' && c <= 'F')))
            throw new InvalidDataException("Invalid Fusion immutable object digest.");
        return Path.Combine(directory, digest + suffix);
    }

    private IDisposable Enter()
    {
        if (entered) throw new InvalidOperationException("Fusion board transaction is already active.");
        entered = true;
        try
        {
            Directory.CreateDirectory(directory);
            var stream = new FileStream(Path.Combine(directory, "owner.lock"), FileMode.OpenOrCreate,
                FileAccess.ReadWrite, FileShare.None);
            return new Lease(this, stream);
        }
        catch { entered = false; throw; }
    }
    private sealed class Lease : IDisposable
    {
        private FusionSaveStore? store; private readonly FileStream file;
        internal Lease(FusionSaveStore store, FileStream file) { this.store = store; this.file = file; }
        public void Dispose()
        {
            if (store == null) return;
            try { file.Dispose(); } finally { store.entered = false; store = null; }
        }
    }

    private static byte[] ReadBounded(string path, int maximum)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        long length = stream.Length;
        if (length < 0 || length > maximum) throw new InvalidDataException("Fusion file exceeds its size limit.");
        byte[] bytes = new byte[(int)length]; int offset = 0;
        while (offset < bytes.Length)
        {
            int count = stream.Read(bytes, offset, bytes.Length - offset);
            if (count == 0) throw new InvalidDataException("Fusion file was truncated while reading.");
            offset += count;
        }
        if (stream.ReadByte() != -1 || stream.Length != length) throw new InvalidDataException("Fusion file changed while reading.");
        return bytes;
    }

    private void CheckQuota(int additionalBytes)
    {
        (long Bytes, int Count) Usage()
        {
            long total = additionalBytes; int count = 0;
            foreach (string path in Directory.EnumerateFiles(directory)) { ++count; total = checked(total + new FileInfo(path).Length); }
            return (total, count);
        }
        var used = Usage();
        if (compacting)
        {
            if (used.Bytes > quota + CompactionReserve || used.Count > MaximumMetadataFiles + 4)
                throw new IOException("Fusion checkpoint reserve is exhausted; existing saves are preserved.");
            return;
        }
        for (int pass = 0; pass < 2 && (used.Bytes > quota || used.Count >= MaximumMetadataFiles); ++pass)
        { CompactHistory(); used = Usage(); }
        if (used.Bytes > quota || used.Count >= MaximumMetadataFiles)
            throw new IOException("Protected Fusion recovery data exceeds the quota; existing saves are preserved.");
    }

    private void PutImmutable(string path, ReadOnlySpan<byte> bytes)
    {
        if (File.Exists(path))
        {
            if (!ReadBounded(path, bytes.Length).AsSpan().SequenceEqual(bytes))
                throw new InvalidDataException("Conflicting Fusion immutable object; it was not overwritten.");
            return;
        }
        CheckQuota(bytes.Length);
        string temporary = path + "." + Convert.ToHexString(RandomNumberGenerator.GetBytes(16)) + ".tmp";
        try
        {
            WriteNew(temporary, bytes);
            if (!ReadBounded(temporary, bytes.Length).AsSpan().SequenceEqual(bytes))
                throw new IOException("Fusion immutable object failed readback.");
            // Non-overwriting publication; a concurrent conflicting object is
            // an error rather than permission to replace another generation.
            File.Move(temporary, path);
            if (!ReadBounded(path, bytes.Length).AsSpan().SequenceEqual(bytes))
                throw new IOException("Fusion immutable object failed published readback.");
        }
        finally { RemoveOwnTemporary(temporary); }
    }
    private static void WriteNew(string path, ReadOnlySpan<byte> bytes)
    {
        using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        stream.Write(bytes); stream.Flush(true);
    }
    private static void RemoveOwnTemporary(string path)
    {
        try { File.Delete(path); } catch (IOException) { } catch (UnauthorizedAccessException) { }
        // An unremovable unique temporary object is retained, never confused
        // with a committed object or used as a reason to delete old packages.
    }

    private void Publish(F.Package package)
    {
        if (package.Owner != owner) throw new InvalidDataException("Fusion recovery owner changed.");
        PutImmutable(ObjectPath(package.Digest, ".package"), package.Encoded.Span);
        // Independent decode verifies the exact published full object before
        // any primary-file mutation, not merely a digest of caller memory.
        var verified = ReadPackage(package.Digest);
        if (verified.Digest != package.Digest) throw new IOException("Fusion recovery package verification failed.");
        foreach (var snapshot in package.Snapshots)
        {
            PutImmutable(ObjectPath(snapshot.PayloadDigest, ".companion"), snapshot.Encoded.Span);
            PutImmutable(ObjectPath(snapshot.NativeDigest, ".binding"), Encoding.ASCII.GetBytes(snapshot.PayloadDigest));
        }
    }
    private F.Package ReadPackage(string digest)
    {
        byte[] bytes = ReadBounded(ObjectPath(digest, ".package"), F.MaximumPackageBytes);
        if (F.Digest(bytes) != digest) throw new InvalidDataException("Fusion recovery package digest mismatch.");
        return F.DecodePackage(bytes, owner);
    }
    internal F.Snapshot ReadSnapshot(ReadOnlySpan<byte> chunk)
    {
        using var lease = Enter();
        string nativeDigest = F.Digest(chunk);
        string payload = Encoding.ASCII.GetString(ReadBounded(ObjectPath(nativeDigest, ".binding"), 64));
        byte[] bytes = ReadBounded(ObjectPath(payload, ".companion"), F.MaximumProjectileRecords * 20 + 8192);
        if (F.Digest(bytes) != payload) throw new InvalidDataException("Fusion companion digest mismatch.");
        return F.DecodeSnapshot(bytes, chunk, owner);
    }

    private bool TargetMatches(F.Package package)
    {
        if (package.NativeAbsent) return !File.Exists(target);
        if (!File.Exists(target)) return false;
        byte[] bytes = ReadBounded(target, F.MaximumContainerBytes);
        return bytes.Length == package.Native.Length && F.Digest(bytes) == package.NativeDigest;
    }
    private string WriteTargetTemporary(F.Package package)
    {
        string temporary = target + ".fusion-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(16)) + ".tmp";
        try
        {
            WriteNew(temporary, package.Native.Span);
            byte[] bytes = ReadBounded(temporary, F.MaximumContainerBytes);
            if (bytes.Length != package.Native.Length || F.Digest(bytes) != package.NativeDigest)
                throw new IOException("Fusion native candidate failed readback.");
            return temporary;
        }
        catch { RemoveOwnTemporary(temporary); throw; }
    }
    private void ReplaceTarget(string temporary, bool previouslyAbsent)
    {
        if (previouslyAbsent) File.Move(temporary, target);
        else File.Replace(temporary, target, null);
    }

    private byte[] Intent(F.Package oldPackage, F.Package candidate)
        => Encoding.ASCII.GetBytes("FINT1\n" + Convert.ToHexString(RandomNumberGenerator.GetBytes(16)) + "\n" + oldPackage.Digest + "\n" + candidate.Digest + "\n");
    private (string Old, string New) ReadIntent(string path)
    {
        byte[] bytes = ReadBounded(path, 256);
        string digest = Path.GetFileNameWithoutExtension(path);
        if (F.Digest(bytes) != digest) throw new InvalidDataException("Fusion intent digest mismatch.");
        string[] fields = Encoding.ASCII.GetString(bytes).Split('\n');
        if (fields.Length != 5 || fields[0] != "FINT1" || fields[1].Length != 32 || fields[4] != ""
            || fields[1].Any(c => !(c >= '0' && c <= '9' || c >= 'A' && c <= 'F')))
            throw new InvalidDataException("Invalid Fusion transaction intent.");
        _ = ObjectPath(fields[2], ".package"); _ = ObjectPath(fields[3], ".package");
        return (fields[2], fields[3]);
    }
    private void Complete(string intentDigest, F.Package selected)
    {
        var previousRoots = operationPackages;
        operationPackages = previousRoots.Concat(new[] { selected, ReadPackage(ReadIntent(ObjectPath(intentDigest, ".intent")).Old) }).ToArray();
        try
        {
            PutImmutable(ObjectPath(intentDigest, ".done"), Encoding.ASCII.GetBytes("FDONE1\n" + intentDigest + "\n" + selected.Digest + "\n"));
            PublishHead(intentDigest, selected.Digest);
        }
        finally { operationPackages = previousRoots; }
    }
    private sealed record Head(string Intent, long Sequence, string Package, string Previous);
    private Head ReadHeadMetadata(string path)
    {
        string[] fields = Encoding.ASCII.GetString(ReadBounded(path, 512)).Split('\n');
        if (fields.Length != 7 || fields[0] != "FHEAD2" || fields[6] != ""
            || !long.TryParse(fields[1], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out long sequence)
            || sequence <= 0 || F.Digest(Encoding.ASCII.GetBytes(string.Join('\n', fields.Take(5)) + "\n")) != fields[5])
            throw new InvalidDataException("Invalid Fusion standalone commit head.");
        _ = ObjectPath(fields[3], ".package"); _ = ObjectPath(fields[4], ".package");
        return new(Path.GetFileNameWithoutExtension(path), sequence, fields[3], fields[4]);
    }
    private void ValidateHead(Head head)
    {
        var pair = ReadIntent(ObjectPath(head.Intent, ".intent"));
        if (!HasCompletion(head.Intent, pair.Old, pair.New)
            || Encoding.ASCII.GetString(ReadBounded(ObjectPath(head.Intent, ".done"), 256)) != "FDONE1\n" + head.Intent + "\n" + head.Package + "\n")
            throw new InvalidDataException("Fusion commit checkpoint does not match its completion.");
        _ = ReadPackage(head.Package); _ = ReadPackage(head.Previous);
    }
    private Head? LatestHead(bool repair)
    {
        var heads = new List<Head>();
        foreach (string path in Directory.EnumerateFiles(directory, "*.head"))
        {
            if (heads.Count >= MaximumMetadataFiles) throw new IOException("Fusion commit history exceeds bounds.");
            heads.Add(ReadHeadMetadata(path));
        }
        heads.Sort((a, b) => a.Sequence.CompareTo(b.Sequence));
        var latest = heads.LastOrDefault();
        if (latest != null) ValidateHead(latest);
        if (latest != null && heads.Count(head => head.Sequence == latest.Sequence) != 1)
            throw new InvalidDataException("Forked Fusion commit checkpoint.");
        if (!repair) return latest;
        var missing = Directory.EnumerateFiles(directory, "*.done")
            .Where(path => !File.Exists(ObjectPath(Path.GetFileNameWithoutExtension(path), ".head"))).Take(2).ToArray();
        if (missing.Length > 1) throw new InvalidDataException("Multiple incomplete Fusion commit heads require explicit recovery.");
        if (missing.Length == 1)
        {
            string intent = Path.GetFileNameWithoutExtension(missing[0]); var pair = ReadIntent(ObjectPath(intent, ".intent"));
            if (!HasCompletion(intent, pair.Old, pair.New)) throw new InvalidDataException("Invalid pending Fusion completion.");
            string selected = Encoding.ASCII.GetString(ReadBounded(missing[0], 256)).Split('\n')[2];
            _ = ReadPackage(selected); PublishHead(intent, selected); return LatestHead(false);
        }
        return latest;
    }
    private void PublishHead(string intent, string package)
    {
        string path = ObjectPath(intent, ".head");
        if (File.Exists(path)) { _ = LatestHead(false); return; }
        var previous = LatestHead(false);
        var pair = ReadIntent(ObjectPath(intent, ".intent"));
        string body = "FHEAD2\n" + checked((previous?.Sequence ?? 0) + 1).ToString(System.Globalization.CultureInfo.InvariantCulture)
            + "\n" + (previous?.Intent ?? "ROOT") + "\n" + package + "\n" + (previous?.Package ?? pair.Old) + "\n";
        var previousRoots = operationPackages;
        operationPackages = previousRoots.Concat(new[] { ReadPackage(package), ReadPackage(previous?.Package ?? pair.Old) }).ToArray();
        try { PutImmutable(path, Encoding.ASCII.GetBytes(body + F.Digest(Encoding.ASCII.GetBytes(body)) + "\n")); }
        finally { operationPackages = previousRoots; }
    }
    private static bool TryDeleteOwn(string path)
    { try { File.Delete(path); return !File.Exists(path); } catch (IOException) { return false; } catch (UnauthorizedAccessException) { return false; } }
    private void CompactHistory()
    {
        if (compacting) return;
        var head = LatestHead(false);
        if (head == null || PendingIntents().Length != 0) return;
        compacting = true;
        try
        {
            var keepPackages = new HashSet<string> { head.Package, head.Previous };
            foreach (var package in operationPackages)
                if (File.Exists(ObjectPath(package.Digest, ".package"))) keepPackages.Add(package.Digest);
            var keepHeads = new HashSet<string> { head.Intent };
            var closedSessions = new List<string>();
            foreach (string path in Directory.EnumerateFiles(directory, "*.session"))
            {
                string epoch = Path.GetFileNameWithoutExtension(path);
                string[] fields = Encoding.ASCII.GetString(ReadBounded(path, 256)).Split('\n');
                if (fields.Length != 4 || fields[0] != "FSESSION1" || fields[1] != epoch || fields[3] != "")
                    throw new InvalidDataException("Invalid Fusion session during compaction.");
                keepPackages.Add(fields[2]);
                if (File.Exists(ObjectPath(epoch, ".sessionclosed")) || File.Exists(ObjectPath(epoch, ".sessionack"))) closedSessions.Add(epoch);
            }
            foreach (string path in Directory.EnumerateFiles(directory, "*.choice"))
            {
                byte[] bytes = ReadBounded(path, 512); string[] fields = Encoding.ASCII.GetString(bytes).Split('\n');
                if (F.Digest(bytes) != Path.GetFileNameWithoutExtension(path) || fields.Length != 5 || fields[0] != "FCHOICE1")
                    throw new InvalidDataException("Invalid Fusion branch choice during compaction.");
                if (fields[1] == "ROOT") continue;
                var branch = ReadHeadMetadata(ObjectPath(fields[1], ".head"));
                keepHeads.Add(branch.Intent); keepPackages.Add(branch.Package); keepPackages.Add(branch.Previous);
            }
            var keepPayloads = new HashSet<string>(); var keepBindings = new HashSet<string>();
            foreach (string digest in keepPackages)
                foreach (var snapshot in ReadPackage(digest).Snapshots)
                { keepPayloads.Add(snapshot.PayloadDigest); keepBindings.Add(snapshot.NativeDigest); }
            // This verified immutable checkpoint is published before any
            // reclamation. The standalone current head already contains both
            // complete committed roots; no reader needs the sequence-1 prefix.
            byte[] checkpoint = Encoding.ASCII.GetBytes("FGC1\n" + head.Intent + "\n" + string.Join('\n', keepPackages.OrderBy(value => value)) + "\n");
            string checkpointPath = ObjectPath(F.Digest(checkpoint), ".checkpoint"); PutImmutable(checkpointPath, checkpoint);
            foreach (string epoch in closedSessions)
                if (TryDeleteOwn(ObjectPath(epoch, ".session")))
                { TryDeleteOwn(ObjectPath(epoch, ".sessionclosed")); TryDeleteOwn(ObjectPath(epoch, ".sessionack")); }
            foreach (string path in Directory.EnumerateFiles(directory, "*.head").ToArray())
            {
                string id = Path.GetFileNameWithoutExtension(path); if (keepHeads.Contains(id)) continue;
                // Leave a readable old head until its intent/completion names
                // are gone. Interrupted pruning therefore leaves only orphans.
                TryDeleteOwn(ObjectPath(id, ".done")); TryDeleteOwn(ObjectPath(id, ".intent"));
                if (!File.Exists(ObjectPath(id, ".done")) && !File.Exists(ObjectPath(id, ".intent"))) TryDeleteOwn(path);
            }
            foreach (string path in Directory.EnumerateFiles(directory).ToArray())
            {
                string id = Path.GetFileNameWithoutExtension(path), extension = Path.GetExtension(path);
                if (extension == ".package" && !keepPackages.Contains(id)
                    || extension == ".companion" && !keepPayloads.Contains(id)
                    || extension == ".binding" && !keepBindings.Contains(id)
                    || extension == ".checkpoint" && path != checkpointPath) TryDeleteOwn(path);
            }
        }
        finally { compacting = false; }
    }
    private bool HasCompletion(string intentDigest, string oldDigest, string newDigest)
    {
        string path = ObjectPath(intentDigest, ".done");
        if (!File.Exists(path)) return false;
        string value = Encoding.ASCII.GetString(ReadBounded(path, 256));
        if (value != "FDONE1\n" + intentDigest + "\n" + oldDigest + "\n"
            && value != "FDONE1\n" + intentDigest + "\n" + newDigest + "\n")
            throw new InvalidDataException("Conflicting Fusion completion record.");
        return true;
    }
    private string[] PendingIntents()
    {
        var pending = new List<string>(); int count = 0;
        foreach (string path in Directory.EnumerateFiles(directory, "*.intent"))
        {
            if (++count > MaximumMetadataFiles) throw new IOException("Fusion transaction history exceeds bounds.");
            var pair = ReadIntent(path); string digest = Path.GetFileNameWithoutExtension(path);
            string oldHead = ObjectPath(digest, ".head");
            if (File.Exists(oldHead)) continue; // published checkpoint or harmless prune orphan
            if (!HasCompletion(digest, pair.Old, pair.New)) pending.Add(path);
        }
        if (pending.Count > 1) throw new InvalidDataException("Multiple unresolved Fusion transactions; explicit recovery is required.");
        return pending.ToArray();
    }

    internal Result Commit(F.Package oldPackage, F.Package candidate)
    {
        if (oldPackage.Owner != owner || candidate.Owner != owner)
            throw new InvalidDataException("Invalid Fusion board transaction owner or candidate.");
        using var lease = Enter();
        operationPackages = new[] { oldPackage, candidate };
        string? temporary = null;
        try
        {
            if (PendingIntents().Length != 0) return new(Outcome.RecoveryRequired, oldPackage, "An earlier Fusion transaction requires recovery.");
            if (!TargetMatches(oldPackage)) return new(Outcome.RecoveryRequired, oldPackage, "Native board file changed outside this transaction.");
            if (oldPackage.NativeDigest == candidate.NativeDigest && oldPackage.NativeAbsent == candidate.NativeAbsent
                && oldPackage.Digest != candidate.Digest)
                return new(Outcome.RecoveryRequired, oldPackage, "Identical native bytes have conflicting Fusion provenance.");
            Publish(oldPackage); checkpoint?.Invoke("old-package");
            Publish(candidate); checkpoint?.Invoke("new-package");
            if (oldPackage.Digest == candidate.Digest) return new(Outcome.Committed, candidate);
            CheckQuota(8192); // completion/head space before primary mutation
            byte[] intent = Intent(oldPackage, candidate); string intentDigest = F.Digest(intent);
            PutImmutable(ObjectPath(intentDigest, ".intent"), intent); checkpoint?.Invoke("intent");
            if (!candidate.NativeAbsent) temporary = WriteTargetTemporary(candidate);
            checkpoint?.Invoke("native-temporary");
            if (!TargetMatches(oldPackage)) return new(Outcome.RecoveryRequired, oldPackage, "Native board file changed before replacement.");
            Exception? replaceError = null;
            try
            {
                if (candidate.NativeAbsent) File.Delete(target);
                else ReplaceTarget(temporary!, oldPackage.NativeAbsent);
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or NotSupportedException)
            { replaceError = error; }
            checkpoint?.Invoke("replacement");
            if (TargetMatches(candidate))
            {
                Complete(intentDigest, candidate); checkpoint?.Invoke("completion");
                return new(Outcome.Committed, candidate);
            }
            if (TargetMatches(oldPackage))
            {
                Complete(intentDigest, oldPackage);
                return new(Outcome.Failed, oldPackage, replaceError?.Message ?? "Native replacement did not commit.");
            }
            return new(Outcome.RecoveryRequired, oldPackage, replaceError?.Message ?? "Native replacement outcome is ambiguous.");
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            // The bridge rolls working buffers back. The verified old full
            // package is never removed, even when target readback/repair fails.
            return new(Outcome.RecoveryRequired, oldPackage, error.Message);
        }
        finally { operationPackages = Array.Empty<F.Package>(); if (temporary != null) RemoveOwnTemporary(temporary); }
    }

    internal F.Package? Recover()
    {
        using var lease = Enter();
        string[] pending = PendingIntents();
        if (pending.Length == 0) return null;
        var pair = ReadIntent(pending[0]);
        F.Package oldPackage = ReadPackage(pair.Old), candidate = ReadPackage(pair.New);
        string intentDigest = Path.GetFileNameWithoutExtension(pending[0]);
        if (oldPackage.NativeDigest == candidate.NativeDigest && oldPackage.NativeAbsent == candidate.NativeAbsent
            && oldPackage.Digest != candidate.Digest)
            throw new InvalidDataException("Ambiguous native/Fusion recovery binding.");
        if (TargetMatches(candidate)) { Publish(candidate); Complete(intentDigest, candidate); return candidate; }
        if (TargetMatches(oldPackage)) { Publish(oldPackage); Complete(intentDigest, oldPackage); return oldPackage; }
        if (File.Exists(target))
        {
            // Partial/conflicting readable bytes are evidence. Without an
            // explicit reviewed choice they cannot be overwritten as old data.
            throw new InvalidDataException("Native board file conflicts with both Fusion recovery packages.");
        }
        if (oldPackage.NativeAbsent) { Complete(intentDigest, oldPackage); return oldPackage; }
        string temporary = WriteTargetTemporary(oldPackage);
        try
        {
            ReplaceTarget(temporary, true);
            if (!TargetMatches(oldPackage)) throw new IOException("Fusion recovery repair failed verification.");
            Publish(oldPackage); Complete(intentDigest, oldPackage); return oldPackage;
        }
        finally { RemoveOwnTemporary(temporary); }
    }

    internal F.Package? CurrentPublishedPackage()
    {
        using var lease = Enter();
        var head = LatestHead(true);
        if (head != null)
        {
            var selected = ReadPackage(head.Package);
            if (!TargetMatches(selected)) return null;
            Publish(selected); return selected;
        }
        F.Package? found = null; int count = 0;
        foreach (string path in Directory.EnumerateFiles(directory, "*.package"))
        {
            if (++count > MaximumMetadataFiles) throw new IOException("Fusion package history exceeds bounds.");
            var package = ReadPackage(Path.GetFileNameWithoutExtension(path));
            if (!TargetMatches(package)) continue;
            if (found != null && found.Digest != package.Digest)
                throw new InvalidDataException("The native board image has conflicting complete Fusion packages.");
            found = package;
        }
        // A completed intent is not required for rehydration. Its exact native
        // image plus validated full package is sufficient. Existing conflicting
        // objects are retained and refused by PutImmutable; only missing names
        // are published, with full readback before native _read may begin.
        if (found != null) Publish(found);
        return found;
    }
    internal bool HasPublishedHistory()
    { return Directory.Exists(directory) && Directory.EnumerateFiles(directory, "*.package").Any(); }
    private byte[] CurrentChoice()
    {
        var head = LatestHead(true); bool absent = !File.Exists(target);
        byte[] native = absent ? Array.Empty<byte>() : ReadBounded(target, F.MaximumContainerBytes);
        return Encoding.ASCII.GetBytes("FCHOICE1\n" + (head?.Intent ?? "ROOT") + "\n" + (absent ? "ABSENT" : "PRESENT") + "\n" + F.Digest(native) + "\n");
    }
    internal bool HasCurrentBranchChoice()
    {
        using var lease = Enter(); byte[] bytes = CurrentChoice(); string path = ObjectPath(F.Digest(bytes), ".choice");
        return File.Exists(path) && ReadBounded(path, 512).AsSpan().SequenceEqual(bytes);
    }
    internal void RecordCurrentBranchChoice()
    {
        using var lease = Enter(); byte[] choice = CurrentChoice();
        if (File.Exists(target))
        {
            byte[] native = ReadBounded(target, F.MaximumContainerBytes);
            PutImmutable(ObjectPath(F.Digest(native), ".branch"), native);
        }
        if (!choice.AsSpan().SequenceEqual(CurrentChoice())) throw new IOException("Native recovery branch changed before confirmation.");
        PutImmutable(ObjectPath(F.Digest(choice), ".choice"), choice);
    }
    internal string OpenSession(F.Package previous)
    {
        using var lease = Enter(); Publish(previous);
        string epoch = F.Digest(RandomNumberGenerator.GetBytes(32));
        PutImmutable(ObjectPath(epoch, ".session"), Encoding.ASCII.GetBytes("FSESSION1\n" + epoch + "\n" + previous.Digest + "\n"));
        return epoch;
    }
    internal void CloseSession(string epoch, string packageDigest)
    {
        using var lease = Enter(); _ = ReadPackage(packageDigest);
        _ = ReadBounded(ObjectPath(epoch, ".session"), 256);
        PutImmutable(ObjectPath(epoch, ".sessionclosed"), Encoding.ASCII.GetBytes("FCLOSED1\n" + epoch + "\n" + packageDigest + "\n"));
    }
    internal string[] UnacknowledgedSessions()
    {
        using var lease = Enter(); var pending = new List<string>(); int count = 0;
        foreach (string path in Directory.EnumerateFiles(directory, "*.session"))
        {
            if (++count > MaximumMetadataFiles) throw new IOException("Fusion session history exceeds bounds.");
            string epoch = Path.GetFileNameWithoutExtension(path);
            string[] values = Encoding.ASCII.GetString(ReadBounded(path, 256)).Split('\n');
            if (values.Length != 4 || values[0] != "FSESSION1" || values[1] != epoch || values[3] != "")
                throw new InvalidDataException("Malformed Fusion open-session record.");
            _ = ReadPackage(values[2]);
            if (File.Exists(ObjectPath(epoch, ".sessionclosed")))
            {
                string[] closed = Encoding.ASCII.GetString(ReadBounded(ObjectPath(epoch, ".sessionclosed"), 256)).Split('\n');
                if (closed.Length != 4 || closed[0] != "FCLOSED1" || closed[1] != epoch || closed[3] != "")
                    throw new InvalidDataException("Malformed Fusion closed-session record.");
                _ = ReadPackage(closed[2]); continue;
            }
            string acknowledgment = ObjectPath(epoch, ".sessionack");
            if (File.Exists(acknowledgment))
            {
                if (Encoding.ASCII.GetString(ReadBounded(acknowledgment, 128)) != "FACK1\n" + epoch + "\n")
                    throw new InvalidDataException("Malformed Fusion session acknowledgment.");
                continue;
            }
            pending.Add(epoch);
        }
        return pending.ToArray();
    }
    internal void AcknowledgeSession(string epoch)
    {
        using var lease = Enter(); _ = ReadBounded(ObjectPath(epoch, ".session"), 256);
        PutImmutable(ObjectPath(epoch, ".sessionack"), Encoding.ASCII.GetBytes("FACK1\n" + epoch + "\n"));
    }
}
