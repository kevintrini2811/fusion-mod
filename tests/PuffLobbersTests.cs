using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool condition, string message) { ++checks; if (!condition) throw new Exception(message); }
Plant Make(FusionPlant identity)
{
    var p = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    PuffLobbers.Initialize(p); PuffShooters.Initialize(p); return p;
}
uint nextId = 0;
Zombie Enemy(Plant p, float x = 400, bool small = false)
{
    var z = new Zombie { DataID = ++nextId, Pointer = (IntPtr)nextId, mBoard = p.mBoard, X = x, mRow = p.mRow,
        mScaleZombie = small ? .7f : 1, mBodyHealth = 1000, mBodyMaxHealth = 1000 };
    p.mBoard.m_zombies.Add(z); return z;
}
Projectile Shot(Plant p, int marker, ProjectileType type = ProjectileType.Cabbage) => new()
{ mBoard = p.mBoard, mProjectileType = type, mCobTargetX = marker, mVelX = 4, mVelZ = 2 };

foreach (var (left, right, expected) in new[] {
    (FusionPlant.PeaShroom, FusionPlant.Bucket, FusionPlant.BuckShroom),
    ((FusionPlant)SeedType.Puffshroom, FusionPlant.Buckshooter, FusionPlant.BuckShroom),
    ((FusionPlant)SeedType.Puffshroom, (FusionPlant)SeedType.Cabbagepult, FusionPlant.SproutPult),
    ((FusionPlant)SeedType.Puffshroom, (FusionPlant)SeedType.Kernelpult, FusionPlant.BabyCorn),
    ((FusionPlant)SeedType.Puffshroom, (FusionPlant)SeedType.Melonpult, FusionPlant.SlicePult) })
{
    Check(FusionRecipes.TryGet(left, right, out var result) && result == expected, "Complete source recipe");
    Check(FusionRecipes.TryGet(right, left, out result) && result == expected, "Reverse source recipe");
}
foreach (var identity in new[] { FusionPlant.BuckShroom, FusionPlant.SproutPult, FusionPlant.BabyCorn, FusionPlant.SlicePult })
{
    var p = Make(identity);
    Check(FusionPlant.From(p) == identity && FusionPlant.HasImportedVisuals(identity), "Saved identity and graphics");
    Check(p.mPlantHealth == (identity == FusionPlant.BuckShroom ? 1000 : 300), "Actual source toughness");
    Check(!PuffFirePatch.Prefix(p) && !PuffShootingPatch.Prefix(p), "Native spore and second timer writer suppressed");
    foreach (int inactive in Enumerable.Range(0, 6))
    {
        p = Make(identity); Enemy(p);
        p.mLaunchCounter = 100; p.mStartRow = 50;
        if (inactive == 0) p.mBoard.mPaused = true;
        if (inactive == 1) p.mIsAsleep = true;
        if (inactive == 2) p.mDead = true;
        if (inactive == 3) p.mIsOnBoard = false;
        if (inactive == 4) p.mSquished = true;
        if (inactive == 5) p.Bungee = true;
        PuffShooterPatch.Prefix(p);
        Check(p.mLaunchCounter == 100 && p.mStartRow == 50 && p.mBoard.Projectiles.Count == 0,
            "Inactive plants retain counters and cannot shoot");
    }
}
foreach (var identity in new[] { FusionPlant.SproutPult, FusionPlant.BabyCorn, FusionPlant.SlicePult })
{
    var p = Make(identity);
    Check(PuffLobbers.Target(p) == null, "No target, no attack");
    var rear = Enemy(p, 100); var friendly = Enemy(p, 300); friendly.mMindControlled = true;
    var air = Enemy(p, 300); air.Category = DamageRangeFlags.Flying;
    var otherRow = Enemy(p, 300); ++otherRow.mRow;
    Check(PuffLobbers.Target(p) == null, "Exclude rear, friendly, flying and other rows");
    var target = Enemy(p, 650);
    Check(PuffLobbers.Target(p) == target, "Full row target");
    for (int i = 0; i < 61; ++i) PuffShooterPatch.Prefix(p);
    Check(p.mBoard.Projectiles.Count == 0, "No shot before original .616667 event");
    PuffShooterPatch.Prefix(p);
    var shot = p.mBoard.Projectiles.Single();
    Check(PuffLobbers.Family(shot) != 0 && shot.mMotionType == ProjectileMotion.Lobbed, "Saved original lob projectile");
    Check(p.mLaunchRate == 300 && p.mLaunchCounter == 239, "Three-second randomized source interval");
    var loaded = Make(identity); Enemy(loaded, 650);
    loaded.mLaunchCounter = p.mLaunchCounter; loaded.mStartRow = p.mStartRow;
    loaded.mShootingCounter = p.mShootingCounter;
    for (int i = 0; i < 38; ++i) { PuffLobbers.Tick(p); PuffLobbers.Tick(loaded); }
    Check(loaded.mBoard.Projectiles.Count == 0 && loaded.mStartRow == 0, "Load after event cannot repeat shot");
    Check(p.mStartRow == loaded.mStartRow && p.mLaunchCounter == loaded.mLaunchCounter, "Saved phase continues identically");
    // Integrate the native velocity fields analytically; the flight must reach
    // the selected target rather than inheriting the Puff's short-range speed.
    float x = shot.mPosX, z = shot.mPosZ, vertical = shot.mVelZ;
    for (int t = 0; t < 150; ++t) { x += shot.mVelX; vertical += shot.mAccZ; z += vertical; }
    float impactZ = shot.mProjectileType is ProjectileType.Cabbage or ProjectileType.Butter ? -35 : -30;
    Check(Math.Abs(x - 650) < .02f && Math.Abs(z - impactZ) < .02f, "Saved ballistic arc reaches target at native impact height");
    // Actual UpdateLobMotion at 0x6c97d8 skips Cabbage/Kernel collision
    // through -30 pixels (-32 for Butter). Ensure the target is still under
    // the descending projectile when this native gate finally opens.
    float gate = shot.mProjectileType == ProjectileType.Butter ? -32 : -30;
    for (int t = 0; z <= gate && t < 20; ++t)
    { x += shot.mVelX; vertical += shot.mAccZ; z += vertical; }
    Check(z > gate && x >= target.GetZombieRect().xMin && x <= target.GetZombieRect().xMax,
        "Native descending-height gate opens before projectile passes target");
    var native = new Projectile { mProjectileType = shot.mProjectileType };
    Check(PuffLobImpactPatch.Prefix(native, target), "Native lobbers retain original impact behavior");
}
var corn = Make(FusionPlant.BabyCorn);
var closeSmall = Enemy(corn, 300, true); var distant = Enemy(corn, 600);
Check(PuffLobbers.Target(corn) == distant, "Corn prioritizes unshrunk target");
distant.mScaleZombie = .7f;
Check(PuffLobbers.Target(corn) == closeSmall, "Corn falls back to nearest shrunk target");
corn.mShootingCounter = 1;
PuffLobbers.Fire(corn, closeSmall);
Check(PuffLobbers.Family(corn.mBoard.Projectiles.Single()) == 3, "Butter selection controls projectile");

