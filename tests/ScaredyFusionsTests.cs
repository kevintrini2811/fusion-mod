using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool condition, string message) { ++checks; if (!condition) throw new Exception(message); }
Plant Make(FusionPlant identity)
{
    var p = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    ScaredyFusions.Initialize(p); return p;
}
Zombie Enemy(Plant p, float x = 600, int row = 2)
{
    var z = new Zombie { mBoard = p.mBoard, DataID = 7, Pointer = (IntPtr)7, X = x, mRow = row,
        Category = DamageRangeFlags.Ground };
    p.mBoard.m_zombies.Add(z); return z;
}
void Tick(Plant p, int count) { for (int i = 0; i < count; ++i) ScaredyTickPatch.Prefix(p); }
foreach (var (ingredient, identity) in new[] {
    (SeedType.Peashooter, FusionPlant.SpeedyShroom), (SeedType.Cherrybomb, FusionPlant.BlastyShroom),
    (SeedType.Iceshroom, FusionPlant.ShiveryShroom), (SeedType.Potatomine, FusionPlant.SpuddyShroom) })
{
    Check(FusionRecipes.TryGet(ingredient, SeedType.Scaredyshroom, out var result) && result == identity, "recipe");
    Check(FusionRecipes.TryGet(SeedType.Scaredyshroom, ingredient, out result) && result == identity, "reverse recipe");
    var p = Make(identity);
    Check(FusionPlant.From(p) == identity && FusionPlant.HasImportedVisuals(identity), "save identity and art");
    Check(!ScaredyShooterPatch.Prefix(p) && !ScaredyShootingPatch.Prefix(p) && !ScaredyFirePatch.Prefix(p), "no native duplicate shots");
    Enemy(p); Tick(p, 30); Check(p.mBoard.Projectiles.Count == 0, "source event delay");
    Tick(p, 1); Check(p.mBoard.Projectiles.Count == 1, "exactly one shot at 0.3 seconds");
}
var speedy = Make(FusionPlant.SpeedyShroom); Enemy(speedy); Tick(speedy, 31);
Check(ScaredyProjectiles.Damage(speedy.mBoard.Projectiles[0]) == 21 && speedy.mTargetX == 1, "first speed stack changes damage");
Tick(speedy, 142); Check(speedy.mBoard.Projectiles.Count == 1, "next shot waits for exact accelerated interval");
Tick(speedy, 1); Check(speedy.mBoard.Projectiles.Count == 2, "second shot after 143 ticks");
Tick(speedy, 3000); Check(speedy.mTargetX == 30, "speed capped at 150 percent");
var oldShot = speedy.mBoard.Projectiles[^1]; Enemy(speedy, 250); Tick(speedy, 1);
Check(speedy.mTargetX == 0 && ScaredyProjectiles.Damage(oldShot) == 50, "fear resets plant but not airborne damage");
var blasty = Make(FusionPlant.BlastyShroom); Enemy(blasty, 250); Tick(blasty, 83);
Check(CherryBombPlants.Blasts.Count == 0, "no explosion before source scared event");
var loaded = Make(FusionPlant.BlastyShroom);
loaded.mState = blasty.mState; loaded.mTargetY = blasty.mTargetY; Enemy(loaded, 250);
Tick(loaded, 1); Check(CherryBombPlants.Blasts.SequenceEqual(new[] { 600 }), "saved mid-fear emits remaining explosion");
Tick(loaded, 300); Check(CherryBombPlants.Blasts.Count == 1, "no repeated explosion while crouched");
loaded.mBoard.m_zombies.Clear(); Tick(loaded, 101); Enemy(loaded, 250); Tick(loaded, 84);
Check(CherryBombPlants.Blasts.Count == 2, "next distinct fear transition explodes");
var ice = Make(FusionPlant.ShiveryShroom); var near = Enemy(ice, 250); var far = Enemy(ice, 1000); Tick(ice, 84);
Check(near.mIceTrapCounter == 400 && near.Damage == 20 && far.Damage == 0, "local freeze and damage");
var spuddy = Make(FusionPlant.SpuddyShroom); Enemy(spuddy); Tick(spuddy, 31);
Check(spuddy.mPlantHealth == 225 && ScaredyProjectiles.Damage(spuddy.mBoard.Projectiles[0]) == 600, "early damage costs 75 HP");
Tick(spuddy, 900); Check(spuddy.mDead && spuddy.mBoard.Projectiles.Count == 4, "fourth early shot kills 300 HP plant");
spuddy = Make(FusionPlant.SpuddyShroom); Tick(spuddy, 2000); Enemy(spuddy); Tick(spuddy, 31);
Check(spuddy.mPlantHealth == 300 && ScaredyProjectiles.Damage(spuddy.mBoard.Projectiles[0]) == 60, "expired fuse stops HP cost and damage boost");
foreach (var inactive in new[] { "pause", "sleep", "carry", "dead", "squished", "bungee" })
{
    var p = Make(FusionPlant.SpuddyShroom); Enemy(p);
    if (inactive == "pause") p.mBoard.mPaused = true;
    if (inactive == "sleep") p.mIsAsleep = true;
    if (inactive == "carry") p.mIsOnBoard = false;
    if (inactive == "dead") p.mDead = true;
    if (inactive == "squished") p.mSquished = true;
    if (inactive == "bungee") p.Bungee = true;
    Tick(p, 300); Check(p.mTargetX == 2000 && p.mBoard.Projectiles.Count == 0, inactive + " stops abilities");
}
var shooter = Make(FusionPlant.BlastyShroom); var victim = Enemy(shooter);
var shot = new Projectile { mBoard = shooter.mBoard, Pointer = (IntPtr)44 };
ScaredyProjectiles.Mark(shot, FusionPlant.BlastyShroom, 40);
ScaredyImpactPatch.Prefix(shot, out var savedImpact);
ScaredyProjectiles.SelectTarget(shot, victim); int damage = 20;
ScaredyProjectiles.AdjustDamage(victim, ref damage); Check(damage == 40, "actual native health writer gets variant damage");
damage = 9; ScaredyProjectiles.AdjustDamage(victim, ref damage); Check(damage == 9, "one write per selected target");
ScaredyProjectiles.SelectTarget(shot, victim); ++victim.DataID;
damage = 20; ScaredyProjectiles.AdjustDamage(victim, ref damage); Check(damage == 20, "recycled target excluded");
ScaredyImpactPatch.Finalizer(savedImpact);
Check(ScaredyProjectiles.CurrentImpact == null && !ScaredyProjectiles.Pending, "impact scope restored");
Check(!ScaredyTorchwoodPatch.Prefix(shot), "spores do not ignite");
ScaredyProjectiles.Mark(shot, FusionPlant.SpeedyShroom, 50); shot.mProjectileType = ProjectileType.Fireball;
ScaredyImpactPatch.Prefix(shot, out savedImpact); ScaredyProjectiles.SelectTarget(shot, victim);
damage = 40; ScaredyProjectiles.AdjustDamage(victim, ref damage); Check(damage == 70, "Torchwood adds twenty");
ScaredyImpactPatch.Finalizer(savedImpact);
Check(ScaredyTickPatch.Prefix(new Plant()), "ordinary native plant unchanged");
foreach (var (ingredient, identity) in new[] {
    (SeedType.Sunflower, FusionPlant.SunnyShroom), (SeedType.Starfruit, FusionPlant.StarryShroom),
    (SeedType.Wallnut, FusionPlant.SturdyShroom), (SeedType.Blover, FusionPlant.CamoShroom) })
{
    Check(FusionRecipes.TryGet(ingredient, SeedType.Scaredyshroom, out var result) && result == identity, "new recipe");
    Check(FusionRecipes.TryGet(SeedType.Scaredyshroom, ingredient, out result) && result == identity, "new reverse recipe");
    var p = Make(identity);
    Check(FusionPlant.From(p) == identity && FusionPlant.HasImportedVisuals(identity), "new saved identity and original art");
    Check(!ScaredyShooterPatch.Prefix(p) && !ScaredyShootingPatch.Prefix(p) && !ScaredyFirePatch.Prefix(p), "new native firing suppressed");
    Tick(p, 100);
    Check(identity == FusionPlant.StarryShroom || p.mBoard.Projectiles.Count == 0, "support variants never shoot");
}
var starry = Make(FusionPlant.StarryShroom);
Tick(starry, 25); Check(starry.mBoard.Projectiles.Count == 0, "star event waits .25 s without target");
Tick(starry, 1); Check(starry.mBoard.Projectiles.Count == 5, "star event creates five original star projectiles");
Check(starry.mBoard.Projectiles.All(p => p.mProjectileType == ProjectileType.Star && p.mMotionType == ProjectileMotion.Star), "native star damage, collision and saved velocity");
Check(starry.mBoard.Projectiles.Count(p => p.mVelX > 0.1f) == 2 && starry.mBoard.Projectiles.Count(p => p.mVelX < -0.1f) == 1,
    "five source directions, including two vertical rays");
