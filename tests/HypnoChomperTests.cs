using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;

int assertions = 0;
void Require(bool condition, string message)
{ ++assertions; if (!condition) throw new Exception(message); }
Plant NewPlant() => new() { mState = PlantState.ChomperBiting, mStateCountdown = 0 };
Zombie Ally(Plant plant, float x = 100) => new() { mBoard = plant.mBoard, mMindControlled = true, X = x };

var plant = NewPlant();
plant.mTargetX = 18; plant.mTargetY = 19;
HypnoChomper.Initialize(plant);
Require(plant.mTargetX == 0 && plant.mTargetY == 0, "Fresh plants must not inherit pooled meal state");
var ally = Ally(plant);
var enemy = new Zombie { mBoard = plant.mBoard, X = 150 };
plant.Search = () => {
    Require(HypnoChomper.SearchingAllies(plant), "Second search must have an allied range scope");
    var args = new object?[] { plant, DamageRangeFlags.Ground };
    typeof(HypnoChomperAlliedRangePatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, args);
    Require((DamageRangeFlags)args[1]! == (DamageRangeFlags.Ground | DamageRangeFlags.OnlyMindcontrolled),
        "Allied search must retain original damage range and change side only");
    return ally;
};
Require(HypnoChomper.Target(plant, 0, PlantWeapon.Primary, enemy) == ally, "Nearest allied target must beat enemy");
Require(!HypnoChomper.SearchingAllies(plant), "Scope must end after successful target query");
enemy.X = 50;
Require(HypnoChomper.Target(plant, 0, PlantWeapon.Primary, enemy) == enemy, "Nearer enemy keeps normal selection");
enemy.X = ally.X;
Require(HypnoChomper.Target(plant, 0, PlantWeapon.Primary, enemy) == enemy, "Native ordinary result wins integer-X ties");
Require(HypnoChomper.Target(plant, 0, PlantWeapon.Secondary, enemy) == enemy, "Secondary weapon must not search allies");
plant.Search = () => throw new InvalidOperationException("expected target failure");
try { HypnoChomper.Target(plant, 0, PlantWeapon.Primary, null); }
catch (InvalidOperationException) {}
Require(!HypnoChomper.SearchingAllies(plant), "Scope must restore on failed native target lookup");

plant.Search = null; plant.Target = ally;
var meal = HypnoChomper.Capture(plant);
Require(meal != null, "Allied bite must be observed before native devour");
plant.mState = PlantState.ChomperBitingGotOne;
HypnoChomper.Complete(meal);
Require(plant.mTargetX == 0 && plant.mTargetY == 0, "Successful animation without dead target must not reward");
ally.mDead = true;
HypnoChomper.Complete(meal);
Require(plant.mTargetX == 1 && plant.mTargetY == 1, "Confirmed allied devour must save digestion and pending reward");
plant.mBoard.FailCoin = true;
HypnoChomper.DropPacket(plant);
Require(plant.mTargetY == 1 && plant.mBoard.Coins.Count == 0, "Full coin pool must preserve pending reward");
plant.mBoard.FailCoin = false;
HypnoChomper.DropPacket(plant);
HypnoChomper.DropPacket(plant);
Require(plant.mBoard.Coins.Count == 1 && plant.mTargetY == 0, "Reward must allocate exactly once after retry");
var packet = plant.mBoard.Coins.Single();
Require(packet.mUsableSeedType == SeedType.Hypnoshroom && packet.mController?.SeedType == SeedType.Hypnoshroom,
    "Saved packet type and controller must agree on Hypno-shroom");
plant.mState = PlantState.ChomperDigesting;
plant.mStateCountdown = 4000;
HypnoChomper.AfterNative(plant, PlantState.ChomperBitingGotOne);
Require(plant.mStateCountdown == 1000 && plant.mTargetX == 0, "Allied meal must digest for ten seconds");
plant.mStateCountdown = 700;
HypnoChomper.AfterNative(plant, PlantState.ChomperDigesting);
Require(plant.mStateCountdown == 700, "Repeated update must not restart digestion");
ChomperFusions.Fertilize(plant);
Require(plant.mStateCountdown == 50, "Fertilizer must shorten digestion to half a second");
plant.mStateCountdown = 12;
ChomperFusions.Fertilize(plant);
Require(plant.mStateCountdown == 12, "Fertilizer must not lengthen short digestion");
plant.mState = PlantState.ChomperBiting; plant.mStateCountdown = 70;
ChomperFusions.Fertilize(plant);
Require(plant.mStateCountdown == 70, "Fertilizer must not skip an uncommitted bite");
plant.mTargetX = 0; plant.mState = PlantState.ChomperDigesting; plant.mStateCountdown = 4000;
HypnoChomper.AfterNative(plant, PlantState.ChomperBitingGotOne);
Require(plant.mStateCountdown == 4000, "Ordinary enemies must retain forty-second digestion");

