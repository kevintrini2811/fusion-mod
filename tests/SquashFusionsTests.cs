using FusionMod;
using Il2CppReloaded.Gameplay;

int count = 0;
void Check(bool condition, string name)
{ if (!condition) throw new Exception(name); ++count; }
Plant MakePlant(FusionPlant identity) => new() { mSeedType = identity.NativeType, mImitaterType = identity.Marker };
Zombie Add(Plant plant, float x = 240, int row = 2)
{
    var zombie = new Zombie { mBoard = plant.mBoard, Rect = new(x-10, 230, x+10, 250), mRow = row,
        Pointer = (IntPtr)(10 + plant.mBoard.m_zombies.Count) };
    plant.mBoard.m_zombies.Add(zombie); return zombie;
}
var variants = new[] { FusionPlant.CherrySquash, FusionPlant.SpicySquash, FusionPlant.IcySquash, FusionPlant.HypnoSquash };
foreach (var variant in variants)
{
    var plant = MakePlant(variant);
    Check(FusionPlant.From(plant) == variant && FusionPlant.HasImportedVisuals(variant), "saved identity and original visuals");
    Check(FusionRecipes.TryGet(SeedType.Squash, variant.Marker, out var result) && result == variant, "forward recipe");
    Check(FusionRecipes.TryGet(variant.Marker, SeedType.Squash, out result) && result == variant, "reverse recipe");
    Check(SquashFusions.Active(plant), "active");
    plant.mBoard.mPaused = true; Check(!SquashFusions.Active(plant), "pause"); plant.mBoard.mPaused = false;
    plant.mOnBungeeState = PlantOnBungeeState.OnBungee; Check(!SquashFusions.Active(plant), "bungee");
    plant.mOnBungeeState = PlantOnBungeeState.NotOnBungee;
    CherryBombPlants.DisposingBoard = plant.mBoard.Pointer; Check(!SquashFusions.Active(plant), "board teardown");
    CherryBombPlants.DisposingBoard = IntPtr.Zero;
    plant.mIsOnBoard = false; Check(!SquashFusions.Active(plant), "carried"); plant.mIsOnBoard = true;
    plant.mApp.GameScene = GameScenes.Loading; Check(!SquashFusions.Active(plant), "loading");
}
var ordinary = MakePlant(SeedType.Squash);
Check(!FusionPlant.IsSquash(FusionPlant.From(ordinary)), "ordinary squash untouched");
var splash = MakePlant(FusionPlant.Splash);
var submerged = Add(splash); submerged.Category = DamageRangeFlags.Submerged;
Check(SquashFusions.FindTarget(splash) == submerged, "Splash acquires submerged target without native range hook");
var groundSplash = Add(splash);
var farSubmerged = Add(splash, 500); farSubmerged.Category = DamageRangeFlags.Submerged;
var otherSubmerged = Add(splash, 240, 1); otherSubmerged.Category = DamageRangeFlags.Submerged;
var friendlySubmerged = Add(splash); friendlySubmerged.Category = DamageRangeFlags.Submerged; friendlySubmerged.mMindControlled = true;
SquashFusions.CrushSubmerged(splash);
Check(submerged.Damage == 1800, "Splash crushes overlapping submerged target");
Check(groundSplash.Damage == 0 && farSubmerged.Damage == 0 && otherSubmerged.Damage == 0
    && friendlySubmerged.Damage == 0, "native ground damage remains single; bounds and friend exclusions retained");
