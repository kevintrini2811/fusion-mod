using FusionMod;
using ZombieLibrary;
using Il2CppReloaded.Gameplay;

static void Check(bool value, string contract)
{ if (!value) throw new Exception(contract); }
static Zombie Target(Board board)
{ var zombie = new Zombie { mBoard = board }; board.m_zombies.Add(zombie); return zombie; }

var board = new Board();
var plant = board.AddPlant(1, 1, FusionPlant.TingleKelp.NativeType, FusionPlant.TingleKelp.Marker)!;
var zombie = Target(board);
TingleKelp.Tick(plant);
Check(plant.mStartRow == 67, "proximity starts original explosion clip");
board.mPaused = true; TingleKelp.Tick(plant);
Check(plant.mStartRow == 67, "pause does not advance explosion");
board.mPaused = false;
for (int i = 0; i < 66; ++i) TingleKelp.Tick(plant);
Check(!plant.mDead && board.m_projectiles.Count == 0, "no early burst");
TingleKelp.Tick(plant); TingleKelp.Explode(plant);
Check(plant.mDead && board.m_projectiles.Count == 30, "exactly one thirty-projectile burst");
var shot = board.m_projectiles[0];
KelpProjectiles.Impact(shot, zombie);
KelpProjectiles.Impact(shot, zombie);
Check(zombie.Damage.SequenceEqual(new[] { 20 }) && KelpEffects.Stacks(zombie) == 1,
    "direct damage precedes one water Kelp application");
var effect = KelpEffects.Find(zombie)!;
KelpEffects.Add(zombie, 200);
Check(effect.mGridItemCounter == 400 && KelpEffects.Stacks(zombie) == 2, "slow durations add without doubling speed penalty");
for (int i = 0; i < 400; ++i) KelpEffects.Tick(effect);
Check(!KelpEffects.Slowed(zombie) && KelpEffects.Stacks(zombie) == 2, "stack history outlives slow");
for (int i = 0; i < 4; ++i) KelpEffects.Add(zombie, 200);
Check(zombie.Damage.Count == 1, "first six applications cause no percentage hit");
KelpEffects.Add(zombie, 200);
Check(zombie.Damage.Last() == 10, "seventh application deals one percent of maximum health");
effect.mGridY = 30; zombie.mBodyHealth = 10000;
KelpEffects.Add(zombie, 200);
Check(zombie.Damage.TakeLast(2).SequenceEqual(new[] { 10, 1000 }), "thirty-first application has two independent damage branches");
zombie.DataID += 1000;
Check(KelpEffects.Find(zombie) == null, "recycled zombie slot cannot inherit effect");
KelpEffects.Tick(effect); Check(effect.mDead, "orphaned saved effect is removed");
var boss = Target(board); boss.mZombieType = ZombieType.Boss;
Check(!KelpEffects.Add(boss, 200) && KelpEffects.Find(boss) == null, "boss immunity");
var failed = Target(board); board.m_gridItems.Fail = true;
Check(!KelpEffects.Add(failed, 200) && failed.Damage.Count == 0, "allocation failure applies no unsaved status");
board.m_gridItems.Fail = false;
var land = Target(board); land.mInPool = false;
KelpProjectiles.Impact(board.m_projectiles[1], land);
Check(land.Damage.Single() == 20 && KelpEffects.Find(land) == null, "land projectile damage does not grant Kelp");
var lethal = Target(board); lethal.mBodyHealth = 1;
int before = board.m_plants.Count;
KelpProjectiles.Impact(board.m_projectiles[2], lethal);
Check(board.m_plants.Count == before + 1, "lethal water Kelp hit sprouts one aquatic seed");
board.Occupied = true;
Check(!TingleKelp.Spawn(board, 1, 1) && board.m_plants.Count == before + 1,
    "reproduction cannot consume an existing plant");
Check(!TingleKelp.Spawn(board, -1, 1), "out-of-bounds reproduction rejected");
var grappleBoard = new Board();
var grapple = grappleBoard.AddPlant(1, 1, FusionPlant.GrappleKelp.NativeType, FusionPlant.GrappleKelp.Marker)!;
GrappleKelp.Initialize(grapple);
var grappleTarget = Target(grappleBoard); grappleTarget.mPosX = 160;
GrappleKelp.Tick(grapple);
for (int i = 0; i < 49; ++i) GrappleKelp.Tick(grapple);
Check(grappleTarget.Damage.Count == 0, "grapple preparation does not attack early");
GrappleKelp.Tick(grapple);
Check(grappleTarget.Damage.SequenceEqual(new[] { 30 }) && KelpEffects.Stacks(grappleTarget) == 2,
    "first grapple event adds Kelp before damage and another stack afterwards");