var restored = NewPlant(); restored.mTargetX = 1; restored.mState = PlantState.ChomperDigesting;
restored.mStateCountdown = 4000;
HypnoChomper.AfterNative(restored, PlantState.ChomperBitingGotOne);
Require(restored.mStateCountdown == 1000, "Saved allied meal must reconstruct without a transient dictionary");
var stale = NewPlant(); stale.Target = Ally(stale);
var staleMeal = HypnoChomper.Capture(stale);
stale.mState = PlantState.ChomperBitingGotOne; stale.Target!.mDead = true; stale.Target.DataID++;
HypnoChomper.Complete(staleMeal);
Require(stale.mTargetY == 0, "Reused target must never reward an older generation");
stale.Target.DataID--; stale.DataID++;
HypnoChomper.Complete(staleMeal);
Require(stale.mTargetY == 0, "Reused plant must never inherit an older meal");
var paused = NewPlant(); paused.Target = Ally(paused); paused.mBoard.mPaused = true;
Require(HypnoChomper.Capture(paused) == null, "Paused board must not observe a bite");
paused.mTargetY = 1; HypnoChomper.DropPacket(paused);
Require(paused.mBoard.Coins.Count == 0, "Paused board must not allocate rewards");

var bitten = NewPlant();
var attacker = new Zombie { mBoard = bitten.mBoard };
bitten.mBoard.FailPlant = true;
Require(HypnoChomper.Bite(bitten, attacker) && !bitten.mDead && !attacker.mMindControlled,
    "Failed replacement must preserve original plant and enemy allegiance");
bitten.mBoard.FailPlant = false;
Require(HypnoChomper.Bite(bitten, attacker) && bitten.mDead && attacker.mMindControlled,
    "Successful bite must replace with native Chomper and hypnotize attacker");
Require(bitten.mBoard.Plants.Count == 1, "Bite replacement must allocate once");
var otherBoard = NewPlant();
Require(HypnoChomper.Bite(otherBoard, new Zombie { mBoard = new Board { Pointer = (IntPtr)77 } })
    && !otherBoard.mDead && otherBoard.mBoard.Plants.Count == 0, "Cross-board bite must not mutate plants");
var pooled = NewPlant();
var pooledAttacker = new Zombie { mBoard = pooled.mBoard };
pooled.mBoard.OnAddPlant = () => pooled.DataID++;
HypnoChomper.Bite(pooled, pooledAttacker);
Require(!pooled.mDead && !pooledAttacker.mMindControlled && pooled.mBoard.Plants.Single().mDead,
    "Plant generation change during allocation must abort and consume temporary replacement only");

// Exercise the real patch boundary. UpdateAbilities decrements the timer before
// running its inlined Chomper body; invoking Complete alone hid this regression.
var patch = typeof(HypnoChomperMealPatch);
void PatchedTick(Plant current, Action nativeBody, bool runOriginal = true)
{
    var args = new object?[] { current, runOriginal, null };
    patch.GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, args);
    if (runOriginal) nativeBody();
    patch.GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new[] { current, args[2] });
}
void NativeBite(Plant current, bool decrement)
{
    if (decrement && current.mStateCountdown > 0) --current.mStateCountdown;
    if (current.mState != PlantState.ChomperBiting || current.mStateCountdown != 0) return;
    var victim = current.FindTargetZombie(current.mRow, PlantWeapon.Primary);
    if (victim == null) current.mState = PlantState.ChomperBitingMissed;
    else { victim.mDead = true; current.mState = PlantState.ChomperBitingGotOne; }
}
var targets = (IEnumerable<MethodBase>)patch.GetMethod("TargetMethods", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, null)!;
Require(targets.Select(method => method.Name).Order().SequenceEqual(new[] { "UpdateAbilities", "UpdateChomper" }),
    "Both the actual inlined caller and standalone native Chomper entry must be hooked");
