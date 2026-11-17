using HarmonyLib;
using Il2CppReloaded.Gameplay;
using R = FusionMod.FusionNativeTimeRecords;

namespace FusionMod;

// Provenance exists only while this native board generation is alive. Loaded
// Fusion shots become ordinary peas before gameplay resumes, so no sidecar is
// required and no GridItem slot is ever allocated for a projectile.
internal static class FusionProjectileRecords
{
    private sealed class Session
    {
        internal readonly Board Board;
        internal readonly long Generation;
        internal readonly Dictionary<uint, R.Entry> Entries = new();
        internal Session(Board board, long generation) { Board = board; Generation = generation; }
    }
    private static readonly Dictionary<IntPtr, Session> Sessions = new();
    private static long nextGeneration;
    private static int unityThread, loadDepth;
    private static bool enabled;
    private static void RequireThread()
    {
        if (!enabled || Environment.CurrentManagedThreadId != unityThread)
            throw new InvalidOperationException("Fusion projectile memory requires its registered Unity thread.");
    }
    internal static void Initialize()
    {
        unityThread = Environment.CurrentManagedThreadId; enabled = true;
        R.ProjectileLookup = Get; R.ProjectileEntries = Entries;
    }
    internal static void Disable()
    {
        enabled = false; R.ProjectileLookup = null; R.ProjectileEntries = null;
        Sessions.Clear(); loadDepth = 0; unityThread = 0;
    }
    internal static bool Enabled => enabled;
    internal static bool Loading => loadDepth != 0;
    internal static bool OutermostLoad => loadDepth == 1;
    private static Session Current(Board board)
    {
        RequireThread();
        if (Sessions.TryGetValue(board.Pointer, out var value)) return value;
        var session = new Session(board, checked(++nextGeneration));
        Sessions.Add(board.Pointer, session); return session;
    }
    internal static long Generation(Board board) => Current(board).Generation;
    internal static bool HasSession(Board? board)
    { if (!enabled || board == null) return false; RequireThread(); return Sessions.ContainsKey(board.Pointer); }
    internal static void Forget(Board board)
    { if (!enabled) return; RequireThread(); Sessions.Remove(board.Pointer); }
    internal static void DisposeBoard(Board board) => Forget(board);
    internal static void BeginLoad()
    { RequireThread(); checked { ++loadDepth; } }
    internal static void EndLoad(Board? board, bool success = true)
    {
        RequireThread();
        try { if (loadDepth == 1 && success) Sessions.Clear(); }
        finally { if (--loadDepth < 0) { loadDepth = 0; throw new InvalidOperationException("Unbalanced Fusion load scope."); } }
    }
    internal static void Birth(Board board, Projectile? shot)
    {
        if (!enabled || shot == null) return;
        RequireThread();
        // Remove even an identical wrapped full ID before any attribution hook.
        if (Sessions.TryGetValue(board.Pointer, out var session)) session.Entries.Remove(shot.DataID);
    }
    private static Projectile? Shot(Board board, uint id)
    {
        var shot = board.m_projectiles.DataArrayTryToGet(id);
        return shot != null && !shot.mDead && shot.DataID == id
            && shot.mBoard?.Pointer == board.Pointer ? shot : null;
    }
    private static R.Entry? Get(Board board, uint id, int createRate)
    {
        RequireThread();
        if (loadDepth != 0 || !ScaredContent.Playing(board) || id == 0) return null;
        var session = Current(board);
        if (session.Entries.TryGetValue(id, out var found))
        {
            if (found.Valid) return R.ValidRate(found.Rate) ? found : null;
            session.Entries.Remove(id);
        }
        if (createRate == 0) return null;
        if (!R.ValidRate(createRate)) throw new ArgumentOutOfRangeException(nameof(createRate));
        var shot = Shot(board, id); if (shot == null) return null;
        var pointer = shot.Pointer;
        var values = new R.ManagedValues { Rate = createRate };
        R.Entry? entry = null;
        bool CurrentEntry() => Sessions.TryGetValue(board.Pointer, out var current)
            && ReferenceEquals(current, session) && current.Entries.TryGetValue(id, out var saved)
            && ReferenceEquals(saved, entry);
        values.IsValid = () => CurrentEntry() && shot.Pointer == pointer && !shot.mDead
            && shot.DataID == id && shot.mBoard?.Pointer == board.Pointer && Shot(board, id)?.Pointer == pointer;
        values.BeforeWrite = () =>
        {
            RequireThread();
            if (!values.IsValid()) throw new InvalidOperationException("Stale Fusion projectile entry cannot mutate a new lifetime.");
        };
        values.Remove = () =>
        { RequireThread(); if (CurrentEntry()) session.Entries.Remove(id); };
        entry = new R.Entry(board, id, values); session.Entries.Add(id, entry); return entry;
    }
    private static R.Entry[] Entries(Board board)
    {
        RequireThread();
        return Sessions.TryGetValue(board.Pointer, out var session)
            ? session.Entries.Values.ToArray() : Array.Empty<R.Entry>();
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.InitLevel))]
internal static class FusionProjectileRecordsInitPatch
{ private static void Prefix(Board __instance) => FusionProjectileRecords.Forget(__instance); }
[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class FusionProjectileRecordsDisposePatch
{ private static void Prefix(Board __instance) => FusionProjectileRecords.DisposeBoard(__instance); }
[HarmonyPatch]
internal static class FusionProjectileRecordsBirthPatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => typeof(Board).GetMethods()
        .Where(method => method.Name == nameof(Board.AddProjectile));
    [HarmonyPriority(Priority.First + 200)]
    private static void Postfix(Board __instance, Projectile? __result) => FusionProjectileRecords.Birth(__instance, __result);
}