Tick(starry, 74); Check(starry.mBoard.Projectiles.Count == 5, "star interval does not fire early");
Tick(starry, 1); Check(starry.mBoard.Projectiles.Count == 10, "star interval .75 s");
Enemy(starry, 250); Tick(starry, 200); Check(starry.mBoard.Projectiles.Count == 10, "fear cancels star salvos");
var sunny = Make(FusionPlant.SunnyShroom);
Tick(sunny, 550); Check(sunny.mShootingCounter == 360 && sunny.mBoard.Suns == 0, "source initial production starts brightening");
Tick(sunny, 299); Check(sunny.mBoard.Suns == 0, "sun waits for brightness maximum");
Tick(sunny, 1); Check(sunny.mBoard.Suns == 1 && ScaredySupport.Brightness(sunny) == 4, "25 sun at brightness four");
Tick(sunny, 3000); Check(sunny.mTargetX == 2000 && sunny.mBoard.Suns > 1, "production interval reaches ten-second minimum");
Enemy(sunny, 250); Tick(sunny, 1); Check(sunny.mTargetX == 5000, "near enemy resets source interval to 25 seconds");
sunny.mLaunchCounter = 1; sunny.mShootingCounter = 0; Tick(sunny, 301);
Check(sunny.mBoard.Suns > 2 && ScaredyFusions.Afraid(sunny), "production continues while scared");
var sturdy = Make(FusionPlant.SturdyShroom); var close = Enemy(sturdy, 250);
Check(sturdy.mPlantHealth == 4000, "sturdy health");
Tick(sturdy, 1000); Check(close.Damage == 0 && ScaredyFusions.Afraid(sturdy), "ten-second fear threshold");
Tick(sturdy, 50); Check(close.Damage == 0, "source delayed repel");
Tick(sturdy, 1); Check(close.Damage == 20 && close.mPosX > 118 && !ScaredyFusions.Afraid(sturdy), "repel damages armor and body and knocks back two units");
Tick(sturdy, 449); Check(!ScaredyFusions.Afraid(sturdy), "five-second fear immunity");
Tick(sturdy, 1); Check(ScaredyFusions.Afraid(sturdy), "fear resumes after immunity");
// Simulate the actual native bite writer, including its pre-Die eaten counter.
sturdy.mPlantHealth = 3;
Check(ScaredySupportBitePatch.Prefix(sturdy, out var damageScope), "sturdy bite enters health scope");
sturdy.mPlantHealth -= 4; ++sturdy.mBoard.mPlantsEaten;
Check(!SturdyLethalDamagePatch.Prefix(sturdy, true) && sturdy.mPlantHealth == 1 && sturdy.mBoard.mPlantsEaten == 0,
    "half damage prevents nominally lethal bite and restores eaten counter");
