using FusionMod;
using Il2CppReloaded.Gameplay;
int checks = 0; uint zombieSequence = 20;
void Check(bool pass, string why) { ++checks; if (!pass) throw new Exception(why); }
Plant Make(bool blover = false) {
    var plant = new Plant { mBoard = new(), Identity = blover ? FusionPlant.MagnetBlover : FusionPlant.LumosMagnet };
    MagnetFusions.Initialize(plant); return plant;
}
Zombie Target(Plant plant, float x = 180, int row = 2) {
    var zombie = new Zombie { DataID = ++zombieSequence, Pointer = (IntPtr)zombieSequence, mBoard = plant.mBoard, TargetX = x, TargetY = 140,
        mRow = row, mHelmHealth = 1000, mHelmType = HelmType.Pail };
    plant.mBoard.m_zombies.Add(zombie); return zombie;
}
void Step(Plant plant, int count = 1) {
    for (int i = 0; i < count; ++i) {
        FusionTime.BeginTick(plant.mBoard);
        MagnetFusionAbilityPatch.Prefix(plant, true, out var invocation);
        try {
            MagnetFusionUpdatePatch.Prefix(plant, true); MagnetFusionUpdatePatch.Postfix(plant);
            MagnetFusionUpdatePatch.Prefix(plant, true); MagnetFusionUpdatePatch.Postfix(plant);
            MagnetFusionAbilityPatch.Postfix(invocation);
        } finally { MagnetFusionAbilityPatch.Finalizer(invocation); }
        ++plant.mBoard.mMainCounter;
    }
}
foreach (int hz in new[] { 100, 120 }) {
    Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / hz;
    var blover = Make(true); var z = Target(blover, 700);
    Step(blover, (int)Math.Ceiling(.49 * hz));
    Check(z.mHelmHealth > 0, "initial acquisition waits0.5s");
    Step(blover, hz / 10);
    Check(z.mHelmHealth == 0 && blover.mBoard.Winds == 1, "one wind and first attraction");
    Check(blover.mTargetX <= 60, "repeated entry cannot advance owner twice");
    int age = blover.mTargetX, cooldown = blover.mLaunchCounter;
    blover.mBoard.mPaused = true; Step(blover, hz);
    Check(blover.mTargetX == age && blover.mLaunchCounter == cooldown, "pause preserves lifetime and digestion");
    blover.mBoard.mPaused = false;
    MagnetFusions.Fertilize(blover);
    Check(blover.mLaunchCounter == 50 && blover.mTargetX == age, "fertilizer caps digestion only");
    while (!blover.mDead) Step(blover);
    Check(blover.mTargetX == 1039 && blover.mBoard.Winds == 1, "lifetime is ten seconds after39cs event");

    var magnet = Make(); var pail = Target(magnet);
    for (int i = 0; i < 3 && pail.mHelmHealth > 0; ++i) Step(magnet);
    Check(pail.mHelmHealth == 0 && magnet.mLaunchCounter == 1500 && pail.BodyHealth == 1000, "LL0 no shock and1500cs");
    var item = magnet.mMagnetItems[0];
    Step(magnet, (int)Math.Ceiling(.3 * hz));
    Check(Math.Abs(item.PosX - 140) < .001 && Math.Abs(item.PosY - 110) < .001, "linear flight reaches exact endpoint at0.3s");
    magnet.Level = 2;
    int before = magnet.mLaunchCounter;
    Step(magnet, hz);
    Check(magnet.mLaunchCounter == before - 100 && MagnetFusions.StoredLight(magnet) == 0, "light change does not rewrite positive timer or cached glow");
    MagnetFusions.Fertilize(magnet);
    var far = Target(magnet, 400); var control = Target(magnet, 410); control.mHelmType = HelmType.None; control.mHelmHealth = 0;
    var otherRow = Target(magnet, 410, 3); otherRow.mHelmType = HelmType.None; otherRow.mHelmHealth = 0;
    var shieldedNeighbor = Target(magnet, 420); shieldedNeighbor.mHelmType = HelmType.None; shieldedNeighbor.mHelmHealth = 0;
    shieldedNeighbor.mShieldType = ShieldType.Door; shieldedNeighbor.mShieldHealth = 1000;
    Step(magnet, hz);
    Check(far.mHelmHealth == 0 && far.BodyHealth == 800 && control.BodyHealth == 800 && otherRow.BodyHealth == 1000,
        "LL2 removes equipment before200 damage, only attraction row");
    Check(shieldedNeighbor.mShieldHealth == 1000 && shieldedNeighbor.BodyHealth == 800,
        "source Shieldless shock bypasses neighboring second armor without removing it");
    Check(MagnetFusions.StoredLight(magnet) == 2 && magnet.mLaunchCounter <= 750, "LL2 interval and light state committed");
    var recreated = Make(); recreated.mLaunchCounter = magnet.mLaunchCounter;
    recreated.mTargetY = magnet.mTargetY; recreated.mBoard.mMainCounter = 4321; FusionTime.Reset(recreated.mBoard);
    Step(recreated, hz);
    Check(recreated.mLaunchCounter == magnet.mLaunchCounter - 100, "reconstructed saved cooldown survives counter rebase");
    var reused = Make(); reused.Level = 2; var oldTarget = Target(reused); var near = Target(reused, 190);
    oldTarget.OnDamage = () => reused.DataID++;
    Step(reused, 2);
    Check(near.BodyHealth == 1000, "owner generation change stops remaining callbacks");
}
// Source land eligibility is independent of native Ground's extra exclusions.
foreach (var phase in new[] { ZombiePhase.DiggerRising, ZombiePhase.DiggerRiseWithoutAxe,
    ZombiePhase.ImpLanding, ZombiePhase.DancerRising,
    ZombiePhase.RisingFromGrave, ZombiePhase.PogoBouncing, ZombiePhase.PogoForwardBounce2,
    ZombiePhase.PogoHighBounce1, ZombiePhase.PogoHighBounce2, ZombiePhase.PogoHighBounce3,
    ZombiePhase.PogoHighBounce4, ZombiePhase.PogoHighBounce5, ZombiePhase.PogoHighBounce6,
    ZombiePhase.PogoForwardBounce7 }) {
    var owner = Make(); owner.Level = 2; Target(owner);
    var neighbor = Target(owner, 190); neighbor.mHelmType = HelmType.None; neighbor.mHelmHealth = 0;
    neighbor.mZombiePhase = phase; neighbor.Targetable = false;
    Step(owner, 2);
    Check(neighbor.BodyHealth == 800, $"source land eligibility includes {phase} without delegating to native Ground");
}
foreach (var phase in new[] { ZombiePhase.PolevaulterInVault, ZombiePhase.DolphinInJump,
    ZombiePhase.BalloonFlying, ZombiePhase.BalloonPopping, ZombiePhase.DiggerTunneling,
    ZombiePhase.DiggerTunnelingPauseWithoutAxe, ZombiePhase.ImpGettingThrown }) {
    var owner = Make(); owner.Level = 2; Target(owner);
    var neighbor = Target(owner, 190); neighbor.mHelmType = HelmType.None; neighbor.mHelmHealth = 0;
    neighbor.mZombiePhase = phase;
    Step(owner, 2);
    Check(neighbor.BodyHealth == 1000, $"source land eligibility excludes {phase}");
}
foreach (var (kind, phase) in new[] { (ZombieType.Boss, ZombiePhase.BossHeadIdleBeforeSpit),
    (ZombieType.Bungee, ZombiePhase.BungeeAtBottom), (ZombieType.Bungee, ZombiePhase.BungeeGrabbing) }) {
    var owner = Make(); var enemy = Target(owner); enemy.mZombieType = kind; enemy.mZombiePhase = phase;
    Check(!MagnetFusions.ShockEligible(enemy), $"source boss/bungee excludes {kind}/{phase}");
}
var snorkelOwner = Make(); var snorkel = Target(snorkelOwner); snorkel.mZombieType = ZombieType.Snorkel;
snorkel.mInPool = true;
Check(!MagnetFusions.ShockEligible(snorkel), "source submerged snorkel excluded");
snorkel.mIsEating = true;
Check(MagnetFusions.ShockEligible(snorkel), "surfaced eating snorkel eligible");
snorkel.mInPool = false; snorkel.mIsEating = false;
Check(MagnetFusions.ShockEligible(snorkel), "land snorkel eligible");
snorkel.mMindControlled = true;
Check(!MagnetFusions.ShockEligible(snorkel), "source mind-controlled shock target excluded");
var mixed = Make(true);
var headlessMagnet = Make(); var headlessMetal = Target(headlessMagnet); headlessMetal.mHasHead = false;
Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = .01;
Step(headlessMagnet, 2);
Check(headlessMetal.mHelmHealth == 0, "source metal search has no additional head-presence restriction");
var unsupportedShieldMagnet = Make(); var unsupportedShield = Target(unsupportedShieldMagnet);
unsupportedShield.mHelmType = HelmType.None; unsupportedShield.mHelmHealth = 0;
unsupportedShield.mShieldType = ShieldType.Garbage; unsupportedShield.mShieldHealth = 1000;
Step(unsupportedShieldMagnet, 2);
Check(unsupportedShieldMagnet.mLaunchCounter == 0 && unsupportedShieldMagnet.mMagnetItems[0].ItemType == MagnetItemType.None,
    "unmapped native garbage shield alone cannot start a source attraction");
Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = .01; Step(mixed, 100);
Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = 1d / 120; Step(mixed, 120);
Check(mixed.mTargetX == 200 && mixed.mBoard.Winds == 1, "rate switch preserves canonical age and one event");
var held = Make(true);
Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = .01;
held.mLaunchCounter = 2; BucketPlants.Held = true;
Step(held, 2);
Check(held.mLaunchCounter == 0 && held.mTargetX == 2 && !BucketPlants.Held && BucketPlants.Clears == 1,
    "held selection does not pause digestion or lifetime and clears before slot reuse");
