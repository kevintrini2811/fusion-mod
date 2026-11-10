using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool value, string message)
{ ++checks; if (!value) throw new Exception(message); }
object? Call(Type type, string method, params object?[] args) => type
    .GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, args);
Plant Add(Board board, FusionPlant identity)
{
    var plant = new Plant { mBoard = board, mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    board.m_plants.Add(plant);
    return plant;
}

var board = new Board();
var nut = Add(board, FusionPlant.NutPot);
var plant = Add(board, SeedType.Peashooter);
for (int i = 0; i < 5; ++i)
{
    var hit = PotDefense.Capture(plant, true)!;
    plant.mPlantHealth -= 4;
    hit.Apply();
    hit.Apply();
}
Check(plant.mPlantHealth == 284 && nut.mTargetY == 0, "Five native bites lose exactly 16 HP; apply is idempotent");
plant.mPlantHealth = 4;
PotDefense.Current = new() { PotDefense.Capture(plant, true)! };
plant.mPlantHealth = 0;
board.mPlantsEaten = 1;
Check(Call(typeof(PotDefenseDeathPatch), "Prefix", plant, true) is false
    && plant.mPlantHealth == 1 && board.mPlantsEaten == 0, "Reduction prevents a lethal native bite before Die");
PotDefense.Current = null;
plant.mPlantHealth = 100;
var stale = PotDefense.Capture(plant, true)!;
++plant.DataID;
plant.mPlantHealth = 96;
stale.Apply();
Check(plant.mPlantHealth == 96, "Pool reuse cannot receive an old hit repair");
nut.mDead = true;
Check(PotDefense.Capture(plant, true) == null, "Removed support loses its effect immediately");

board = new();
Add(board, FusionPlant.SpikePot);
plant = Add(board, FusionPlant.SpikeNut);
Check(PotDefense.Damage(plant, 500) == 250 && plant.mPlantHealth == 300, "Vehicle contact damage is halved without mutating health");

board = new();
Add(board, FusionPlant.GarlicPot);
plant = Add(board, FusionPlant.PoisonPea);
var zombie = new Zombie { mBoard = board };
object?[] bite = { plant, null };
Call(typeof(PotDefenseBitePatch), "Prefix", bite);
plant.mPlantHealth -= 4;
Call(typeof(PotDefenseBitePatch), "Postfix", zombie);
Call(typeof(PotDefenseBitePatch), "Finalizer", bite[1]);
Check(plant.mPlantHealth == 290 && zombie.mYuckyFace && zombie.mTargetRow == -1130,
    "Garlic bite costs ten HP and saves the center preference");
zombie.mRow = 0; zombie.mYuckyFaceCounter = 20;
object?[] direction = { zombie, null };
Call(typeof(GarlicPotRowScope), "Prefix", direction);
Check(PotDefense.RowRandom == 0, "Fast-forward at counter 20 prefers the lower adjacent row");
Call(typeof(GarlicPotRowScope), "Finalizer", direction[1]);
Check(PotDefense.RowRandom == null, "Random scope is restored");

var shot = new Projectile { mBoard = board, mProjectileType = ProjectileType.Wintermelon,
    mCobTargetRow = MelonPot.ShotMarker };
var direct = new Zombie { mBoard = board };
MelonPot.CurrentSplash = new(shot, shot.DataID, direct);
object?[] damage = { zombie, 1 };
Call(typeof(MelonPotSplashDamagePatch), "Prefix", damage);
Check((int)damage[1]! == 53, "Crowd falloff is removed from native secondary splash");
damage = new object?[] { direct, 80 };
Call(typeof(MelonPotSplashDamagePatch), "Prefix", damage);
Check((int)damage[1]! == 80, "Direct hit retains its damage");
++shot.DataID;
damage = new object?[] { zombie, 1 };
Call(typeof(MelonPotSplashDamagePatch), "Prefix", damage);
Check((int)damage[1]! == 1, "Reused projectile cannot inherit a splash context");
MelonPot.CurrentSplash = null;

board = new();
var spike = Add(board, FusionPlant.SpikePot);
zombie = new Zombie { mBoard = board };
board.m_zombies.Add(zombie);
for (int i = 0; i <= SpikePot.EventTick; ++i) SpikePot.Tick(spike);
Check(zombie.Damage == 20 && spike.mStartRow > 0, "Original animation event deals exactly one 20-damage pulse");
board.mPaused = true;
int counter = spike.mStartRow;
SpikePot.Tick(spike);
Check(spike.mStartRow == counter && zombie.Damage == 20, "Pause stops animation and damage");
board.mPaused = false;
zombie.mZombieType = ZombieType.Zamboni;
SpikePot.Tick(spike);
Check(spike.mDead && zombie.mDead, "Tire pop consumes the pot and destroys the vehicle");


// New reactive supports: actual effect allocation, independent clocks, target
// ownership, native burst boundaries, lethal sacrifice and crater coexistence.
board = new();
var cherryPot = Add(board, FusionPlant.CherryPot);
plant = Add(board, FusionPlant.Cherrypeater);
plant.mPlantHealth = 200;
ReactivePots.Cherry(board, 40, 40, 0, 80);
Check(plant.mPlantHealth == 220 && board.m_gridItems.Count == 1, "Cherry explosion heals and allocates one buff");
var buff = board.m_gridItems[0];
buff.mGridItemCounter = 17;
ReactivePots.Cherry(board, 40, 40, 0, 80);
Check(board.m_gridItems.Count == 1 && buff.mGridItemCounter == 500,
    "Cherry duration refreshes without stacking");
plant.mLaunchCounter = 27;
ReactivePots.Speed(plant); ReactivePots.Speed(plant);
Check(plant.mLaunchCounter == 26, "Extra cooldown tick cannot skip native Repeater event at 25");
ReactivePots.Speed(plant); ReactivePots.Speed(plant);
Check(plant.mLaunchCounter == 26, "Normal native tick owns crossing the exact burst threshold");
board.mPaused = true;
ReactivePots.TickEffect(buff);
Check(buff.mGridItemCounter == 500, "Pause retains effect duration");
board.mPaused = false;
++plant.DataID;
ReactivePots.TickEffect(buff);
Check(buff.mDead, "Pool reuse invalidates the old recipient DataID");

board = new();
var hotPot = Add(board, FusionPlant.HotPot);
plant = Add(board, FusionPlant.BlazePea);
plant.mPlantHealth = 100;
ReactivePots.Fire(board, 0); ReactivePots.Fire(board, 0);
Check(plant.mPlantHealth == 120 && board.m_gridItems.Count == 1, "Hot Pot throttles duplicate row effects");
for (int i = 0; i < 20; ++i) ReactivePots.Tick(hotPot);
ReactivePots.Fire(board, 0);
Check(plant.mPlantHealth == 140 && board.m_gridItems.Count == 2, "Hot Pot stacks after its 0.2-second cooldown");
board.m_gridItems[0].mGridItemCounter = 1;
ReactivePots.TickEffect(board.m_gridItems[0]);
Check(board.m_gridItems[0].mDead && !board.m_gridItems[1].mDead, "Stack durations expire independently");

// Original JalaPot grants its bonus only to fire shooters. The user's
// Foul-shroom and Squash Spreader examples must not receive a modifier.
foreach (var identity in new[] { FusionPlant.FoulShroom, FusionPlant.SquashSpreader })
{
    var excludedBoard = new Board(); Add(excludedBoard, FusionPlant.HotPot);
    var excluded = Add(excludedBoard, identity); excluded.mPlantHealth = 100;
    ReactivePots.Fire(excludedBoard, 0);
    Check(excludedBoard.m_gridItems.Count == 0 && excluded.mPlantHealth == 100,
        "Non-fire shooter receives no Hot Pot bonus");
}
foreach (var identity in new[] { FusionPlant.BlazePea, FusionPlant.BlazeRepeater,
    FusionPlant.SplitBlaze, FusionPlant.GatlingBlaze, FusionPlant.ScorchedThreepeater,
    FusionPlant.FlameShroom, FusionPlant.FlameGloomShroom, FusionPlant.PhoenixThreepeater,
    FusionPlant.FlameCattail })
{
    var fireBoard = new Board(); Add(fireBoard, FusionPlant.HotPot);
    var recipient = Add(fireBoard, identity); recipient.mLaunchCounter = 100;
    ReactivePots.Fire(fireBoard, 0);
    for (int tick = 0; tick < 20; ++tick) { ReactivePots.Speed(recipient); --recipient.mLaunchCounter; }
    Check(recipient.mLaunchCounter == 70 && fireBoard.m_gridItems.Count == 1,
        "Each eligible fire shooter advances 30 attack ticks in 20 native ticks (+50%)");
}

// All nine native animated recipients share the same reset hazard,
// including single peas, split peas, Gatlings and Explode-o-shooter.
foreach (var identity in new[] { FusionPlant.BlazePea, FusionPlant.BlazeRepeater,
    FusionPlant.SplitBlaze, FusionPlant.GatlingBlaze, FusionPlant.CherryShooter,
    FusionPlant.Cherrypeater, FusionPlant.SplitCherry, FusionPlant.GatlingCherry,
    FusionPlant.ExplodeOShooter })
{
    var animationBoard = new Board();
    bool flame = FusionPlant.IsBlazeShooter(identity);
    Add(animationBoard, flame ? FusionPlant.HotPot : FusionPlant.CherryPot);
    var animated = Add(animationBoard, identity);
    if (flame) ReactivePots.Fire(animationBoard, 0);
    else ReactivePots.Cherry(animationBoard, 40, 40, 0, 80);
    animated.mLaunchCounter = 80; animated.mShootingCounter = 20;
    for (int tick = 0; tick < 10; ++tick)
    {
        ReactivePots.Speed(animated); --animated.mShootingCounter; --animated.mLaunchCounter;
    }
    var savedCredit = animationBoard.m_gridItems[0];
    Check(animated.mLaunchCounter == 70 && savedCredit.mGridY == 10,
        "Every native animated recipient preserves its pending shot and bonus credit");
    animated.mShootingCounter = 0;
    ReactivePots.Speed(animated);
    Check(animated.mLaunchCounter == 65 && savedCredit.mGridY == 1,
        "Every native animated recipient spends the retained bonus after its animation");
}

// Native Repeater prepares a 26-tick animation at launch=25 and at
// launch=0. UpdateShooting runs first and emits when its counter reaches 1.
// A speed bonus must never reset a pending animation before that event.
foreach (var identity in new[] { FusionPlant.BlazeRepeater, FusionPlant.Cherrypeater })
{
    var burstBoard = new Board();
    var support = Add(burstBoard, identity == FusionPlant.BlazeRepeater ? FusionPlant.HotPot : FusionPlant.CherryPot);
    var repeater = Add(burstBoard, identity);
    if (identity == FusionPlant.BlazeRepeater) ReactivePots.Fire(burstBoard, 0);
    else ReactivePots.Cherry(burstBoard, 40, 40, 0, 80);
    repeater.mLaunchCounter = 30;
    int prepared = 0, emitted = 0;
    for (int tick = 0; tick < 450; ++tick)
    {
        ReactivePots.Speed(repeater);
        if (repeater.mShootingCounter > 0 && --repeater.mShootingCounter == 1) ++emitted;
        bool reset = --repeater.mLaunchCounter <= 0;
        if (reset) repeater.mLaunchCounter = 150;
        if (reset || repeater.mLaunchCounter == 25)
        {
            Check(repeater.mShootingCounter <= 1, "Buff cannot cancel an unfinished native shot animation");
            repeater.mShootingCounter = 26;
            ++prepared;
        }
    }
    Check(emitted >= 8 && prepared - emitted <= 1, "Repeated complete two-pea volleys under pot buff");
    var credit = burstBoard.m_gridItems[0];
    repeater.mShootingCounter = 20; repeater.mLaunchCounter = 40; credit.mGridY = 1;
    ReactivePots.Speed(repeater);
    Check(repeater.mLaunchCounter == 40 && credit.mGridY == 2, "Busy animation saves bonus half-ticks");
    repeater.mShootingCounter = 0;
    ReactivePots.Speed(repeater);
    Check(repeater.mLaunchCounter == 39 && credit.mGridY == 1, "Saved bonus advances the following idle cooldown");
    repeater.mLaunchCounter = 26; credit.mGridY = 6;
    ReactivePots.Speed(repeater);
    Check(repeater.mLaunchCounter == 26 && credit.mGridY == 7, "Burst boundary keeps all unspent bonus credit");
}

// Gatling's native 100-tick animation emits at 68, 51, 35 and 18.
// Two stacked Hot Pot bonuses used to reach the next launch before shot four.
{
    var gatlingBoard = new Board(); var support = Add(gatlingBoard, FusionPlant.HotPot);
    var gatling = Add(gatlingBoard, FusionPlant.GatlingBlaze);
    ReactivePots.Fire(gatlingBoard, 0);
    for (int i = 0; i < 20; ++i) ReactivePots.Tick(support);
    ReactivePots.Fire(gatlingBoard, 0);
    gatling.mLaunchCounter = 150; gatling.mShootingCounter = 100;
    int inVolley = 0, completed = 0;
    for (int tick = 0; tick < 450; ++tick)
    {
        ReactivePots.Speed(gatling);
        if (gatling.mShootingCounter > 0)
        {
            --gatling.mShootingCounter;
            if (gatling.mShootingCounter is 68 or 51 or 35 or 18) ++inVolley;
        }
        if (--gatling.mLaunchCounter <= 0)
        {
            Check(inVolley == 4, "Stacked speed retains every projectile of the Gatling volley");
            ++completed; inVolley = 0;
            gatling.mLaunchCounter = 150; gatling.mShootingCounter = 100;
        }
    }
    Check(completed >= 4, "Multiple full Gatling volleys under stacked speed");
}

board = new();
var hypnoPot = Add(board, FusionPlant.HypnoPot);
zombie = new Zombie { mBoard = board, mMindControlled = true, X = 85 };
board.m_zombies.Add(zombie);
ReactivePots.Tick(hypnoPot);
Check(zombie.mBodyHealth == 101 && board.m_gridItems.Count == 1,
    "Hypno aura uses source radius one and heals only friendly zombies");
Check(ReactivePots.Reduce(zombie, 1) + ReactivePots.Reduce(zombie, 1) + ReactivePots.Reduce(zombie, 1) == 2,
    "Fractional protection preserves two thirds of three one-point hits");
// Reconstruct only serialized fields; no pointer registry may own the buff.
var original = board.m_gridItems[0];
var restored = new GridItem { mBoard = board, mGridItemType = original.mGridItemType,
    mGridItemState = original.mGridItemState, mSunCount = original.mSunCount,
    mSeedType = original.mSeedType, mGridX = original.mGridX, mGridY = original.mGridY,
    mGridItemCounter = original.mGridItemCounter };
board.m_gridItems.Clear(); board.m_gridItems.Add(restored);
hypnoPot.mDead = true; zombie.X = 800;
Check(ReactivePots.Reduce(zombie, 300) == 200, "Saved protection survives leaving or removing its pot");
++zombie.DataID;
Check(ReactivePots.Reduce(zombie, 300) == 300, "Zombie reuse cannot inherit protection");
ReactivePots.TickEffect(restored);
Check(restored.mDead, "Orphaned permanent effects are removed");

board = new();
hypnoPot = Add(board, FusionPlant.HypnoPot);
plant = Add(board, SeedType.Peashooter);
zombie = new Zombie { mBoard = board };
Check(ReactivePots.Bite(hypnoPot, zombie) && hypnoPot.mDead && zombie.mMindControlled
    && !plant.mDead && board.m_plants.Any(p => p.mSeedType == SeedType.Flowerpot && p.mImitaterType == SeedType.None),
    "Bite replaces only the support and hypnotizes the attacker");

board = new();
var doomPot = Add(board, FusionPlant.DoomPot);
Check(ReactivePots.PreventCrater(board, 0, 0) && doomPot.mPlantMaxHealth == 225
    && doomPot.mPlantHealth == 180, "First blocked crater costs 75 max HP and 20 percent of reduced max");
doomPot.mPlantHealth = 1;
Check(ReactivePots.PreventCrater(board, 0, 0) && doomPot.mDead,
    "Lethal health sacrifice still prevents the crater");
doomPot = Add(board, FusionPlant.DoomPot);
doomPot.mPlantHealth = 100;
var crater = new GridItem { mBoard = board, mGridItemType = GridItemType.Crater,
    mGridItemCounter = 18000, mSeedType = SeedType.Iceshroom, mSunCount = 1000 };
for (int i = 0; i < 100; ++i) ReactivePots.Crater(crater);
Check(crater.mGridItemCounter == 17900 && doomPot.mPlantHealth == 106,
    "Crater supplies one extra decay tick and heals 2 percent per second");
Check(crater.mSunCount == 1000, "Crater regeneration cannot overwrite Demise-shroom freeze duration");
doomPot.mDead = true;
ReactivePots.Crater(crater);
Check(crater.mGridItemCounter == 17900, "Removing the last Doom Pot restores normal crater progress");
board = new();
hypnoPot = Add(board, FusionPlant.HypnoPot);
zombie = new Zombie { mBoard = board };
board.FailPlant = true;
ReactivePots.Bite(hypnoPot, zombie);
Check(!hypnoPot.mDead && !zombie.mMindControlled, "Failed replacement keeps the original pot and does not hypnotize");

board = new();
doomPot = Add(board, FusionPlant.DoomPot);
var nativeDoom = Add(board, SeedType.Doomshroom);
plant = Add(board, SeedType.Peashooter);
Check(Call(typeof(DoomPotSpecialPatch), "Prefix", nativeDoom, true) is false
    && nativeDoom.mDead && !plant.mDead && !doomPot.mDead,
    "Native Doom override consumes the exploding mushroom while preserving the remaining stack");
Check(Call(typeof(DoomPotSpecialPatch), "Prefix", nativeDoom, false) is true,
    "Another prefix retaining ownership cannot trigger a second native Doom effect");

Console.WriteLine($"{checks} defensive pot checks passed.");

OffensivePotsCases.Run();
