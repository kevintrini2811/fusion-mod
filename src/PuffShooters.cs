using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Puff-shroom owns terrain, small-plant targeting, coffee, carrying and saves.
// Replace its shooter only. Launch cooldown and original clip phase serialize.
internal static class PuffShooters
{
    // Native peas hit the ground below 28 pixels. Keep their simulated flight
    // above that boundary while preserving the small mushroom's muzzle pose.
    internal const float FlightHeight = 40;
    internal const float Speed = 6 * ScaredyFusions.SourceUnit / 100;
    internal const int Shot = -460010, RearShot = -460017, ReboundedShot = -460018;
    internal static int Duration(FusionPlant identity) => identity == FusionPlant.GatlingPeaShroom ? 125
        : identity == FusionPlant.SplitPeaShroom ? 72 : identity == FusionPlant.RepeatShroom ? 79 : 50;

    internal static bool Contains(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mCobTargetX is Shot or RearShot or ReboundedShot;

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsPuffShooter(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = FusionPlant.From(plant) == FusionPlant.BuckShroom ? 1000 : 300;
        plant.mLaunchRate = 225;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = 0;
    }

    internal static bool HasTarget(Plant plant, FusionPlant identity)
    {
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(plant.mBoard, zombie, plant.GetDamageRangeFlags(PlantWeapon.Primary))) continue;
            // PeaSmallPuff inherits the full-row Plant search; SplitPuff also
            // accepts enemies behind it. SmallPuff's short range is unrelated.
            if (identity == FusionPlant.SplitPeaShroom
                || zombie.GetZombieRect().xMax > plant.mX + plant.mWidth / 2f) return true;
        }
        return false;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(PuffShooters))) return;
        var identity = FusionPlant.From(plant);
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || !HasTarget(plant, identity)) return;
            plant.mStartRow = Duration(identity);
            // Actual PlantShootUpdate resets to interval * Random(.95, 1.05).
            plant.mLaunchCounter = 214 + plant.mBoard.Rand(23);
        }
        int elapsed = Duration(identity) - plant.mStartRow;
        if (identity == FusionPlant.GatlingPeaShroom)
        {
            if (elapsed is 50 or 70 or 90 or 110) Fire(plant, false);
        }
        else if (identity == FusionPlant.SplitPeaShroom)
        {
            if (elapsed == 38) { Fire(plant, false); Fire(plant, true); }
            if (elapsed == 63) Fire(plant, true);
        }
        else if (elapsed == 38 || identity == FusionPlant.RepeatShroom && elapsed == 67) Fire(plant, false);
        --plant.mStartRow;
    }

    private static void Fire(Plant plant, bool rear)
    {
        float x = plant.mX + plant.mWidth / 2f + (rear ? -10 : 15);
        var shot = plant.mBoard.AddProjectile(x, plant.mY + plant.mHeight / 2f - 10,
            plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
        if (shot == null || shot.mDead) return;
        bool iron = FusionPlant.From(plant) == FusionPlant.BuckShroom;
        shot.mCobTargetX = iron ? IronProjectiles.SmallShot : rear ? RearShot : Shot;
        // Native Star motion supplies saved arbitrary horizontal speed and
        // ordinary pea collision/damage. Only the left-edge rebound is custom.
        shot.mMotionType = ProjectileMotion.Star;
        shot.mVelX = rear ? -Speed : Speed;
        shot.mVelY = shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mShadowY = shot.mPosY + FlightHeight;
        shot.mDamageRangeFlags = plant.GetDamageRangeFlags(PlantWeapon.Primary);
        if (iron) ImportedPeaVisuals.EnsureIronProjectile(shot);
        else ImportedPeaVisuals.EnsurePuffProjectile(shot);
    }

    internal static void Rebound(Projectile shot)
    {
        if (!Contains(shot) || shot.mDead || shot.mBoard == null || shot.mBoard.mPaused) return;
        if (shot.mCobTargetX == RearShot && shot.mPosX <= 0)
        {
            shot.mPosX = -shot.mPosX;
            shot.mX = (int)shot.mPosX;
            shot.mVelX = FusionProjectileTime.SourceVelocity(Speed);
            shot.mCobTargetX = ReboundedShot;
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PuffInitializePatch
{
    internal static void Postfix(Plant __instance) => PuffShooters.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class PuffShooterPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (FusionPlant.IsPuffJumper(FusionPlant.From(__instance))) return false;
        if (FusionPlant.From(__instance) == FusionPlant.BallCactus)
        {
            PuffContactFusions.TickBall(__instance);
            return false;
        }
        if (FusionPlant.IsPuffLobber(FusionPlant.From(__instance)))
        {
            PuffLobbers.Tick(__instance);
            return false;
        }
        if (FusionPlant.IsPuffSpecial(FusionPlant.From(__instance)))
        {
            PuffSpecials.Tick(__instance);
            return false;
        }
        if (FusionPlant.IsPuffSupport(FusionPlant.From(__instance)))
        {
            PuffSupport.Tick(__instance);
            return false;
        }
        if (!FusionPlant.IsPuffShooter(FusionPlant.From(__instance))) return true;
        PuffShooters.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class PuffShootingPatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsPuffVariant(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class PuffFirePatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsPuffVariant(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class PuffProjectilePatch
{
    internal static void Prefix(Projectile __instance) => PuffShooters.Rebound(__instance);
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && PuffShooters.Contains(__instance)) ImportedPeaVisuals.EnsurePuffProjectile(__instance);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class PuffTorchwoodPatch
{
    // Original TorchWood accepts bullet 0/15; puff peas are bullet 10.
    internal static bool Prefix(Projectile __instance) => !PuffShooters.Contains(__instance);
}
