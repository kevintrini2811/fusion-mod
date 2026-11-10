using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using UnityEngine;

namespace FusionMod;

// Fusion's combat money is separate from Replanted's profile/shop currency.
// A marked, off-board ZenTool persists this level's wallet through native saves.
internal static class GoldenBean
{
    private const int WalletMarker = -521014;
    private static Board? owner;
    private static bool selected;
    private static GoldenBeanUi? ui;
    internal static bool Playing(Board board) => board.mApp != null
        && board.mApp.GameScene == GameScenes.Playing
        && board.mApp.GameMode != GameMode.ChallengeZenGarden
        && board.Pointer != CherryBombPlants.DisposingBoard;
    internal static bool Wallet(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == WalletMarker;

    private static GridItem? FindWallet(Board board, bool create)
    {
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false } item && Wallet(item)) return item;
        if (!create) return null;
        var wallet = board.m_gridItems.DataArrayAlloc();
        if (wallet == null) return null;
        try
        {
            wallet.Init(board.mApp, GridItemType.ZenTool);
            wallet.mGridItemState = GridItemState.Normal;
            wallet.mSunCount = WalletMarker;
            wallet.mGridX = wallet.mGridY = -1000;
            wallet.mX = wallet.mY = -1000;
            wallet.mGridItemCounter = 0;
            return wallet;
        }
        catch { wallet.GridItemDie(); throw; }
    }

    internal static int Balance(Board board) => DebugMode.Owns(board) ? int.MaxValue
        : FindWallet(board, false)?.mGridItemCounter ?? 0;

    internal static bool Credit(Board board, int amount)
    {
        if (!Playing(board) || amount <= 0) return false;
        var wallet = FindWallet(board, true);
        if (wallet == null)
        {
            FusionMod.Instance.LoggerInstance.Error("Golden bean: could not allocate the saved level wallet; retaining profile credit.");
            return false;
        }
        wallet.mGridItemCounter = (int)Math.Min(int.MaxValue, (long)wallet.mGridItemCounter + amount);
        return true;
    }

    internal static bool Armed(Board board) => selected && owner?.Pointer == board.Pointer
        && board.mCursorObject.CursorType == CursorType.MoneySign;

    internal static void Select()
    {
        if (owner == null || !Playing(owner) || owner.mPaused) return;
        if (Armed(owner)) { Cancel(); return; }
        owner.ClearCursor(ignoreControlType: true, playerIndex: 0);
        DebugMode.CancelZombiePlacement();
        selected = true;
        owner.mCursorObject.CursorType = CursorType.MoneySign;
    }

    private static void Cancel()
    {
        if (owner?.mCursorObject?.CursorType == CursorType.MoneySign)
            owner.ClearCursor(ignoreControlType: true, playerIndex: 0);
        selected = false;
    }

    internal static bool MouseDown(Board board, int x, int y, int clicks, int player)
    {
        if (!Armed(board)) return true;
        if (clicks < 0) { Cancel(); return false; }
        if (!Playing(board) || board.mPaused || player != 0) return false;
        if (board.mSeedBank.ContainsPoint(x, y)) { Cancel(); return true; }
        int col = board.PixelToGridX(x, y), row = board.PixelToGridY(x, y);
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return false;
        var plant = board.GetPlantsOnLawn(col, row).NormalPlant;
        if (!GoldenPlants.Live(plant)) return false;
        try { if (Use(board, plant!)) Cancel(); }
        catch (Exception error) { FusionMod.Instance.LoggerInstance.Error($"Golden bean: {error}"); }
        return false;
    }

    internal static bool Use(Board board, Plant plant)
    {
        if (!Playing(board) || board.mPaused || !GoldenPlants.Live(plant)
            || plant.mBoard.Pointer != board.Pointer) return false;
        var identity = FusionPlant.From(plant);
        int cost = GoldenPlants.IsGolden(identity) ? 1000 : GoldenPlants.UpgradeCost(board, identity);
        var wallet = FindWallet(board, true);
        if (cost == 0 || wallet == null || !DebugMode.Owns(board) && wallet.mGridItemCounter < cost) return false;
        Plant affected = plant;
        if (GoldenPlants.IsGolden(identity))
        {
            if (!GoldenPlants.Ultimate(plant)) return false;
        }
        else
        {
            FusionPlant result = identity == FusionPlant.SilverNut ? FusionPlant.GoldenNut : FusionPlant.GoldenSunflower;
            if (FusionService.CheckFusionPlacement(board, plant, result) != PlantingReason.Ok) return false;
            // Allocate and mark before consumption. A failed allocation retains
            // both original plant and money. Loading never repeats this path.
            var grown = board.AddPlant(plant.mPlantCol, plant.mRow, result.NativeType, result.Marker);
            if (!GoldenPlants.Live(grown)) return false;
            affected = grown;
            GoldenPlants.Ultimate(grown);
            PlayerPlantingScope.Consume(plant);
        }
        if (!DebugMode.Owns(board))
        {
            wallet.mGridItemCounter -= cost;
            // Money.UsedEvent grants one 25-sun pickup per 500 coins spent,
            // capped at 40 pickups. Debug spending produces no free rewards.
            for (int i = 0; i < Math.Min(40, cost / 500); ++i)
                board.AddCoin(affected.mX + (i % 5) * 8, affected.mY, CoinType.Sun, CoinMotion.FromPlant);
        }
        try { affected.UpdateReanim(); affected.UpdateReanimColor(); }
        catch (Exception error) { FusionMod.Instance.LoggerInstance.Error($"Golden bean visual refresh: {error.Message}"); }
        return true;
    }

    internal static void Tick(Board board)
    {
        if (!Playing(board)) return;
        if (owner?.Pointer != board.Pointer) { Clear(); owner = board; }
        if (selected && !Armed(board)) selected = false;
        GoldenPlants.Tick(board);
        // Keep the native money counter available beside the bean control.
        board.ShowCoinBank();
    }

    internal static void UpdateUi()
    {
        if (owner == null || !Playing(owner)) { ui?.Dispose(); ui = null; return; }
        if (UnityEngine.InputSystem.Keyboard.current?.escapeKey.wasPressedThisFrame == true) Cancel();
        try
        {
            ui ??= GoldenBeanUi.TryCreate();
            ui?.Update(!owner.mPaused, Armed(owner), DebugMode.Owns(owner), DebugMode.Owns(owner) ? "∞" : Balance(owner).ToString());
        }
        catch (Exception error)
        {
            ui?.Dispose(); ui = null;
            FusionMod.Instance.LoggerInstance.Error($"Golden bean UI: {error.Message}");
        }
    }

    internal static void Clear()
    {
        Cancel();
        ui?.Dispose(); ui = null; owner = null;
    }

    internal static void BeforeDispose(Board board) { if (owner?.Pointer == board.Pointer) Clear(); }
    internal static void PrepareSave() => Cancel();
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class GoldenBeanTickPatch
{
    private static void Postfix(Board __instance) => GoldenBean.Tick(__instance);
}
[HarmonyPatch(typeof(Board), nameof(Board.MouseDown))]
internal static class GoldenBeanInputPatch
{
    [HarmonyPriority(Priority.First + 1100)]
    private static bool Prefix(Board __instance, int x, int y, int theClickCount, int playerIndex) =>
        GoldenBean.MouseDown(__instance, x, y, theClickCount, playerIndex);
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class GoldenBeanWalletPatch
{
    private static bool Prefix(GridItem __instance) => !GoldenBean.Wallet(__instance);
}
[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class GoldenBeanDisposePatch
{
    private static void Prefix(Board __instance) => GoldenBean.BeforeDispose(__instance);
}
[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Serialize))]
internal static class GoldenBeanSavePatch
{
    private static void Prefix() => GoldenBean.PrepareSave();
}
