using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Only explicitly attributed Fusion projectiles cross this boundary. Custom
// centisecond motion and overloaded position fields are excluded by Domains.
internal static class FusionProjectileTime
{
    internal const int Age = 1, Planar = 2, Vertical = 4, Gravity = 8, FixedStep = 16;
    internal const int Ballistic = Age | Planar | Vertical | Gravity;
    private const int AgeClock = 32;
    [ThreadStatic] internal static Board? FireBoard;
    internal sealed record AgeRead(Projectile Shot, IntPtr Board, uint Id, int NativeAge, float Seconds, AgeRead? Previous);
    [ThreadStatic] private static AgeRead? ReadingAge;
    internal static bool Owns(Projectile shot) => shot.mBoard != null
        && (FusionProjectileDomains.CanonicalAge(shot) || FusionProjectileDomains.TryFlags(shot, out _));

    internal static void Attribute(Projectile shot)
    {
        // Delay conversion until the spawning owner has finished assigning its
        // marker/motion. Native initialization itself still writes 100-Hz data.
        FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID, 100);
    }

    internal static void AdoptConversion(Projectile shot)
    {
        // An existing marked source projectile has an explicit legacy domain;
        // an unmarked native projectile adopted live has today's native domain.
        Prepare(shot);
        if (!FusionProjectileDomains.CanonicalAge(shot)
            && FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID) == null)
            NativeLaunch(shot);
    }
    internal static float SourceVelocity(float value) => value * (float)(100 * FusionTime.NativeTickSeconds);
    internal static float SourceGravity(float value)
    { float scale = (float)(100 * FusionTime.NativeTickSeconds); return value * scale * scale; }

    internal static void NativeLaunch(Projectile shot, int flags = Ballistic)
    {
        var record = FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile,
            shot.DataID, FusionNativeTimeRecords.CurrentRate);
        if (record == null) return;
        int rate = FusionNativeTimeRecords.CurrentRate;
        if ((record.Flags & AgeClock) != 0)
        {
            SaveAge(shot, record);
            shot.mProjectileAge = (int)Math.Clamp(Math.Floor((double)record.A * rate / 600 + 1e-9), 0, int.MaxValue);
            record.B = shot.mProjectileAge;
        }
        else { record.A = Math.Max(0, shot.mProjectileAge) * (600f / rate); record.B = shot.mProjectileAge; }
        record.Rate = rate;
        record.Flags = flags | AgeClock;
    }

    internal static void SourceLaunch(Projectile shot, int flags)
    {
        var record = FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID, 100);
        if (record == null) return;
        record.Rate = 100;
        record.Flags = flags;
        Rebase(shot, record);
    }

    internal static void SourceMotionHandoff(Projectile shot, int flags)
    {
        var record = FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID, 100);
        if (record == null) return;
        // The age already belonged to the native outer update. Only the
        // velocity fields are changing from a canonical custom motion owner.
        if (record.Flags == 0) record.Flags = Age;
        Rebase(shot, record);
        float scale = (float)(100 * FusionTime.NativeTickSeconds);
        if ((flags & Planar) != 0) { shot.mVelX *= scale; shot.mVelY *= scale; }
        if ((flags & Vertical) != 0) shot.mVelZ *= scale;
        if ((flags & Gravity) != 0) shot.mAccZ *= scale * scale;
        record.Flags = flags | (record.Flags & AgeClock);
    }

    internal static void Prepare(Projectile shot)
    {
        if (shot.mDead || shot.mBoard == null || !ScaredContent.Playing(shot.mBoard) || shot.mBoard.mPaused) return;
        var record = FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID);
        if (!FusionProjectileDomains.TryFlags(shot, out int flags))
        {
            // A native flying cherry can become a fully custom attached bomb.
            if (FusionProjectileDomains.CanonicalAge(shot) && record != null) FusionNativeTimeRecords.Remove(record);
            return;
        }
        // Published marked legacy fields were source steps at 100 Hz. An
        // unmarked projectile without saved provenance is never inferred here.
        record ??= FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID, 100);
        if (record == null) return;
        if ((record.Flags & AgeClock) != 0 && (!float.IsFinite(record.A) || !float.IsFinite(record.B) || record.A < 0)) return;
        record.Flags = flags | (record.Flags & AgeClock);
        Rebase(shot, record);
    }

    private static void SaveAge(Projectile shot, FusionNativeTimeRecords.Entry record)
    {
        if ((record.Flags & AgeClock) == 0)
        {
            record.A = Math.Max(0, shot.mProjectileAge) * (600f / record.Rate);
            record.Flags |= AgeClock;
        }
        else
        {
            float delta = shot.mProjectileAge - record.B;
            record.A = delta >= 0 ? record.A + delta * (600f / record.Rate)
                : Math.Max(0, shot.mProjectileAge) * (600f / record.Rate);
        }
        record.B = shot.mProjectileAge;
    }

    internal static void Rebase(Projectile shot, FusionNativeTimeRecords.Entry record)
    {
        int rate = FusionNativeTimeRecords.CurrentRate, old = record.Rate;
        if ((record.Flags & Age) != 0) SaveAge(shot, record);
        if (rate == old) return;
        float scale = FusionNativeTimeRecords.VelocityScale(old, rate);
        if ((record.Flags & Age) != 0)
        {
            shot.mProjectileAge = (int)Math.Clamp(Math.Floor((double)record.A * rate / 600 + 1e-9), 0, int.MaxValue);
            record.B = shot.mProjectileAge;
        }
        if ((record.Flags & Planar) != 0) { shot.mVelX *= scale; shot.mVelY *= scale; }
        if ((record.Flags & Vertical) != 0) shot.mVelZ *= scale;
        if ((record.Flags & Gravity) != 0) shot.mAccZ *= scale * scale;
        record.Rate = rate;
    }

    internal static float AgeSeconds(Projectile shot)
    {
        if (ReadingAge is { } reading && reading.Shot.Pointer == shot.Pointer && reading.Id == shot.DataID)
            return reading.Seconds;
        if (FusionProjectileDomains.CanonicalAge(shot)) return FusionTime.SourceSeconds(shot.mProjectileAge);
        var record = FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID);
        if (record != null && (record.Flags & AgeClock) != 0)
            return (record.A + (shot.mProjectileAge - record.B) * (600f / record.Rate)) / 600;
        // An identified old marker has the published 100-unit legacy contract.
        int rate = record?.Rate ?? (FusionProjectileDomains.TryFlags(shot, out _) ? 100 : FusionNativeTimeRecords.CurrentRate);
        return (float)shot.mProjectileAge / rate;
    }

    internal static AgeRead? BeginAgeRead(Projectile shot)
    {
        if (!Owns(shot) || FusionProjectileDomains.CanonicalAge(shot)) return null;
        float seconds = AgeSeconds(shot);
        var scope = new AgeRead(shot, shot.mBoard.Pointer, shot.DataID, shot.mProjectileAge, seconds, ReadingAge);
        ReadingAge = scope;
        // The pinned native motion methods only read age. Their literal 20/60
        // thresholds and homing acceleration use the canonical source scale;
        // the actual native age and saved 600-unit remainder stay unchanged.
        shot.mProjectileAge = (int)Math.Clamp(Math.Floor(seconds * 100d + 1e-5), 0, int.MaxValue);
        return scope;
    }
    internal static void EndAgeRead(AgeRead? scope)
    {
        if (scope == null) return;
        if (scope.Shot.DataID == scope.Id && scope.Shot.mBoard?.Pointer == scope.Board
            && !FusionProjectileDomains.CanonicalAge(scope.Shot)) scope.Shot.mProjectileAge = scope.NativeAge;
        ReadingAge = scope.Previous;
    }

    internal readonly record struct ThreeRead(Projectile Shot, IntPtr Board, uint Id, float NextVelocity);
    internal static ThreeRead? BeginThreepeater(Projectile shot)
    {
        if (shot.mMotionType != ProjectileMotion.Threepeater || !Owns(shot)) return null;
        double quantum = 100 * FusionTime.NativeTickSeconds;
        double decay = Math.Pow(.97, quantum);
        float old = shot.mVelY;
        // Integrate the published geometric source trajectory over this native
        // interval. Native then applies its .97 damping and shadow update;
        // restore the correctly decayed per-native-step velocity afterwards.
        shot.mVelY *= (float)((1 - decay) / (.03 * quantum));
        return new ThreeRead(shot, shot.mBoard.Pointer, shot.DataID, old * (float)decay);
    }
    internal static void EndThreepeater(ThreeRead? scope)
    {
        if (scope is { } saved && saved.Shot.DataID == saved.Id && saved.Shot.mBoard?.Pointer == saved.Board
            && saved.Shot.mMotionType == ProjectileMotion.Threepeater && !FusionProjectileDomains.CanonicalAge(saved.Shot))
            saved.Shot.mVelY = saved.NextVelocity;
    }

    internal static void BeforeNormalMotion(Projectile shot)
    {
        Prepare(shot);
        var record = FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID);
        if (record == null || (record.Flags & FixedStep) == 0 || shot.mDead || shot.mBoard.mPaused) return;
        // Native Straight/Threepeater advance X by a literal 3.33. Correct
        // before the original method so its subsequent checks see the endpoint.
        shot.mPosX += 3.33f * (100f / record.Rate - 1);
    }

    internal static void Cleanup(Board board)
    {
        var live = new HashSet<uint>();
        for (int i = 0; i < board.m_projectiles.Count; ++i)
            if (board.m_projectiles[i] is { mDead: false } shot) live.Add(shot.DataID);
        foreach (var record in FusionNativeTimeRecords.Entries(board, FusionNativeTimeRecords.Kind.Projectile))
            if (!live.Contains(record.EntityId)) FusionNativeTimeRecords.Remove(record);
        FusionCraterTime.Cleanup(board);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class FusionProjectileTimeUpdatePatch
{
    [HarmonyPriority(Priority.First + 2000)]
    private static void Prefix(Projectile __instance) => FusionProjectileTime.Prepare(__instance);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class FusionProjectileTimeMotionPatch
{
    [HarmonyPriority(Priority.First + 2000)]
    private static void Prefix(Projectile __instance) => FusionProjectileTime.BeforeNormalMotion(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class FusionProjectileTimeFireScope
{
    private static void Prefix(Plant __instance, out Board? __state)
    {
        __state = FusionProjectileTime.FireBoard;
        FusionProjectileTime.FireBoard = FusionPlant.From(__instance).Marker != SeedType.None ? __instance.mBoard : null;
    }
    private static void Finalizer(Board? __state) => FusionProjectileTime.FireBoard = __state;
}
[HarmonyPatch]
internal static class FusionProjectileTimeBirthPatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => typeof(Board).GetMethods()
        .Where(method => method.Name == nameof(Board.AddProjectile));
    private static void Postfix(Board __instance, Projectile __result)
    {
        if (__result != null && !__result.mDead && FusionProjectileTime.FireBoard?.Pointer == __instance.Pointer)
            FusionProjectileTime.Attribute(__result);
    }
}
[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class FusionProjectileTimeCleanupPatch
{ private static void Postfix(Board __instance) => FusionProjectileTime.Cleanup(__instance); }

[HarmonyPatch]
internal static class FusionProjectileNativeAgeReadPatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => typeof(Projectile).GetMethods()
        .Where(method => method.Name is nameof(Projectile.UpdateNormalMotion) or nameof(Projectile.UpdateLobMotion));
    [HarmonyPriority(-2000)]
    private static void Prefix(Projectile __instance, bool __runOriginal, out FusionProjectileTime.AgeRead? __state)
    { __state = __runOriginal ? FusionProjectileTime.BeginAgeRead(__instance) : null; }
    private static void Finalizer(FusionProjectileTime.AgeRead? __state) => FusionProjectileTime.EndAgeRead(__state);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class FusionProjectileThreepeaterReadPatch
{
    [HarmonyPriority(-1500)]
    private static void Prefix(Projectile __instance, bool __runOriginal, out FusionProjectileTime.ThreeRead? __state)
    { __state = __runOriginal ? FusionProjectileTime.BeginThreepeater(__instance) : null; }
    private static void Finalizer(FusionProjectileTime.ThreeRead? __state) => FusionProjectileTime.EndThreepeater(__state);
}
