using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool condition, string message) { ++checks; if (!condition) throw new Exception(message); }
Plant Make(FusionPlant identity)
{
    var plant = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    PuffSpecials.Initialize(plant);
    return plant;
}
Zombie Enemy(Plant plant, float x = 300)
{
    var zombie = new Zombie { mBoard = plant.mBoard, X = x, mRow = plant.mRow };
    plant.mBoard.m_zombies.Add(zombie);
    return zombie;
}
var recipes = new[] {
    ((FusionPlant)SeedType.Puffshroom, (FusionPlant)SeedType.Iceshroom, FusionPlant.HailShroom),
    ((FusionPlant)SeedType.Puffshroom, (FusionPlant)SeedType.Snowpea, FusionPlant.IcicleShroom),
    (FusionPlant.HailShroom, (FusionPlant)SeedType.Peashooter, FusionPlant.IcicleShroom),
    (FusionPlant.PeaShroom, (FusionPlant)SeedType.Iceshroom, FusionPlant.IcicleShroom),
    ((FusionPlant)SeedType.Puffshroom, (FusionPlant)SeedType.Starfruit, FusionPlant.TwinkleShroom),
    ((FusionPlant)SeedType.Puffshroom, (FusionPlant)SeedType.Potatomine, FusionPlant.NuggetShroom)
};
foreach (var (left, right, expected) in recipes)
{
    Check(FusionRecipes.TryGet(left, right, out var result) && result == expected, "Complete source recipe");
    Check(FusionRecipes.TryGet(right, left, out result) && result == expected, "Reverse source recipe");
}
foreach (var identity in recipes.Select(r => r.Item3).Distinct())
{
    var plant = Make(identity);
    Check(FusionPlant.From(plant) == identity, "Saved identity reconstructs");
    Check(FusionPlant.HasImportedVisuals(identity), "Original graphics selected");
    Check(plant.mPlantHealth == 300 && plant.mPlantMaxHealth == 300, "Source health");
    Check(!PuffFirePatch.Prefix(plant) && !PuffShootingPatch.Prefix(plant), "No native duplicate spore or counter writer");
    foreach (int inactive in Enumerable.Range(0, 6))
    {
        var paused = Make(identity);
        Enemy(paused, 240);
        if (inactive == 0) paused.mBoard.mPaused = true;
        if (inactive == 1) paused.mIsAsleep = true;
        if (inactive == 2) paused.mDead = true;
        if (inactive == 3) paused.mIsOnBoard = false;
        if (inactive == 4) paused.mSquished = true;
        if (inactive == 5) paused.Bungee = true;
        int timer = paused.mTargetX;
        PuffSpecials.Tick(paused);
        Check(paused.mTargetX == timer && paused.mStartRow == 0 && paused.mBoard.Projectiles.Count == 0,
            "Inactive plant retains saved timers and cannot attack or reproduce");
    }
}
var hail = Make(FusionPlant.HailShroom);
var contact = Enemy(hail, 300);
PuffSpecials.ContactFreeze(hail);
Check(!hail.mDead && contact.mIceTrapCounter == 0, "Hail excludes distant contact");
contact.X = 240; contact.mMindControlled = true;
PuffSpecials.ContactFreeze(hail);
Check(!hail.mDead, "Hail excludes friendly contact");
contact.mMindControlled = false; contact.Category = DamageRangeFlags.Flying;
PuffSpecials.ContactFreeze(hail);
Check(!hail.mDead, "Hail excludes airborne contact");
contact.Category = DamageRangeFlags.Ground; contact.mIceTrapCounter = 400;
PuffSpecials.ContactFreeze(hail);
Check(!hail.mDead, "Hail waits for an unfrozen contact");
contact.mIceTrapCounter = 0;
PuffSpecials.ContactFreeze(hail);
Check(hail.mDead && contact.mIceTrapCounter == 400 && contact.Damage == 0, "Hail freezes once without damage and consumes itself");
Check(hail.mBoard.Projectiles.Count == 0, "Hail creates no projectiles");
var bittenHail = Make(FusionPlant.HailShroom);
var bitingZombie = Enemy(bittenHail, 300);
Check(!HailBiteContactPatch.Prefix(bitingZombie, bittenHail) && bittenHail.mDead
    && bitingZombie.mIceTrapCounter == 400, "Actual native bite contact freezes even before body rectangles overlap the small sprite");


