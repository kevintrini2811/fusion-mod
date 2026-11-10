using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Each flight belongs to a complete native zombie DataID. GridItems serialize
// vertical velocity, ground altitude and elapsed ticks, including mid-flight.
internal static class SpringEffects
{
    private const int Marker = -34320;
    private const float Unit = 80f / 1.35f;
    internal static bool IsEffect(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == Marker;
    private static Zombie? Owner(GridItem item)
    {
        for (int i = 0; i < item.mBoard.m_zombies.Count; ++i)
            if (item.mBoard.m_zombies[i] is { } zombie && zombie.DataID == unchecked((uint)item.mGridX)
                && !zombie.mDead && !zombie.IsDeadOrDying()) return zombie;
        return null;
    }
    internal static bool Airborne(Zombie zombie)
    {
        for (int i = 0; i < zombie.mBoard.m_gridItems.Count; ++i)
            if (zombie.mBoard.m_gridItems[i] is { mDead: false } item && IsEffect(item)
                && unchecked((uint)item.mGridX) == zombie.DataID) return true;
        return false;
    }
    internal static void Launch(Zombie zombie)
    {
        if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled || zombie.mAltitude > 0 || Airborne(zombie)
            || zombie.mZombieType is ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target
                or ZombieType.Zamboni or ZombieType.Catapult or ZombieType.Bungee
            || !zombie.EffectedByDamage(DamageRangeFlags.Ground | DamageRangeFlags.Dog)) return;
        if (zombie.mZombieType == ZombieType.Imp) { zombie.DieWithLoot(); return; }
        var item = zombie.mBoard.m_gridItems.DataArrayAlloc();
        if (item == null) return;
        try
        {
            item.Init(zombie.mBoard.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = Marker;
            item.mGridX = unchecked((int)zombie.DataID);
            item.mGridItemCounter = 0;
            item.mPosX = zombie.mAltitude;
            item.mPosY = (zombie.mZombieType is ZombieType.Gargantuar or ZombieType.RedeyeGargantuar ? 1.5f : 3.5f) * Unit * .01f;
            item.mX = item.mY = -1000;
            zombie.StopEating();
        }
        catch { item.GridItemDie(); throw; }
    }
    internal static void Tick(GridItem item)
    {
        if (!KelpEffects.Playing(item.mBoard)) return;
        var zombie = Owner(item);
        if (zombie == null) { item.GridItemDie(); return; }
        if (!FusionTime.SourceStep(item.mBoard, typeof(SpringEffects), item.Pointer, item.DataID)) return;
        item.mPosY -= 9.81f * Unit * .0001f;
        zombie.mAltitude += item.mPosY;
        if (++item.mGridItemCounter >= 100 || zombie.mAltitude <= item.mPosX)
        {
            zombie.mAltitude = item.mPosX;
            item.GridItemDie();
        }
    }
    internal sealed record Position(Zombie Zombie, uint Id, IntPtr Board, float X, float Y, float Altitude);
    internal static Position? Capture(Zombie zombie) => Airborne(zombie)
        ? new(zombie, zombie.DataID, zombie.mBoard.Pointer, zombie.mPosX, zombie.mPosY, zombie.mAltitude) : null;
    internal static void Restore(Position? position)
    {
        if (position == null || position.Zombie.DataID != position.Id
            || position.Zombie.mBoard?.Pointer != position.Board || position.Zombie.mDead) return;
        var zombie = position.Zombie;
        zombie.mPosX = position.X; zombie.mPosY = position.Y; zombie.mAltitude = position.Altitude;
        zombie.mX = (int)position.X; zombie.mY = (int)position.Y;
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class SpringEffectTickPatch
{
    private static bool Prefix(GridItem __instance)
    {
        if (!SpringEffects.IsEffect(__instance)) return true;
        SpringEffects.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class SpringEffectDrawPatch
{ private static bool Prefix(GridItem __instance) => !SpringEffects.IsEffect(__instance); }
[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieWalking))]
internal static class SpringWalkingPatch
{ private static bool Prefix(Zombie __instance) => !SpringEffects.Airborne(__instance); }
[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateActions))]
internal static class SpringActionPatch
{ private static bool Prefix(Zombie __instance) => !SpringEffects.Airborne(__instance); }

// UpdatePlaying inlines position/terrain handling and can call specialized
// height writers even when UpdateZombieWalking is suppressed. Preserve status
// ticking, but give the saved launch effect sole ownership of displacement.
[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdatePlaying))]
internal static class SpringPositionScope
{
    private static void Prefix(Zombie __instance, out SpringEffects.Position? __state) =>
        __state = SpringEffects.Capture(__instance);
    private static void Finalizer(SpringEffects.Position? __state) => SpringEffects.Restore(__state);
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.CheckSquish))]
internal static class SpringCrushPatch
{ private static bool Prefix(Zombie __instance) => !SpringEffects.Airborne(__instance); }
