using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Each original TempAddLightLevel coroutine adds one level independently for
// thirty seconds. Native hidden grid records retain each lifetime across saves.
internal static class Glowver
{
    private const int Marker = -1301080;
    internal static bool IsEffect(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == Marker;

    internal static int Level(Board board)
    {
        int level = 0;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false } item && IsEffect(item)
                && item.mGridItemCounter > 0) ++level;
        return level;
    }

    internal static bool Grant(Board board)
    {
        var item = board.m_gridItems.DataArrayAlloc();
        if (item == null) return false;
        try
        {
            item.Init(board.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = Marker;
            item.mSeedType = SeedType.None;
            item.mGridItemCounter = 3000;
            item.mGridX = item.mGridY = -1;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            return true;
        }
        catch { item.GridItemDie(); throw; }
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class GlowverEffectTickPatch
{
    [HarmonyPriority(Priority.First)]
    internal static bool Prefix(GridItem __instance)
    {
        if (!Glowver.IsEffect(__instance)) return true;
        if (BloverFusions.Playing(__instance.mBoard) && --__instance.mGridItemCounter <= 0)
            __instance.GridItemDie();
        return false;
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class GlowverEffectDrawPatch
{
    private static bool Prefix(GridItem __instance) => !Glowver.IsEffect(__instance);
}
