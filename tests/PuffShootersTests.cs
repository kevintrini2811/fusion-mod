using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool condition, string message)
{
    ++checks;
    if (!condition) throw new Exception(message);
}
Plant Make(FusionPlant identity)
{
    var plant = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    PuffShooters.Initialize(plant);
    return plant;
}
Zombie Target(Plant plant, float x = 400, int row = 2)
{
    var zombie = new Zombie { mBoard = plant.mBoard, X = x, mRow = row };
    plant.mBoard.m_zombies.Add(zombie);
    return zombie;
}
var variants = new[] { FusionPlant.PeaShroom, FusionPlant.RepeatShroom,
    FusionPlant.SplitPeaShroom, FusionPlant.GatlingPeaShroom };
var times = new[] { new[] {38}, new[] {38,67}, new[] {38,38,63}, new[] {50,70,90,110} };
for (int variant = 0; variant < variants.Length; ++variant)
{
    var identity = variants[variant];
    var plant = Make(identity);
    Check(FusionPlant.From(plant) == identity, "Persisted identity");
    Check(plant.mPlantHealth == 300 && plant.mPlantMaxHealth == 300, "Source health");
    Check(plant.mLaunchRate == 225, "Source interval");
    Check(FusionPlant.HasImportedVisuals(identity), "Original visuals registered");
    Check(!PuffFirePatch.Prefix(plant) && !PuffShootingPatch.Prefix(plant), "Native duplicate fire suppressed");
    PuffShooters.Tick(plant);
    Check(plant.mStartRow == 0 && plant.mBoard.Projectiles.Count == 0, "No enemy, no burst");
    var enemy = Target(plant);
    Check(PuffShooters.HasTarget(plant, identity), "Same-row enemy");
    enemy.mRow++;
    Check(!PuffShooters.HasTarget(plant, identity), "Other row excluded");
    enemy.mRow--;
    enemy.mMindControlled = true;
    Check(!PuffShooters.HasTarget(plant, identity), "Friendly excluded");
    enemy.mMindControlled = false;
    enemy.X = 100;
    Check(PuffShooters.HasTarget(plant, identity) == (identity == FusionPlant.SplitPeaShroom), "Rear target rules");
    enemy.X = 400;
    int emitted = 0;
    for (int tick = 0; tick < PuffShooters.Duration(identity); ++tick)
    {
        PuffShooters.Tick(plant);
        while (emitted < plant.mBoard.Projectiles.Count)
        {
            Check(tick == times[variant][emitted], "Original animation event tick");
            var shot = plant.mBoard.Projectiles[emitted++];
            Check(PuffShooters.Contains(shot), "Saved projectile marker");
            Check(!RetaliationProjectiles.IsPea(shot), "Radial renderer does not steal Puff projectile");
            Check(!PuffTorchwoodPatch.Prefix(shot), "Source bullet 10 excludes Torchwood");
            Check(shot.mVelX < 0 == (identity == FusionPlant.SplitPeaShroom && emitted > 1), "Shot direction");
        }
    }
    Check(emitted == times[variant].Length && plant.mStartRow == 0, "Burst count and completion");
    // Same serialized cooldown/phase after loading produces the same suffix.
    var loaded = Make(identity);
    Target(loaded);
    loaded.mLaunchCounter = plant.mLaunchCounter;
    loaded.mStartRow = 20;
    plant.mStartRow = 20;
    plant.mBoard.Projectiles.Clear();
    for (int tick = 0; tick < 20; ++tick) { PuffShooters.Tick(plant); PuffShooters.Tick(loaded); }
    Check(plant.mBoard.Projectiles.Count == loaded.mBoard.Projectiles.Count
        && plant.mLaunchCounter == loaded.mLaunchCounter && plant.mStartRow == loaded.mStartRow, "Loaded burst continuation");
    foreach (int inactive in Enumerable.Range(0, 6))
    {
        var paused = Make(identity); Target(paused);
        paused.mStartRow = 10; paused.mLaunchCounter = 100;
        if (inactive == 0) paused.mBoard.mPaused = true;
        if (inactive == 1) paused.mIsAsleep = true;
        if (inactive == 2) paused.mDead = true;
        if (inactive == 3) paused.mIsOnBoard = false;
        if (inactive == 4) paused.mSquished = true;
        if (inactive == 5) paused.Bungee = true;
        PuffShooters.Tick(paused);
        Check(paused.mStartRow == 10 && paused.mLaunchCounter == 100, "Inactive simulation freezes");
    }
}
var rear = new Projectile { mCobTargetX = PuffShooters.RearShot, mVelX = -PuffShooters.Speed, mPosX = -2 };
PuffShooters.Rebound(rear);
Check(rear.mPosX == 2 && rear.mX == 2 && rear.mVelX > 0
    && rear.mCobTargetX == PuffShooters.ReboundedShot, "Reflect at left edge and preserve identity");