// Real native armor-borrow source and the actual Fusion Harmony boundary
// methods run here. Native TakeDamage itself is an explicit isolating model.
Plant BobsledCase(out Zombie attracted, out Zombie neighbor) {
    var plant = Make(); plant.Level = 2;
    attracted = Target(plant, 180); neighbor = Target(plant, 190);
    foreach (var zombie in new[] { attracted, neighbor }) {
        zombie.mZombieType = ZombieType.Bobsled; zombie.AddedBucket = true;
        zombie.mGraveY = 1100; zombie.mHelmType = HelmType.Bobsled;
        zombie.mHelmHealth = zombie.mHelmMaxHealth = 6000;
    }
    return plant;
}
void Inner(Plant plant, bool normal = true, Action? beforeReturn = null) {
    var borrow = ZombieLibrary.NativeArmorMagnet.Begin(plant);
    try {
        MagnetFusionUpdatePatch.Prefix(plant, true);
        beforeReturn?.Invoke();
        if (normal) MagnetFusionUpdatePatch.Postfix(plant);
    } finally { ZombieLibrary.NativeArmorMagnet.Restore(borrow); }
}
Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE = .01;
var sledMagnet = BobsledCase(out var sled, out var neighborSled);
FusionTime.BeginTick(sledMagnet.mBoard);
MagnetFusionAbilityPatch.Prefix(sledMagnet, true, out var outer);
try {
    Inner(sledMagnet, beforeReturn: () => {
        Check(sled.mGraveY == 1100 && sled.mHelmType == HelmType.None,
            "real borrow has not committed removed added helmet before inner finalizer");
        Check(neighborSled.mGraveY == 1100 && neighborSled.mHelmHealth == 1100,
            "neighbor remains temporarily borrowed without damage inside scope");
    });
    Check(sled.mGraveY == 0 && sled.mHelmType == HelmType.Bobsled && sled.mHelmHealth == 6000,
        "real restore commits removed helmet and restores sled before shock");
    MagnetFusionAbilityPatch.Postfix(outer);
    Check(sled.mGraveY == 0 && sled.mHelmHealth == 5800 && sled.BodyHealth == 1000,
        "shock reaches restored native sled, never removed added helmet");
    Check(neighborSled.mGraveY == 900 && neighborSled.mHelmType == HelmType.Bobsled && neighborSled.mHelmHealth == 6000,
        "neighbor added helmet damage survives restore and native sled remains unchanged");
    MagnetFusionAbilityPatch.Postfix(outer);
    Check(sled.mHelmHealth == 5800 && neighborSled.mGraveY == 900, "outer postfix commits shock once");
} finally { MagnetFusionAbilityPatch.Finalizer(outer); }
Check(MagnetFusions.CurrentAbility == null, "outer finalizer clears synchronous frame");

