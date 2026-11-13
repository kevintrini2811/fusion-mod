using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace FusionMod;

// Pure managed format code. Native hooks must supply an already finalized,
// immutable board image; decoding never initializes a native board or profile.
internal static class FusionSaveFormat
{
    internal const int NativeVersion = 14, RequiredVersion = 0x4655010e;
    internal const int NonceMarker = -521095, NonceSchema = 1, ScalarSize = 136;
    internal const int MaximumChunkBytes = 10 * 1024 * 1024;
    internal const int MaximumContainerBytes = 128 * 1024 * 1024;
    internal const int MaximumProjectileRecords = 8192, MaximumChunks = 64;
    private const uint CompanionMagic = 0x31504346, PackageMagic = 0x31504b46;
    private const int Schema = 1;

    internal readonly record struct Owner(string Profile, string FileName)
    {
        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(Profile) || string.IsNullOrWhiteSpace(FileName)
                || Encoding.UTF8.GetByteCount(Profile) > 1024 || Encoding.UTF8.GetByteCount(FileName) > 4096)
                throw new InvalidDataException("Invalid Fusion save owner.");
        }
    }

    internal readonly record struct Nonce(uint A, uint B, uint C, uint D)
    {
        internal bool IsZero => (A | B | C | D) == 0;
        internal static Nonce Create()
        {
            Span<byte> bytes = stackalloc byte[16];
            Nonce result;
            do { RandomNumberGenerator.Fill(bytes); result = Read(bytes); } while (result.IsZero);
            return result;
        }
        internal static Nonce Read(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != 16) throw new InvalidDataException("Invalid Fusion snapshot nonce length.");
            return new(BinaryPrimitives.ReadUInt32LittleEndian(bytes), BinaryPrimitives.ReadUInt32LittleEndian(bytes[4..]),
                BinaryPrimitives.ReadUInt32LittleEndian(bytes[8..]), BinaryPrimitives.ReadUInt32LittleEndian(bytes[12..]));
        }
        internal byte[] Bytes()
        {
            var bytes = new byte[16];
            BinaryPrimitives.WriteUInt32LittleEndian(bytes, A); BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(4), B);
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(8), C); BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(12), D);
            return bytes;
        }
        internal (int X, int Y, float PosX, float PosY, float GoalX, float GoalY) Fields()
            => (unchecked((int)A), unchecked((int)B), C & 65535, C >> 16, D & 65535, D >> 16);
        internal static Nonce FromFields(int x, int y, float a, float b, float c, float d)
        {
            static uint Limb(float value)
            {
                if (!float.IsFinite(value) || value < 0 || value > 65535 || value != MathF.Truncate(value)
                    || BitConverter.SingleToInt32Bits(value) == int.MinValue)
                    throw new InvalidDataException("Invalid Fusion snapshot nonce limb.");
                return (uint)value;
            }
            var result = new Nonce(unchecked((uint)x), unchecked((uint)y), Limb(a) | Limb(b) << 16, Limb(c) | Limb(d) << 16);
            if (result.IsZero) throw new InvalidDataException("Fusion snapshot nonce is zero.");
            return result;
        }
    }

    // Float bits are part of the payload; no JSON number conversion or age
    // reconstruction can alter them between serialization and recovery.
    internal readonly record struct ProjectileRecord(uint Id, int Rate, int Flags, int ABits, int BBits)
    {
        internal float A => BitConverter.Int32BitsToSingle(ABits);
        internal float B => BitConverter.Int32BitsToSingle(BBits);
        internal void Validate()
        {
            if (Id == 0 || Rate is not (100 or 120) || (Flags & ~63) != 0
                || !float.IsFinite(A) || !float.IsFinite(B) || A < 0 || B < 0)
                throw new InvalidDataException("Invalid Fusion projectile provenance.");
        }
    }

    internal sealed class Snapshot
    {
        internal readonly Owner Owner;
        internal readonly int Category, LevelId, ScalarOffset;
        internal readonly Nonce Nonce;
        internal readonly string NativeDigest, PayloadDigest;
        private readonly byte[] chunk, scalar, encoded;
        private readonly ProjectileRecord[] records;
        internal ReadOnlyMemory<byte> NativeChunk => chunk;
        internal ReadOnlyMemory<byte> Scalar => scalar;
        internal ReadOnlyMemory<byte> Encoded => encoded;
        internal IReadOnlyList<ProjectileRecord> Records => Array.AsReadOnly(records);
        internal string Binding => $"{Category}:{LevelId}:{Convert.ToHexString(Nonce.Bytes())}:{chunk.Length}:{NativeDigest}:{PayloadDigest}";

        internal Snapshot(Owner owner, int category, int levelId, Nonce nonce, ReadOnlySpan<byte> nativeChunk,
            int scalarOffset, ReadOnlySpan<byte> scalarImage, IEnumerable<ProjectileRecord> projectileRecords)
        {
            owner.Validate();
            if (category < 0 || category >= MaximumChunks || nonce.IsZero || nativeChunk.Length < 20
                || nativeChunk.Length > MaximumChunkBytes || scalarImage.Length != ScalarSize
                || scalarOffset < 20 || scalarOffset > nativeChunk.Length - ScalarSize
                || !nativeChunk.Slice(scalarOffset, ScalarSize).SequenceEqual(scalarImage))
                throw new InvalidDataException("Invalid Fusion native snapshot image.");
            ValidateHeader(nativeChunk, levelId);
            ValidateScalar(scalarImage, nonce);
            Owner = owner; Category = category; LevelId = levelId; Nonce = nonce; ScalarOffset = scalarOffset;
            chunk = nativeChunk.ToArray(); scalar = scalarImage.ToArray(); NativeDigest = Digest(chunk);
            records = projectileRecords.Take(MaximumProjectileRecords + 1).OrderBy(item => item.Id).ToArray();
            if (records.Length > MaximumProjectileRecords) throw new InvalidDataException("Too many Fusion projectile records.");
            uint previous = 0;
            foreach (var record in records)
            {
                record.Validate();
                if (record.Id == previous) throw new InvalidDataException("Duplicate Fusion projectile DataID.");
                previous = record.Id;
            }
            encoded = EncodeSnapshot(this);
            PayloadDigest = Digest(encoded);
        }
    }

    internal sealed class Package
    {
        internal readonly Owner Owner;
        internal readonly bool NativeAbsent;
        internal readonly string NativeDigest, Digest;
        private readonly byte[] native, encoded;
        private readonly Snapshot[] snapshots;
        internal ReadOnlyMemory<byte> Native => native;
        internal ReadOnlyMemory<byte> Encoded => encoded;
        internal IReadOnlyList<Snapshot> Snapshots => Array.AsReadOnly(snapshots);
        internal Package(Owner owner, bool absent, ReadOnlySpan<byte> nativeBytes, IEnumerable<Snapshot> requiredSnapshots)
        {
            owner.Validate();
            if (nativeBytes.Length > MaximumContainerBytes || absent && !nativeBytes.IsEmpty)
                throw new InvalidDataException("Invalid Fusion recovery container.");
            Owner = owner; NativeAbsent = absent; native = nativeBytes.ToArray(); NativeDigest = FusionSaveFormat.Digest(native);
            snapshots = requiredSnapshots.Take(MaximumChunks + 1).OrderBy(item => item.Category).ToArray();
            if (snapshots.Length > MaximumChunks || absent && snapshots.Length != 0)
                throw new InvalidDataException("Invalid Fusion recovery snapshots.");
            int previous = -1;
            foreach (var snapshot in snapshots)
            {
                if (snapshot.Owner != owner || snapshot.Category == previous)
                    throw new InvalidDataException("Conflicting Fusion recovery owner/category.");
                previous = snapshot.Category;
            }
            encoded = EncodePackage(this); Digest = FusionSaveFormat.Digest(encoded);
        }
    }

    internal static string Digest(ReadOnlySpan<byte> bytes) => Convert.ToHexString(SHA256.HashData(bytes));

    internal static void ValidateHeader(ReadOnlySpan<byte> chunk, int expectedLevel)
    {
        // Magic is owned by the native build and checked by the bridge against
        // SaveHeader.MAGIC_NUMBER. This pure codec never invents a magic value.
        if (chunk.Length < 20 || BinaryPrimitives.ReadInt32LittleEndian(chunk[4..]) != RequiredVersion
            || BinaryPrimitives.ReadInt32LittleEndian(chunk[8..]) != expectedLevel
            || BinaryPrimitives.ReadInt32LittleEndian(chunk[12..]) != chunk.Length - 20
            || BinaryPrimitives.ReadInt32LittleEndian(chunk[16..]) != chunk.Length)
            throw new InvalidDataException("Invalid marked Fusion native header.");
    }

    internal static void ValidateScalar(ReadOnlySpan<byte> scalar, Nonce expected)
    {
        if (scalar.Length != ScalarSize) throw new InvalidDataException("Invalid Fusion nonce scalar length.");
        // Local functions cannot capture a ReadOnlySpan; retain one bounded copy.
        var scalarCopy = scalar.ToArray();
        int I(int at) => BinaryPrimitives.ReadInt32LittleEndian(scalarCopy.AsSpan(at, 4));
        if (I(0) != BitConverter.SingleToInt32Bits(-1000f) || I(4) != BitConverter.SingleToInt32Bits(-1000f)
            || scalar[16] != 0 || I(70) != 9 || I(74) != 0 || I(86) != NonceSchema
            || scalar[90] != 0 || I(132) != NonceMarker)
            throw new InvalidDataException("Invalid Fusion nonce scalar contract.");
        var nonce = Nonce.FromFields(I(78), I(82), BitConverter.Int32BitsToSingle(I(91)),
            BitConverter.Int32BitsToSingle(I(95)), BitConverter.Int32BitsToSingle(I(99)), BitConverter.Int32BitsToSingle(I(103)));
        if (nonce != expected) throw new InvalidDataException("Fusion snapshot nonce does not match its companion.");
    }

    private static byte[] EncodeSnapshot(Snapshot snapshot)
    {
        using var stream = new MemoryStream(); using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
        writer.Write(CompanionMagic); writer.Write(Schema); WriteOwner(writer, snapshot.Owner);
        writer.Write(snapshot.Category); writer.Write(snapshot.LevelId); writer.Write(snapshot.Nonce.Bytes());
        writer.Write(snapshot.NativeChunk.Length); writer.Write(Convert.FromHexString(snapshot.NativeDigest));
        writer.Write(snapshot.ScalarOffset); writer.Write(snapshot.Scalar.Span); writer.Write(snapshot.Records.Count);
        foreach (var record in snapshot.Records)
        { writer.Write(record.Id); writer.Write(record.Rate); writer.Write(record.Flags); writer.Write(record.ABits); writer.Write(record.BBits); }
        writer.Flush(); return stream.ToArray();
    }

    internal static Snapshot DecodeSnapshot(ReadOnlySpan<byte> encoded, ReadOnlySpan<byte> nativeChunk, Owner owner)
    {
        // Bound before copying/allocating and before trusting a record count.
        if (encoded.Length > MaximumProjectileRecords * 20 + 8192) throw new InvalidDataException("Fusion companion exceeds bounds.");
        var reader = new Reader(encoded);
        if (reader.UInt() != CompanionMagic || reader.Int() != Schema || reader.Owner() != owner)
            throw new InvalidDataException("Unknown Fusion companion format or owner.");
        int category = reader.Int(), level = reader.Int(); var nonce = Nonce.Read(reader.Bytes(16));
        int length = reader.Int(); var digest = reader.Bytes(32).ToArray();
        if (length != nativeChunk.Length || !SHA256.HashData(nativeChunk).AsSpan().SequenceEqual(digest))
            throw new InvalidDataException("Fusion companion does not match native bytes.");
        int offset = reader.Int(); var scalar = reader.Bytes(ScalarSize).ToArray();
        int count = reader.Int();
        if (count < 0 || count > MaximumProjectileRecords || reader.Remaining != count * 20)
            throw new InvalidDataException("Invalid Fusion companion record count.");
        var records = new ProjectileRecord[count]; uint previous = 0;
        for (int i = 0; i < count; ++i)
        {
            records[i] = new(reader.UInt(), reader.Int(), reader.Int(), reader.Int(), reader.Int());
            if (records[i].Id <= previous) throw new InvalidDataException("Unsorted or duplicate Fusion projectile DataID.");
            previous = records[i].Id;
        }
        reader.End();
        var snapshot = new Snapshot(owner, category, level, nonce, nativeChunk, offset, scalar, records);
        if (!snapshot.Encoded.Span.SequenceEqual(encoded)) throw new InvalidDataException("Noncanonical Fusion companion.");
        return snapshot;
    }

    private static byte[] EncodePackage(Package package)
    {
        using var stream = new MemoryStream(); using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
        writer.Write(PackageMagic); writer.Write(Schema); WriteOwner(writer, package.Owner);
        writer.Write(package.NativeAbsent); writer.Write(package.Native.Length); writer.Write(package.Native.Span);
        writer.Write(package.Snapshots.Count);
        foreach (var snapshot in package.Snapshots)
        {
            writer.Write(snapshot.NativeChunk.Length); writer.Write(snapshot.NativeChunk.Span);
            writer.Write(snapshot.Encoded.Length); writer.Write(snapshot.Encoded.Span);
        }
        writer.Flush(); return stream.ToArray();
    }

    // Total package limits include the compressed primary and full uncompressed
    // marked chunks. Keeping both is deliberate: recovery needs no native code.
    internal const int MaximumPackageBytes = MaximumContainerBytes + MaximumChunks * MaximumChunkBytes + 16 * 1024 * 1024;
    internal static Package DecodePackage(ReadOnlySpan<byte> encoded, Owner owner)
    {
        if (encoded.Length > MaximumPackageBytes) throw new InvalidDataException("Fusion recovery package exceeds bounds.");
        var reader = new Reader(encoded);
        if (reader.UInt() != PackageMagic || reader.Int() != Schema || reader.Owner() != owner)
            throw new InvalidDataException("Unknown Fusion recovery package or owner.");
        byte absentValue = reader.Byte();
        if (absentValue > 1) throw new InvalidDataException("Invalid Fusion absence marker.");
        int nativeLength = reader.Length(MaximumContainerBytes); var native = reader.Bytes(nativeLength).ToArray();
        int count = reader.Int();
        if (count < 0 || count > MaximumChunks) throw new InvalidDataException("Invalid Fusion recovery snapshot count.");
        var snapshots = new List<Snapshot>(); int previous = -1;
        for (int i = 0; i < count; ++i)
        {
            int chunkLength = reader.Length(MaximumChunkBytes); var chunk = reader.Bytes(chunkLength).ToArray();
            int companionLength = reader.Length(MaximumProjectileRecords * 20 + 8192);
            var snapshot = DecodeSnapshot(reader.Bytes(companionLength), chunk, owner);
            if (snapshot.Category <= previous) throw new InvalidDataException("Unsorted Fusion recovery categories.");
            previous = snapshot.Category; snapshots.Add(snapshot);
        }
        reader.End();
        var package = new Package(owner, absentValue == 1, native, snapshots);
        if (!package.Encoded.Span.SequenceEqual(encoded)) throw new InvalidDataException("Noncanonical Fusion recovery package.");
        return package;
    }

    private static void WriteOwner(BinaryWriter writer, Owner owner)
    { WriteText(writer, owner.Profile); WriteText(writer, owner.FileName); }
    private static void WriteText(BinaryWriter writer, string value)
    { byte[] bytes = Encoding.UTF8.GetBytes(value); writer.Write(bytes.Length); writer.Write(bytes); }

    private ref struct Reader
    {
        private ReadOnlySpan<byte> remaining;
        internal Reader(ReadOnlySpan<byte> bytes) => remaining = bytes;
        internal int Remaining => remaining.Length;
        internal ReadOnlySpan<byte> Bytes(int length)
        {
            if (length < 0 || length > remaining.Length) throw new InvalidDataException("Truncated Fusion save metadata.");
            var result = remaining[..length]; remaining = remaining[length..]; return result;
        }
        internal byte Byte() => Bytes(1)[0];
        internal int Int() => BinaryPrimitives.ReadInt32LittleEndian(Bytes(4));
        internal uint UInt() => BinaryPrimitives.ReadUInt32LittleEndian(Bytes(4));
        internal int Length(int maximum)
        { int length = Int(); return length >= 0 && length <= maximum ? length : throw new InvalidDataException("Fusion metadata length exceeds bounds."); }
        internal Owner Owner()
        {
            var utf8 = new UTF8Encoding(false, true);
            var owner = new Owner(utf8.GetString(Bytes(Length(1024))), utf8.GetString(Bytes(Length(4096))));
            owner.Validate(); return owner;
        }
        internal void End() { if (!remaining.IsEmpty) throw new InvalidDataException("Trailing Fusion metadata bytes."); }
    }
}
