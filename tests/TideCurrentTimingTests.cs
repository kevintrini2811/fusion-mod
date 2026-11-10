using FusionMod;
using Il2CppReloaded.Gameplay;
using Clock = Il2CppReloaded.Constants.Gameplay;

int checks = 0;
void Check(bool condition, string message) { ++checks; if (!condition) throw new Exception(message); }
void Near(float actual, float expected, string message) => Check(Math.Abs(actual - expected) < .01f, message + $":{actual}!={expected}");
Plant Tide(Board board)
{ var plant = new Plant { mBoard = board }; board.m_plants.Add(plant); return plant; }
void NativeWalking(Zombie zombie)
{
    if (zombie.mBoard.mPaused) return;
    var state = TideCurrent.Capture(zombie);
    try {
        if (zombie.mIceTrapCounter <= 0 && zombie.mButteredCounter <= 0)
        {
            float speed = zombie.mVelX * (zombie.Chilled ? .5f : 1);
            zombie.mPosX += zombie.Backwards ? speed : -speed;
            zombie.mX = (int)zombie.mPosX;
        }
        TideCurrent.Finish(zombie, state); // Real Harmony postfix.
    } finally { TideCurrent.Finish(zombie, state); } // Also invokes finalizer.
}
foreach (int hz in new[] { 100, 120 })
foreach (string mode in new[] { "normal", "chilled", "frozen", "buttered", "backwards", "none", "two", "paused" })
{
    Clock.TIME_FOR_UPDATE = 1d / hz;
    var board = new Board();
    var plant = Tide(board);
    if (mode == "none") board.m_plants.Clear();
    if (mode == "two") Tide(board);
    var zombie = new Zombie { mBoard = board, mVelX = 60f / hz,
        Chilled = mode == "chilled", Backwards = mode == "backwards",
        mIceTrapCounter = mode == "frozen" ? 10 : 0, mButteredCounter = mode == "buttered" ? 10 : 0 };
    board.m_zombies.Add(zombie); board.mPaused = mode == "paused";
    for (int i = 0; i < hz; ++i)
    {
        FusionTime.BeginTick(board);
        NativeWalking(zombie);
        foreach (var source in board.m_plants) { TideCurrent.Tick(source); TideCurrent.Tick(source); }
        ++board.mMainCounter;
    }
    float expected = mode == "none" ? 440 : mode == "backwards" ? 570 : mode == "two" ? 520 : mode == "paused" ? 500 : 510;
    Near(zombie.mPosX, expected, "actual native delta + canonical current:" + mode + hz);
    if (mode != "none") Check(plant.mTargetX == (mode == "paused" ? 0 : 100), "growth source clock is rate independent:" + mode);
}
{
    var board = new Board(); var plant = Tide(board);
    var zombie = new Zombie { mBoard = board }; board.m_zombies.Add(zombie);
    foreach (string mutation in new[] { "board", "generation", "noPool", "friendly", "noTide" })
    {
        zombie.mBoard = board; zombie.mPosX = 500; zombie.mInPool = true; zombie.mMindControlled = false; plant.mDead = false;
        var state = TideCurrent.Capture(zombie); Check(state != null, "active pool enemy captured");
        zombie.mPosX = 499;
        if (mutation == "board") zombie.mBoard = new Board();
        if (mutation == "generation") ++zombie.DataID;
        if (mutation == "noPool") zombie.mInPool = false;
        if (mutation == "friendly") zombie.mMindControlled = true;
        if (mutation == "noTide") plant.mDead = true;
        TideCurrent.Finish(zombie, state);
        Near(zombie.mPosX, 499, "scope cannot mutate invalidated target:" + mutation);
    }
    zombie.mBoard = board; zombie.mPosX = 500; zombie.mInPool = true; zombie.mMindControlled = false; plant.mDead = false;
    var saved = TideCurrent.Capture(zombie); zombie.mPosX = 499;
    TideCurrent.Finish(zombie, saved); Near(zombie.mPosX, 500, "postfix restores actual forward delta");
    zombie.mPosX = 498; TideCurrent.Finish(zombie, saved);
    Near(zombie.mPosX, 498, "finalizer cannot overwrite later movement after completed postfix");
    saved = TideCurrent.Capture(zombie); zombie.mPosX = 497;
    try { throw new InvalidOperationException("native walking failed"); }
    catch (InvalidOperationException) {}
    finally { TideCurrent.Finish(zombie, saved); }
    Near(zombie.mPosX, 498, "exception finalizer restores only captured forward delta");
}
Console.WriteLine($"PASS {checks} TideCurrent checks with actual source clock; native walking body is an explicit boundary model.");
