using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Fusion's zombie attack removes 50 HP per bite. Native Replanted EatPlant
// removes 4. Accumulate actual HP loss so source effects retain their rate.
internal static class BiteEvents
{
    private const int Marker = -26150;
    internal static bool Owns(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == Marker;
    internal static int Count(Plant plant, Zombie zombie, int lost)
    {
        if (lost <= 0) return 0;
        var board = plant.mBoard;
        GridItem? state = null;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { } item && !item.mDead && Owns(item)
                && item.mGridX == unchecked((int)plant.DataID) && item.mGridY == unchecked((int)zombie.DataID))
            { state = item; break; }
        if (state == null)
        {
            state = board.m_gridItems.DataArrayAlloc();
            if (state == null) return 0;
            try
            {
                state.Init(board.mApp, GridItemType.ZenTool);
                state.mGridItemState = GridItemState.Normal;
                state.mSunCount = Marker;
                state.mGridX = unchecked((int)plant.DataID);
                state.mGridY = unchecked((int)zombie.DataID);
                state.mGridItemCounter = 0;
                state.mX = state.mY = state.mPosX = state.mPosY = -1000;
            }
            catch { state.GridItemDie(); throw; }
        }
        long damage = (long)state.mGridItemCounter + lost;
        state.mGridItemCounter = (int)(damage % 50);
        return (int)(damage / 50);
    }
    internal static void Tick(GridItem item)
    {
        if (item.mBoard.mPaused) return;
        bool plant = false, zombie = false;
        for (int i = 0; i < item.mBoard.m_plants.Count; ++i)
            if (item.mBoard.m_plants[i] is { } p && !p.mDead && p.DataID == unchecked((uint)item.mGridX)) { plant = true; break; }
        for (int i = 0; i < item.mBoard.m_zombies.Count; ++i)
            if (item.mBoard.m_zombies[i] is { } z && !z.mDead && z.DataID == unchecked((uint)item.mGridY)) { zombie = true; break; }
        if (!plant || !zombie) item.GridItemDie();
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class BiteEventTickPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(GridItem __instance)
    { if (!BiteEvents.Owns(__instance)) return true; BiteEvents.Tick(__instance); return false; }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class BiteEventDrawPatch
{ private static bool Prefix(GridItem __instance) => !BiteEvents.Owns(__instance); }