for (int i = 50; i < 83; ++i) GrappleKelp.Tick(grapple);
Check(grappleTarget.Damage.TakeLast(2).SequenceEqual(new[] { 30, 15 }),
    "second grapple event uses updated Kelp count for its extra hit");
float beforeKnockback = grappleTarget.mPosX;
for (int i = 83; i < 117; ++i) GrappleKelp.Tick(grapple);
Check(grappleTarget.mPosX > beforeKnockback && KelpEffects.Stacks(grappleTarget) == 4,
    "third grapple event adds Kelp, damages and knocks back");
Check(GrappleKelp.Animation(grapple, 0).Clip == "f(x)=sinx", "controller clip follows stored attack time");
// The third hit must preserve contact even before the native periodic chew
// check changes the zombie to eating. Damage and Kelp still apply normally.
foreach (float position in new[] { 140f, 130f })
{
    var contactBoard = new Board();
    var contactPlant = contactBoard.AddPlant(1, 1,
        FusionPlant.GrappleKelp.NativeType, FusionPlant.GrappleKelp.Marker)!;
    GrappleKelp.Initialize(contactPlant);
    var eater = Target(contactBoard);
    eater.mPosX = position;
    Check(GrappleKelp.InBiteRange(contactPlant, eater), "native bite overlap reached");
    GrappleKelp.Hit(contactPlant, 2);
    Check(eater.mPosX == position && eater.Damage.SequenceEqual(new[] { 30 })
        && KelpEffects.Stacks(eater) == 1,
        "contact survives third hit without losing damage or Kelp status");
}
var approaching = Target(grappleBoard);
approaching.mPosX = 141;
Check(!GrappleKelp.InBiteRange(grapple, approaching), "nineteen pixels is outside native bite range");
GrappleKelp.Hit(grapple, 2);
Check(approaching.mPosX > 141, "distant third hit retains original knockback");
var nutBoard = new Board();
var nut = nutBoard.AddPlant(1, 1, FusionPlant.TangleNut.NativeType, FusionPlant.TangleNut.Marker)!;
TangleNut.Initialize(nut);
var crusher = Target(nutBoard);
WeedFusions.CurrentCrush = new(crusher, nutBoard, crusher.DataID);
TangleNut.Crush(nut); TangleNut.Crush(nut);
Check(nut.mPlantHealth == 3950 && KelpEffects.Stacks(crusher) == 1,
    "same-frame crush cannot apply repeated health costs or stacks");
TangleNut.Tick(nut); TangleNut.Tick(nut); TangleNut.Crush(nut);
Check(nut.mPlantHealth == 3900 && KelpEffects.Stacks(crusher) == 2, "crush pulses at source 50Hz");
nutBoard.mPaused = true; TangleNut.Tick(nut);
Check(nut.mTargetY == 2, "pause preserves crush cadence");
nutBoard.mPaused = false; nut.mTargetY = 0; crusher.mInPool = false;
TangleNut.Crush(nut);
Check(nut.mPlantHealth == 3900, "dry crushing has no water counter-effect");
var spikeBoard = new Board();
var spike = spikeBoard.AddPlant(1, 1, FusionPlant.SpikeKelp.NativeType, FusionPlant.SpikeKelp.Marker)!;
SpikeKelp.Initialize(spike);
var driver = Target(spikeBoard); driver.mZombieType = ZombieType.Catapult;
SpikeKelp.Tick(spike);
for (int i = 0; i < 32; ++i) SpikeKelp.Tick(spike);
Check(driver.Damage.Count == 0, "spike waits for original contact event");
SpikeKelp.Tick(spike);
Check(driver.Damage.Single() == 20 && spike.mPlantHealth == 290 && KelpEffects.Stacks(driver) == 1,
    "spike driver hit deals twenty, applies Kelp and costs ten health");
driver.mZombiePhase = ZombiePhase.ImpGettingThrown;
Check(!SpikeKelp.Target(spike, driver), "airborne imp excluded by source status rule");
driver.mZombiePhase = ZombiePhase.ZombieNormal;
WeedFusions.CurrentCrush = new(driver, spikeBoard, driver.DataID);
Check(SpikeKelp.DriverCrush(spike), "driver contact preserves repeated spike attacks");
Console.WriteLine("Kelp fusion adapter contracts passed.");

