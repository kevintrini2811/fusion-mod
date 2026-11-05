using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Input;
using Il2CppSource.Controllers;

namespace FusionMod;

[HarmonyPatch(typeof(GamepadCursorController), nameof(GamepadCursorController.Update))]
internal static class ShroomControllerPatch
{
    private static void Prefix(GamepadCursorController __instance, out bool __state)
    {
        __state = false;
        var board = __instance.Board;
        if (board == null || board.mPaused
            || board.mApp.GameScene != GameScenes.Playing
            || board.mApp.GameMode == GameMode.ChallengeZenGarden
            || __instance.m_cursor == null || !__instance.m_cursor.Enabled
            || __instance.m_playerInput == null || !__instance.m_playerInput.Paired
            || __instance.m_playerInput.CurrentControlType != ControlType.Controller) return;

        var (x, y) = PlayerCursorScope.MousePosition(board, __instance.PlayerIndex);
        int col = board.PixelToGridX(x, y), row = board.PixelToGridY(x, y);
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return;
        var plants = ShroomSlots.Plants(board, col, row);
        if (plants.Count == 0 || !plants.All(p => ShroomSlots.IsSmall(FusionPlant.From(p)))) return;

        // Native idle interpolation targets the cell anchor, not a small
        // plant's subfield. Skip only that interpolation, retaining movement,
        // mouse synchronization, confirmation and the installed glove controls.
        __instance.m_cursorResetTimer = float.MaxValue;
        __state = true;
    }

    private static void Finalizer(GamepadCursorController __instance, bool __state)
    {
        // Resume the native delay when leaving this cell or changing device.
        // Never leave the sentinel on the pooled controller after an exception.
        if (__state) __instance.m_cursorResetTimer = 0;
    }
}
