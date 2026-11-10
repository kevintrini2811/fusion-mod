using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native saved potato states own arming/rearming. Native kelp owns its victim
// DataID, drag-under, loot and disposal; no parallel victim registry exists.
internal static class UtilityMines
{
    internal static bool Managed(FusionPlant identity) => identity == FusionPlant.TangleMine
        || identity == FusionPlant.PotatoMasher;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!Managed(identity) && identity != FusionPlant.KelpMine) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = plant.mTargetX = plant.mTargetY = 0;
        plant.mState = identity == FusionPlant.PotatoMasher ? PlantState.PotatoArmed : PlantState.Notready;
        plant.mStateCountdown = identity == FusionPlant.TangleMine ? 1400 + plant.mBoard.Rand(201) : 0;
    }

    internal static void Fertilize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if ((identity == FusionPlant.LandMine || identity == FusionPlant.SeaMine)
            && plant.mStartRow < 2 && plant.mStateCountdown > 50) plant.mStateCountdown = 50;
        // These source subclasses have no fertilizer switch case. The generic
        // AttributeCountdown cap shortens burial; it does not skip rising.
        if (Managed(identity) && plant.mState == PlantState.Notready && plant.mStateCountdown > 50)
            plant.mStateCountdown = 50;
    }

    internal static Zombie? Target(Plant plant, bool grab)
    {
        float center = plant.mX + plant.mWidth / 2f;
        Zombie? best = null;
        float distance = float.MaxValue;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(plant.mBoard, zombie, MineFusions.MineTargets)
                || grab && zombie.mZombieType == ZombieType.Boss) continue;
            var rect = zombie.GetZombieRect();
            float left = center - 40, right = center + 80;
            if (rect.xMax < left || rect.xMin > right) continue;
            float next = Math.Abs((rect.xMin + rect.xMax) / 2 - center);
            if (next < distance) { best = zombie; distance = next; }
        }
        return best;
    }

    internal static void Tick(Plant plant)
    {
        if (!MineFusions.Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(UtilityMines))) return;
        var identity = FusionPlant.From(plant);
        // Native common UpdateAbilities normally decrements this countdown.
        if (plant.mStateCountdown > 0) --plant.mStateCountdown;
        if (plant.mState == PlantState.TanglekelpGrabbing)
        {
            plant.UpdateTanglekelp();
            return;
        }
        if (plant.mState == PlantState.Notready && plant.mStateCountdown == 0)
        { plant.mState = PlantState.PotatoRising; plant.mStateCountdown = identity == FusionPlant.PotatoMasher ? 50 : MineFusions.RiseTicks; }
        else if (plant.mState == PlantState.PotatoRising && plant.mStateCountdown == 0)
            plant.mState = PlantState.PotatoArmed;
        if (plant.mState != PlantState.PotatoArmed) return;
        if (identity == FusionPlant.TangleMine)
        {
            if (plant.mShootingCounter > 0) --plant.mShootingCounter;
            var nearby = MineFusions.Nearest(plant, false);
            float distance = nearby == null ? 1200 : nearby.mPosX - plant.mX - plant.mWidth / 2f;
            if (++plant.mLaunchCounter > MineFusions.FlashInterval(distance))
            { plant.mLaunchCounter = 0; plant.mShootingCounter = 8; }
            // UpdateTanglekelp performs native acquisition, drag and cleanup.
            // Our FindTargetZombie prefix supplies land rather than pool geometry.
            plant.UpdateTanglekelp();
            return;
        }
        if (Target(plant, false) == null) return;
        var context = PlayerPlantingScope.Current;
        try
        {
            PlayerPlantingScope.Current = null;
            var child = plant.mBoard.AddPlant(plant.mPlantCol, plant.mRow,
                FusionPlant.PotatoMash.NativeType, FusionPlant.PotatoMash.Marker);
            if (child == null || child.mDead || !child.mIsOnBoard) return;
            // Commit rearming only after allocation. Underlay and pumpkin stay.
            plant.mState = PlantState.Notready;
            plant.mStateCountdown = 1500;
        }
        finally { PlayerPlantingScope.Current = context; }
    }

    internal sealed record DeathState(CherryBombPlants.Death Death, uint VictimId, bool Detonate);

    internal static DeathState? Capture(Plant plant, bool runOriginal)
    {
        var identity = FusionPlant.From(plant);
        if (!runOriginal || !CherryBombPlants.CanTriggerDeath(plant)
            || identity != FusionPlant.TangleMine && identity != FusionPlant.KelpMine) return null;
        return new(new(plant.mBoard, identity, plant.DataID, plant.mPlantCol, plant.mRow,
            plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight / 2f, plant.mRenderOrder),
            identity == FusionPlant.TangleMine ? unchecked((uint)plant.mTargetZombieID) : 0,
            identity == FusionPlant.KelpMine || plant.mState == PlantState.TanglekelpGrabbing && plant.mStateCountdown == 0);
    }

    internal static void Death(DeathState state)
    {
        var death = state.Death;
        if (death.Identity == FusionPlant.TangleMine)
        {
            // A premature death releases/finishes the reserved victim as in
            // KelpMine_land.DieEventMustExecute, but creates no mine explosion.
            var victim = death.Board.m_zombies;
            for (int i = 0; i < victim.Count; ++i)
                if (victim[i] is { mDead: false } zombie && zombie.DataID == state.VictimId)
                { zombie.DieWithLoot(); break; }
            if (!state.Detonate) return; // interrupted grab, not AnimKill
        }
        MineFusions.Blast(death.Board, death.X, death.Y,
            death.Identity == FusionPlant.KelpMine ? 1.5f * ScaredyFusions.SourceUnit : ScaredyFusions.SourceUnit,
            death.Identity == FusionPlant.KelpMine ? 600 : 1800, death.Order,
            row: death.Row, rowRange: death.Identity == FusionPlant.KelpMine ? 1 : 0);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class UtilityMineInitializePatch
{ private static void Postfix(Plant __instance) => UtilityMines.Initialize(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class UtilityMineAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!UtilityMines.Managed(FusionPlant.From(__instance))) return true;
        UtilityMines.Tick(__instance); return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdatePotato))]
internal static class UtilityMinePotatoPatch
{ private static bool Prefix(Plant __instance) => !UtilityMines.Managed(FusionPlant.From(__instance)); }

[HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
internal static class TangleMineTargetPatch
{
    private static bool Prefix(Plant __instance, ref Zombie? __result)
    {
        var identity = FusionPlant.From(__instance);
        if (identity != FusionPlant.TangleMine && identity != FusionPlant.KelpMine) return true;
        __result = UtilityMines.Target(__instance, true); return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class UtilityMineShootingPatch
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        return !UtilityMines.Managed(identity) && identity != FusionPlant.KelpMine && identity != FusionPlant.SeaMine;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class UtilityMineFirePatch
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        return !UtilityMines.Managed(identity) && identity != FusionPlant.KelpMine && identity != FusionPlant.SeaMine;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class UtilityMineDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out UtilityMines.DeathState? __state) =>
        __state = UtilityMines.Capture(__instance, __runOriginal);
    internal static void Postfix(Plant __instance, UtilityMines.DeathState? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Death.Id
            && __instance.mBoard?.Pointer == __state.Death.Board.Pointer) UtilityMines.Death(__state);
    }
}
