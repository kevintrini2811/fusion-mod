using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool value, string reason) { ++checks; if (!value) throw new Exception(reason); }
Plant Make(FusionPlant identity)
{
    var plant = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    SupportFusions.Initialize(plant);
    return plant;
}
Zombie Enemy(Board board, float x = 100) => new() { mBoard = board, mPosX = x };
void Ticks(Plant plant, int count) { for (int i = 0; i < count; ++i) SupportFusions.Tick(plant); }
var recipes = new (FusionPlant A, FusionPlant B, FusionPlant Result)[] {
    (SeedType.Blover, SeedType.Doomshroom, FusionPlant.FalloutBlover),
    (SeedType.Starfruit, SeedType.Sunflower, FusionPlant.Solarfruit),
    (SeedType.Garlic, SeedType.Hypnoshroom, FusionPlant.HypnoGarlic),
    (SeedType.Blover, SeedType.Chomper, FusionPlant.Flytrapper)
};
foreach (var (a, b, result) in recipes)
{
    Check(FusionRecipes.TryGet(a, b, out var found) && found == result, "source recipe");
    Check(FusionRecipes.TryGet(b, a, out found) && found == result, "reverse recipe");
    var plant = Make(result);
    Check(FusionPlant.From(plant) == result && FusionPlant.HasImportedVisuals(result), "saved identity and art");
    Check(plant.mPlantHealth == 300 && plant.mPlantMaxHealth == 300, "source base health");
    plant.mImitaterType = SeedType.None;
    Check(FusionPlant.From(plant) == (FusionPlant)plant.mSeedType, "ordinary plant stays ordinary");
}

var fallout = Make(FusionPlant.FalloutBlover);
var victim = Enemy(fallout.mBoard); fallout.mBoard.m_zombies.Add(victim);
var ally = Enemy(fallout.mBoard); ally.mMindControlled = true; fallout.mBoard.m_zombies.Add(ally);
var boss = Enemy(fallout.mBoard); boss.mZombieType = ZombieType.Boss; fallout.mBoard.m_zombies.Add(boss);
var balloon = Enemy(fallout.mBoard); balloon.mZombieType = ZombieType.Balloon; fallout.mBoard.m_zombies.Add(balloon);
Ticks(fallout, 48);
Check(victim.Damage == 0 && !fallout.mDead, "startup waits for original blow event");
Ticks(fallout, 1);
Check(victim.Damage == 180 && balloon.Damage == 180 && ally.Damage == 0 && boss.Damage == 0,
    "first pulse includes air, excludes allies and boss");
Check(fallout.mBoard.mFogBlownCountDown == 3000, "original thirty-second fog clear");
Ticks(fallout, 999);
Check(!fallout.mDead && victim.Damage == 1800, "ten pulses before final event");
Ticks(fallout, 1);
Check(fallout.mDead && victim.Damage == 1980 && fallout.mTargetX == 10, "initial plus ten timed pulses, then death");
Ticks(fallout, 1000);
Check(victim.Damage == 1980, "dead plant never repeats effects");
var interrupted = Make(FusionPlant.FalloutBlover); interrupted.mLaunchCounter = 1;
var oldTarget = Enemy(interrupted.mBoard); var reused = Enemy(interrupted.mBoard);
interrupted.mBoard.m_zombies.AddRange(new[] { oldTarget, reused });
oldTarget.OnDamage = () => ++reused.DataID;
Ticks(interrupted, 1);
Check(oldTarget.Damage == 180 && reused.Damage == 0, "pool reuse during pulse skips stale target");

foreach (var identity in new[] { FusionPlant.FalloutBlover, FusionPlant.Solarfruit, FusionPlant.HypnoGarlic })
{
    foreach (string stop in new[] { "pause", "carry", "bungee", "dead", "sleep", "loading", "dispose" })
    {
        var plant = Make(identity); plant.mLaunchCounter = 20; plant.mStartRow = 10;
        if (stop == "pause") plant.mBoard.mPaused = true;
        if (stop == "carry") plant.OffGround = true;
        if (stop == "bungee") plant.mOnBungeeState = PlantOnBungeeState.OnBungee;
        if (stop == "dead") plant.mDead = true;
        if (stop == "sleep") plant.mIsAsleep = true;
        if (stop == "loading") plant.mApp.GameScene = GameScenes.Loading;
        if (stop == "dispose") CherryBombPlants.DisposingBoard = plant.mBoard.Pointer;
        Ticks(plant, 50);
        Check(plant.mLaunchCounter == 20 && plant.mStartRow == 10, "inactive timers stop: " + stop);
        CherryBombPlants.DisposingBoard = IntPtr.Zero;
    }
}

