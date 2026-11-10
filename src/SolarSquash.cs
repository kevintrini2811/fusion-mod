using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class SolarSquash
{
    internal sealed record Death(Board Board, uint Id, int Col, int Row,
        float X, float Y, int Generation, bool Shoveled);
    [ThreadStatic] internal static IntPtr ShovelingBoard;

    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.SolarSquash)
            // Native Squash uses mStartRow for its trajectory. Its unused,
            // serialized launch counter owns the increasing return cost.
            plant.mLaunchCounter = 1;
    }

    internal static Death? Capture(Plant plant, bool runOriginal)
    {
        if (!runOriginal || FusionPlant.From(plant) != FusionPlant.SolarSquash
            || !CherryBombPlants.CanTriggerDeath(plant)) return null;
        var board = plant.mBoard;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        return new(board, plant.DataID, board.PixelToGridX(x, y), plant.mRow,
            x, y, Math.Max(1, plant.mLaunchCounter), ShovelingBoard == board.Pointer);
    }

    internal static void OnDeath(Death death)
    {
        var board = death.Board;
        // Awake starts at 1; DieEvent uses 25 * (generation + 1).
        int cost = (int)Math.Min(int.MaxValue, 25L * (death.Generation + 1L));
        if (!death.Shoveled && death.Col >= 0 && death.Col < 9 && death.Row >= 0
            && death.Row < board.GetNumRows()
            && FusionService.CheckNativePlacement(board, death.Col, death.Row, SeedType.Squash) == PlantingReason.Ok
            && board.TakeSunMoney(cost, 0))
        {
            bool created = false;
            try
            {
                var identity = FusionPlant.SolarSquash;
                var plant = board.AddPlant(death.Col, death.Row, identity.NativeType, identity.Marker);
                if (plant != null && !plant.mDead && plant.mIsOnBoard)
                {
                    plant.mLaunchCounter = death.Generation == int.MaxValue ? int.MaxValue : death.Generation + 1;
                    created = true;
                    return;
                }
            }
            finally
            {
                if (!created) board.AddSunMoney(cost, 0);
            }
        }
        // The original failure branch emits two ordinary sun coins, including
        // shovel removal. It does not invent an extra return attempt.
        board.AddCoin(death.X, death.Y, CoinType.Sun, CoinMotion.FromPlant);
        board.AddCoin(death.X, death.Y, CoinType.Sun, CoinMotion.FromPlant);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SolarSquashInitializePatch
{
    internal static void Postfix(Plant __instance) => SolarSquash.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class SolarSquashDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out SolarSquash.Death? __state) =>
        __state = SolarSquash.Capture(__instance, __runOriginal);

    internal static void Postfix(Plant __instance, SolarSquash.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer) SolarSquash.OnDeath(__state);
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.MouseDownWithTool))]
internal static class SolarSquashShovelScope
{
    internal static void Prefix(Board __instance, CursorType theCursorType, out IntPtr __state)
    {
        __state = SolarSquash.ShovelingBoard;
        if (theCursorType == CursorType.Shovel) SolarSquash.ShovelingBoard = __instance.Pointer;
    }
    internal static void Finalizer(IntPtr __state) => SolarSquash.ShovelingBoard = __state;
}
