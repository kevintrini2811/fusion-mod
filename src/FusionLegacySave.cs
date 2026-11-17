using System.Buffers.Binary;
using System.Security.Cryptography;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppReloaded.Services;
using Il2CppOOI.Platforms.Steam;

namespace FusionMod;

// Compatibility with the former external-provenance save header only. New
// saves use native format 14 and never need an external projectile snapshot.
internal static class FusionLegacySave
{
    internal const int LegacyVersion = 0x4655010e, NativeVersion = 14;
    private const int MaximumBytes = 128 * 1024 * 1024;
    private static readonly HashSet<(IntPtr Platform, string File)> Blocked = new();
    internal static bool IsBlocked(PlatformService platform, string file) => Blocked.Contains((platform.Pointer, file));
    internal static void Block(SerializationService service, Exception error)
    {
        var platform = service.m_platformService.TryCast<PlatformService>();
        if (platform != null) Blocked.Add((platform.Pointer, service.GetSaveFilename()));
        FusionMod.Instance.LoggerInstance.Error("Fusion legacy save preserved; native writes to this file are blocked for this session: " + error);
    }
    internal static bool Marked(byte[] bytes) => bytes.Length >= 20
        && BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(4)) == LegacyVersion;
    internal static bool HasMarkedBuffer(SerializationService service)
    {
        for (int i = 0; i < service.m_saveBuffer.Count; ++i)
        {
            var buffer = service.m_saveBuffer[i];
            if (buffer.Length >= 20 && (buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24) == LegacyVersion)
                return true;
        }
        return false;
    }

    internal static string Preserve(string target, string profile)
    {
        if (!Guid.TryParse(profile, out var guid) || !Path.IsPathFullyQualified(target))
            throw new InvalidDataException("Legacy Fusion backup identity is invalid.");
        byte[] bytes;
        using (var file = new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            if (file.Length > MaximumBytes) throw new InvalidDataException("Legacy Fusion save exceeds backup limit.");
            bytes = new byte[checked((int)file.Length)];
            int offset = 0;
            while (offset < bytes.Length)
            {
                int read = file.Read(bytes, offset, bytes.Length - offset);
                if (read == 0) throw new EndOfStreamException("Legacy Fusion save ended during backup.");
                offset += read;
            }
            if (file.ReadByte() != -1 || file.Length != bytes.Length) throw new IOException("Legacy Fusion save changed while reading.");
        }
        string digest = Convert.ToHexString(SHA256.HashData(bytes));
        string directory = Path.Combine(Path.GetDirectoryName(target)!, "FusionLegacyBackups", guid.ToString("D"));
        Directory.CreateDirectory(directory);
        string backup = Path.Combine(directory, digest + ".ig.dat");
        if (!File.Exists(backup))
        {
            string temp = backup + "." + Guid.NewGuid().ToString("N") + ".tmp";
            // A failed write remains available for diagnosis; no native or old
            // companion file is deleted or overwritten by this migration.
            using (var file = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            { file.Write(bytes); file.Flush(true); }
            File.Move(temp, backup, false);
        }
        if (!File.ReadAllBytes(backup).AsSpan().SequenceEqual(bytes)) throw new IOException("Legacy Fusion backup verification failed.");
        return backup;
    }

    internal static bool Prepare(SerializationService service)
    {
        int category = (int)service.GetCurrentSaveCategory();
        if (category < 0 || category >= service.m_saveBuffer.Count)
        {
            for (int i = 0; i < service.m_saveBuffer.Count; ++i)
                if (Marked(service.m_saveBuffer[i].ToArray()))
                    throw new InvalidDataException("Legacy Fusion save has no unambiguous native category.");
            return false;
        }
        byte[] bytes = service.m_saveBuffer[category].ToArray();
        if (!Marked(bytes)) return false;
        int length = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(16));
        if (BinaryPrimitives.ReadInt32LittleEndian(bytes) != SerializationService.SaveHeader.MAGIC_NUMBER
            || length < 20 || length > bytes.Length || length > 10 * 1024 * 1024)
            throw new InvalidDataException("Legacy Fusion category header is invalid.");
        var platform = service.m_platformService.TryCast<PlatformService>()?.Platform.TryCast<SteamPlatform>()
            ?? throw new NotSupportedException("Legacy Fusion backup requires the native Steam save path.");
        string profile = service.m_userService?.ActiveUserProfile?.mGuid
            ?? throw new InvalidDataException("Legacy Fusion profile is unavailable.");
        string backup = Preserve(platform._platformFilePath(service.GetSaveFilename()), profile);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(4), NativeVersion);
        service.m_saveBuffer[category] = new Il2CppStructArray<byte>(bytes);
        FusionMod.Instance.LoggerInstance.Msg("Preserved legacy Fusion board before native load: " + backup);
        return true;
    }
}

[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Deserialize))]
internal static class FusionLegacySaveLoadPatch
{
    [HarmonyPriority(Priority.First + 2000)]
    private static bool Prefix(SerializationService __instance, out bool __state, ref bool __result)
    {
        __state = false;
        if (!FusionLegacySave.HasMarkedBuffer(__instance)) return true;
        try { __state = FusionLegacySave.Prepare(__instance); return true; }
        catch (Exception error) { FusionLegacySave.Block(__instance, error); __result = false; return false; }
    }
    private static void Finalizer(SerializationService __instance, bool __state, Exception? __exception)
    { if (__state && __exception != null) FusionLegacySave.Block(__instance, __exception); }
}

[HarmonyPatch(typeof(PlatformService), nameof(PlatformService.SaveFile))]
internal static class FusionLegacySaveWritePatch
{
    private static bool Prefix(PlatformService __instance, string fileName, ref bool __result)
    { if (!FusionLegacySave.IsBlocked(__instance, fileName)) return true; __result = false; return false; }
}
[HarmonyPatch(typeof(PlatformService), nameof(PlatformService.DeleteFile))]
internal static class FusionLegacySaveDeletePatch
{
    private static bool Prefix(PlatformService __instance, string fileName, ref bool __result)
    { if (!FusionLegacySave.IsBlocked(__instance, fileName)) return true; __result = false; return false; }
}
