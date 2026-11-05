using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Plant owns the persisted identity and counters. No wall clocks or animation
// callbacks produce gameplay. Native mushroom, terrain and coffee rules remain.
internal static class IceInstantFusions
{
    internal const float CherryRadius = 2f / 1.35f * 80;
    private static bool Active(Plant plant) => FumeFusions.Active(plant);

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsIceInstant(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mDoSpecialCountdown = 0;
        plant.mStartRow = plant.mTargetX = plant.mTargetY = 0;
        // These types do not use native shooting. mLaunchCounter and mStartRow
        // serialize; the replacement ability path below is their sole writer.
        plant.mLaunchCounter = identity == FusionPlant.BlackCherry ? 150
            : identity == FusionPlant.DemiseShroom ? 96
            : identity == FusionPlant.SundaeShroom ? 400 + plant.mBoard.Rand(301) : 0;
    }

    internal static CherryBombPlants.Death Capture(Plant plant) => new(plant.mBoard,
        FusionPlant.From(plant), plant.DataID, plant.mPlantCol, plant.mRow,
        plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight / 2f, plant.mRenderOrder,
        plant.mTargetX);

    internal static void OnIceFusion(Plant plant)
    {
        // Use the native global freeze, including board and pool state, and
        // existing direct-IceZombies hooks for healing. Never replay on load.
        if (PumpkinSupport.StoreIce(plant.mBoard, plant.mPlantCol, plant.mRow)) return;
        plant.IceZombies();
        plant.mBoard.mApp.AddTodParticle(400, 300, 400000, ParticleEffect.IceSparkle);
        plant.mBoard.mApp.SoundSystem.PlayFoley(FoleyType.Frozen);
    }