Plant SpreadPlant(Board board, FusionPlant identity, int row = 1)
{
    var p = board.AddPlant(1, row, identity.NativeType, identity.Marker)!;
    KelpSpreadFusions.Initialize(p); return p;
}
var spreadBoard = new Board();
var spread = SpreadPlant(spreadBoard, FusionPlant.KelpSpreader);
var fish = Target(spreadBoard); fish.mPosX = 400;
for (int i = 0; i < 50; ++i) KelpSpreadFusions.ShootTick(spread);
Check(spreadBoard.m_projectiles.Count == 0, "kelp waits for original half-second shot event");
KelpSpreadFusions.ShootTick(spread);
Check(spreadBoard.m_projectiles.Count == 3 && spreadBoard.m_projectiles.Select(p => p.mRow).Distinct().Count() == 3,
    "kelp fires all three rows with aquatic target");
var straight = spreadBoard.m_projectiles.First(p => p.mRow == fish.mRow);
KelpProjectiles.Impact(straight, fish);
Check(fish.Damage.First() == 30 && KelpEffects.Stacks(fish) == 1,
    "straight kelp uses PlantData thirty damage before applying Tangle");
var radiusBoard = new Board(); var outer = Target(radiusBoard); outer.mPosX = 50; outer.mPosY = 0;
MiniSquashProjectiles.Area(radiusBoard, outer.mRow, 0, 40, 40, 0);
Check(outer.Damage.Count == 0, "mini squash half-unit radius excludes outer neighbor");
MiniSquashProjectiles.Area(radiusBoard, outer.mRow, 0, 40, 600, 0, sourceRadius: 1);
Check(outer.Damage.SequenceEqual(new[] { 600 }), "strangle has independent full-unit radius");
var managedBoard = new Board(); var managed = SpreadPlant(managedBoard, FusionPlant.KelpSpreader);
var managedEnemy = Target(managedBoard); managedEnemy.mPosX = 400;
for (int i = 0; i < 51; ++i) SquashSpreadAbilityPatch.Prefix(managed);
Check(managedBoard.m_projectiles.Count == 3,
    "ability entry emits kelp even when managed native calls bypass Harmony");
Check(!KelpSpreadFusions.NativeTick, "native kelp context restored");

var dryBoard = new Board(); var dry = SpreadPlant(dryBoard, FusionPlant.KelpSpreader);
var dryEnemy = Target(dryBoard); dryEnemy.mPosX = 400; dryEnemy.mInPool = false; dryEnemy.mRow = dry.mRow - 1;
for (int i = 0; i < 51; ++i) KelpSpreadFusions.ShootTick(dry);
Check(dryBoard.m_projectiles.Count == 3, "adjacent land zombie triggers all three kelp shots");
KelpProjectiles.Impact(dryBoard.m_projectiles.First(p => p.mRow == dryEnemy.mRow), dryEnemy);
Check(dryEnemy.Damage.SequenceEqual(new[] { 30 }) && KelpEffects.Stacks(dryEnemy) == 0,
    "land target takes damage without aquatic Tangle status");
var distantBoard = new Board(); var distant = SpreadPlant(distantBoard, FusionPlant.KelpSpreader);
var distantEnemy = Target(distantBoard); distantEnemy.mPosX = 400; distantEnemy.mRow = distant.mRow + 2;
for (int i = 0; i < 51; ++i) KelpSpreadFusions.ShootTick(distant);
Check(distantBoard.m_projectiles.Count == 0, "kelp excludes zombies beyond adjacent rows");
var edgeBoard = new Board(); var edge = SpreadPlant(edgeBoard, FusionPlant.SquashSpreader, 0);
var edgeEnemy = Target(edgeBoard); edgeEnemy.mRow = 0; edgeEnemy.mPosX = 400;
for (int i = 0; i < 51; ++i) KelpSpreadFusions.ShootTick(edge);
Check(edgeBoard.m_projectiles.Count == 2, "squash fires two existing rows at edge");
for (int i = 0; i < 20; ++i) KelpSpreadFusions.ShootTick(edge);
Check(edgeBoard.m_projectiles.Count == 3 && edgeBoard.m_projectiles.Count(p => p.mRow == 0) == 2,
    "missing squash row redirects after point-two seconds");