ScaredySupportBitePatch.Postfix(damageScope); ScaredySupportBitePatch.Finalizer(damageScope);
Check(sturdy.mPlantHealth == 1 && ScaredySupport.Hits == null, "damage reduction applied once and scope restored");
Check(ScaredySupportBitePatch.Prefix(sturdy, out damageScope), "second bite scope");
sturdy.mPlantHealth -= 4;
Check(SturdyLethalDamagePatch.Prefix(sturdy, true) && sturdy.mPlantHealth == -1, "truly lethal half damage stays lethal");
ScaredySupportBitePatch.Postfix(damageScope); ScaredySupportBitePatch.Finalizer(damageScope);
sturdy.mPlantHealth = 100;
var hostile = new Projectile { mBoard = sturdy.mBoard, mProjectileType = ProjectileType.Basketball };
sturdy.mBoard.m_plants.Add(sturdy);
SturdyProjectileDamagePatch.Prefix(hostile, out damageScope);
sturdy.mPlantHealth -= 75;
SturdyProjectileDamagePatch.Postfix(damageScope); SturdyProjectileDamagePatch.Finalizer(damageScope);
Check(sturdy.mPlantHealth == 63, "odd projectile damage rounds as source integer division");
SturdyProjectileDamagePatch.Prefix(hostile, out damageScope);
var nested = ScaredySupport.Hits;
SturdyProjectileDamagePatch.Prefix(hostile, out var inner);
SturdyProjectileDamagePatch.Postfix(inner); SturdyProjectileDamagePatch.Finalizer(inner);
sturdy.mPlantHealth -= 20;
SturdyProjectileDamagePatch.Postfix(damageScope); SturdyProjectileDamagePatch.Finalizer(damageScope);
Check(sturdy.mPlantHealth == 53 && nested![0].Applied, "nested scope cannot prematurely consume damage observation");
SturdyProjectileDamagePatch.Prefix(hostile, out damageScope);
++sturdy.DataID; sturdy.mPlantHealth = 80;
SturdyProjectileDamagePatch.Postfix(damageScope); SturdyProjectileDamagePatch.Finalizer(damageScope);
Check(sturdy.mPlantHealth == 80, "recycled plant never receives old reduction");
sturdy.mPlantHealth = 4000;
RetaliationNuts.JackBoard = RetaliationNuts.RadiusBoard = sturdy.mBoard.Pointer;
++sturdy.mBoard.mPlantsEaten;
Check(!SturdyLethalDamagePatch.Prefix(sturdy, true) && sturdy.mPlantHealth == 3100,
    "native jack removal becomes 900 damage while scared");
