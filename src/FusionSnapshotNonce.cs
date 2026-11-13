using HarmonyLib;
using Il2CppReloaded.Gameplay;
using F = FusionMod.FusionSaveFormat;
using R = FusionMod.FusionNativeTimeRecords;

namespace FusionMod;

internal static class FusionSnapshotNonce
{
    internal sealed class Preparation
    {
        internal readonly Board Board;
        internal readonly long Generation;
        internal readonly GridItem Item;
        internal readonly uint ItemId;
        internal readonly F.Nonce Value, Previous;
        internal readonly bool Created;
        internal int? WriterOffset { get; set; }
        private R.Entry? clock;
        private int clockRate, clockCounter, clockA, clockB, clockC, clockD;
        internal Preparation(Board board, GridItem item, F.Nonce value, F.Nonce previous, bool created)
        { Board = board; Generation = FusionProjectileRecords.Generation(board); Item = item; ItemId = item.DataID;
            Value = value; Previous = previous; Created = created; }
        internal void Validate()
        {
            if (!FusionProjectileRecords.HasSession(Board) || FusionProjectileRecords.Generation(Board) != Generation
                || Item.mDead || Item.DataID != ItemId || Read(Item) != Value)
                throw new InvalidDataException("Fusion snapshot nonce owner changed during serialization.");
        }
        internal void CaptureClock()
        {
            clock = R.ReadClock(Board, out bool marked);
            if (!marked || clock == null || !clock.Valid || !R.ValidRate(clock.Rate))
                throw new InvalidDataException("Prepared Fusion clock is unavailable.");
            clockRate = clock.Rate; clockCounter = clock.Flags;
            clockA = BitConverter.SingleToInt32Bits(clock.A); clockB = BitConverter.SingleToInt32Bits(clock.B);
            clockC = BitConverter.SingleToInt32Bits(clock.C); clockD = BitConverter.SingleToInt32Bits(clock.D);
            ValidateClock();
        }
        internal void ValidateClock()
        {
            if (clock == null || !clock.Valid || clock.Rate != clockRate || clock.Flags != clockCounter
                || Board.mMainCounter != clockCounter || BitConverter.SingleToInt32Bits(clock.A) != clockA
                || BitConverter.SingleToInt32Bits(clock.B) != clockB || BitConverter.SingleToInt32Bits(clock.C) != clockC
                || BitConverter.SingleToInt32Bits(clock.D) != clockD)
                throw new InvalidDataException("Prepared Fusion clock changed before native snapshot publication.");
        }
        internal void Rollback()
        {
            if (Item.mDead || Item.DataID != ItemId || !FusionProjectileRecords.HasSession(Board)
                || FusionProjectileRecords.Generation(Board) != Generation) return;
            if (Created) Item.GridItemDie(); else Write(Item, Previous);
        }
    }
    internal static bool Reserved(GridItem item) => item.mSunCount == F.NonceMarker
        && FusionProjectileRecords.HasSession(item.mBoard);
    internal static F.Nonce Read(GridItem item)
    {
        if (item.mDead || item.mGridItemType != GridItemType.ZenTool || item.mGridItemState != GridItemState.Normal
            || item.mSunCount != F.NonceMarker || item.mGridItemCounter != F.NonceSchema
            || item.mX != -1000 || item.mY != -1000 || item.mVisible)
            throw new InvalidDataException("Invalid Fusion snapshot nonce record.");
        return F.Nonce.FromFields(item.mGridX, item.mGridY, item.mPosX, item.mPosY, item.mGoalX, item.mGoalY);
    }
    private static void Write(GridItem item, F.Nonce nonce)
    {
        var fields = nonce.Fields(); item.mGridX = fields.X; item.mGridY = fields.Y;
        item.mPosX = fields.PosX; item.mPosY = fields.PosY; item.mGoalX = fields.GoalX; item.mGoalY = fields.GoalY;
    }
    internal static Preparation Prepare(Board board)
    {
        if (!FusionProjectileRecords.HasSession(board)) throw new InvalidOperationException("Fusion save coordinator has no board session.");
        GridItem? nonceItem = null;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || item.mSunCount != F.NonceMarker) continue;
            if (nonceItem != null) throw new InvalidDataException("Duplicate Fusion snapshot nonce records.");
            _ = Read(item); nonceItem = item;
        }
        var clock = R.ReadClock(board, out bool clockMarked);
        if (clockMarked && (clock == null || !R.ValidRate(clock.Rate)))
            throw new InvalidDataException("Invalid required Fusion clock record.");
        int required = (nonceItem == null ? 1 : 0) + (clock == null ? 1 : 0);
        if (board.m_gridItems.Count < 0 || board.m_gridItems.Capacity < board.m_gridItems.Count
            || required > board.m_gridItems.Capacity - board.m_gridItems.Count)
            throw new IOException("Not enough native GridItem slots for the Fusion snapshot nonce and clock; previous save is preserved.");
        // This entire preparation precedes Board.Serialize and every store call.
        // Pending dead entries are included in Count and cannot supply a slot.
        bool created = nonceItem == null; F.Nonce previous = created ? default : Read(nonceItem!);
        F.Nonce value = F.Nonce.Create();
        if (created)
        {
            nonceItem = board.m_gridItems.DataArrayAlloc();
            if (nonceItem == null) throw new IOException("Cannot allocate the Fusion snapshot nonce.");
            try
            {
                nonceItem.mGridItemType = GridItemType.ZenTool; nonceItem.mGridItemState = GridItemState.Normal;
                nonceItem.mSunCount = F.NonceMarker; nonceItem.mGridItemCounter = F.NonceSchema;
                nonceItem.mX = nonceItem.mY = -1000; nonceItem.mVisible = false; Write(nonceItem, value);
                nonceItem.Init(board.mApp, GridItemType.ZenTool);
                if (nonceItem.mController == null) throw new IOException("Fusion snapshot nonce controller is unavailable.");
                nonceItem.mController.Hide();
            }
            catch { nonceItem.GridItemDie(); throw; }
        }
        else Write(nonceItem!, value);
        var preparation = new Preparation(board, nonceItem!, value, previous, created);
        try
        {
            // Capture must create and initialize its own phase record in one
            // operation. A schema-less placeholder would be rejected by the
            // existing marked-clock decoder on the next Capture invocation.
            if (!FusionTime.Capture(board)) throw new IOException("Cannot capture the required Fusion clock state.");
            preparation.CaptureClock(); preparation.Validate(); return preparation;
        }
        catch
        {
            if (clock == null && R.ReadClock(board, out _) is { } createdClock) R.Remove(createdClock);
            preparation.Rollback(); throw;
        }
    }
    internal static void VerifyLoaded(Board board, F.Nonce expected)
    {
        int count = 0;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || item.mSunCount != F.NonceMarker) continue;
            if (++count != 1 || Read(item) != expected) throw new InvalidDataException("Loaded Fusion snapshot nonce does not match the validated companion.");
        }
        if (count != 1) throw new InvalidDataException("Loaded Fusion snapshot nonce is missing.");
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class FusionSnapshotNonceUpdatePatch
{ private static bool Prefix(GridItem __instance) => !FusionSnapshotNonce.Reserved(__instance); }
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class FusionSnapshotNonceDrawPatch
{ private static bool Prefix(GridItem __instance) => !FusionSnapshotNonce.Reserved(__instance); }
[HarmonyPatch(typeof(GridItem), nameof(GridItem.PostUpdate))]
internal static class FusionSnapshotNoncePostUpdatePatch
{ private static bool Prefix(GridItem __instance) => !FusionSnapshotNonce.Reserved(__instance); }
