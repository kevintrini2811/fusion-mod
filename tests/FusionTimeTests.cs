using FusionMod;
using Il2CppReloaded.Gameplay;
using NativeClock = Il2CppReloaded.Constants.Gameplay;

static class Program
{
    private sealed class OtherOwner {}
    private static int checks;
    private static void Check(bool value, string message)
    { ++checks; if (!value) throw new Exception(message); }
    private static void Near(double actual, double expected, string message) =>
        Check(Math.Abs(actual - expected) < 1e-7, $"{message}: {actual} != {expected}");

    private static int Advance(Board board, Plant plant, int ticks)
    {
        int pulses = 0;
        for (int i = 0; i < ticks; ++i)
        {
            FusionTime.BeginTick(board);
            double before = FusionTime.ElapsedSeconds(board);
            bool pulse = FusionTime.SourceStep(plant, typeof(Program));
            if (pulse) ++pulses;
            Check(!FusionTime.SourceStep(plant, typeof(Program)), "an owner consumes each entity step once");
            Check(FusionTime.SourceStep(plant, typeof(OtherOwner)) == pulse, "independent owner shares the source pulse");
            FusionTime.BeginTick(board);
            Near(FusionTime.ElapsedSeconds(board), before, "duplicate outer entry does not advance time");
            ++board.mMainCounter; // Native UpdateGame changes this before its postfixes.
            Near(FusionTime.ElapsedSeconds(board), before, "postfix reads retain the same immutable sample");
        }
        return pulses;
    }

    private static void Frequency(double period, int nativeTicks)
    {
        NativeClock.TIME_FOR_UPDATE = period;
        var board = new Board(); var plant = new Plant { mBoard = board };
        Check(Advance(board, plant, nativeTicks) == 100, "one simulated second contains 100 canonical source steps");
        Near(FusionTime.ElapsedSeconds(board), 1, "one second accumulated");
        Check(FusionTime.SourceTicks(board) == 100, "source frame count matches elapsed time");
        FusionTime.Reset(board);
    }

