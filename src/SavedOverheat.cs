using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The permanent status is a serialized native item, associated by generation-
// checked DataID. No grave coordinates, projectile fields or profile data change.
internal static class SavedOverheat
{
    private const int Marker = -527001;
    internal static bool IsEffect(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == Marker;
    internal static GridItem? Find(Zombie zombie)
    {
        if (zombie.mBoard == null) return null;
        for (int i = 0; i < zombie.mBoard.m_gridItems.Count; ++i)
            if (zombie.mBoard.m_gridItems[i] is { mDead: false } item && IsEffect(item)
                && unchecked((uint)item.mGridX) == zombie.DataID) return item;
        return null;
    }
    internal static bool Add(Zombie zombie)
    {
        if (Find(zombie) != null) return true;
        var item = zombie.mBoard.m_gridItems.DataArrayAlloc();
        if (item == null) return false;
        try
        {
            item.Init(zombie.mBoard.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = Marker;
            item.mSeedType = SeedType.Jalapeno;
            item.mGridX = unchecked((int)zombie.DataID);
            item.mGridItemCounter = -1;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            return true;
        }
        catch { item.GridItemDie(); throw; }
    }
    internal static void Remove(Zombie zombie) => Find(zombie)?.GridItemDie();
    internal static void Tick(GridItem item)
    {
        var board = item.mBoard;
        if (board.mPaused || board.mApp == null || board.mApp.GameScene != GameScenes.Playing) return;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.DataID != unchecked((uint)item.mGridX)) continue;
            if (zombie.mDead) item.GridItemDie();
            return;
        }
        item.GridItemDie();
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class SavedOverheatTickPatch
{
    internal static bool Prefix(GridItem __instance)
    { if (!SavedOverheat.IsEffect(__instance)) return true; SavedOverheat.Tick(__instance); return false; }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class SavedOverheatDrawPatch
{ internal static bool Prefix(GridItem __instance) => !SavedOverheat.IsEffect(__instance); }
