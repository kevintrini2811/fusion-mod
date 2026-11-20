using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native grid items preserve the payload and full zombie generation across saves.
internal static class HypnoBiteEffects
{
    private const int CurseMarker = -526020, ZodiacMarker = -526021;
    private const int BurstCount = 60, BurstInterval = 10, StarDamage = 20;

    internal static bool IsEffect(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount is CurseMarker or ZodiacMarker;

    private static bool Playing(Board? board) => ScaredContent.Playing(board)
        && board!.Pointer != CherryBombPlants.DisposingBoard && !board.mPaused;

    private static GridItem? FindCurse(Board board, uint zombieId)
    {
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false, mSunCount: CurseMarker } item
                && IsEffect(item) && unchecked((uint)item.mGridX) == zombieId) return item;
        return null;
    }

    internal static void Attach(Plant plant, Zombie zombie, bool curse)
    {
        var board = zombie.mBoard;
        uint plantId = plant.DataID, zombieId = zombie.DataID;
        var item = board.m_gridItems.DataArrayAlloc();
        if (item == null) return;
        uint itemId = item.DataID;
        try
        {
            item.Init(board.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = curse ? CurseMarker : ZodiacMarker;
            item.mSeedType = SeedType.Hypnoshroom;
            item.mGridX = unchecked((int)zombieId);
            item.mGridY = item.mGridItemCounter = 0;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            if (!HypnoPlants.MindControl(zombie) || zombie.DataID != zombieId
                || zombie.mBoard?.Pointer != board.Pointer || zombie.mDead || zombie.IsDeadOrDying())
            {
                if (item.DataID == itemId && !item.mDead) item.GridItemDie();
                return;
            }
        }
        catch
        {
            if (item.DataID == itemId && !item.mDead) item.GridItemDie();
            throw;
        }

        if (plant.DataID == plantId && plant.mBoard?.Pointer == board.Pointer && ChomperFusions.Active(plant))
            PlayerPlantingScope.Consume(plant);
        if (!curse && item.DataID == itemId && !item.mDead && zombie.DataID == zombieId
            && zombie.mBoard?.Pointer == board.Pointer && !zombie.mDead && !zombie.IsDeadOrDying())
        {
            FusionTime.SourceStep(board, typeof(HypnoBiteEffects), item.Pointer, itemId);
            Burst(item, zombie);
        }
    }

    private static void Burst(GridItem item, Zombie zombie)
    {
        int burst = item.mGridY++;
        item.mGridItemCounter = BurstInterval;
        var board = item.mBoard;
        var origin = zombie.GetZombieRect().center;
        uint zombieId = zombie.DataID, itemId = item.DataID;
        int order = zombie.mRenderOrder, row = zombie.mRow;
        // Source emits five sea stars at 20 * (burst + 3 * star) degrees.
        for (int star = 0; star < 5; ++star)
        {
            if (item.DataID != itemId || item.mDead || zombie.DataID != zombieId
                || zombie.mBoard?.Pointer != board.Pointer || zombie.mDead || zombie.IsDeadOrDying()
                || board.Pointer == CherryBombPlants.DisposingBoard) break;
            StarProjectiles.Spawn(board, origin.x, origin.y, order, row,
                6, 20 * (burst + 3 * star), 4, StarDamage);
        }
    }

    internal static void Tick(GridItem item)
    {
        var board = item.mBoard;
        if (!Playing(board)) return;
        uint zombieId = unchecked((uint)item.mGridX);
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.DataID != zombieId || zombie.mBoard?.Pointer != board.Pointer) continue;
            if (zombie.mDead || zombie.IsDeadOrDying())
            {
                if (item.mSunCount == CurseMarker) Explode(DeathAt(item, zombie));
                else item.GridItemDie();
            }
            else if (item.mSunCount == ZodiacMarker
                && FusionTime.SourceStep(board, typeof(HypnoBiteEffects), item.Pointer, item.DataID)
                && --item.mGridItemCounter <= 0)
            {
                uint itemId = item.DataID;
                if (item.mGridY < BurstCount) Burst(item, zombie);
                if (item.DataID == itemId && !item.mDead && item.mGridY >= BurstCount) item.GridItemDie();
            }
            return;
        }
        item.GridItemDie();
    }

    internal sealed record Death(GridItem Item, uint ItemId, Zombie Zombie, uint ZombieId,
        Board Board, int Col, int Row, float X, float Y, int Order);

    private static Death? DeathAt(GridItem item, Zombie zombie)
    {
        var board = zombie.mBoard;
        int row = zombie.mRow;
        if (row < 0 || row >= board.GetNumRows()) return null;
        var center = zombie.GetZombieRect().center;
        int col = Math.Clamp(board.PixelToGridX((int)center.x, (int)center.y), 0, 8);
        return new(item, item.DataID, zombie, zombie.DataID, board, col, row,
            board.GridToPixelX(col, row) + 40, board.GridToPixelY(col, row) + 40, zombie.mRenderOrder);
    }

    internal static Death? Capture(Zombie zombie)
    {
        var board = zombie.mBoard;
        if (!zombie.mMindControlled || zombie.mDead || !Playing(board)) return null;
        var item = FindCurse(board, zombie.DataID);
        return item == null ? null : DeathAt(item, zombie);
    }

    internal static void Explode(Death? death)
    {
        if (death == null || !Playing(death.Board) || death.Item.DataID != death.ItemId || death.Item.mDead
            || death.Zombie.DataID != death.ZombieId || death.Zombie.mBoard?.Pointer != death.Board.Pointer
            || !(death.Zombie.mDead || death.Zombie.IsDeadOrDying())) return;
        death.Item.GridItemDie();
        DoomEffects.Explode(death.Board, death.Col, death.Row, death.X, death.Y, death.Order);
    }
}

[HarmonyPatch]
internal static class CurseAllyDeathPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.TakeDamage)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.DieWithLoot)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.DieNoLoot)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.Update)) };
    private static void Prefix(Zombie __instance, out HypnoBiteEffects.Death? __state) =>
        __state = HypnoBiteEffects.Capture(__instance);
    private static void Postfix(HypnoBiteEffects.Death? __state) => HypnoBiteEffects.Explode(__state);
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class HypnoBiteSavedItemPatch
{
    private static bool Prefix(GridItem __instance)
    {
        if (!HypnoBiteEffects.IsEffect(__instance)) return true;
        HypnoBiteEffects.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class HypnoBiteSavedItemDrawPatch
{
    private static bool Prefix(GridItem __instance) => !HypnoBiteEffects.IsEffect(__instance);
}
