using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool condition, string reason) { ++checks; if (!condition) throw new Exception(reason); }
Plant Make(FusionPlant identity, Board? board = null)
{
    var plant = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker, mBoard = board ?? new() };
    ShooterGrowth.Initialize(plant);
    plant.mBoard.m_plants.Add(plant);
    return plant;
}
Projectile Fire(Plant plant)
{
    PoisonFireScope.Prefix(plant, out var previous);
    try { var projectile = new Projectile { mBoard = plant.mBoard }; PoisonProjectileCreatedPatch.Postfix(projectile); return projectile; }
    finally { PoisonFireScope.Finalizer(previous); }
}
Zombie Enemy(Board board, float x = 0, int row = 0)
{
    var zombie = new Zombie { mBoard = board, X = x, mRow = row };
    board.m_zombies.Add(zombie);
    return zombie;
}
void Tick(Board board, int ticks = 1)
{
    for (int i = 0; i < ticks; ++i) { ++board.mMainCounter; PoisonShooters.Update(board); }
}
var recipes = new (FusionPlant A, FusionPlant B, FusionPlant Result)[] {
    (SeedType.Garlic, SeedType.Peashooter, FusionPlant.PoisonPea),
    (SeedType.Garlic, SeedType.Repeater, FusionPlant.PoisonRepeater),
    (SeedType.Peashooter, FusionPlant.PoisonPea, FusionPlant.PoisonRepeater),
    (SeedType.Garlic, SeedType.Splitpea, FusionPlant.SplitPoison),
    (SeedType.Peashooter, FusionPlant.PoisonRepeater, FusionPlant.SplitPoison),
    (FusionPlant.PoisonPea, SeedType.Repeater, FusionPlant.SplitPoison),
    (SeedType.Garlic, SeedType.Gatlingpea, FusionPlant.GatlingPoison),
    (SeedType.Peashooter, FusionPlant.SplitPoison, FusionPlant.GatlingPoison),
    (FusionPlant.PoisonPea, SeedType.Splitpea, FusionPlant.GatlingPoison),
    (FusionPlant.PoisonRepeater, SeedType.Repeater, FusionPlant.GatlingPoison) };
foreach (var (a, b, result) in recipes)
{
    Check(FusionRecipes.TryGet(a, b, out var actual) && actual == result, "source recipe");
    Check(FusionRecipes.TryGet(b, a, out actual) && actual == result, "reversed recipe");
}
foreach (var identity in new[] { FusionPlant.PoisonPea, FusionPlant.PoisonRepeater, FusionPlant.SplitPoison, FusionPlant.GatlingPoison })
{
    var plant = Make(identity);
    Check(FusionPlant.From(plant) == identity && FusionPlant.HasImportedVisuals(identity), "saved identity/art");
    var shot = Fire(plant);
    Check(PoisonShooters.Contains(shot) && !PoisonTorchwoodPatch.Prefix(shot), "poison origin/ordinary Torchwood exclusion");
    var zombie = Enemy(plant.mBoard);
    shot.DoImpact(zombie);
    Check(zombie.Damage == 30 && PoisonShooters.Level(zombie) == 1, "direct hit then one poison level");
    Check(PoisonShooters.CurrentImpact == null, "impact context restored");
    shot.DataID++;
    Check(!PoisonShooters.Contains(shot), "pooled projectile rejects stale origin");
}
PoisonShooters.Clear();
var single = Make(FusionPlant.PoisonPea);
var board = single.mBoard;
var target = Enemy(board);
var bullet = Fire(single);
for (int i = 0; i < 5; ++i) bullet.DoImpact(target);
Check(PoisonShooters.Level(target) == 5 && !PoisonShooters.IsPoisoned(target), "five levels without premature poison");
bullet.DoImpact(target);
Check(PoisonShooters.IsPoisoned(target) && PoisonShooters.CloudCount(board) == 1, "sixth hit starts poison/cloud");
int direct = target.Damage;
Tick(board);
Check(target.Damage == direct + 120, "first poison tick bypasses shields");
int damage = target.Damage;
PoisonShooters.Update(board);
Check(target.Damage == damage, "duplicate board tick ignored");
board.mPaused = true;
Tick(board, 50);
Check(target.Damage == damage, "pause causes no damage");
// Native simulation does not advance mMainCounter while paused.
board.mMainCounter -= 50;
board.mPaused = false;
target.X = 1000;
Tick(board, 99);
Check(target.Damage == damage, "one-second damage interval");
Tick(board);
Check(target.Damage == damage + 120, "next poison tick");
target.mIceTrapCounter = 100;
Tick(board, 500);
Check(PoisonShooters.IsPoisoned(target), "freeze suspends poison duration");
target.mIceTrapCounter = 0;
Tick(board, 399);
Check(!PoisonShooters.IsPoisoned(target) && PoisonShooters.Level(target) == 0, "expiry resets levels");
Check(PoisonShooters.CloudCount(board) == 0, "six-second cloud expires");

