using FusionMod;
using Il2CppReloaded.Gameplay;

void Check(bool value, string message)
{ if (!value) throw new Exception(message); }
Plant Make(FusionPlant identity, Board? board = null, int col = 0)
{
    var plant = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker,
        mBoard = board ?? new(), mPlantCol = col, mX = col * 80 };
    plant.mBoard.m_plants.Add(plant);
    BeamShooters.Initialize(plant);
    return plant;
}
Zombie Enemy(Plant plant, float x = 500)
{
    var zombie = new Zombie { mBoard = plant.mBoard, mRow = plant.mRow, X = x };
    plant.mBoard.m_zombies.Add(zombie); return zombie;
}
void Tick(Plant plant, int count)
{ for (int i = 0; i < count; ++i) BeamShooters.Tick(plant); }

foreach (int energy in new[] { 0, 1, 2, 9, 10, 1000 })
{
    var plant = Make(FusionPlant.Beamshooter); var zombie = Enemy(plant);
    plant.mTargetX = energy;
    Tick(plant, 9); Check(zombie.Hits == 0, "initial .1 second delay");
    Tick(plant, 1);
    int cost = energy <= 1 ? 0 : 1 + energy / 10;
    Check(zombie.Damage == 10 * (1 + cost) && plant.mTargetX == energy - cost, "energy threshold and rounding");
    Tick(plant, 19); Check(zombie.Hits == 1, "no early repeated pulse");
    Tick(plant, 1); Check(zombie.Hits == 2, ".2 second attack cadence");
}
foreach (var (identity, count) in new[] { (FusionPlant.Beamshooter, 1), (FusionPlant.Rebeamer, 2),
    (FusionPlant.SplitBeam, 1), (FusionPlant.GatlingBeam, 4) })
{
    var plant = Make(identity);
    var enemies = Enumerable.Range(0, 6).Select(i => Enemy(plant, 400 + i * 10)).ToArray();
    Tick(plant, 10);
    Check(enemies.Count(z => z.Hits == 1) == count && enemies.All(z => z.Hits <= 1), "distinct nearest targets");
    enemies[0].X = 800; Tick(plant, 20);
    Check(enemies[0].Hits == 2, "valid lock persists when target moves");
    ++enemies[0].DataID; enemies[0].mDead = true;
    Tick(plant, 10); Check(enemies[count].Hits == 1, "pooled handle invalidated and next target acquired");
}
var staggered = Make(FusionPlant.Rebeamer); var oldTarget = Enemy(staggered, 400);
Tick(staggered, 15); var newTarget = Enemy(staggered, 500);
Tick(staggered, 9); Check(newTarget.Hits == 0, "new target receives its own initial delay");
Tick(staggered, 1); Check(newTarget.Hits == 1 && oldTarget.Hits == 1, "independent target clocks");
Tick(staggered, 5); Check(oldTarget.Hits == 2 && newTarget.Hits == 1, "old target retains original phase");
var light = Make(FusionPlant.Beamshooter); light.mBoard.Light = 3;
Tick(light, 49); Check(light.mTargetX == 0, "light waits .5 seconds");
Tick(light, 1); Check(light.mTargetX == 3, "light adds level");
light.mTargetX = 999; Tick(light, 50); Check(light.mTargetX == 1000, "energy cap");
var sender = Make(FusionPlant.GatlingBeam); sender.mTargetX = 100;
var adjacent = Make(FusionPlant.SplitBeam, sender.mBoard, 1);
var relay = Make(FusionPlant.SplitBeam, sender.mBoard, 3);
var farther = Make(FusionPlant.SplitBeam, sender.mBoard, 5);
var victim = Enemy(sender);
Tick(sender, 5);
Check(relay.mTargetX == 48 && sender.mTargetX == 89 && adjacent.mTargetX == 0 && farther.mTargetX == 0,
    "relay skips adjacent tile, consumes once, multiplies by target count");
