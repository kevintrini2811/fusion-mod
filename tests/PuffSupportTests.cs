using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool ok, string message) { ++checks; if (!ok) throw new Exception(message); }
Plant Make(FusionPlant identity)
{
    var plant = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    PuffSupport.Initialize(plant);
    return plant;
}
Zombie Enemy(Plant plant, float x = 240)
{
    var zombie = new Zombie { mBoard = plant.mBoard, X = x, mRow = plant.mRow };
    plant.mBoard.m_zombies.Add(zombie);
    return zombie;
}
var recipes = new[] {
    (FusionPlant.GnomeShroom, SeedType.Hypnoshroom),
    (FusionPlant.UmbrellaCap, SeedType.Umbrella),
    (FusionPlant.NutShroom, SeedType.Wallnut), (FusionPlant.BellPepper, SeedType.Jalapeno),
    (FusionPlant.BoomShroom, SeedType.Doomshroom), (FusionPlant.PennyShroom, SeedType.Marigold) };
foreach (var (identity, ingredient) in recipes)
{
    var plant = Make(identity);
    Check(FusionPlant.From(plant) == identity && FusionPlant.HasImportedVisuals(identity), "Saved identity and original graphics");
    Check(FusionRecipes.TryGet(SeedType.Puffshroom, ingredient, out var result) && result == identity, "Source recipe");
    Check(FusionRecipes.TryGet(ingredient, SeedType.Puffshroom, out result) && result == identity, "Reverse recipe");
    Check(!FusionRecipes.TryGet(FusionPlant.PeaShroom, ingredient, out _), "No invented variant recipe");
    Check(plant.mPlantHealth == (identity == FusionPlant.NutShroom ? 2000 : 300), "Source health");
    Check(!PuffFirePatch.Prefix(plant) && !PuffShootingPatch.Prefix(plant), "No native spores or second counter writer");
    foreach (int inactive in Enumerable.Range(0, 6))
    {
        var frozen = Make(identity);
        Enemy(frozen);
        if (inactive == 0) frozen.mBoard.mPaused = true;
        if (inactive == 1) frozen.mIsAsleep = true;
        if (inactive == 2) frozen.mDead = true;
        if (inactive == 3) frozen.mIsOnBoard = false;
        if (inactive == 4) frozen.mSquished = true;
        if (inactive == 5) frozen.Bungee = true;
        int maturity = frozen.mTargetX;
        PuffSupport.Tick(frozen);
        Check(frozen.mTargetX == maturity && frozen.mStartRow == 0 && frozen.mShootingCounter == 0,
            "Inactive ability keeps counters");
    }
}
var nut = Make(FusionPlant.NutShroom);
Enemy(nut);
for (int i=0;i<1000;++i) PuffShooterPatch.Prefix(nut);
Check(!nut.mDead && nut.mPlantHealth == 2000 && nut.mBoard.Projectiles.Count == 0, "Nut blocks without shooting or exploding");
var bell = Make(FusionPlant.BellPepper);
var target = Enemy(bell, 600);
PuffSupport.Tick(bell);
Check(bell.mStartRow == 0, "Distant zombie does not ignite");
target.X = 240; target.mMindControlled = true;
PuffSupport.Tick(bell);
Check(bell.mStartRow == 0, "Friendly does not ignite");
target.mMindControlled = false; target.Category = DamageRangeFlags.Flying;
PuffSupport.Tick(bell);
Check(bell.mStartRow == 0, "Airborne does not ignite");
target.Category = DamageRangeFlags.Ground; ++target.mRow;
PuffSupport.Tick(bell);
Check(bell.mStartRow == 0, "Other row does not ignite");
--target.mRow;
PuffSupport.Tick(bell);
Check(bell.mStartRow == 100 && !bell.mDead, "Proximity starts original one-second clip");
var waitingBell = Make(FusionPlant.BellPepper);
waitingBell.mBoard = bell.mBoard;
bell.mBoard.m_plants.Add(bell); bell.mBoard.m_plants.Add(waitingBell);
PuffSupport.Tick(waitingBell);
Check(waitingBell.mStartRow == 0, "A second pepper waits while the tile has an active fuse");
var loadedBell=Make(FusionPlant.BellPepper);
loadedBell.mStartRow=bell.mStartRow;
DoomEffects.Calls.Clear();
for(int i=0;i<99;++i) { PuffSupport.Tick(bell); PuffSupport.Tick(loadedBell); }
Check(!bell.mDead && !loadedBell.mDead && DoomEffects.Calls.Count==0, "Fuse does not fire early; no enemy required after trigger");
PuffSupport.Tick(bell); PuffSupport.Tick(loadedBell);
Check(bell.mDead && loadedBell.mDead && DoomEffects.Calls.Count==2, "Saved fuse completes once");
Check(DoomEffects.Calls.All(c=>c.Row==2 && c.Fire && c.Damage==600), "Shared fire-line adapter receives original damage and row");
PuffSupport.Tick(bell);
Check(DoomEffects.Calls.Count==2, "Dead bell cannot repeat");
var crushed=Make(FusionPlant.BellPepper);
Check(!BellPepperCrushPatch.Prefix(crushed) && crushed.mDead && DoomEffects.Calls.Count==3, "OnCrash ignites immediately");
var eaten=Make(FusionPlant.BellPepper); eaten.Die();
Check(DoomEffects.Calls.Count==3, "Ordinary bell death has no invented explosion");
var boom=Make(FusionPlant.BoomShroom);
var close=Enemy(boom,280);
PuffSupport.Tick(boom);
Check(!boom.mDead, "Boom requires collider contact, not wide blast proximity");
close.X=240;
var neighbor=Enemy(boom,300);
var outside=Enemy(boom,450);
var friendly=Enemy(boom); friendly.mMindControlled=true;
var air=Enemy(boom); air.Category=DamageRangeFlags.Flying;
var otherRow=Enemy(boom); ++otherRow.mRow;
PuffSupport.Tick(boom);
Check(boom.mDead && close.Damage==1800 && neighbor.Damage==1800, "Contact explodes once per nearby enemy, no count multiplier");
Check(outside.Damage==0 && friendly.Damage==0 && air.Damage==0 && otherRow.Damage==0, "Blast target boundaries");
boom.Die();
Check(close.Damage==1800 && neighbor.Damage==1800, "Repeated death does not repeat blast");
var dying=Make(FusionPlant.BoomShroom); var victim=Enemy(dying);
dying.Die(); Check(victim.Damage==1800, "Ordinary death also explodes");
var consumed=Make(FusionPlant.BoomShroom); var spared=Enemy(consumed);
PlayerPlantingScope.ConsumingIngredient=consumed.Pointer; consumed.Die(); PlayerPlantingScope.ConsumingIngredient=IntPtr.Zero;
Check(spared.Damage==0, "Fusion consumption and rollback suppress death effect");
var disposing=Make(FusionPlant.BoomShroom); var unhurt=Enemy(disposing);
CherryBombPlants.DisposingBoard=disposing.mBoard.Pointer; disposing.Die(); CherryBombPlants.DisposingBoard=IntPtr.Zero;
Check(unhurt.Damage==0, "Board teardown suppresses blast");
var recycled=Make(FusionPlant.BoomShroom); var replacement=Enemy(recycled);
recycled.OnDeath=()=>++recycled.DataID; recycled.Die();
Check(replacement.Damage==0, "Pool reuse during death invalidates snapshot");
var penn=Make(FusionPlant.PennyShroom);
PuffShooterPatch.Prefix(penn);
Check(penn.mShootingCounter==360 && penn.mLaunchCounter==2500, "Producer starts source brightening and 25-second base interval");
for(int i=0;i<49;++i) PuffSupport.Tick(penn);
Check(penn.mBoard.Coins.Count==0, "No premature payout");
PuffSupport.Tick(penn);
Check(penn.mBoard.Coins.Count==2 && penn.mBoard.Coins.All(c=>c.Type==CoinType.Gold), "100 money in ordinary saved gold pickups");
Check(Math.Abs(PuffSupport.Brightness(penn)-1.5f)<.001, "Original brightening progression");
// Loading copies only saved native fields, without running PlantInitialize again.
var saved=Make(FusionPlant.PennyShroom);
penn.mTargetX=1; penn.mShootingCounter=311;
saved.mTargetX=penn.mTargetX; saved.mShootingCounter=penn.mShootingCounter; saved.mLaunchCounter=penn.mLaunchCounter;
penn.mBoard.Coins.Clear();
PuffSupport.Tick(penn); PuffSupport.Tick(saved);
Check(penn.mTargetX==0 && penn.mTargetY==50 && saved.mTargetY==50, "120-second growth starts saved half-second clip");
Check(penn.mBoard.Coins.Count==6 && saved.mBoard.Coins.Count==6, "Same-tick maturity pays 300; saved countdown agrees");
for(int i=0;i<50;++i) PuffSupport.Tick(penn);
Check(penn.mTargetY==0 && penn.mTargetX==0, "Mature idle follows original growth clip");
var age=Make(FusionPlant.PennyShroom);
for(int i=0;i<11999;++i) PuffSupport.Tick(age);
Check(age.mTargetX==1, "Growth does not happen before 120 seconds");
PuffSupport.Tick(age);
Check(age.mTargetX==0, "Growth at exactly 120 seconds");
var native=new Plant {mSeedType=SeedType.Puffshroom,mPlantHealth=7};
PuffSupport.Initialize(native);
Check(native.mPlantHealth==7 && PuffFirePatch.Prefix(native) && BellPepperCrushPatch.Prefix(native), "Native Puff unchanged");
Check(FusionRecipes.TryGet(SeedType.Puffshroom, SeedType.Sunflower, out var sunResult)
    && sunResult == (FusionPlant)SeedType.Sunshroom && !FusionPlant.HasImportedVisuals(sunResult)
    && !FusionPlant.IsPuffSupport(sunResult), "Sun-shroom recipe uses native lifecycle and artwork");
