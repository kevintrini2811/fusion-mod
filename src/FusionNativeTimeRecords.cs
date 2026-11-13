using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native-decayed counters and native motion need a saved rate/provenance tag.
// Their owners define Flags and A-D; this owner only stores and finds records.
internal static class FusionNativeTimeRecords
{
    internal enum Kind { Status = 1, Projectile = 2, Crater = 3, Clock = 4 }
    private const int MarkerBase = -521090;
    internal const uint ClockEntity = 0xf17ec10c;
    private readonly record struct Key(IntPtr Board, Kind Kind, uint Entity);

    internal sealed class Entry
    {
        private readonly GridItem? item;
        private readonly uint itemId;
        private readonly ManagedValues? managed;
        internal readonly Board Board;
        internal readonly Kind Category;
        internal readonly uint EntityId;

        internal Entry(Board board, Kind kind, uint entity, GridItem item)
        { Board = board; Category = kind; EntityId = entity; this.item = item; itemId = item.DataID; }

        internal Entry(Board board, uint entity, ManagedValues values)
        { Board = board; Category = Kind.Projectile; EntityId = entity; managed = values; }

        internal bool Valid => managed != null ? managed.IsValid() : item != null && !item.mDead && item.DataID == itemId
            && IsRecord(item) && item.mSunCount == MarkerBase - (int)Category
            && unchecked((uint)item.mGridX) == EntityId;
        internal int Rate { get => managed?.Rate ?? item!.mGridItemCounter; set { if (managed != null) managed.Rate = value; else item!.mGridItemCounter = value; } }
        internal int Flags { get => managed?.Flags ?? item!.mGridY; set { if (managed != null) managed.Flags = value; else item!.mGridY = value; } }
        internal float A { get => managed?.A ?? item!.mPosX; set { if (managed != null) managed.A = value; else item!.mPosX = value; } }
        internal float B { get => managed?.B ?? item!.mPosY; set { if (managed != null) managed.B = value; else item!.mPosY = value; } }
        internal float C { get => managed?.C ?? item!.mGoalX; set { if (managed != null) managed.C = value; else item!.mGoalX = value; } }
        internal float D { get => managed?.D ?? item!.mGoalY; set { if (managed != null) managed.D = value; else item!.mGoalY = value; } }
        internal void Die()
        {
            // The target may already be dead. Managed removal authenticates
            // this exact entry/session itself and must still reclaim that key.
            if (managed != null) { managed.Remove(); return; }
            if (Valid) item!.GridItemDie();
        }
    }

    internal sealed class ManagedValues
    {
        private int rate, flags;
        private float a, b, c, d;
        internal Action? BeforeWrite;
        internal int Rate { get => rate; set { BeforeWrite?.Invoke(); rate = value; } }
        internal int Flags { get => flags; set { BeforeWrite?.Invoke(); flags = value; } }
        internal float A { get => a; set { BeforeWrite?.Invoke(); a = value; } }
        internal float B { get => b; set { BeforeWrite?.Invoke(); b = value; } }
        internal float C { get => c; set { BeforeWrite?.Invoke(); c = value; } }
        internal float D { get => d; set { BeforeWrite?.Invoke(); d = value; } }
        internal Func<bool> IsValid = null!;
        internal Action Remove = null!;
    }
    // Installed only after the complete save coordinator has registered its
    // guards. Status, Crater and Clock always retain their native backing.
    internal static Func<Board, uint, int, Entry?>? ProjectileLookup;
    internal static Func<Board, Entry[]>? ProjectileEntries;

    private static readonly Dictionary<Key, Entry> Cache = new();
    internal static int CurrentRate => (int)Math.Round(1 / FusionTime.NativeTickSeconds);
    internal static bool ValidRate(int rate) => rate is 100 or 120;