SquashFusions.CrushSubmerged(ordinary);
Check(ordinary.mBoard.m_zombies.Count == 0, "ordinary squash retains native landing");
foreach (var variant in variants)
{
    var plant = MakePlant(variant);
    bool extended = variant == FusionPlant.CherrySquash || variant == FusionPlant.SpicySquash;
    var far = Add(plant, 450);
    Check((SquashFusions.FindTarget(plant) == far) == extended, "forward acquisition range");
    var close = Add(plant, 260);
    Check(SquashFusions.FindTarget(plant) == close, "nearest target");
    close.mMindControlled = true; far.mDead = true;
    Check(SquashFusions.FindTarget(plant) == null, "dead and friendly exclusion");
    close.mMindControlled = false; close.mRow = 1;
    Check(SquashFusions.FindTarget(plant) == null, "other row exclusion");
    close.mRow = 2; close.Category = DamageRangeFlags.Flying;
    Check(SquashFusions.FindTarget(plant) == null, "airborne exclusion");
}
var ice = MakePlant(FusionPlant.IcySquash);
var centre = Add(ice);
var adjacent = Add(ice, 300, 1);
var outside = Add(ice, 430);
var otherRow = Add(ice, 240, 0);
var immune = Add(ice); immune.ImmuneToFreeze = true;
var friendly = Add(ice); friendly.mMindControlled = true;
var frozen = Add(ice); frozen.mIceTrapCounter = 900;
SnowPea.CreatingIceTrap = (IntPtr)777;
SquashFusions.AfterLanding(SquashFusions.Capture(ice));
Check(centre.Damage == 600 && centre.mIceTrapCounter == 400, "ice splash and four second freeze, no extra 20");
Check(adjacent.Damage == 600, "adjacent row splash");
Check(outside.Damage == 0 && otherRow.Damage == 0 && friendly.Damage == 0, "splash bounds and friend exclusion");
Check(immune.Damage == 600 && immune.mIceTrapCounter == 0, "freeze immunity keeps damage");
Check(frozen.mIceTrapCounter == 900, "longer freeze retained");
Check(SnowPea.CreatingIceTrap == (IntPtr)777, "freeze context restored");
var fire = MakePlant(FusionPlant.SpicySquash);
var farFire = Add(fire, 850); farFire.mIceTrapCounter = 400;
var wrongFireRow = Add(fire, 240, 1);
SquashFusions.AfterLanding(SquashFusions.Capture(fire));
Check(farFire.Damage == 1800 && farFire.ColdRemovals == 1 && farFire.mIceTrapCounter == 0, "entire fire row and thaw");
Check(wrongFireRow.Damage == 0 && fire.mApp.SoundSystem.Calls == 1, "fire row bounds and sound");
var cherry = MakePlant(FusionPlant.CherrySquash);
SquashFusions.AfterLanding(SquashFusions.Capture(cherry));
Check(CherryBombPlants.Last?.Identity == FusionPlant.CherrySquash && CherryBombPlants.Last.X == 240,
    "existing cherry explosion path at landing position");
var hypno = MakePlant(FusionPlant.HypnoSquash);
var victim = Add(hypno);
var flags = DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody;
SquashFusionLandingPatch.Prefix(hypno, out var previous);
try
{
    Check(SquashFusions.ConvertCrush(victim, 1800, flags) && victim.mMindControlled && victim.Damage == 0, "hypnosis replaces crush");
    victim.mMindControlled = false; victim.mZombieType = ZombieType.Gargantuar;
    Check(SquashFusions.ConvertCrush(victim, 1800, flags), "ordinary giant inherits hypnosis");
    victim.mMindControlled = false; victim.mZombieType = ZombieType.Boss;
    Check(!SquashFusions.ConvertCrush(victim, 1800, flags), "immune boss keeps native damage");
    victim.mZombieType = ZombieType.Normal; victim.FailControl = true;
    Check(!SquashFusions.ConvertCrush(victim, 1800, flags), "unsuccessful control keeps native damage");
    victim.FailControl = false;
    Check(!SquashFusions.ConvertCrush(victim, 600, flags) && !SquashFusions.ConvertCrush(victim, 1800, 0), "unrelated damage untouched");
    victim.mBoard = new Board { Pointer = (IntPtr)99 };
    Check(!SquashFusions.ConvertCrush(victim, 1800, flags), "foreign board untouched");
    var outer = SquashFusions.CurrentLanding;
    SquashFusionLandingPatch.Prefix(ordinary, out var nested);
    try { Check(SquashFusions.CurrentLanding == null, "ordinary nested crush clears context"); throw new InvalidOperationException(); }
    catch (InvalidOperationException) { }
    finally { SquashFusionLandingPatch.Finalizer(nested); }
    Check(SquashFusions.CurrentLanding == outer, "nested failure restores context");
}
finally { SquashFusionLandingPatch.Finalizer(previous); }
Check(SquashFusions.CurrentLanding == null, "landing context cleared");
SquashFusionLandingPatch.Prefix(cherry, out previous);
try { CherryBombPlants.Last = null; ++cherry.DataID; SquashFusionLandingPatch.Postfix(cherry);
    Check(CherryBombPlants.Last == null, "pooled plant cannot emit stale landing"); }