Check(FusionRecipes.TryGet(SeedType.Sunflower, SeedType.Puffshroom, out sunResult)
    && sunResult == (FusionPlant)SeedType.Sunshroom, "Reverse native Sun-shroom recipe");
foreach (float scale in new[] { 1f, .8f, .5f })
{
    var gnome = Make(FusionPlant.GnomeShroom);
    var eater = Enemy(gnome);
    eater.mScaleZombie = scale;
    eater.mBodyHealth = 201; eater.mBodyMaxHealth = 270;
    eater.mHelmHealth = 401; eater.mHelmMaxHealth = 1100;
    eater.mShieldHealth = 101; eater.mShieldMaxHealth = 440;
    eater.mFlyingHealth = 5; eater.mFlyingMaxHealth = 20;
    Check(!PuffSupport.IsSmall(eater), "Native scale is not a shrink marker");
    Check(!GnomeShroomBitePatch.Prefix(eater, gnome, true), "Gnome bite replaces ordinary plant damage");
    Check(gnome.mDead && eater.mMindControlled, "One bite consumes gnome and hypnotizes eater");
    Check(PuffSupport.IsSmall(eater) && Math.Abs(eater.mScaleZombie - scale * .7f) < .00001f, "Source scale multiplier");
    Check(eater.mBodyHealth == 140 && eater.mBodyMaxHealth == 189 && eater.mHelmHealth == 280
        && eater.mHelmMaxHealth == 770 && eater.mShieldHealth == 70 && eater.mShieldMaxHealth == 308,
        "All six health fields shrink separately with source truncation");
    Check(eater.mFlyingHealth == 3 && eater.mFlyingMaxHealth == 14,
        "Balloon armor uses its native separate health pool and source truncation");
    var restored = new Zombie { mScaleZombie = eater.mScaleZombie, mBodyHealth = eater.mBodyHealth };
    PuffSupport.Shrink(restored);
    Check(restored.mBodyHealth == 140 && restored.mScaleZombie == eater.mScaleZombie, "Saved scale prevents repeated shrink");
}
var sleepGnome = Make(FusionPlant.GnomeShroom); sleepGnome.mIsAsleep = true;
var sleeperEater = Enemy(sleepGnome);
Check(GnomeShroomBitePatch.Prefix(sleeperEater, sleepGnome, true) && !sleeperEater.mMindControlled,
    "Sleeping gnome retains ordinary native bite");
