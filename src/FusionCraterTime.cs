using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class FusionCraterTime
{
    internal static void SetDuration(GridItem crater, int sourceTicks)
    {
        var record = FusionNativeTimeRecords.Get(crater.mBoard, FusionNativeTimeRecords.Kind.Crater,
            crater.DataID, FusionNativeTimeRecords.CurrentRate);
        if (record == null) return; // Preserve the existing native state on allocation failure.
        crater.mGridItemCounter = FusionTime.NativeTicks(sourceTicks);
        record.Rate = FusionNativeTimeRecords.CurrentRate;
        record.Flags = 1;
        record.A = sourceTicks * 6f;
        record.B = crater.mGridItemCounter;
    }

    internal static void Prepare(GridItem crater)
    {
        // Demise's mSunCount is a separate custom clock; only the ordinary
        // native crater lifetime in mGridItemCounter crosses this boundary.
        if (crater.mGridItemType != GridItemType.Crater
            || crater.mDead || crater.mBoard.mPaused) return;
        var record = FusionNativeTimeRecords.Get(crater.mBoard, FusionNativeTimeRecords.Kind.Crater, crater.DataID);
        // Old ordinary craters have no Fusion origin marker. Do not guess.
        if (record == null || record.Flags != 1) return;
        int rate = FusionNativeTimeRecords.CurrentRate;
        // A is exact remaining 1/600-second units; B is the last projected
        // native remainder. Native decay is observed without ceil accumulation.
        if (crater.mGridItemCounter > record.B || !float.IsFinite(record.A) || record.A < 0)
        { FusionNativeTimeRecords.Remove(record); return; }
        float remaining = Math.Max(0, record.A - (record.B - crater.mGridItemCounter) * (600f / record.Rate));
        crater.mGridItemCounter = (int)Math.Clamp(Math.Ceiling((double)remaining * rate / 600 - 1e-9), 0, int.MaxValue);
        record.A = remaining;
        record.B = crater.mGridItemCounter;
        record.Rate = rate;
    }

    internal static void Cleanup(Board board)
    {
        var live = new HashSet<uint>();
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false, mGridItemType: GridItemType.Crater } item) live.Add(item.DataID);
        foreach (var record in FusionNativeTimeRecords.Entries(board, FusionNativeTimeRecords.Kind.Crater))
            if (!live.Contains(record.EntityId)) FusionNativeTimeRecords.Remove(record);
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class FusionCraterTimeUpdatePatch
{
    [HarmonyPriority(Priority.First + 2000)]
    private static void Prefix(GridItem __instance) => FusionCraterTime.Prepare(__instance);
}