finally { SquashFusionLandingPatch.Finalizer(previous); }
ice.mState = PlantState.SquashLook; ice.mStateCountdown = 40; ice.mTargetX = 100;
Check(SquashFusions.Animation(ice).Clip == "lookleft", "left original look");
ice.mTargetX = 400; Check(SquashFusions.Animation(ice).Clip == "lookright", "right original look");
ice.mState = PlantState.SquashPreLaunch; ice.mStateCountdown = 0;
Check(SquashFusions.Animation(ice) == ("jump", .55f), "source movement event boundary");
ice.mState = PlantState.SquashFalling; ice.mStateCountdown = 5;
Check(Math.Abs(SquashFusions.Animation(ice).Time - 1f/3) < .00001f, "native hit matches original AttackZombie event");
ice.mState = PlantState.SquashDoneFalling; ice.mStateCountdown = 0;
Check(SquashFusions.Animation(ice).Time < 1.5f, "completed clip never loops to start");

// New variants exercise production callbacks and the actual native HP writers.
foreach (var (left, right, expected) in new (FusionPlant, FusionPlant, FusionPlant)[] {
    (SeedType.Garlic, SeedType.Wallnut, FusionPlant.FoulNut),
    (SeedType.Starfruit, SeedType.Wallnut, FusionPlant.StarNut),
    (SeedType.Peashooter, SeedType.Squash, FusionPlant.PeaSquash),
    (SeedType.Squash, FusionPlant.Bucket, FusionPlant.IronSquash) })
{
    Check(FusionRecipes.TryGet(left, right, out var result) && result == expected
        && FusionRecipes.TryGet(right, left, out result) && result == expected, "all source recipes in both orders");
    var p = MakePlant(expected);
    Check(FusionPlant.From(p) == expected && FusionPlant.HasImportedVisuals(expected), "new identity reload and original art");
    RetaliationInitializePatch.Postfix(p);
    Check(p.mPlantHealth == (expected == FusionPlant.IronSquash ? 1000 : expected == FusionPlant.PeaSquash ? 300 : 4000),
        "variant health initialization");
}
var peaSquash = MakePlant(FusionPlant.PeaSquash);
SquashFusions.AfterLanding(SquashFusions.Capture(peaSquash));
var burst = peaSquash.mBoard.Projectiles;
Check(burst.Count == 108, "three complete 36-pea rings at landing");
Check(burst.All(p => p.mMotionType == ProjectileMotion.Star && RetaliationProjectiles.IsPea(p)
    && p.mDamageRangeFlags == DamageRangeFlags.Ground), "native radial flight, ground collision and ordinary pea damage");
foreach (float speed in new[] { 4f, 4.8f, 5.6f })
    Check(burst.Count(p => Math.Abs(MathF.Sqrt(p.mVelX*p.mVelX+p.mVelY*p.mVelY)-speed)<.001f) == 36,
        "each original ring speed occurs 36 times");
var spore = new Projectile { mMotionType=ProjectileMotion.Star, mProjectileType=ProjectileType.Pea, mCobTargetX=-40030 };
Check(!RetaliationProjectiles.IsPea(spore), "gutsy spores retain their own original artwork instead of radial pea artwork");
for (int contact = 0; contact < 3; ++contact)
{
    var glacier = new Projectile { mMotionType=ProjectileMotion.Star, mProjectileType=ProjectileType.Pea, mCobTargetX=-614000-contact };
    Check(!RetaliationProjectiles.IsPea(glacier), "glacier keeps its original artwork after each contact");
}
var vertical = burst[0]; vertical.mPosY = 651;
RetaliationProjectileBoundsPatch.Postfix(vertical);
Check(vertical.mDead, "vertical peas cannot escape indefinitely");
var flame = burst[1]; flame.mProjectileType = ProjectileType.Fireball; flame.mPosY = -81;
RetaliationProjectileBoundsPatch.Postfix(flame);
Check(flame.mDead, "Torchwood conversion keeps radial lifetime bounds");
var ironSquash = MakePlant(FusionPlant.IronSquash);
RetaliationInitializePatch.Postfix(ironSquash);
SquashFusionLandingPatch.Prefix(ironSquash, out var savedLanding);
int crushDamage = 1800;
Check(HypnoSquashCrushPatch.Prefix(Add(ironSquash), ref crushDamage, flags) && crushDamage == 3240,
    "iron modifies actual crushing damage");