sleepGnome.mIsAsleep = false;
Check(GnomeShroomBitePatch.Prefix(sleeperEater, sleepGnome, false) && !sleepGnome.mDead,
    "Prior bite rejection cannot trigger gnome");
var ordinary = new Plant { mSeedType = SeedType.Puffshroom };
Check(GnomeShroomBitePatch.Prefix(Enemy(ordinary), ordinary, true), "Ordinary Puff bite unchanged");
foreach (bool small in new[] { false, true })
{
    var cap = Make(FusionPlant.UmbrellaCap);
    var foe = Enemy(cap); foe.mPosX = 240; foe.mScaleZombie = small ? .7f : 1;
    var friendlyCapTarget = Enemy(cap); friendlyCapTarget.mMindControlled = true; friendlyCapTarget.mPosX = 240;
    PuffSupport.Tick(cap);
    Check(cap.mStartRow == 50 && !cap.mDead, "Contact starts original block clip");
    for (int i = 0; i < 16; ++i) PuffSupport.Tick(cap);
    Check(foe.mPosX == 240 && !cap.mDead, "No pulse before source animation event");
    var loadedCap = Make(FusionPlant.UmbrellaCap); loadedCap.mStartRow = cap.mStartRow;
    var loadedFoe = Enemy(loadedCap); loadedFoe.mPosX = 240; loadedFoe.mScaleZombie = foe.mScaleZombie;
    PuffSupport.Tick(cap); PuffSupport.Tick(loadedCap);
    Check(Math.Abs(foe.mPosX - (240 + 2 * ScaredyFusions.SourceUnit)) < .001 && loadedFoe.mPosX == foe.mPosX,
        "Saved block event repels by two source units");
    Check(foe.Damage == 0 && friendlyCapTarget.mPosX == 240, "Pulse has no damage and excludes friendlies");
    Check(cap.mDead == !small && loadedCap.mDead == !small && cap.mPlantHealth == (small ? 250 : 300),
        "Large target consumes cap; small target costs only 50 HP");
}
var mixedCap = Make(FusionPlant.UmbrellaCap);
Enemy(mixedCap).mScaleZombie = .7f; Enemy(mixedCap);
PuffSupport.Tick(mixedCap);
for (int i = 0; i < 17; ++i) PuffSupport.Tick(mixedCap);
Check(mixedCap.mDead, "Any large pulse target consumes cap even among small targets");
var vacantCap = Make(FusionPlant.UmbrellaCap); vacantCap.mStartRow = 34;
PuffSupport.Tick(vacantCap);
Check(vacantCap.mPlantHealth == 250 && !vacantCap.mDead, "Actual AnimBlock charges 50 when no targets remain");
var parentCap = Make(FusionPlant.UmbrellaCap);
parentCap.mTargetX = 1;
PuffSupport.Tick(parentCap);
Check(parentCap.mBoard.m_plants.Single().mTargetX == 3000 && parentCap.mTargetX == 3000,
    "Reproduction gives parent and offspring independent fresh 30-second timers");
