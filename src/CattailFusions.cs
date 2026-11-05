using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Cattail retains all-lane targeting, homing, terrain and serialization.
// This owner replaces its attack clock with the two original animation events.
internal static class CattailFusions
{
    internal const int ShootTicks = 84;
    private const int IceShot = -540001, FireShot = -540002;
    [ThreadStatic] internal static Plant? Firing;
    [ThreadStatic] internal static Hit? Current;
    internal sealed record Hit(Projectile Shot, uint ShotId, Zombie Target, uint TargetId)
    { internal bool Pending = true; }

    internal static int Family(Projectile shot) => shot.mProjectileType == ProjectileType.Spike
        ? shot.mCobTargetX == IceShot ? 1 : shot.mCobTargetX == FireShot ? 2 : 0 : 0;

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsElementalCattail(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = plant.mDoSpecialCountdown = 0;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || plant.FindTargetZombie(plant.mRow, PlantWeapon.Primary) == null) return;
            plant.mLaunchCounter = 143 + plant.mBoard.Rand(15);
            plant.mStartRow = ShootTicks;
        }
        int elapsed = ShootTicks - plant.mStartRow + 1;
        --plant.mStartRow;
        if (elapsed is not (34 or 67)) return;
        var target = plant.FindTargetZombie(plant.mRow, PlantWeapon.Primary);
        if (target != null) plant.Fire(target, plant.mRow, PlantWeapon.Primary);
    }

    internal static void Track(Projectile shot)
    {
        if (shot.mDead || shot.mProjectileType != ProjectileType.Spike || Firing is not { } plant
            || plant.mBoard?.Pointer != shot.mBoard?.Pointer || !ChomperFusions.Active(plant)) return;
        shot.mCobTargetX = FusionPlant.From(plant) == FusionPlant.FrostCattail ? IceShot : FireShot;
        shot.mCobTargetRow = 40;
        ImportedPeaVisuals.EnsureCattailProjectile(shot);
    }

    internal static void Damage(Zombie zombie, ref int damage)
    {
        var hit = Current;
        if (hit == null || !hit.Pending || hit.Shot.DataID != hit.ShotId
            || zombie.Pointer != hit.Target.Pointer || zombie.DataID != hit.TargetId
            || zombie.mBoard?.Pointer != hit.Shot.mBoard?.Pointer) return;
        hit.Pending = false;
        int family = Family(hit.Shot);
        int multiplier = family == 1 ? zombie.mIceTrapCounter > 0 ? 4 : zombie.mChilledCounter > 0 ? 2 : 1
            : Overheat.Has(zombie) ? 3 : 1;
        damage = (int)Math.Min(int.MaxValue, Math.Max(0, (long)hit.Shot.mCobTargetRow) * multiplier);
        if (family == 1)
        {
            zombie.ApplyChill(false);
            SnowPea.Hit(zombie, 10);
        }
        else
        {
            zombie.RemoveColdEffects();
            Overheat.Apply(zombie);
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class CattailInitializePatch
{ private static void Postfix(Plant __instance) => CattailFusions.Initialize(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class CattailAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsElementalCattail(FusionPlant.From(__instance))) return true;
        CattailFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class CattailShooterPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsElementalCattail(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class CattailShootingPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsElementalCattail(FusionPlant.From(__instance)); }

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class CattailFireScope
{
    private static void Prefix(Plant __instance, out Plant? __state)
    {
        __state = CattailFusions.Firing;
        CattailFusions.Firing = FusionPlant.IsElementalCattail(FusionPlant.From(__instance)) ? __instance : null;
    }
    private static void Finalizer(Plant? __state) => CattailFusions.Firing = __state;
}
[HarmonyPatch]

internal static class CattailProjectilePatch
{
    private static System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> TargetMethods() =>
        typeof(Board).GetMethods().Where(method => method.Name == nameof(Board.AddProjectile)); private static void Postfix(Projectile? __result) { if (__result != null) CattailFusions.Track(__result); } }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class CattailProjectileVisualPatch
{
    private static void Postfix(Projectile __instance)
    { if (!__instance.mDead && CattailFusions.Family(__instance) != 0) ImportedPeaVisuals.EnsureCattailProjectile(__instance); }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class CattailImpactScope
{
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Projectile __instance, Zombie theZombie, out CattailFusions.Hit? __state)
    {
        __state = CattailFusions.Current;
        CattailFusions.Current = !__instance.mDead && theZombie != null && CattailFusions.Family(__instance) != 0
            ? new(__instance, __instance.DataID, theZombie, theZombie.DataID) : null;
    }
    private static void Finalizer(CattailFusions.Hit? __state) => CattailFusions.Current = __state;
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class CattailDamagePatch
{
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Zombie __instance, ref int theDamage) => CattailFusions.Damage(__instance, ref theDamage);
}
