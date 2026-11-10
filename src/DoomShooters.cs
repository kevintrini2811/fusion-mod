using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native saved fields own attack phase, accumulated damage, volley count and
// rest. Fire still owns native projectile allocation and Threepeater motion.
internal static class DoomShooters
{
    [ThreadStatic] internal static Plant? Firing;
    [ThreadStatic] internal static Projectile? Created;
    internal static bool Owns(FusionPlant identity) => FusionPlant.IsDoomShooter(identity)
        || identity == FusionPlant.DeathStar;
    internal const int ShootTicks = 100;
    internal static int Duration(Plant plant) => FusionPlant.From(plant) == FusionPlant.DoomPea
        ? Math.Min(ShootTicks, Math.Max(20, plant.mLaunchRate)) : ShootTicks;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!Owns(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = 150;
        plant.mLaunchCounter = identity == FusionPlant.DeathStar ? 300 : 0;
        plant.mStartRow = plant.mShootingCounter = plant.mTargetX = plant.mTargetY = 0;
        plant.mDoSpecialCountdown = 0;
    }

    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.DeathStar && plant.mLaunchCounter > 50)
            plant.mLaunchCounter = 50;
    }

    internal static bool HasTarget(Plant plant)
    {
        bool three = FusionPlant.From(plant) == FusionPlant.DoomSpreader;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
                && Math.Abs(zombie.mRow - plant.mRow) <= (three ? 1 : 0)
                && zombie.GetZombieRect().xMax >= plant.mX + 40
                && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) return true;
        }
        return false;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(DoomShooters))) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.DeathStar)
        {
            if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
            if (plant.mLaunchCounter == 0)
            {
                // Allocation must succeed before the plant disappears. A
                // failed allocation retries without replaying a fusion reward.
                if (DoomProjectiles.Meteor(plant) != null) plant.Die();
            }
            return;
        }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || !HasTarget(plant)) return;
            plant.mTargetY = Duration(plant);
            plant.mStartRow = plant.mTargetY;
            plant.mLaunchCounter = 150;
            if (identity == FusionPlant.DoomSpreader) plant.mTargetX = plant.mTargetX % 7 + 1;
        }
        int duration = plant.mTargetY > 0 ? plant.mTargetY : ShootTicks;
        int elapsed = duration - plant.mStartRow;
        if (identity == FusionPlant.DoomSpreader)
        {
            if (elapsed == 50)
                for (int row = Math.Max(0, plant.mRow - 1);
                     row <= Math.Min(plant.mBoard.GetNumRows() - 1, plant.mRow + 1); ++row) Shoot(plant, row);
            if (elapsed == 70 && (plant.mRow == 0 || plant.mRow == plant.mBoard.GetNumRows() - 1))
                Shoot(plant, plant.mRow);
        }
        else if (identity == FusionPlant.GatlingDoom ? elapsed is 34 or 50 or 67 or 84 : elapsed == duration * 67 / 100)
            Shoot(plant, plant.mRow);
        --plant.mStartRow;
    }

    internal static void Shoot(Plant plant, int row)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        var identity = FusionPlant.From(plant);
        int damage = 40, family = 1;
        if (identity == FusionPlant.DoomPea)
        {
            plant.mTargetX = Math.Min(60, Math.Max(0, plant.mTargetX) + 1);
            damage += plant.mTargetX;
            // DoomPeashooter.Shoot1 reads the left Frenzy-shroom's interval
            // and writes its accumulated damage back to that same neighbour.
            Plant? left = null;
            for (int i = 0; i < plant.mBoard.m_plants.Count; ++i)
                if (plant.mBoard.m_plants[i] is { } candidate
                    && candidate.mPlantCol == plant.mPlantCol - 1 && candidate.mRow == plant.mRow
                    && ChomperFusions.Active(candidate) && FusionPlant.From(candidate) == FusionPlant.FrenzyShroom)
                { left = candidate; break; }
            plant.mLaunchRate = left != null && ChomperFusions.Active(left)
                && FusionPlant.From(left) == FusionPlant.FrenzyShroom ? Math.Clamp(left.mTargetX, 20, 150) : 150;
            plant.mLaunchCounter = Math.Max(1, plant.mLaunchRate - (plant.mTargetY > 0 ? plant.mTargetY : ShootTicks) * 67 / 100);
            if (left != null && ChomperFusions.Active(left) && FusionPlant.From(left) == FusionPlant.FrenzyShroom)
                left.mShootingCounter = damage;
        }
        else if (identity == FusionPlant.GatlingDoom)
        {
            plant.mTargetX = plant.mTargetX % 16 + 1;
            damage = plant.mTargetX == 16 ? 1800 : 300;
            family = plant.mTargetX == 16 ? 3 : 2;
            if (family == 3) plant.mLaunchCounter = 450;
        }
        else if (plant.mTargetX == 7) { family = 3; damage = 120; }
        var old = Firing;
        var oldCreated = Created;
        Firing = plant;
        Created = null;
        try
        {
            plant.Fire(null!, row, PlantWeapon.Primary);
            if (Created is { mDead: false } shot) DoomProjectiles.Track(shot, family, damage);
        }
        finally { Firing = old; Created = oldCreated; }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class DoomShooterInitializePatch
{ private static void Postfix(Plant __instance) => DoomShooters.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class DoomShooterAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!DoomShooters.Owns(FusionPlant.From(__instance))) return true;
        DoomShooters.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class DoomShooterShootingPatch
{ private static bool Prefix(Plant __instance) => !DoomShooters.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class DoomShooterFirePatch
{
    private static bool Prefix(Plant __instance) => !DoomShooters.Owns(FusionPlant.From(__instance))
        || DoomShooters.Firing?.Pointer == __instance.Pointer;
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class DoomShooterProjectilePatch
{
    private static void Postfix(Projectile __instance)
    {
        if (DoomShooters.Firing is { } plant && plant.mBoard?.Pointer == __instance.mBoard?.Pointer
            && __instance.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea)
            DoomShooters.Created = __instance;
    }
}