crushDamage = 20;
HypnoSquashCrushPatch.Prefix(Add(ironSquash), ref crushDamage, 0);
Check(crushDamage == 20, "unrelated damage stays unchanged in landing scope");
SquashFusionLandingPatch.Postfix(ironSquash);
Check(ironSquash.mBoard.Projectiles.Count == 0, "iron landing does not gain fire or pea effects");
SquashFusionLandingPatch.Finalizer(savedLanding);
RetaliationNuts.JackBoard = ironSquash.mBoard.Pointer;
RetaliationNuts.RadiusBoard = ironSquash.mBoard.Pointer;
ironSquash.mBoard.mPlantsEaten = 1;
Check(!RetaliationDeathPatch.Prefix(ironSquash, true) && ironSquash.mPlantHealth == 100,
    "first real jack explosion inflicts 900 and preserves iron squash");
Check(ironSquash.mBoard.mPlantsEaten == 0, "surviving iron squash is not counted as eaten");
Check(RetaliationDeathPatch.Prefix(ironSquash, true) && ironSquash.mPlantHealth == -800,
    "second explosion remains lethal");
RetaliationNuts.JackBoard = RetaliationNuts.RadiusBoard = IntPtr.Zero;
var star = MakePlant(FusionPlant.StarNut);
RetaliationInitializePatch.Postfix(star);
star.mBoard.m_plants.Add(star);
var biter = Add(star);
RetaliationBitePatch.Prefix(biter, star, out var biteState);
star.mPlantHealth -= 50;
RetaliationBitePatch.Postfix(biter, biteState);
RetaliationBitePatch.Finalizer(biteState);
Check(star.mBoard.Projectiles.Count == 5, "one full source bite emits exactly five stars");
var stars = star.mBoard.Projectiles;
Check(stars.Count(p => Math.Abs(p.mVelX) < .001f) == 2
    && stars.Count(p => p.mVelX < -4) == 1 && stars.Count(p => p.mVelX > 0.1f) == 2,
    "five source star directions and faster rear shot");
Check(RetaliationDeathPatch.Prefix(star, true) && stars.Count == 5, "healthy noncombat removal emits nothing");
var enemyPea = new Projectile { mBoard=star.mBoard, mProjectileType=ProjectileType.ZombiePea };
RetaliationProjectileHitPatch.Prefix(enemyPea, out var hitState);
star.mPlantHealth -= 20;
RetaliationProjectileHitPatch.Postfix(hitState);
Check(stars.Count == 10, "zombie pea health writer also retaliates");
RetaliationProjectileHitPatch.Prefix(enemyPea, out hitState);
RetaliationProjectileHitPatch.Postfix(hitState);
Check(stars.Count == 10, "missed projectile emits nothing");
var basketball = new Projectile { mBoard=star.mBoard, mProjectileType=ProjectileType.Basketball };
RetaliationProjectileHitPatch.Prefix(basketball, out hitState);
star.mPlantHealth = -1;
RetaliationProjectileHitPatch.Postfix(hitState);
Check(stars.Count == 15, "lethal basketball hit retaliates");
Check(RetaliationDeathPatch.Prefix(star, true) && stars.Count == 15, "delayed native death cannot double retaliation");
star.mPlantHealth = 4000;
RetaliationSquishScope.Prefix(star, out var squashHit);
star.mSquished = true;
RetaliationSquishScope.Postfix(star, squashHit);
Check(stars.Count == 20, "native squish transition retaliates once");
RetaliationSquishScope.Prefix(star, out squashHit);
RetaliationSquishScope.Postfix(star, squashHit);
Check(stars.Count == 20, "already squished nut does not retaliate again");
star.mSquished = false;
RetaliationNuts.RadiusBoard = star.mBoard.Pointer;
PlayerPlantingScope.ConsumingIngredient = star.Pointer;
RetaliationDeathPatch.Prefix(star, true);
Check(stars.Count == 20, "fusion consumption cannot retaliate even inside combat context");
PlayerPlantingScope.ConsumingIngredient = IntPtr.Zero;
RetaliationDeathPatch.Prefix(star, false);
Check(stars.Count == 20, "suppressed native death cannot retaliate");
RetaliationDeathPatch.Prefix(star, true);
Check(stars.Count == 25, "actual explosion removal retaliates");
RetaliationNuts.RadiusBoard = IntPtr.Zero;
var foul = MakePlant(FusionPlant.FoulNut);
RetaliationInitializePatch.Postfix(foul);
var foulEater = Add(foul);
RetaliationBitePatch.Prefix(foulEater, foul, out biteState);
foul.mPlantHealth -= 50;
RetaliationBitePatch.Postfix(foulEater, biteState);
RetaliationBitePatch.Finalizer(biteState);
Check(PoisonShooters.Bites == 1 && foulEater.mYuckyFaceCounter == 1, "real foul bite poisons and starts native row change");
RetaliationBitePatch.Prefix(foulEater, foul, out biteState);
RetaliationBitePatch.Postfix(foulEater, biteState);
RetaliationBitePatch.Finalizer(biteState);
Check(PoisonShooters.Bites == 1, "failed bite has no poison or row change");
RetaliationBitePatch.Prefix(foulEater, foul, out biteState);
foul.mPlantHealth -= 50; foulEater.DataID++;
RetaliationBitePatch.Postfix(foulEater, biteState);
RetaliationBitePatch.Finalizer(biteState);
Check(PoisonShooters.Bites == 1, "recycled eater cannot inherit a bite");
Check(RetaliationNuts.CurrentBite == null, "bite context restored");