var cabbage = Make(FusionPlant.SproutPult);
var smallTarget = Enemy(cabbage, 300, true); var fresh = Enemy(cabbage, 400); var third = Enemy(cabbage, 500);
var bounce = Shot(cabbage, PuffLobbers.CabbageShot);
PuffLobbers.Impact(bounce, smallTarget);
Check(smallTarget.Damage == 40 && !bounce.mDead && PuffLobbers.Family(bounce) == 1, "First cabbage hit captures shrink");
Check(Math.Abs(bounce.mVelX - 1.6f) < .001f && bounce.mVelZ == -1, "Actual source bounce velocity factors");
PuffLobbers.Impact(bounce, smallTarget);
Check(smallTarget.Hits == 1, "Bounce cannot immediately hit same zombie again");
Zombie collision = smallTarget;
PuffLobCollisionPatch.Postfix(bounce, ref collision);
Check(collision == null, "Native collision excludes prior saved target");
var restored = Shot(cabbage, (int)bounce.mCobTargetX);
restored.mTargetZombieID = bounce.mTargetZombieID;
PuffLobbers.Impact(restored, fresh);
Check(PuffSupport.IsSmall(fresh) && fresh.mBodyHealth == 700 && fresh.Damage == 40, "Saved cabbage propagates shrink before damage");
PuffLobbers.Impact(restored, third);
Check(restored.mDead && third.Damage == 40 && PuffSupport.IsSmall(third), "Third hit ends projectile after two bounces");
var ordinary = Shot(cabbage, PuffLobbers.CabbageShot);
var normal = Enemy(cabbage);
PuffLobbers.Impact(ordinary, normal);
Check(!PuffSupport.IsSmall(normal), "Ordinary cabbage never invents shrink");
var impTarget = Enemy(cabbage, 600); impTarget.mZombieType = ZombieType.Imp;
PuffLobbers.Impact(bounce, impTarget);
Check(impTarget.mDead, "Cabbage propagation dispatches source Imp shrink override");
PuffLobbers.Impact(ordinary, null);
Check(ordinary.mDead, "Ground landing ends bouncing cabbage");

