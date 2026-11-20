using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class TrippyShroom
{
    internal const int FirstColor = -426000, LastColor = FirstColor - 6;
    private const int Marker = -526002, AllColors = 0x7f;

    internal static int ColorIndex(Projectile shot) => FirstColor - (int)shot.mCobTargetX;

    internal static bool IsColorRecord(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == Marker;

    private static bool CanHypnotize(Board board, Zombie? zombie) => zombie != null
        && zombie.mBoard?.Pointer == board.Pointer && !zombie.mDead && !zombie.IsDeadOrDying()
        && !zombie.mMindControlled && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target);

    private static bool RecordColor(Zombie zombie, int color)
    {
        var board = zombie.mBoard;
        if (board == null || !CanHypnotize(board, zombie)) return false;
        GridItem? record = null;
        uint id = zombie.DataID;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false } item && IsColorRecord(item)
                && unchecked((uint)item.mGridX) == id)
            { record = item; break; }

        if (record == null)
        {
            record = board.m_gridItems.DataArrayAlloc();
            if (record == null) return false;
            try
            {
                record.Init(board.mApp, GridItemType.ZenTool);
                record.mGridItemState = GridItemState.Normal;
                record.mSunCount = Marker;
                record.mGridX = unchecked((int)id);
                record.mGridY = 0;
                record.mGridItemCounter = -1;
                record.mX = record.mY = record.mPosX = record.mPosY = -1000;
                record.mVisible = false;
            }
            catch { record.GridItemDie(); throw; }
        }

        record.mGridY |= 1 << color;
        if (record.mGridY != AllColors) return false;
        record.GridItemDie();
        HypnoPlants.MindControl(zombie);
        return true;
    }

    internal static void Impact(Projectile shot, Zombie? zombie)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null) return;
        bool hit = zombie != null && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled
            && zombie.mBoard?.Pointer == board.Pointer;
        uint id = zombie?.DataID ?? 0;
        var flags = hit ? shot.GetDamageFlags(zombie!) : (DamageFlags)0;
        int color = ColorIndex(shot), damage = ScaredyProjectiles.Damage(shot);
        shot.Die();
        if (hit && zombie != null && zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer
            && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled && ScaredContent.Playing(board)
            && !RecordColor(zombie, color)) zombie.TakeDamage(damage, flags);
    }

    internal static void RemoveStaleRecord(GridItem item)
    {
        var board = item.mBoard;
        if (board == null || !ScaredContent.Playing(board) || board.mPaused) return;
        var id = unchecked((uint)item.mGridX);
        var zombie = board.ZombieTryToGet((ZombieID)id);
        if (zombie == null || zombie.DataID != id || !CanHypnotize(board, zombie)) item.GridItemDie();
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class TrippyImpactPatch
{
    private static bool Prefix(Projectile __instance, Zombie theZombie, bool __runOriginal)
    {
        if (!__runOriginal || ScaredyProjectiles.Family(__instance) != 7) return true;
        TrippyShroom.Impact(__instance, theZombie);
        return false;
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class TrippyColorRecordUpdatePatch
{
    private static bool Prefix(GridItem __instance)
    {
        if (!TrippyShroom.IsColorRecord(__instance)) return true;
        TrippyShroom.RemoveStaleRecord(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class TrippyColorRecordDrawPatch
{
    private static bool Prefix(GridItem __instance) => !TrippyShroom.IsColorRecord(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class TrippyBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || FusionPlant.From(thePlant) != FusionPlant.TrippyShroom
            || !HypnoPlants.ReplaceAfterBite(thePlant, __instance, SeedType.Scaredyshroom);
}
