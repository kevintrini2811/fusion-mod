using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0, boardId = 10;
void Check(bool condition, string message) { ++checks; if (!condition) throw new Exception(message); }
Plant Make(FusionPlant identity)
{
    var board = new Board { Pointer = (IntPtr)(++boardId), mMainCounter = 0 };
    var plant = new Plant { Identity = identity, mBoard = board };
    board.Plants.Add(plant); return plant;
}
void Step(Board board, Action body)
{ FusionTime.BeginTick(board); body(); ++board.mMainCounter; }
var ability = typeof(Plant).GetMethod(nameof(Plant.UpdateAbilities))!;
var mealPrefix = typeof(HypnoChomperMealPatch).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static)!;
var mealPostfix = typeof(HypnoChomperMealPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static)!;
var chomperPostfix = typeof(ChomperFusionAbilitiesPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static)!;
void MealTick(Plant plant, Action nativeBody)
{
    bool run = NativePlantClocks.Allow(plant, ability);
    var args = new object?[] { plant, run, null };
    mealPrefix.Invoke(null, args);
    if (run) nativeBody();
    mealPostfix.Invoke(null, new[] { plant, args[2] });
}
foreach (int hz in new[] { 100, 120 })
{
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / hz;
    var hypno = Make(FusionPlant.HypnoChomper);
    hypno.mState = PlantState.ChomperBiting; hypno.mStateCountdown = 70;
    hypno.Target = new Zombie { mBoard = hypno.mBoard, mMindControlled = true };
    void NativeBite()
    {
        if (hypno.mStateCountdown > 0) --hypno.mStateCountdown;
        if (hypno.mState == PlantState.ChomperBiting && hypno.mStateCountdown == 0)
        { hypno.Target.mDead = true; hypno.mState = PlantState.ChomperBitingGotOne; }
    }
    int nativeSteps = 0;
    while (hypno.mState == PlantState.ChomperBiting)
    {
        Step(hypno.mBoard, () => { MealTick(hypno, NativeBite); MealTick(hypno, NativeBite); });
        ++nativeSteps;
    }
    Check(nativeSteps == (int)Math.Ceiling(.7 * hz - 1e-9), "native70 bite counter commits at0.7 seconds");
    Check(hypno.mBoard.Coins.Count == 1 && hypno.mTargetX == 1, "nested/same-entry calls award one allied meal");
    // A controller loop completion is polled by the native ability body.
    // The observer must preserve the1000 canonical allied digestion override.
    bool entered = false;
    while (!entered) Step(hypno.mBoard, () => MealTick(hypno, () => {
        hypno.mState = PlantState.ChomperDigesting; hypno.mStateCountdown = 4000; entered = true;
    }));
    Check(hypno.mStateCountdown == 1000 && hypno.mTargetX == 0, "allied digestion writes1000 canonical centiseconds");
    int before = hypno.mStateCountdown;
    for (int i = 0; i < hz; ++i) Step(hypno.mBoard, () => MealTick(hypno, () => --hypno.mStateCountdown));
    Check(hypno.mStateCountdown == before - 100, "native digestion remains canonical at either rate");
    hypno.mBoard.mPaused = true;
    for (int i = 0; i < hz; ++i) Step(hypno.mBoard, () => MealTick(hypno, () => --hypno.mStateCountdown));
    Check(hypno.mStateCountdown == before - 100 && hypno.mBoard.Coins.Count == 1, "pause preserves countdown and reward");
    hypno.mBoard.mPaused = false;
    hypno.mBoard.mMainCounter = 98765; FusionTime.Reset(hypno.mBoard);
    before = hypno.mStateCountdown;
    for (int i = 0; i < hz; ++i) Step(hypno.mBoard, () => MealTick(hypno, () => --hypno.mStateCountdown));
    Check(hypno.mStateCountdown == before - 100, "saved allied digestion survives board counter rebase");

    var shooter = Make(FusionPlant.ChompShooter);
    shooter.mState = PlantState.ChomperDigesting;
    int startingSpawns = RollingProjectiles.Spawns;
    int sourceTicks = 0;
    for (int i = 0; i < hz * 3; ++i) Step(shooter.mBoard, () => {
        if (FusionTime.IsSourcePulse(shooter.mBoard)) ++sourceTicks;
        PlantState beforeState = sourceTicks == 1 ? PlantState.ChomperBitingGotOne : PlantState.ChomperDigesting;
        ChomperFusions.AfterNative(shooter, beforeState);
        ChomperFusions.AfterNative(shooter, beforeState);
    });
    Check(shooter.mLaunchCounter == 300 && RollingProjectiles.Spawns == startingSpawns + 1,
        "nested observer decrements shooter clip once per source step, with one event per cycle");
    Check(shooter.mStateCountdown == 3000, "native digestion transition keeps canonical3000");
    // Harmony may omit a bool prefix when an earlier gate rejects the native
    // entry. Its default nullable state must not fabricate a transition.
    shooter.mLaunchCounter = 17;
    Step(shooter.mBoard, () => chomperPostfix.Invoke(null, new object?[] { shooter, null }));
    Check(shooter.mLaunchCounter == 17, "missing prefix state cannot fabricate a native transition");
    var nut = Make(FusionPlant.ChompNut); nut.mPlantMaxHealth = 4000; nut.mPlantHealth = 1000;
    nut.mState = PlantState.Ready;
    bool observed = false;
    while (!observed) Step(nut.mBoard, () => {
        if (!FusionTime.IsSourcePulse(nut.mBoard)) return;
        observed = true;
        ChomperFusions.AfterNative(nut, PlantState.ChomperDigesting);
        ChomperFusions.AfterNative(nut, PlantState.ChomperDigesting);
    });
    Check(nut.mPlantHealth == 2000, "nested digestion completion cannot duplicate nut healing");
}
// Real Gulper leaf, with adversarial native death callbacks. No simulated
// update body can hide a missing generation/owner/phase guard here.
foreach (string mutation in new[] { "none", "plant-id", "plant-board", "plant-identity", "plant-dead",
    "plant-ground", "plant-health", "plant-stage", "plant-phase", "plant-countdown", "pause", "zombie-id", "zombie-board", "veto" })
{
    var plant = Make(FusionPlant.GulperShroom);
    GulperShroom.Initialize(plant);
    plant.mTargetX = 3; plant.mState = PlantState.ChomperBiting;
    var zombie = new Zombie { mBoard = plant.mBoard, mZombieType = ZombieType.Gargantuar };
    plant.Target = zombie;
    var originalBoard = plant.mBoard;
    zombie.VetoDeath = mutation == "veto";
    zombie.OnDie = () => {
        switch (mutation)
        {
            case "plant-id": ++plant.DataID; break;
            case "plant-board": plant.mBoard = new Board { Pointer = (IntPtr)777 }; break;
            case "plant-identity": plant.Identity = FusionPlant.Other; break;
            case "plant-dead": plant.mDead = true; break;
            case "plant-ground": plant.OffGround = true; break;
            case "plant-health": plant.mPlantHealth = 0; break;
            case "plant-stage": plant.mTargetX = 0; break;
            case "plant-phase": plant.mState = PlantState.Ready; break;
            case "plant-countdown": plant.mStateCountdown = 70; break;
            case "pause": originalBoard.mPaused = true; break;
            case "zombie-id": ++zombie.DataID; break;
            case "zombie-board": zombie.mBoard = new Board { Pointer = (IntPtr)778 }; break;
        }
    };
    Check(GulperShroom.GiantMeal(plant, out _), "Gulper owns the attempted final giant bite");
    Check((plant.mState == PlantState.ChomperBitingGotOne) == (mutation == "none"),
        "death callback cannot invent a successful giant meal: " + mutation);
    Check(originalBoard.mOnlyBurnedZombies == (mutation != "none"),
        "all-burned achievement changes only for a confirmed valid meal: " + mutation);
}
{
    var plant = Make(FusionPlant.GulperShroom);
    plant.mTargetX = 3; plant.mState = PlantState.ChomperBiting;
    var zombie = new Zombie { mBoard = plant.mBoard, mZombieType = ZombieType.Gargantuar };
    plant.Search = () => { ++plant.DataID; return zombie; };
    Check(GulperShroom.GiantMeal(plant, out _) && !zombie.mDead
        && plant.mState != PlantState.ChomperBitingGotOne,
        "target query cannot rebind a recycled owner generation into the old giant meal");
}
foreach (int hz in new[] { 100, 120 })
{
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / hz;
    var plant = Make(FusionPlant.GulperShroom);
    GulperShroom.Initialize(plant);
    for (int stage = 0; stage < 3; ++stage)
    {
        plant.mState = PlantState.ChomperDigesting;
        bool entered = false;
        while (!entered) Step(plant.mBoard, () => {
            if (!FusionTime.IsSourcePulse(plant.mBoard)) return;
            entered = true;
            GulperShroom.AfterNative(plant, PlantState.ChomperBitingGotOne);
            GulperShroom.AfterNative(plant, PlantState.ChomperBitingGotOne);
        });
        Check(plant.mStateCountdown == (4 - stage) * 1000, "Gulper digestion is canonical at either native rate");
        GulperShroom.Fertilize(plant);
        Check(plant.mStateCountdown == 50, "Fertilizer shortens the actual digestion field to fifty centiseconds");
        bool swallowed = false;
        while (!swallowed) Step(plant.mBoard, () => {
            if (!FusionTime.IsSourcePulse(plant.mBoard)) return;
            swallowed = true; plant.mState = PlantState.ChomperSwallowing;
            GulperShroom.AfterNative(plant, PlantState.ChomperDigesting);
            GulperShroom.AfterNative(plant, PlantState.ChomperDigesting);
        });
        Check(plant.mTargetX == stage + 1 && plant.mTargetY == 100,
            "Repeated observer calls cannot double growth or consume the new wait");
        for (int i = 0; i < hz; ++i) Step(plant.mBoard, () => {
            GulperShroom.AfterNative(plant, plant.mState); GulperShroom.AfterNative(plant, plant.mState);
        });
        Check(plant.mTargetY == 0 && plant.mLaunchRate == 50,
            "Growth waits exactly one simulation second before the half-second scale tween");
        for (int i = 0; i < hz / 2; ++i) Step(plant.mBoard, () => GulperShroom.AfterNative(plant, plant.mState));
        Check(Math.Abs(GulperShroom.Scale(plant) - (.2f + (stage + 1) * .1f) / .22f) < .00001f,
            "Original root scale reaches the next saved growth size at either rate");
    }
    Check(!GulperShroom.Low(plant) && GulperShroom.Digestion(plant) == 1000,
        "Mature Gulper is no longer short and keeps ten-second digestion");
    bool repeatedMeal = false;
    while (!repeatedMeal) Step(plant.mBoard, () => {
        if (!FusionTime.IsSourcePulse(plant.mBoard)) return;
        repeatedMeal = true; plant.mState = PlantState.ChomperSwallowing;
        GulperShroom.AfterNative(plant, PlantState.ChomperDigesting);
    });
    Check(plant.mTargetX == 3 && plant.mTargetY == 0 && plant.mLaunchRate == 0
        && Math.Abs(GulperShroom.Scale(plant) - .5f / .22f) < .00001f,
        "Repeated mature meal preserves0.5 rather than inventing a0.4-to0.5 growth");
}
// Native writer conflict regression, not an assumption of an unchanged field.
// Verified native UpdateAbilities6b62af calls UpdateShooting6bc710. With a
// live/awake/ground Chomper and nonzero Plant+0xdc,6bc804..6bc812 performs
// load/dec/store. Only this bounded native instruction effect is modeled;
// production scheduler and Gulper leaf execute unchanged below.
var shooting = typeof(Plant).GetMethod(nameof(Plant.UpdateShooting))!;
void NativeGulperTick(Plant plant)
{
    if (!NativePlantClocks.Allow(plant, ability)) return;
    if (NativePlantClocks.Allow(plant, shooting) && plant.mShootingCounter != 0)
        --plant.mShootingCounter;
    GulperShroom.AfterNative(plant, plant.mState);
    GulperShroom.AfterNative(plant, plant.mState);
}
foreach (int hz in new[] { 100, 120 })
{
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / hz;
    var plant = Make(FusionPlant.GulperShroom);
    GulperShroom.Initialize(plant);
    Check(plant.mShootingCounter == 0 && GulperShroom.Scale(plant) == 1f,
        "New Gulper keeps native shooting at zero and original initial0.22 scale");
    for (int i = 0; i < hz / 2; ++i) Step(plant.mBoard, () => NativeGulperTick(plant));
    Check(Math.Abs(GulperShroom.Scale(plant) - .2f / .22f) < .00001f,
        "Initial half-second tween reconstructs without a scale accumulator");
    // The live23.3.0 contract after the first completed growth. Restore its
    // actual positive shooting scalar, then keep the proven native writer on.
    plant.mTargetX = 1; plant.mTargetY = plant.mLaunchRate = 0;
    plant.mShootingCounter = 300000;
    for (int i = 0; i < hz * 13; ++i) Step(plant.mBoard, () => NativeGulperTick(plant));
    Check(plant.mShootingCounter == 298700,
        "Proven native shooting countdown actually continues after the tween");
    Check(Math.Abs(GulperShroom.Scale(plant) - .3f / .22f) < .00001f,
        "Thirteen-second natural native countdown cannot shrink completed growth");
    plant.mBoard.mPaused = true;
    for (int i = 0; i < hz; ++i) Step(plant.mBoard, () => NativeGulperTick(plant));
    Check(plant.mShootingCounter == 298700 && Math.Abs(GulperShroom.Scale(plant) - .3f / .22f) < .00001f,
        "Pause preserves native countdown and reconstructed growth");
    plant.mBoard.mPaused = false;
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / (hz == 100 ? 120 : 100);
    int changedHz = hz == 100 ? 120 : 100;
    for (int i = 0; i < changedHz; ++i) Step(plant.mBoard, () => NativeGulperTick(plant));
    Check(plant.mShootingCounter == 298600 && Math.Abs(GulperShroom.Scale(plant) - .3f / .22f) < .00001f,
        "Native-rate switch keeps exactly one countdown and no growth drift");
    // Load reconstructs the scalar from already saved growth/phase fields;
    // initializing or trusting the old drifted shooting value is unnecessary.
    var restored = Make(FusionPlant.GulperShroom);
    restored.mTargetX = restored.mTargetY = 0; restored.mLaunchRate = 25;
    Check(Math.Abs(GulperShroom.Scale(restored) - .21f / .22f) < .00001f,
        "Initial half-tween reconstructs0.21 independently of any legacy counter");
    restored.mTargetX = 2; restored.mTargetY = 0; restored.mLaunchRate = 25;
    restored.mShootingCounter = plant.mShootingCounter;
    Check(Math.Abs(GulperShroom.Scale(restored) - .35f / .22f) < .00001f,
        "A halfway saved growth tween reconstructs its exact current size");
    restored.mTargetY = 100; restored.mLaunchRate = 0;
    Check(Math.Abs(GulperShroom.Scale(restored) - .3f / .22f) < .00001f,
        "A saved growth wait retains the previous stage size");
}
// Actual supported UpdateAbilities order: prefix, UpdateShooting, decrement
// +0xa0 at6b62bd, then inline Chomper branch6b76d5. It never calls the
// standalone UpdateChomper hook. Its native giant fallback is40/flags0 at
//6b7808 followed by BitingMissed at6b780d. These instruction effects are the
// explicit native boundary; both real Harmony methods and clock gates run.
var gulperPrefix = typeof(GulperNativeChomperPatch).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static)!;
var gulperPostfix = typeof(GulperNativeChomperPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static)!;
var chomper = typeof(Plant).GetMethod(nameof(Plant.UpdateChomper))!;
int abilityWork = 0, nativeFallbacks = 0;
void GulperBiteEntry(Plant plant, bool standalone = false)
{
    MethodBase entry = standalone ? chomper : ability;
    bool run = NativePlantClockPatch.Prefix(plant, entry);
    var args = new object?[] { plant, entry, run, null };
    bool allow = (bool)gulperPrefix.Invoke(null, args)!;
    if (run && allow)
    {
        // Other native ability work must still run on a successful meal.
        if (!standalone)
        {
            ++abilityWork;
            if (plant.mStateCountdown > 0) --plant.mStateCountdown;
        }
        if (plant.mState == PlantState.ChomperBiting && plant.mStateCountdown == 0)
        {
            var target = plant.FindTargetZombie(plant.mRow, PlantWeapon.Primary);
            if (target?.mZombieType is ZombieType.Gargantuar or ZombieType.RedeyeGargantuar)
            { ++nativeFallbacks; target.TakeDamage(40, 0); plant.mState = PlantState.ChomperBitingMissed; }
        }
        // GotOne retains its native animation-loop wait (not completed here).
    }
    gulperPostfix.Invoke(null, new[] { plant, args[3] });
}
foreach (int hz in new[] { 100, 120 })
{
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / hz;
    var plant = Make(FusionPlant.GulperShroom);
    GulperShroom.Initialize(plant);
    plant.mTargetX = 3; plant.mTargetY = plant.mLaunchRate = 0;
    plant.mState = PlantState.ChomperBiting; plant.mStateCountdown = 70;
    plant.Target = new Zombie { mBoard = plant.mBoard, mZombieType = ZombieType.Gargantuar };
    int workBefore = abilityWork, fallbackBefore = nativeFallbacks, ticks = 0;
    while (plant.mState == PlantState.ChomperBiting)
    {
        Step(plant.mBoard, () => { GulperBiteEntry(plant); GulperBiteEntry(plant); });
        ++ticks;
    }
    Check(plant.Target.mDead && plant.mState == PlantState.ChomperBitingGotOne && nativeFallbacks == fallbackBefore,
        "Actual inline1-to0 bite order consumes the mature giant before native fallback");
    Check(ticks == (int)Math.Ceiling(.7 * hz - 1e-9) && abilityWork - workBefore == 70
        && plant.mStateCountdown == 0,
        "Meal occurs at70 source steps while preserving all native ability work and its decrement");
    Check(plant.mTargetX == 3 && plant.mTargetY == 0 && plant.mLaunchRate == 0,
        "Mature giant bite preserves the completed growth phase");

    plant.Target = new Zombie { mBoard = plant.mBoard, mZombieType = ZombieType.RedeyeGargantuar };
    plant.mState = PlantState.ChomperBiting; plant.mStateCountdown = 1;
    plant.mBoard.mPaused = true;
    for (int i = 0; i < hz; ++i) Step(plant.mBoard, () => GulperBiteEntry(plant));
    Check(!plant.Target.mDead && plant.mStateCountdown == 1, "Paused bite cannot preempt the next native event");
    plant.mBoard.mPaused = false;
    while (plant.mState == PlantState.ChomperBiting) Step(plant.mBoard, () => GulperBiteEntry(plant));
    Check(plant.Target.mDead && plant.mState == PlantState.ChomperBitingGotOne,
        "Resumed1-to0 event consumes Redeye once through the same owner gate");

    plant.Target = new Zombie { mBoard = plant.mBoard, mZombieType = ZombieType.Gargantuar };
    plant.mTargetX = 2; plant.mState = PlantState.ChomperBiting; plant.mStateCountdown = 1;
    fallbackBefore = nativeFallbacks;
    while (plant.mState == PlantState.ChomperBiting) Step(plant.mBoard, () => GulperBiteEntry(plant));
    Check(!plant.Target.mDead && plant.mState == PlantState.ChomperBitingMissed
        && nativeFallbacks == fallbackBefore + 1 && plant.Target.mBodyHealth == 230,
        "Juvenile giant rejection remains the one native40-damage fallback");

    plant.mTargetX = 3; plant.mState = PlantState.ChomperBiting; plant.mStateCountdown = 0;
    while (plant.mState == PlantState.ChomperBiting) Step(plant.mBoard, () => GulperBiteEntry(plant, standalone: true));
    Check(plant.Target.mDead && plant.mState == PlantState.ChomperBitingGotOne,
        "Standalone native Chomper entry still handles its already-zero bite event");

    plant.Target = new Zombie { mBoard = plant.mBoard, mZombieType = ZombieType.Gargantuar, VetoDeath = true };
    plant.mState = PlantState.ChomperBiting; plant.mStateCountdown = 1;
    workBefore = abilityWork;
    bool attempted = false;
    while (!attempted) Step(plant.mBoard, () => {
        attempted = FusionTime.IsSourcePulse(plant.mBoard); GulperBiteEntry(plant);
    });
    Check(!plant.Target.mDead && plant.mState == PlantState.ChomperBiting
        && plant.mStateCountdown == 1 && abilityWork == workBefore,
        "Inline death veto cannot commit a meal or resume the intercepted old native entry");
}
{
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / 120;
    var plant = Make(FusionPlant.GulperShroom);
    GulperShroom.Initialize(plant);
    plant.mTargetX = 3; plant.mState = PlantState.ChomperBiting; plant.mStateCountdown = 1;
    plant.Target = new Zombie { mBoard = plant.mBoard, mZombieType = ZombieType.Gargantuar };
    int before = abilityWork;
    Step(plant.mBoard, () => {
        Check(!FusionTime.IsSourcePulse(plant.mBoard), "First120Hz native entry is a real null pulse");
        GulperBiteEntry(plant); GulperBiteEntry(plant, standalone: true);
    });
    Check(!plant.Target.mDead && plant.mStateCountdown == 1 && abilityWork == before,
        "Neither inline nor standalone giant owner executes on a120Hz null pulse");
}
{
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = .01;
    var plant = Make(FusionPlant.GulperShroom);
    GulperShroom.Initialize(plant);
    plant.mTargetX = 3; plant.mTargetY = plant.mLaunchRate = 0;
    plant.mState = PlantState.ChomperBiting; plant.mStateCountdown = 1;
    plant.Target = new Zombie { mBoard = plant.mBoard, mZombieType = ZombieType.Gargantuar, VetoDeath = true };
    plant.Target.OnDie = () => plant.mState = PlantState.ChomperBitingGotOne;
    int before = abilityWork;
    Step(plant.mBoard, () => GulperBiteEntry(plant));
    Check(!plant.Target.mDead && plant.mBoard.mOnlyBurnedZombies
        && plant.mState == PlantState.ChomperBitingGotOne && plant.mStateCountdown == 1 && abilityWork == before,
        "Death veto plus callback-written GotOne cannot masquerade as a committed meal or resume native work");
}
Console.WriteLine($"PASS {checks} actual-clock Chomper/Gulper owner and real death-callback checks; native bodies remain explicit offline boundary models.");