foreach (bool inline in new[] { true, false })
{
    var actual = NewPlant(); actual.Target = Ally(actual); actual.mStateCountdown = inline ? 1 : 0;
    PatchedTick(actual, () => NativeBite(actual, inline));
    Require(actual.mState == PlantState.ChomperBitingGotOne && actual.mTargetX == 1 && actual.mBoard.Coins.Count == 1,
        "Prefix/native decrement-and-devour/Postfix must record a real allied meal on both paths");
    PatchedTick(actual, () => { actual.mState = PlantState.ChomperDigesting; actual.mStateCountdown = 4000; });
    Require(actual.mStateCountdown == 1000 && actual.mBoard.Coins.Count == 1,
        "Next native digestion transition must use ten seconds without another reward");
}
var countdown = NewPlant(); countdown.Target = Ally(countdown); countdown.mStateCountdown = 70;
for (int tick = 0; tick < 69; ++tick) PatchedTick(countdown, () => NativeBite(countdown, true));
Require(countdown.mStateCountdown == 1 && countdown.mBoard.Coins.Count == 0 && countdown.mTargetX == 0,
    "Capturing an earlier bite tick must never award before native devour");
PatchedTick(countdown, () => NativeBite(countdown, true));
Require(countdown.mBoard.Coins.Count == 1 && countdown.mTargetX == 1,
    "Final one-to-zero native countdown tick must award once");
var nested = NewPlant(); nested.Target = Ally(nested); nested.mStateCountdown = 1;
PatchedTick(nested, () => {
    --nested.mStateCountdown;
    PatchedTick(nested, () => NativeBite(nested, false));
});
Require(nested.mBoard.Coins.Count == 1 && nested.mTargetX == 1,
    "Nested UpdateAbilities/UpdateChomper postfixes must not duplicate an allied reward");
PatchedTick(nested, () => PatchedTick(nested, () => {
    nested.mState = PlantState.ChomperDigesting;
    nested.mStateCountdown = 4000;
}));
Require(nested.mStateCountdown == 1000 && nested.mTargetX == 0 && nested.mBoard.Coins.Count == 1,
    "Nested digestion postfixes must preserve the inner allied shortening and exactly one packet");
var shortened = NewPlant(); shortened.mTargetX = 1;
PatchedTick(shortened, () => {
    shortened.mState = PlantState.ChomperDigesting;
    shortened.mStateCountdown = 50;
});
Require(shortened.mStateCountdown == 50 && shortened.mTargetX == 0,
    "Allied digestion must preserve an already shorter native countdown");
var nativeDuration = NewPlant();
PatchedTick(nativeDuration, () => PatchedTick(nativeDuration, () => {
    nativeDuration.mState = PlantState.ChomperDigesting;
    nativeDuration.mStateCountdown = 4000;
}));
Require(nativeDuration.mStateCountdown == 4000,
    "Nested ordinary digestion must retain the duration assigned by its native owner");
var ordinary = NewPlant(); ordinary.Target = new Zombie { mBoard = ordinary.mBoard }; ordinary.mStateCountdown = 1;
PatchedTick(ordinary, () => NativeBite(ordinary, true));
Require(ordinary.mBoard.Coins.Count == 0 && ordinary.mTargetX == 0, "Native enemy devour must not award a Hypno packet");
PatchedTick(ordinary, () => { ordinary.mState = PlantState.ChomperDigesting; ordinary.mStateCountdown = 4000; });
Require(ordinary.mStateCountdown == 4000, "Native enemy digestion must remain forty seconds");
var miss = NewPlant(); miss.Target = Ally(miss); miss.mStateCountdown = 1;
PatchedTick(miss, () => { miss.Target = null; NativeBite(miss, true); });
Require(miss.mState == PlantState.ChomperBitingMissed && miss.mBoard.Coins.Count == 0 && miss.mTargetX == 0,
    "A target that leaves between Prefix and native commit must not award");
var skipped = NewPlant(); skipped.Target = Ally(skipped); skipped.mStateCountdown = 1;
PatchedTick(skipped, () => NativeBite(skipped, true), runOriginal: false);
Require(skipped.mState == PlantState.ChomperBiting && skipped.mBoard.Coins.Count == 0,
    "An original body skipped by another patch must not commit a meal");
Console.WriteLine($"Hypno Chomper offline checks: {assertions} assertions passed; no game code loaded.");
