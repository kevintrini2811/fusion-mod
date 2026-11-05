using Il2CppReloaded.Gameplay;

namespace FusionMod;

// CanPlantAt has no player argument. Its synchronous preview and tool callers
// supply the owner here without changing planting's separate creation scope.
internal static class PlayerCursorScope
{
    internal sealed record Context(Board Board, int PlayerIndex, int X, int Y);
    [ThreadStatic] internal static Context? Current;

    internal static int PlayerIndex(Board board)
    {
        if (Current?.Board.Pointer == board.Pointer) return Current.PlayerIndex;
        var planting = PlayerPlantingScope.Current;
        return planting?.Board.Pointer == board.Pointer ? planting.PlayerIndex : 0;
    }

    internal static (int X, int Y) MousePosition(Board board, int playerIndex) =>
        (board.mApp.WidgetManager.LastMouseX[playerIndex] - board.mX,
         board.mApp.WidgetManager.LastMouseY[playerIndex] - board.mY);

    internal static Context Capture(Board board, int playerIndex)
    {
        var (x, y) = MousePosition(board, playerIndex);
        return new Context(board, playerIndex, x, y);
    }

    internal static (int X, int Y) Position(Board board)
    {
        if (Current?.Board.Pointer == board.Pointer) return (Current.X, Current.Y);
        var planting = PlayerPlantingScope.Current;
        return planting?.Board.Pointer == board.Pointer
            ? (planting.X, planting.Y) : MousePosition(board, 0);
    }
}
