using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native shooters own targeting, timing and collision. Cherry bullets are
// distinct in Fusion: 40 direct damage, without ordinary Torchwood conversion.
internal static class CherryShooter
{
    // Native peas do not use the saved cob target. Initialization and Reset
    // clear it; native serialization preserves this provenance for absorption.
    private const float CherryMarker = -41001, ExplosiveMarker = -41010, CatalyzedMarker = -41011, CatalyzedFireMarker = -41012;
    [ThreadStatic] internal static Plant? FiringPlant;
    [ThreadStatic] internal static IntPtr ImpactZombie;
    [ThreadStatic] internal static bool DamagePending;

    internal static void Track(Projectile projectile)
    {
        var plant = FiringPlant;
        if (plant == null || projectile.mBoard == null || projectile.mDead
            || plant.mBoard?.Pointer != projectile.mBoard.Pointer
            || projectile.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea)) return;
        if (TryShot(projectile)) return;
        bool explosive = FusionPlant.From(plant) == FusionPlant.ExplodeOShooter;
        projectile.mCobTargetX = explosive ? ExplosiveMarker : CherryMarker;
        if (explosive) ImportedPeaVisuals.RegisterExplodeProjectile(projectile);
        else ImportedPeaVisuals.RegisterCherryProjectile(projectile);
    }

    internal static bool TryShot(Projectile projectile) => projectile.mBoard != null
        && projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea or ProjectileType.Fireball or ProjectileType.PeashooterFireball
        && projectile.mCobTargetX is CherryMarker or ExplosiveMarker or CatalyzedMarker or CatalyzedFireMarker;

    internal static bool IsExplosive(Projectile projectile) => TryShot(projectile)
        && projectile.mCobTargetX is ExplosiveMarker or CatalyzedMarker or CatalyzedFireMarker;

    internal static int ExplosionDamage(Projectile projectile) => projectile.mCobTargetX == CatalyzedMarker ? 80
        : projectile.mCobTargetX == CatalyzedFireMarker ? 60 : 300;

    internal static void Catalyze(Projectile projectile)
    {
        projectile.mCobTargetX = projectile.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball ? CatalyzedFireMarker : CatalyzedMarker;
        ImportedPeaVisuals.ForgetProjectile(projectile);
        ImportedPeaVisuals.RegisterExplodeProjectile(projectile);
    }

    internal static void Forget(Projectile projectile)
    {
        if (TryShot(projectile)) projectile.mCobTargetX = 0;
        ImportedPeaVisuals.ForgetProjectile(projectile);
    }

}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class CherryFireScope
{
    private static void Prefix(Plant __instance, out Plant? __state)
    {
        __state = CherryShooter.FiringPlant;
        var identity = FusionPlant.From(__instance);
        CherryShooter.FiringPlant = FusionPlant.IsCherryShooter(identity)
            ? __instance : null;
    }
    private static void Finalizer(Plant? __state) => CherryShooter.FiringPlant = __state;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class CherryProjectileInitializedPatch
{
    private static void Postfix(Projectile __instance) => CherryShooter.Track(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class CherryImpactScope
{
    internal static void Prefix(Projectile __instance, Zombie theZombie, out (IntPtr Zombie, bool Pending) __state)
    {
        __state = (CherryShooter.ImpactZombie, CherryShooter.DamagePending);
        CherryShooter.ImpactZombie = IntPtr.Zero;
        CherryShooter.DamagePending = false;
        if (!CherryShooter.TryShot(__instance) || CherryShooter.IsExplosive(__instance) || theZombie == null
            || theZombie.mDead || theZombie.IsDeadOrDying()) return;
        CherryShooter.ImpactZombie = theZombie.Pointer;
        CherryShooter.DamagePending = true;
    }
    internal static void Finalizer((IntPtr Zombie, bool Pending) __state)
    { CherryShooter.ImpactZombie = __state.Zombie; CherryShooter.DamagePending = __state.Pending; }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class CherryDamagePatch
{
    internal static void Prefix(Zombie __instance, ref int theDamage)
    {
        if (!CherryShooter.DamagePending || CherryShooter.ImpactZombie != __instance.Pointer) return;
        CherryShooter.DamagePending = false;
        theDamage = 40;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class CherryTorchwoodPatch
{
    private static bool Prefix(Projectile __instance) => !CherryShooter.TryShot(__instance);
}

[HarmonyPatch]
internal static class CherryProjectileCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        new[] { AccessTools.Method(typeof(Projectile), "Die"), AccessTools.Method(typeof(Projectile), "Reset") };
    private static void Prefix(Projectile __instance) => CherryShooter.Forget(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class CherryProjectileRestorePatch
{
    private static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && CherryShooter.TryShot(__instance))
            ImportedPeaVisuals.EnsureCherryProjectile(__instance);
    }
}
