using System.Buffers.Binary;
using System.Security.Cryptography;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppOOI.Platforms.Steam;
using HarmonyLib;
using ReplantedModUi;
using F = FusionMod.FusionSaveFormat;
using NativeBuffers = Il2CppSystem.Collections.Generic.List<Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<byte>>;

namespace FusionMod;

// All scopes belong to one concrete service/profile/file generation. A refused
// nested call never owns a scope and therefore cannot unwind another call.
internal static class FusionSaveCoordinator
{
    internal enum Operation { Read, Serialize, Clear, Load }
    internal sealed class OwnerState
    {
        internal readonly SerializationService Service;
        internal readonly F.Owner Identity;
        internal readonly long Generation;
        internal readonly string Target;
        internal readonly FusionSaveStore Store;
        internal readonly Dictionary<int, F.Snapshot> Snapshots = new();
        internal NativeBuffers? Published;
        internal byte[]? NativeBytes;
        internal bool NativeAbsent, Held, Quarantined, ReadValidated;
        internal bool LastCommit;
        internal bool BranchChoiceRequired;
        internal bool AcknowledgmentPending;
        internal string? Error;
        internal string? SessionEpoch, LastPackage;
        internal string[] InterruptedSessions = Array.Empty<string>();
        internal bool SessionsChecked;
        internal F.Package? ExpectedPackage;
        internal Scope? Active;
        internal OwnerState(SerializationService service, F.Owner identity, long generation, string target)
        {
            Service = service; Identity = identity; Generation = generation; Target = target;
            Store = new(identity, target, Path.Combine(Path.GetDirectoryName(target)!, "FusionProvenance"));
        }
    }
    internal sealed class Scope
    {
        internal readonly OwnerState Owner;
        internal readonly Operation Operation;
        internal readonly NativeBuffers Previous;
        internal readonly Dictionary<int, F.Snapshot> PreviousSnapshots;
        internal readonly long Generation;
        internal readonly Board? Board;
        internal FusionSnapshotNonce.Preparation? Nonce;
        internal F.ProjectileRecord[]? Records;
        internal F.Snapshot? LoadingSnapshot;
        internal bool ExpectedSavedData;
        internal FusionSaveStore.Result? Result;
        internal bool Completed;
        internal Scope(OwnerState owner, Operation operation, Board? board)
        {
            Owner = owner; Operation = operation; Board = board; Generation = owner.Generation;
            Previous = owner.Service.m_saveBuffer;
            PreviousSnapshots = new(owner.Snapshots);
        }
    }
    private static readonly Dictionary<IntPtr, OwnerState> Owners = new();
    private static readonly Dictionary<IntPtr, Scope> ServiceScopes = new();
    private static long nextGeneration;
    private static int unityThread;
    private static bool ready;
    private static readonly Func<bool>? DebugActive = OptionalModApi.Bind<Func<bool>>("DebugMod", "DebugMod.DebugMode", "get_Active");
    internal static bool Ready => ready && DebugActive?.Invoke() != true;
    internal static void Initialize()
    {
        unityThread = Environment.CurrentManagedThreadId;
        string assembly = Path.Combine(MelonLoader.Utils.MelonEnvironment.GameRootDirectory, "GameAssembly.dll");
        using var file = File.OpenRead(assembly); using var sha = SHA256.Create();
        if (Convert.ToHexString(sha.ComputeHash(file)) != "E7333EAF87996FA44B4A3526986272319B7D01F7ACDBCFF52451652F61CEB696")
            throw new NotSupportedException("Fusion save adapter requires the reviewed native Replanted build.");
        FusionSaveLifecycle.Initialize();
        FusionProjectileRecords.EnableAfterCoordinatorReady(); FusionTime.PersistenceBlocked = Blocks; FusionTime.PersistenceCapture = ClockBoundary;
        ready = true;
    }
    internal static void Dispose()
    { ready = false; FusionSaveLifecycle.Dispose(); FusionSaveRecoveryUi.Hide(); FusionTime.PersistenceBlocked = null; FusionTime.PersistenceCapture = null;
        FusionProjectileRecords.Disable(); Owners.Clear(); ServiceScopes.Clear(); }
    private static void RequireThread()
    {
        if (Environment.CurrentManagedThreadId != unityThread)
            throw new InvalidOperationException("Fusion board persistence requires the registered Unity thread.");
    }
    private static NativeBuffers Clone(NativeBuffers source)
    {
        var result = new NativeBuffers();
        for (int i = 0; i < source.Count; ++i) result.Add(new Il2CppStructArray<byte>(source[i].ToArray()));
        return result;
    }
    private static F.Owner Identity(SerializationService service)
    {
        string profile = service.m_userService?.ActiveUserProfile?.mGuid
            ?? throw new InvalidDataException("Fusion save profile is unavailable.");
        string fileName = service.GetSaveFilename(); var owner = new F.Owner(profile, fileName); owner.Validate(); return owner;
    }
    private static OwnerState Resolve(SerializationService service)
    {
        RequireThread(); var identity = Identity(service);
        if (Owners.TryGetValue(service.Pointer, out var existing) && existing.Identity == identity) return existing;
        var platform = service.m_platformService.TryCast<PlatformService>()?.Platform.TryCast<SteamPlatform>()
            ?? throw new NotSupportedException("Fusion transactional saves require the inspected Steam backend.");
        string target = platform._platformFilePath(identity.FileName);
        if (!Path.IsPathFullyQualified(target)) throw new InvalidDataException("Native Fusion board-file path is not absolute.");
        var owner = new OwnerState(service, identity, checked(++nextGeneration), Path.GetFullPath(target));
        Owners[service.Pointer] = owner;
        if (existing != null && service.App?.Board is { } live && FusionProjectileRecords.HasSession(live))
            Hold(owner, new InvalidDataException("Profile changed while the previous owner's board is alive. Leave this attempt before loading the new profile."));
        return owner;
    }
    private static bool StillOwns(OwnerState owner) => Owners.TryGetValue(owner.Service.Pointer, out var current)
        && ReferenceEquals(current, owner) && current.Generation == owner.Generation && Identity(owner.Service) == owner.Identity;
    private static byte[] ReadNative(OwnerState owner)
    {
        using var stream = new FileStream(owner.Target, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length < 0 || stream.Length > F.MaximumContainerBytes) throw new InvalidDataException("Native board file exceeds Fusion limits.");
        byte[] bytes = new byte[(int)stream.Length]; int at = 0;
        while (at < bytes.Length)
        {
            int read = stream.Read(bytes, at, bytes.Length - at);
            if (read == 0) throw new InvalidDataException("Native board file was truncated during Fusion preflight.");
            at += read;
        }
        if (stream.ReadByte() != -1 || stream.Length != bytes.Length) throw new IOException("Native board file changed during Fusion preflight.");
        return bytes;
    }
    internal static void Hold(OwnerState owner, Exception error, bool quarantine = false)
    {
        owner.Held = true; owner.Quarantined |= quarantine; owner.Error = error.Message;
        FusionMod.Instance.LoggerInstance.Error("Fusion save refused; preserved snapshots remain available: " + error);
        FusionSaveRecoveryUi.Show(owner);
    }
    internal static void Refuse(SerializationService service, Exception error)
    {
        try { Hold(Resolve(service), error); }
        catch { FusionMod.Instance.LoggerInstance.Error("Fusion board operation refused before native mutation: " + error); }
    }
    internal static bool Blocks(Board? board)
    {
        if (!Ready || board == null) return false;
        RequireThread();
        if (FusionSaveLifecycle.QuitPending(board)) return true;
        foreach (var owner in Owners.Values)
        {
            if (owner.Service.App?.Board?.Pointer != board.Pointer) continue;
            if (!FusionProjectileRecords.MatchesOwner(board, MapOwner(owner)))
            { Hold(owner, new InvalidDataException("This board's projectile map belongs to another Fusion save owner.")); return true; }
            if (owner.Held || owner.Quarantined || owner.Active != null) return true;
            if (owner.ReadValidated && owner.SessionEpoch == null)
            {
                try { owner.SessionEpoch = owner.Store.OpenSession(new F.Package(owner.Identity, owner.NativeAbsent, owner.NativeBytes!, owner.Snapshots.Values)); }
                catch (Exception error) { Hold(owner, error); return true; }
            }
        }
        return false;
    }
    private static Scope Start(OwnerState owner, Operation operation, Board? board)
    {
        if (owner.Active != null || ServiceScopes.ContainsKey(owner.Service.Pointer))
            throw new InvalidOperationException("Nested Fusion board transaction refused.");
        var scope = new Scope(owner, operation, board);
        var working = Clone(scope.Previous);
        owner.Active = scope; ServiceScopes.Add(owner.Service.Pointer, scope);
        try { owner.Service.m_saveBuffer = working; return scope; }
        catch { owner.Active = null; ServiceScopes.Remove(owner.Service.Pointer); throw; }
    }
    private static ReadOnlySpan<byte> Chunk(Il2CppStructArray<byte> buffer)
    {
        byte[] bytes = buffer.ToArray();
        if (bytes.Length < 20) throw new InvalidDataException("Native board category buffer is truncated.");
        int length = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(16));
        if (length < 20 || length > bytes.Length || length > F.MaximumChunkBytes)
            throw new InvalidDataException("Native board category length exceeds bounds.");
        return bytes.AsSpan(0, length).ToArray();
    }
    private static bool Marked(Il2CppStructArray<byte> buffer) => buffer.Length >= 20
        && BinaryPrimitives.ReadInt32LittleEndian(buffer.ToArray().AsSpan(4)) == F.RequiredVersion;
    private static void ValidatePublished(OwnerState owner)
    {
        var snapshots = new Dictionary<int, F.Snapshot>();
        for (int category = 0; category < owner.Service.m_saveBuffer.Count; ++category)
        {
            var buffer = owner.Service.m_saveBuffer[category];
            if (!Marked(buffer)) continue;
            byte[] chunk = Chunk(buffer).ToArray();
            if (BinaryPrimitives.ReadInt32LittleEndian(chunk) != SerializationService.SaveHeader.MAGIC_NUMBER)
                throw new InvalidDataException("Marked Fusion native magic is invalid.");
            var snapshot = owner.Store.ReadSnapshot(chunk);
            if (snapshot.Category != category || snapshot.Owner != owner.Identity)
                throw new InvalidDataException("Fusion companion is bound to a different native category.");
            snapshots.Add(category, snapshot);
        }
        if (!StillOwns(owner)) throw new InvalidDataException("Fusion save owner changed during validation.");
        owner.Snapshots.Clear(); foreach (var pair in snapshots) owner.Snapshots.Add(pair.Key, pair.Value);
    }
    internal static Scope? BeginRead(SerializationService service)
    {
        if (!Ready) return null;
        var owner = Resolve(service);
        if (owner.Active != null || ServiceScopes.ContainsKey(service.Pointer))
            throw new InvalidOperationException("Reentrant native Fusion reader refused before buffer cleanup.");
        _ = owner.Store.Recover();
        owner.ExpectedPackage = owner.Store.CurrentPublishedPackage();
        if (!owner.SessionsChecked)
        { owner.InterruptedSessions = owner.Store.UnacknowledgedSessions(); owner.SessionsChecked = true; }
        owner.NativeAbsent = !File.Exists(owner.Target);
        owner.NativeBytes = owner.NativeAbsent ? Array.Empty<byte>() : ReadNative(owner);
        if (owner.ExpectedPackage == null && owner.Store.HasPublishedHistory() && !owner.Store.HasCurrentBranchChoice())
        {
            owner.BranchChoiceRequired = true;
            throw new InvalidDataException("The native board file differs from the retained Fusion history. Both branches are preserved. Use current native save explicitly to import this branch, or return to the main menu.");
        }
        owner.BranchChoiceRequired = false;
        return Start(owner, Operation.Read, null);
    }
    internal static void CompleteRead(Scope scope)
    {
        var owner = scope.Owner;
        ValidatePublished(owner);
        if (owner.NativeAbsent != !File.Exists(owner.Target)
            || !owner.NativeAbsent && F.Digest(ReadNative(owner)) != F.Digest(owner.NativeBytes!))
            throw new IOException("Native board file changed during Fusion reading.");
        if (owner.ExpectedPackage != null)
        {
            foreach (var snapshot in owner.ExpectedPackage.Snapshots)
                if (!owner.Snapshots.TryGetValue(snapshot.Category, out var current) || current.Binding != snapshot.Binding)
                    throw new InvalidDataException("Native reader lost a required marked Fusion category.");
        }
        owner.Published = owner.Service.m_saveBuffer; owner.ReadValidated = true;
        scope.Completed = true;
    }
    internal static Scope? BeginSerialize(SerializationService service)
    {
        if (!Ready) return null;
        var owner = Resolve(service);
        owner.LastCommit = false;
        if (owner.Held || owner.Quarantined) throw new InvalidOperationException("The Fusion board is held for save recovery.");
        if (!owner.ReadValidated)
        {
            service._read();
            if (!owner.ReadValidated || owner.Held) throw new InvalidDataException("Fusion native reader did not publish a validated owner image.");
        }
        var board = service.App?.Board ?? throw new InvalidOperationException("Fusion board serialization has no board.");
        if (!FusionProjectileRecords.MatchesOwner(board, MapOwner(owner)))
            throw new InvalidDataException("Serialization cannot attach a previous profile's projectile map to this owner.");
        _ = FusionProjectileRecords.Generation(board);
        var scope = Start(owner, Operation.Serialize, board);
        try
        {
            scope.Nonce = FusionSnapshotNonce.Prepare(board);
            scope.Records = FusionProjectileRecords.Freeze(board);
            return scope;
        }
        catch (Exception error) { Finish(scope, error); throw; }
    }
    internal static void ObserveNonceWriter(GridItem item, int position)
    {
        if (!Ready || !FusionSnapshotNonce.Reserved(item)) return;
        RequireThread();
        var scope = ServiceScopes.Values.SingleOrDefault(value => value.Operation == Operation.Serialize
            && value.Nonce?.Item.Pointer == item.Pointer)
            ?? throw new InvalidOperationException("Nonce serialization outside its Fusion owner scope.");
        scope.Nonce!.Validate();
        if (scope.Nonce.WriterOffset != null) throw new InvalidDataException("Fusion nonce serialized more than once.");
        scope.Nonce.WriterOffset = checked(position + 20);
    }
    internal static void BeforeSave(SerializationService service)
    {
        if (!Ready) return;
        var owner = Resolve(service);
        if (owner.Held || owner.Quarantined) throw new InvalidOperationException("Held Fusion board cannot write its native save.");
        if (!ServiceScopes.TryGetValue(service.Pointer, out var scope) || !ReferenceEquals(scope.Owner, owner)
            || scope.Operation is not (Operation.Serialize or Operation.Clear))
            throw new InvalidOperationException("Native board writer lacks a Fusion transaction scope.");
        if (scope.Operation == Operation.Serialize)
        {
            scope.Nonce!.Validate();
            scope.Nonce.ValidateClock();
            int category = (int)service.GetCurrentSaveCategory(); var buffer = service.m_saveBuffer[category];
            byte[] chunk = Chunk(buffer).ToArray();
            BinaryPrimitives.WriteInt32LittleEndian(chunk.AsSpan(4), F.RequiredVersion);
            for (int i = 0; i < 4; ++i) buffer[i + 4] = chunk[i + 4];
            int offset = scope.Nonce.WriterOffset ?? throw new InvalidDataException("Fusion nonce writer invocation is missing.");
            int level = BinaryPrimitives.ReadInt32LittleEndian(chunk.AsSpan(8));
            var snapshot = new F.Snapshot(owner.Identity, category, level, scope.Nonce.Value, chunk, offset,
                chunk.AsSpan(offset, F.ScalarSize), scope.Records!);
            owner.Snapshots[category] = snapshot;
        }
        foreach (var pair in owner.Snapshots.ToArray())
        {
            if (!Marked(service.m_saveBuffer[pair.Key])) { owner.Snapshots.Remove(pair.Key); continue; }
            if (!Chunk(service.m_saveBuffer[pair.Key]).SequenceEqual(pair.Value.NativeChunk.Span))
                throw new InvalidDataException("A marked Fusion category changed without its matching provenance.");
        }
    }
    internal static bool CommitPlatformFile(PlatformService platform, string fileName, Il2CppStructArray<byte> data, out bool result)
    {
        result = false;
        if (!Ready) return false;
        RequireThread();
        var scope = ServiceScopes.Values.SingleOrDefault(value => value.Owner.Identity.FileName == fileName
            && value.Owner.Service.m_platformService.Pointer == platform.Pointer);
        if (scope == null)
        {
            var protectedOwner = Owners.Values.FirstOrDefault(value => value.Identity.FileName == fileName
                && value.Service.m_platformService.Pointer == platform.Pointer && (value.Held || value.Snapshots.Count != 0));
            if (protectedOwner == null) return false;
            Hold(protectedOwner, new InvalidOperationException("Unscoped write to a protected Fusion board file was refused.")); return true;
        }
        var owner = scope.Owner;
        if (owner.Held || owner.Quarantined) { scope.Result = new(FusionSaveStore.Outcome.Failed,
            new F.Package(owner.Identity, owner.NativeAbsent, owner.NativeBytes!, scope.PreviousSnapshots.Values), owner.Error); return true; }
        if (!StillOwns(owner) || scope.Operation is not (Operation.Serialize or Operation.Clear))
            throw new InvalidDataException("Fusion platform writer owner/scope changed.");
        var oldPackage = new F.Package(owner.Identity, owner.NativeAbsent,
            owner.NativeBytes ?? throw new InvalidDataException("Fusion previous native image is unavailable."), scope.PreviousSnapshots.Values);
        var candidate = new F.Package(owner.Identity, false, data.ToArray(), owner.Snapshots.Values);
        scope.Result = owner.Store.Commit(oldPackage, candidate);
        if (scope.Result.Outcome == FusionSaveStore.Outcome.Committed)
        {
            owner.NativeAbsent = false; owner.NativeBytes = candidate.Native.ToArray(); owner.Published = owner.Service.m_saveBuffer;
            owner.LastPackage = candidate.Digest;
            owner.LastCommit = true;
            scope.Completed = true; result = true;
        }
        else Hold(owner, new IOException(scope.Result.Error ?? "Fusion native transaction did not commit."));
        return true;
    }
    internal static void Finish(Scope? scope, Exception? error)
    {
        if (scope == null) return;
        var owner = scope.Owner;
        try
        {
            if (scope.Operation == Operation.Load && StillOwns(owner)) owner.Service.m_saveBuffer = scope.Previous;
            if (error != null || !scope.Completed || owner.Held)
            {
                if (StillOwns(owner)) owner.Service.m_saveBuffer = scope.Previous;
                owner.Snapshots.Clear(); foreach (var pair in scope.PreviousSnapshots) owner.Snapshots.Add(pair.Key, pair.Value);
                scope.Nonce?.Rollback();
                Hold(owner, error ?? new IOException("Fusion board operation did not complete."), scope.Operation == Operation.Load);
            }
        }
        finally
        {
            if (scope.Board != null && scope.Nonce != null) FusionProjectileRecords.Unfreeze(scope.Board, scope.Nonce.Generation);
            if (ReferenceEquals(owner.Active, scope)) owner.Active = null;
            if (ServiceScopes.TryGetValue(owner.Service.Pointer, out var current) && ReferenceEquals(current, scope))
                ServiceScopes.Remove(owner.Service.Pointer);
        }
    }
    internal static bool SuppressVisuals => Ready && Owners.Values.Any(owner => owner.Service.App?.Board != null
        && (owner.Held || owner.Quarantined || owner.Active != null || FusionSaveLifecycle.QuitPending(owner.Service.App.Board)));
    internal static bool IsCurrent(OwnerState owner)
    { try { return StillOwns(owner); } catch { return false; } }
    private static bool ClockBoundary(Board board, bool captured)
    {
        if (!Ready) return true;
        var scope = ServiceScopes.Values.SingleOrDefault(value => value.Operation == Operation.Serialize && value.Board?.Pointer == board.Pointer);
        try
        {
            if (scope?.Nonce == null) throw new InvalidOperationException("Direct Board.Serialize lacks its prepared Fusion clock scope.");
            if (!captured) throw new InvalidDataException("Fusion clock capture failed at the native Board.Serialize boundary.");
            scope.Nonce.ValidateClock(); return true;
        }
        catch (Exception error) { FailActivity(board.mApp, error); return false; }
    }
    internal static OwnerState OwnerFor(Il2CppReloaded.TreeStateActivities.GameplayActivity activity)
        => Resolve(activity.m_serializationService.TryCast<SerializationService>()
            ?? throw new InvalidDataException("Fusion gameplay serialization owner is unavailable."));
    internal static FusionProjectileRecords.OwnerBinding MapOwner(OwnerState owner)
        => new(owner.Identity, owner.Generation, owner.Service.Pointer);
    internal static string SelectedBinding(OwnerState owner)
    {
        int category = (int)owner.Service.GetCurrentSaveCategory();
        var level = owner.Service.m_gameplayService.CurrentLevelData
            ?? throw new InvalidDataException("Fusion selected level is unavailable.");
        if (!owner.Service.LevelDataHasSave(level)) return $"EMPTY:{category}:{level.Pointer}";
        if (owner.Snapshots.TryGetValue(category, out var snapshot)) return snapshot.Binding;
        return $"LEGACY:{category}:" + F.Digest(Chunk(owner.Service.m_saveBuffer[category]));
    }
    internal static bool Preflight(Il2CppReloaded.TreeStateActivities.GameplayActivity activity)
    {
        if (!Ready) return true;
        var service = activity.m_serializationService.TryCast<SerializationService>()
            ?? throw new InvalidDataException("Fusion gameplay serialization owner is unavailable.");
        var owner = Resolve(service);
        if (!owner.ReadValidated) service._read();
        if (owner.Held || owner.Quarantined || !owner.ReadValidated) throw new InvalidDataException(owner.Error ?? "Fusion reader is not ready.");
        ValidatePublished(owner);
        if (owner.InterruptedSessions.Length != 0 && !owner.AcknowledgmentPending)
            throw new InvalidDataException("The previous session ended without a confirmed current save. Retry acknowledges this notice and loads the preserved board.");
        if (owner.SessionEpoch == null)
            owner.SessionEpoch = owner.Store.OpenSession(new F.Package(owner.Identity, owner.NativeAbsent, owner.NativeBytes!, owner.Snapshots.Values));
        return true;
    }
    internal static Scope? BeginLoad(SerializationService service)
    {
        if (!Ready) return null;
        var owner = Resolve(service);
        if (!owner.ReadValidated || owner.Held || owner.Quarantined) throw new InvalidOperationException("Fusion marked load has no valid entry preflight.");
        FusionSaveLifecycle.ConsumeLoadAuthorization(service, owner, SelectedBinding(owner));
        int category = (int)service.GetCurrentSaveCategory(); var board = service.App?.Board
            ?? throw new InvalidOperationException("Fusion load board is unavailable.");
        var scope = Start(owner, Operation.Load, board);
        scope.ExpectedSavedData = service.LevelDataHasSave(service.m_gameplayService.CurrentLevelData);
        try
        {
            if (scope.ExpectedSavedData && owner.Snapshots.TryGetValue(category, out var snapshot))
            {
                scope.LoadingSnapshot = snapshot;
                if (!Chunk(scope.Previous[category]).SequenceEqual(snapshot.NativeChunk.Span))
                    throw new InvalidDataException("Fusion load buffer changed after preflight.");
                FusionProjectileRecords.BeginLoad(board, MapOwner(owner), snapshot.Records);
                var buffer = service.m_saveBuffer[category];
                Span<byte> version = stackalloc byte[4]; BinaryPrimitives.WriteInt32LittleEndian(version, F.NativeVersion);
                for (int i = 0; i < 4; ++i) buffer[i + 4] = version[i];
            }
            return scope;
        }
        catch (Exception error) { Finish(scope, error); throw; }
    }
    internal static void CompleteLoad(Scope scope, bool success)
    {
        if (success)
        {
            if (scope.LoadingSnapshot != null)
            {
                FusionSnapshotNonce.VerifyLoaded(scope.Board!, scope.LoadingSnapshot.Nonce);
                FusionProjectileRecords.CompleteLoad(scope.Board!);
            }
            else FusionProjectileRecords.ReloadLegacy(scope.Board!);
            scope.Completed = true; return;
        }
        // No native category is a normal new game. A valid marked/native chunk
        // that failed to deserialize is quarantined, not reclassified as empty.
        int category = (int)scope.Owner.Service.GetCurrentSaveCategory();
        byte[] header = scope.Previous[category].ToArray();
        bool existed = header.Length >= 20 && BinaryPrimitives.ReadInt32LittleEndian(header) == SerializationService.SaveHeader.MAGIC_NUMBER;
        if ((!existed || !scope.ExpectedSavedData) && scope.LoadingSnapshot == null) { scope.Completed = true; return; }
        Hold(scope.Owner, new InvalidDataException("Native board loading failed; the existing save has been preserved."), true);
    }
    internal static Scope? BeginClear(SerializationService service)
    {
        if (!Ready) return null;
        var owner = Resolve(service);
        if (owner.Held || owner.Quarantined || owner.Active != null) throw new InvalidOperationException("Fusion recovery/load state blocks native board clearing.");
        if (!owner.ReadValidated) service._read();
        if (!owner.ReadValidated || owner.Held) throw new InvalidDataException("Fusion clear lacks a validated previous native image.");
        return Start(owner, Operation.Clear, null);
    }
    internal static bool PermitLeafClear(SerializationService service)
    {
        if (!Ready) return true;
        if (!Owners.TryGetValue(service.Pointer, out var owner)) return true; // constructor initialization
        return owner.Active?.Operation is Operation.Read or Operation.Clear or Operation.Serialize;
    }
    internal static bool CommitDelete(PlatformService platform, string fileName, out bool result)
    {
        result = false;
        if (!Ready) return false;
        var scope = ServiceScopes.Values.SingleOrDefault(value => value.Owner.Identity.FileName == fileName
            && value.Owner.Service.m_platformService.Pointer == platform.Pointer);
        if (scope == null)
        {
            var protectedOwner = Owners.Values.FirstOrDefault(value => value.Identity.FileName == fileName
                && value.Service.m_platformService.Pointer == platform.Pointer && (value.Held || value.Snapshots.Count != 0));
            if (protectedOwner == null) return false;
            Hold(protectedOwner, new InvalidOperationException("Unscoped native deletion of a protected Fusion board was refused.")); return true;
        }
        var owner = scope.Owner;
        if (scope.Operation != Operation.Clear || owner.Held || owner.Quarantined)
        { Hold(owner, new InvalidOperationException("Native deletion during Fusion loading/recovery was refused.")); return true; }
        var oldPackage = new F.Package(owner.Identity, owner.NativeAbsent, owner.NativeBytes!, scope.PreviousSnapshots.Values);
        var candidate = new F.Package(owner.Identity, true, Array.Empty<byte>(), Array.Empty<F.Snapshot>());
        scope.Result = owner.Store.Commit(oldPackage, candidate);
        if (scope.Result.Outcome != FusionSaveStore.Outcome.Committed)
        { Hold(owner, new IOException(scope.Result.Error)); return true; }
        owner.NativeAbsent = true; owner.NativeBytes = Array.Empty<byte>(); owner.Snapshots.Clear();
        owner.Published = owner.Service.m_saveBuffer; owner.LastPackage = candidate.Digest;
        scope.Completed = true; result = true; return true;
    }
    internal static void Retry(OwnerState owner)
    {
        RequireThread();
        if (owner.BranchChoiceRequired) owner.Store.RecordCurrentBranchChoice();
        owner.AcknowledgmentPending = true; owner.Held = owner.Quarantined = false;
        owner.ReadValidated = false; owner.Service._read();
        if (owner.Held || !owner.ReadValidated) return;
        owner.Error = null; FusionSaveRecoveryUi.Hide();
    }
    internal static void CompleteAcknowledgment(OwnerState owner)
    {
        if (!owner.AcknowledgmentPending || owner.Held || owner.Quarantined || !owner.ReadValidated || !IsCurrent(owner)) return;
        foreach (string epoch in owner.InterruptedSessions) owner.Store.AcknowledgeSession(epoch);
        owner.InterruptedSessions = Array.Empty<string>(); owner.AcknowledgmentPending = false;
    }
    internal static bool ProfileDeletion(SerializationService service, string profileGuid)
    {
        if (!Ready) return true;
        string fileName = service.GetSaveFilename(profileGuid);
        var identity = new F.Owner(profileGuid, fileName);
        var platform = service.m_platformService.TryCast<PlatformService>()?.Platform.TryCast<SteamPlatform>()
            ?? throw new NotSupportedException("Fusion profile deletion requires the inspected Steam backend.");
        var passive = new OwnerState(service, identity, checked(++nextGeneration), Path.GetFullPath(platform._platformFilePath(fileName)));
        if (!passive.Store.HasPublishedHistory()) return true;
        var active = Owners.TryGetValue(service.Pointer, out var current) && current.Identity == identity ? current : null;
        if (ServiceScopes.ContainsKey(service.Pointer) || active is { Held: true } || active is { Quarantined: true })
            throw new InvalidOperationException("Profile board-file deletion is blocked while its Fusion transaction/recovery is unresolved.");
        _ = passive.Store.Recover();
        var previous = passive.Store.CurrentPublishedPackage()
            ?? throw new InvalidDataException("Profile board-file deletion requires an explicit recovery choice for its conflicting native branch.");
        var absent = new F.Package(identity, true, Array.Empty<byte>(), Array.Empty<F.Snapshot>());
        var result = passive.Store.Commit(previous, absent);
        if (result.Outcome != FusionSaveStore.Outcome.Committed) throw new IOException(result.Error ?? "Fusion profile board-file deletion did not commit.");
        if (active != null)
        {
            active.NativeAbsent = true; active.NativeBytes = Array.Empty<byte>(); active.Snapshots.Clear();
            for (int i = 0; i < service.m_saveBuffer.Count; ++i)
                for (int at = 0; at < Math.Min(20, service.m_saveBuffer[i].Length); ++at) service.m_saveBuffer[i][at] = 0;
            active.Published = service.m_saveBuffer;
        }
        return false;
    }
    internal static OwnerState? ActiveOwner() => Ready ? Owners.Values.FirstOrDefault(owner => owner.Service.App?.Board != null) : null;
    internal static bool LastSaveCommitted(Il2CppReloaded.TreeStateActivities.GameplayActivity activity)
        => Owners.Values.Any(owner => owner.Service.App?.Pointer == activity.Pointer && owner.LastCommit && !owner.Held);
    internal static void FailActivity(Il2CppReloaded.TreeStateActivities.GameplayActivity activity, Exception error)
    {
        var service = activity.m_serializationService.TryCast<SerializationService>();
        if (service != null) Refuse(service, error); else FusionMod.Instance.LoggerInstance.Error(error.ToString());
    }
    internal static void FailTransition(Il2CppTekly.TreeState.TreeStateManager manager,
        Il2CppSystem.Collections.Generic.List<Il2CppTekly.TreeState.TreeState> states, Exception error)
    {
        for (int i = 0; i < states.Count; ++i)
            for (int j = 0; j < states[i].m_activities.Count; ++j)
            {
                var activity = states[i].m_activities[j].TryCast<Il2CppReloaded.TreeStateActivities.GameplayActivity>();
                if (activity != null) { FailActivity(activity, error); return; }
            }
        if (ActiveOwner() is { } owner) Hold(owner, error); else FusionMod.Instance.LoggerInstance.Error(error.ToString());
    }
}