foreach (bool small in new[] { false, true })
{
    var p = Make(FusionPlant.SlicePult); var target = Enemy(p, small: small); var bystander = Enemy(p);
    var slice = Shot(p, PuffLobbers.MelonShot);
    PuffLobImpactPatch.Prefix(slice, target);
    Check(target.Damage == (small ? 160 : 80) && target.Hits == 1 && slice.mDead && bystander.Hits == 0,
        "Slice has source multiplier, exactly one direct hit and no splash");
}
foreach (int kind in Enumerable.Range(0, 5))
{
    var p = Make(FusionPlant.BabyCorn); var target = Enemy(p, small: kind == 1 || kind == 2);
    if (kind == 2) target.mButteredCounter = 600;
    if (kind == 3) target.mZombieType = ZombieType.Imp;
    if (kind == 4) target.mZombieType = ZombieType.Boss;
    var butter = Shot(p, PuffLobbers.ButterShot, ProjectileType.Butter);
    PuffLobbers.Impact(butter, target);
    Check(target.Damage == 40 && butter.mDead, "Small butter direct damage and lifetime");
    if (kind == 0) Check(PuffSupport.IsSmall(target) && target.mButteredCounter == 0, "First butter shrinks without immobilizing");
    if (kind == 1) Check(target.mButteredCounter == 150, "Repeated butter immobilizes for 1.5 seconds");
    if (kind == 2) Check(target.mButteredCounter == 600, "Preserve longer butter");
    if (kind == 3) Check(target.mDead, "Small butter kills imps");
    if (kind == 4) Check(!PuffSupport.IsSmall(target), "Boss retains shrink immunity");
}
var recycle = Make(FusionPlant.BabyCorn); var recycled = Enemy(recycle);
recycled.OnDamage = () => ++recycled.DataID;
PuffLobbers.Impact(Shot(recycle, PuffLobbers.ButterShot, ProjectileType.Butter), recycled);
Check(!PuffSupport.IsSmall(recycled), "Damage callback pool reuse cannot shrink another identity");
var buck = Make(FusionPlant.BuckShroom); Enemy(buck, 700);
for (int i = 0; i < 39; ++i) PuffShooterPatch.Prefix(buck);
var ironShot = buck.mBoard.Projectiles.Single();
Check(IronProjectiles.IsSmall(ironShot) && !RetaliationProjectiles.IsPea(ironShot), "Small iron identity owns renderer and damage");
foreach (var identity in new[] { FusionPlant.SilverCabbage, FusionPlant.SilverMelon })
{
    var p = Make(identity); SilverLobbers.Initialize(p);
    Enemy(p, 650);
    for (int i = 0; i < SilverLobbers.EmitTick(identity); ++i) SilverLobbers.Tick(p);
    Check(p.mBoard.Projectiles.Count == 0, "Silver waits for original emission event");
    SilverLobbers.Tick(p);
    var shot = p.mBoard.Projectiles.Single();
    Check(SilverLobbers.Family(shot) == (identity == FusionPlant.SilverCabbage ? 1 : 2)
        && shot.mMotionType == ProjectileMotion.Lobbed, "Silver saved projectile family");
    var loadedSilver = Make(identity); SilverLobbers.Initialize(loadedSilver); Enemy(loadedSilver, 650);
    loadedSilver.mLaunchCounter = p.mLaunchCounter; loadedSilver.mStartRow = p.mStartRow;
    for (int i = p.mStartRow; i > 0; --i) SilverLobbers.Tick(loadedSilver);
    Check(loadedSilver.mBoard.Projectiles.Count == 0 && loadedSilver.mStartRow == 0,
        "Restoring after Silver emission cannot duplicate projectile");
    p.mBoard.mPaused = true; int counter = p.mLaunchCounter;
    SilverLobbers.Tick(p);
    Check(p.mLaunchCounter == counter, "Silver pause preserves saved counters");
}
foreach (bool melon in new[] { false, true })
{
    var p = Make(melon ? FusionPlant.SilverMelon : FusionPlant.SilverCabbage);
    var direct = Enemy(p, 400); var splash = Enemy(p, 440); var friend = Enemy(p, 440);
    friend.mMindControlled = true;
    var shot = new Projectile { mBoard = p.mBoard, mProjectileType = melon ? ProjectileType.Melon : ProjectileType.Cabbage,
        mCobTargetX = melon ? SilverLobbers.MelonShot : SilverLobbers.CabbageShot,
        mPosX = 400, mPosY = 240, mRow = p.mRow };
    SilverLobbers.Impact(shot, direct);
    Check(shot.mDead && direct.Damage == (melon ? 100 : 60), "Silver direct hit without native double damage");
    Check(splash.Damage == (melon ? 33 : 0) && friend.Damage == 0,
        "Silver Melon splash excludes friends and does not apply to cabbage");
    SilverLobbers.Impact(shot, direct);
    Check(direct.Hits == 1, "Removed Silver projectile cannot hit twice");
}
var ground = Make(FusionPlant.SilverMelon); var groundTarget = Enemy(ground, 400);
var groundShot = new Projectile { mBoard = ground.mBoard, mProjectileType = ProjectileType.Melon,
    mCobTargetX = SilverLobbers.MelonShot, mPosX = 400, mPosY = 240, mRow = ground.mRow };