// A different magnet has already exposed both helmets when a Lumos ability
// reenters. Its inner Restore owns no helmets and cannot release the outer borrow.
var nestedMagnet = BobsledCase(out var nestedSled, out var nestedNeighbor);
var enclosingBorrow = ZombieLibrary.NativeArmorMagnet.Begin(nestedMagnet);
FusionTime.BeginTick(nestedMagnet.mBoard);
MagnetFusionAbilityPatch.Prefix(nestedMagnet, true, out var nestedInvocation);
try {
    Inner(nestedMagnet);
    MagnetFusionAbilityPatch.Postfix(nestedInvocation);
    Check(nestedSled.mGraveY == 1100 && nestedSled.mHelmType == HelmType.None && nestedNeighbor.mHelmHealth == 1100,
        "nested shock waits while enclosing borrow remains active");
} finally { MagnetFusionAbilityPatch.Finalizer(nestedInvocation); }
ZombieLibrary.NativeArmorMagnet.Restore(enclosingBorrow);
Check(nestedSled.mGraveY == 0 && nestedSled.mHelmType == HelmType.Bobsled && nestedSled.mHelmHealth == 5800,
    "nested shock reaches sled only after enclosing restore commits helmet removal");
Check(nestedNeighbor.mGraveY == 900 && nestedNeighbor.mHelmType == HelmType.Bobsled && nestedNeighbor.mHelmHealth == 6000,
    "nested shock preserves damage to neighboring added helmet");