parentCap.mBoard.SpawnFails = true; parentCap.mTargetX = 1;
PuffSupport.Tick(parentCap);
Check(parentCap.mTargetX == 3000 && parentCap.mBoard.m_plants.Count == 1, "Full tile rearms reproduction for a later retry");
Check(recipes.All(r => Make(r.Item1).mBoard.Projectiles.Count == 0), "No invented projectiles");
var supportedCap=Make(FusionPlant.UmbrellaCap);
var capSupport=new Plant {mBoard=supportedCap.mBoard,mPlantCol=supportedCap.mPlantCol,mRow=supportedCap.mRow,
    mSeedType=SeedType.Flowerpot,mImitaterType=SeedType.Umbrella};
supportedCap.mBoard.m_plants.Add(capSupport);
var supportedFoe=Enemy(supportedCap); supportedFoe.mScaleZombie=.7f; supportedFoe.mPosX=240;
supportedCap.mStartRow=34;
PuffSupport.Tick(supportedCap);
Check(Math.Abs(supportedFoe.mPosX-(240+3*ScaredyFusions.SourceUnit))<.01f&&!supportedCap.mDead&&supportedCap.mPlantHealth==250,
    "Umbrella Pot boosts small umbrella to three units without changing health cost");

Console.WriteLine($"{checks} Puff support checks passed; no game code loaded.");
