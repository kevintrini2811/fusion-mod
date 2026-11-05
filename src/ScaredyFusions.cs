using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Scaredy-shroom remains the terrain, coffee, damage and save adapter.
// Replace only its fear/shooter AI; keep UpdateAbilities' common lifecycle work.
internal static class ScaredyFusions
{
    internal const float SourceUnit = 80f / 1.35f;
    internal static bool Active(Plant plant) => ChomperFusions.Active(plant) && !plant.mIsAsleep;
    internal static bool Afraid(Plant plant) => plant.mState is PlantState.ScaredyshroomLowering
        or PlantState.ScaredyshroomScared;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsScaredy(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.SturdyShroom ? 4000 : 300;
        plant.mState = PlantState.Notready;
        plant.mLaunchCounter = plant.mStartRow = plant.mTargetY = 0;
        plant.mTargetX = identity == FusionPlant.SpuddyShroom ? 2000 : identity == FusionPlant.SunnyShroom ? 5000 : 0;
        if (identity == FusionPlant.FrenzyShroom) plant.mTargetX = 150;
        plant.mShootingCounter = 0;
        if (identity == FusionPlant.FrenzyShroom) plant.mShootingCounter = 20;
        if (identity == FusionPlant.SunnyShroom) plant.mLaunchCounter = 400 + plant.mBoard.Rand(301);
    }

    internal static bool Protected(Plant plant)
    {
        var shell = plant.mBoard.GetPlantsOnLawn(plant.mPlantCol, plant.mRow).PumpkinPlant;
        return shell != null && ChomperFusions.Active(shell) && FusionPlant.From(shell) == FusionPlant.ScarelessPumpkin;
    }

    internal static bool Near(Plant plant, Zombie zombie, float halfWidth = 1.25f)
        => !Protected(plant) && EnemyNear(plant, zombie, halfWidth);

    // Pot fear has the same enemy geometry, but Scareless Pumpkin protects
    // Scaredy-shroom descendants only, not the independent pot controller.
    internal static bool EnemyNear(Plant plant, Zombie zombie, float halfWidth = 1.25f)
    {
        if (zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            // Fear is a proximity query, not a weapon hit. Pool, airborne and
            // submerged phases must not make a living enemy disappear from it.
            || zombie.mBoard?.Pointer != plant.mBoard.Pointer || zombie.mDead
            || zombie.IsDeadOrDying() || zombie.mMindControlled
            || Math.Abs(zombie.mRow - plant.mRow) > 1) return false;
        float x = plant.mX + plant.mWidth / 2f;
        var rect = zombie.GetZombieRect();
        return rect.xMax >= x - halfWidth * SourceUnit && rect.xMin <= x + halfWidth * SourceUnit;
    }

