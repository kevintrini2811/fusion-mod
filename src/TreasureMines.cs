using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native serialized fields own the full treasure cycle and the remaining cards.
internal static class TreasureMines
{
    private const int TreasureMarker = -5331296;
    internal static bool TreasureItem(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == TreasureMarker;
    internal static bool Treasure(Board board, int col, int row)
    {
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false } item && TreasureItem(item) && item.mGridX == col && item.mGridY == row) return true;
        return false;
    }
    internal static bool SetTreasure(Board board, int col, int row, bool enabled)
    {
        if (col < 0 || col > 8 || row < 0 || row >= board.GetNumRows()) return false;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false } old && TreasureItem(old) && old.mGridX == col && old.mGridY == row)
            { if (enabled) return true; old.GridItemDie(); }
        if (!enabled) return true;
        var item = board.m_gridItems.DataArrayAlloc();
        if (item == null) return false;
        try
        {
            item.Init(board.mApp, GridItemType.ZenTool); item.mGridItemState = GridItemState.Normal;
            item.mSunCount = TreasureMarker; item.mGridX = col; item.mGridY = row;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            return true;
        }
        catch { item.GridItemDie(); throw; }
    }
    internal const int RiseTicks = 84, GiveTicks = 200, LowerTicks = 84;
    private static readonly SeedType[] Cards = Enum.GetValues<SeedType>()
        .Where(t => t >= 0 && t < SeedType.NumSeedTypes && t != SeedType.Imitater).ToArray();
    internal static bool Owns(Plant plant) => FusionPlant.From(plant) == FusionPlant.MarigoldMine;
    internal static void Initialize(Plant plant)
    {
        if (!Owns(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mState = PlantState.Notready;
        plant.mLaunchCounter = 3000;
        plant.mStateCountdown = plant.mShootingCounter = plant.mTargetX = plant.mTargetY = 0;
        plant.mStartRow = 3;
    }
    internal static void Fertilize(Plant plant)
    {
        if (Owns(plant) && plant.mLaunchCounter > 50) plant.mLaunchCounter = 50;
        if (FusionPlant.From(plant) == FusionPlant.ThreetatoMine && plant.mState == PlantState.Notready && plant.mStateCountdown > 50)
            plant.mStateCountdown = 50;
    }
    internal static void Tick(Plant plant)
    {
        if (!MineFusions.Active(plant)) return;
        if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        // Source cooldown runs during the give animation as well.
        if (plant.mLaunchCounter > 0)
        {
            --plant.mLaunchCounter;
            if (Treasure(plant.mBoard, plant.mPlantCol, plant.mRow))
            { plant.mTargetY ^= 1; if (plant.mTargetY == 0 && plant.mLaunchCounter > 0) --plant.mLaunchCounter; }
        }
        if (plant.mStateCountdown > 0) --plant.mStateCountdown;
        if (plant.mState == PlantState.PotatoRising && plant.mStateCountdown == 0)
        { plant.mState = PlantState.PotatoArmed; plant.mStateCountdown = GiveTicks; }
        else if (plant.mState == PlantState.PotatoArmed)
        {
            if (plant.mStateCountdown == 100) Give(plant);
            if (plant.mDead) return;
            if (plant.mStateCountdown == 0) { plant.mState = PlantState.PotatoMashed; plant.mStateCountdown = LowerTicks; }
        }
        else if (plant.mState == PlantState.PotatoMashed && plant.mStateCountdown == 0) plant.mState = PlantState.Notready;
        if (plant.mLaunchCounter == 0 && plant.mState == PlantState.Notready)
        { plant.mLaunchCounter = 3000; plant.mState = PlantState.PotatoRising; plant.mStateCountdown = RiseTicks; }
        if (++plant.mTargetX > Math.Max(1, plant.mLaunchCounter / 10)) { plant.mTargetX = 0; plant.mShootingCounter = 8; }
    }
    private static void Give(Plant plant)
    {
        var board = plant.mBoard;
        int roll = board.Rand(100);
        bool treasure = Treasure(board, plant.mPlantCol, plant.mRow);
        if (!treasure && roll == 0)
        {
            // Source destroys its RandomZombie wrapper immediately. Resolve the
            // random supported land zombie directly without registering a wrapper.
            ZombieType[] candidates = { ZombieType.Normal, ZombieType.TrafficCone, ZombieType.Pail,
                ZombieType.Newspaper, ZombieType.Door, ZombieType.Football, ZombieType.Polevaulter,
                ZombieType.JackInTheBox, ZombieType.Ladder, ZombieType.Imp };
            var zombie = board.AddZombieInRow(candidates[board.Rand(candidates.Length)], plant.mRow, 0);
            if (zombie == null) return;
            zombie.mPosX = plant.mX + plant.mWidth / 2f; zombie.mX = (int)zombie.mPosX;
            zombie.mFromWave = -1;
        }
        else if (roll < (treasure ? 33 : 11))
        {
            var coin = board.AddCoin(plant.mX, plant.mY, CoinType.UsableSeedPacket, CoinMotion.FromPlant);
            if (coin == null || coin.mDead) return;
            var type = Cards[board.Rand(Cards.Length)];
            coin.mUsableSeedType = type;
            if (coin.mController != null) coin.mController.SeedType = type;
            if (--plant.mStartRow == 0) plant.Die();
        }
        else
        {
            board.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
            // Combat currency belongs to the existing saved level wallet.
            LevelMoney.Mark(board.AddCoin(plant.mX, plant.mY, CoinType.Gold, CoinMotion.FromPlant), 10);
        }
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class TreasureTileTickPatch
{ private static bool Prefix(GridItem __instance) => !TreasureMines.TreasureItem(__instance); }
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class TreasureTileDrawPatch
{ private static bool Prefix(GridItem __instance) => !TreasureMines.TreasureItem(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TreasureMineInitializePatch
{ private static void Postfix(Plant __instance) => TreasureMines.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class TreasureMineAbilityPatch
{
    private static bool Prefix(Plant __instance)
    { if (!TreasureMines.Owns(__instance)) return true; TreasureMines.Tick(__instance); return false; }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdatePotato))]
internal static class TreasureMinePotatoPatch
{ private static bool Prefix(Plant __instance) => !TreasureMines.Owns(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class TreasureMineShootingPatch
{ private static bool Prefix(Plant __instance) => !TreasureMines.Owns(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class TreasureMineDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && TreasureMines.Owns(__instance) && CherryBombPlants.CanTriggerDeath(__instance)
            ? new(__instance.mBoard, FusionPlant.MarigoldMine, __instance.DataID, __instance.mPlantCol, __instance.mRow,
                __instance.mX + __instance.mWidth / 2f, __instance.mY + __instance.mHeight / 2f, __instance.mRenderOrder) : null;
    }
    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id && __instance.mBoard?.Pointer == __state.Board.Pointer)
            MineFusions.Blast(__state.Board, __state.X, __state.Y, ScaredyFusions.SourceUnit, 1800, __state.Order, row: __state.Row);
    }
}
