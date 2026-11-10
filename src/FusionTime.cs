using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Published custom save fields use source centiseconds. Only explicitly owned
// clocks consume these steps; native counters cross a separate unit boundary.
internal static class FusionTime
{
    internal const double SourceTickSeconds = .01;
    private const double Epsilon = 1e-9;
    private readonly record struct Consumer(object Owner, IntPtr Entity, uint Generation);

    private sealed class Clock
    {
        internal int LastNativeCounter;
        internal bool Sampled, Pulse, HasStep, Exact = true, Legacy, Blocked, ErrorReported;
        internal int Phase;
        internal int? StoredCounter;
        internal double Seconds, Fraction, StepSeconds, PreviousStepSeconds;
        internal long SourceTicks;
        internal readonly HashSet<Consumer> Consumers = new();
    }

    private static readonly Dictionary<IntPtr, Clock> Clocks = new();
    private static int loadDepth;
    private const int LimbMask = (1 << 21) - 1;
    private const int Format = 0x610000, LegacyFlag = 0x100, HasStepFlag = 0x200, Previous120Flag = 0x400;

    internal static double NativeTickSeconds
    {
        get
        {
            double seconds = Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE;
            if (!double.IsFinite(seconds) || seconds <= 0)
                throw new InvalidOperationException("The native simulation interval must be finite and positive.");
            // The two native modes are stored as float constants. Normalize
            // their representation error before pulse/ceil arithmetic: .01f
            // otherwise yields 401 native ticks for a four-second duration.
            if (Math.Abs(seconds - SourceTickSeconds) < 1e-8) return SourceTickSeconds;
            if (Math.Abs(seconds - 1d / 120) < 1e-8) return 1d / 120;
            return seconds;
        }
    }

    private static bool Running(Board board) => ScaredContent.Playing(board) && !board.mPaused;

    // Called once at the outer native entry, before any board-prefix consumer.
    // Readers never infer elapsed time from a counter that can change mid-call.
    internal static void BeginTick(Board board)
    {
        if (loadDepth != 0 || !Running(board)) return;
        var clock = ReadClock(board)!;
        if (clock.Sampled && clock.LastNativeCounter == board.mMainCounter) return;

        double step = NativeTickSeconds;
        clock.PreviousStepSeconds = clock.HasStep ? clock.StepSeconds : step;
        clock.HasStep = true;
        clock.StepSeconds = step;
        clock.LastNativeCounter = board.mMainCounter;
        clock.Sampled = true;
        clock.Consumers.Clear();
        int rate = Rate(step);
        if (clock.Exact && FusionNativeTimeRecords.ValidRate(rate))
        {
            int units = clock.Phase + (rate == 100 ? 6 : 5);
            clock.Pulse = units >= 6;
            if (clock.SourceTicks > long.MaxValue - units / 6)
            {
                clock.Pulse = false; clock.Blocked = true;
                Error(clock, "source tick range exhausted; phase was not advanced."); return;
            }
            clock.SourceTicks += units / 6;
            clock.Phase = units % 6;
            clock.Fraction = clock.Phase / 600d;
            clock.Seconds = clock.SourceTicks * SourceTickSeconds + clock.Fraction;
        }
        else
        {
            clock.Exact = false;
            clock.Seconds += step; clock.Fraction += step;
            clock.Pulse = clock.Fraction + Epsilon >= SourceTickSeconds;
            if (clock.Pulse) { clock.Fraction = Math.Max(0, clock.Fraction - SourceTickSeconds); ++clock.SourceTicks; }
        }
    }

