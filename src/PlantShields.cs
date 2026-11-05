using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Shield belongs to its recipient, including after moving/removing its producer.
// Native items save the full recipient generation and remaining shield points.
internal static class PlantShields
{
    private const int Marker = -613250;
    internal static bool Owns(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == Marker;
    private static GridItem? Find(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_gridItems.Count; ++i)
            if (plant.mBoard.m_gridItems[i] is { mDead: false } item && Owns(item)
                && unchecked((uint)item.mGridX) == plant.DataID) return item;
        return null;
    }
    internal static int Amount(Plant plant) => Math.Max(0, Find(plant)?.mGridItemCounter ?? 0);
    internal static void Add(Plant plant, int amount)
    {
        var item = Find(plant);
        if (item == null)
        {
            item = plant.mBoard.m_gridItems.DataArrayAlloc();
            if (item == null) return;
            try
            {
                item.Init(plant.mApp, GridItemType.ZenTool);
                item.mGridItemState = GridItemState.Normal;
                item.mSunCount = Marker;
                item.mGridX = unchecked((int)plant.DataID);
                item.mGridItemCounter = 0;
                item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            }
            catch { item.GridItemDie(); throw; }
        }
        item.mGridItemCounter = (int)Math.Min(int.MaxValue, (long)item.mGridItemCounter + amount);
    }
    internal static int Absorb(Plant plant, int damage)
    {
        var item = Find(plant);
        if (item == null || damage <= 0) return damage;
        int used = Math.Min(damage, Math.Max(0, item.mGridItemCounter));
        item.mGridItemCounter -= used;
        if (item.mGridItemCounter == 0) item.GridItemDie();
        return damage - used;
    }
    internal static void Tick(GridItem item)
    {
        if (item.mBoard.mPaused) return;
        for (int i = 0; i < item.mBoard.m_plants.Count; ++i)
            if (item.mBoard.m_plants[i] is { mDead: false } plant
                && plant.DataID == unchecked((uint)item.mGridX)) return;
        item.GridItemDie();
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class PlantShieldTickPatch
{
    private static bool Prefix(GridItem __instance)
    { if (!PlantShields.Owns(__instance)) return true; PlantShields.Tick(__instance); return false; }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class PlantShieldDrawPatch
{ private static bool Prefix(GridItem __instance) => !PlantShields.Owns(__instance); }