var jack = new Zombie { mBoard = ironSquash.mBoard, mZombieType = ZombieType.JackInTheBox };
IronSquashExplosionScope.Prefix(jack, out var oldJack);
Check(RetaliationNuts.JackBoard == ironSquash.mBoard.Pointer, "jack and inlined UpdateActions share explosion scope");
var normalZombie = new Zombie { mBoard = ironSquash.mBoard, mZombieType = ZombieType.Normal };
IronSquashExplosionScope.Prefix(normalZombie, out var nestedJack);
Check(RetaliationNuts.JackBoard == IntPtr.Zero, "ordinary nested zombie clears iron resistance context");
IronSquashExplosionScope.Finalizer(nestedJack);
Check(RetaliationNuts.JackBoard == ironSquash.mBoard.Pointer, "nested failure restores jack context");
IronSquashExplosionScope.Finalizer(oldJack);
Check(RetaliationNuts.JackBoard == IntPtr.Zero, "jack context is always cleared");

var fireHead = new Zombie { mBoard = star.mBoard, mZombieType = ZombieType.JalapenoHead };
StarNutFireScope.Prefix(fireHead, out var oldFire);
int starsBeforeFire = stars.Count;
RetaliationDeathPatch.Prefix(star, true);
Check(stars.Count == starsBeforeFire + 5, "fire row removal retaliates through actual and inlined fire paths");
StarNutFireScope.Finalizer(oldFire);
Check(RetaliationNuts.FireBoard == IntPtr.Zero, "fire context restored");

