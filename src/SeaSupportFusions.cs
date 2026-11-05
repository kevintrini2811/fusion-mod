using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Sea-shroom owns terrain, sleep/coffee and serialization. These variants
// replace native ability/shooting writers; animation events are visual only.
internal static class SeaSupportFusions
{
    internal const int FeedTicks = 100, ActivateTicks = 84, SwellTicks = 177, ShotTicks = 50;
    internal static bool Low(Plant plant) => FusionPlant.From(plant) == FusionPlant.BeaconShroom
        || FusionPlant.From(plant) == FusionPlant.SwellShroom
        || FusionPlant.From(plant) == FusionPlant.HermitShroom && plant.mTargetX == 0;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsSeaSupport(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = ShotTicks;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = plant.mTargetY = 0;
        plant.mDoSpecialCountdown = 0;
        plant.mTargetX = identity == FusionPlant.BeaconShroom ? 3000 : 0;
    }

    internal static bool CanFeed(Plant plant, FusionPlant incoming) => incoming == (FusionPlant)SeedType.Seashroom
        && SeaFusions.Active(plant) && (FusionPlant.From(plant) == FusionPlant.JellyShroom
            || FusionPlant.From(plant) == FusionPlant.SwellShroom
            || FusionPlant.From(plant) == FusionPlant.HermitShroom);

    internal static void Feed(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        // Commit before callbacks. A new feed restarts the original action;
        // it never initializes the recipient or resets its health.
        if (identity == FusionPlant.HermitShroom)
        {
            plant.mTargetX = 1000;
            plant.mShootingCounter = 1; // sea, activation, active, withdrawal
            plant.mStartRow = 0;
        }
        plant.mTargetY = identity == FusionPlant.SwellShroom ? SwellTicks : FeedTicks;
    }

    internal static void Fertilize(Plant plant)
    {
        // Fertilize.Upgrade caps positive AttributeCountdown at .5 seconds.
        // This also shortens Hermit's current courage; it grants no new buff.
        if (FusionPlant.IsSeaSupport(FusionPlant.From(plant)) && plant.mTargetX > 50) plant.mTargetX = 50;
    }

    internal static void Tick(Plant plant)
    {
        if (!SeaFusions.Active(plant)) return;
        var identity = FusionPlant.From(plant);
        if (plant.mTargetX > 0 && --plant.mTargetX == 0)
        {
            if (identity == FusionPlant.BeaconShroom)
            { plant.mShootingCounter = 1; plant.mTargetY = 100; }
            else if (identity == FusionPlant.HermitShroom)
            { plant.mShootingCounter = 4; plant.mTargetY = ActivateTicks; plant.mStartRow = 0; }
        }
        if (plant.mTargetY > 0)
        {
            --plant.mTargetY;
            // Source event times: Jelly .833333s, Swell 1.4s, sea splash .75s.
            if (identity == FusionPlant.JellyShroom && plant.mTargetY == 16) Summon(plant);
            else if (identity == FusionPlant.SwellShroom && plant.mTargetY == 37) Freeze(plant);
            else if (identity == FusionPlant.HermitShroom && plant.mShootingCounter == 1
                && plant.mTargetY == 25)
                plant.mBoard.mApp.AddTodParticle(plant.mX + plant.mWidth / 2f,
                    plant.mY + plant.mHeight / 2f, plant.mRenderOrder, ParticleEffect.PoolSplash);
            if (identity == FusionPlant.HermitShroom && plant.mTargetY == 0)
            {
                if (plant.mShootingCounter == 1)
                { plant.mShootingCounter = 2; plant.mTargetY = ActivateTicks; }
                else if (plant.mShootingCounter == 2) plant.mShootingCounter = 3;
                else if (plant.mShootingCounter == 4) plant.mShootingCounter = 0;
            }
            return;
        }
        if (identity != FusionPlant.HermitShroom || plant.mShootingCounter != 3) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow > 0)
        {
            if (--plant.mStartRow == 20) SeaProjectiles.Spawn(plant, 1, 0);
            if (plant.mStartRow > 0) return;
        }
        if (plant.mLaunchCounter == 0 && SeaFusions.Target(plant, false) != null)
        { plant.mStartRow = ShotTicks; plant.mLaunchCounter = ShotTicks; }
    }

    internal static void Summon(Plant plant)
    {
        var zombie = plant.mBoard.AddZombieAtCell(ZombieType.Normal, plant.mPlantCol, plant.mRow);
        if (zombie == null || zombie.mDead)
            throw new InvalidOperationException("Jelly-shroom could not allocate its summoned zombie.");
        // Native ordinary zombies in pool rows load the Ducky Tube artwork
        // and enter water through CheckForPool. The DuckyTube enum is not
        // eligible for that native pool state machine. Native initialization
        // and mind control own controller, team, wave accounting and saving.
        HypnoPlants.MindControl(zombie);
    }

    internal static void Freeze(Plant plant)
    {
        var board = plant.mBoard;
        float center = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || Math.Abs(zombie.mRow - plant.mRow) > 1
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Dog)) continue;
            var rect = zombie.GetZombieRect();
            float dx = center - Math.Clamp(center, rect.xMin, rect.xMax);
            float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= 4 * SeaFusions.Unit * SeaFusions.Unit)
                targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie,
                DamageRangeFlags.Ground | DamageRangeFlags.Dog)) continue;
            zombie.TakeDamage(20, DamageFlags.BypassesShield);
            if (zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer)
                IceInstantFusions.Freeze(zombie, 400);
        }
        board.mApp.AddTodParticle(center, plant.mY + plant.mHeight / 2f,
            plant.mRenderOrder, ParticleEffect.IceSparkle);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Frozen);
    }

    internal static bool Bite(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.JellyShroom || !SeaFusions.Active(plant)) return false;
        if (zombie.mBoard?.Pointer != plant.mBoard.Pointer || zombie.mDead || zombie.IsDeadOrDying()
            || zombie.mMindControlled) return false;
        if (!HypnoPlants.MindControl(zombie)) return false;
        PlayerPlantingScope.Consume(plant);
        return true;
    }

    internal static void Illuminate(Board board)
    {
        if (!board.StageHasFog() || board.mPaused || board.mApp.GameScene != GameScenes.Playing) return;
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant == null || FusionPlant.From(plant) != FusionPlant.BeaconShroom && FusionPlant.From(plant) != FusionPlant.JackOPlantern && FusionPlant.From(plant) != FusionPlant.IllumiNut
                || !SeaFusions.Active(plant)) continue;
            var fog = board.mGridCelFog;
            if (fog == null) continue;
            var array = fog.Cast<Il2CppSystem.Array>();
            PotFusions.FadeFog(fog.Pointer, array.GetLength(0), array.GetLength(1), plant.mPlantCol, plant.mRow,
                FusionPlant.From(plant) == FusionPlant.BeaconShroom && plant.mShootingCounter != 0 ? 2 : 1);
        }
    }


}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SeaSupportInitializePatch
{ private static void Postfix(Plant __instance) => SeaSupportFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SeaSupportAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsSeaSupport(FusionPlant.From(__instance))) return true;
        SeaSupportFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class SeaSupportShootingPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsSeaSupport(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SeaSupportFirePatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsSeaSupport(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class JellyBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || thePlant == null || !SeaSupportFusions.Bite(thePlant, __instance);
}
[HarmonyPatch(typeof(Board), "UpdateFog")]
internal static class SeaSupportFogPatch
{ private static void Postfix(Board __instance) => SeaSupportFusions.Illuminate(__instance); }
