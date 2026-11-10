using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool pass, string message) { ++checks; if (!pass) throw new Exception(message); }
Plant Make(FusionPlant identity, Board? board = null)
{
    board ??= new Board { Pointer = (IntPtr)(100 + checks) };
    var plant = new Plant { mBoard = board, mSeedType = identity.NativeType,
        mImitaterType = identity.Marker, mIsOnBoard = true };
    board.m_plants.Add(plant); return plant;
}
void Step(Board board, Action action)
{ FusionTime.BeginTick(board); action(); ++board.mMainCounter; }
MethodBase Entry(string name) => typeof(Plant).GetMethod(name)!;
var abilities = Entry(nameof(Plant.UpdateAbilities));
var shooting = Entry(nameof(Plant.UpdateShooting));
var chomper = Entry(nameof(Plant.UpdateChomper));
Check(NativePlantClockPatch.TargetMethods().Count() == 7, "seven actual native entry boundaries");
Check(NativePlantClockPatch.TargetMethods().All(m => m != null), "every native method resolves");
foreach (int hz in new[] { 100, 120 })
{
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / hz;
    var plant = Make(FusionPlant.HypnoChomper);
    var board = plant.mBoard;
    int abilityTicks = 0, shootingTicks = 0, chomperTicks = 0, ownerTicks = 0;
    plant.mStateCountdown = 4000;
    for (int i = 0; i < hz; ++i)
        Step(board, () => {
            if (NativePlantClocks.Allow(plant, abilities))
            {
                ++abilityTicks; --plant.mStateCountdown;
                if (NativePlantClocks.Allow(plant, shooting)) ++shootingTicks;
                if (NativePlantClocks.Allow(plant, chomper)) ++chomperTicks;
            }
            if (FusionTime.SourceStep(plant, typeof(Program))) ++ownerTicks;
            Check(!NativePlantClocks.Allow(plant, abilities), "duplicate actual entry cannot consume another tick");
            Check(!NativePlantClocks.Allow(plant, shooting), "nested shooter entry deduplicates independently");
        });
    Check(abilityTicks == 100 && shootingTicks == 100 && chomperTicks == 100 && ownerTicks == 100,
        "all independent native entries and custom owner receive100 source pulses");
    Check(plant.mStateCountdown == 3900, "native4000 and saved remainder remain canonical");
    board.mPaused = true;
    int paused = plant.mStateCountdown;
    Step(board, () => { if (NativePlantClocks.Allow(plant, abilities)) --plant.mStateCountdown; });
    Check(plant.mStateCountdown == paused, "pause preserves canonical native remainder");
    board.mPaused = false;
    // Reconstruct a saved plant on an unrelated board counter with the same units.
    board.mMainCounter = 100000; FusionTime.Reset(board);
    plant.mStateCountdown = 1234;
    int pulses = 0;
    for (int i = 0; i < hz; ++i) Step(board, () => {
        if (NativePlantClocks.Allow(plant, abilities)) { --plant.mStateCountdown; ++pulses; }
    });
    Check(pulses == 100 && plant.mStateCountdown == 1134, "save rebase neither scales nor replays canonical fields");
    var vanilla = Make(new FusionPlant(SeedType.Chomper));
    Check(NativePlantClocks.Allow(vanilla, abilities) && NativePlantClocks.Allow(vanilla, abilities),
        "vanilla bypasses typed source gate even without clock");
    var nativeGloom = Make(FusionPlant.GloomShroom);
    Check(NativePlantClocks.Owns(nativeGloom), "registered source Gloom owns canonical clock");
    Check(!NativePlantClocks.Owns(Make(FusionPlant.Torchweed))
        && !NativePlantClocks.Owns(Make(FusionPlant.RoastedSpikeweed)), "existing actual-native-time owners excluded");
    board.mApp.GameScene = GameScenes.Menu;
    Check(NativePlantClocks.Allow(plant, abilities) && NativePlantClocks.Allow(plant, abilities),
        "menu and previews retain native lifecycle without source clock");
    Check(CustomShooterClocks.Owns(FusionPlant.ThornNut), "ThornNut custom clip cannot be decremented by native shooter");
    foreach (int sourceRemaining in new[] { 0, 1, 1234, -7 })
    {
        // Exercise the actual legacy identity transition with the native float
        // period, including relinquishing the registered Fusion clock.
        Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = (float)(1d / hz);
        var legacy = Make(new FusionPlant(SeedType.Puffshroom, SeedType.Sunflower));
        legacy.mTargetX = sourceRemaining; legacy.mLaunchCounter = 900;
        Check(NativeSunShroom.Migrate(legacy), "legacy source Sun-shroom migrates once");
        int expectedNative = (int)Math.Ceiling(Math.Max(0, sourceRemaining) * hz / 100d - 1e-9);
        Check(legacy.mStateCountdown == expectedNative,
            "maturity remainder crosses source-to-native units at identity conversion");
        Check(legacy.mSeedType == SeedType.Sunshroom && legacy.mImitaterType == SeedType.None
            && !NativePlantClocks.Owns(legacy), "migrated ordinary Sun-shroom relinquishes typed Fusion gate");
        Check(legacy.mState == (sourceRemaining == 0 ? PlantState.SunshroomBig : PlantState.SunshroomSmall)
            && legacy.mTargetX == 0, "migration retains native maturity state and retires old source field");
        Check(!NativeSunShroom.Migrate(legacy) && legacy.mStateCountdown == expectedNative,
            "repeat load/controller query cannot convert native maturity twice");
        Check(legacy.mLaunchCounter == 900, "maturity handoff does not reinterpret an unrelated native launch counter");
        Check(NativePlantClocks.Allow(legacy, abilities) && NativePlantClocks.Allow(legacy, abilities),
            "ordinary native Sun-shroom updates remain ungated after migration");
    }
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / hz;

    var kelp = Make(FusionPlant.GrappleKelp);
    kelp.mBoard.Water = true;
    var pot = Make(FusionPlant.SeaPot, kelp.mBoard);
    pot.mPlantCol = kelp.mPlantCol; pot.mRow = kelp.mRow;
    void Advance(Plant current) => ++current.mTargetX;
    for (int i = 0; i < hz; ++i) Step(kelp.mBoard, () => {
        SeaPot.Advance(kelp, Advance); SeaPot.Advance(kelp, Advance);
    });
    Check(kelp.mTargetX == 150, "wet custom kelp advances150 source quanta per second; repeated callback deduplicates");
    var nut = Make(FusionPlant.TangleNut); nut.mTargetY = 100;
    for (int i = 0; i < hz; ++i) Step(nut.mBoard, () => { TangleNut.Tick(nut); TangleNut.Tick(nut); });
    Check(nut.mTargetY == 0, "TangleNut independent owner uses100 source steps");
    var spreader = Make(FusionPlant.KelpSpreader); spreader.mRow = 2;
    spreader.mBoard.m_zombies.Add(new Zombie { mBoard = spreader.mBoard, mRow = 2, mPosX = 300 });
    for (int i = 0; i < hz; ++i) Step(spreader.mBoard, () => {
        KelpSpreadFusions.Update(spreader); KelpSpreadFusions.Update(spreader);
    });
    Check(spreader.mBoard.m_projectiles.Count == 3 && spreader.mLaunchCounter == 51,
        "custom kelp shooting emits three rows once at0.5 seconds with canonical remaining cooldown");
    var strangle = Make(FusionPlant.StrangleKelp);
    strangle.mState = PlantState.TanglekelpGrabbing;
    strangle.mTargetY = 215; strangle.mStateCountdown = 100;
    for (int i = 0; i < hz / 2; ++i) Step(strangle.mBoard, () => {
        KelpSpreadFusions.Update(strangle); KelpSpreadFusions.Update(strangle);
    });
    Check(strangle.mTargetY == 165 && strangle.mStateCountdown == 100,
        "strangle custom attack and native countdown handoff retain canonical units");
    var kelpPostfix = typeof(KelpSpreadAbilityPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static)!;
    Step(strangle.mBoard, () => kelpPostfix.Invoke(null, new object?[] { strangle, null }));
    Check(strangle.mTargetY == 165, "missing native kelp prefix state cannot reset saved attack phase");
}
// A live tick-rate switch keeps a single canonical saved remainder.
var changing = Make(FusionPlant.HypnoChomper); changing.mStateCountdown = 1000;
foreach (int hz in new[] { 120, 100, 120, 100 })
{
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / hz;
    for (int i = 0; i < hz / 2; ++i) Step(changing.mBoard, () => {
        if (NativePlantClocks.Allow(changing, abilities)) --changing.mStateCountdown;
    });
}
Check(changing.mStateCountdown == 800, "mixed rate retains exactly two elapsed seconds");
// The explicit moisture acceleration may re-enter abilities synchronously;
// ordinary accidental re-entry remains blocked by the entry's consumption key.
Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = .01;
var boosted = Make(FusionPlant.TangleWeed);
Step(boosted.mBoard, () => {
    Check(NativePlantClocks.Allow(boosted, abilities), "initial source native ability accepted");
    Check(!NativePlantClocks.Allow(boosted, abilities), "ordinary recursive native ability rejected");
    SeaPot.AdvancingNative = true;
    try { Check(NativePlantClocks.Allow(boosted, abilities), "explicit moisture extra source step accepted"); }
    finally { SeaPot.AdvancingNative = false; }
    Check(!NativePlantClocks.Allow(boosted, abilities), "moisture scope does not leak");
});
Console.WriteLine($"PASS {checks} native plant clock checks:100/120Hz, nested entries, pause, saves, rate switches, wet kelp, native exclusions.");