var staleNested = BobsledCase(out var staleNestedSled, out var staleNestedNeighbor);
enclosingBorrow = ZombieLibrary.NativeArmorMagnet.Begin(staleNested);
FusionTime.BeginTick(staleNested.mBoard);
MagnetFusionAbilityPatch.Prefix(staleNested, true, out nestedInvocation);
try { Inner(staleNested); MagnetFusionAbilityPatch.Postfix(nestedInvocation); }
finally { MagnetFusionAbilityPatch.Finalizer(nestedInvocation); }
++staleNested.DataID;
ZombieLibrary.NativeArmorMagnet.Restore(enclosingBorrow);
Check(staleNestedSled.mHelmHealth == 6000 && staleNestedNeighbor.mGraveY == 1100,
    "owner generation is checked again after the enclosing borrow");

var exceptionCase = BobsledCase(out var exceptionSled, out _);
enclosingBorrow = ZombieLibrary.NativeArmorMagnet.Begin(exceptionCase);
int callbacks = 0;
var callbackError = new InvalidOperationException("callback failure");
var originalError = new InvalidOperationException("original failure");
ZombieLibrary.NativeArmorMagnet.RunAfterRestore(() => throw callbackError);
ZombieLibrary.NativeArmorMagnet.RunAfterRestore(() => ++callbacks);
var combined = ZombieLibrary.NativeArmorMagnetSelectionPatch.Finalizer(enclosingBorrow, originalError) as AggregateException;
Check(callbacks == 1 && exceptionSled.mHelmType == HelmType.Bobsled,
    "a failing callback does not skip later callbacks or armor restoration");
