using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// CabbageBlover/CornBlover throw freely, rather than aiming at a zombie.
// Native serialized plant and projectile fields own every clock and velocity.
internal static class AirLobbers
{
    internal const int ShootTicks = 50, ShotEvent = 25;
    private const int Marker = -529100;
    internal static bool IsShot(Projectile shot) => shot.mCobTargetX == Marker
        && shot.mProjectileType is ProjectileType.Cabbage or ProjectileType.Kernel or ProjectileType.Butter;
    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsAirLobber(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = plant.mDoSpecialCountdown = 0;
    }
    private static bool HasTarget(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_zombies.Count; i++)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow
                || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float x = plant.mX + plant.mWidth / 2f;
            if (rect.xMax >= x - 120 && rect.xMin <= x + 120) return true;
        }
        return false;
    }
    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow > 0 && --plant.mStartRow == ShotEvent) Fire(plant);
        if (plant.mStartRow > 0 || plant.mLaunchCounter > 0 || !HasTarget(plant)) return;
        plant.mLaunchCounter = 285 + plant.mBoard.Rand(31);
        plant.mStartRow = ShootTicks;
    }
    private static void Fire(Plant plant)
    {
        for (int side = -1; side <= 1; side++)
        {
            var type = FusionPlant.From(plant) == FusionPlant.CabbageDropper ? ProjectileType.Cabbage
                : plant.mBoard.Rand(4) == 0 ? ProjectileType.Butter : ProjectileType.Kernel;
            var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
                plant.mY + plant.mHeight / 2f, plant.mRenderOrder + 1, plant.mRow, type);
            if (shot == null || shot.mDead) continue;
            shot.mCobTargetX = Marker;
            shot.mCobTargetRow = type == ProjectileType.Kernel ? 20 : 40;
            shot.mDamageRangeFlags = DamageRangeFlags.Ground;
            shot.mMotionType = ProjectileMotion.Lobbed;
            shot.mVelX = side * 2 * ScaredyFusions.SourceUnit * .01f;
            shot.mVelY = 0;
            shot.mPosZ = -1.5f * ScaredyFusions.SourceUnit;
            shot.mVelZ = -4 * ScaredyFusions.SourceUnit * .01f;
            shot.mAccZ = 15 * ScaredyFusions.SourceUnit * .0001f;
            shot.mShadowY = shot.mPosY;
            ImportedPeaVisuals.EnsureAirLobProjectile(shot);
        }
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class AirLobberInitializePatch
{ private static void Postfix(Plant __instance) => AirLobbers.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class AirLobberAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsAirLobber(FusionPlant.From(__instance))) return true;
        AirLobbers.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class AirLobberSpecialPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsAirLobber(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class AirLobberShootingPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsAirLobber(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class AirLobberImpactPatch
{
    private static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (!AirLobbers.IsShot(__instance)) return true;
        LobberImpact.Apply(__instance, theZombie, FumeProjectiles.ModifiedDamage(__instance, __instance.mCobTargetRow),
            false, __instance.mProjectileType == ProjectileType.Butter, 0, false);
        return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class AirLobberVisualPatch
{ private static void Postfix(Projectile __instance) => ImportedPeaVisuals.EnsureAirLobProjectile(__instance); }
