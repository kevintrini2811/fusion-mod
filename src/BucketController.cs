using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Input;
using Il2CppSource.Controllers;
using UnityEngine;

namespace FusionMod;

// Use the player's existing confirm/cancel bindings. Native confirm performs
// seed-cursor housekeeping after MouseDown, which must not clear our pickup.
internal static class BucketController
{
    private static IntPtr cancelledTool;

    internal static bool CanUse(GamepadCursorController controller) => controller != null && controller
        && controller.gameObject.activeInHierarchy && controller.PlayerIndex == 0 && Application.isFocused
        && controller.Board != null && controller.Board.mApp != null && !controller.Board.mPaused
        && controller.Board.mApp.GameScene == GameScenes.Playing
        && controller.Board.mApp.GameMode != GameMode.ChallengeZenGarden
        && controller.m_cursor != null && controller.m_cursor.Enabled
        && controller.m_playerInput != null && controller.m_playerInput.Paired
        && controller.m_playerInput.CurrentControlType == ControlType.Controller;

    internal static bool Confirm(GamepadCursorController controller)
    {
        if (!CanUse(controller)) return true;
        var board = controller.Board;
        bool selected = BucketPlants.SelectedOn(board);
        var cursor = board.mCursorObject;
        if (!selected && cursor.CursorType is not (CursorType.Normal or CursorType.PlantFromBank)) return true;
        controller._updateMousePosition();
        int x = board.mApp.WidgetManager.LastMouseX[0] - board.mX;
        int y = board.mApp.WidgetManager.LastMouseY[0] - board.mY;
        if (selected) { BucketPlants.MouseDown(board, x, y, 1, 0); return false; }
        int col = board.PixelToGridX(x, y), row = board.PixelToGridY(x, y);
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return true;
        if (cursor.CursorType == CursorType.PlantFromBank)
        {
            var incoming = cursor.mType == SeedType.Imitater
                ? new FusionPlant(cursor.mImitaterType) : new FusionPlant(cursor.mType, cursor.mImitaterType);
            if ((int)incoming.NativeType < 0 || incoming.NativeType >= SeedType.NumSeedTypes) return true;
            var target = board.GetPlantsOnLawn(col, row).NormalPlant;
            // Planting, upgrades, coffee, repairs and recipes keep their normal
            // confirm action even when a magnet owns usable metal on this tile.
            if (FusionService.TryResolve(board, incoming, col, row, out var ingredient, out var result)
                    && FusionService.CheckFusionPlacement(board, ingredient, result) == PlantingReason.Ok
                || target != null && BucketPlants.CanFeed(target, incoming)
                || FusionService.CheckNativePlacement(board, col, row, incoming.NativeType) == PlantingReason.Ok)
                return true;
        }
        return !BucketPlants.TrySelect(board, x, y);
    }

    internal static bool Cancel(GamepadCursorController controller)
    {
        if (!CanUse(controller) || !BucketPlants.SelectedOn(controller.Board)) return true;
        // Cancel and the shovel/tool action can share B. Consume that gesture's
        // later tool release as well, so cancelling never digs up the magnet.
        cancelledTool = controller.Pointer;
        BucketPlants.Clear();
        return false;
    }

    internal static bool Tool(GamepadCursorController controller)
    {
        if (cancelledTool == controller.Pointer) return false;
        if (!CanUse(controller) || !BucketPlants.SelectedOn(controller.Board)) return true;
        BucketPlants.Clear();
        return false;
    }

    internal static void ToolUpdated(GamepadCursorController controller)
    {
        if (cancelledTool == controller.Pointer && controller.m_secondaryAction?.IsPressed() != true)
            cancelledTool = IntPtr.Zero;
    }
    internal static void Forget(GamepadCursorController controller)
    { if (cancelledTool == controller.Pointer) cancelledTool = IntPtr.Zero; }
}

[HarmonyPatch(typeof(GamepadCursorController), nameof(GamepadCursorController._onCursorConfirmed))]
internal static class BucketControllerConfirmPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(GamepadCursorController __instance) => BucketController.Confirm(__instance);
}

[HarmonyPatch(typeof(GamepadCursorController), nameof(GamepadCursorController._onCursorCancel))]
internal static class BucketControllerCancelPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(GamepadCursorController __instance) => BucketController.Cancel(__instance);
}

[HarmonyPatch]
internal static class BucketControllerToolPatch
{
    private static MethodBase TargetMethod() => AccessTools.Method("LawnGlove.ControllerTools:Use");
    private static bool Prefix(GamepadCursorController controller) => BucketController.Tool(controller);
}

[HarmonyPatch]
internal static class BucketControllerToolReleasePatch
{
    private static MethodBase TargetMethod() => AccessTools.Method("LawnGlove.ControllerTools:Update");
    private static void Postfix(GamepadCursorController controller) => BucketController.ToolUpdated(controller);
}

[HarmonyPatch(typeof(GamepadCursorController), nameof(GamepadCursorController.OnDie))]
internal static class BucketControllerDisposePatch
{
    private static void Prefix(GamepadCursorController __instance) => BucketController.Forget(__instance);
}