Check(combined != null && combined.Flatten().InnerExceptions.Contains(originalError)
    && combined.Flatten().InnerExceptions.Contains(callbackError), "original and callback errors both survive finalizer");
enclosingBorrow = ZombieLibrary.NativeArmorMagnet.Begin(exceptionCase);
ZombieLibrary.NativeArmorMagnet.RunAfterRestore(() => ++callbacks);
Check(ZombieLibrary.NativeArmorMagnetSelectionPatch.Finalizer(enclosingBorrow, null) == null && callbacks == 2,
    "next borrow starts clean after callback failure");

var failed = BobsledCase(out var failedSled, out var failedNeighbor);
FusionTime.BeginTick(failed.mBoard);
MagnetFusionAbilityPatch.Prefix(failed, true, out outer);
try {
    // Model an inner exception suppressed by some later Harmony finalizer:
    // the inner postfix is skipped although the outer caller returns normally.
    Inner(failed, normal: false);
    MagnetFusionAbilityPatch.Postfix(outer);
    Check(failedSled.mHelmHealth == 6000 && failedNeighbor.mGraveY == 1100,
        "abnormal inner return cannot replay pending shock after exception suppression");
} finally { MagnetFusionAbilityPatch.Finalizer(outer); }

var reusedOwner = BobsledCase(out var reusedSled, out _);
FusionTime.BeginTick(reusedOwner.mBoard);
MagnetFusionAbilityPatch.Prefix(reusedOwner, true, out outer);
try {
    Inner(reusedOwner); ++reusedOwner.DataID;
    MagnetFusionAbilityPatch.Postfix(outer);
    Check(reusedSled.mHelmHealth == 6000, "owner generation changed before deferred shock");
} finally { MagnetFusionAbilityPatch.Finalizer(outer); }

var nesting = BobsledCase(out var nestingSled, out _);
FusionTime.BeginTick(nesting.mBoard);
MagnetFusionAbilityPatch.Prefix(nesting, true, out outer);
try {
    Inner(nesting);
    MagnetFusionAbilityPatch.Postfix(null);
    MagnetFusionAbilityPatch.Finalizer(null);
    Check(MagnetFusions.CurrentAbility == outer.Frame,
        "skipped prefix null state neither throws nor removes parent frame");
    var child = Make();
    MagnetFusionAbilityPatch.Prefix(child, true, out var nested);
    try { Check(MagnetFusions.CurrentAbility != outer.Frame, "nested invocation isolates parent snapshot"); }
    finally { MagnetFusionAbilityPatch.Finalizer(nested); }
    Check(MagnetFusions.CurrentAbility == outer.Frame, "nested finalizer restores exact parent frame");
    MagnetFusionAbilityPatch.Postfix(outer);
    Check(nestingSled.mHelmHealth == 5800, "parent pending shock survives unrelated nested ability");
} finally { MagnetFusionAbilityPatch.Finalizer(outer); }

var thrown = BobsledCase(out var thrownSled, out _);
FusionTime.BeginTick(thrown.mBoard);
MagnetFusionAbilityPatch.Prefix(thrown, true, out outer);
try { Inner(thrown); /* Outer exception skips postfix. */ }
finally { MagnetFusionAbilityPatch.Finalizer(outer); }
Check(thrownSled.mHelmHealth == 6000 && MagnetFusions.CurrentAbility == null,
    "outer exception discards snapshot and restores frame");
Console.WriteLine($"PASS {checks} magnet clock/behavior/borrow-boundary checks; native damage and visuals remain external boundaries.");