sturdy.mState = PlantState.Notready; ++sturdy.mBoard.mPlantsEaten;
Check(!SturdyLethalDamagePatch.Prefix(sturdy, true) && sturdy.mPlantHealth == 1300,
    "awake sturdy takes full 1800 jack damage");
RetaliationNuts.JackBoard = RetaliationNuts.RadiusBoard = IntPtr.Zero;
var camo = Make(FusionPlant.CamoShroom); Tick(camo, 1000);
Check(camo.mState == PlantState.Notready && camo.mBoard.Projectiles.Count == 0, "camo stays aloft and never attacks");
var protectedPlant = Make(FusionPlant.SpeedyShroom); protectedPlant.mBoard = camo.mBoard;
protectedPlant.mState = PlantState.ScaredyshroomScared; camo.mBoard.m_plants.Add(camo);
Check(ScaredySupport.BiteProtected(protectedPlant), "camo protects nearby scared fusion");
protectedPlant.mImitaterType = SeedType.None;
Check(ScaredySupport.BiteProtected(protectedPlant), "camo protects ordinary native scaredy");
protectedPlant.mPlantCol += 2; Check(!ScaredySupport.BiteProtected(protectedPlant), "aura excludes outside three columns");
protectedPlant.mPlantCol -= 2; camo.mIsAsleep = true;
Check(!ScaredySupport.BiteProtected(protectedPlant), "sleeping camo cannot protect neighbors");
camo.mIsAsleep = false; protectedPlant.mState = PlantState.ScaredyshroomRaising;
Check(!ScaredySupport.BiteProtected(protectedPlant), "aura stops when protected plant rises");
protectedPlant.mState = PlantState.ScaredyshroomScared;
Check(!ScaredySupportBitePatch.Prefix(protectedPlant, out damageScope), "actual bite entry enforces aura");
ScaredySupportBitePatch.Finalizer(damageScope);
bool targetable = true; CamoTargetPatch.Postfix(camo, ZombieAttackType.DriveOver, ref targetable);
Check(!targetable, "flying camo excludes native ground target selection");
targetable = true; CamoTargetPatch.Postfix(protectedPlant, ZombieAttackType.DriveOver, ref targetable);
Check(targetable, "neighbor aura protects only chewing");
foreach (var identity in new[] { FusionPlant.SunnyShroom, FusionPlant.StarryShroom, FusionPlant.SturdyShroom, FusionPlant.CamoShroom })
    foreach (var inactive in new[] { "pause", "sleep", "carry", "dead", "squished", "bungee" })
    {
        var p = Make(identity); Enemy(p, 250);
        if (inactive == "pause") p.mBoard.mPaused = true;
        if (inactive == "sleep") p.mIsAsleep = true;
        if (inactive == "carry") p.mIsOnBoard = false;
        if (inactive == "dead") p.mDead = true;
        if (inactive == "squished") p.mSquished = true;
        if (inactive == "bungee") p.Bungee = true;
        var before = (p.mLaunchCounter, p.mTargetX, p.mTargetY, p.mState);
        Tick(p, 1000);
        Check(before == (p.mLaunchCounter, p.mTargetX, p.mTargetY, p.mState) && p.mBoard.Suns == 0 && p.mBoard.Projectiles.Count == 0,
            identity + " stops " + inactive);
    }
