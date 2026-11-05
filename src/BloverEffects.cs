using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Fusion attacks reserve saved backing records for their status effects.
// Existing status owners retain their rules, ticks and death cleanup.
internal static class BloverEffects
{
    private const int Poison = -31001, Cryo = -31002, Heat = -31003;
    internal static bool IsEffect(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount is Poison or Cryo or Heat;
    private static GridItem? Find(Zombie zombie, int marker)
    {
        for (int i = 0; i < zombie.mBoard.m_gridItems.Count; ++i)
            if (zombie.mBoard.m_gridItems[i] is { mDead: false } item && IsEffect(item)
                && item.mSunCount == marker && unchecked((uint)item.mGridX) == zombie.DataID) return item;
        return null;
    }
    internal static bool Reserve(Zombie zombie, bool cryo) => Reserve(zombie, cryo ? Cryo : Poison);
    internal static bool ReserveOverheat(Zombie zombie) => Reserve(zombie, Heat);
    private static bool Reserve(Zombie zombie, int marker)
    {
        if (Find(zombie, marker) != null) return true;
        var item = zombie.mBoard.m_gridItems.DataArrayAlloc();
        if (item == null) return false;
        try
        {
            item.Init(zombie.mBoard.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = marker;
            item.mSeedType = SeedType.None;
            item.mGridX = unchecked((int)zombie.DataID);
            item.mGridY = 0;
            item.mGridItemCounter = marker == Cryo ? 100 : 0;
            item.mRenderOrder = 1;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            return true;
        }
        catch { item.GridItemDie(); throw; }
    }
    private static Zombie? Owner(GridItem item)
    {
        for (int i = 0; i < item.mBoard.m_zombies.Count; ++i)
            if (item.mBoard.m_zombies[i] is { } zombie && zombie.DataID == unchecked((uint)item.mGridX)
                && BloverFusions.Live(item.mBoard, zombie) && !zombie.mMindControlled) return zombie;
        return null;
    }
    internal static void Restore(Board board)
    {
        if (!BloverFusions.Playing(board)) return;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || !IsEffect(item)) continue;
            var zombie = Owner(item);
            if (zombie == null) { item.GridItemDie(); continue; }
            if (item.mSunCount == Poison)
                PoisonShooters.Restore(zombie, item.mGridY, item.mGridItemCounter, item.mRenderOrder);
            else if (item.mSunCount == Cryo) SnowPea.Restore(zombie, item.mGridY, item.mGridItemCounter);
            else if (!Overheat.Has(zombie)) Overheat.Apply(zombie);
        }
    }
    internal static void Save(Board board)
    {
        if (!BloverFusions.Playing(board)) return;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || !IsEffect(item)) continue;
            var zombie = Owner(item);
            if (zombie == null) { item.GridItemDie(); continue; }
            if (item.mSunCount == Poison)
            {
                var state = PoisonShooters.Capture(zombie);
                if (state == null) { item.GridItemDie(); continue; }
                item.mGridY = state.Value.Level;
                item.mGridItemCounter = state.Value.Remaining;
                item.mRenderOrder = state.Value.Countdown;
            }
            else if (item.mSunCount == Cryo)
            {
                var state = SnowPea.Capture(zombie);
                if (state == null) { item.GridItemDie(); continue; }
                item.mGridY = state.Value.Points;
                item.mGridItemCounter = state.Value.Threshold;
            }
            else if (!Overheat.Has(zombie)) item.GridItemDie();
        }
    }
}
[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class BloverSavedEffectsPatch
{
    private static void Prefix(Board __instance) => BloverEffects.Restore(__instance);
    // Finalizers run after all ordinary postfixes, including poison damage.
    private static void Finalizer(Board __instance) => BloverEffects.Save(__instance);
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class BloverSavedItemPatch
{
    private static bool Prefix(GridItem __instance) => !BloverEffects.IsEffect(__instance);
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class BloverSavedItemDrawPatch
{
    private static bool Prefix(GridItem __instance) => !BloverEffects.IsEffect(__instance);
}
// Keep native ref-struct readers out of IL2CPP Harmony trampolines. Clear
// transient caches once before loading; saved GridItems remain native-owned and
// restore the statuses on the first UpdateGame after successful deserialization.
[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Deserialize))]
internal static class BloverLoadPatch
{
    private static void Prefix(SerializationService __instance)
    {
        var board = __instance.App?.Board;
        if (board == null) return;
        PoisonShooters.ForgetBoard(board);
        SnowPea.ForgetBoard(board);
    }
}
