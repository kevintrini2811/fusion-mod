using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class Jawbreaker
{
    internal sealed record Return(Board Board, uint Id, int Col, int Row);
    internal static Return? Capture(Plant plant, bool runOriginal)
    {
        if (!runOriginal || FusionPlant.From(plant) != FusionPlant.Jawbreaker || plant.mLaunchCounter != 1
            || !CherryBombPlants.CanTriggerDeath(plant)
            || SolarSquash.ShovelingBoard == plant.mBoard.Pointer) return null;
        var board = plant.mBoard;
        int col = board.PixelToGridX(plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight / 2f);
        // Actual ChomperSquash.DieEvent excludes the source's final column.
        return col >= 0 && col < 8 ? new(board, plant.DataID, col, plant.mRow) : null;
    }
    internal static void OnDeath(Return saved)
    {
        if (FusionService.CheckNativePlacement(saved.Board, saved.Col, saved.Row, SeedType.Squash) != PlantingReason.Ok) return;
        // Explicit marker prevents player AddPlant interception from fusing again.
        var identity = FusionPlant.Jawbreaker;
        saved.Board.AddPlant(saved.Col, saved.Row, identity.NativeType, identity.Marker);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class JawbreakerInitializePatch
{
    internal static void Postfix(Plant __instance)
    {
        if (FusionPlant.From(__instance) == FusionPlant.Jawbreaker) __instance.mLaunchCounter = 0;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class JawbreakerDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out Jawbreaker.Return? __state) =>
        __state = Jawbreaker.Capture(__instance, __runOriginal);
    internal static void Postfix(Plant __instance, Jawbreaker.Return? __state)
    {
        if (__state != null && __instance.DataID == __state.Id && __instance.mDead
            && __instance.mBoard?.Pointer == __state.Board.Pointer) Jawbreaker.OnDeath(__state);
    }
}