    private static int Rate(double seconds) => seconds == .01 ? 100 : seconds == 1d / 120 ? 120 : 0;
    private static void Error(Clock clock, string message)
    {
        if (clock.ErrorReported) return;
        clock.ErrorReported = true;
        FusionMod.Instance.LoggerInstance.Error("Fusion source clock: " + message);
    }
    private static bool Integer(float value, int maximum) => float.IsFinite(value) && value >= 0 && value <= maximum && value == MathF.Truncate(value);
    private static bool Decode(FusionNativeTimeRecords.Entry entry, out long ticks, out int phase, out int flags)
    {
        ticks = 0; phase = flags = 0;
        if (!entry.Valid || !FusionNativeTimeRecords.ValidRate(entry.Rate)
            || !Integer(entry.A, LimbMask) || !Integer(entry.B, LimbMask) || !Integer(entry.C, LimbMask)
            || !Integer(entry.D, 0xffffff)) return false;
        flags = (int)entry.D;
        phase = flags & 7;
        if ((flags & ~(LegacyFlag | HasStepFlag | Previous120Flag | 7)) != Format || phase > 5) return false;
        ticks = (long)(int)entry.A | ((long)(int)entry.B << 21) | ((long)(int)entry.C << 42);
        return true;
    }
    private static Clock CreateClock(Board board)
    {
        double step = NativeTickSeconds;
        var clock = new Clock { StepSeconds = step, PreviousStepSeconds = step };
        var record = FusionNativeTimeRecords.ReadClock(board, out bool marked);
        if (marked)
        {
            if (record != null && Decode(record, out long ticks, out int phase, out int flags) && record.Flags == board.mMainCounter)
            {
                clock.SourceTicks = ticks; clock.Phase = phase; clock.Fraction = phase / 600d;
                clock.Seconds = ticks * SourceTickSeconds + clock.Fraction;
                clock.StepSeconds = 1d / record.Rate;
                clock.PreviousStepSeconds = (flags & Previous120Flag) != 0 ? 1d / 120 : .01;
                clock.HasStep = (flags & HasStepFlag) != 0; clock.Legacy = (flags & LegacyFlag) != 0;
                clock.StoredCounter = record.Flags;
            }
            else
            {
                clock.Blocked = true;
                Error(clock, "invalid marked save retained; using a new local zero epoch, not reconstructed history.");
            }
            return clock;
        }
        // Compatibility anchor only. An unmarked save cannot reveal its historical rate mixture.
        clock.Legacy = board.mMainCounter != 0;
        int rate = Rate(step);
        if (FusionNativeTimeRecords.ValidRate(rate))
        {
            long units = (long)Math.Max(0, board.mMainCounter) * (rate == 100 ? 6 : 5);
            clock.SourceTicks = units / 6; clock.Phase = (int)(units % 6);
            clock.Fraction = clock.Phase / 600d; clock.Seconds = clock.SourceTicks * SourceTickSeconds + clock.Fraction;
        }
        else
        {
            clock.Exact = false; clock.Seconds = Math.Max(0, board.mMainCounter) * step;
            clock.SourceTicks = (long)Math.Floor(clock.Seconds / SourceTickSeconds + Epsilon);
            clock.Fraction = Math.Max(0, clock.Seconds - clock.SourceTicks * SourceTickSeconds);
        }
        return clock;
    }
    private static Clock? ReadClock(Board board)
    {
        if (Clocks.TryGetValue(board.Pointer, out var clock)) return clock;
        if (loadDepth != 0) return null;
        return Clocks[board.Pointer] = CreateClock(board);
    }
    internal static void Capture(Board board)
    {
        if (loadDepth != 0) return;
        var clock = ReadClock(board)!;
        if (clock.Blocked) return;
        int rate = Rate(clock.StepSeconds), previous = Rate(clock.PreviousStepSeconds);
        if (!clock.Exact || !FusionNativeTimeRecords.ValidRate(rate) || !FusionNativeTimeRecords.ValidRate(previous))
        { Error(clock, "unsupported simulation interval; exact phase was not saved."); return; }
        var entry = FusionNativeTimeRecords.ReadClock(board, out bool marked);
        if (marked && (entry == null || !Decode(entry, out _, out _, out _) || clock.StoredCounter != entry.Flags))
        { clock.Blocked = true; Error(clock, "marked record changed or became invalid; it was not overwritten."); return; }
        entry ??= FusionNativeTimeRecords.Get(board, FusionNativeTimeRecords.Kind.Clock, FusionNativeTimeRecords.ClockEntity, rate);
        if (entry == null) { Error(clock, "cannot allocate phase record; exact phase was not saved."); return; }
        entry.Rate = rate; entry.Flags = board.mMainCounter;
        entry.A = clock.SourceTicks & LimbMask; entry.B = (clock.SourceTicks >> 21) & LimbMask; entry.C = (clock.SourceTicks >> 42) & LimbMask;
        entry.D = Format | clock.Phase | (clock.Legacy ? LegacyFlag : 0) | (clock.HasStep ? HasStepFlag : 0) | (previous == 120 ? Previous120Flag : 0);
        clock.StoredCounter = entry.Flags;
    }
    internal static void BeginLoad() => ++loadDepth;
    internal static void EndLoad(Board? board, bool success)
    {
        try
        {
            if (loadDepth == 1 && success && board != null)
            {
                // Other load hooks may have cached wrappers before the native array was replaced.
                FusionNativeTimeRecords.Forget(board);
                Clocks.Remove(board.Pointer);
                Clocks[board.Pointer] = CreateClock(board);
            }
        }
        finally { --loadDepth; }
    }

    internal static bool IsSourcePulse(Board board) => loadDepth == 0 && Running(board)
        && Clocks.TryGetValue(board.Pointer, out var clock) && clock.Pulse;

    // Continuous source actions need every actual native delta, including a
    // 120-Hz entry without a centisecond pulse. Share the existing lifetime-
    // aware consumer gate; callers still own their saved canonical counters.
    internal static bool NativeStep(Plant plant, object owner)
    {
        var board = plant.mBoard;
        return loadDepth == 0 && Running(board) && Clocks.TryGetValue(board.Pointer, out var clock)
            && clock.Sampled && clock.Consumers.Add(new Consumer(owner, plant.Pointer, plant.DataID));
    }

