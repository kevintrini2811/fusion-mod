using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Puff owns sleep, terrain, coffee, carrying and saving. Only these variants'
// shooter and reproduction counters replace the native spore shooter.
internal static class PuffSpecials
{
    internal const int ShootTicks = 50;
    internal const int IcicleShot = -470018, NuggetShot = -470092;
    internal const float Range = 2.5f * ScaredyFusions.SourceUnit;

    internal static bool IsIcicle(Projectile shot) => shot.mProjectileType == ProjectileType.Snowpea
        && shot.mCobTargetX == IcicleShot;
    internal static bool IsNugget(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mCobTargetX == NuggetShot;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsPuffSpecial(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = identity == FusionPlant.NuggetShroom ? 200 : 225;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = 0;
        plant.mTargetX = identity == FusionPlant.NuggetShroom ? ReproductionDelay(plant.mBoard) : 0;
    }

    internal static int ReproductionDelay(Board board) => 1900 + board.Rand(201);

    internal static bool HasTarget(Plant plant, FusionPlant identity)
    {
        // Native Starfruit has the same five rays and already owns their
        // geometry, eligible damage categories and board limits.
        if (identity == FusionPlant.TwinkleShroom) return plant.FindStarFruitTarget();
        float x = plant.mX + plant.mWidth / 2f;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(plant.mBoard, zombie, plant.GetDamageRangeFlags(PlantWeapon.Primary))) continue;
            var rect = zombie.GetZombieRect();
            float center = (rect.xMin + rect.xMax) / 2;
            // IcePuff inherits PeaSmallPuff, whose search covers the row.
            if (center > x && (identity == FusionPlant.IcicleShroom || center < x + Range)) return true;
        }
        return false;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.HailShroom) return; // Ability tick owns contact.
        if (identity == FusionPlant.NuggetShroom && plant.mTargetX > 0 && --plant.mTargetX == 0)
        {
            // Actual PotatoPuff.AttributeEvent rearms after success and stops
            // after failed placement. Children receive their own fresh timer.
            if (ShroomSlots.TrySpawn(plant.mBoard, identity, plant.mPlantCol, plant.mRow))
                plant.mTargetX = ReproductionDelay(plant.mBoard);
        }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || !HasTarget(plant, identity)) return;
            plant.mStartRow = ShootTicks;
            plant.mLaunchCounter = identity == FusionPlant.NuggetShroom
                ? 190 + plant.mBoard.Rand(21) : 214 + plant.mBoard.Rand(23);
        }
        int elapsed = ShootTicks - plant.mStartRow;
        if (elapsed == (identity == FusionPlant.TwinkleShroom ? 25 : 38))
        {
            if (identity == FusionPlant.TwinkleShroom)
                RetaliationProjectiles.Stars(plant.mBoard, plant.mX + plant.mWidth / 2f,
                    plant.mY + plant.mHeight / 2f, plant.mRenderOrder, plant.mRow);
            else Fire(plant, identity == FusionPlant.IcicleShroom);
        }
        --plant.mStartRow;
    }

    internal static void ContactFreeze(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            // HasBuff(Freeze) excludes an already frozen contact. SmallIceShroom
            // consumes itself only for the selected live, same-row ground foe.
            if (zombie == null || zombie.mIceTrapCounter > 0
                || !PuffSupport.GroundEnemy(plant.mBoard, zombie, plant.mRow)) continue;
            // Source colliders meet at ground height. Native ZombieRect is a
            // body rectangle, so small-plant subfield Y is not its ground Y.
            var rect = zombie.GetZombieRect();
            float x = plant.mX + plant.mWidth / 2f;
            if (Math.Abs(x - Math.Clamp(x, rect.xMin, rect.xMax)) > PuffSupport.BoomContactHalfWidth) continue;
            FreezeContact(plant, zombie);
            return;
        }
    }

    internal static bool FreezeContact(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.HailShroom || !ChomperFusions.Active(plant) || plant.mIsAsleep
            || zombie.mIceTrapCounter > 0 || !PuffSupport.GroundEnemy(plant.mBoard, zombie, plant.mRow)) return false;
        IceInstantFusions.Freeze(zombie, 400);
        plant.mBoard.mApp.AddTodParticle(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f, plant.mRenderOrder, ParticleEffect.IceSparkle);
        plant.mBoard.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.Frozen);
        plant.Die();
        return true;
    }

    private static void Fire(Plant plant, bool ice)
    {
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f
                + (ice ? .3f : .1f) * ScaredyFusions.SourceUnit,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, plant.mRow,
            ice ? ProjectileType.Snowpea : ProjectileType.Pea);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = ice ? IcicleShot : NuggetShot;
        shot.mMotionType = ProjectileMotion.Star;
        shot.mVelX = PuffShooters.Speed;
        shot.mVelY = shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
        shot.mDamageRangeFlags = plant.GetDamageRangeFlags(PlantWeapon.Primary);
        ImportedPeaVisuals.EnsurePuffSpecialProjectile(shot);
    }

    internal static void OnDeath(CherryBombPlants.Death death) =>
        MineFusions.Blast(death.Board, death.X, death.Y, ScaredyFusions.SourceUnit,
            300, death.Order, row: death.Row);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PuffSpecialInitializePatch
{
    internal static void Postfix(Plant __instance) => PuffSpecials.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class NuggetDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.NuggetShroom
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            ? new(__instance.mBoard, FusionPlant.NuggetShroom, __instance.DataID,
                __instance.mPlantCol, __instance.mRow, __instance.mX + __instance.mWidth / 2f,
                __instance.mY + __instance.mHeight / 2f, __instance.mRenderOrder) : null;
    }
    internal static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer) PuffSpecials.OnDeath(__state);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class PuffSpecialTorchwoodPatch
{
    internal static bool Prefix(Projectile __instance) => !PuffSpecials.IsIcicle(__instance) && !PuffSpecials.IsNugget(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class PuffSpecialProjectilePatch
{
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && (PuffSpecials.IsIcicle(__instance) || PuffSpecials.IsNugget(__instance)))
            ImportedPeaVisuals.EnsurePuffSpecialProjectile(__instance);
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class HailBiteContactPatch
{
    [HarmonyPriority(Priority.First + 200)]
    internal static bool Prefix(Zombie __instance, Plant thePlant) =>
        thePlant == null || !PuffSpecials.FreezeContact(thePlant, __instance);
}
