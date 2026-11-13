using HarmonyLib;
using Il2CppReloaded.Gameplay;
using F = FusionMod.FusionSaveFormat;
using R = FusionMod.FusionNativeTimeRecords;

namespace FusionMod;

// Runtime backing for Projectile-kind records. The coordinator installs this
// backend only after persistence and refusal hooks are ready. No native slot is
// allocated by Get, birth attribution, motion conversion or projectile cleanup.
internal static class FusionProjectileRecords
{
    internal readonly record struct OwnerBinding(F.Owner Identity, long Generation, IntPtr Service);
    private sealed class Session
    {
        internal readonly Board Board;
        internal readonly long Generation;
        internal readonly OwnerBinding? Owner;
        internal readonly Dictionary<uint, R.Entry> Entries = new();
        internal readonly Dictionary<uint, F.ProjectileRecord> Staged = new();
        internal bool Loading, Frozen;
        internal Session(Board board, long generation, OwnerBinding? owner) { Board = board; Generation = generation; Owner = owner; }
    }
    private static readonly Dictionary<IntPtr, Session> Sessions = new();
    private static readonly Dictionary<IntPtr, OwnerBinding> Bindings = new();
    private static long nextGeneration;
    private static bool enabled;
    private static int unityThread;
    private static void RequireThread()
    {
        if (Environment.CurrentManagedThreadId != unityThread)
            throw new InvalidOperationException("Fusion projectile persistence requires its registered Unity thread.");
    }
    internal static void EnableAfterCoordinatorReady()
    {
        if (enabled) return;
        unityThread = Environment.CurrentManagedThreadId;
        R.ProjectileLookup = Get; R.ProjectileEntries = Entries; enabled = true;
    }
    internal static void Disable()
    { enabled = false; R.ProjectileLookup = null; R.ProjectileEntries = null; Sessions.Clear(); Bindings.Clear(); }
    internal static bool Enabled => enabled;
    private static Session Current(Board board)
    {
        RequireThread();
        if (Sessions.TryGetValue(board.Pointer, out var session)) return session;
        OwnerBinding? owner = Bindings.TryGetValue(board.Pointer, out var bound) ? bound : null;
        if (FusionSaveCoordinator.Ready && owner == null)
            throw new InvalidOperationException("Fusion projectile board has no authorized owner generation.");
        session = new Session(board, checked(++nextGeneration), owner); Sessions.Add(board.Pointer, session);
        try { ImportLegacy(session); return session; }
        catch { Sessions.Remove(board.Pointer); throw; }
    }
    internal static long Generation(Board board) => Current(board).Generation;
    internal static void BindNewBoard(Board board, OwnerBinding owner)
    {
        RequireThread(); owner.Identity.Validate();
        Sessions.Remove(board.Pointer); Bindings[board.Pointer] = owner;
        _ = Current(board);
    }
    internal static bool MatchesOwner(Board board, OwnerBinding owner)
    {
        RequireThread();
        return Bindings.TryGetValue(board.Pointer, out var binding) && binding == owner
            && (!Sessions.TryGetValue(board.Pointer, out var session) || session.Owner == owner);
    }
    internal static bool HasSession(Board? board)
    { if (!enabled || board == null) return false; RequireThread(); return Sessions.ContainsKey(board.Pointer); }
    internal static void Forget(Board board)
    {
        if (!enabled) return; RequireThread();
        // A native InitLevel within the same staged Deserialize does not own
        // another load attempt and cannot discard its validated staging map.
        if (Sessions.TryGetValue(board.Pointer, out var session) && session.Loading) return;
        Sessions.Remove(board.Pointer);
    }
    internal static void DisposeBoard(Board board)
    { if (enabled) { RequireThread(); Sessions.Remove(board.Pointer); Bindings.Remove(board.Pointer); } }
    internal static void Birth(Board board, Projectile? shot)
    {
        if (!enabled || shot == null || shot.mDead) return;
        RequireThread();
        if (!Sessions.TryGetValue(board.Pointer, out var session) || session.Loading) return;
        if (session.Frozen) throw new InvalidOperationException("Projectile creation during a frozen Fusion snapshot.");
        // A native pool slot can be reused before the regular end-of-tick sweep.
        // Invalidate its exact full ID before the existing attribution postfix.
        session.Entries.Remove(shot.DataID); session.Staged.Remove(shot.DataID);
    }
    private static Projectile? Shot(Board board, uint id)
    {
        var shot = board.m_projectiles.DataArrayTryToGet(id);
        return shot != null && !shot.mDead && shot.DataID == id && shot.mBoard?.Pointer == board.Pointer ? shot : null;
    }
    private static R.Entry Bind(Session session, Projectile shot, F.ProjectileRecord record)
    {
        record.Validate(); uint id = record.Id; IntPtr pointer = shot.Pointer;
        var values = new R.ManagedValues { Rate = record.Rate, Flags = record.Flags, A = record.A, B = record.B };
        R.Entry? entry = null;
        values.BeforeWrite = () =>
        {
            RequireThread();
            if (!Sessions.TryGetValue(session.Board.Pointer, out var current) || !ReferenceEquals(current, session))
                throw new InvalidOperationException("Stale Fusion projectile entry cannot mutate another board generation.");
            if (!session.Frozen) return;
            var error = new InvalidOperationException("Projectile provenance mutation during a frozen Fusion snapshot.");
            FusionSaveCoordinator.FailActivity(session.Board.mApp, error); throw error;
        };
        values.IsValid = () => Sessions.TryGetValue(session.Board.Pointer, out var current) && ReferenceEquals(current, session)
            && current.Entries.TryGetValue(id, out var saved) && ReferenceEquals(saved, entry)
            && shot.Pointer == pointer && shot.DataID == id && !shot.mDead && shot.mBoard?.Pointer == session.Board.Pointer;
        values.Remove = () =>
        {
            RequireThread();
            if (!Sessions.TryGetValue(session.Board.Pointer, out var active) || !ReferenceEquals(active, session)
                || !session.Entries.TryGetValue(id, out var current) || !ReferenceEquals(current, entry)) return;
            values.BeforeWrite(); session.Entries.Remove(id);
        };
        entry = new R.Entry(session.Board, id, values); session.Entries.Add(id, entry); return entry;
    }
    private static R.Entry? Get(Board board, uint id, int createRate)
    {
        RequireThread();
        if (!ScaredContent.Playing(board) && !(Sessions.TryGetValue(board.Pointer, out var loading) && loading.Loading)) return null;
        var session = Current(board);
        if (session.Entries.TryGetValue(id, out var entry))
        {
            if (entry.Valid) return R.ValidRate(entry.Rate) ? entry : null;
            if (session.Frozen) throw new InvalidOperationException("Projectile identity changed during a frozen Fusion snapshot.");
            session.Entries.Remove(id);
        }
        var shot = Shot(board, id);
        if (shot == null) return null;
        if (session.Staged.Remove(id, out var staged)) return Bind(session, shot, staged);
        if (createRate == 0) return null;
        if (!R.ValidRate(createRate)) throw new ArgumentOutOfRangeException(nameof(createRate));
        if (session.Frozen || session.Loading)
            throw new InvalidDataException("Required Fusion projectile provenance is unavailable; legacy fallback is refused.");
        return Bind(session, shot, new(id, createRate, 0, 0, 0));
    }
    private static R.Entry[] Entries(Board board)
    {
        var session = Current(board);
        foreach (uint id in session.Staged.Keys.ToArray()) _ = Get(board, id, 0);
        return session.Entries.Values.ToArray();
    }
    private static void ImportLegacy(Session session)
    {
        // Validate every owned legacy scalar before retiring any of its native
        // backing records. An unrelated marker/type is never reclaimed.
        var imported = new List<(GridItem Item, uint ItemId, F.ProjectileRecord Record)>();
        var seen = new HashSet<uint>();
        for (int i = 0; i < session.Board.m_gridItems.Count; ++i)
        {
            var item = session.Board.m_gridItems[i];
            if (item == null || item.mDead || item.mSunCount != -521092) continue;
            uint id = unchecked((uint)item.mGridX);
            if (!R.IsRecord(item) || !seen.Add(id) || item.mGoalX != 0 || item.mGoalY != 0)
                throw new InvalidDataException("Malformed or duplicate legacy Fusion projectile provenance.");
            var record = new F.ProjectileRecord(id, item.mGridItemCounter, item.mGridY,
                BitConverter.SingleToInt32Bits(item.mPosX), BitConverter.SingleToInt32Bits(item.mPosY));
            record.Validate(); imported.Add((item, item.DataID, record));
        }
        foreach (var value in imported)
        {
            var shot = Shot(session.Board, value.Record.Id);
            if (shot != null && !FusionProjectileDomains.CanonicalAge(shot)) Bind(session, shot, value.Record);
        }
        foreach (var value in imported)
            if (!value.Item.mDead && value.Item.DataID == value.ItemId) value.Item.GridItemDie();
    }
    internal static void BeginLoad(Board board, OwnerBinding owner, IEnumerable<F.ProjectileRecord> records)
    {
        if (!enabled) throw new InvalidOperationException("Fusion persistence coordinator is not ready.");
        RequireThread();
        if (!MatchesOwner(board, owner)) throw new InvalidDataException("Fusion load map belongs to another profile or owner generation.");
        var session = new Session(board, checked(++nextGeneration), owner) { Loading = true };
        foreach (var record in records)
        {
            record.Validate();
            if (!session.Staged.TryAdd(record.Id, record)) throw new InvalidDataException("Duplicate staged Fusion projectile DataID.");
        }
        Sessions[board.Pointer] = session;
    }
    internal static void CompleteLoad(Board board)
    {
        var session = Current(board);
        foreach (uint id in session.Staged.Keys.ToArray())
            if (Get(board, id, 0) == null) throw new InvalidDataException("Fusion companion contains an absent projectile DataID.");
        ValidateComplete(session); session.Loading = false;
    }
    internal static void ReloadLegacy(Board board)
    { RequireThread(); Sessions.Remove(board.Pointer); _ = Current(board); }
    private static void ValidateComplete(Session session)
    {
        var live = new HashSet<uint>();
        for (int i = 0; i < session.Board.m_projectiles.Count; ++i)
        {
            var shot = session.Board.m_projectiles[i];
            if (shot == null || shot.mDead) continue;
            if (!live.Add(shot.DataID)) throw new InvalidDataException("Duplicate live native projectile DataID.");
            // Match Prepare exactly: canonical source-aged custom motion owns
            // no native-rate record. All record-bearing native domains do.
            if (FusionProjectileDomains.CanonicalAge(shot)) continue;
            bool required = session.Entries.ContainsKey(shot.DataID) || FusionProjectileDomains.TryFlags(shot, out _);
            if (required && (!session.Entries.TryGetValue(shot.DataID, out var entry) || !entry.Valid))
                throw new InvalidDataException("Live projectile is missing required Fusion provenance.");
        }
        foreach (var entry in session.Entries.Values)
        {
            if (!entry.Valid || !live.Contains(entry.EntityId)) continue;
            var shot = Shot(session.Board, entry.EntityId)!;
            if (FusionProjectileDomains.CanonicalAge(shot)) continue;
            new F.ProjectileRecord(entry.EntityId, entry.Rate, entry.Flags,
                BitConverter.SingleToInt32Bits(entry.A), BitConverter.SingleToInt32Bits(entry.B)).Validate();
            if (entry.C != 0 || entry.D != 0) throw new InvalidDataException("Unexpected Fusion projectile provenance extension.");
        }
    }
    internal static F.ProjectileRecord[] Freeze(Board board)
    {
        var session = Current(board);
        if (session.Loading || session.Frozen) throw new InvalidOperationException("Fusion projectile snapshot is already active.");
        ValidateComplete(session); session.Frozen = true;
        return session.Entries.Values.Where(entry => entry.Valid && Shot(board, entry.EntityId) is { } shot
                && !FusionProjectileDomains.CanonicalAge(shot))
            .Select(entry => new F.ProjectileRecord(entry.EntityId, entry.Rate, entry.Flags,
                BitConverter.SingleToInt32Bits(entry.A), BitConverter.SingleToInt32Bits(entry.B)))
            .OrderBy(record => record.Id).ToArray();
    }
    internal static void Unfreeze(Board board, long generation)
    {
        RequireThread();
        if (Sessions.TryGetValue(board.Pointer, out var session) && session.Generation == generation) session.Frozen = false;
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