foreach (var (first, second, expected) in new[] {
    (SeedType.Squash, SeedType.Starfruit, FusionPlant.StarrySquash),
    (SeedType.Squash, SeedType.Sunflower, FusionPlant.SolarSquash),
    (SeedType.Tanglekelp, SeedType.Jalapeno, FusionPlant.SpicyKelp),
    (SeedType.Tanglekelp, SeedType.Doomshroom, FusionPlant.DoomKelp) })
{
    Check(FusionRecipes.TryGet(first, second, out var result) && result == expected, "new forward recipe");
    Check(FusionRecipes.TryGet(second, first, out result) && result == expected, "new reverse recipe");
    Check(FusionPlant.From(MakePlant(expected)) == expected && FusionPlant.HasImportedVisuals(expected), "new saved identity and visuals");
}
var starry = MakePlant(FusionPlant.StarrySquash);
SquashFusionLandingPatch.Prefix(starry, out var oldStarry);
crushDamage = 1800;
HypnoSquashCrushPatch.Prefix(Add(starry), ref crushDamage, flags);
Check(crushDamage == 900, "central starry crush is halved");
SquashFusionLandingPatch.Finalizer(oldStarry);
var leftStar = Add(starry); leftStar.Rect = new(110, 200, 130, 280);
var rightStar = Add(starry); rightStar.Rect = new(350, 200, 370, 280);
var farStar = Add(starry); farStar.Rect = new(700, 200, 720, 280);
var airStar = Add(starry); airStar.Category = DamageRangeFlags.Flying;
SquashFusions.AfterLanding(SquashFusions.Capture(starry));
Check(leftStar.Damage == 1800 && rightStar.Damage == 1800, "both diagonal circles can overlap the same zombie");
Check(farStar.Damage == 0 && airStar.Damage == 0, "starry range and airborne exclusions");
Check(starry.mBoard.Projectiles.Count == 0, "starry creates no invented projectiles");
var solarSquash = MakePlant(FusionPlant.SolarSquash);
SolarSquash.Initialize(solarSquash);
Check(solarSquash.mLaunchCounter == 1, "first return costs fifty");
solarSquash.mSquished = true;
Check(SolarSquash.Capture(solarSquash, true) != null, "crushing does not suppress solar return");
solarSquash.mSquished = false;
var solarBoard = solarSquash.mBoard; solarBoard.Sun = 200;
var solarDeath = SolarSquash.Capture(solarSquash, true)!;
Check(solarDeath.Col == 3, "return uses landing position instead of original planting column");
SolarSquash.OnDeath(solarDeath);
Check(solarBoard.Sun == 150 && solarBoard.Created == 1 && solarBoard.LastCreated!.mLaunchCounter == 2, "first paid return");
SolarSquash.OnDeath(solarDeath with { Generation = 2 });
Check(solarBoard.Sun == 75 && solarBoard.LastCreated!.mLaunchCounter == 3, "second paid return");
solarBoard.FailSpawn = 1;
SolarSquash.OnDeath(solarDeath);
Check(solarBoard.Sun == 75 && solarBoard.Coins == 2, "failed creation refunds reserved cost plus final fifty-sun coins");
solarBoard.ThrowSpawn = true;
try { SolarSquash.OnDeath(solarDeath); throw new Exception("expected spawn failure"); }
catch (InvalidOperationException) { }
Check(solarBoard.Sun == 75, "exception refunds reserved cost"); solarBoard.ThrowSpawn = false;
solarBoard.Sun = 0;
SolarSquash.OnDeath(solarDeath);
Check(solarBoard.Coins == 4, "insufficient sun yields final fifty");
solarBoard.Sun = 200;
SolarSquashShovelScope.Prefix(solarBoard, CursorType.Shovel, out var oldShovel);
var shoveled = SolarSquash.Capture(solarSquash, true)!;
SolarSquashShovelScope.Finalizer(oldShovel);
SolarSquash.OnDeath(shoveled);
Check(solarBoard.Sun == 200 && solarBoard.Coins == 6, "shoveling never resurrects or charges");
Check(SolarSquash.ShovelingBoard == IntPtr.Zero, "shovel context restored");
PlayerPlantingScope.ConsumingIngredient = solarSquash.Pointer;
Check(SolarSquash.Capture(solarSquash, true) == null, "fusion consumption never resurrects");
PlayerPlantingScope.ConsumingIngredient = IntPtr.Zero;
CherryBombPlants.DisposingBoard = solarBoard.Pointer;
Check(SolarSquash.Capture(solarSquash, true) == null, "teardown never resurrects");
CherryBombPlants.DisposingBoard = IntPtr.Zero;
Check(SolarSquash.Capture(solarSquash, false) == null, "deferred death never resurrects");
SolarSquashDeathPatch.Prefix(solarSquash, true, out var capturedSolar);
solarSquash.mDead = true; ++solarSquash.DataID;
SolarSquashDeathPatch.Postfix(solarSquash, capturedSolar);
Check(solarBoard.Coins == 6, "pooled solar plant rejects stale death");
foreach (var kelpIdentity in new[] { FusionPlant.SpicyKelp, FusionPlant.DoomKelp })
{
    var kelp = MakePlant(kelpIdentity);
    kelp.mSquished = true;
    Check(KelpFusions.Capture(kelp, true) != null, "crushing does not suppress kelp death effect");
    kelp.mSquished = false;
    var nearKelp = Add(kelp); nearKelp.mIceTrapCounter = 300;
    var kelpOtherRow = Add(kelp); kelpOtherRow.mRow = 1;
    var distantKelp = Add(kelp); distantKelp.Rect = new(700, 200, 720, 280);
    var friendlyKelp = Add(kelp); friendlyKelp.mMindControlled = true;
    KelpFusionDeathPatch.Prefix(kelp, true, out var capturedKelp);
    kelp.mDead = true;
    KelpFusionDeathPatch.Postfix(kelp, capturedKelp);
    Check(nearKelp.Damage == 1800, "kelp death damage");
    Check(kelpOtherRow.Damage == (kelpIdentity == FusionPlant.DoomKelp ? 1800 : 0), "kelp row versus radial effect");
    Check(distantKelp.Damage == (kelpIdentity == FusionPlant.SpicyKelp ? 1800 : 0), "kelp radius-five bound versus full row");
    Check(friendlyKelp.Damage == 0, "kelp never hits friendly zombies");
    Check(nearKelp.ColdRemovals == (kelpIdentity == FusionPlant.SpicyKelp ? 1 : 0), "only spicy kelp removes cold");
    kelp.mDead = false;
    PlayerPlantingScope.ConsumingIngredient = kelp.Pointer;
    Check(KelpFusions.Capture(kelp, true) == null, "kelp fusion consumption suppresses death effect");
    PlayerPlantingScope.ConsumingIngredient = IntPtr.Zero;
    Check(KelpFusions.Capture(kelp, false) == null, "deferred kelp death suppressed");
    KelpFusionDeathPatch.Prefix(kelp, true, out capturedKelp);
    kelp.mDead = true; ++kelp.DataID;
    KelpFusionDeathPatch.Postfix(kelp, capturedKelp);
    Check(nearKelp.Damage == 1800, "pooled kelp rejects stale death");
}
// New variants: exercise the real damage/death/target callbacks with offline doubles.
foreach (var identity in new[] { FusionPlant.DoomSquash, FusionPlant.Jawbreaker, FusionPlant.DoomChomper, FusionPlant.Frostbiter })
{
    var p = MakePlant(identity);
    Check(FusionPlant.From(p) == identity && FusionPlant.HasImportedVisuals(identity), "new saved pair and original model");
    Check(FusionRecipes.TryGet(identity.NativeType, identity.Marker, out var r) && r == identity,
        "new recipe available in forward order");
    Check(FusionRecipes.TryGet(identity.Marker, identity.NativeType, out r) && r == identity,
        "new recipe available in reverse order");
}
var doom = MakePlant(FusionPlant.DoomSquash);
var doomNear = Add(doom, 500); var doomFar = Add(doom, 560);
var doomFriend = Add(doom); doomFriend.mMindControlled = true;
var doomedSupport = MakePlant(SeedType.Wallnut); doomedSupport.mBoard = doom.mBoard;
doomedSupport.mPlantCol = 3; doomedSupport.mRow = doom.mRow;
var safePlant = MakePlant(SeedType.Wallnut); safePlant.mBoard = doom.mBoard;
safePlant.mPlantCol = 4; safePlant.mRow = doom.mRow;
doom.mBoard.m_plants.AddRange(new[] { doomedSupport, safePlant });
SquashFusions.AfterLanding(SquashFusions.Capture(doom));
Check(doomNear.Damage == 1800 && doomFar.Damage == 0 && doomFriend.Damage == 0, "doom five-world-unit radius and target guards");
Check(doomedSupport.mDead && !safePlant.mDead, "crater destroys destination stack only");
Check(doom.mBoard.m_gridItems.Count == 1 && doom.mBoard.m_gridItems[0].mGridItemCounter == 18000,
    "saved 180-second native crater");