    internal static bool SourceStep(Board board, object owner, IntPtr entity = default, uint generation = 0)
    {
        if (loadDepth != 0 || !Running(board) || !Clocks.TryGetValue(board.Pointer, out var clock)
            || !clock.Pulse) return false;
        return clock.Consumers.Add(new Consumer(owner, entity == default ? board.Pointer : entity, generation));
    }

    internal static bool SourceStep(Plant plant, object owner) =>
        SourceStep(plant.mBoard, owner, plant.Pointer, plant.DataID);
    internal static bool SourceStep(Projectile shot, object owner) =>
        SourceStep(shot.mBoard, owner, shot.Pointer, shot.DataID);
    internal static bool SourceStep(Zombie zombie, object owner) =>
        SourceStep(zombie.mBoard, owner, zombie.Pointer, zombie.DataID);

    internal static double ElapsedSeconds(Board board) => ReadClock(board)?.Seconds ?? 0;
    internal static long SourceTicks(Board board) => ReadClock(board)?.SourceTicks ?? 0;
    internal static double StepSeconds(Board board) => loadDepth == 0 && Running(board) ? ReadClock(board)?.StepSeconds ?? NativeTickSeconds : 0;
    internal static double PreviousStepSeconds(Board board) => ReadClock(board)?.PreviousStepSeconds ?? 0;

    internal static float SourceSeconds(int sourceTicks) => sourceTicks * .01f;
    internal static float NativeSeconds(int nativeTicks) => (float)(nativeTicks * NativeTickSeconds);
    internal static int NativeTicks(int sourceTicks) => sourceTicks <= 0
        ? sourceTicks : NativeTicksFromSeconds(sourceTicks * SourceTickSeconds);
    internal static int NativeTicksFromSeconds(double seconds)
    {
        if (!double.IsFinite(seconds) || seconds < 0)
            throw new ArgumentOutOfRangeException(nameof(seconds));
        double ticks = Math.Ceiling(seconds / NativeTickSeconds - Epsilon);
        return (int)Math.Clamp(ticks, 0, int.MaxValue);
    }

    // Cache loss after a native load is distinct from starting a new level epoch.
    internal static void Reset(Board? board)
    {
        if (board != null && loadDepth == 0) Clocks.Remove(board.Pointer);
    }
    internal static void Initialize(Board board)
    {
        if (loadDepth != 0) return;
        var clock = new Clock { StepSeconds = NativeTickSeconds, PreviousStepSeconds = NativeTickSeconds };
        Clocks[board.Pointer] = clock;
        var record = FusionNativeTimeRecords.ReadClock(board, out bool marked);
        if (!marked) return;
        if (record != null && Decode(record, out _, out _, out _)) FusionNativeTimeRecords.Remove(record);
        else { clock.Blocked = true; Error(clock, "invalid prior-epoch marker retained; new level starts at a local zero epoch."); }
    }
    internal static void Dispose(Board board) => Clocks.Remove(board.Pointer);

    // Keep accumulated time and allow a fresh native entry after a stage transition.
    // Native InitSurvivalStage itself does not reset mMainCounter.
    internal static void Rebase(Board board)
    {
        if (Clocks.TryGetValue(board.Pointer, out var clock))
        { clock.Sampled = false; clock.Pulse = false; clock.Consumers.Clear(); }
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class FusionTimeStepPatch
{
    [HarmonyPriority(Priority.First + 100)]
    private static void Prefix(Board __instance) => FusionTime.BeginTick(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.InitLevel))]
internal static class FusionTimeInitializePatch
{ private static void Prefix(Board __instance) => FusionTime.Initialize(__instance); }

[HarmonyPatch(typeof(Board), nameof(Board.InitSurvivalStage))]
internal static class FusionTimeSurvivalPatch
{ private static void Postfix(Board __instance) => FusionTime.Rebase(__instance); }

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class FusionTimeDisposePatch
{ private static void Prefix(Board __instance) => FusionTime.Dispose(__instance); }

[HarmonyPatch(typeof(Board), nameof(Board.Serialize))]
internal static class FusionTimeSavePatch
{
    [HarmonyPriority(Priority.First + 100)]
    private static void Prefix(Board __instance) => FusionTime.Capture(__instance);
}

[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Deserialize))]
internal static class FusionTimeLoadPatch
{
    [HarmonyPriority(Priority.First + 200)]
    private static void Prefix(out bool __state) { FusionTime.BeginLoad(); __state = true; }
    [HarmonyPriority(Priority.Last)]
    private static void Finalizer(SerializationService __instance, bool __result, bool __state, Exception? __exception)
    {
        if (!__state) return;
        Board? board = null;
        bool success = __result && __exception == null;
        try { if (success) board = __instance.App?.Board; }
        finally { FusionTime.EndLoad(board, success); }
    }
}