    private static void Hook(Type type, string method, params object[] args) =>
        type.GetMethod(method, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.Invoke(null, args);

    private static void NativeRecords()
    {
        var board = new Board();
        const uint target = 0xf1234567;
        var entry = FusionNativeTimeRecords.Get(board, FusionNativeTimeRecords.Kind.Status, target, 100)!;
        Check(entry.Valid && entry.EntityId == target, "record retains full unsigned DataID");
        entry.Flags = 7; entry.A = 400; entry.B = 900; entry.C = 12; entry.D = -8;
        Check(board.m_gridItems.Count == 1 && board.m_gridItems[0].mX == -1000, "record creates one hidden native item");
        Check(ReferenceEquals(entry, FusionNativeTimeRecords.Get(board, entry.Category, target)), "lookup reuses valid native generation");
        FusionNativeTimeRecords.Forget(board);
        var restored = FusionNativeTimeRecords.Get(board, entry.Category, target)!;
        Check(restored.Rate == 100 && restored.Flags == 7 && restored.A == 400 && restored.B == 900
            && restored.C == 12 && restored.D == -8, "native serialized fields reconstruct provenance after cache loss");
        Check(FusionNativeTimeRecords.Entries(board, entry.Category).Length == 1, "enumeration finds exactly one persisted record");
        var otherKind = FusionNativeTimeRecords.Get(board, FusionNativeTimeRecords.Kind.Projectile, target, 120)!;
        Check(otherKind.Rate == 120 && board.m_gridItems.Count == 2, "entity categories do not alias");
        restored.Rate = 99;
        Check(FusionNativeTimeRecords.Get(board, entry.Category, target, 120) == null
            && board.m_gridItems.Count == 2, "invalid saved provenance is preserved without a replacement guess");
        restored.Rate = 100;
        FusionNativeTimeRecords.Remove(restored);
        Check(!restored.Valid && FusionNativeTimeRecords.Get(board, entry.Category, target) == null, "removal kills native record and invalidates cache");
        var recycled = FusionNativeTimeRecords.Get(board, entry.Category, target, 120)!;
        board.m_gridItems[^1].DataID++;
        Check(!recycled.Valid, "reused native record pointer invalidates old wrapper");
        Check(FusionNativeTimeRecords.Get(board, entry.Category, target) is { Valid: true }, "lookup reconstructs current full record generation");
        var failed = new Board(); failed.m_gridItems.FailAllocation = true;
        Check(FusionNativeTimeRecords.Get(failed, entry.Category, 9, 120) == null
            && FusionMod.FusionMod.Instance.LoggerInstance.Errors == 1, "allocation failure reports error without modifying target");
        Check(FusionNativeTimeRecords.RebaseRemaining(1, 100, 120) == 2, "remaining duration rounds up");
        Check(FusionNativeTimeRecords.RebaseAge(1, 100, 120) == 1, "elapsed age rounds down");
        Check(FusionNativeTimeRecords.RebaseRemaining(400, 100, 120) == 480
            && FusionNativeTimeRecords.RebaseRemaining(480, 120, 100) == 400, "exact remaining duration survives roundtrip");
        Check(FusionNativeTimeRecords.RebaseRemaining(-1, 100, 120) == -1, "permanent native sentinel remains unchanged");
        Check(FusionNativeTimeRecords.RebaseRemaining(int.MaxValue, 100, 120) == int.MaxValue, "rate conversion saturates instead of overflowing");
        Check(Math.Abs(FusionNativeTimeRecords.VelocityScale(120, 100) - 1.2) < 1e-6, "native velocity scale preserves units");
    }

    private static void NativeHeads()
    {
        foreach (double period in new[] { .01, 1d / 120 }) {
            NativeClock.TIME_FOR_UPDATE = period;
            var board = new Board();
            var zombie = new Zombie { mBoard = board, mPhaseCounter = 150 };
            int fires = 0, poses = 0, walking = 0;
            for (int i = 0; i < (int)Math.Round(3 / period); ++i) {
                FusionTime.BeginTick(board);
                var update = NativeFusionZombieClocks.BeforeUpdate(zombie);
                if (zombie.mPhaseCounter > 0) --zombie.mPhaseCounter;
                var action = NativeFusionZombieClocks.BeforeActions(zombie);
                ++walking; // Other native work runs regardless of the head clock.
                if (zombie.mPhaseCounter == 35) ++poses;
                if (zombie.mPhaseCounter == 0) { ++fires; zombie.mPhaseCounter = 150; }
                NativeFusionZombieClocks.AfterActions(zombie, action);
                NativeFusionZombieClocks.AfterUpdate(zombie, update);
                ++board.mMainCounter;
            }
            Check(fires == 2 && poses == 2 && zombie.mPhaseCounter == 150, "summoned native head emits two shots/poses per three seconds at either rate");
            Check(walking == (int)Math.Round(3 / period), "native non-head work runs on every native tick");
        }
        NativeClock.TIME_FOR_UPDATE = 1d / 120;
        var skippedBoard = new Board(); var head = new Zombie { mBoard = skippedBoard, mPhaseCounter = 1 };
        FusionTime.BeginTick(skippedBoard); // First 120Hz sample has no source pulse.
        var saved = NativeFusionZombieClocks.BeforeUpdate(head);
        --head.mPhaseCounter;
        var suppressed = NativeFusionZombieClocks.BeforeActions(head);
        Check(head.mPhaseCounter == -1, "non-source head branch cannot prematurely fire at zero");
        var nested = NativeFusionZombieClocks.BeforeActions(head);
        NativeFusionZombieClocks.AfterActions(head, nested);
        Check(head.mPhaseCounter == -1, "nested action entry retains scoped event suppression");
        NativeFusionZombieClocks.AfterActions(head, suppressed);
        NativeFusionZombieClocks.AfterUpdate(head, saved);
        Check(head.mPhaseCounter == 1, "skipped source step restores only actual native decrement");
        saved = NativeFusionZombieClocks.BeforeUpdate(head);
        head.mPhaseCounter = 90; // Another owner deliberately changes state.
        NativeFusionZombieClocks.AfterUpdate(head, saved);
        Check(head.mPhaseCounter == 90, "unrelated phase writes survive finalizer");
        head.Summoned = false;
        Check(NativeFusionZombieClocks.BeforeUpdate(head) == null
            && NativeFusionZombieClocks.BeforeActions(head) == null, "unchanged native heads are untouched");
        NativeClock.TIME_FOR_UPDATE = .01;
    }

    public static void Main()
    {
        Frequency(.01, 100);
        Frequency(1d / 120, 120);
        Frequency((float).01, 100);
        Frequency((float)(1d / 120), 120);
        NativeClock.TIME_FOR_UPDATE = (float).01;
        Check(FusionTime.NativeTicks(400) == 400 && FusionTime.NativeTicks(1400) == 1400,
            "native float 100 Hz mode has no spurious ceil tick");

        var board = new Board(); var plant = new Plant { mBoard = board };
        NativeClock.TIME_FOR_UPDATE = 1d / 120;
        int pulses = Advance(board, plant, 60);
        NativeClock.TIME_FOR_UPDATE = .01;
        pulses += Advance(board, plant, 50);
        Check(pulses == 100, "120-to-100 switch preserves fractional source time");
        Near(FusionTime.ElapsedSeconds(board), 1, "rate switch does not reinterpret previous elapsed time");

        var reverse = new Board(); var reversePlant = new Plant { mBoard = reverse };
        Check(Advance(reverse, reversePlant, 50) == 50, "100 Hz first half-second");
        NativeClock.TIME_FOR_UPDATE = 1d / 120;
        Check(Advance(reverse, reversePlant, 60) == 50, "100-to-120 switch preserves canonical pulse count");
        Near(FusionTime.ElapsedSeconds(reverse), 1, "reverse rate switch preserves elapsed time");
        NativeClock.TIME_FOR_UPDATE = .01;
        FusionTime.BeginTick(board);
        var outer = typeof(Program).GetMethod(nameof(Main))!;
        var inner = typeof(Program).GetMethod(nameof(ToString))!;
        Check(FusionTime.IsSourcePulse(board), "nonconsuming pulse observer sees current step");
        Check(FusionTime.SourceStep(plant, outer) && FusionTime.SourceStep(plant, inner), "nested method entries have distinct consumption keys");
        Check(!FusionTime.SourceStep(plant, outer), "same method entry cannot consume twice");
        ++board.mMainCounter;

        double paused = FusionTime.ElapsedSeconds(board);
        board.mPaused = true;
        for (int i = 0; i < 10; ++i) {
            FusionTime.BeginTick(board);
            Check(!FusionTime.SourceStep(plant, typeof(Program)), "paused owners do not advance");
        }
        Near(FusionTime.ElapsedSeconds(board), paused, "pause adds no elapsed time");
        Near(FusionTime.StepSeconds(board), 0, "paused physics delta is zero");
        board.mPaused = false;
        Check(Advance(board, plant, 1) == 1, "resume advances the next native step");

        FusionTime.BeginTick(board);
        Check(FusionTime.SourceStep(plant, typeof(Program)), "first generation can advance");
        ++plant.DataID;
        Check(FusionTime.SourceStep(plant, typeof(Program)), "a reused pointer is a different full generation");
        var other = new Board(); var otherPlant = new Plant { mBoard = other, Pointer = plant.Pointer, DataID = plant.DataID };
        FusionTime.BeginTick(other);
        Check(FusionTime.SourceStep(otherPlant, typeof(Program)), "another board has an independent consumer set");

        double previous = FusionTime.ElapsedSeconds(board);
        board.mMainCounter = 0;
        FusionTime.Rebase(board);
        FusionTime.BeginTick(board);
        Near(FusionTime.ElapsedSeconds(board), previous + .01, "survival rebase retains accumulated time");

        NativeClock.TIME_FOR_UPDATE = 1d / 120;
        var loaded = new Board { mMainCounter = 120 };
        FusionTime.BeginTick(loaded);
        Check(FusionTime.SourceTicks(loaded) == 100, "a whole-second load anchor has no floating modulo phantom pulse");
        Near(FusionTime.ElapsedSeconds(loaded), 1 + 1d / 120, "loaded anchor advances exactly once");
        loaded.Playing = false;
        double stopped = FusionTime.ElapsedSeconds(loaded);
        ++loaded.mMainCounter; FusionTime.BeginTick(loaded);
        Near(FusionTime.ElapsedSeconds(loaded), stopped, "inactive scene does not advance");

        Check(FusionTime.NativeTicks(400) == 480, "source duration converts to 120 native ticks per second");
        Check(FusionTime.NativeTicks(0) == 0 && FusionTime.NativeTicks(-1) == -1, "zero and permanent sentinels survive conversion");
        Check(FusionTime.NativeTicks(67) == 81, "duration is never shortened below its source event");
        Near(FusionTime.NativeSeconds(480), 4, "native seconds boundary");
        Near(FusionTime.SourceSeconds(150), 1.5, "source seconds boundary");
        NativeClock.TIME_FOR_UPDATE = .01;
        Check(FusionTime.NativeTicks(400) == 400, "100 Hz conversion is identity");
        NativeClock.TIME_FOR_UPDATE = double.NaN;
        bool rejected = false;
        try { _ = FusionTime.NativeTickSeconds; } catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "invalid native interval is rejected");
        NativeClock.TIME_FOR_UPDATE = .01;

        var service = new Il2CppReloaded.Services.SerializationService {
            App = new Il2CppReloaded.Services.App { Board = board }
        };
        double preLoad = FusionTime.ElapsedSeconds(board);
        Hook(typeof(FusionTimeLoadPatch), "Prefix", new object[] { false });
        Hook(typeof(FusionTimeLoadPatch), "Finalizer", service, false, true, null!);
        Near(FusionTime.ElapsedSeconds(board), preLoad, "failed deserialize preserves accumulated clock");
        board.mMainCounter = 10;
        Hook(typeof(FusionTimeLoadPatch), "Prefix", new object[] { false });
        Hook(typeof(FusionTimeLoadPatch), "Finalizer", service, true, true, null!);
        FusionTime.BeginTick(board);
        Near(FusionTime.ElapsedSeconds(board), .11, "successful deserialize resets to restored native anchor");
        NativeRecords();
        NativeHeads();

        FusionTime.Reset(board);
        var reused = new Board { Pointer = board.Pointer };
        FusionTime.BeginTick(reused);
        Near(FusionTime.ElapsedSeconds(reused), .01, "disposed board pointer starts a fresh clock epoch");
        Console.WriteLine($"PASS {checks} actual FusionTime assertions.");
    }
}