var pausedPhase = edge.mStartRow; edgeBoard.mPaused = true;
KelpSpreadFusions.ShootTick(edge);
Check(edge.mStartRow == pausedPhase, "pause preserves new shooter animation");
var dragBoard = new Board(); var drag = SpreadPlant(dragBoard, FusionPlant.StrangleKelp);
var victim = Target(dragBoard); victim.mPosX = 120;
for (int use = 0; use < 3; ++use)
{
    victim.mDead = false; victim.mBodyHealth = 1000;
    drag.mState = PlantState.TanglekelpGrabbing; drag.mStateCountdown = 0;
    drag.mTargetZombieID = (ZombieID)victim.DataID;
    Check(!KelpSpreadFusions.KelpTick(drag), "strangle owns native removal boundary");
    Check(victim.mDead && drag.mTargetX == use + 1 && drag.mDead == (use == 2),
        "exactly three saved drag uses, full victim DataID");
}
var staleBoard = new Board(); var staleDrag = SpreadPlant(staleBoard, FusionPlant.StrangleKelp);
var recycled = Target(staleBoard); staleDrag.mState = PlantState.TanglekelpGrabbing;
staleDrag.mTargetZombieID = (ZombieID)recycled.DataID; ++recycled.DataID;
KelpSpreadFusions.KelpTick(staleDrag);
Check(!recycled.mDead && staleDrag.mTargetX == 1, "recycled victim remains alive");
var jumpBoard = new Board(); var jumpPlant = SpreadPlant(jumpBoard, FusionPlant.SquashSpreader);
MiniSquashProjectiles.Spawn(jumpPlant, 1);
var jump = jumpBoard.m_projectiles.Single(); var nearby = Target(jumpBoard);
// Isolate landing splash from the independent direct-contact path. The target
// remains within the splash radius, but outside the projectile's body sweep.
nearby.mPosX = jump.mPosX + 25; nearby.mPosY = jump.mPosY - 40;
jump.mPosZ = -.01f; jump.mVelZ = 1;
MiniSquashProjectiles.Move(jump);
Check(!jump.mDead && jump.mVelZ < 0 && nearby.Damage.Contains(40), "landing damages nearby targets then bounces");
var miniContactBoard = new Board(); var miniContactPlant = SpreadPlant(miniContactBoard, FusionPlant.SquashSpreader);
MiniSquashProjectiles.Spawn(miniContactPlant, 1);
var contact = miniContactBoard.m_projectiles.Single(); var touched = Target(miniContactBoard);
touched.mPosX = contact.mPosX; touched.mPosY = contact.mPosY - 40;
contact.mPosZ = -.01f; contact.mVelZ = 1;
MiniSquashProjectiles.Move(contact);
Check(touched.Damage.Contains(40) && contact.mVelZ == 0, "direct body contact owns its separate vertical stop");
jumpBoard.Water = true; jump.mPosZ = -.01f; jump.mVelZ = 1;
int previousHits = nearby.Damage.Count;
MiniSquashProjectiles.Move(jump);
Check(jump.mDead && nearby.Damage.Count == previousHits, "mini squash sinks on water without land damage");
var weedBoard = new Board(); var weed = SpreadPlant(weedBoard, FusionPlant.Squashweed);
var walker = Target(weedBoard); walker.mPosX = weed.mX + 40; walker.mInPool = false;
for (int i = 0; i < 33; ++i) KelpSpreadFusions.WeedTick(weed);
Check(walker.Damage.SequenceEqual(new[] { 20 }) && weedBoard.m_projectiles.Count == 1,
    "squashweed contact emits damage and one mini squash at original event");
Check(walker.mPosX > weed.mX + 40, "squashweed knocks surviving contact target back");
int weedPhase = weed.mStartRow; weedBoard.mPaused = true; KelpSpreadFusions.WeedTick(weed);
Check(weed.mStartRow == weedPhase, "pause freezes squashweed attack");
var spentBoard = new Board();
var spentKelp = SpreadPlant(spentBoard, FusionPlant.KelpSpreader);
var spentVictim = Target(spentBoard);
spentKelp.mState = PlantState.TanglekelpGrabbing;
spentKelp.mStateCountdown = 1;
spentKelp.mTargetZombieID = (ZombieID)spentVictim.DataID;
Check(KelpSpreadFusions.KelpTick(spentKelp) && !spentKelp.mDead && !spentVictim.mDead,
    "incomplete grab retains both plant and victim");
spentKelp.mStateCountdown = 0;
Check(!KelpSpreadFusions.KelpTick(spentKelp), "completed grab replaces native removal once");
Check(spentVictim.mDead && spentKelp.mDead && spentKelp.mTargetZombieID == 0,
    "Kelp-spreader inherits the source single-use drag and removes its victim");