DoomEffects.Explode(doom.mBoard, 3, 2, 240, 240, 0);
Check(doom.mBoard.m_gridItems.Count == 1, "existing crater refreshed without duplicate");
var jaw = MakePlant(FusionPlant.Jawbreaker); jaw.mBoard.m_plants.Add(jaw);
JawbreakerInitializePatch.Postfix(jaw);
var food = Add(jaw, 300); var secondFood = Add(jaw, 310);
SquashFusionLandingPatch.Prefix(jaw, out var jawOuter);
try
{
    Check(SquashFusions.ConvertCrush(food, 1800, flags) && food.mDead && jaw.mLaunchCounter == 1,
        "jawbreaker swallows one and saves return eligibility");
    Check(!SquashFusions.ConvertCrush(secondFood, 1800, flags) && !secondFood.mDead,
        "later landing targets retain ordinary crush");
}
finally { SquashFusionLandingPatch.Finalizer(jawOuter); }
var jawReturn = Jawbreaker.Capture(jaw, true);
Check(jawReturn?.Col == 3, "jawbreaker returns at landing column");
jaw.mDead = true; JawbreakerDeathPatch.Postfix(jaw, jawReturn);
Check(jaw.mBoard.LastCreated?.mImitaterType == SeedType.Chomper, "jawbreaker recreates saved variant");
jaw.mDead = false; SolarSquash.ShovelingBoard = jaw.mBoard.Pointer;
Check(Jawbreaker.Capture(jaw, true) == null, "shovel never returns jawbreaker");
SolarSquash.ShovelingBoard = IntPtr.Zero;
PlayerPlantingScope.ConsumingIngredient = jaw.Pointer;
Check(Jawbreaker.Capture(jaw, true) == null, "fusion consumption never returns jawbreaker");
PlayerPlantingScope.ConsumingIngredient = IntPtr.Zero;
jaw.mX = 640;
Check(Jawbreaker.Capture(jaw, true) == null, "source excludes last-column return");
foreach (var type in new[] { ZombieType.Gargantuar, ZombieType.RedeyeGargantuar, ZombieType.Boss,
    ZombieType.Target, ZombieType.Gravestone })
{
    food.mDead = false; food.mZombieType = type;
    Check(!ChomperVariants.CanSwallow(food), "unswallowable targets retain crush");
}
foreach (var type in new[] { ZombieType.Zamboni, ZombieType.Catapult, ZombieType.Bobsled })
{
    food.mDead = false; food.mZombieType = type;
    Check(ChomperVariants.CanSwallow(food), "ordinary vehicles retain native swallowing");
}
var doomMouth = MakePlant(FusionPlant.DoomChomper); var doomVictim = Add(doomMouth, 400);
doomMouth.mState = PlantState.ChomperBitingGotOne;
ChomperVariants.AfterNative(doomMouth, PlantState.ChomperBiting);
Check(doomVictim.Damage == 1800 && doomMouth.mBoard.m_gridItems.Single().mGridX == 1,
    "doom chomper explosion one column ahead only after successful swallow");
