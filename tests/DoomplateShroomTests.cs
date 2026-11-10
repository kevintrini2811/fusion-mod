using FusionMod;
using Il2CppReloaded.Gameplay;
using TickRate = Il2CppReloaded.Constants.Gameplay;

internal static class DoomplateTests
{
    private static int checks;
    private static void Check(bool condition, string name)
    { ++checks; if (!condition) throw new Exception(name); }
    private static void Near(double actual, double expected, string name) => Check(Math.Abs(actual - expected) < .0001, name);
    private static void NoActiveScopes(string name)
    {
        var field = typeof(DoomplateShroom).GetField("detonations", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
        Check(((System.Collections.ICollection)field.GetValue(null)!).Count == 0, name);
    }
    private static Plant Fresh(double rate = 100)
    {
        TickRate.TIME_FOR_UPDATE = 1 / rate;
        BucketDrops.Drops.Clear(); CherryBombPlants.DisposingBoard = IntPtr.Zero;
        var board = new Board(); var plant = new Plant { mBoard = board };
        board.m_plants.Add(plant); DoomplateShroom.Initialize(plant); return plant;
    }
    private static Zombie Target(Plant plant, int health = 1000)
    {
        var zombie = new Zombie { mBoard = plant.mBoard, mBodyHealth = health };
        plant.mBoard.m_zombies.Add(zombie); return zombie;
    }
    private static void Step(Plant plant, bool repeated = true)
    {
        FusionTime.BeginTick(plant.mBoard);
        DoomplateShroom.Tick(plant);
        if (repeated) DoomplateShroom.Tick(plant);
        ++plant.mBoard.mMainCounter;
    }
    private static void Cadence(int rate, int nativeTicks)
    {
        var plant = Fresh(rate); var zombie = Target(plant);
        for (int i = 0; i < nativeTicks - 1; ++i)
        { Step(plant); Check(!plant.mDead && zombie.Received == 0, $"no early blast {rate}/{i}"); }
        Step(plant);
        Check(plant.mDead && zombie.Received == 3240, $"source event at exact deadline {rate}");
        Check(BucketDrops.Drops.Count == 1 && plant.mBoard.Craters == 1, "one kill bucket and crater");
        Step(plant); DoomplateShroom.Detonate(plant);
        Check(zombie.Received == 3240 && BucketDrops.Drops.Count == 1, "no post-death replay");
        NoActiveScopes("normal detonation finally removes its scope");
    }
    private static void PausedAndUnavailable()
    {
        var plant = Fresh();
        foreach (Action<Plant> gate in new Action<Plant>[] {
            p => p.mBoard.mPaused = true, p => p.mIsAsleep = true, p => p.mSquished = true,
            p => p.OffGround = true, p => p.mOnBungeeState = PlantOnBungeeState.OnBungee,
            p => p.mIsOnBoard = false, p => p.mApp.GameScene = GameScenes.Loading,
            p => p.mApp.GameMode = GameMode.ChallengeZenGarden,
            p => CherryBombPlants.DisposingBoard = p.mBoard.Pointer })
        {
            plant = Fresh(); gate(plant); Step(plant);
            Check(plant.mLaunchCounter == 96 && plant.mBoard.Craters == 0, "inactive gate preserves saved timer");
        }
        plant = Fresh(); Step(plant); Check(plant.mLaunchCounter == 95, "same native entry consumed once");
        plant.mBoard.mPaused = true; int remaining = plant.mLaunchCounter;
        for (int i = 0; i < 10; ++i) Step(plant);
        Check(plant.mLaunchCounter == remaining, "pause does not consume fuse");
        plant.mBoard.mPaused = false; Step(plant); Check(plant.mLaunchCounter == remaining - 1, "resume consumes one source step");
    }
    private static void KillRewards()
    {
        var plant = Fresh(); var board = plant.mBoard;
        var killed = Target(plant); var survivor = Target(plant, 10000);
        var friend = Target(plant); friend.mMindControlled = true;
        var oldDead = Target(plant); oldDead.mDead = true;
        var oldDying = Target(plant); oldDying.Dying = true;
        var distant = Target(plant); distant.X = 600;
        var recycled = Target(plant); recycled.OnDamage = () => ++recycled.DataID;
        var transferred = Target(plant); transferred.OnDamage = () => transferred.mBoard = new Board();
        var flying = Target(plant); flying.Category = DamageRangeFlags.Flying;
        var submerged = Target(plant); submerged.Category = DamageRangeFlags.Submerged;
        float shadowX = plant.ShadowX, shadowY = plant.ShadowY;
        board.Random.Enqueue(0); board.Random.Enqueue(4000);
        plant.OnDie = () => { DoomplateShroom.Detonate(plant); plant.mX = 900; plant.mY = 900; plant.ShadowX = 999; plant.ShadowY = 999; };
        killed.OnDamage = () => DoomplateShroom.Detonate(plant);
        DoomplateShroom.Detonate(plant);
        Check(killed.Received == 3240 && survivor.mBodyHealth == 6760, "ash hits killed and surviving enemies exactly once");
        Check(friend.Received == 0 && oldDead.Received == 0 && oldDying.Received == 0 && distant.Received == 0, "friendly dead dying and outside radius excluded");
        Check(recycled.Received == 3240 && transferred.Received == 3240, "real damage precedes generation validation");
        Check(BucketDrops.Drops.Count == 3, "only confirmed stable kills grant buckets");
        Check(flying.Received == 3240 && submerged.Received == 3240, "source doom target categories");
        Check(killed.LastFlags == (DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody), "ash damage flags");
        Near(BucketDrops.Drops[0].X, shadowX - 2 * DoomplateShroom.SourceUnit, "drop x captures original shadow and negative source scatter");
        Near(BucketDrops.Drops[0].Y, shadowY + 2 * DoomplateShroom.SourceUnit, "drop y captures original shadow and positive source scatter");
        Check(BucketDrops.Drops.All(d => d.Row == 2 && d.Board == board && d.Identity == FusionPlant.Bucket), "drop row and board capture");
    }
    private static void DeathBoundaries()
    {
        foreach (Action<Plant> mutation in new Action<Plant>[] {
            p => p.Veto = true, p => p.OnDie = () => ++p.DataID,
            p => p.OnDie = () => p.mBoard = new Board() })
        {
            var plant = Fresh(); var board = plant.mBoard; var zombie = Target(plant);
            mutation(plant); DoomplateShroom.Detonate(plant);
            Check(zombie.Received == 0 && board.Craters == 0 && BucketDrops.Drops.Count == 0, "death veto or recycled source cannot blast");
            NoActiveScopes("veto or recycled source finally removes its scope");
        }
        var thrower = Fresh(); var target = Target(thrower);
        thrower.OnDie = () => throw new InvalidOperationException("native death failed");
        try { DoomplateShroom.Detonate(thrower); throw new Exception("missing death exception"); }
        catch (InvalidOperationException) {}
        Check(target.Received == 0 && thrower.mStartRow == 1, "death exception cannot reward or replay");
        NoActiveScopes("death exception finally removes its scope");
    }
    private static void StackAndCrater()
    {
        var plant = Fresh(120); var board = plant.mBoard;
        var support = new Plant { mBoard = board, Identity = FusionPlant.Other };
        var resistant = new Plant { mBoard = board, Identity = FusionPlant.DoomNut };
        var adjacent = new Plant { mBoard = board, Identity = FusionPlant.Other, mPlantCol = 4 };
        board.m_plants.AddRange(new[] { support, resistant, adjacent });
        DoomplateShroom.Detonate(plant);
        Check(support.mDead && !resistant.mDead && !adjacent.mDead, "native crater consumes only source tile stack with existing DoomNut exemption");
        var crater = board.m_gridItems.Single(g => g.mGridItemType == GridItemType.Crater);
        Check(crater.mGridX == 3 && crater.mGridY == 2 && crater.mGridItemCounter == 21600, "180 second crater in native 120 Hz units");
        TickRate.TIME_FOR_UPDATE = .01; FusionCraterTime.Prepare(crater);
        Check(crater.mGridItemCounter == 18000, "crater owner rebases native ticks");
    }
    private static GridItem Copy(GridItem source, Board board) => new() {
        mBoard = board, DataID = source.DataID, mDead = source.mDead,
        mGridItemType = source.mGridItemType, mGridItemState = source.mGridItemState,
        mGridX = source.mGridX, mGridY = source.mGridY, mGridItemCounter = source.mGridItemCounter,
        mSunCount = source.mSunCount, mPosX = source.mPosX, mPosY = source.mPosY,
        mGoalX = source.mGoalX, mGoalY = source.mGoalY };
    private static void SavedPhase()
    {
        var plant = Fresh(120); Target(plant);
        for (int i = 0; i < 17; ++i) Step(plant);
        TickRate.TIME_FOR_UPDATE = .01;
        for (int i = 0; i < 11; ++i) Step(plant);
        int remaining = plant.mLaunchCounter; long ticks = FusionTime.SourceTicks(plant.mBoard);
        FusionTime.Capture(plant.mBoard);
        var loaded = new Board { mMainCounter = plant.mBoard.mMainCounter };
        loaded.m_gridItems.AddRange(plant.mBoard.m_gridItems.Select(g => Copy(g, loaded)));
        var restored = new Plant { mBoard = loaded, mLaunchCounter = remaining };
        loaded.m_plants.Add(restored); var target = Target(restored);
        FusionTime.BeginLoad(); FusionTime.EndLoad(loaded, true);
        Check(restored.mLaunchCounter == remaining && FusionTime.SourceTicks(loaded) == ticks, "saved fields and real clock reconstruct without replay");
        TickRate.TIME_FOR_UPDATE = 1d / 120;
        while (!restored.mDead) Step(restored);
        Check(FusionTime.SourceTicks(loaded) - ticks == remaining && target.Received == 3240, "mixed-rate loaded remainder consumes exact source pulses");
        Check(BucketDrops.Drops.Count == 1, "loaded fuse rewards once");
    }
    private static void OtherDoomCaller()
    {
        var plant = Fresh(); var zombie = Target(plant);
        DoomEffects.Damage(plant.mBoard, 2, 280, 240, 100);
        Check(zombie.Received == 1800 && BucketDrops.Drops.Count == 0, "default doom damage and absent reward unchanged");
    }
    private static void Crushing()
    {
        var plant = Fresh(); var zombie = Target(plant); plant.mIsAsleep = true;
        Check(!DoomplateCrushPatch.Prefix(plant), "crushing uses inherited source OnCrash instead of ordinary squash");
        Check(plant.mDead && zombie.Received == 3240 && BucketDrops.Drops.Count == 1, "sleeping mushroom explodes immediately when crushed");
        Check(plant.mLaunchCounter == 96, "crushing does not wait for fuse");
        DoomplateCrushPatch.Prefix(plant);
        Check(zombie.Received == 3240 && BucketDrops.Drops.Count == 1, "repeated crush does not replay");
        var ordinary = Fresh(); ordinary.Identity = FusionPlant.Other;
        Check(DoomplateCrushPatch.Prefix(ordinary) && !ordinary.mDead, "ordinary native squash remains available");
    }
    private static void DisposalDuringDamage()
    {
        var plant = Fresh(); var board = plant.mBoard; var target = Target(plant);
        target.OnDamage = () => CherryBombPlants.DisposingBoard = board.Pointer;
        DoomplateShroom.Detonate(plant);
        Check(target.Received == 3240, "damage can enter native board disposal");
        Check(BucketDrops.Drops.Count == 0, "damage-triggered board disposal cannot append a stale bucket");
        Check(board.Craters == 0, "damage-triggered board disposal cannot create a stale crater");
        plant = Fresh(); board = plant.mBoard; target = Target(plant);
        var laterTarget = Target(plant);
        target.OnDamage = () =>
        {
            CherryBombPlants.DisposingBoard = board.Pointer;
            board.DisposeBoard();
            CherryBombPlants.DisposingBoard = IntPtr.Zero;
        };
        DoomplateShroom.Detonate(plant);
        Check(CherryBombPlants.DisposingBoard == IntPtr.Zero, "native disposal pointerguard already restored before damage returns");
        Check(BucketDrops.Drops.Count == 0 && board.Craters == 0,
            "completed synchronous disposal permanently invalidates captured reward and crater");
        Check(laterTarget.Received == 0 && board.mApp.Particles == 0 && board.mApp.SoundSystem.Calls == 0,
            "completed disposal aborts later target and effect writes");
        NoActiveScopes("completed disposal finally removes its scope");
        plant = Fresh(); board = plant.mBoard; target = Target(plant);
        plant.OnDie = () => board.DisposeBoard();
        DoomplateShroom.Detonate(plant);
        Check(target.Received == 0 && BucketDrops.Drops.Count == 0, "disposal during plant death invalidates the entire pending payload");
        plant = Fresh(); board = plant.mBoard; target = Target(plant);
        var nested = new Plant { mBoard = board }; DoomplateShroom.Initialize(nested); board.m_plants.Add(nested);
        var nestedTarget = Target(nested);
        target.OnDamage = () => DoomplateShroom.Detonate(nested);
        nestedTarget.OnDamage = () =>
        {
            CherryBombPlants.DisposingBoard = board.Pointer;
            board.DisposeBoard();
            CherryBombPlants.DisposingBoard = IntPtr.Zero;
        };
        DoomplateShroom.Detonate(plant);
        Check(target.Received == 3240 && nestedTarget.Received == 3240, "nested same-board damage completes before disposal returns");
        Check(BucketDrops.Drops.Count == 0 && board.Craters == 0,
            "one completed disposal invalidates every nested active Doomplate payload");
        Check(board.mApp.Particles == 0, "nested invalidation suppresses both pending visual writes");
        NoActiveScopes("nested disposal finally removes every scope");
        plant = Fresh(); board = plant.mBoard;
        var firstSupport = new Plant { mBoard = board, Identity = FusionPlant.Other };
        var laterSupport = new Plant { mBoard = board, Identity = FusionPlant.Other };
        board.m_plants.AddRange(new[] { firstSupport, laterSupport });
        firstSupport.OnDie = () =>
        {
            CherryBombPlants.DisposingBoard = board.Pointer;
            board.DisposeBoard();
            CherryBombPlants.DisposingBoard = IntPtr.Zero;
        };
        DoomplateShroom.Detonate(plant);
        Check(firstSupport.mDead && !laterSupport.mDead && board.Craters == 0,
            "disposal during crater stack cleanup prevents later deaths and crater allocation");
        plant = Fresh(); Target(plant); DoomplateShroom.Detonate(plant);
        Check(BucketDrops.Drops.Count == 1 && plant.mBoard.Craters == 1, "valid kill reward remains available after disposal guard");
    }
    private static void Main()
    {
        Cadence(100, 96); Cadence(120, 116); PausedAndUnavailable(); KillRewards();
        DeathBoundaries(); StackAndCrater(); SavedPhase(); OtherDoomCaller(); Crushing(); DisposalDuringDamage();
        NoActiveScopes("suite leaves no active detonation scope");
        Console.WriteLine($"PASS {checks} Doomplate owner checks; real source clock, records, ash and crater owners. Native bodies are boundary models.");
    }
}