rear.mPosX = -1;
PuffShooters.Rebound(rear);
Check(rear.mPosX == -1, "Only one rebound");
rear.mCobTargetX = PuffShooters.RearShot; rear.mBoard.mPaused = true;
PuffShooters.Rebound(rear);
Check(rear.mPosX == -1 && rear.mCobTargetX == PuffShooters.RearShot, "Paused rebound");
var native = new Plant { mSeedType = SeedType.Puffshroom, mPlantHealth = 7 };
PuffShooters.Initialize(native);
Check(native.mPlantHealth == 7 && PuffShooterPatch.Prefix(native)
    && PuffFirePatch.Prefix(native), "Native Puff unchanged");
var recipes = new (FusionPlant A, FusionPlant B, FusionPlant Result)[] {
    (SeedType.Puffshroom, SeedType.Peashooter, FusionPlant.PeaShroom),
    (SeedType.Puffshroom, SeedType.Repeater, FusionPlant.RepeatShroom),
    (FusionPlant.PeaShroom, SeedType.Peashooter, FusionPlant.RepeatShroom),
    (SeedType.Puffshroom, SeedType.Splitpea, FusionPlant.SplitPeaShroom),
    (FusionPlant.PeaShroom, SeedType.Repeater, FusionPlant.SplitPeaShroom),
    (FusionPlant.RepeatShroom, SeedType.Peashooter, FusionPlant.SplitPeaShroom),
    (SeedType.Puffshroom, SeedType.Gatlingpea, FusionPlant.GatlingPeaShroom),
    (FusionPlant.PeaShroom, SeedType.Splitpea, FusionPlant.GatlingPeaShroom),
    (FusionPlant.RepeatShroom, SeedType.Repeater, FusionPlant.GatlingPeaShroom),
    (FusionPlant.SplitPeaShroom, SeedType.Peashooter, FusionPlant.GatlingPeaShroom) };
foreach (var (a, b, expected) in recipes)
{
    Check(FusionRecipes.TryGet(a, b, out var result) && result == expected, "Source recipe");
    Check(FusionRecipes.TryGet(b, a, out result) && result == expected, "Reverse source recipe");
}
// Reproduce the reported instant impact through the native ground branch.
var oldShot = new Projectile {mPosY=200, mShadowY=212, mVelX=PuffShooters.Speed,
    mMotionType=ProjectileMotion.Star};
oldShot.UpdateNormalMotion();
Check(oldShot.mDead, "Old 12-pixel flight height reproduces immediate impact");
foreach (var identity in variants)
foreach (int slotOffsetY in new[] {-16,12})
{
    var shooter=Make(identity);
    shooter.mY+=slotOffsetY;
    var victim=Target(shooter,700);
    for(int i=0;i<PuffShooters.Duration(identity);++i) PuffShooters.Tick(shooter);
    var projectiles=shooter.mBoard.Projectiles;
    foreach(var projectile in projectiles)
    {
        Check(projectile.mShadowY-projectile.mPosY>=28, "All muzzle positions clear native ground threshold");
        projectile.UpdateNormalMotion();
        Check(!projectile.mDead, "Projectile survives first native movement tick");
    }
    for(int tick=0;tick<600;++tick)
        foreach(var projectile in projectiles)
        {
            PuffShooters.Rebound(projectile);
            projectile.UpdateNormalMotion();
        }
    Check(victim.Damage==20*times[Array.IndexOf(variants,identity)].Length,
        "All burst projectiles reach distant enemy, including both rear rebounds");
}
Console.WriteLine($"{checks} Puff shooter checks passed; no game code loaded.");