ChomperVariants.AfterNative(doomMouth, PlantState.Ready);
Check(doomVictim.Damage == 1800, "idle transition never repeats explosion");
doomMouth.mState = PlantState.ChomperDigesting;
ChomperVariants.AfterNative(doomMouth, PlantState.ChomperBitingGotOne);
Check(doomMouth.mStateCountdown == 6000, "doom chomper saves sixty-second digestion");
var frostMouth = MakePlant(FusionPlant.Frostbiter); var warmFood = Add(frostMouth);
frostMouth.mState = PlantState.ChomperBiting; frostMouth.mStateCountdown = 0;
Zombie targetFood = warmFood;
FrostbiterTargetPatch.Postfix(frostMouth, ref targetFood);
Check(targetFood == null && !warmFood.mDead, "unfrozen target cannot be swallowed");
frostMouth.mState = PlantState.Ready; targetFood = warmFood;
FrostbiterTargetPatch.Postfix(frostMouth, ref targetFood);
Check(targetFood == warmFood, "unfrozen target still starts bite animation");
frostMouth.mState = PlantState.ChomperBiting; warmFood.mIceTrapCounter = 100; targetFood = warmFood;
FrostbiterTargetPatch.Postfix(frostMouth, ref targetFood);
Check(targetFood == warmFood, "frozen target remains native swallow candidate");
warmFood.mIceTrapCounter = 0;
ChomperVariants.FrostBite(frostMouth, false);
Check(warmFood.Damage == 40 && SnowPea.Points.GetValueOrDefault(warmFood.Pointer) == 51
    && warmFood.mChilledCounter == 1500, "failed frost bite: forty damage, fifty-one source points, fifteen-second chill");
var farFrost = Add(frostMouth, 850);
ChomperVariants.FrostBite(frostMouth, true);
Check(farFrost.Damage == 20 && farFrost.mIceTrapCounter == 100, "successful swallow freezes entire board for one second");
Check(warmFood.Damage == 100, "source successful bite also emits local forty-damage pulse");
farFrost.mIceTrapCounter = 900; ChomperVariants.Freeze(farFrost, 100);
Check(farFrost.mIceTrapCounter == 900, "one-second effect preserves longer freeze");
Check(SnowPea.CreatingIceTrap == (IntPtr)777, "new freeze restores outer effect context");
var frostRange = new UnityEngine.Rect { width = 40 };
ChomperVariantsRangePatch.Postfix(frostMouth, ref frostRange);
Check(Math.Abs(frostRange.width - 3 * 80f / 1.35f) < .001f
    && Math.Abs(frostRange.x - (240 + 80f / 1.35f)) < .001f, "frostbiter uses source cast origin and three-unit range");
frostMouth.mState = PlantState.ChomperBiting;
ChomperVariantsAbilitiesPatch.Prefix(frostMouth, true, out var oldBite);
Check(!FrostbiterNativeBiteDamagePatch.Prefix(warmFood, 40, 0), "giant direct bite replaced by local pulse");
Check(FrostbiterNativeBiteDamagePatch.Prefix(warmFood, 41, 0), "unrelated native damage preserved");
ChomperVariantsAbilitiesPatch.Finalizer(oldBite);
Check(ChomperVariants.Current == null, "new synchronous bite context cleared");
Console.WriteLine($"Squash fusions: {count} assertions passed; no game execution.");