SilverLobbers.Impact(groundShot, null);
Check(groundTarget.Damage == 33, "Silver Melon ground impact retains splash");
Check(SilverLobbers.SplashDamage(30) == 26 && SilverLobbers.SplashDamage(1000) == 1,
    "Original Silver crowd cap and minimum");
Check(SilverLobbers.Family(new Projectile { mProjectileType = ProjectileType.Melon }) == 0,
    "Native melon retains native impact");
var silverHypno = Make(FusionPlant.SilverHypnoShroom);
var eater = Enemy(silverHypno); eater.BiteTarget = silverHypno;
silverHypno.mIsAsleep = true;
Check(SilverHypnoShroom.Capture(eater) == null, "Sleeping Silver Hypno gives no hypnosis reward");
silverHypno.mIsAsleep = false;
var bite = SilverHypnoShroom.Capture(eater);
SilverHypnoShroom.Reward(eater, bite);
Check(silverHypno.mBoard.Coins.Count == 0, "No reward without native consumption and hypnosis");
silverHypno.Die(); eater.StartMindControlled(); SilverHypnoShroom.Reward(eater, bite);
Check(silverHypno.mBoard.Coins.Count == 2 && silverHypno.mBoard.Coins.All(c => c.Type == CoinType.Gold),
    "Successful native Silver hypnosis pays two saved native gold pickups");
Check(SilverHypnoShroom.Capture(eater) == null, "Consumed mushroom or friendly eater cannot earn another reward");
var replacedHypno = Make(FusionPlant.SilverHypnoShroom);
var replacedEater = Enemy(replacedHypno); replacedEater.BiteTarget = replacedHypno;
var replacedBite = SilverHypnoShroom.Capture(replacedEater);
replacedHypno.Die(); replacedEater.StartMindControlled(); ++replacedHypno.DataID;
SilverHypnoShroom.Reward(replacedEater, replacedBite);
Check(replacedHypno.mBoard.Coins.Count == 0, "Recycled plant cannot inherit Silver hypnosis reward");
// New silver kernel: target fallback, saved choice, collision and money.
var kernel = Make(FusionPlant.SilverKernel);
var fallback = Enemy(kernel, 450); ++fallback.mRow;
Check(SilverLobbers.Target(kernel) == fallback, "Kernel can target another row");
var ownRow = Enemy(kernel, 700);
Check(SilverLobbers.Target(kernel) == ownRow, "Kernel prioritizes own row over distance");
kernel.mShootingCounter = 1;
SilverLobbers.Fire(kernel, fallback);
var silverButter = kernel.mBoard.Projectiles.Single();
Check(SilverLobbers.Family(silverButter) == 4 && silverButter.mRow == fallback.mRow,
    "Saved silver butter identity and destination row");
Check(silverButter.mCobTargetRow == 2 && kernel.mTargetY == 5,
    "First payout retains five-coin remainder in saved plant");
SilverLobbers.Impact(silverButter, fallback);
Check(fallback.Damage == 60 && fallback.mButteredCounter == 400,
    "Silver butter deals sixty and immobilizes for four seconds");
Check(kernel.mBoard.Coins.Count == 2, "First payout creates twenty native coins");
SilverLobbers.Impact(silverButter, fallback);
Check(kernel.mBoard.Coins.Count == 2, "Duplicate impact cannot duplicate payout");
SilverLobbers.Fire(kernel, ownRow);
var groundButter = kernel.mBoard.Projectiles.Last();
Check(groundButter.mCobTargetRow == 3 && kernel.mTargetY == 0,
    "Second payout settles saved remainder");
