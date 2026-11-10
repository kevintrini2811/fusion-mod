using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native projectiles have no owner reference. Capture the firing scope rather
// than changing unrelated backwards projectiles (e.g. Leftpeater).
internal static class SplitPeaProjectiles
{
    [ThreadStatic] internal static Board? FiringBoard;
    private static readonly Dictionary<IntPtr, IntPtr> Backshots = new();

    internal static void Track(Board board, Projectile projectile)
    {
        if (FiringBoard?.Pointer == board.Pointer && projectile != null && !projectile.mDead)
            Backshots[projectile.Pointer] = board.Pointer;
    }

    internal static void Forget(Projectile projectile) => Backshots.Remove(projectile.Pointer);

    internal static void ForgetBoard(Board board)
    {
        foreach (var key in Backshots.Where(entry => entry.Value == board.Pointer).Select(entry => entry.Key).ToArray())
            Backshots.Remove(key);
    }

    internal static void Reflect(Projectile projectile)
    {
        if (projectile.mDead || projectile.mMotionType != ProjectileMotion.Backwards
            || (!Backshots.ContainsKey(projectile.Pointer) && !HypnoShooters.Contains(projectile)) || projectile.mBoard == null) return;

        // Projectile coordinates are board-local; mX offsets the board from the
        // screen origin. Reflect overshoot before native off-screen destruction.
        float leftEdge = -projectile.mBoard.mX;
        if (projectile.mPosX > leftEdge) return;
        projectile.mPosX = leftEdge + (leftEdge - projectile.mPosX);
        projectile.mX = projectile.mPosX;
        // Straight and Backwards both move at the native 3.33 units/tick. Merely
        // negating mVelX is ineffective: these motions use a fixed native step.
        projectile.mMotionType = ProjectileMotion.Straight;
        projectile.mVelX = -projectile.mVelX;
        projectile.mVelY = -projectile.mVelY;
        projectile.mRotation += MathF.PI;
        Forget(projectile);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SplitPeaFireScope
{
    private static void Prefix(Plant __instance, PlantWeapon thePlantWeapon, out Board? __state)
    {
        __state = SplitPeaProjectiles.FiringBoard;
        SplitPeaProjectiles.FiringBoard = __instance.mSeedType == SeedType.Splitpea
            && thePlantWeapon == PlantWeapon.Secondary ? __instance.mBoard : null;
    }

    private static void Finalizer(Board? __state) => SplitPeaProjectiles.FiringBoard = __state;
}

[HarmonyPatch]
internal static class SplitPeaProjectileCreatedPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        typeof(Board).GetMethods().Where(method => method.Name == nameof(Board.AddProjectile));

    private static void Postfix(Board __instance, Projectile __result) =>
        SplitPeaProjectiles.Track(__instance, __result);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class SplitPeaLeftEdgePatch
{
    // Normal motion calls collision after moving, before removing off-screen
    // shots. Native collision/damage and Torchwood conversion run as usual.
    private static void Prefix(Projectile __instance) => SplitPeaProjectiles.Reflect(__instance);
}

[HarmonyPatch]
internal static class SplitPeaProjectileCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Projectile), nameof(Projectile.Die));
        yield return AccessTools.Method(typeof(Projectile), nameof(Projectile.Reset));
    }

    private static void Prefix(Projectile __instance) => SplitPeaProjectiles.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class SplitPeaBoardCleanupPatch
{
    private static void Prefix(Board __instance) => SplitPeaProjectiles.ForgetBoard(__instance);
}