var solar = Make(FusionPlant.Solarfruit);
solar.mBoard.Sun = 25;
Check(!SupportFusions.ShootSolar(solar) && solar.mBoard.Sun == 25, "actual source requires strictly more than 25 sun");
solar.mBoard.Sun = 26;
Check(SupportFusions.ShootSolar(solar) && solar.mBoard.Sun == 1, "one paid shot");
Check(!SupportFusions.ShootSolar(solar) && solar.mBoard.Takes == 1, "no double spending during animation");
Ticks(solar, 20);
Check(solar.mBoard.Shots.Count == 0, "wait for speed-two original animation event");
Ticks(solar, 1);
Check(solar.mBoard.Shots.Count == 5 && solar.mBoard.Shots.All(SupportFusions.IsSolarShot), "five marked stars");
Check(solar.mBoard.Shots.Select(p => p.Angle).SequenceEqual(new[] { 30f, -30f, 180f, 90f, -90f }), "source directions");
Ticks(solar, 100);
Check(solar.mBoard.Shots.Count == 5, "no automatic targeting without accessory");
solar.mBoard.Sun = 100; solar.mBoard.FailTakeSun = true;
Check(!SupportFusions.ShootSolar(solar) && solar.mStartRow == 0, "failed payment creates no attack");
solar.mBoard.FailTakeSun = false;
foreach (var cursor in new[] { CursorType.Shovel, CursorType.Glove })
{
    solar.mBoard.mCursorObject.CursorType = cursor;
    Check(SolarfruitClickPatch.Prefix(solar, 1, 0) && solar.mStartRow == 0, "tool click does not fire");
}
solar.mBoard.mCursorObject.CursorType = CursorType.Normal;
solar.mBoard.Carried = new();
Check(SolarfruitClickPatch.Prefix(solar, 1, 0), "carried plant blocks click ability");
solar.mBoard.Carried = null;
Check(SolarfruitClickPatch.Prefix(solar, -1, 0) && SolarfruitClickPatch.Prefix(solar, 1, 1), "release and other player ignored");
Check(!SolarfruitClickPatch.Prefix(solar, 1, 0) && solar.mBoard.Sun == 75, "normal click fires");

var automatic = Make(FusionPlant.Solarfruit);
automatic.mBoard.Lawn.PumpkinPlant = Make(FusionPlant.StarryPumpkin);
Ticks(automatic, 1);
Check(automatic.mBoard.Takes == 1, "conditional Starry Pumpkin synergy");
Ticks(automatic, 35);
Check(automatic.mBoard.Takes == 2 && automatic.mBoard.Shots.Count == 5, "automatic cycle costs sun every .35 seconds");
automatic.mBoard.Lawn.PumpkinPlant!.mDead = true;
Ticks(automatic, 100);
Check(automatic.mBoard.Takes == 2 && automatic.mBoard.Shots.Count == 10, "removing accessory ends auto fire, paid shot completes");

// Copy the saved fields to a new pool occupant; resume without initialization.
var loaded = Make(FusionPlant.Solarfruit);
loaded.mStartRow = 14; loaded.mLaunchCounter = 15;
Ticks(loaded, 1);
Check(loaded.mBoard.Shots.Count == 5, "saved windup resumes at the single event");
Ticks(loaded, 30);
Check(loaded.mBoard.Shots.Count == 5, "saved event never repeats");
var shot = loaded.mBoard.Shots[0]; var hit = Enemy(loaded.mBoard);
SolarfruitImpactPatch.Prefix(shot, hit, out var previous);
int damage = 20; SolarfruitDamagePatch.Prefix(hit, ref damage);
Check(damage == 100, "loaded marker supplies 100 damage");
damage = 20; SolarfruitDamagePatch.Prefix(hit, ref damage);
Check(damage == 20, "consume one actual damage write");
SolarfruitImpactPatch.Finalizer(previous);
SolarfruitImpactPatch.Prefix(shot, hit, out previous);
var ordinaryShot = new Projectile { mBoard = loaded.mBoard, mProjectileType = ProjectileType.Star };
SolarfruitImpactPatch.Prefix(ordinaryShot, hit, out var nested);
damage = 20; SolarfruitDamagePatch.Prefix(hit, ref damage);
Check(damage == 20, "ordinary nested impact has no solar damage");
SolarfruitImpactPatch.Finalizer(nested);
damage = 20; SolarfruitDamagePatch.Prefix(hit, ref damage);
Check(damage == 100, "nested finalizer restores solar impact");
SolarfruitImpactPatch.Finalizer(previous);
SolarfruitImpactPatch.Prefix(shot, hit, out previous); ++hit.DataID;
damage = 20; SolarfruitDamagePatch.Prefix(hit, ref damage);
Check(damage == 20, "pooled victim cannot inherit damage context");
SolarfruitImpactPatch.Finalizer(previous);