foreach (var identity in new[] { FusionPlant.SunnyShroom, FusionPlant.StarryShroom, FusionPlant.SturdyShroom })
{
    var original = Make(identity);
    if (identity == FusionPlant.SturdyShroom) { Enemy(original, 250); Tick(original, 1030); }
    else if (identity == FusionPlant.SunnyShroom) Tick(original, 849);
    else Tick(original, 15);
    var restored = Make(identity);
    restored.mState = original.mState;
    restored.mLaunchCounter = original.mLaunchCounter;
    restored.mStartRow = original.mStartRow;
    restored.mTargetX = original.mTargetX;
    restored.mTargetY = original.mTargetY;
    restored.mShootingCounter = original.mShootingCounter;
    if (identity == FusionPlant.SturdyShroom) Enemy(restored, 250);
    int sunBefore = original.mBoard.Suns, shotsBefore = original.mBoard.Projectiles.Count;
    Tick(original, 30); Tick(restored, 30);
    Check((original.mState, original.mLaunchCounter, original.mStartRow, original.mTargetX, original.mTargetY, original.mShootingCounter)
        == (restored.mState, restored.mLaunchCounter, restored.mStartRow, restored.mTargetX, restored.mTargetY, restored.mShootingCounter),
        identity + " saved fields resume identical phase");
    Check(original.mBoard.Suns - sunBefore == restored.mBoard.Suns
        && original.mBoard.Projectiles.Count - shotsBefore == restored.mBoard.Projectiles.Count,
        identity + " restored phase produces each remaining effect exactly once");
    if (identity == FusionPlant.SturdyShroom)
        Check(original.mBoard.m_zombies[0].Damage == 20 && restored.mBoard.m_zombies[0].Damage == 20,
            "saved delayed repel resumes once");
}
Console.WriteLine($"Scaredy fusions: {checks} checks passed; no game code loaded.");

var protectedShroom = Make(FusionPlant.ShiveryShroom);
protectedShroom.mBoard.m_plants.Add(new Plant { mBoard = protectedShroom.mBoard,
    mSeedType = SeedType.Pumpkinshell, mImitaterType = SeedType.Scaredyshroom,
    mPlantCol = protectedShroom.mPlantCol, mRow = protectedShroom.mRow });
Enemy(protectedShroom, 250); Tick(protectedShroom, 200);
Check(!ScaredyFusions.Afraid(protectedShroom) && protectedShroom.mBoard.Projectiles.Count > 0,
    "Scareless Pumpkin prevents imported Scaredy fear without disabling shots");
