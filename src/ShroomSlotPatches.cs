using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

// Coffee's native DoSpecial sets one mushroom's saved 100-tick wake timer.
// Extend that successful action; UpdateAbilities still owns waking and audio.
[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class TileCoffeePatch
{
    private static void Prefix(Plant __instance, out bool __state) => __state =
        __instance.mSeedType == SeedType.InstantCoffee && !__instance.mDead
        && __instance.mState != PlantState.DoingSpecial && ChomperFusions.Active(__instance);

    private static void Postfix(Plant __instance, bool __state)
    {
        if (!__state || __instance.mState != PlantState.DoingSpecial) return;
        foreach (var plant in ShroomSlots.Plants(__instance.mBoard, __instance.mPlantCol, __instance.mRow))
            if (plant.mIsAsleep && plant.mWakeUpCounter <= 0) plant.mWakeUpCounter = 100;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
internal static class TileCoffeePlacementPatch
{
    private static void Postfix(Board __instance, int theGridX, int theGridY, SeedType theType,
        ref PlantingReason __result)
    {
        if (FusionService.CheckingNativePlacement || theType != SeedType.InstantCoffee
            || __result is not (PlantingReason.NotHere or PlantingReason.NeedsSleeping) || theGridX < 0 || theGridX >= 9
            || theGridY < 0 || theGridY >= __instance.GetNumRows()) return;
        if (ShroomSlots.Plants(__instance, theGridX, theGridY).Any(p => p.mIsAsleep && p.mWakeUpCounter <= 0))
            __result = PlantingReason.Ok;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.GetTopPlantAt))]
internal static class ShroomSelectionPatch
{
    internal static void Postfix(Board __instance, int theGridX, int theGridY, ref Plant __result)
    {
        int player = PlayerCursorScope.PlayerIndex(__instance);
        if (__instance.CursorObjects[player]?.CursorType != CursorType.Glove
            || (player == 0 && LawnGloveMod.GetCarriedPlant(__instance) != null)
            || theGridX < 0 || theGridX >= 9 || theGridY < 0 || theGridY >= __instance.GetNumRows()) return;
        var stack = __instance.GetPlantsOnLawn(theGridX, theGridY);
        if (stack.PumpkinPlant is { } shell && FusionPlant.IsPumpkinFusion(FusionPlant.From(shell)))
        { __result = shell; return; }
        var pot = stack.UnderPlant;
        if (pot != null && pot.mSeedType == SeedType.Flowerpot && ShroomSlots.Live(pot)
            && PlayerCursorScope.Position(__instance).Y >= __instance.GridToPixelY(theGridX, theGridY) + 80 * 2 / 3f)
        { __result = pot; return; }
        var plants = ShroomSlots.Plants(__instance, theGridX, theGridY);
        if (plants.Count == 0 || !plants.All(p => ShroomSlots.IsSmall(FusionPlant.From(p)))) return;
        int slot = ShroomSlots.Hover(__instance, theGridX, theGridY);
        // Empty subfields must not pick up another member of the cell.
        __result = plants.FirstOrDefault(p => ShroomSlots.Slot(p) == slot)!;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.ToolHitTest))]
internal static class ShroomGloveHoverPatch
{
    private static void Prefix(Board __instance, int x, int y, int playerIndex,
        out PlayerCursorScope.Context? __state)
    {
        __state = PlayerCursorScope.Current;
        PlayerCursorScope.Current = new PlayerCursorScope.Context(__instance, playerIndex, x, y);
    }

    private static void Finalizer(PlayerCursorScope.Context? __state) => PlayerCursorScope.Current = __state;

    private static void Postfix(Board __instance, int x, int y, ref Plant __result)
    {
        // HighlightPlantsForMouse uses ToolHitTest, whereas glove pickup uses
        // GetTopPlantAt. Both must resolve the same subfield before native code
        // applies its hover color. Leave carried fusion targets native.
        ShroomSelectionPatch.Postfix(__instance, __instance.PixelToGridX(x, y),
            __instance.PixelToGridY(x, y), ref __result);
        int toolPlayer = PlayerCursorScope.PlayerIndex(__instance);
        int toolCol = __instance.PixelToGridX(x, y), toolRow = __instance.PixelToGridY(x, y);
        if (__instance.CursorObjects[toolPlayer]?.CursorType is CursorType.Normal or CursorType.Shovel
            && toolCol >= 0 && toolCol < 9 && toolRow >= 0 && toolRow < __instance.GetNumRows())
        {
            var pot = __instance.GetPlantsOnLawn(toolCol, toolRow).UnderPlant;
            if (pot != null && pot.mSeedType == SeedType.Flowerpot && ShroomSlots.Live(pot)
                && y >= __instance.GridToPixelY(toolCol, toolRow) + 80 * 2 / 3f)
            { __result = pot; return; }
        }
        // Native shovel use also enters ToolHitTest, even when the cursor is
        // Normal. Resolve overlapping small sprites by the same subfield as
        // glove pickup and placement; preserve native support-layer priority.
        if (__instance.CursorObjects[PlayerCursorScope.PlayerIndex(__instance)]?.CursorType
                is not (CursorType.Normal or CursorType.Shovel)
            || __result == null || !ShroomSlots.IsSmall(FusionPlant.From(__result))) return;
        int col = __instance.PixelToGridX(x, y), row = __instance.PixelToGridY(x, y);
        if (col < 0 || col >= 9 || row < 0 || row >= __instance.GetNumRows()) return;
        var plants = ShroomSlots.Plants(__instance, col, row);
        if (!plants.All(p => ShroomSlots.IsSmall(FusionPlant.From(p)))) return;
        int slot = ShroomSlots.Hover(__instance, col, row, x, y);
        __result = plants.FirstOrDefault(p => ShroomSlots.Slot(p) == slot)!;
    }
}

[HarmonyPatch(typeof(CursorPreview), nameof(CursorPreview.Update))]
internal static class ShroomSeedPreviewPatch
{
    private static void Prefix(CursorPreview __instance, out PlayerCursorScope.Context? __state)
    {
        __state = PlayerCursorScope.Current;
        var board = __instance.mBoard;
        if (board != null)
            PlayerCursorScope.Current = PlayerCursorScope.Capture(board, __instance.PlayerIndex);
    }

    private static void Finalizer(PlayerCursorScope.Context? __state) => PlayerCursorScope.Current = __state;

    private static void Postfix(CursorPreview __instance)
    {
        var board = __instance.mBoard;
        var cursor = board?.CursorObjects[__instance.PlayerIndex];
        if (board == null || cursor == null || cursor.CursorType != CursorType.PlantFromBank
            || !ShroomSlots.IsSmall(cursor.mType) || cursor.mImitaterType != SeedType.None
            || __instance.mController == null || __instance.mGridX < 0 || __instance.mGridX >= 9
            || __instance.mGridY < 0 || __instance.mGridY >= board.GetNumRows()
            || board.CanPlantAt(__instance.mGridX, __instance.mGridY, cursor.mType) != PlantingReason.Ok) return;
        int slot = ShroomSlots.Hover(board, __instance.mGridX, __instance.mGridY);
        if (FusionService.ResolveAll(board, cursor.mType, __instance.mGridX, __instance.mGridY).Count == 0)
            ShroomSlots.TryPlacementSlot(board, cursor.mType, __instance.mGridX, __instance.mGridY, out slot);
        var offset = ShroomSlots.Offset(slot);
        // Native Update rewrites both coordinates each tick, including terrain
        // offsets. Move its anchor once, then refresh the existing controller.
        __instance.mX += offset.X;
        __instance.mY += offset.Y;
        __instance.mController.UpdatePosition();
    }
}

// Extend the installed glove in FusionMod. No replacement glove DLL and no
// duplicate carry lifecycle: its restore/cancel/save/cooldown still own pickup.
[HarmonyPatch]
internal static class ShroomGlovePlacementPatch
{
    private static MethodBase TargetMethod() => AccessTools.Method("LawnGlove.PlantMover:CanPlace");
    private static bool Prefix(Board board, Plant plant, int col, int row, ref bool __result)
    {
        if (!ShroomSlots.IsSmall(FusionPlant.From(plant))) return true;
        __result = FusionService.CanPlaceCarried(board, plant, col, row);
        return false;
    }
}

[HarmonyPatch]
internal static class ShroomGloveMovePatch
{
    private static MethodBase TargetMethod() => AccessTools.Method("LawnGlove.PlantMover:TryMove");
    private static bool Prefix(Board board, Plant plant, int col, int row, ref bool __result)
    {
        if (!ShroomSlots.IsSmall(FusionPlant.From(plant))) return true;
        __result = FusionService.TryPlaceCarried(board, plant, col, row);
        return false;
    }
}

[HarmonyPatch]
internal static class ShroomGlovePreviewPatch
{
    private static MethodBase TargetMethod() => AccessTools.Method("LawnGlove.CarriedPlant:UpdatePreview");
    private static readonly FieldInfo Anchor = AccessTools.Field("LawnGlove.CarriedPlant:gridAnchor");
    private static readonly FieldInfo Preview = AccessTools.Field("LawnGlove.CarriedPlant:gridPreview");

    private static void Postfix(object __instance)
    {
        var anchor = (ReloadedObject)Anchor.GetValue(__instance)!;
        var board = anchor.mBoard;
        var plant = LawnGloveMod.GetCarriedPlant(board);
        if (!anchor.mVisible || plant == null || !ShroomSlots.IsSmall(FusionPlant.From(plant))) return;
        int x = board.mApp.WidgetManager.LastMouseX[0] - board.mX;
        int y = board.mApp.WidgetManager.LastMouseY[0] - board.mY;
        int col = board.PixelToGridX(x, y), row = board.PixelToGridY(x, y);
        int slot = ShroomSlots.Hover(board, col, row);
        if (FusionService.ResolveAll(board, FusionPlant.From(plant), col, row, plant).Count == 0)
            ShroomSlots.TryPlacementSlot(board, FusionPlant.From(plant), col, row, out slot, plant);
        var offset = ShroomSlots.Offset(slot);
        anchor.mX = board.GridToPixelX(col, row) + offset.X;
        anchor.mY = board.GridToPixelY(col, row) + offset.Y;
        if (Preview.GetValue(__instance) is Il2CppSource.Controllers.PreviewController preview) preview.UpdatePosition();
    }
}