// Target queries borrow an ordinary aquatic identity, never another fusion.
foreach (var edible in new[] { FusionPlant.GrappleKelp, FusionPlant.TangleNut })
{
    var p = new Board().AddPlant(1, 1, edible.NativeType, edible.Marker)!;
    EdibleKelpTargetPatch.Prefix(p, out var scope);
    Check(p.mSeedType == SeedType.Seashroom && p.mImitaterType == SeedType.None,
        "edible kelp query uses a neutral aquatic identity");
    EdibleKelpTargetPatch.Prefix(p, out var nested);
    EdibleKelpTargetPatch.Finalizer(nested);
    Check(p.mSeedType == SeedType.Seashroom, "nested target query preserves outer identity scope");
    EdibleKelpTargetPatch.Finalizer(scope);
    Check(FusionPlant.From(p) == edible, "target query restores both saved identity fields");
    EdibleKelpTargetPatch.Prefix(p, out scope);
    ++p.DataID;
    p.mSeedType = SeedType.Peashooter;
    EdibleKelpTargetPatch.Finalizer(scope);
    Check(p.mSeedType == SeedType.Peashooter, "target query never corrupts a recycled plant");
}
var poolSquash = Target(new Board());
poolSquash.mZombieType = ZombieType.SquashHead;
SquashZombiePoolPatch.Prefix(poolSquash, out var poolScope);
Check(poolSquash.mZombieType == ZombieType.Normal, "squash enters native pool allowlist during query");
poolSquash.mInPool = true;
SquashZombiePoolPatch.Finalizer(poolSquash, poolScope);
Check(poolSquash.mInPool && poolSquash.mZombieType == ZombieType.SquashHead,
    "pool transition retains water state and restores squash identity");
SquashZombiePoolPatch.Prefix(poolSquash, out poolScope);
++poolSquash.DataID;
poolSquash.mZombieType = ZombieType.Catapult;
SquashZombiePoolPatch.Finalizer(poolSquash, poolScope);
Check(poolSquash.mZombieType == ZombieType.Catapult, "pool query never corrupts a recycled zombie");
Console.WriteLine("Kelp and squash spread contracts prepared.");

foreach (var kind in new[] { ZombieType.Football, ZombieType.JackInTheBox, ZombieType.Dancer,
    ZombieType.BackupDancer, ZombieType.Yeti, ZombieType.Gargantuar, ZombieType.RedeyeGargantuar })
{
    var water = new Board(); water.mPlantRow[2] = PlantRowType.Pool;
    bool allowed = false;
    Check(!ExtraZombiePoolRowPatch.Prefix(water, 2, kind, ref allowed) && allowed,
        $"{kind} may use an available pool row");
    var actor = Target(water); actor.mZombieType = kind;
    SquashZombiePoolPatch.Prefix(actor, out var entry);
    Check(actor.mZombieType == ZombieType.Normal, $"{kind} enters native water transition");
    actor.mInPool = true;
    SquashZombiePoolPatch.Finalizer(actor, entry);
    Check(actor.mZombieType == kind && actor.mInPool, $"{kind} keeps its original AI identity in water");
    water.BlockedRows.Add(2); allowed = true;
    Check(!ExtraZombiePoolRowPatch.Prefix(water, 2, kind, ref allowed) && !allowed,
        $"{kind} cannot use an unavailable row");
}
{
    var water = new Board(); water.mPlantRow[2] = PlantRowType.Pool; water.BlockedRows.Add(1);
    bool allowed = true;
    Check(!ExtraZombiePoolRowPatch.Prefix(water, 2, ZombieType.Dancer, ref allowed) && !allowed,
        "dancer retains native neighbor requirement");
    Check(ExtraZombiePoolRowPatch.Prefix(water, 2, ZombieType.Zamboni, ref allowed),
        "unrequested types retain native pool restrictions");
    Check(ExtraZombiePoolRowPatch.Prefix(water, 0, ZombieType.Gargantuar, ref allowed),
        "land rows retain native rules");
}

