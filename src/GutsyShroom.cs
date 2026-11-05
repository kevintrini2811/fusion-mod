using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// ScaredFume selects smoke for its nearest forward enemy within seven source
// units; otherwise two spores. It never enters Scaredy-shroom's fear state.
internal static class GutsyShroom
{
    private const float SporeMarker = -40030;
    internal sealed record Impact(Projectile Projectile, uint ProjectileId, Zombie Zombie, uint ZombieId, IntPtr Board);
    [ThreadStatic] internal static Impact? CurrentImpact;
    [ThreadStatic] internal static bool Pending;

    internal static bool IsShot(Projectile projectile) => projectile.mCobTargetX == SporeMarker
        && projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea;

    internal static int Duration(Plant plant) => plant.mTargetX == 1 ? 117 : 67;
    internal static float AnimationTime(Plant plant) => (Duration(plant) - plant.mStartRow) * .01f
        * (plant.mTargetX == 1 ? 1 : 2);

    internal static void Tick(Plant plant)
    {
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            Zombie? nearest = null;
            float nearestX = float.MaxValue;
            float x = plant.mX + plant.mWidth / 2f;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            {
                var zombie = plant.mBoard.m_zombies[i];
                if (zombie == null || zombie.mRow != plant.mRow
                    || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                    || !SquashFusions.Eligible(plant.mBoard, zombie, plant.GetDamageRangeFlags(PlantWeapon.Primary))) continue;
                var rect = zombie.GetZombieRect();
                float target = (rect.xMin + rect.xMax) / 2;
                if (target <= x || target > plant.mBoard.GridToPixelX(8, plant.mRow) + 160 || target >= nearestX) continue;
                nearest = zombie; nearestX = target;
            }
            if (nearest == null) return;
            // Persist the selected clip/mode for the entire attack and on load.
            plant.mTargetX = nearestX < x + 7f / 1.35f * 80 ? 1 : 0;
            plant.mStartRow = Duration(plant);
            plant.mLaunchCounter = 150;
        }
        int elapsed = Duration(plant) - plant.mStartRow;
        if (plant.mTargetX == 1 && elapsed == 75)
        {
            FumeFusions.Smoke(plant, FusionPlant.GutsyShroom);
            FumeFusions.Hit(plant, FusionPlant.GutsyShroom);
        }
        else if (plant.mTargetX == 0 && elapsed is 38 or 55) Shoot(plant);
        --plant.mStartRow;
    }

    private static void Shoot(Plant plant)
    {
        var projectile = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f + 20,
            plant.mY + plant.mHeight / 2f, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
        if (projectile == null || projectile.mDead) return;
        projectile.mCobTargetX = SporeMarker;
        projectile.mMotionType = ProjectileMotion.Star;
        projectile.mVelX = 6f / 1.35f * 80 / 100;
        projectile.mVelY = projectile.mVelZ = projectile.mAccZ = projectile.mPosZ = 0;
        projectile.mShadowY = projectile.mPosY + PuffShooters.FlightHeight;
        projectile.mDamageRangeFlags = DamageRangeFlags.Ground;
        ImportedPeaVisuals.EnsureGutsyProjectile(projectile);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class GutsyImpactPatch
{
    internal static void Prefix(Projectile __instance, Zombie theZombie,
        out (GutsyShroom.Impact? Impact, bool Pending) __state)
    {
        __state = (GutsyShroom.CurrentImpact, GutsyShroom.Pending);
        GutsyShroom.CurrentImpact = GutsyShroom.IsShot(__instance) && !__instance.mDead
            && theZombie != null && !theZombie.mDead && !theZombie.IsDeadOrDying()
            && theZombie.mBoard?.Pointer == __instance.mBoard?.Pointer
            ? new(__instance, __instance.DataID, theZombie, theZombie.DataID, __instance.mBoard!.Pointer) : null;
        GutsyShroom.Pending = false;
    }
    internal static void Finalizer((GutsyShroom.Impact? Impact, bool Pending) __state)
    { GutsyShroom.CurrentImpact = __state.Impact; GutsyShroom.Pending = __state.Pending; }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.GetDamageFlags))]
internal static class GutsyDamageTargetPatch
{
    internal static void Postfix(Projectile __instance, Zombie theZombie)
    {
        var impact = GutsyShroom.CurrentImpact;
        if (impact != null && impact.Projectile.Pointer == __instance.Pointer && impact.ProjectileId == __instance.DataID
            && impact.Zombie.Pointer == theZombie?.Pointer && impact.ZombieId == theZombie.DataID
            && impact.Board == theZombie.mBoard?.Pointer) GutsyShroom.Pending = true;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class GutsyDamagePatch
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Zombie __instance, ref int theDamage)
    {
        var impact = GutsyShroom.CurrentImpact;
        if (!GutsyShroom.Pending || impact == null || impact.Zombie.Pointer != __instance.Pointer
            || impact.ZombieId != __instance.DataID || impact.Board != __instance.mBoard?.Pointer) return;
        GutsyShroom.Pending = false;
        theDamage = 30;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class GutsyTorchwoodPatch
{
    internal static bool Prefix(Projectile __instance) => !GutsyShroom.IsShot(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class GutsyProjectileVisualPatch
{
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && GutsyShroom.IsShot(__instance)) ImportedPeaVisuals.EnsureGutsyProjectile(__instance);
    }
}

// Star motion integrates shadow Y. Restore the native pea flight height before
// collision, on every tick and after loading, rather than only at creation.
[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class GutsyFlightHeightPatch
{
    internal static void Prefix(Projectile __instance)
    {
        if (GutsyShroom.IsShot(__instance))
            __instance.mShadowY = __instance.mPosY + PuffShooters.FlightHeight;
    }
}
