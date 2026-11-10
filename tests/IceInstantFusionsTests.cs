using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool ok, string message) { ++checks; if (!ok) throw new Exception(message); }
Plant Make(FusionPlant identity, Board? board = null)
{
    var plant = new Plant { mBoard = board ?? new Board(), mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    plant.mBoard.m_plants.Add(plant);
    IceInstantFusions.Initialize(plant);
    return plant;
}
Zombie Target(Board board, int row = 2, int health = 10000)
{
    var zombie = new Zombie { mBoard = board, mRow = row, Health = health };
    board.m_zombies.Add(zombie); return zombie;
}
Projectile Cherry(Board board, bool explosive = false)
{
    var producer = new Plant { mBoard = board, mSeedType = SeedType.Peashooter,
        mImitaterType = explosive ? SeedType.ExplodeONut : SeedType.Cherrybomb };
    CherryShooter.FiringPlant = producer;
    var shot = new Projectile { mBoard = board, mProjectileType = ProjectileType.Pea, mPosX = 242 };
    CherryShooter.Track(shot); CherryShooter.FiringPlant = null;
    return shot;
}
foreach (var (a,b,result) in new[] {
    (SeedType.Cherrybomb,SeedType.Doomshroom,FusionPlant.BlackCherry),
    (SeedType.Cherrybomb,SeedType.Iceshroom,FusionPlant.FrozenCherry),
    (SeedType.Doomshroom,SeedType.Iceshroom,FusionPlant.DemiseShroom),
    (SeedType.Iceshroom,SeedType.Sunflower,FusionPlant.SundaeShroom) })
{
    Check(FusionRecipes.TryGet(a,b,out var found) && found == result, "forward recipe");
    Check(FusionRecipes.TryGet(b,a,out found) && found == result, "reverse recipe");
    Check(FusionPlant.From(Make(result)) == result, "saved identity");
}

var black = Make(FusionPlant.BlackCherry);
var survivor = Target(black.mBoard); var killed = Target(black.mBoard, health: 50);
var far = Target(black.mBoard); far.Rect = new(700,210,720,270);
IceInstantFusions.OnFusion(black);
Check(black.mTargetX == 2 && survivor.Damage == 1800 && killed.Damage == 1800 && far.Damage == 0,
    "initial cherry counts damaged survivors and kills, excluding distant targets");
black.mBoard.mPaused = true; IceInstantFusions.Tick(black);
Check(black.mLaunchCounter == 150, "pause retains fuse");
black.mBoard.mPaused = false;
for (int i=0;i<149;++i) IceInstantFusions.Tick(black);
Check(!black.mDead && survivor.Damage == 1800, "no premature doom blast");
IceInstantFusions.Tick(black);
Check(black.mDead && survivor.Damage == 4200, "one scaled doom blast: 1800 + 2*300");
Check(black.mBoard.m_gridItems.Count == 1, "black cherry leaves one crater");
IceInstantFusions.Tick(black); Check(survivor.Damage == 4200, "no replay after death");
var early = Make(FusionPlant.BlackCherry); var earlyTarget = Target(early.mBoard);
early.mTargetX = 3; BlackCherryCrushPatch.Prefix(early);
Check(earlyTarget.Damage == 2700, "crushing detonates boosted doom once");
var removed=Make(FusionPlant.BlackCherry);var removedTarget=Target(removed.mBoard);removed.Die();
Check(removedTarget.Damage==0,"ordinary removal cannot trigger an extra doom event");

var frozen = Make(FusionPlant.FrozenCherry);
var normal = new Projectile { mBoard=frozen.mBoard,mProjectileType=ProjectileType.Pea,mPosX=242 };
Check(!IceInstantFusions.Absorb(normal) && !normal.mDead, "ordinary peas are not absorbed");
var elevated = Cherry(frozen.mBoard); elevated.mPosZ = -10;
Check(!IceInstantFusions.Absorb(elevated), "elevated cherry rejected");
var otherRow = Cherry(frozen.mBoard); otherRow.mRow=1;
Check(!IceInstantFusions.Absorb(otherRow), "different row rejected");
var fire = Cherry(frozen.mBoard); fire.mProjectileType = ProjectileType.Fireball;
Check(IceInstantFusions.Absorb(fire) && frozen.mTargetX == 2, "fire cherry contributes two");
var ordinary = Cherry(frozen.mBoard);
Check(IceInstantFusions.Absorb(ordinary) && frozen.mTargetX == 3, "pink cherry contributes one");
var saved = Cherry(frozen.mBoard,true);
var loaded = new Projectile {mBoard=frozen.mBoard,mProjectileType=ProjectileType.Pea,mPosX=242,mCobTargetX=saved.mCobTargetX};
Check(CherryShooter.IsExplosive(loaded), "serialized marker reconstructs explosive provenance");
frozen.mTargetX=95;
Check(IceInstantFusions.Absorb(loaded) && frozen.mTargetX == 0 && frozen.mBoard.Coins.Count == 1,
    "100 energy creates exactly one card and discards overflow");
Check(frozen.mBoard.Coins[0].mUsableSeedType == SeedType.Cherrybomb
    && frozen.mBoard.Coins[0].mController.SeedType == SeedType.Cherrybomb, "card and controller identify cherry bomb");
Check(!IceInstantFusions.Absorb(loaded) && frozen.mBoard.Coins.Count == 1, "dead projectile cannot credit twice");
var cold = Target(frozen.mBoard); frozen.Die();
Check(cold.Damage == 1800 && cold.mIceTrapCounter == 1000, "frozen cherry death blast and ten-second freeze");
var immune = Target(new Board()); immune.Immune=true; IceInstantFusions.Freeze(immune,1000);
Check(immune.mIceTrapCounter == 0, "native freeze immunity");
var longer = Target(new Board()); longer.mIceTrapCounter=1500; IceInstantFusions.Freeze(longer,500);
Check(longer.mIceTrapCounter == 1500, "longer existing freeze preserved");

var demise = Make(FusionPlant.DemiseShroom); var distant = Target(demise.mBoard,4);
distant.Rect=new(730,410,750,470);
for(int i=0;i<96;++i) IceInstantFusions.Tick(demise);
Check(distant.Damage==1800 && distant.mIceTrapCounter==500, "demise hits and freezes globally");
var crater=demise.mBoard.m_gridItems.Single();
Check(IceInstantFusions.LevelFrozen(demise.mBoard) && crater.mSunCount==1000, "ten-second spawn gate stored in crater");
demise.mBoard.mPaused=true; IceInstantFusions.TickLevelFreeze(demise.mBoard);
Check(crater.mSunCount==1000,"pause preserves spawn freeze");
demise.mBoard.mPaused=false;
for(int i=0;i<1000;++i) IceInstantFusions.TickLevelFreeze(demise.mBoard);
Check(!IceInstantFusions.LevelFrozen(demise.mBoard),"spawning resumes after gate expires");

var sundae=Make(FusionPlant.SundaeShroom);var icy=Target(sundae.mBoard);icy.mIceTrapCounter=400;
SolarShooter.Suns.Clear(); sundae.mLaunchCounter=1;
IceInstantFusions.Tick(sundae);
Check(sundae.mTargetY==360 && sundae.mBoard.Coins.Count==0,"source brightening precedes production");
for(int i=0;i<300;++i) IceInstantFusions.Tick(sundae);
Check(sundae.mStartRow==1 && sundae.mBoard.Coins.Count==1 && SolarShooter.Suns.SequenceEqual(new[]{15}),
    "first production drops 25 and 15 per previously frozen zombie");
Check(icy.Damage==20 && icy.mIceTrapCounter==400 && icy.mChilledCounter>0,"first stage chills without new freeze");
for(int stage=2;stage<=3;++stage)
{
    sundae.mTargetY=61; IceInstantFusions.Tick(sundae);
    Check(sundae.mStartRow==stage,"saved production phase advances once");
}
Check(sundae.mDead && icy.Damage==60 && icy.mIceTrapCounter==400 && sundae.mBoard.Coins.Count==3,
    "third production freezes for four seconds and removes sundae");
IceInstantFusions.Tick(sundae); Check(sundae.mBoard.Coins.Count==3,"dead producer emits no extra sun");
// Silver plants: persistent identity, terminal activation, immunities and pause.
foreach (var (ingredient, identity, health) in new[] {
    (SeedType.Wallnut, FusionPlant.SilverNut, 6000),
    (SeedType.Garlic, FusionPlant.SilverGarlic, 600),
    (SeedType.Iceshroom, FusionPlant.SilverIceShroom, 300),
    (SeedType.Doomshroom, FusionPlant.SilverDoomShroom, 300),
    (SeedType.Hypnoshroom, FusionPlant.SilverHypnoShroom, 300),
    (SeedType.Cabbagepult, FusionPlant.SilverCabbage, 300),
    (SeedType.Melonpult, FusionPlant.SilverMelon, 300),
    (SeedType.Sunflower, FusionPlant.SilverSunflower, 300) })
{
    Check(FusionRecipes.TryGet(ingredient, SeedType.Marigold, out var silverResult) && silverResult == identity,
        "silver source recipe");
    Check(FusionRecipes.TryGet(SeedType.Marigold, ingredient, out silverResult) && silverResult == identity,
        "silver reverse recipe");
    var silver = Make(identity); SilverPlants.Initialize(silver);
    Check(FusionPlant.From(silver) == identity && FusionPlant.HasImportedVisuals(identity), "silver saved marker and graphics");
    Check(silver.mPlantHealth == health && silver.mPlantMaxHealth == health, "silver source health");
    silver.mImitaterType = SeedType.None;
    Check(FusionPlant.From(silver) == (FusionPlant)ingredient, "ordinary native identity unchanged");
}
var silverIce = Make(FusionPlant.SilverIceShroom); SilverPlants.Initialize(silverIce);
var silverTarget = Target(silverIce.mBoard);
var silverImmune = Target(silverIce.mBoard); silverImmune.Immune = true;
silverIce.mIsAsleep = true; SilverPlants.Tick(silverIce);
Check(silverIce.mLaunchCounter == 150, "sleep preserves silver activation delay");
silverIce.mIsAsleep = false; silverIce.mWakeUpCounter = 2; SilverPlants.Tick(silverIce);
Check(silverIce.mLaunchCounter == 150, "coffee wake delay preserves silver activation delay");
silverIce.mWakeUpCounter = 0; silverIce.mBoard.mPaused = true; SilverPlants.Tick(silverIce);
Check(silverIce.mLaunchCounter == 150, "pause preserves silver activation delay");
silverIce.mBoard.mPaused = false;
for (int i = 0; i < 149; ++i) SilverPlants.Tick(silverIce);
Check(!silverIce.mDead && silverTarget.Damage == 0, "silver waits full activation delay");
SilverPlants.Tick(silverIce);
Check(silverIce.mDead && silverTarget.mIceTrapCounter == 500 && silverTarget.Damage == 20,
    "silver terminal effect freezes for five seconds with one damage application");
Check(silverImmune.mIceTrapCounter == 0 && silverImmune.Damage == 20, "native freeze immunity survives silver effect");
SilverPlants.Tick(silverIce); SilverPlants.Explode(silverIce);
Check(silverTarget.Damage == 20, "silver death cannot replay frost");
var crushedSilver = Make(FusionPlant.SilverIceShroom); SilverPlants.Initialize(crushedSilver);
var crushTarget = Target(crushedSilver.mBoard); SilverPlants.Explode(crushedSilver);
Check(crushedSilver.mDead && crushTarget.mIceTrapCounter == 500, "crushing bypasses remaining fuse");
var stableDoom = Make(FusionPlant.SilverDoomShroom); stableDoom.mDoSpecialCountdown = 100;
SilverPlants.Initialize(stableDoom);
Check(stableDoom.mDoSpecialCountdown == 0, "Silver Doom clears native activation countdown");
for (int i = 0; i < 200; ++i) SilverPlants.Tick(stableDoom);
Check(!stableDoom.mDead && stableDoom.mBoard.m_gridItems.Count == 0,
    "Silver Doom remains stable without an explosion or crater");
Check(!SilverDoomCrushPatch.Prefix(stableDoom, true) && stableDoom.mDead
    && stableDoom.mBoard.m_gridItems.Count == 0, "Crushing stable Doom removes it without a crater");
Console.WriteLine($"Ice instant fusions: {checks} checks passed; no game code loaded.");
