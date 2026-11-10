using FusionMod;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using NativeClock = Il2CppReloaded.Constants.Gameplay;
using System.Reflection;
using System.Collections;

static class SaveTests
{
    static int checks;
    const BindingFlags Hidden = BindingFlags.Static | BindingFlags.NonPublic;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static void Near(double actual, double expected, string message) => Check(Math.Abs(actual - expected) < 1e-8, message);
    static object? Hook(Type owner, string method, params object?[] args) => owner.GetMethod(method, Hidden)!.Invoke(null, args);
    static void BeginLoad() => Hook(typeof(FusionTimeLoadPatch), "Prefix", new object?[] { false });
    static void EndLoad(SerializationService service, bool success = true, Exception? error = null) =>
        Hook(typeof(FusionTimeLoadPatch), "Finalizer", service, success, true, error);
    static SerializationService Service(Board? board) => new() { App = new App { Board = board } };
    static void Capture(Board board) => Hook(typeof(FusionTimeSavePatch), "Prefix", board);
    static long Step(Board board, int count, int rate)
    {
        NativeClock.TIME_FOR_UPDATE = 1d / rate;
        for (int i = 0; i < count; i++) { FusionTime.BeginTick(board); board.mMainCounter++; }
        return (long)count * (rate == 100 ? 6 : 5);
    }
    // Copy only actual serialized native scalar fields, not managed Clock/Entry wrappers.
    static GridItem Copy(GridItem item) => new() {
        DataID = item.DataID, mDead = item.mDead, mGridItemType = item.mGridItemType, mGridItemState = item.mGridItemState,
        mSunCount = item.mSunCount, mGridX = item.mGridX, mGridY = item.mGridY, mGridItemCounter = item.mGridItemCounter,
        mPosX = item.mPosX, mPosY = item.mPosY, mGoalX = item.mGoalX, mGoalY = item.mGoalY
    };
    static Board Fields(Board original)
    {
        Capture(original);
        var saved = new Board { mMainCounter = original.mMainCounter };
        foreach (var item in original.m_gridItems.Where(item => !item.mDead)) saved.m_gridItems.Add(Copy(item));
        return saved;
    }
    static Board Load(Board fields, Board? into = null)
    {
        var target = into ?? new Board(); var service = Service(target);
        BeginLoad();
        target.mMainCounter = fields.mMainCounter;
        target.m_gridItems.Clear(); foreach (var item in fields.m_gridItems) target.m_gridItems.Add(Copy(item));
        EndLoad(service); return target;
    }
    static GridItem ClockItem(Board board) => board.m_gridItems.Single(item => !item.mDead && item.mSunCount == -521094);
    static void Oracle(Board board, long units, string context)
    {
        Check(FusionTime.SourceTicks(board) == units / 6, context + " exact rational SourceTicks");
        Near(FusionTime.ElapsedSeconds(board), units / 600d, context + " rational elapsed seconds");
    }
    static void Mixed()
    {
        NativeClock.TIME_FOR_UPDATE = 1d / 120;
        var board = new Board(); long units = Step(board, 60, 120) + Step(board, 50, 100);
        Check(board.mMainCounter == 110, "native fixture has110 counters"); Oracle(board, units, "mixed original");
        var fields = Fields(board); var loaded = Load(fields); Oracle(loaded, units, "reader before first tick");
        Check(!FusionTime.IsSourcePulse(loaded), "restored snapshot cannot replay saved pulse");
        int itemCount = loaded.m_gridItems.Count;
        for (int i = 0; i < 10; i++) { Oracle(loaded, units, "repeated pure reader"); _ = FusionTime.StepSeconds(loaded); _ = FusionTime.PreviousStepSeconds(loaded); }
        Check(loaded.m_gridItems.Count == itemCount, "readers never allocate native records");
        units += Step(loaded, 1, 100); Oracle(loaded, units, "next native100 step is Source101");
        Check(FusionTime.SourceTicks(loaded) == 101 && FusionTime.SourceTicks(loaded) % 100 == 1, "no extra second/modulo event");
        // Independent smallest wrong-pulse counterexample and repeated field-only roundtrips.
        board = new Board(); units = Step(board, 1, 100) + Step(board, 6, 120);
        loaded = Load(Fields(board)); Oracle(loaded, units, "small counterexample");
        units += Step(loaded, 1, 120); Oracle(loaded, units, "first120 after reload");
        Check(!FusionTime.IsSourcePulse(loaded) && FusionTime.SourceTicks(loaded) == 6, "restored0phase must not emit false120 pulse");
        for (int i = 0; i < 120; i++) {
            int rate = i % 3 == 0 ? 100 : 120;
            units += Step(loaded, 1, rate);
            var next = Load(Fields(loaded)); Oracle(next, units, "alternating roundtrip" + i);
            long before = FusionTime.SourceTicks(next); Step(next, 1, rate); long nextUnits = units + (rate == 100 ? 6 : 5);
            Check(FusionTime.IsSourcePulse(next) == (nextUnits / 6 > before), "next pulse equals independent rational oracle");
            loaded = Load(Fields(loaded)); // Continue at exactly the saved boundary, not the probe tick.
        }
    }
    static void Lifecycle()
    {
        var board = new Board(); long units = Step(board, 7, 120);
        board.mPaused = true; Capture(board); var saved = Fields(board); var paused = Load(saved); paused.mPaused = true;
        for (int i = 0; i < 20; i++) { FusionTime.BeginTick(paused); Oracle(paused, units, "paused save/load"); Check(FusionTime.StepSeconds(paused) == 0, "pause delta0"); }
        paused.mPaused = false; units += Step(paused, 1, 100); Oracle(paused, units, "unpaused rate switch");
        Check(FusionTime.PreviousStepSeconds(paused) == 1d / 120, "loaded preceding native step retained across rate change");
        units += Step(paused, 1, 100); Check(FusionTime.SourceStep(paused, typeof(SaveTests)), "source owner before survival");
        FusionTime.Rebase(paused); Check(!FusionTime.IsSourcePulse(paused) && !FusionTime.SourceStep(paused, typeof(SaveTests)), "survival clears stale pulse/consumers without advance");
        Oracle(paused, units, "survival no native counter reset");
        paused.mMainCounter = -499; FusionTime.Rebase(paused); var stage = Load(Fields(paused)); Oracle(stage, units, "external signed counter rebase persists phase");
        units += Step(stage, 1, 120); Oracle(stage, units, "negative native counter legal after load");
        foreach (int counter in new[] { 0, 99 }) {
            var previous = new Board(); Step(previous, 3, 100); Capture(previous); var oldRecord = ClockItem(previous); oldRecord.mGridY = counter;
            previous.mMainCounter = counter;
            Hook(typeof(FusionTimeInitializePatch), "Prefix", previous);
            Check(oldRecord.mDead && FusionTime.SourceTicks(previous) == 0, "InitLevel kills own old record and starts zero epoch even counter" + counter);
            previous.mMainCounter = 0; Step(previous, 1, 100); Check(FusionTime.SourceTicks(previous) == 1, "new level does not reload old phase");
        }
        board = new Board(); units = Step(board, 2, 100); var service = Service(board);
        BeginLoad(); Hook(typeof(FusionTimeInitializePatch), "Prefix", board);
        FusionTime.BeginTick(board); Capture(board);
        Check(FusionTime.SourceTicks(board) == 2 && board.m_gridItems.Count == 0, "load scope preserves intact cache and never allocates/advances");
        EndLoad(service, false); Oracle(board, units, "failed load retains intact cache");
        var noBoardService = Service(null); BeginLoad(); var during = new Board { mMainCounter = 500 }; noBoardService.App!.Board = during;
        Check(FusionTime.SourceTicks(during) == 0 && FusionTime.ElapsedSeconds(during) == 0, "late board has neutral loading view, no legacy anchor");
        Check(during.m_gridItems.Count == 0, "loading read no record allocation");
        EndLoad(noBoardService, false, new IOException("native read failure"));
        Check(FusionTime.SourceTicks(during) == 500, "failed scope released without cached neutral/legacy view");
        BeginLoad(); BeginLoad(); EndLoad(service, true); Check(FusionTime.SourceTicks(board) == 2, "nested success cannot publish through outer load");
        EndLoad(service, false); Check(FusionTime.SourceTicks(board) == 2, "outer failure preserves epoch");
        BeginLoad(); Hook(typeof(FusionTimeDisposePatch), "Prefix", board); Check(FusionTime.SourceTicks(board) == 0, "disposed board cache released even inside load"); EndLoad(service, false);
        // Complete successful load on the same native board pointer with stale record cache.
        var original = new Board(); units = Step(original, 6, 120) + Step(original, 1, 100); var serialized = Fields(original);
        var oldEntry = FusionNativeTimeRecords.ReadClock(original, out _); units += Step(original, 20, 100);
        var same = Load(serialized, original); Oracle(same, 36, "same-pointer successful native array replacement");
        Check(!FusionTime.IsSourcePulse(same), "successful load removes old pulse/consumers");
        _ = oldEntry; // Old managed wrapper cannot determine the loaded clock payload.
        var badService = Service(new Board()); badService.App!.ThrowOnBoard = true;
        BeginLoad(); bool threw = false;
        try { EndLoad(badService); } catch (TargetInvocationException) { threw = true; }
        Check(threw, "simulated board getter error reached finalizer");
        Check(FusionTime.SourceTicks(new Board { mMainCounter = 10 }) == 10, "finalizer releases scope even if app board getter throws");
    }
    static void NativeBoundaries()
    {
        NativeClock.TIME_FOR_UPDATE = .01;
        var board = new Board();
        FusionTime.BeginTick(board); // Outer source sample precedes native counter++.
        long sampled = FusionTime.SourceTicks(board);
        Capture(board);
        Check(ClockItem(board).mGridY == 0 && sampled == 1, "midstep capture records current old counter, never fabricated counter+1");
        Check(FusionTime.SourceTicks(board) == sampled, "capture cannot advance source time");
        var beforeIncrement = Load(Fields(board));
        Check(FusionTime.SourceTicks(beforeIncrement) == sampled && !FusionTime.IsSourcePulse(beforeIncrement), "old-counter snapshot restores exact phase without replay pulse");
        Step(beforeIncrement, 1, 100);
        Check(FusionTime.SourceTicks(beforeIncrement) == sampled + 1, "load starts a fresh complete step, not partial native-loop resume");
        board.mMainCounter++;
        var afterIncrement = Fields(board);
        Check(ClockItem(afterIncrement).mGridY == 1 && FusionTime.SourceTicks(board) == sampled, "normal postincrement capture has new counter and same completed source phase");
        var service = Service(new Board { Playing = false }); BeginLoad();
        var loading = service.App!.Board!; loading.mMainCounter = afterIncrement.mMainCounter;
        foreach (var item in afterIncrement.m_gridItems) loading.m_gridItems.Add(Copy(item));
        EndLoad(service);
        Check(FusionTime.SourceTicks(loading) == sampled && loading.m_gridItems.Count == 1, "successful nonPlaying load reads phase from native records without allocation");
        FusionTime.BeginTick(loading);
        Check(FusionTime.SourceTicks(loading) == sampled && !FusionTime.IsSourcePulse(loading), "LevelIntro observation does not advance loaded phase");
        loading.Playing = true; Step(loading, 1, 100);
        Check(FusionTime.SourceTicks(loading) == sampled + 1, "first actual Playing step advances loaded phase once");
        var consumed = new Board(); Step(consumed, 1, 100); Check(FusionTime.SourceStep(consumed, typeof(SaveTests)), "save fixture consumed current pulse");
        var resumed = Load(Fields(consumed)); Check(!FusionTime.SourceStep(resumed, typeof(SaveTests)), "load cannot replay old consumer pulse");
        Step(resumed, 1, 100);
        Check(FusionTime.SourceStep(resumed, typeof(SaveTests)) && !FusionTime.SourceStep(resumed, typeof(SaveTests)), "fresh loaded step has fresh once-per-owner consumption");
    }
    static void Corruption()
    {
        var baseBoard = new Board(); Step(baseBoard, 6, 120); var fields = Fields(baseBoard);
        Action<GridItem>[] corruptions = {
            item => item.mGridX = 123, item => item.mGridItemCounter = 99, item => item.mGridY++,
            item => item.mPosX = float.NaN, item => item.mPosY = float.PositiveInfinity,
            item => item.mGoalX = .5f, item => item.mPosX = 1 << 21,
            item => item.mGoalY = 0x620000, item => item.mGoalY = 0x610006,
            item => item.mGridItemState = GridItemState.Other, item => item.mGridItemType = GridItemType.Crater
        };
        foreach (var corrupt in corruptions) {
            var stored = new Board { mMainCounter = fields.mMainCounter }; stored.m_gridItems.Add(Copy(ClockItem(fields))); corrupt(stored.m_gridItems[0]);
            var damaged = stored.m_gridItems[0]; string before = Bits(damaged); int errors = FusionMod.FusionMod.Instance.LoggerInstance.Errors;
            var loaded = Load(stored); Check(FusionTime.SourceTicks(loaded) == 0, "corrupt marked save starts explicit local zero epoch");
            for (int i = 0; i < 5; i++) { _ = FusionTime.SourceTicks(loaded); Capture(loaded); }
            Check(FusionMod.FusionMod.Instance.LoggerInstance.Errors == errors + 1, "corruption logs once per board epoch");
            Check(loaded.m_gridItems.Count == 1 && Bits(loaded.m_gridItems[0]) == before, "corrupt record retained without overwrite or legacy replacement");
        }
        var duplicate = new Board { mMainCounter = fields.mMainCounter }; duplicate.m_gridItems.Add(Copy(ClockItem(fields))); duplicate.m_gridItems.Add(Copy(ClockItem(fields)));
        var both = Load(duplicate); Check(FusionTime.SourceTicks(both) == 0, "duplicate marked save is not arbitrary first winner"); Capture(both); Check(both.m_gridItems.Count == 2, "duplicate markers retained");
        var live = new Board(); Step(live, 1, 100); Capture(live); var item = ClockItem(live); item.mGridY++; string changed = Bits(item);
        Capture(live); Check(Bits(item) == changed, "unexpected live stored counter cannot be overwritten");
        var failed = new Board(); Step(failed, 1, 100); failed.m_gridItems.FailAllocation = true; int initialErrors = FusionMod.FusionMod.Instance.LoggerInstance.Errors;
        Capture(failed); Capture(failed); Check(failed.m_gridItems.Count == 0 && FusionTime.SourceTicks(failed) == 1, "allocation failure preserves running clock without false record");
        Check(FusionMod.FusionMod.Instance.LoggerInstance.Errors == initialErrors + 1, "allocation errors bounded per epoch");
        failed.m_gridItems.FailAllocation = false; Capture(failed); Check(FusionTime.SourceTicks(Load(Fields(failed))) == 1, "later allocation recovery saves actual phase");
        var unsupported = new Board(); NativeClock.TIME_FOR_UPDATE = 1d / 90; FusionTime.BeginTick(unsupported); unsupported.mMainCounter++; Capture(unsupported);
        Check(unsupported.m_gridItems.Count == 0, "unsupported rate never claims exact new-format persistence");
        NativeClock.TIME_FOR_UPDATE = .01;
    }
    static string Bits(GridItem item) => string.Join(",", item.mGridItemType, item.mGridItemState, item.mSunCount, item.mGridX, item.mGridY, item.mGridItemCounter,
        BitConverter.SingleToInt32Bits(item.mPosX), BitConverter.SingleToInt32Bits(item.mPosY), BitConverter.SingleToInt32Bits(item.mGoalX), BitConverter.SingleToInt32Bits(item.mGoalY));
    static void LargeAndLegacy()
    {
        foreach (long count in new[] { (1L << 21) - 1, 1L << 21, (1L << 42) + 17, (1L << 53) + 123, long.MaxValue }) {
            var board = new Board(); Step(board, 1, 120);
            var clocks = (IDictionary)typeof(FusionTime).GetField("Clocks", Hidden)!.GetValue(null)!;
            var clock = clocks[board.Pointer]!; clock.GetType().GetField("SourceTicks", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(clock, count);
            var loaded = Load(Fields(board)); Check(FusionTime.SourceTicks(loaded) == count, "three exact float limbs retain full positive long" + count);
            Step(loaded, 1, 120);
            Check(FusionTime.SourceTicks(loaded) == (count == long.MaxValue ? count : count + 1), "large tick range cannot wrap negative");
        }
        var unmarked = new Board { mMainCounter = 110 }; NativeClock.TIME_FOR_UPDATE = .01;
        var legacy = Load(unmarked); Check(FusionTime.SourceTicks(legacy) == 110, "unmarked legacy retains compatibility anchor, not claimed historical mixed phase");
        Check(legacy.m_gridItems.Count == 0, "legacy load/read does not allocate records");
        var status = FusionNativeTimeRecords.Get(legacy, FusionNativeTimeRecords.Kind.Status, 42, 100)!; status.Flags = 7; status.A = 400; var native = legacy.m_gridItems[0]; string before = Bits(native);
        Capture(legacy); Check(Bits(native) == before && FusionNativeTimeRecords.Entries(legacy, FusionNativeTimeRecords.Kind.Status).Length == 1, "global clock save leaves status schema and payload untouched");
    }
    static void Main() { NativeClock.TIME_FOR_UPDATE = .01; Mixed(); Lifecycle(); NativeBoundaries(); Corruption(); LargeAndLegacy(); Console.WriteLine($"PASS {checks} actual clock save assertions"); }
}
