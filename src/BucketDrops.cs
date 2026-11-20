using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

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
        internal MagnetItemType GeneratedMetal;
        internal MagnetItemType Metal => GeneratedMetal != MagnetItemType.None ? GeneratedMetal
            : Identity == FusionPlant.Bucket ? MagnetItemType.Pail1
            : Identity == FusionPlant.FootballHelmet ? MagnetItemType.FootballHelmet1 : MagnetItemType.None;
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
    private const int SavedHead = -521013, SavedMetal = -521100;
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
        if (identity != FusionPlant.EyesZombieHead || Persist(drop, SavedHead)) drops.Add(drop);
    }

    private static bool Persist(Drop drop, int marker)
    {
        var item = drop.Board.m_gridItems.DataArrayAlloc();
        if (item == null) return false;
        try
        {
            item.Init(drop.Board.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = marker;
            item.mX = item.mY = -1000;
            drop.Persistence = item; drop.PersistenceId = item.DataID;
            Save(drop);
            return true;
        }
        catch { item.GridItemDie(); throw; }
    }

    private static readonly MagnetItemType[] SolarMetals = { MagnetItemType.Pail1, MagnetItemType.FootballHelmet1,
        MagnetItemType.PickAxe, MagnetItemType.JackInTheBox, MagnetItemType.Door1, MagnetItemType.Pogo1 };

    internal static void ProduceMetal(Plant plant)
    {
        var board = plant.mBoard;
        int col = plant.mPlantCol + 1, row = plant.mRow;
        var kind = SolarMetals[board.Rand(SolarMetals.Length)];
        float x = Math.Clamp(plant.mX + plant.mWidth / 2f + 80, 30, 770);
        float y = board.GetPosYBasedOnRow(x, row) + 40;
        var neighbor = col < 9 ? board.GetPlantsOnLawn(col, row).NormalPlant : null;
        if (neighbor != null && MagnetFusions.TryReceive(neighbor, kind, x, y - 55)) return;
        AddMetal(board, row, x, y, kind, height: 55);
        board.mApp.SoundSystem.PlayFoley(FoleyType.MagnetShroom);
    }

    private static void AddMetal(Board board, int row, float x, float y, MagnetItemType type, float height, int age = 0)
    {
        var drop = new Drop(board, row, x, y, 0)
        { Identity = Plantable(type), GeneratedMetal = type, Height = height, Age = age };
        if (Persist(drop, SavedMetal - (int)type)) drops.Add(drop);
    }

    internal static FusionPlant Plantable(MagnetItemType type) => type switch
    {
        MagnetItemType.Pail1 or MagnetItemType.Pail2 or MagnetItemType.Pail3 => FusionPlant.Bucket,
        MagnetItemType.FootballHelmet1 or MagnetItemType.FootballHelmet2 or MagnetItemType.FootballHelmet3 => FusionPlant.FootballHelmet,
        _ => SeedType.None,
    };

    internal static bool IsEquipment(MagnetItemType type) => type != MagnetItemType.None
        && type is not (MagnetItemType.SilverCoin or MagnetItemType.GoldCoin or MagnetItemType.Diamond);

    private static MagnetItemType SavedMetalType(GridItem item)
    {
        var type = (MagnetItemType)(SavedMetal - item.mSunCount);
        return Array.IndexOf(SolarMetals, type) >= 0 ? type : MagnetItemType.None;
    }

    private static bool IsSavedTool(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal;
    internal static bool IsSavedHead(GridItem item) => IsSavedTool(item) && item.mSunCount == SavedHead;
    internal static bool IsSavedDrop(GridItem item) => IsSavedHead(item)
        || IsSavedTool(item) && SavedMetalType(item) != MagnetItemType.None;
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
            if (item == null || item.mDead || !IsSavedDrop(item)
                || drops.Any(d => d.Board.Pointer == board.Pointer && d.Persistence?.Pointer == item.Pointer
                    && d.PersistenceId == item.DataID)) continue;
            float velocity = BitConverter.Int32BitsToSingle(item.mGridY);
            if (item.mGridX < 0 || item.mGridX >= board.GetNumRows() || item.mGridItemCounter is < 0 or > 800
                || !float.IsFinite(item.mPosX) || !float.IsFinite(item.mPosY) || !float.IsFinite(item.mGoalX)
                || !float.IsFinite(item.mGoalY) || !float.IsFinite(velocity))
            { item.GridItemDie(); continue; }
            var metal = SavedMetalType(item);
            drops.Add(new Drop(board, item.mGridX, item.mPosX, item.mPosY, velocity, knownSpawn: false)
            { Identity = metal == MagnetItemType.None ? FusionPlant.EyesZombieHead : Plantable(metal),
                GeneratedMetal = metal, Height = item.mGoalX, VelocityZ = item.mGoalY,
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
    // Transfer one unheld physical metal item to the magnet's serialized slot.
    // Eyes heads are not metal. No second drop or lifetime reroll is created.
    internal static bool TakeForMagnet(Plant plant, float radius, int rows, out int row)
    {
        row = 0;
        var drop = drops.FirstOrDefault(d => d.Board.Pointer == plant.mBoard.Pointer
            && d.Metal != MagnetItemType.None && !BucketPlants.HoldingDrop(d)
            && MagnetFusions.InRange(plant, d.X, d.Y, d.Row, radius, rows));
        if (drop == null) return false;
        row = drop.Row;
        MagnetFusions.Capture(plant, drop.Metal, drop.X, drop.Y);
        int kind = drop.GeneratedMetal != MagnetItemType.None ? 3 : drop.Identity == FusionPlant.FootballHelmet ? 2 : 1;
        plant.mTargetY = MagnetFusions.StoredLight(plant) * 4 + kind;
        plant.mLaunchRate = drop.Age;
        Consume(drop);
        return true;
    }
    internal sealed record MagnetCapture(Board Board, uint Id, int Kind, int Age, int Row, float X, float Y, MagnetItemType Metal);
    internal static MagnetCapture? CaptureMagnet(Plant plant)
    {
        int kind = plant.mTargetY % 4;
        if (kind == 0 || plant.mMagnetItems[0].ItemType == MagnetItemType.None) return null;
        var item = plant.mMagnetItems[0];
        return new(plant.mBoard, plant.DataID, kind, Math.Clamp(plant.mLaunchRate, 0, 800),
            plant.mRow, item.PosX, item.PosY, item.ItemType);
    }
    internal static void ReleaseFromMagnet(Plant plant, MagnetCapture capture)
    {
        // Called only after a confirmed death of this exact owner generation.
        // Native death may have cleared its item slots, so use the pre-death data.
        plant.mTargetY = plant.mLaunchRate = 0;
        if (!Playing(capture.Board) || PlayerPlantingScope.ConsumingIngredient == plant.Pointer) return;
        if (capture.Kind == 3)
        {
            AddMetal(capture.Board, capture.Row, capture.X, capture.Y, capture.Metal, height: 0, capture.Age);
            return;
        }
        drops.Add(new Drop(capture.Board, capture.Row, capture.X, capture.Y, 0)
        { Identity = capture.Kind == 2 ? FusionPlant.FootballHelmet : FusionPlant.Bucket,
            Age = capture.Age, Height = 0 });
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
                drop.Visual ??= new ImportedPeaVisuals.BucketDropView(drop.Board, drop.Identity, drop.GeneratedMetal);
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
    private static bool Prefix(GridItem __instance) => !BucketDrops.IsSavedDrop(__instance);
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class SavedHeadDrawPatch
{
    private static bool Prefix(GridItem __instance) => !BucketDrops.IsSavedDrop(__instance);
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