var garlic = Make(FusionPlant.HypnoGarlic); garlic.mPlantHealth = 1;
var biter = Enemy(garlic.mBoard);
garlic.mBoard.FailCreate = true;
Check(SupportFusions.BiteGarlic(garlic, biter) && !garlic.mDead && !biter.mMindControlled, "failed replacement retains ingredient and effect");
garlic.mBoard.FailCreate = false;
Check(SupportFusions.BiteGarlic(garlic, biter) && garlic.mDead && biter.mMindControlled,
    "successful bite hypnotizes after replacement allocation");
Check(garlic.mBoard.Created is { mSeedType: SeedType.Garlic, mPlantHealth: 300, mPlantMaxHealth: 300 }, "fresh ordinary Garlic");
Check(biter.mYuckyFace && biter.mYuckyFaceCounter == 169 && biter.Stops == 1, "native saved row-change event armed");
Check(SupportFusions.BiteGarlic(garlic, biter) && garlic.mBoard.Allocations == 2, "dead plant cannot repeat replacement");
var contact = Make(FusionPlant.HypnoGarlic); var friend = Enemy(contact.mBoard, 70); friend.mMindControlled = true;
var hostile = Enemy(contact.mBoard, 70); var far = Enemy(contact.mBoard, 500); far.mMindControlled = true;
contact.mBoard.m_zombies.AddRange(new[] { friend, hostile, far });
Ticks(contact, 49); Check(!friend.mYuckyFace, "contact waits .5 seconds");
Ticks(contact, 1);
Check(friend.mYuckyFace && !hostile.mYuckyFace && !far.mYuckyFace, "only nearby friendly ground contact redirects");
Ticks(contact, 100); Check(friend.Stops == 1, "ongoing row change is not restarted");

var fly = Make(FusionPlant.Flytrapper); var air = Enemy(fly.mBoard, 110); air.mZombieType = ZombieType.Balloon;
fly.mBoard.m_zombies.Add(air);
Check(SupportFusions.FlyTarget(fly) == air, "air can be acquired for native swallow");
var ground = Enemy(fly.mBoard, 100); fly.mBoard.m_zombies.Add(ground);
Check(SupportFusions.FlyTarget(fly) == ground, "nearest eligible target wins");
ground.mRow = 1; Check(SupportFusions.FlyTarget(fly) == air, "other row ignored");
foreach (var excluded in new[] { ZombieType.Boss, ZombieType.Gravestone, ZombieType.Target,
    ZombieType.Zamboni, ZombieType.Catapult, ZombieType.Bungee })
{
    air.mZombieType = excluded;
    Check(SupportFusions.FlyTarget(fly) == null, "source swallow exclusion " + excluded);
}
air.mZombieType = ZombieType.Balloon; air.mMindControlled = true;
Check(SupportFusions.FlyTarget(fly) == null, "friendly air never swallowed");
air.mMindControlled = false; air.mPosX = 500;
Check(SupportFusions.FlyTarget(fly) == null, "original one-unit reach");
bool targetable = true; FlytrapperFlyingPatch.Postfix(fly, ref targetable);
Check(!targetable && SupportTickPatch.Prefix(fly), "flying defense with native Chomper state machine");
Check(!SolarfruitNativeShooterPatch.Prefix(solar) && !FalloutNativeSpecialPatch.Prefix(fallout), "native duplicate effects suppressed");
Console.WriteLine($"Support fusion regressions: {checks} checks passed.");
