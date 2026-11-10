using FusionMod;
using Il2CppReloaded.Gameplay;
using Gameplay = Il2CppReloaded.Constants.Gameplay;

int assertions = 0;
void Require(bool condition, string message)
{ ++assertions; if (!condition) throw new Exception(message); }
Plant Add(Board board, FusionPlant identity)
{
    var plant = new Plant { mBoard = board, mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    board.m_plants.Add(plant); return plant;
}
(Board Board, Plant Source, Plant Pot) New(double step)
{
    Gameplay.TIME_FOR_UPDATE = step;
    var board = new Board(); FusionTime.Reset(board); FusionTime.BeginTick(board);
    return (board, Add(board, SeedType.Threepeater), Add(board, FusionPlant.TriPot));
}
void Tick(Board board, Plant pot)
{
    if (!board.mPaused) board.mMainCounter = unchecked(board.mMainCounter + 1);
    FusionTime.BeginTick(board);
    ShootingPots.Tick(pot);
}
void Schedule(Plant source, int triggers = 1)
{
    for (int i = 0; i < triggers * 6; ++i) source.Fire(null, source.mRow, PlantWeapon.Primary);
}
foreach (double step in new[] { .01, 1d / 120 })
{
    var (board, source, pot) = New(step);
    Schedule(source);
    double scheduled = FusionTime.ElapsedSeconds(board);
    ShootingPots.Tick(pot);
    ShootingPots.Tick(pot);
    Require(pot.mStartRow == 20 && board.Shots.Count == 6, "Same-native-step updates must not shorten a newly scheduled pair");
    int ticks = 0;
    while (board.Shots.Count == 6 && ++ticks < 40) Tick(board, pot);
    double elapsed = FusionTime.ElapsedSeconds(board) - scheduled;
    Require(board.Shots.Count == 8 && Math.Abs(elapsed - .2) <= step + 1e-8,
        $"Real clock pair must occur after .2 seconds at native step {step}: {elapsed}");
    ShootingPots.Tick(pot);
    Require(board.Shots.Count == 8, "Consumed pair cannot replay within its native step");

    (board, source, pot) = New(step); Schedule(source, 2);
    for (int i = 0; i < (int)Math.Ceiling(.45 / step); ++i) Tick(board, pot);
    Require(board.Shots.Count == 16 && pot.mLaunchCounter == 0,
        "Two queued pairs must retain separate source delays and exact shot count");

    (board, source, pot) = New(step); Schedule(source);
    for (int i = 0; i < 7; ++i) Tick(board, pot);
    int remaining = pot.mStartRow;
    board.mPaused = true;
    for (int i = 0; i < 100; ++i) Tick(board, pot);
    Require(pot.mStartRow == remaining && board.Shots.Count == 6, "Pause freezes the canonical remaining delay");
    board.mPaused = false;
    board.m_plants.Remove(pot);
    var loaded = Add(board, FusionPlant.TriPot);
    loaded.mLaunchCounter = pot.mLaunchCounter;
    loaded.mStartRow = pot.mStartRow;
    loaded.mDoSpecialCountdown = pot.mDoSpecialCountdown;
    loaded.mTargetZombieID = pot.mTargetZombieID;
    board.mMainCounter = 100000;
    FusionTime.Reset(board); FusionTime.BeginTick(board);
    double began = FusionTime.ElapsedSeconds(board);
    while (board.Shots.Count == 6 && FusionTime.ElapsedSeconds(board) - began < 1) Tick(board, loaded);
    Require(board.Shots.Count == 8 && loaded.mLaunchCounter == 0,
        "Saved tagged relative state must survive native-counter rebasing and a fresh clock epoch");
    Require(Math.Abs((FusionTime.ElapsedSeconds(board) - began) - remaining * .01) <= step + 1e-8,
        "Loaded relative state must retain its remaining source seconds");
}
foreach ((double before, double after) in new[] { (.01, 1d / 120), (1d / 120, .01) })
{
    var (board, source, pot) = New(before); Schedule(source);
    for (int i = 0; i < 7; ++i) Tick(board, pot);
    int remaining = pot.mStartRow;
    double began = FusionTime.ElapsedSeconds(board);
    Gameplay.TIME_FOR_UPDATE = after;
    while (board.Shots.Count == 6 && FusionTime.ElapsedSeconds(board) - began < 1) Tick(board, pot);
    Require(board.Shots.Count == 8 && Math.Abs((FusionTime.ElapsedSeconds(board) - began) - remaining * .01) <= .01 + 1e-8,
        "Mid-delay native rate change must preserve remaining source seconds");
}
foreach (int counter in new[] { 400, int.MaxValue - 5 })
{
    var (board, source, pot) = New(.01);
    board.mMainCounter = counter;
    FusionTime.Reset(board); FusionTime.BeginTick(board);
    pot.mLaunchCounter = 1; pot.mTargetZombieID = (ZombieID)source.DataID;
    pot.mStartRow = unchecked(counter + 13); pot.mDoSpecialCountdown = 0;
    for (int i = 0; i < 12; ++i) Tick(board, pot);
    Require(board.Shots.Count == 0 && pot.mStartRow == 1 && pot.mDoSpecialCountdown == ShootingPots.RelativeBonusDelay,
        "Legacy absolute deadline must migrate once without a double decrement, including integer wrap");
    Tick(board, pot);
    Require(board.Shots.Count == 2 && pot.mLaunchCounter == 0, "Migrated legacy pair fires exactly once at its remaining boundary");
}
Console.WriteLine($"PASS {assertions} actual-clock TriPot checks: 100/120 Hz, same-step deduplication, rate changes, pause, saved relative state, legacy deadlines.");
