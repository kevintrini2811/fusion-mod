using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateReanimColor))]
internal static class FusionHighlightPatch
{
    private static bool IsFusionTarget(Plant plant)
    {
        var board = plant.mBoard;
        if (board == null || board.mPaused || plant.mDead || !plant.mIsOnBoard) return false;

        var carried = LawnGloveMod.GetCarriedPlant(board);
        if (carried != null)
            return FusionService.CanPlaceCarried(board, carried, plant.mPlantCol, plant.mRow)
                && (BucketPlants.CanFeed(plant, FusionPlant.From(carried))
                    || FusionService.ResolveAll(board, FusionPlant.From(carried), plant.mPlantCol, plant.mRow, carried)
                        .Any(plan => plan.Target.Pointer == plant.Pointer));

        if (BucketPlants.IsTarget(plant)) return true;

        var cursor = board.mCursorObject;
        // Match the implemented seed-card fusion path, including its Imitater
        // exclusion. An empty glove or another tool supplies no ingredient.
        return cursor != null && cursor.CursorType == CursorType.PlantFromBank
            && cursor.mImitaterType == SeedType.None
            && (SeaFusions.CanFeed(plant, cursor.mType) || FusionService.ResolveAll(board, cursor.mType, plant.mPlantCol, plant.mRow)
                .Any(plan => plan.Target.Pointer == plant.Pointer
                    && FusionService.CheckFusionPlacement(board, plant, plan.Result) == PlantingReason.Ok));
    }

    private static void Prefix(Plant __instance, out bool __state)
    {
        __state = __instance.mHighlighted;
        // Let native code choose exactly the shovel/glove hover color. Scope
        // our flag to this color update: no persistent selection state, no
        // clearing somebody else's hover, no stale flags in saves or on cancel.
        if (!__state && IsFusionTarget(__instance)) __instance.mHighlighted = true;
    }

    // Capture while the temporary fusion flag still applies. The finalizer
    // restores native state before the imported art updates later in the frame.
    private static void Postfix(Plant __instance) => ImportedPeaVisuals.CaptureHighlight(__instance);

    private static void Finalizer(Plant __instance, bool __state) => __instance.mHighlighted = __state;
}
