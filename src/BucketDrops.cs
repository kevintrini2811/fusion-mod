using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// User-defined drop chance; original Fusion item appearance and lifetime.
internal static class BucketDrops
{
    internal sealed class Drop
    {
        internal readonly Board Board;
        internal readonly int Row;
        internal readonly float? SpawnX, SpawnGroundY;
        internal FusionPlant Identity = FusionPlant.Bucket;
        internal float X, GroundY, Height = 55, VelocityX, VelocityZ = -3.2f;
        internal int Age;
        internal bool VisualFailed;
        internal GridItem? Persistence;
        internal uint PersistenceId;
        internal ImportedPeaVisuals.BucketDropView? Visual;
        internal float Y => GroundY - Height;
        internal Drop(Board board, int row, float x, float groundY, float velocity, bool knownSpawn = true)
        {
            Board = board; Row = row; X = x; GroundY = groundY; VelocityX = velocity;
            // Fresh callers supply the materialized position after their X clamp.
            // Saved heads retain motion state, not their historical spawn origin.
            SpawnX = knownSpawn ? x : null;
            SpawnGroundY = knownSpawn ? groundY : null;
        }
    }
    internal sealed record Hit(Board Board, uint Id, bool Football = false)
    {
        internal bool Completed;
    }
    private static readonly List<Drop> drops = new();
    internal static int Count => drops.Count;
    internal readonly record struct DropSnapshot(FusionPlant Identity, int Row, float X, float Y, int Age,
        float? SpawnX, float? SpawnGroundY);
    internal static IReadOnlyList<DropSnapshot> Snapshot(Board? board)
    {
        if (board == null || !Playing(board)) return Array.Empty<DropSnapshot>();
        // Observe only materialized owner state. Reading never restores saved
        // items, advances lifetime/physics or exposes mutable Drop instances.
        return Array.AsReadOnly(drops.Where(drop => drop.Board.Pointer == board.Pointer)
            .Select(drop => new DropSnapshot(drop.Identity, drop.Row, drop.X, drop.Y, drop.Age, drop.SpawnX, drop.SpawnGroundY)).ToArray());
    }
    private static bool Playing(Board board) => board.mApp != null
        && board.mApp.GameScene == GameScenes.Playing
        && board.mApp.GameMode != GameMode.ChallengeZenGarden
        && board.Pointer != CherryBombPlants.DisposingBoard;

