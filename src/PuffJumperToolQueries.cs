using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

// Controller shovel dispatch supplies its tool explicitly while PlantFromBank
// stays selected. The caller marks the query; it never hides across gameplay.
[HarmonyPatch(typeof(Board), nameof(Board.MouseDownWithTool))]
internal static class PuffJumperShovelClickScope
{
    internal sealed class Click
    {
        internal readonly Board Board;
        private readonly IntPtr boardPointer;
        internal readonly int X, Y, Player;
        internal int Depth;
        internal bool Finished;
        internal Click(Board board, int x, int y, int player)
        { Board = board; boardPointer = board.Pointer; X = x; Y = y; Player = player; }
        internal bool Valid => Board.Pointer == boardPointer && ScaredContent.Playing(Board)
            && Board.PixelToGridX(X,Y) is >= 0 and < 9
            && Board.PixelToGridY(X,Y) >= 0 && Board.PixelToGridY(X,Y) < Board.GetNumRows();
    }
    [ThreadStatic] internal static Click? Current;
    internal static void Prefix(Board __instance, int x, int y, int theClickCount,
        CursorType theCursorType, int playerIndex, bool __runOriginal, out Click? __state)
    {
        __state = Current;
        Current = __runOriginal && theClickCount > 0 && theCursorType == CursorType.Shovel
            && playerIndex is >= 0 and <= 1 ? new(__instance,x,y,playerIndex) : null;
    }
    internal static void Finalizer(Click? __state) => Current = __state;
}

// Keep the hide through the existing selection postfixes, then restore before
// the caller can pick up, remove or damage the selected plant.
[HarmonyPatch(typeof(Board), nameof(Board.ToolHitTest))]
internal static class PuffJumperToolQuery
{
    internal sealed record Query(PuffJumperShovelClickScope.Click? Click, PlantQueryHide? Hide);
    internal static void Prefix(Board __instance, int x, int y, int playerIndex, bool __runOriginal, out Query? __state)
    {
        __state = null;
        if (PuffJumperShovelClickScope.Current is { } click)
        {
            bool match = __runOriginal && click.Depth == 0 && !click.Finished && click.Valid
                && __instance.Pointer == click.Board.Pointer && playerIndex == click.Player && x == click.X && y == click.Y
                && PuffJumperBungeeScope.Current == null && PuffJumperProjectileCandidateScope.Current == null
                && PuffJumperBossScope.Current == null && PuffJumperGloveClickScope.Current == null;
            ++click.Depth;
            try
            {
                if (match) click.Finished = true;
                __state = new(click, match ? PuffJumperQueries.Air(__instance) : null);
            }
            catch { --click.Depth; throw; }
            return;
        }
        if (!__runOriginal || playerIndex is < 0 or > 1 || !ScaredContent.Playing(__instance)
            || __instance.CursorObjects[playerIndex]?.CursorType is not (CursorType.Normal or CursorType.Shovel or CursorType.Glove)) return;
        __state = new(null, PuffJumperQueries.Air(__instance));
    }
    internal static void Finalizer(Query? __state)
    {
        if (__state == null) return;
        try { __state.Hide?.Dispose(); }
        finally { if (__state.Click != null) --__state.Click.Depth; }
    }
}

// Both mouse and controller glove pickup enter this installed managed owner.
// The marker alone never changes plant flags across its gameplay body.
[HarmonyPatch]
internal static class PuffJumperGloveClickScope
{
    internal sealed class Click
    {
        internal readonly Board Board;
        private readonly IntPtr boardPointer;
        internal readonly int Col, Row;
        internal int Depth;
        internal Click(Board board, int x, int y)
        { Board = board; boardPointer = board.Pointer; Col = board.PixelToGridX(x,y); Row = board.PixelToGridY(x,y); }
        internal bool Valid => Board.Pointer == boardPointer && ScaredContent.Playing(Board)
            && Board.CursorObjects[0]?.CursorType == CursorType.Glove && LawnGloveMod.GetCarriedPlant(Board) == null;
    }
    [ThreadStatic] internal static Click? Current;
    internal static MethodBase TargetMethod() => AccessTools.Method("LawnGlove.LawnGloveMod:HandleBoardClick");
    internal static void Prefix(Board board, int x, int y, int clicks, int player, out Click? __state)
    {
        __state = Current;
        Current = clicks >= 0 && player == 0 ? new Click(board,x,y) : null;
    }
    internal static void Finalizer(Click? __state) => Current = __state;
}

[HarmonyPatch(typeof(Board), nameof(Board.GetTopPlantAt))]
internal static class PuffJumperGloveQuery
{
    internal sealed record Query(PuffJumperGloveClickScope.Click Click, PlantQueryHide? Hide);
    internal static void Prefix(Board __instance, int theGridX, int theGridY, PlantPriority thePriority,
        bool __runOriginal, out Query? __state)
    {
        __state = null;
        if (PuffJumperGloveClickScope.Current is not { } click) return;
        bool match = __runOriginal && click.Depth == 0 && click.Valid
            && PuffJumperBungeeScope.Current == null && PuffJumperProjectileCandidateScope.Current == null
            && PuffJumperBossScope.Current == null && __instance.Pointer == click.Board.Pointer
            && theGridX == click.Col && theGridY == click.Row
            && theGridX >= 0 && theGridX < 9 && theGridY >= 0 && theGridY < __instance.GetNumRows()
            && thePriority == PlantPriority.DiggingOrder;
        ++click.Depth;
        try { __state = new(click, match ? PuffJumperQueries.Air(__instance) : null); }
        catch { --click.Depth; throw; }
    }
    internal static void Finalizer(Query? __state)
    {
        if (__state == null) return;
        try { __state.Hide?.Dispose(); }
        finally { --__state.Click.Depth; }
    }
}