// Pool presentation never moves the native collision or saved altitude.
foreach (var kind in new[] { ZombieType.Gargantuar, ZombieType.RedeyeGargantuar,
    ZombieType.Dancer, ZombieType.BackupDancer, ZombieType.JackInTheBox, ZombieType.Football })
{
    var actor = Target(new Board()); actor.mZombieType = kind; actor.mAltitude = -40;
    var draw = new ZombieDrawPosition { BodyY = 40, ClipHeight = 32 };
    PoolZombieDepthPatch.Apply(actor, ref draw);
    Check(draw.BodyY == 40 && draw.ImageOffsetY > 0 && Math.Abs(draw.BodyY + draw.ImageOffsetY - draw.ClipHeight - 8) < .001f,
        $"{kind} sits deeper without moving the waterline");
    Check(actor.mAltitude == -40, "presentation leaves saved altitude unchanged");
    actor.mAltitude = 0; draw = new() { BodyY = 0, ClipHeight = 0 };
    PoolZombieDepthPatch.Apply(actor, ref draw);
    Check(draw.BodyY == 0 && draw.ImageOffsetY == 0 && draw.ClipHeight == 0, "entry starts without a depth jump");
    actor.mInPool = false; actor.mAltitude = -40;
    PoolZombieDepthPatch.Apply(actor, ref draw);
    Check(draw.BodyY == 0, "land remains unchanged");
}
{
    var water = new Board();
    var leader = Target(water);
    var dancer = Target(water); dancer.mZombieType = ZombieType.BackupDancer;
    dancer.mZombiePhase = ZombiePhase.DancerRising; dancer.mAltitude = -200;
    PoolSummonedDancerPatch.Postfix(leader, (ZombieID)dancer.DataID);
    Check(!dancer.mInPool, "summoned dancer discards the factory row water state");
    Check(!SquashZombiePoolPatch.Prefix(dancer, out var rising), "water entry waits for the rise");
    SquashZombiePoolPatch.Finalizer(dancer, rising);
    Check(dancer.mZombieType == ZombieType.BackupDancer && dancer.mAltitude == -200,
        "rise preserves identity and native altitude");
    PoolDancerRisePatch.Prefix(dancer, out var beforeRise);
    PoolDancerRisePatch.Postfix(dancer, beforeRise);
    Check(dancer.PoolChecks == 0, "ongoing rise does not start pool entry");
    dancer.mZombiePhase = ZombiePhase.ZombieNormal;
    PoolDancerRisePatch.Postfix(dancer, beforeRise);
    Check(dancer.PoolChecks == 1 && dancer.mAltitude == 0, "completed rise starts native pool query");
    dancer.PoolChecks = 0; dancer.DataID++;
    PoolDancerRisePatch.Postfix(dancer, beforeRise);
    Check(dancer.PoolChecks == 0, "rise callback never changes a reused actor");
}

// Moisture follows live terrain and supports, never a cached plant pointer.
var moisture = new Board();
var seaPot = new Plant { mBoard = moisture, mPlantCol = 2, mRow = 1,
    mSeedType = FusionPlant.SeaPot.NativeType, mImitaterType = FusionPlant.SeaPot.Marker };
var recipient = new Plant { mBoard = moisture, mPlantCol = 3, mRow = 1,
    mSeedType = FusionPlant.GrappleKelp.NativeType, mImitaterType = FusionPlant.GrappleKelp.Marker };
moisture.m_plants.Add(seaPot); moisture.m_plants.Add(recipient);
Check(!SeaPot.Wet(seaPot) && !SeaPot.Boosted(recipient), "dry pots grant no speed");
moisture.WaterCells.Add((2, 2));
Check(SeaPot.Wet(seaPot) && SeaPot.Boosted(recipient), "nearby water moistens adjacent recipients");
int steps = 0;
for (int i = 0; i < 100; ++i) { moisture.mMainCounter = i; steps += SeaPot.Steps(recipient); }
Check(steps == 150, "one hundred simulation ticks yield one hundred fifty ability ticks");
moisture.m_plants.Add(new Plant { mBoard = moisture, mPlantCol = 3, mRow = 1,
    mSeedType = FusionPlant.SeaPot.NativeType, mImitaterType = FusionPlant.SeaPot.Marker });
Check(SeaPot.Steps(recipient) == 1, "overlapping wet pots do not stack on odd ticks");
recipient.mPlantCol = 5;
Check(!SeaPot.Boosted(recipient), "moving recipient outside both areas removes bonus");
recipient.mPlantCol = 3; moisture.m_plants.RemoveAt(2); seaPot.mDead = true;
Check(!SeaPot.Boosted(recipient), "dead pot cannot retain moisture bonus");
seaPot.mDead = false; moisture.mPaused = true;
Check(!SeaPot.Boosted(recipient), "paused board does not advance bonus clocks");
Check(SeaPot.Wet(seaPot), "pause retains the moisture indicator");
