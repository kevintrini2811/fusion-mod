using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Each ability replaces its native seed's ability/shooting clocks. All counters
// below are native serialized fields, including the independently grown spore.
internal static class DoomFusions
{
    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsDoomFusion(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.DoomNut ? 4000 : 300;
        plant.mDoSpecialCountdown = plant.mStartRow = plant.mTargetX = plant.mTargetY = 0;
        plant.mLaunchCounter = identity == FusionPlant.Doomflower ? 5000
            : identity == FusionPlant.DoomSeed ? 3000 : 0;
        if (identity == FusionPlant.DoomSeed) plant.mIsAsleep = false;
        if (identity == FusionPlant.CraterTot)
        {
            plant.mState = PlantState.Notready;
            plant.mStateCountdown = 1400 + plant.mBoard.Rand(201);
        }
    }

    internal static void Fertilize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if ((identity == FusionPlant.Doomflower || identity == FusionPlant.DoomSeed)
            && plant.mLaunchCounter > 50) plant.mLaunchCounter = 50;
        if (identity == FusionPlant.CraterTot)
        { plant.mState = PlantState.PotatoArmed; plant.mStateCountdown = 0; }
    }

    internal static void Tick(Plant plant)
    {
        if (!MineFusions.Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(DoomFusions))) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.Doomflower)
        {
            if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
            plant.mLaunchCounter = 5000;
            if (plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless) return;
            plant.mBoard.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
            GrowSpore(plant);
        }
        else if (identity == FusionPlant.DoomSeed)
        {
            if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
            if (InDoomRange(plant)) Detonate(plant);
        }
        else if (identity == FusionPlant.CraterTot)
        {
            if (plant.mStateCountdown > 0) --plant.mStateCountdown;
            if (plant.mState == PlantState.Notready && plant.mStateCountdown == 0)
            { plant.mState = PlantState.PotatoRising; plant.mStateCountdown = 50; }
            else if (plant.mState == PlantState.PotatoRising && plant.mStateCountdown == 0)
                plant.mState = PlantState.PotatoArmed;
            if (plant.mState == PlantState.PotatoArmed && MineFusions.Nearest(plant, true) != null)
                Detonate(plant);
            if (plant.mTargetY > 0) --plant.mTargetY;
            if (++plant.mLaunchCounter >= 300)
            { plant.mLaunchCounter = 0; plant.mTargetY = 20; }
        }
    }

    private static bool InDoomRange(Plant plant)
    {
        float x = plant.mX + plant.mWidth / 2, y = plant.mY + plant.mHeight / 2;
        float radius = 4f / 1.35f * 80;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !IceInstantFusions.Eligible(plant.mBoard, zombie)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= radius * radius) return true;
        }
        return false;
    }

    internal static bool Empty(Board board, int col, int row, FusionPlant identity)
    {
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return false;
        var occupant = board.GetPlantsOnLawn(col, row).NormalPlant;
        return (occupant == null || occupant.mDead)
            && FusionService.CheckNativePlacement(board, col, row, identity.NativeType) == PlantingReason.Ok;
    }

    private static void GrowSpore(Plant parent)
    {
        var board = parent.mBoard;
        var cells = new List<(int Col, int Row)>();
        for (int col = parent.mPlantCol - 1; col <= parent.mPlantCol + 1; ++col)
        for (int row = parent.mRow - 1; row <= parent.mRow + 1; ++row)
            if (Empty(board, col, row, FusionPlant.DoomSeed)) cells.Add((col, row));
        while (cells.Count > 0)
        {
            int index = board.Rand(cells.Count);
            var (col, row) = cells[index]; cells.RemoveAt(index);
            if (Spawn(board, col, row, FusionPlant.DoomSeed) != null) break;
        }
    }

    internal static Plant? Spawn(Board board, int col, int row, FusionPlant identity)
    {
        if (!Empty(board, col, row, identity)) return null;
        var previous = PlayerPlantingScope.Current;
        PlayerPlantingScope.Current = null;
        try
        {
            var plant = board.AddPlant(col, row, identity.NativeType, identity.Marker);
            return plant != null && !plant.mDead && plant.mIsOnBoard ? plant : null;
        }
        finally { PlayerPlantingScope.Current = previous; }
    }

    internal static void Detonate(Plant plant)
    {
        if (plant.mStartRow != 0 || !MineFusions.Active(plant)) return;
        plant.mStartRow = 1; // Commit before Die and nested damage callbacks.
        var death = IceInstantFusions.Capture(plant);
        plant.Die();
        if (plant.DataID != death.Id) return;
        if (!plant.mDead) { plant.mStartRow = 0; return; }
        if (death.Identity == FusionPlant.CraterTot)
            MineFusions.Blast(death.Board, death.X, death.Y, 80, 1800, death.Order, ash: true);
        DoomEffects.Explode(death.Board, death.Col, death.Row, death.X, death.Y, death.Order);
        if (death.Identity == FusionPlant.CraterTot) PotatoSeedProjectiles.Scatter(death);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class DoomFusionInitializePatch
{ private static void Postfix(Plant __instance) => DoomFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class DoomFusionAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsDoomFusion(FusionPlant.From(__instance))) return true;
        DoomFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class DoomFusionShootingPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsDoomFusion(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class DoomFusionSpecialPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsDoomFusion(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class CraterTotCrushPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.CraterTot || !MineFusions.Active(__instance)) return true;
        DoomFusions.Detonate(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class DoomNutDeathPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Plant __instance, out CherryBombPlants.Death? __state)
    {
        __state = FusionPlant.From(__instance) == FusionPlant.DoomNut
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            && __instance.mStartRow == 0 ? IceInstantFusions.Capture(__instance) : null;
        // Native jack-in-the-box explosions call Die without numeric damage.
        if (FusionPlant.From(__instance) != FusionPlant.DoomNut || !MineFusions.Active(__instance)
            || PlayerPlantingScope.ConsumingIngredient == __instance.Pointer
            || RetaliationNuts.RadiusBoard != __instance.mBoard.Pointer
            || RetaliationNuts.JackBoard != __instance.mBoard.Pointer) return true;
        --__instance.mBoard.mPlantsEaten;
        return false;
    }
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Plant __instance, bool __runOriginal, CherryBombPlants.Death? __state)
    {
        if (!__runOriginal || __state == null || !__instance.mDead
            || __instance.DataID != __state.Id || __instance.mStartRow != 0) return;
        __instance.mStartRow = 1;
        DoomEffects.Explode(__state.Board, __state.Col, __state.Row, __state.X, __state.Y, __state.Order);
    }
}