    internal static void OnFusion(Plant plant)
    {
        // DoomCherry.Start counts damaged zombies, including surviving bosses
        // and armored targets. Loading must not replay this first explosion.
        plant.mTargetX = CherryBombPlants.Explode(Capture(plant), sourceRadius: CherryRadius);
    }

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsIceInstant(identity) || !Active(plant)) return;
        if (identity == FusionPlant.FrozenCherry) return;
        if (identity == FusionPlant.SundaeShroom)
        {
            if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
            // Original SunBright rises from 1 to 4 in .02 steps per physics
            // frame, then falls in .1 steps. Source physics runs at 50 Hz.
            if (plant.mTargetY > 0 && --plant.mTargetY == 60) Produce(plant);
            if (!plant.mDead && plant.mTargetY == 0 && plant.mLaunchCounter == 0)
            {
                plant.mTargetY = 360;
                plant.mLaunchCounter = 2375 + plant.mBoard.Rand(251);
            }
            return;
        }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mLaunchCounter > 0) return;
        {
            var death = Capture(plant);
            // Commit before death/damage callbacks can reenter this ability.
            plant.mStartRow = 1;
            plant.Die();
            if (plant.DataID != death.Id || !plant.mDead) return;
            if (identity == FusionPlant.BlackCherry) return; // Death callback owns this variant.
            bool demise = identity == FusionPlant.DemiseShroom;
            DoomEffects.Damage(death.Board, death.Row, death.X, death.Y, death.Order,
                damage: demise ? 1800 : 1800 + 300 * death.Stage,
                global: demise, freezeTicks: demise ? 500 : 0);
            var crater = DoomEffects.LeaveCrater(death.Board, death.Col, death.Row);
            if (demise && crater != null)
            {
                // Craters never read/write seed or sun in native simulation.
                // Both fields serialize unconditionally. This original crater
                // also carries the ten-second level freeze across save/load.
                crater.mSeedType = SeedType.Iceshroom;
                crater.mSunCount = 1000;
            }
        }
    }

    private static void Produce(Plant plant)
    {
        var board = plant.mBoard;
        board.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
        // Source checks the frozen state before applying this production's ice.
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie != null && Eligible(board, zombie) && zombie.mIceTrapCounter > 0)
                SolarShooter.DropSun(board, zombie.mPosX, zombie.mPosY, 15);
        }
        ++plant.mStartRow;
        bool last = plant.mStartRow >= 3;
        Ice(board, last ? 400 : 0, plant.mPlantCol, plant.mRow);
        if (last) plant.Die();
    }

    internal static float Brightness(Plant plant) => plant.mTargetY <= 0 ? 1
        : plant.mTargetY > 60 ? 1 + (360 - plant.mTargetY) * .01f : 1 + plant.mTargetY * .05f;

    internal static bool Eligible(Board board, Zombie zombie) =>
        SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Flying
            | DamageRangeFlags.Submerged | DamageRangeFlags.Dog | DamageRangeFlags.OffGround
            | DamageRangeFlags.Underground);

    internal static void Freeze(Zombie zombie, int ticks)
    {
        if (zombie.mDead || zombie.IsDeadOrDying()) return;
        int previous = zombie.mIceTrapCounter;
        var old = SnowPea.CreatingIceTrap;
        SnowPea.CreatingIceTrap = zombie.Pointer;
        try { zombie.HitIceTrap(); }
        finally { SnowPea.CreatingIceTrap = old; }
        if (zombie.CanBeFrozen() && zombie.mIceTrapCounter > 0)
            zombie.mIceTrapCounter = Math.Max(previous, ticks);
    }

    internal static bool Ice(Board board, int freezeTicks, int col = -1, int row = -1)
    {
        if (col >= 0 && PumpkinSupport.StoreIce(board, col, row)) return false;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie != null && Eligible(board, zombie)) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !Eligible(board, zombie)) continue;
            if (freezeTicks > 0) Freeze(zombie, freezeTicks);
            else zombie.ApplyChill(false);
            zombie.TakeDamage(20, DamageFlags.BypassesShield);
        }
        board.mApp.AddTodParticle(400, 300, 400000, ParticleEffect.IceSparkle);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Frozen);
        return true;
    }

    internal static bool Absorb(Projectile projectile)
    {
        var board = projectile.mBoard;
        if (board == null || board.mPaused || projectile.mDead || projectile.mPosZ != 0
            || !CherryShooter.TryShot(projectile)) return false;
        // Source IceCherry accepts friendly, level, same-row cherries. Its
        // trigger is 3.5300236 units wide at prefab scale .35, offset .2657588.
        const float offset = .26575875f * .35f / 1.35f * 80;
        const float halfWidth = 3.5300236f * .35f / 1.35f * 40;
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant == null || FusionPlant.From(plant) != FusionPlant.FrozenCherry
                || plant.mRow != projectile.mRow || !Active(plant)) continue;
            float center = plant.mX + plant.mWidth / 2f + offset;
            float next = projectile.mPosX + projectile.mVelX;
            if (Math.Max(projectile.mPosX, next) < center - halfWidth
                || Math.Min(projectile.mPosX, next) > center + halfWidth) continue;
            int energy = CherryShooter.IsExplosive(projectile) ? 10
                : projectile.mProjectileType == ProjectileType.Fireball ? 2 : 1;
            // Credit only after real removal. Die never impacts a zombie.
            uint id = projectile.DataID;
            projectile.Die();
            if (!projectile.mDead || projectile.DataID != id) return true;
            plant.mTargetX += energy;
            if (plant.mTargetX >= 100)
            {
                DropCherryCard(plant);
                // Original resets to zero, rather than retaining overflow.
                plant.mTargetX = 0;
            }
            return true;
        }
        return false;
    }

    private static void DropCherryCard(Plant plant)
    {
        var coin = plant.mBoard.AddCoin(plant.mX, plant.mY, CoinType.UsableSeedPacket, CoinMotion.FromPlant);
        if (coin == null) throw new InvalidOperationException("Frozen Cherry card could not be created.");
        coin.mUsableSeedType = SeedType.Cherrybomb;
        if (coin.mController != null) coin.mController.SeedType = SeedType.Cherrybomb;
    }

    internal static bool LevelFrozen(Board board)
    {
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item != null && !item.mDead && item.mGridItemType == GridItemType.Crater
                && item.mSeedType == SeedType.Iceshroom && item.mSunCount > 0) return true;
        }
        return false;
    }

    internal static void TickLevelFreeze(Board board)
    {
        if (board.mPaused || board.mApp == null || board.mApp.GameScene != GameScenes.Playing
            || board.Pointer == CherryBombPlants.DisposingBoard) return;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item != null && !item.mDead && item.mGridItemType == GridItemType.Crater
                && item.mSeedType == SeedType.Iceshroom && item.mSunCount > 0) --item.mSunCount;
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class IceInstantInitializePatch
{
    private static void Postfix(Plant __instance) => IceInstantFusions.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class IceInstantAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsIceInstant(FusionPlant.From(__instance))) return true;
        IceInstantFusions.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class IceInstantSpecialPatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsIceInstant(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class IceInstantNativeFirePatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsIceInstant(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class FrozenCherryDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && (FusionPlant.From(__instance) == FusionPlant.FrozenCherry
                || (FusionPlant.From(__instance) == FusionPlant.BlackCherry && __instance.mStartRow == 1))
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            ? IceInstantFusions.Capture(__instance) : null;
    }
    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state == null || !__instance.mDead || __instance.DataID != __state.Id) return;
        if (__state.Identity == FusionPlant.FrozenCherry)
            CherryBombPlants.Explode(__state, freezeTicks: 1000, sourceRadius: IceInstantFusions.CherryRadius);
        else
        {
            DoomEffects.Damage(__state.Board, __state.Row, __state.X, __state.Y, __state.Order,
                damage: 1800 + 300 * __state.Stage);
            DoomEffects.LeaveCrater(__state.Board, __state.Col, __state.Row);
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class BlackCherryCrushPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.BlackCherry
            || !CherryBombPlants.CanTriggerDeath(__instance) || __instance.NotOnGround()) return true;
        // Source Crashed triggers AttributeEvent. Ordinary removal (including
        // shoveling) is not a second timed/crash event.
        __instance.mStartRow = 1;
        __instance.Die();
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class FrozenCherryAbsorptionPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Projectile __instance) => !IceInstantFusions.Absorb(__instance);
}

[HarmonyPatch(typeof(Board), "UpdateZombieSpawning")]
internal static class DemiseLevelFreezePatch
{
    private static bool Prefix(Board __instance) => !IceInstantFusions.LevelFrozen(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class DemiseLevelFreezeTickPatch
{
    // Run before the native tick: a newly created crater starts at a full 1000.
    private static void Prefix(Board __instance) => IceInstantFusions.TickLevelFreeze(__instance);
}