PoisonShooters.Clear();
single = Make(FusionPlant.PoisonPea);
board = single.mBoard;
target = Enemy(board);
bullet = Fire(single);
for (int i = 0; i < 7; ++i) bullet.DoImpact(target);
target.X = 1000;
Tick(board, 501);
Check(PoisonShooters.IsPoisoned(target), "further hits add five seconds");
Tick(board, 499);
Check(!PoisonShooters.IsPoisoned(target), "extended duration expires");

PoisonShooters.Clear();
single = Make(FusionPlant.PoisonPea);
board = single.mBoard;
Make(FusionPlant.PoisonRepeater, board);
Make(FusionPlant.SplitPoison, board);
var gatling = Make(FusionPlant.GatlingPoison, board);
Make(SeedType.Peashooter, board);
Check(PoisonShooters.RowDamage(board, 0) == 200, "all four family weights 1+2+3+4");
gatling.OffGround = true;
Check(PoisonShooters.RowDamage(board, 0) == 120, "carried shooters excluded");
gatling.OffGround = false;
target = Enemy(board);
bullet = Fire(single);
for (int i = 0; i < 6; ++i) bullet.DoImpact(target);
var nearby = Enemy(board, 50);
var outside = Enemy(board, 71);
var otherRow = Enemy(board, 5, 1);
Tick(board, 100);
Check(nearby.Damage == 200 && PoisonShooters.Level(nearby) == 1, "cloud radius, family damage, one level");
Check(outside.Damage == 0 && otherRow.Damage == 0, "outside radius/other row excluded");
for (int i = 0; i < 6; ++i) bullet.DoImpact(nearby);
Check(PoisonShooters.CloudCount(board) == 1, "one cloud per row");
Tick(board, 501);
Check(PoisonShooters.CloudCount(board) == 0, "later hits do not refresh cloud lifetime");

PoisonShooters.Clear();
single = Make(FusionPlant.PoisonPea);
board = single.mBoard;
target = Enemy(board);
bullet = Fire(single);
for (int i = 0; i < 6; ++i) bullet.DoImpact(target);
target.DataID++;
Tick(board);
Check(PoisonShooters.Level(target) == 0 && !PoisonShooters.IsPoisoned(target), "pooled zombie cannot inherit poison");
target.mMindControlled = true;
Check(PoisonShooters.BeginImpact(bullet, target) == null, "ally excluded");
target.mMindControlled = false;
var boss = Enemy(board); boss.mZombieType = ZombieType.Boss;
bullet.DoImpact(boss);
Check(boss.Damage == 30 && PoisonShooters.Level(boss) == 0, "boss retains impact damage/status exclusion");
var foreign = Enemy(new Board { Pointer = (IntPtr)70 });
Check(PoisonShooters.BeginImpact(bullet, foreign) == null, "foreign board excluded");
PoisonImpactScope.Prefix(bullet, target, out var outer);
try
{
    var unrelated = new Projectile { mBoard = board };
    PoisonImpactScope.Prefix(unrelated, target, out var nested);
    try { Check(PoisonShooters.CurrentImpact == null, "unrelated nested impact clears context"); }
    finally { PoisonImpactScope.Finalizer(nested); }
    int hit = 20; PoisonDamagePatch.Prefix(target, ref hit);
    Check(hit == 30, "outer impact restored");
    hit = 90; PoisonDamagePatch.Prefix(target, ref hit);
    Check(hit == 90, "only direct damage replaced");
}
finally { PoisonImpactScope.Finalizer(outer); }
PoisonShooters.ForgetBoard(board);
Check(PoisonShooters.CloudCount(board) == 0 && !PoisonShooters.Contains(bullet), "board cleanup");
for (int i = 0; i < 6; ++i) PoisonShooters.AddLevel(board, target);
board.mMainCounter = 0; PoisonShooters.Update(board);
Check(PoisonShooters.Level(target) == 0, "counter rewind clears stale board state");