foreach (var identity in new[] { FusionPlant.IcicleShroom, FusionPlant.NuggetShroom })
{
    var plant = Make(identity);
    var target = Enemy(plant, 600);
    Check(PuffSpecials.HasTarget(plant, identity) == (identity == FusionPlant.IcicleShroom), "IcePuff inherits full-row PeaSmallPuff search; Nugget stays short range");
    target.X = 300; ++target.mRow;
    Check(!PuffSpecials.HasTarget(plant, identity), "Short shooter excludes other rows");
    --target.mRow;
    for (int i = 0; i < 38; ++i) PuffSpecials.Tick(plant);
    Check(plant.mBoard.Projectiles.Count == 0 && plant.mStartRow == 12, "Original .3833-second shoot event");
    PuffSpecials.Tick(plant);
    var shot = plant.mBoard.Projectiles.Single();
    Check(identity == FusionPlant.IcicleShroom ? PuffSpecials.IsIcicle(shot) : PuffSpecials.IsNugget(shot), "Saved original projectile family");
    Check(shot.mVelX == PuffShooters.Speed && shot.mShadowY - shot.mPosY == 40, "Source speed above native ground cutoff");
    Check(!PuffSpecialTorchwoodPatch.Prefix(shot), "Source Torchwood excludes both bullet types");
    Check(!RetaliationProjectiles.IsPea(shot), "Radial renderer cannot steal Nugget spores");
    var loaded = Make(identity);
    loaded.mLaunchCounter = plant.mLaunchCounter;
    loaded.mStartRow = plant.mStartRow;
    loaded.mTargetX = plant.mTargetX;
    Enemy(loaded, 300);
    for (int i = 0; i < 11; ++i) { PuffSpecials.Tick(plant); PuffSpecials.Tick(loaded); }
    Check(loaded.mStartRow == plant.mStartRow && loaded.mLaunchCounter == plant.mLaunchCounter
        && loaded.mBoard.Projectiles.Count == 0, "Restored attack phase cannot repeat an emitted shot");
    int interval = identity == FusionPlant.NuggetShroom ? 200 : 225;
    Check(plant.mLaunchRate == interval && plant.mLaunchCounter == interval - 49, "Actual ScriptableObject interval");
}

var star = Make(FusionPlant.TwinkleShroom);
for (int i = 0; i < 100; ++i) PuffSpecials.Tick(star);
Check(star.mBoard.Projectiles.Count == 0, "Twinkle uses native star target geometry");
star.mBoard.StarTarget = true;
for (int i = 0; i < 26; ++i) PuffSpecials.Tick(star);
Check(star.mBoard.Projectiles.Count == 5, "Twinkle emits exactly five stars at .25 seconds");
Check(star.mBoard.Projectiles.All(p => p.mProjectileType == ProjectileType.Star && RetaliationProjectiles.IsStar(p)), "All five use original stars");
Check(star.mBoard.Projectiles.Count(p => p.mVelX < -4) == 1
    && star.mBoard.Projectiles.Count(p => Math.Abs(p.mVelX) < .001f) == 2, "Rear and vertical star rays retained");

var nugget = Make(FusionPlant.NuggetShroom);
Check(nugget.mTargetX == 2000, "Reproduction starts at source twenty seconds with .95-1.05 random range");
for (int i = 0; i < 1999; ++i) PuffSpecials.Tick(nugget);
Check(nugget.mBoard.m_plants.Count == 0, "No premature reproduction");
PuffSpecials.Tick(nugget);
var child = nugget.mBoard.m_plants.Single();
Check(FusionPlant.From(child) == FusionPlant.NuggetShroom && child.mTargetX == 2000,
    "Child has correct saved variant and independent reproduction timer");
Check(nugget.mTargetX == 2000, "Actual source AttributeEvent rearms parent on success");
nugget.mBoard.SpawnFails = true; nugget.mTargetX = 1;
PuffSpecials.Tick(nugget);
Check(nugget.mTargetX == 0 && nugget.mBoard.m_plants.Count == 1, "Failed/full placement stops reproduction as source does");
for (int i = 0; i < 2500; ++i) PuffSpecials.Tick(nugget);
Check(nugget.mBoard.m_plants.Count == 1, "Stopped parent cannot retry or duplicate offspring");

NuggetDeathPatch.Prefix(nugget, true, out var death);
Check(death != null, "Ordinary Nugget death captures ability");
nugget.mDead = true;
NuggetDeathPatch.Postfix(nugget, death);
var blast = MineFusions.Blasts.Last();
Check(blast.Damage == 300 && blast.Radius == ScaredyFusions.SourceUnit && blast.Row == nugget.mRow,
    "Nugget death uses same-row one-unit 300-damage potato blast");
nugget.mDead = false;
NuggetDeathPatch.Prefix(nugget, false, out death);
Check(death == null, "Deferred ingredient death cannot explode");
PlayerPlantingScope.ConsumingIngredient = nugget.Pointer;
NuggetDeathPatch.Prefix(nugget, true, out death);
Check(death == null, "Fusion consumption cannot explode");
PlayerPlantingScope.ConsumingIngredient = IntPtr.Zero;
CherryBombPlants.DisposingBoard = nugget.mBoard.Pointer;
NuggetDeathPatch.Prefix(nugget, true, out death);
Check(death == null, "Board disposal cannot explode");
CherryBombPlants.DisposingBoard = IntPtr.Zero;
Console.WriteLine($"Puff specials: {checks} checks passed.");