[HarmonyPatch(typeof(SerializationService), "_read")]
internal static class FusionSaveReadPatch
{
    [HarmonyPriority(Priority.First + 2000)]
    private static bool Prefix(SerializationService __instance, out FusionSaveCoordinator.Scope? __state)
    {
        __state = null;
        if (!FusionSaveCoordinator.Ready) return true;
        try { __state = FusionSaveCoordinator.BeginRead(__instance); return true; }
        catch (Exception error) { FusionSaveCoordinator.Refuse(__instance, error); return false; }
    }
    private static void Postfix(FusionSaveCoordinator.Scope? __state)
    {
        if (__state == null) return;
        try { FusionSaveCoordinator.CompleteRead(__state); }
        catch (Exception error) { FusionSaveCoordinator.Refuse(__state.Owner.Service, error); }
    }
    private static void Finalizer(FusionSaveCoordinator.Scope? __state, Exception? __exception)
        => FusionSaveCoordinator.Finish(__state, __exception);
}
[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Serialize))]
internal static class FusionSaveSerializePatch
{
    [HarmonyPriority(Priority.First + 2000)]
    private static bool Prefix(SerializationService __instance, out FusionSaveCoordinator.Scope? __state)
    {
        __state = null;
        try { __state = FusionSaveCoordinator.BeginSerialize(__instance); return true; }
        catch (Exception error) { FusionSaveCoordinator.Refuse(__instance, error); return false; }
    }
    private static void Finalizer(FusionSaveCoordinator.Scope? __state, Exception? __exception)
        => FusionSaveCoordinator.Finish(__state, __exception);
}
[HarmonyPatch(typeof(SerializationService), "_save")]
internal static class FusionSaveWriterPatch
{
    [HarmonyPriority(Priority.First + 2000)] private static bool Prefix(SerializationService __instance)
    {
        try { FusionSaveCoordinator.BeforeSave(__instance); return true; }
        catch (Exception error) { FusionSaveCoordinator.Refuse(__instance, error); return false; }
    }
}
[HarmonyPatch(typeof(PlatformService), nameof(PlatformService.SaveFile))]
internal static class FusionSavePlatformPatch
{
    [HarmonyPriority(Priority.First + 2000)]
    private static bool Prefix(PlatformService __instance, string fileName, Il2CppStructArray<byte> data, ref bool __result)
    {
        try
        {
            if (!FusionSaveCoordinator.CommitPlatformFile(__instance, fileName, data, out bool result)) return true;
            __result = result; return false;
        }
        catch (Exception error)
        {
            if (FusionSaveCoordinator.ActiveOwner() is { } owner) FusionSaveCoordinator.Hold(owner, error);
            __result = false; return false;
        }
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Serialize))]
internal static class FusionSaveNonceWriterPatch
{
    private static void Prefix(GridItem __instance, ref Il2CppSource.Serialization.ReloadedBinaryWriter writer)
    {
        try { FusionSaveCoordinator.ObserveNonceWriter(__instance, writer.Position); }
        catch (Exception error) { if (FusionSaveCoordinator.ActiveOwner() is { } owner) FusionSaveCoordinator.Hold(owner, error); }
    }
}
[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class FusionSaveBoardHoldPatch
{
    [HarmonyPriority(Priority.First + 5000)]
    private static bool Prefix(Board __instance) => !FusionSaveCoordinator.Blocks(__instance);
}

[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Deserialize))]
internal static class FusionSaveLoadPatch
{
    [HarmonyPriority(Priority.First + 3000)]
    private static bool Prefix(SerializationService __instance, out FusionSaveCoordinator.Scope? __state, ref bool __result)
    {
        __state = null;
        try { __state = FusionSaveCoordinator.BeginLoad(__instance); return true; }
        catch (Exception error) { FusionSaveCoordinator.Refuse(__instance, error); __result = false; return false; }
    }
    [HarmonyPriority(Priority.First)]
    private static void Postfix(FusionSaveCoordinator.Scope? __state, bool __result)
    {
        if (__state == null) return;
        try { FusionSaveCoordinator.CompleteLoad(__state, __result); }
        catch (Exception error) { FusionSaveCoordinator.Hold(__state.Owner, error, true); }
    }
    private static void Finalizer(FusionSaveCoordinator.Scope? __state, Exception? __exception)
        => FusionSaveCoordinator.Finish(__state, __exception);
}
[HarmonyPatch]
internal static class FusionSaveClearPatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => new[] { "Clear", "ClearCurrent", "ClearAll", "DeleteSaveFile" }
        .Select(name => AccessTools.Method(typeof(SerializationService), name));
    [HarmonyPriority(Priority.First + 3000)]
    private static bool Prefix(SerializationService __instance, out FusionSaveCoordinator.Scope? __state)
    {
        __state = null;
        try { __state = FusionSaveCoordinator.BeginClear(__instance); return true; }
        catch (Exception error) { FusionSaveCoordinator.Refuse(__instance, error); return false; }
    }
    private static void Finalizer(FusionSaveCoordinator.Scope? __state, Exception? __exception)
        => FusionSaveCoordinator.Finish(__state, __exception);
}
[HarmonyPatch]
internal static class FusionSaveLeafClearPatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => new[] { "ClearBuffer", "ClearBuffers", "HandleVasebreakerSlotUpgrade", "HandleSurvivalEndlessSlotUpgrade" }
        .Select(name => AccessTools.Method(typeof(SerializationService), name));
    [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(SerializationService __instance)
        => FusionSaveCoordinator.PermitLeafClear(__instance);
}
[HarmonyPatch(typeof(PlatformService), nameof(PlatformService.DeleteFile))]
internal static class FusionSavePlatformDeletePatch
{
    [HarmonyPriority(Priority.First + 3000)]
    private static bool Prefix(PlatformService __instance, string fileName, ref bool __result)
    {
        try
        {
            if (!FusionSaveCoordinator.CommitDelete(__instance, fileName, out bool result)) return true;
            __result = result; return false;
        }
        catch (Exception error) { FusionMod.Instance.LoggerInstance.Error(error.ToString()); __result = false; return false; }
    }
}
[HarmonyPatch(typeof(SerializationService), "OnUserProfileDeleted")]
internal static class FusionSaveProfileDeletedPatch
{
    [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(SerializationService __instance, string profileGuid)
    {
        try { return FusionSaveCoordinator.ProfileDeletion(__instance, profileGuid); }
        catch (Exception error) { FusionSaveCoordinator.Refuse(__instance, error); return false; }
    }
}