var marked = protectedShroom.mBoard.Projectiles[0];
Check(marked.mDamageRangeFlags == DamageRangeFlags.Ground, "Custom Scaredy shot gets ground target mask");
marked.mDamageRangeFlags = 0; ScaredyProjectileVisualPatch.Prefix(marked);
Check(marked.mDamageRangeFlags == DamageRangeFlags.Ground, "Loaded old shot repaired before native collision");

var protectedNative = new Plant { mBoard = protectedShroom.mBoard, mSeedType = SeedType.Scaredyshroom,
    mPlantCol = protectedShroom.mPlantCol, mRow = protectedShroom.mRow, mState = PlantState.ScaredyshroomScared };
Check(!ScaredyTickPatch.Prefix(protectedNative) && protectedNative.mState == PlantState.Ready,
    "Scareless Pumpkin restores native Scaredy-shroom courage");

// Frenzy must not acquire adjacent-lane targets through the three-row fume helper.
var otherLaneFrenzy = Make(FusionPlant.FrenzyShroom);
var otherLaneEnemy = Enemy(otherLaneFrenzy, 600);
otherLaneEnemy.mRow = otherLaneFrenzy.mRow + 1;
Tick(otherLaneFrenzy, 200);
Check(otherLaneFrenzy.mBoard.Projectiles.Count == 0 && otherLaneFrenzy.mStartRow == 0,
    "Frenzy does not start shooting at another lane");
otherLaneEnemy.mRow = otherLaneFrenzy.mRow;
Tick(otherLaneFrenzy, 100);
Check(otherLaneFrenzy.mBoard.Projectiles.Count > 0, "Frenzy acquires the same enemy after a lane change");

// Fear must not inherit the shooting weapon's damage mask. An enemy changing
// phase in an adjacent lane must cancel the pending shot and retain fear.
foreach (var identity in new[] { FusionPlant.SpeedyShroom, FusionPlant.BlastyShroom,
    FusionPlant.ShiveryShroom, FusionPlant.SpuddyShroom, FusionPlant.StarryShroom,
    FusionPlant.SunnyShroom, FusionPlant.SturdyShroom, FusionPlant.FrenzyShroom })
    foreach (int offset in new[] { -1, 1 })
    {
        var p = Make(identity);
        p.mStartRow = 20;
        var enemy = Enemy(p, p.mX + p.mWidth / 2, p.mRow + offset);
        enemy.Category = DamageRangeFlags.Flying;
        Tick(p, 200);
        Check(ScaredyFusions.Afraid(p) && p.mBoard.Projectiles.Count == 0,
            "Non-ground adjacent threat never resumes a pending shot");
        enemy.mMindControlled = true;
        Tick(p, 1);
        Check(!ScaredyFusions.Afraid(p), "Friendly zombie stops causing fear");
    }
foreach (int offset in new[] { -1, 1 })
{
    var p = Make(FusionPlant.FrenzyShroom);
    p.mTargetX = 20;
    var enemy = Enemy(p, p.mX + p.mWidth / 2, p.mRow + offset);
    enemy.Category = DamageRangeFlags.Flying;
    int craters = DoomEffects.Craters;
    Tick(p, 1);
    Check(p.mDead && DoomEffects.Craters == craters + 1,
        "Charged Frenzy detonates immediately outside its shooting damage mask");
}

