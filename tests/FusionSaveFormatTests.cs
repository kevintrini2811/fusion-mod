using System.Buffers.Binary;
using FusionMod;
using F = FusionMod.FusionSaveFormat;

static class FusionSaveFormatTests
{
    private static int checks;
    private static void Check(bool value, string name)
    { if (!value) throw new Exception(name); ++checks; }
    private static void Refuse(Action action, string name)
    {
        try { action(); } catch (InvalidDataException) { ++checks; return; }
        throw new Exception(name);
    }
    private static byte[] Image(F.Nonce nonce)
    {
        var result = new byte[F.ScalarSize]; var values = nonce.Fields();
        void I(int offset, int value) => BinaryPrimitives.WriteInt32LittleEndian(result.AsSpan(offset, 4), value);
        void Float(int offset, float value) => I(offset, BitConverter.SingleToInt32Bits(value));
        Float(0, -1000); Float(4, -1000); I(70, 9); I(74, 0); I(78, values.X); I(82, values.Y);
        I(86, F.NonceSchema); Float(91, values.PosX); Float(95, values.PosY); Float(99, values.GoalX); Float(103, values.GoalY);
        I(132, F.NonceMarker); return result;
    }
    private static byte[] Chunk(F.Nonce nonce, int level = 42)
    {
        var result = new byte[20 + F.ScalarSize + 32];
        BinaryPrimitives.WriteInt32LittleEndian(result.AsSpan(4), F.RequiredVersion);
        BinaryPrimitives.WriteInt32LittleEndian(result.AsSpan(8), level);
        BinaryPrimitives.WriteInt32LittleEndian(result.AsSpan(12), result.Length - 20);
        BinaryPrimitives.WriteInt32LittleEndian(result.AsSpan(16), result.Length);
        Image(nonce).CopyTo(result, 20); return result;
    }
    internal static void Main()
    {
        var owner = new F.Owner("test-profile", "actual-board-slot.sav");
        var nonce = new F.Nonce(0xffffffff, 0x80000000, 0xffff0000, 0x80000001);
        for (int bit = 0; bit < 128; ++bit)
        {
            byte[] bytes = new byte[16]; bytes[bit / 8] = (byte)(1 << bit % 8);
            var value = F.Nonce.Read(bytes); var fields = value.Fields();
            Check(F.Nonce.FromFields(fields.X, fields.Y, fields.PosX, fields.PosY, fields.GoalX, fields.GoalY) == value,
                "nonce lost bit " + bit);
        }
        for (int limb = 0; limb <= ushort.MaxValue; ++limb)
        {
            var value = F.Nonce.FromFields(-1, int.MinValue, limb, limb, limb, limb);
            var fields = value.Fields();
            if (fields.PosX != limb || fields.PosY != limb || fields.GoalX != limb || fields.GoalY != limb)
                throw new Exception("nonce limb quantized");
        }
        Check(true, "all uint16 limbs");
        foreach (float bad in new[] { float.NaN, float.PositiveInfinity, -0f, -1f, .5f, 65536f })
            Refuse(() => F.Nonce.FromFields(1, 2, bad, 0, 0, 0), "invalid nonce limb accepted");
        Refuse(() => F.Nonce.FromFields(0, 0, 0, 0, 0, 0), "zero nonce accepted");
        var records = Enumerable.Range(1, 200).Select(index => new F.ProjectileRecord(
            index == 200 ? uint.MaxValue : (uint)index, index % 2 == 0 ? 120 : 100, 33,
            BitConverter.SingleToInt32Bits(index / 7f), BitConverter.SingleToInt32Bits(index / 11f))).ToArray();
        var chunk = Chunk(nonce); var snapshot = new F.Snapshot(owner, 1, 42, nonce, chunk, 20, Image(nonce), records.Reverse());
        var copy = F.DecodeSnapshot(snapshot.Encoded.Span, chunk, owner);
        Check(copy.Binding == snapshot.Binding && copy.Records.SequenceEqual(records), "full provenance round trip");
        Check(copy.Records[^1].Id == uint.MaxValue && copy.Records.Count == 200, "full ID or 128-entry limit");
        chunk[25] ^= 1;
        Check(copy.NativeChunk.Span[25] != chunk[25], "snapshot aliases caller buffer");
        Refuse(() => F.DecodeSnapshot(snapshot.Encoded.Span, chunk, owner), "changed native chunk accepted");
        chunk = snapshot.NativeChunk.ToArray();
        var encoded = snapshot.Encoded.ToArray(); BinaryPrimitives.WriteInt32LittleEndian(encoded.AsSpan(encoded.Length - 4), 0x7fc00000);
        Refuse(() => F.DecodeSnapshot(encoded, chunk, owner), "invalid projectile bits accepted");
        Refuse(() => F.DecodeSnapshot(snapshot.Encoded.Span, chunk, new F.Owner("other", owner.FileName)), "wrong owner accepted");
        Refuse(() => new F.Snapshot(owner, 1, 42, nonce, chunk, 20, Image(nonce), records.Concat(new[] { records[0] })), "duplicate DataID accepted");
        Refuse(() => new F.Snapshot(owner, 1, 42, nonce, chunk, int.MaxValue, Image(nonce), records), "offset overflow accepted");
        var scalar = Image(nonce); scalar[132] ^= 1;
        Refuse(() => new F.Snapshot(owner, 1, 42, nonce, chunk, 20, scalar, records), "altered scalar accepted");
        var package = new F.Package(owner, false, new byte[] { 1, 2, 3 }, new[] { snapshot });
        var packageCopy = F.DecodePackage(package.Encoded.Span, owner);
        Check(packageCopy.Digest == package.Digest && packageCopy.Snapshots[0].Binding == snapshot.Binding, "complete package round trip");
        var absent = new F.Package(owner, true, ReadOnlySpan<byte>.Empty, Array.Empty<F.Snapshot>());
        Check(F.DecodePackage(absent.Encoded.Span, owner).NativeAbsent, "absence not preserved");
        for (int cut = 0; cut < package.Encoded.Length; ++cut)
        {
            int length = cut;
            Refuse(() => F.DecodePackage(package.Encoded.Span[..length], owner), "truncated package accepted");
        }
        Refuse(() => F.DecodePackage(package.Encoded.ToArray().Concat(new byte[] { 0 }).ToArray(), owner), "trailing package data accepted");
        Refuse(() => new F.Package(owner, true, new byte[] { 1 }, Array.Empty<F.Snapshot>()), "absence with bytes accepted");
        Console.WriteLine($"Passed {checks} Fusion save format checks; no native/game/profile execution.");
    }
}
