using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using R = FusionMod.FusionNativeTimeRecords;

namespace FusionMod;

internal static class FusionProjectileLoad
{
    internal static void ForgetBoard(Board board)
    {
        BlazeShooters.ForgetBoard(board); IronProjectiles.ForgetBoard(board);
        RollingProjectiles.ForgetBoard(board); SplitPeaProjectiles.ForgetBoard(board);
        SolarShooter.ForgetProjectiles(board); PoisonShooters.ForgetProjectiles(board);
        ImportedPeaVisuals.ForgetProjectiles(board);
    }
    private static void Forget(Projectile shot)
    {
        BlazeShooters.Forget(shot); IronProjectiles.Forget(shot);
        RollingProjectiles.Forget(shot); SplitPeaProjectiles.Forget(shot);
        SolarShooter.Forget(shot); PoisonShooters.Forget(shot);
    }
    internal static bool LegacyNonce(GridItem item)
    {
        static bool Limb(float value) => float.IsFinite(value) && value >= 0 && value <= 65535
            && value == MathF.Truncate(value) && BitConverter.SingleToInt32Bits(value) != int.MinValue;
        return !item.mDead && item.mGridItemType == GridItemType.ZenTool && item.mGridItemState == GridItemState.Normal
            && item.mSunCount == -521095 && item.mGridItemCounter == 1 && item.mX == -1000 && item.mY == -1000
            && !item.mVisible && Limb(item.mPosX) && Limb(item.mPosY) && Limb(item.mGoalX) && Limb(item.mGoalY)
            && (item.mGridX != 0 || item.mGridY != 0 || item.mPosX != 0 || item.mPosY != 0 || item.mGoalX != 0 || item.mGoalY != 0);
    }

    internal static void Normalize(Board board)
    {
        ForgetBoard(board);
        // Native Deserialize has finished rebuilding its arrays/controllers.
        // Keep native slot/ID ownership and let the native initializer supply
        // the correct lane/roof geometry, motion, dimensions and controller.
        for (int i = 0; i < board.m_projectiles.Count; ++i)
        {
            var shot = board.m_projectiles[i];
            if (shot == null || shot.mDead || shot.mBoard?.Pointer != board.Pointer
                || shot.DataID == 0 || board.m_projectiles.DataArrayTryToGet(shot.DataID)?.Pointer != shot.Pointer
                || !FusionProjectileDomains.Marked(shot)) continue;
            uint id = shot.DataID; var pointer = shot.Pointer;
            if (!float.IsFinite(shot.mX) || !float.IsFinite(shot.mY) || shot.mRow < 0 || shot.mRow >= board.GetNumRows())
            { shot.Die(); continue; }
            int x = (int)Math.Clamp((double)shot.mX, int.MinValue, int.MaxValue);
            int y = (int)Math.Clamp((double)shot.mY, int.MinValue, int.MaxValue);
            int row = shot.mRow, order = shot.mRenderOrder;
            Forget(shot);
            shot.mController?.Die();
            // CreateProjectileController runs before ProjectileInitialize writes
            // most fields. Remove our identity before any controller hooks run.
            shot.mCobTargetX = 0; shot.mCobTargetRow = 0;
            shot.mProjectileType = ProjectileType.Pea; shot.mMotionType = ProjectileMotion.Straight;
            shot.ProjectileInitialize(x, y, order, row, ProjectileType.Pea);
            if (shot.DataID != id || shot.Pointer != pointer || shot.mBoard?.Pointer != board.Pointer
                || board.m_projectiles.DataArrayTryToGet(id)?.Pointer != pointer || FusionProjectileDomains.Marked(shot))
                throw new InvalidDataException("Native Fusion projectile replacement changed ownership or retained a Fusion marker.");
        }
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || item.mBoard?.Pointer != board.Pointer || item.DataID == 0
                || board.m_gridItems.DataArrayTryToGet(item.DataID)?.Pointer != item.Pointer) continue;
            if (item.mSunCount == -521092 && R.IsRecord(item) || LegacyNonce(item)) item.GridItemDie();
        }
        R.Forget(board);
    }
}

[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Deserialize))]
internal static class FusionProjectileLoadPatch
{
    [HarmonyPriority(Priority.First + 1000)]
    private static void Prefix(out bool __state)
    { FusionProjectileRecords.BeginLoad(); __state = true; }
    [HarmonyPriority(Priority.First)]
    private static void Postfix(SerializationService __instance, bool __result)
    {
        if (__result && FusionProjectileRecords.OutermostLoad && __instance.App?.Board is { } board)
            FusionProjectileLoad.Normalize(board);
    }
    [HarmonyPriority(Priority.Last + 1)]
    private static void Finalizer(SerializationService __instance, bool __result, bool __state, Exception? __exception)
    {
        if (!__state) return;
        try
        {
            // Native false paths precede Board.Deserialize. An exception can
            // follow partial native mutation, so old lifetimes cannot survive.
            if (__exception != null && FusionProjectileRecords.OutermostLoad && __instance.App?.Board is { } board)
                FusionProjectileLoad.ForgetBoard(board);
        }
        finally { FusionProjectileRecords.EndLoad(null, __result || __exception != null); }
    }
}

[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Serialize))]
internal static class FusionTimeSavePatch
{
    [HarmonyPriority(Priority.First + 100)]
    private static void Prefix(SerializationService __instance)
    {
        // Board.Serialize carries a ref-struct writer. Keep that native method
        // outside Harmony trampolines and prepare at its ordinary caller.
        var board = __instance.App?.Board;
        if (board == null) return;
        if (!FusionTime.Capture(board)) throw new IOException("Cannot capture Fusion clock state before native save.");
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false } item && R.IsRecord(item))
                R.InitializeController?.Invoke(item);
    }
}
