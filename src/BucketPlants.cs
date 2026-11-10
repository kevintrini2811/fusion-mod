using System.Reflection;
using HarmonyLib;
using LawnGlove;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Collected pails and helmets remain owned by their saved MagnetItem until
// placement succeeds. Selection is transient; cancellation never consumes it.
internal static class BucketPlants
{
    private static Plant? magnet;
    private static uint magnetId;
    private static int slot;
    private static BucketDrops.Drop? heldDrop;
    private static FusionPlant SelectedIdentity => heldDrop?.Identity ??
        (magnet != null && Helmet(magnet.mMagnetItems[slot]) ? FusionPlant.FootballHelmet : FusionPlant.Bucket);
    private static Board? SelectionBoard => magnet?.mBoard ?? heldDrop?.Board;
    private static bool ValidSelection => magnet != null ? Valid : BucketDrops.Contains(heldDrop);
    internal static bool HoldingDrop(BucketDrops.Drop drop) => ReferenceEquals(heldDrop, drop)
        && drop.Board.mCursorObject.CursorType == CursorType.Glove;
    internal static bool IsIron(FusionPlant identity) => identity == FusionPlant.BuckNut
        || identity == FusionPlant.Buckshooter || identity == FusionPlant.BuckShroom;
    internal static bool CanFeed(Plant plant, FusionPlant incoming)
    {
        var identity = FusionPlant.From(plant);
        return SeaFusions.CanFeed(plant, incoming) || ChomperFusions.Active(plant)
            && (incoming == FusionPlant.FootballHelmet && identity == FusionPlant.FootballNut
                || incoming == FusionPlant.Bucket && (IsIron(identity) || identity == FusionPlant.IronSquash
                    || identity == FusionPlant.FootballShroom || identity == FusionPlant.RustShroom || identity == FusionPlant.MetalPult || identity == FusionPlant.BoltPult || identity == FusionPlant.IronStar));
    }
    internal static void Feed(Plant plant)
    {
        if (SeaFusions.CanFeed(plant, SeedType.Seashroom)) { SeaFusions.Feed(plant); return; }
        plant.mPlantHealth = (int)Math.Min(Math.Max(plant.mPlantMaxHealth, plant.mPlantHealth),
            (long)plant.mPlantHealth + (FusionPlant.From(plant) == FusionPlant.FootballNut ? 5000 : 500));
    }
    private static bool Helmet(MagnetItem item) => item.ItemType is MagnetItemType.FootballHelmet1
        or MagnetItemType.FootballHelmet2 or MagnetItemType.FootballHelmet3;
    private static bool Pail(MagnetItem item) => item.ItemType is MagnetItemType.Pail1
        or MagnetItemType.Pail2 or MagnetItemType.Pail3;
    private static bool Valid => magnet != null && magnet.DataID == magnetId
        && ChomperFusions.Active(magnet) && magnet.mMagnetItems != null
        && slot < magnet.mMagnetItems.Length && (Pail(magnet.mMagnetItems[slot]) || Helmet(magnet.mMagnetItems[slot]));
    internal static bool IsTarget(Plant plant)
    {
        var board = plant.mBoard;
        if (!ValidSelection || board == null || SelectionBoard?.Pointer != board.Pointer
            || board.mCursorObject.CursorType != CursorType.Glove) return false;
        return CanFeed(plant, SelectedIdentity)
            || (FusionService.TryResolve(board, SelectedIdentity, plant.mPlantCol, plant.mRow,
                    out var target, out var result) && target.Pointer == plant.Pointer
                && FusionService.CheckFusionPlacement(board, target, result) == PlantingReason.Ok);
    }
    internal static bool Holding(Plant plant) => magnet?.Pointer == plant.Pointer
        && magnetId == plant.DataID && Valid
        && plant.mBoard.mCursorObject.CursorType == CursorType.Glove;
    internal static void Clear()
    {
        var board = SelectionBoard;
        magnet = null;
        heldDrop = null;
        if (board?.mCursorObject.CursorType == CursorType.Glove) board.ClearCursor();
        ImportedPeaVisuals.ClearBucketCursor();
    }
    internal static void UpdateCursor()
    {
        if (magnet == null && heldDrop == null) return;
        // Pause must not cancel a valid selection.
        var board = SelectionBoard;
        if (board == null || board.mCursorObject.CursorType != CursorType.Glove) { Clear(); return; }
        if (board.mPaused) return;
        if (!ValidSelection) { Clear(); return; }
        try { ImportedPeaVisuals.ShowBucketCursor(board, SelectedIdentity); }
        catch (Exception error)
        {
            Clear();
            FusionMod.Instance.LoggerInstance.Error($"Bucket cursor could not be created; item retained: {error}");
        }
    }
    internal static bool MouseDown(Board board, int x, int y, int clicks, int player)
    {
        if (player != 0 || board.mApp == null || board.mApp.GameScene != GameScenes.Playing
            || board.mApp.GameMode == GameMode.ChallengeZenGarden) return true;
        if ((magnet != null || heldDrop != null) && (SelectionBoard?.Pointer != board.Pointer
            || board.mCursorObject.CursorType != CursorType.Glove)) Clear();
        if (magnet != null || heldDrop != null)
        {
            if (clicks < 0) { Clear(); board.ClearCursor(); return false; }
            if (board.mPaused) return false;
            if (!ValidSelection) { Clear(); board.ClearCursor(); return false; }
            int col = board.PixelToGridX(x, y), row = board.PixelToGridY(x, y);
            if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return false;
            var target = board.GetPlantsOnLawn(col, row).NormalPlant;
            if (target != null && CanFeed(target, SelectedIdentity)) Feed(target);
            else
            {
                FusionPlant result = SelectedIdentity;
                Plant? ingredient = null;
                if (FusionService.TryResolve(board, SelectedIdentity, col, row, out var found, out var fusion))
                { ingredient = found; result = fusion; }
                if (ingredient == null && SelectedIdentity == FusionPlant.EyesZombieHead) return false;
                var reason = ingredient == null
                    ? FusionService.CheckNativePlacement(board, col, row, result.NativeType)
                    : FusionService.CheckFusionPlacement(board, ingredient, result);
                if (reason != PlantingReason.Ok) return false;
                var created = board.AddPlant(col, row, result.NativeType, result.Marker);
                if (created == null || created.mDead || !created.mIsOnBoard) return false;
                if (ingredient != null) FusionService.CompleteFusion(created, ingredient, SelectedIdentity);
            }
            if (heldDrop is { } used) BucketDrops.Consume(used);
            else magnet!.mMagnetItems[slot].ItemType = MagnetItemType.None;
            Clear();
            board.ClearCursor();
            return false;
        }
        if (board.mPaused || clicks < 0 || board.mCursorObject.CursorType != CursorType.Normal) return true;
        if (BucketDrops.TryPick(board, x, y, out var drop))
        {
            board.ClearCursor();
            heldDrop = drop;
            board.mCursorObject.CursorType = CursorType.Glove;
            return false;
        }
        int gridX = board.PixelToGridX(x, y), gridY = board.PixelToGridY(x, y);
        if (gridX < 0 || gridX >= 9 || gridY < 0 || gridY >= board.GetNumRows()) return true;
        var plant = board.GetPlantsOnLawn(gridX, gridY).NormalPlant;
        if (plant == null || FusionPlant.From(plant) != (FusionPlant)SeedType.Magnetshroom
            || !ChomperFusions.Active(plant) || plant.mMagnetItems == null) return true;
        for (int i = 0; i < plant.mMagnetItems.Length; ++i)
        {
            if (!Pail(plant.mMagnetItems[i]) && !Helmet(plant.mMagnetItems[i])) continue;
            board.ClearCursor();
            magnet = plant; magnetId = plant.DataID; slot = i;
            board.mCursorObject.CursorType = CursorType.Glove;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class BucketInitializePatch
{
    private static void Postfix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        // PuffShooters owns the small iron plant's distinct 1000 HP profile.
        if (identity == FusionPlant.BuckShroom) return;
        if (identity != FusionPlant.Bucket && !BucketPlants.IsIron(identity)) return;
        __instance.mPlantHealth = __instance.mPlantMaxHealth = identity == FusionPlant.BuckNut ? 8000 : 2000;
        if (identity == FusionPlant.Buckshooter) __instance.mLaunchRate = 150;
        if (identity == FusionPlant.Bucket)
        {
            __instance.mState = PlantState.Notready;
            __instance.mStateCountdown = 0;
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class BucketHoldPatch
{
    private static bool Prefix(Plant __instance) => !BucketPlants.Holding(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.MouseDown))]
internal static class BucketMousePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Board __instance, int x, int y, int theClickCount, int playerIndex)
    {
        try { return BucketPlants.MouseDown(__instance, x, y, theClickCount, playerIndex); }
        catch (Exception error)
        {
            FusionMod.Instance.LoggerInstance.Error($"Bucket placement failed; remaining item retained: {error}");
            return false;
        }
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class BucketBoardCleanupPatch
{
    private static void Prefix(Board __instance)
    { BucketPlants.Clear(); BucketDrops.Clear(__instance); ImportedPeaVisuals.ClearEyesZombies(__instance); }
}

[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Serialize))]
internal static class BucketSavePatch
{
    private static void Prefix() { BucketPlants.Clear(); BucketDrops.BeforeSave(); ImportedPeaVisuals.ClearEyesZombies(); }
}

// Potato Mine supplies the native low-plant target/terrain category only.
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdatePotato))]
internal static class BucketPotatoPatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.Bucket
        && FusionPlant.From(__instance) != FusionPlant.FootballHelmet;
}

// Both tools use the native glove cursor. An explicit LawnGlove toolbar action
// must release our selection before the glove arms, even if its enum is unchanged.
[HarmonyPatch]
internal static class BucketGloveToolChangePatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(LawnGloveMod), "Toggle"),
        AccessTools.Method(typeof(LawnGloveMod), "Activate") };
    private static void Prefix() => BucketPlants.Clear();
}
