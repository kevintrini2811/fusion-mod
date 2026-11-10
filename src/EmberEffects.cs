using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The native GridItem owns persistence and the zombie DataID owns association.
// mGridY is the source's independent corpse-explosion flag, not a second ember.
internal static class EmberEffects
{
    private const int Marker = -526001;
    internal static bool IsEffect(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == Marker;

    private static GridItem? Find(Zombie zombie)
    {
        if (zombie.mBoard == null) return null;
        for (int i = 0; i < zombie.mBoard.m_gridItems.Count; ++i)
            if (zombie.mBoard.m_gridItems[i] is { mDead: false } item && IsEffect(item)
                && unchecked((uint)item.mGridX) == zombie.DataID) return item;
        return null;
    }

    internal static bool Has(Zombie zombie) => Find(zombie) != null;

    internal static bool Apply(Zombie zombie)
    {
        if (!ScaredContent.Playing(zombie.mBoard) || zombie.mDead || zombie.IsDeadOrDying()
            || zombie.mMindControlled || zombie.mZombieType is ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target)
            return false;
        var item = Find(zombie);
        if (item == null)
        {
            item = zombie.mBoard.m_gridItems.DataArrayAlloc();
            if (item == null) return false;
            try
            {
                item.Init(zombie.mBoard.mApp, GridItemType.ZenTool);
                item.mGridItemState = GridItemState.Normal;
                item.mSunCount = Marker;
                item.mSeedType = SeedType.Doomshroom;
                item.mGridX = unchecked((int)zombie.DataID);
                item.mGridItemCounter = -1;
                item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            }
            catch { item.GridItemDie(); throw; }
        }
        item.mGridY = 1; // Repeated bites refresh the flag; never stack bursts.
        return true;
    }

    internal sealed record Bite(Plant Plant, uint PlantId, int Health, Zombie Zombie, uint ZombieId);
    internal static Bite? BeforeBite(Plant plant, Zombie zombie) => plant != null
        && FusionPlant.From(plant) == FusionPlant.DoomGarlic && MineFusions.Active(plant)
        && plant.mPlantHealth > 0 && zombie.mBoard?.Pointer == plant.mBoard.Pointer
        && !zombie.mDead && !zombie.mMindControlled && !zombie.IsDeadOrDying()
        ? new(plant, plant.DataID, plant.mPlantHealth, zombie, zombie.DataID) : null;
    internal static void AfterBite(Bite? bite)
    {
        if (bite == null || bite.Plant.DataID != bite.PlantId || bite.Zombie.DataID != bite.ZombieId
            || bite.Plant.mBoard?.Pointer != bite.Zombie.mBoard?.Pointer
            || bite.Plant.mPlantHealth >= bite.Health || !Apply(bite.Zombie)) return;
        var board = bite.Zombie.mBoard;
        if (board != null && BloverEffects.Reserve(bite.Zombie, false))
            PoisonShooters.PoisonBite(board, bite.Zombie);
        if (bite.Zombie.mYuckyFaceCounter == 0) bite.Zombie.mYuckyFaceCounter = 1;
    }

    internal sealed record Death(Zombie Zombie, Board Board, uint Id, float X, float Y, int Row, int Order);
    internal static Death? Capture(Zombie zombie)
    {
        if (!ScaredContent.Playing(zombie.mBoard) || zombie.mBoard.mPaused || zombie.mDead
            || Find(zombie) == null) return null;
        var rect = zombie.GetZombieRect();
        return new(zombie, zombie.mBoard, zombie.DataID, (rect.xMin + rect.xMax) / 2,
            (rect.yMin + rect.yMax) / 2, zombie.mRow, zombie.mRenderOrder);
    }
    internal static void Finish(Death? death)
    {
        if (death == null || death.Zombie.DataID != death.Id
            || death.Zombie.mBoard?.Pointer != death.Board.Pointer
            || !(death.Zombie.mDead || death.Zombie.IsDeadOrDying() || death.Zombie.mBodyHealth <= 0)) return;
        var item = Find(death.Zombie);
        if (item == null) return;
        bool explode = item.mGridY != 0;
        item.GridItemDie(); // Consume before the first nested death callback.
        var nuts = new List<(Plant Plant, uint Id)>();
        for (int i = 0; i < death.Board.m_plants.Count; ++i)
        {
            var plant = death.Board.m_plants[i];
            if (plant == null || FusionPlant.From(plant) != FusionPlant.DoomNut || !MineFusions.Active(plant)) continue;
            float x = plant.mX + plant.mWidth / 2, y = plant.mY + plant.mHeight / 2;
            float radius = 4f * 80 / 1.35f;
            if ((x - death.X) * (x - death.X) + (y - death.Y) * (y - death.Y) <= radius * radius)
                nuts.Add((plant, plant.DataID));
        }
        foreach (var (plant, id) in nuts)
            if (plant.DataID == id && MineFusions.Active(plant)) ReactivePots.Heal(plant, 100);
        if (explode)
            DoomEffects.Damage(death.Board, death.Row, death.X, death.Y, death.Order,
                damage: 300, radius: 2f * 80 / 1.35f);
    }

    internal static void Tick(GridItem item)
    {
        if (!ScaredContent.Playing(item.mBoard) || item.mBoard.mPaused) return;
        for (int i = 0; i < item.mBoard.m_zombies.Count; ++i)
            if (item.mBoard.m_zombies[i] is { } zombie && zombie.DataID == unchecked((uint)item.mGridX))
            {
                if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mBodyHealth <= 0)
                {
                    var rect = zombie.GetZombieRect();
                    Finish(new(zombie, item.mBoard, zombie.DataID, (rect.xMin + rect.xMax) / 2,
                        (rect.yMin + rect.yMax) / 2, zombie.mRow, zombie.mRenderOrder));
                }
                return;
            }
        item.GridItemDie(); // A pooled or removed owner cannot mark a new zombie.
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class DoomGarlicBitePatch
{
    private static void Prefix(Zombie __instance, Plant thePlant, out EmberEffects.Bite? __state) =>
        __state = EmberEffects.BeforeBite(thePlant, __instance);
    private static void Postfix(EmberEffects.Bite? __state) => EmberEffects.AfterBite(__state);
}
[HarmonyPatch]
internal static class EmberDeathPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.TakeDamage)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.ApplyBurn)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.DieWithLoot)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.DieNoLoot)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.Update)) };
    private static void Prefix(Zombie __instance, out EmberEffects.Death? __state) => __state = EmberEffects.Capture(__instance);
    private static void Postfix(EmberEffects.Death? __state) => EmberEffects.Finish(__state);
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class EmberSavedItemPatch
{
    private static bool Prefix(GridItem __instance)
    { if (!EmberEffects.IsEffect(__instance)) return true; EmberEffects.Tick(__instance); return false; }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class EmberSavedItemDrawPatch
{ private static bool Prefix(GridItem __instance) => !EmberEffects.IsEffect(__instance); }