    internal static Hit? BeforeDamage(Zombie zombie, DamageFlags flags)
    {
        var board = zombie.mBoard;
        return board != null && Playing(board) && !board.mPaused && !zombie.mDead
            && !zombie.IsDeadOrDying() && !zombie.mMindControlled
            && (zombie.mHelmType == HelmType.Pail || zombie.mHelmType == HelmType.Football && zombie.mZombieType == ZombieType.Football) && zombie.mHelmHealth > 0
            && (flags & DamageFlags.DoesntLeaveBody) == 0
            ? new(board, zombie.DataID, zombie.mHelmType == HelmType.Football) : null;
    }
    internal static void AfterDamage(Zombie zombie, Hit? before)
    {
        // Wait for the complete hit. Existing bucket drops require a surviving
        // body; football helmets also permit ordinary lethal damage.
        // Magnet extraction never enters this damage transition.
        if (before is not { } hit || hit.Completed) return;
        hit.Completed = true;
        if (zombie.DataID != hit.Id
            || zombie.mBoard?.Pointer != hit.Board.Pointer || !Playing(hit.Board)
            || !hit.Football && (zombie.mDead || zombie.IsDeadOrDying() || zombie.mBodyHealth <= 0)
            || zombie.mHelmHealth > 0 || zombie.mMindControlled) return;
        if ((hit.Board.Rand() & 1) != 0) return;
        float x = Math.Clamp(zombie.mPosX + 40, 30, 770);
        float y = zombie.mPosY + zombie.mHeight - 20;
        float velocity = (hit.Board.Rand() % 2401) / 1000f - 1.2f;
        drops.Add(new Drop(hit.Board, zombie.mRow, x, y, velocity)
        { Identity = hit.Football ? FusionPlant.FootballHelmet : FusionPlant.Bucket });
    }
    internal static void Add(Board board, int row, float x, float y, FusionPlant identity)
    {
        float velocity = (board.Rand() % 2401) / 1000f - 1.2f;
        var drop = new Drop(board, row, Math.Clamp(x, 30, 770), y, velocity) { Identity = identity };
        if (identity == FusionPlant.EyesZombieHead)
        {
            var item = board.m_gridItems.DataArrayAlloc();
            if (item == null) return;
            try
            {
                item.Init(board.mApp, GridItemType.ZenTool);
                item.mGridItemState = GridItemState.Normal;
                item.mSunCount = -521013;
                item.mX = item.mY = -1000;
                drop.Persistence = item; drop.PersistenceId = item.DataID;
                Save(drop);
            }
            catch { item.GridItemDie(); throw; }
        }
        drops.Add(drop);
    }
    internal static bool IsSavedHead(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == -521013;
    private static void Save(Drop drop)
    {
        var item = drop.Persistence;
        if (item == null || item.mDead || item.DataID != drop.PersistenceId) return;
        item.mGridX = drop.Row;
        item.mGridY = BitConverter.SingleToInt32Bits(drop.VelocityX);
        item.mGridItemCounter = drop.Age;
        item.mPosX = drop.X; item.mPosY = drop.GroundY;
        item.mGoalX = drop.Height; item.mGoalY = drop.VelocityZ;
    }
    private static void Restore(Board board)
    {
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || !IsSavedHead(item)
                || drops.Any(d => d.Board.Pointer == board.Pointer && d.Persistence?.Pointer == item.Pointer
                    && d.PersistenceId == item.DataID)) continue;
            float velocity = BitConverter.Int32BitsToSingle(item.mGridY);
            if (item.mGridX < 0 || item.mGridX >= board.GetNumRows() || item.mGridItemCounter is < 0 or > 800
                || !float.IsFinite(item.mPosX) || !float.IsFinite(item.mPosY) || !float.IsFinite(item.mGoalX)
                || !float.IsFinite(item.mGoalY) || !float.IsFinite(velocity))
            { item.GridItemDie(); continue; }
            drops.Add(new Drop(board, item.mGridX, item.mPosX, item.mPosY, velocity, knownSpawn: false)
            { Identity = FusionPlant.EyesZombieHead, Height = item.mGoalX, VelocityZ = item.mGoalY,
                Age = item.mGridItemCounter, Persistence = item, PersistenceId = item.DataID });
        }
    }
    internal static bool Contains(Drop? drop) => drop != null && drops.Contains(drop) && Playing(drop.Board);
    internal static bool TryPick(Board board, int x, int y, out Drop? picked)
    {
        picked = drops.LastOrDefault(d => d.Board.Pointer == board.Pointer
            && Math.Abs(d.X - x) <= 28 && Math.Abs(d.Y - y) <= 30);
        return picked != null;
    }
    internal static void Consume(Drop drop)
    {
        if (!drops.Remove(drop)) return;
        drop.Visual?.Dispose(); drop.Visual = null;
        if (drop.Persistence is { mDead: false } item && item.DataID == drop.PersistenceId) item.GridItemDie();
    }
    internal static void Tick(Board board)
    {
        if (!Playing(board) || board.mPaused) return;
        Restore(board);
        if (!FusionTime.SourceStep(board, typeof(BucketDrops))) return;
        foreach (var drop in drops.Where(d => d.Board.Pointer == board.Pointer).ToArray())
        {
            if (BucketPlants.HoldingDrop(drop)) continue;
            if (++drop.Age > 800) { Consume(drop); continue; }
            if (drop.Height <= 0) { Save(drop); continue; }
            float oldX = drop.X;
            drop.X = Math.Clamp(drop.X + drop.VelocityX, 30, 770);
            drop.GroundY += board.GetPosYBasedOnRow(drop.X, drop.Row) - board.GetPosYBasedOnRow(oldX, drop.Row);
            drop.VelocityZ += .0784f;
            drop.Height = Math.Max(0, drop.Height - drop.VelocityZ);
            Save(drop);
        }
    }
    internal static void UpdateVisuals()
    {
        foreach (var drop in drops.ToArray())
        {
            if (!Playing(drop.Board)) continue;
            try
            {
                if (drop.VisualFailed) continue;
                drop.Visual ??= new ImportedPeaVisuals.BucketDropView(drop.Board, drop.Identity);
                float alpha = BucketPlants.HoldingDrop(drop) ? 0
                    : drop.Age <= 400 || drop.Age / 25 % 2 == 0 ? 1 : .25f;
                drop.Visual.Update(drop.X, drop.Y, alpha);
            }
            catch (Exception error)
            {
                drop.Visual?.Dispose(); drop.Visual = null; drop.VisualFailed = true;
                FusionMod.Instance.LoggerInstance.Error($"Bucket drop art failed: {error}");
            }
        }
    }
    internal static void BeforeSave()
    {
        // Never serialize orphan presentation controllers.
        foreach (var drop in drops)
        { Save(drop); drop.Visual?.Dispose(); drop.Visual = null; drop.VisualFailed = false; }
    }
    internal static void Clear(Board? board = null)
    {
        foreach (var drop in drops.Where(d => board == null || d.Board.Pointer == board.Pointer).ToArray()) Consume(drop);
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class SavedHeadUpdatePatch
{
    private static bool Prefix(GridItem __instance) => !BucketDrops.IsSavedHead(__instance);
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class SavedHeadDrawPatch
{
    private static bool Prefix(GridItem __instance) => !BucketDrops.IsSavedHead(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class BucketHelmLossPatch
{
    private static void Prefix(Zombie __instance, DamageFlags theDamageFlags, out BucketDrops.Hit? __state) =>
        __state = BucketDrops.BeforeDamage(__instance, theDamageFlags);
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Zombie __instance, BucketDrops.Hit? __state) => BucketDrops.AfterDamage(__instance, __state);
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class BucketDropTickPatch
{
    private static void Postfix(Board __instance) => BucketDrops.Tick(__instance);
}