    internal static bool IsRecord(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal
        && item.mSunCount <= MarkerBase - 1 && item.mSunCount >= MarkerBase - 4;

    // createRate=0 is a lookup. Creating a record never converts entity fields.
    // Invalid saved provenance is retained and never guessed as legacy data.
    internal static Entry? Get(Board board, Kind kind, uint entity, int createRate = 0)
    {
        if (entity == 0) return null;
        if (kind == Kind.Projectile && ProjectileLookup != null) return ProjectileLookup(board, entity, createRate);
        if (kind != Kind.Clock && !ScaredContent.Playing(board)) return null;
        var key = new Key(board.Pointer, kind, entity);
        if (Cache.TryGetValue(key, out var cached))
        {
            if (cached.Valid) return ValidRate(cached.Rate) ? cached : null;
            Cache.Remove(key);
        }
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || !IsRecord(item)
                || item.mSunCount != MarkerBase - (int)kind
                || unchecked((uint)item.mGridX) != entity) continue;
            var found = new Entry(board, kind, entity, item);
            Cache[key] = found;
            return ValidRate(found.Rate) ? found : null;
        }
        if (createRate == 0) return null;
        if (!ValidRate(createRate)) throw new ArgumentOutOfRangeException(nameof(createRate));
        var allocated = board.m_gridItems.DataArrayAlloc();
        if (allocated == null)
        {
            if (kind != Kind.Clock)
                FusionMod.Instance.LoggerInstance.Error("Cannot allocate Fusion native-time provenance; native entity state is preserved.");
            return null;
        }
        try
        {
            allocated.Init(board.mApp, GridItemType.ZenTool);
            allocated.mGridItemState = GridItemState.Normal;
            allocated.mSunCount = MarkerBase - (int)kind;
            allocated.mGridX = unchecked((int)entity);
            allocated.mGridY = 0;
            allocated.mGridItemCounter = createRate;
            allocated.mX = allocated.mY = -1000;
            allocated.mPosX = allocated.mPosY = allocated.mGoalX = allocated.mGoalY = 0;
            return Cache[key] = new Entry(board, kind, entity, allocated);
        }
        catch { allocated.GridItemDie(); throw; }
    }

    // Clock lookup also recognizes malformed marked records. Never replace them as legacy.
    internal static Entry? ReadClock(Board board, out bool marked)
    {
        GridItem? found = null;
        marked = false;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || item.mSunCount != MarkerBase - (int)Kind.Clock) continue;
            if (marked) return null;
            marked = true; found = item;
        }
        return found != null && IsRecord(found) && unchecked((uint)found.mGridX) == ClockEntity
            ? new Entry(board, Kind.Clock, ClockEntity, found) : null;
    }

    internal static Entry[] Entries(Board board, Kind kind)
    {
        if (kind == Kind.Projectile && ProjectileEntries != null) return ProjectileEntries(board);
        var entries = new List<Entry>();
        var seen = new HashSet<uint>();
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || !IsRecord(item)
                || item.mSunCount != MarkerBase - (int)kind) continue;
            uint entity = unchecked((uint)item.mGridX);
            if (seen.Add(entity) && Get(board, kind, entity) is { } entry) entries.Add(entry);
        }
        return entries.ToArray();
    }

    internal static void Remove(Entry entry)
    {
        Cache.Remove(new Key(entry.Board.Pointer, entry.Category, entry.EntityId));
        entry.Die();
    }

    internal static int RebaseRemaining(int value, int fromRate, int toRate)
    {
        if (value <= 0 || fromRate == toRate) return value;
        if (!ValidRate(fromRate) || !ValidRate(toRate)) throw new ArgumentOutOfRangeException(nameof(fromRate));
        return (int)Math.Min(int.MaxValue, Math.Ceiling((double)value * toRate / fromRate - 1e-9));
    }

    internal static int RebaseAge(int value, int fromRate, int toRate)
    {
        if (value <= 0 || fromRate == toRate) return value;
        if (!ValidRate(fromRate) || !ValidRate(toRate)) throw new ArgumentOutOfRangeException(nameof(fromRate));
        return (int)Math.Min(int.MaxValue, Math.Floor((double)value * toRate / fromRate + 1e-9));
    }

    internal static float VelocityScale(int fromRate, int toRate)
    {
        if (!ValidRate(fromRate) || !ValidRate(toRate)) throw new ArgumentOutOfRangeException(nameof(fromRate));
        return (float)fromRate / toRate;
    }

    internal static void Forget(Board? board)
    {
        if (board == null) return;
        foreach (var key in Cache.Keys.Where(key => key.Board == board.Pointer).ToArray()) Cache.Remove(key);
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class FusionNativeTimeItemUpdatePatch
{ private static bool Prefix(GridItem __instance) => !FusionNativeTimeRecords.IsRecord(__instance); }

[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class FusionNativeTimeItemDrawPatch
{ private static bool Prefix(GridItem __instance) => !FusionNativeTimeRecords.IsRecord(__instance); }

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class FusionNativeTimeDisposePatch
{ private static void Prefix(Board __instance) => FusionNativeTimeRecords.Forget(__instance); }

[HarmonyPatch(typeof(Board), nameof(Board.InitLevel))]
internal static class FusionNativeTimeInitializePatch
{ private static void Prefix(Board __instance) => FusionNativeTimeRecords.Forget(__instance); }

[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Deserialize))]
internal static class FusionNativeTimeLoadPatch
{
    private static void Prefix(SerializationService __instance) => FusionNativeTimeRecords.Forget(__instance.App?.Board);
    private static void Postfix(SerializationService __instance, bool __result)
    { if (__result) FusionNativeTimeRecords.Forget(__instance.App?.Board); }
}