SilverLobbers.Impact(groundButter, null);
Check(kernel.mBoard.Coins.Count == 5, "Butter pays on ground, fifty total across two payouts");
kernel.mShootingCounter = 0;
SilverLobbers.Fire(kernel, ownRow);
var silverCorn = kernel.mBoard.Projectiles.Last();
SilverLobbers.Impact(silverCorn, ownRow);
Check(SilverLobbers.Family(silverCorn) == 3 && ownRow.Damage == 40
    && ownRow.mButteredCounter == 0 && kernel.mBoard.Coins.Count == 5,
    "Silver kernel has forty damage, no stun and no money");
// Pots affect native kernels and both imported corn variants through the same live support lookup.
foreach (var cornIdentity in new[] {(FusionPlant)SeedType.Kernelpult, FusionPlant.BabyCorn, FusionPlant.SilverKernel})
{
    var top = Make(cornIdentity);
    var pot = new Plant { mBoard=top.mBoard, mPlantCol=top.mPlantCol, mRow=top.mRow,
        mSeedType=SeedType.Flowerpot, mImitaterType=SeedType.Kernelpult };
    top.mBoard.m_plants.Add(pot);
    Check(PotBoosts.ButterRange(top)==2, "Butter Pot doubles chance for every available kernel family");
    ++pot.mPlantCol;
    Check(PotBoosts.ButterRange(top)==4, "Moving pot removes butter bonus");
    --pot.mPlantCol; pot.mDead=true;
    Check(PotBoosts.ButterRange(top)==4, "Dead pot cannot provide butter bonus");
}
foreach (var cabbageIdentity in new[] {(FusionPlant)SeedType.Cabbagepult, FusionPlant.SproutPult, FusionPlant.SilverCabbage})
{
    var top=Make(cabbageIdentity);
    var pot=new Plant {mBoard=top.mBoard,mPlantCol=top.mPlantCol,mRow=top.mRow,
        mSeedType=SeedType.Flowerpot,mImitaterType=SeedType.Cabbagepult};
    top.mBoard.m_plants.Add(pot);
    top.mLaunchCounter=300;
    CabbagePotIntervalPatch.Postfix(top,0);
    Check(top.mLaunchCounter==200,"Cabbage Pot scales a newly reset interval to two thirds");
    top.mLaunchCounter=199;
    CabbagePotIntervalPatch.Postfix(top,200);
    Check(top.mLaunchCounter==199,"Existing interval is not scaled every tick");
    top.mBoard.mPaused=true; top.mLaunchCounter=300;
    CabbagePotIntervalPatch.Postfix(top,0);
    Check(top.mLaunchCounter==300,"Paused board cannot acquire a speed adjustment");
}
PotBoosts.ChoosingButter=true;
int range=4; ButterPotNativeRandomPatch.Prefix(ref range);
Check(range==2,"Native butter choice uses original animation branch at probability one half");
range=15; ButterPotNativeRandomPatch.Prefix(ref range);
Check(range==15,"Unrelated random ranges stay unchanged");
PotBoosts.ChoosingButter=false; range=4; ButterPotNativeRandomPatch.Prefix(ref range);
Check(range==4,"Unscoped native randomness stays unchanged");

var boostedMelon = Make(FusionPlant.SilverMelon);
var melonSupport = new Plant { mBoard = boostedMelon.mBoard, mSeedType = SeedType.Flowerpot,
    mImitaterType = SeedType.Melonpult, mPlantCol = boostedMelon.mPlantCol, mRow = boostedMelon.mRow };
boostedMelon.mBoard.m_plants.Add(melonSupport);
var boostedDirect = Enemy(boostedMelon, 400);
var boostedCrowd = Enumerable.Range(0, 30).Select(_ => Enemy(boostedMelon, 440)).ToArray();
SilverLobbers.Fire(boostedMelon, boostedDirect);
var boostedShot = boostedMelon.mBoard.Projectiles.Single();
Check(MelonPot.Boosted(boostedShot), "Silver melon saves its support bonus on emission");
melonSupport.mDead = true;
boostedShot.mPosX = 400; boostedShot.mPosY = 240;
SilverLobbers.Impact(boostedShot, boostedDirect);
Check(boostedDirect.Damage == 100 && boostedCrowd.All(z => z.Damage == 66),
    "Airborne Silver bonus survives pot removal and ignores crowd falloff");

Console.WriteLine($"{checks} Puff and Silver lobber checks passed; no game code loaded.");