    internal static void Tick(Plant plant)
    {
        if (!Active(plant)) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.FrenzyShroom) { FrenzyShroom.Tick(plant); return; }
        if (identity == FusionPlant.CamoShroom) return;
        if ((identity == FusionPlant.SpuddyShroom || identity == FusionPlant.HungryShroom) && plant.mTargetX > 0) --plant.mTargetX;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        bool near = false;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && Near(plant, zombie)) { near = true; break; }

        uint plantId = plant.DataID;
        if (identity == FusionPlant.SunnyShroom) ScaredySupport.ProduceTick(plant, near);
        if (identity == FusionPlant.SturdyShroom && ScaredySupport.SturdyTick(plant, near)) return;
        if (plant.DataID != plantId || !Active(plant)) return;
        if (near && !Afraid(plant) && (identity != FusionPlant.SturdyShroom || plant.mTargetX >= 0))
        {
            plant.mState = PlantState.ScaredyshroomLowering;
            plant.mTargetY = 100;
            plant.mStartRow = 0;
        }
        if (near && identity == FusionPlant.SpeedyShroom) plant.mTargetX = 0;
        if (!near && Afraid(plant))
        {
            plant.mState = PlantState.ScaredyshroomRaising;
            plant.mTargetY = GrowTicks(identity);
        }
        if (plant.mState == PlantState.ScaredyshroomLowering)
        {
            // Original scared clip emits AnimScaredy/AnimFreeze at 5/6 s.
            if (plant.mTargetY == 17) FearEffect(plant, identity);
            if (--plant.mTargetY <= 0) plant.mState = PlantState.ScaredyshroomScared;
            return;
        }
        if (plant.mState == PlantState.ScaredyshroomScared) return;
        if (plant.mState == PlantState.ScaredyshroomRaising)
        {
            if (--plant.mTargetY <= 0) plant.mState = PlantState.Notready;
            return;
        }
        if (identity == FusionPlant.SunnyShroom || identity == FusionPlant.SturdyShroom) return;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            bool target = false;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && zombie.mRow == plant.mRow
                    && FumeFusions.InRange(plant, zombie, FusionPlant.FumeSpreader)) { target = true; break; }
            if (!target && identity != FusionPlant.StarryShroom) return;
            plant.mStartRow = ShootTicks(identity);
        }
        // Every original shoot clip emits AnimShoot at 0.3 s.
        if (plant.mStartRow == (identity == FusionPlant.StarryShroom ? 17 : 20))
        {
            uint id = plant.DataID;
            Shoot(plant, identity);
            if (plant.mDead || plant.DataID != id) return;
        }
        --plant.mStartRow;
    }

    internal static int ShootTicks(FusionPlant identity) => identity == FusionPlant.StarryShroom ? 42 : 50;

    internal static int GrowTicks(FusionPlant identity) => identity == FusionPlant.SpeedyShroom
        || identity == FusionPlant.ShiveryShroom || identity == FusionPlant.SunnyShroom ? 102 : 100;

    private static void FearEffect(Plant plant, FusionPlant identity)
    {
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        if (identity == FusionPlant.HungryShroom) EatingShrooms.Fear(plant);
        if (identity == FusionPlant.BlastyShroom)
            CherryBombPlants.Explode(new(plant.mBoard, identity, plant.DataID, plant.mPlantCol,
                plant.mRow, x, y, plant.mRenderOrder), sourceRadius: 2 * SourceUnit, damage: 600);
        if (identity != FusionPlant.ShiveryShroom) return;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && Near(plant, zombie, 1.5f))
                targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (zombie.mDead || zombie.DataID != id || zombie.mBoard?.Pointer != plant.mBoard.Pointer) continue;
            IceInstantFusions.Freeze(zombie, 400);
            zombie.TakeDamage(20, DamageFlags.BypassesShield);
        }
    }

    private static void Shoot(Plant plant, FusionPlant identity)
    {
        if (identity == FusionPlant.HungryShroom) { EatingShrooms.ShootHungry(plant); return; }
        if (identity == FusionPlant.StarryShroom)
        {
            plant.mLaunchCounter = 50;
            RetaliationProjectiles.Stars(plant.mBoard, plant.mX + plant.mWidth / 2f,
                plant.mY + plant.mHeight / 2f, plant.mRenderOrder, plant.mRow);
            return;
        }
        int damage;
        if (identity == FusionPlant.SpeedyShroom)
        {
            plant.mTargetX = Math.Min(30, plant.mTargetX + 1);
            damage = 20 + plant.mTargetX;
            plant.mLaunchCounter = (int)MathF.Round(150 / (1 + .05f * plant.mTargetX)) - 30;
        }
        else
        {
            damage = identity == FusionPlant.SpuddyShroom ? plant.mTargetX > 0 ? 600 : 60 : 40;
            plant.mLaunchCounter = identity == FusionPlant.SpuddyShroom ? 270 : 120;
        }
        var muzzle = ImportedPeaVisuals.ScaredyMuzzle(plant);
        var shot = plant.mBoard.AddProjectile(muzzle.X + (identity == FusionPlant.ShiveryShroom ? .1f * SourceUnit : 0),
            muzzle.Y, plant.mRenderOrder + 1, plant.mRow, identity == FusionPlant.ShiveryShroom ? ProjectileType.Snowpea : ProjectileType.Pea);
        if (shot == null || shot.mDead) return;
        ScaredyProjectiles.Mark(shot, identity, damage);
        if (identity == FusionPlant.SpuddyShroom && plant.mTargetX > 0)
        {
            plant.mPlantHealth -= 75;
            if (plant.mPlantHealth <= 0) plant.Die();
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ScaredyInitializePatch
{
    internal static void Postfix(Plant __instance) => ScaredyFusions.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateScaredyShroom))]
internal static class ScaredyTickPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsScaredy(FusionPlant.From(__instance)))
        {
            if (__instance.mSeedType != SeedType.Scaredyshroom || !ScaredyFusions.Active(__instance)
                || !ScaredyFusions.Protected(__instance)) return true;
            // UpdateAbilities calls native UpdateShooter independently.
            if (ScaredyFusions.Afraid(__instance) || __instance.mState == PlantState.ScaredyshroomRaising)
                __instance.PlayIdleAnim(12);
            __instance.mState = PlantState.Ready;
            return false;
        }
        ScaredyFusions.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class ScaredyShooterPatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsScaredy(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class ScaredyShootingPatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsScaredy(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class ScaredyFirePatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsScaredy(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class SpuddyDeathPatch
{
    internal static void Prefix(Plant __instance, out CherryBombPlants.Death? __state)
    {
        __state = FusionPlant.From(__instance) == FusionPlant.SpuddyShroom && CherryBombPlants.CanTriggerDeath(__instance)
            ? new(__instance.mBoard, FusionPlant.SpuddyShroom, __instance.DataID, __instance.mPlantCol,
                __instance.mRow, __instance.mX + __instance.mWidth / 2f,
                __instance.mY + __instance.mHeight / 2f, __instance.mRenderOrder) : null;
    }
    internal static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id)
            MineFusions.Blast(__state.Board, __state.X, __state.Y, ScaredyFusions.SourceUnit, 600, __state.Order);
    }
}