PoisonShooters.Clear();
single = Make(FusionPlant.PoisonPea);
board = single.mBoard;
target = Enemy(board);
bullet = Fire(single);
for (int i = 0; i < 6; ++i) bullet.DoImpact(target);
// Let the first cloud expire while poison lasts longer under freeze.
target.mIceTrapCounter = 1;
Tick(board, 601);
target.Health = 10;
bullet.DoImpact(target);
Check(PoisonShooters.CloudCount(board) == 1, "lethal hit on poisoned enemy still creates cloud");
target.mDead = true;
PoisonZombieDeathCleanupPatch.Postfix(target);
Check(PoisonShooters.Level(target) == 0, "death clears poison immediately");
var natural = Fire(Make(SeedType.Peashooter));
Check(!PoisonShooters.Contains(natural) && PoisonTorchwoodPatch.Prefix(natural), "native projectile unchanged");

PoisonShooters.Clear();
single = Make(FusionPlant.PoisonPea);
board = single.mBoard;
var first = Enemy(board);
var reused = Enemy(board);
for (int i = 0; i < 6; ++i) { PoisonShooters.AddLevel(board, first); PoisonShooters.AddLevel(board, reused); }
first.OnDamage = () => { reused.DataID++; PoisonShooters.AddLevel(board, reused); };
Tick(board);
Check(PoisonShooters.Level(reused) == 1, "damage callback pool reuse preserves the new occupant's state");

foreach (var identity in new[] { FusionPlant.PoisonRepeater, FusionPlant.SplitPoison })
{
    var plant = Make(identity);
    Check(plant.mStartRow == 1500 && !ShooterGrowth.Grow(plant, 1, 0), "saved growth timer/early click");
    for (int i = 0; i < 1500; ++i) ShooterGrowth.Tick(plant);
    plant.mPlantHealth = 121;
    Check(ShooterGrowth.Grow(plant, 1, 0) && plant.mDead, "ready click consumes original after creation");
    var result = plant.mBoard.Created!;
    Check(FusionPlant.From(result) == (identity == FusionPlant.PoisonRepeater ? FusionPlant.SplitPoison : FusionPlant.GatlingPoison), "correct growth");
    Check(result.mPlantHealth == 121, "growth retains damage");
}

PoisonShooters.Clear();
board = new Board(); target = Enemy(board);
PoisonShooters.PoisonBite(board, target);
Check(PoisonShooters.Level(target) == 1 && PoisonShooters.IsPoisoned(target), "foul bite immediately poisons at one level");
Tick(board);
Check(target.Damage == 20 && PoisonShooters.CloudCount(board) == 0, "foul poison ticks without a shooter or a miasma");
for (int i = 0; i < 35; ++i) PoisonShooters.PoisonBite(board, target);
Check(PoisonShooters.Level(target) == 30, "SetPoison source level cap is 30");
PoisonShooters.Clear();
board = new Board(); target = Enemy(board);
for (int i = 0; i < 6; ++i) PoisonShooters.AddLevel(board, target);
Check(PoisonShooters.Level(target) == 6, "sixth shooter hit also increments the poison level");
PoisonShooters.PoisonBite(board, target);
Check(PoisonShooters.Level(target) == 7, "foul bite shares existing shooter poison levels");
var savedPoison = PoisonShooters.Capture(target)!.Value;
PoisonShooters.Forget(target);
PoisonShooters.Restore(target, savedPoison.Level, savedPoison.Remaining, savedPoison.Countdown);
Check(PoisonShooters.Capture(target) == savedPoison, "saved poison restores exact tick phase and level");
PoisonShooters.Restore(target, 1, 1, 1);
Check(PoisonShooters.Capture(target) == savedPoison, "restore never overwrites a live status");
PoisonShooters.Forget(target);
PoisonShooters.Restore(target, 90, -10, 900);
Check(PoisonShooters.Capture(target) == (30, 0, 100), "saved poison validates serialized bounds");
Console.WriteLine($"Poison shooters: {checks} assertions passed; no game executed.");

var savedGarlic = new Projectile { mCobTargetX = PoisonShooters.GarlicTorchMarker };
Check(PoisonShooters.Contains(savedGarlic), "saved garlic conversion needs no transient shot cache");
savedGarlic.mCobTargetX = PoisonShooters.ToxicTorchMarker;
Check(!PoisonShooters.Contains(savedGarlic), "toxic fire projectile has its own impact owner");
