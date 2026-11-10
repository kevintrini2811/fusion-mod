using FusionMod;
using Il2CppReloaded.Gameplay;
using NativeClock = Il2CppReloaded.Constants.Gameplay;

int checks = 0;
void Check(bool pass, string name) { ++checks; if (!pass) throw new Exception(name); }
Board NonPulse()
{
    NativeClock.TIME_FOR_UPDATE = 1d / 120;
    var board = new Board(); FusionTime.BeginTick(board);
    Check(!FusionTime.IsSourcePulse(board), "fixture begins on non-source tick");
    return board;
}
Zombie Head(Board board, int phase = 70, int pointer = 100, uint id = 1) =>
    new() { mBoard = board, mPhaseCounter = phase, Pointer = (IntPtr)pointer, DataID = id };
foreach (bool frozen in new[] { false, true })
foreach (bool buttered in new[] { false, true })
{
    var board = NonPulse(); var head = Head(board);
    var scope = NativeFusionZombieClocks.BeforeUpdate(head);
    // Native Update706583..706595 checks both statuses before its sole writer.
    if (!frozen && !buttered) --head.mPhaseCounter;
    var action = NativeFusionZombieClocks.BeforeActions(head);
    Check(head.mPhaseCounter == -1, "all non-source head actions suppress equality events");
    NativeFusionZombieClocks.AfterActions(head, action);
    NativeFusionZombieClocks.AfterUpdate(head, scope);
    Check(head.mPhaseCounter == 70, "frozen/buttered non-source update preserves saved remainder");
}
foreach (string mutation in new[] { "board", "pointer", "generation" })
{
    var board = NonPulse(); var head = Head(board);
    var update = NativeFusionZombieClocks.BeforeUpdate(head);
    --head.mPhaseCounter;
    var action = NativeFusionZombieClocks.BeforeActions(head);
    if (mutation == "board") head.mBoard = new Board();
    if (mutation == "pointer") head.Pointer = (IntPtr)991;
    if (mutation == "generation") ++head.DataID;
    NativeFusionZombieClocks.AfterActions(head, action);
    Check(head.mPhaseCounter == -1, "action finalizer cannot write across" + mutation);
    // A replacement native entity may coincidentally have oldPhase-1.
    head.mPhaseCounter = 69;
    NativeFusionZombieClocks.AfterUpdate(head, update);
    Check(head.mPhaseCounter == 69, "update finalizer cannot write across" + mutation);
}
{
    var board = NonPulse(); var outer = Head(board, 71); var inner = Head(board, 42, 101, 2);
    var outerScope = NativeFusionZombieClocks.BeforeUpdate(outer); --outer.mPhaseCounter;
    var outerAction = NativeFusionZombieClocks.BeforeActions(outer);
    var innerScope = NativeFusionZombieClocks.BeforeUpdate(inner); --inner.mPhaseCounter;
    var innerAction = NativeFusionZombieClocks.BeforeActions(inner);
    NativeFusionZombieClocks.AfterActions(inner, innerAction);
    NativeFusionZombieClocks.AfterUpdate(inner, innerScope);
    Check(inner.mPhaseCounter == 42 && outer.mPhaseCounter == -1, "nested different head restores only its own remainder");
    NativeFusionZombieClocks.AfterActions(outer, outerAction);
    NativeFusionZombieClocks.AfterUpdate(outer, outerScope);
    Check(outer.mPhaseCounter == 71, "outer action scope survives nested update/finalizer");
}
{
    var board = NonPulse(); var head = Head(board);
    var update = NativeFusionZombieClocks.BeforeUpdate(head); --head.mPhaseCounter;
    var action = NativeFusionZombieClocks.BeforeActions(head);
    try { throw new InvalidOperationException("native action failed"); }
    catch (InvalidOperationException) { }
    finally {
        NativeFusionZombieClocks.AfterActions(head, action);
        NativeFusionZombieClocks.AfterUpdate(head, update);
    }
    Check(head.mPhaseCounter == 70, "exception finalizers restore sentinel and native decrement");
}
{
    var board = new Board(); var head = Head(board, 1234);
    foreach (int hz in new[] { 120, 100, 120, 100 })
    {
        NativeClock.TIME_FOR_UPDATE = 1d / hz;
        for (int i = 0; i < hz / 2; ++i)
        {
            FusionTime.BeginTick(board);
            var scope = NativeFusionZombieClocks.BeforeUpdate(head);
            --head.mPhaseCounter;
            NativeFusionZombieClocks.AfterUpdate(head, scope);
            ++board.mMainCounter;
        }
    }
    Check(head.mPhaseCounter == 1034, "mixed native rates consume200 canonical steps in two seconds");
    board.mMainCounter = 98765; FusionTime.Reset(board);
    NativeClock.TIME_FOR_UPDATE = 1d / 120;
    for (int i = 0; i < 120; ++i)
    {
        FusionTime.BeginTick(board);
        var scope = NativeFusionZombieClocks.BeforeUpdate(head);
        --head.mPhaseCounter;
        NativeFusionZombieClocks.AfterUpdate(head, scope);
        ++board.mMainCounter;
    }
    Check(head.mPhaseCounter == 934, "saved native head remainder survives rebased board clock");
    board.mPaused = true;
    Check(NativeFusionZombieClocks.BeforeUpdate(head) == null
        && NativeFusionZombieClocks.BeforeActions(head) == null, "paused native entry does not install action sentinel");
}
Console.WriteLine($"PASS {checks} independent native zombie clock review checks: guarded decay, scope identity, nested heads, exceptions, rate switches and saved remainders.");