Check(victim.Hits == 0, "relay takes precedence over zombies");
Tick(sender, 9); Check(relay.mTargetX == 48, "relay delay");
Tick(sender, 1); Check(relay.mTargetX == 88 && sender.mTargetX == 80, ".1 second relay cadence");
relay.mTargetX = 999; Tick(sender, 10); Check(relay.mTargetX == 1000, "relay receive cap");
relay.mBoard.Carried = relay; Tick(sender, 10);
Check(farther.mTargetX > 0, "carried relay is unavailable");
var click = Make(FusionPlant.Beamshooter); var first = Enemy(click, 400); Tick(click, 10);
var second = Enemy(click, 300); Tick(click, 20); Check(second.Hits == 0, "new target does not steal lock");
BeamShooters.Click(click, 1, 0); Tick(click, 10); Check(second.Hits == 1, "click reacquires nearest");
foreach (var state in new[] { "pause", "carry", "sleep", "dead", "bungee" })
{
    var plant = Make(FusionPlant.Beamshooter); var zombie = Enemy(plant); Tick(plant, 4);
    int phase = plant.mLaunchCounter, attribute = plant.mTargetY;
    if (state == "pause") plant.mBoard.mPaused = true;
    if (state == "carry") plant.mBoard.Carried = plant;
    if (state == "sleep") plant.mIsAsleep = true;
    if (state == "dead") plant.mDead = true;
    if (state == "bungee") plant.Bungee = true;
    Tick(plant, 100);
    Check(phase == plant.mLaunchCounter && attribute == plant.mTargetY && zombie.Hits == 0, "inactive clock: " + state);
}
var saved = Make(FusionPlant.GatlingBeam); var savedEnemy = Enemy(saved); saved.mTargetX = 100;
Tick(saved, 9);
var loaded = Make(FusionPlant.GatlingBeam, saved.mBoard);
loaded.mTargetX = saved.mTargetX; loaded.mTargetY = saved.mTargetY; loaded.mLaunchCounter = saved.mLaunchCounter;
loaded.mStateCountdown = saved.mStateCountdown;
loaded.mLaunchRate = saved.mLaunchRate; loaded.mTargetZombieID = saved.mTargetZombieID;
loaded.mStartRow = saved.mStartRow; loaded.mShootingCounter = saved.mShootingCounter;
loaded.mDoSpecialCountdown = saved.mDoSpecialCountdown;
Tick(loaded, 1); Check(savedEnemy.Hits == 1 && savedEnemy.Damage == 120, "saved phase resumes without extra delay");
var reentrant = Make(FusionPlant.GatlingBeam); var a = Enemy(reentrant); var b = Enemy(reentrant, 600);
a.OnDamage = () => { b.mDead = true; ++b.DataID; };
Tick(reentrant, 10); Check(a.Hits == 1 && b.Hits == 0, "damage callback cannot hit recycled next target");
foreach (bool coldFirst in new[] { false, true })
{
    var source = Make(FusionPlant.Beamshooter);
    Make(FusionPlant.AuroraPlantern, source.mBoard, coldFirst ? 1 : 2);
    Make(FusionPlant.Triplantern, source.mBoard, coldFirst ? 2 : 1);
    var crossingVictim = Enemy(source, 500);
    Tick(source, 10);
    Check(crossingVictim.Damage == 70 && crossingVictim.mChilledCounter == 150,
        "continuous beam applies two splitter rays and cold in either lantern order");
}
Console.WriteLine("Beam shooter checks passed.");

// Saved once-per-zombie freeze and HP-normalized bite invariants.
var aurora = Make(FusionPlant.AuroraPlantern);
PlanternFusions.Initialize(aurora);
var nearby = Enemy(aurora, 65); nearby.mIceTrapCounter = 200;
for (int i = 0; i < 100; ++i) PlanternFusions.Tick(aurora);
Check(nearby.Damage == 20 && nearby.mChilledCounter == 150 && nearby.mIceTrapCounter == 500,
    "Aurora source pulse, chill and one freeze refresh");
nearby.mIceTrapCounter = 100;
for (int i = 0; i < 100; ++i) PlanternFusions.Tick(aurora);
Check(nearby.mIceTrapCounter == 100 && nearby.Damage == 40, "freeze memory survives repeated pulses");
++nearby.DataID;
for (int i = 0; i < 100; ++i) PlanternFusions.Tick(aurora);
Check(nearby.mIceTrapCounter == 500, "new full zombie handle receives its own freeze");
var splitter = Make(FusionPlant.Triplantern, aurora.mBoard, 2);
PlanternFusions.Initialize(splitter);
var raySource = Make(FusionPlant.RayShroom, aurora.mBoard, 1);
var downstream = Enemy(raySource, 400);
PlanternFusions.LaserHit(raySource, downstream);
Check(downstream.Damage == 60, "two extra 30-damage rays only downstream of splitter");
var upstream = Enemy(raySource, 100);
PlanternFusions.LaserHit(raySource, upstream);
Check(upstream.Damage == 0, "splitter never damages upstream targets");
for (int i = 0; i < 12; ++i) Check(BiteEvents.Count(aurora, nearby, 4) == 0, "partial bites accumulate");
Check(BiteEvents.Count(aurora, nearby, 4) == 1, "52 native HP trigger one source bite");
Check(BiteEvents.Count(aurora, nearby, 48) == 1, "fractional HP survives subsequent events");
aurora.mBoard.m_gridItems.Full = true; ++nearby.DataID;
Check(BiteEvents.Count(aurora, nearby, 50) == 0, "storage failure does not repeat unsaved effects");

// Blacklight bursts include nearby ground enemies, including after a lethal hit.
var blacklightSource = Make(FusionPlant.Beamshooter);
Make(FusionPlant.BlacklightPlantern, blacklightSource.mBoard, 2);
var blacklightTarget = Enemy(blacklightSource, 500);
var splashTarget = Enemy(blacklightSource, 530);
var distantTarget = Enemy(blacklightSource, 650);
PlanternFusions.LaserHit(blacklightSource, blacklightTarget);
Check(blacklightTarget.Damage == 10 && splashTarget.Damage == 10 && distantTarget.Damage == 0,
    "Blacklight uses source radius and ground damage");
blacklightTarget.mDead = true;
PlanternFusions.LaserHit(blacklightSource, blacklightTarget);
Check(splashTarget.Damage == 20, "lethal direct laser still releases Blacklight splash");
Console.WriteLine("Blacklight checks passed.");