// Frenzy uses its own saved interval, synchronization and lethal fear event.
foreach (var identity in new[] { FusionPlant.SpeedyShroom, FusionPlant.BlastyShroom,
    FusionPlant.ShiveryShroom, FusionPlant.SpuddyShroom, FusionPlant.StarryShroom,
    FusionPlant.SunnyShroom, FusionPlant.SturdyShroom, FusionPlant.FrenzyShroom })
{
    foreach (int rowOffset in new[] { -1, 1 })
    {
        var scared = Make(identity);
        // Interrupt a shot with a zombie in either adjacent lane.
        scared.mStartRow = 20;
        Enemy(scared, scared.mX + scared.mWidth / 2, scared.mRow + rowOffset);
        Tick(scared, 200);
        Check(scared.mBoard.Projectiles.Count == 0 && ScaredyFusions.Afraid(scared),
            "Adjacent-lane fear cancels shots and keeps every Scaredy shooter crouched");
    }
}
foreach (int rowOffset in new[] { -1, 1 })
{
    var charged = Make(FusionPlant.FrenzyShroom);
    charged.mTargetX = 20;
    charged.mStartRow = 1;
    Enemy(charged, charged.mX + charged.mWidth / 2, charged.mRow + rowOffset);
    int beforeCrater = DoomEffects.Craters;
    Tick(charged, 1);
    Check(charged.mDead && DoomEffects.Craters == beforeCrater + 1
        && charged.mBoard.Projectiles.Count == 0,
        "Charged Frenzy detonates on the first adjacent-lane proximity tick");
    Tick(charged, 200);
    Check(DoomEffects.Craters == beforeCrater + 1, "Frenzy explosion cannot repeat after death");
}
var frenzy=Make(FusionPlant.FrenzyShroom);
Check(FusionRecipes.TryGet(SeedType.Scaredyshroom,SeedType.Doomshroom,out var frenzyResult)
    && frenzyResult==FusionPlant.FrenzyShroom,"Frenzy source recipe");
Check(FusionRecipes.TryGet(SeedType.Doomshroom,SeedType.Scaredyshroom,out frenzyResult)
    && frenzyResult==FusionPlant.FrenzyShroom,"Frenzy reverse recipe");
var frenzyEnemy=Enemy(frenzy,600);
for(int i=0;i<3000&&frenzy.mTargetX>20;++i) ScaredyFusions.Tick(frenzy);
Check(frenzy.mTargetX==20 && frenzy.mBoard.Projectiles.Count>=13,"Attacks accelerate to source .2 second floor");
Check(frenzy.mBoard.Projectiles.All(p=>ScaredyProjectiles.Family(p)==5&&ScaredyProjectiles.Damage(p)==20),
    "Dark spores retain saved family and damage");
int frenzyShots=frenzy.mBoard.Projectiles.Count;
frenzy.mBoard.mPaused=true;for(int i=0;i<50;++i)ScaredyFusions.Tick(frenzy);
Check(frenzy.mBoard.Projectiles.Count==frenzyShots&&frenzy.mTargetX==20,"Pause preserves acceleration and attack phase");
frenzy.mBoard.mPaused=false;
var rightDoom=new Plant{mBoard=frenzy.mBoard,mSeedType=FusionPlant.DoomPea.NativeType,mImitaterType=FusionPlant.DoomPea.Marker,
    mPlantCol=frenzy.mPlantCol+1,mRow=frenzy.mRow,mTargetX=37};
frenzy.mBoard.m_plants.Add(rightDoom);
for(int i=0;i<60;++i)ScaredyFusions.Tick(frenzy);
Check(frenzy.mShootingCounter==77&&ScaredyProjectiles.Damage(frenzy.mBoard.Projectiles.Last())==77,
    "Adjacent Doom Pea synchronizes base and permanent bonus into saved shot");
rightDoom.mDead=true;for(int i=0;i<60;++i)ScaredyFusions.Tick(frenzy);
Check(frenzy.mShootingCounter==77,"Synchronized damage survives removal of adjacent Doom Pea");
frenzyEnemy.X=frenzy.mX+frenzy.mWidth/2;
int cratersBefore=DoomEffects.Craters;
for(int i=0;i<90&&!frenzy.mDead;++i)ScaredyFusions.Tick(frenzy);
Check(frenzy.mDead&&DoomEffects.Craters==cratersBefore+1&&DoomEffects.Calls.Last().Damage==1800,
    "Peak fear detonates once with native Doom crater");
var ordinaryFrenzy=Make(FusionPlant.FrenzyShroom);Enemy(ordinaryFrenzy,ordinaryFrenzy.mX+ordinaryFrenzy.mWidth/2);
for(int i=0;i<101;++i)ScaredyFusions.Tick(ordinaryFrenzy);
Check(!ordinaryFrenzy.mDead&&ordinaryFrenzy.mTargetX==150&&ScaredyFusions.Afraid(ordinaryFrenzy),
    "Fear below peak resets interval without exploding");
