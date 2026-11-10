using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;

int assertions=0;
CherryBombPlants.DisposingBoard=IntPtr.Zero;
void Require(bool valid,string message) { ++assertions; if(!valid) throw new Exception(message); }
Plant PlantOf(FusionPlant identity) => new() { mSeedType=identity.NativeType,mImitaterType=identity.Marker };
object? Invoke(Type type,string method,params object?[] args) => type.GetMethod(method,BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,args);

(FusionPlant A,FusionPlant B,FusionPlant Result)[] recipes = [
    (SeedType.Peashooter,SeedType.Wallnut,FusionPlant.PeaNut),
    (SeedType.Chomper,SeedType.Peashooter,FusionPlant.ChompShooter),
    (SeedType.Chomper,SeedType.Wallnut,FusionPlant.ChompNut),
    (FusionPlant.ChompNut,SeedType.Peashooter,FusionPlant.Chompzilla),
    (FusionPlant.ChompShooter,SeedType.Wallnut,FusionPlant.Chompzilla),
    (SeedType.Chomper,FusionPlant.PeaNut,FusionPlant.Chompzilla)];
foreach(var (a,b,result) in recipes)
{
    Require(FusionRecipes.TryGet(a,b,out var forward)&&forward==result,"Missing forward recipe");
    Require(FusionRecipes.TryGet(b,a,out var reverse)&&reverse==result,"Missing reverse recipe");
    Require(FusionPlant.From(PlantOf(result))==result,"Saved pair loses identity");
}
var nut=PlantOf(FusionPlant.ChompNut); nut.mPlantMaxHealth=4000; nut.mPlantHealth=3500;
ChomperFusions.AfterNative(nut,PlantState.ChomperDigesting);
Require(nut.mPlantHealth==4000,"Digestion healing must stop at 4000");
nut.mPlantHealth=500;
ChomperFusions.AfterNative(nut,PlantState.ChomperBiting);
Require(nut.mPlantHealth==500,"Failed bite must not heal");
nut.mBoard.mPaused=true;
ChomperFusions.AfterNative(nut,PlantState.ChomperDigesting);
Require(nut.mPlantHealth==500,"Pause must not heal");

var shooter=PlantOf(FusionPlant.ChompShooter); shooter.mState=PlantState.ChomperDigesting;
ChomperFusions.AfterNative(shooter,PlantState.ChomperBiting);
Require(shooter.mStateCountdown==3000,"Digestion must become 30 seconds");
for(int i=0;i<81;i++) ChomperFusions.AfterNative(shooter,PlantState.ChomperDigesting);
Require(shooter.mBoard.Projectiles.Count==0,"Chunk must wait for original animation event");
ChomperFusions.AfterNative(shooter,PlantState.ChomperDigesting);
Require(shooter.mBoard.Projectiles.Count==1,"First chunk must appear at shoot event");
for(int i=0;i<300;i++) ChomperFusions.AfterNative(shooter,PlantState.ChomperDigesting);
Require(shooter.mBoard.Projectiles.Count==2,"Chunks must repeat every three seconds");
shooter.mState=PlantState.Ready;
for(int i=0;i<300;i++) ChomperFusions.AfterNative(shooter,PlantState.Ready);
Require(shooter.mBoard.Projectiles.Count==2,"No chunks outside digestion");

var zilla=PlantOf(FusionPlant.Chompzilla); zilla.mPlantHealth=zilla.mPlantMaxHealth=4000;
zilla.Target=new Zombie { mBodyHealth=2_000_000 };
for(int i=0;i<=92;i++) ChomperFusions.UpdateChompzilla(zilla);
Require(zilla.Target.Damage==1_000_000,"First special bite must deal one million");
Require(zilla.mStateCountdown==4000&&zilla.mPlantHealth==16000,"Special bite must heal with 16000 cap and enter cooldown");
for(int i=0;i<175;i++) ChomperFusions.UpdateChompzilla(zilla);
Require(zilla.Target.Damage==1_000_200,"Must keep ordinary bites during special cooldown");
zilla.mPlantHealth=5000;zilla.mLaunchCounter=1;zilla.mBoard.mMainCounter=100;zilla.Target=null;
ChomperFusions.UpdateChompzilla(zilla);
Require(zilla.mPlantHealth==4800,"Excess health must decay 200 per second");
zilla.mPlantHealth=3900;ChomperFusions.UpdateChompzilla(zilla);
Require(zilla.mPlantHealth==3900,"Decay must never lower ordinary health");
zilla.mLaunchCounter=84;zilla.Target=new Zombie { mBodyHealth=400 };
ChomperFusions.UpdateChompzilla(zilla);
Require(zilla.Target.Damage==500&&zilla.mPlantHealth==4300,"Weak unarmoured target must give 500 bite and 400 healing");
Require(zilla.mBoard.Projectiles.Count>0,"Bite must spit chunk");
zilla.mBoard.mPaused=true;int phase=zilla.mLaunchCounter;ChomperFusions.UpdateChompzilla(zilla);
Require(zilla.mLaunchCounter==phase,"Pause must freeze cooldowns");zilla.mBoard.mPaused=false;
zilla.mBoard.mMainCounter=101;zilla.mLaunchCounter=84;zilla.Target=null;ChomperFusions.UpdateChompzilla(zilla);
Require(zilla.mPlantHealth==4300,"Miss must not heal");
zilla.mPlantHealth=4000;
Require((bool)Invoke(typeof(ChompzillaCrushPatch),"Prefix",zilla)! == false&&zilla.mPlantHealth==2667&&!zilla.mDead,"Crush must deal at most 1333");
zilla.mPlantHealth=100;Invoke(typeof(ChompzillaCrushPatch),"Prefix",zilla);
Require(zilla.mDead,"Capped hit must still kill low health plant");
var normal=PlantOf(SeedType.Chomper);
Require((bool)Invoke(typeof(ChompzillaCrushPatch),"Prefix",normal)!,"Native chomper must keep native crush");
var explosion=PlantOf(FusionPlant.Chompzilla);explosion.mPlantHealth=4000;
Require((bool)Invoke(typeof(ChompzillaExplosionDamagePatch),"Prefix",explosion)!,"Unscoped removal must stay native");
ChomperFusions.PlantExplosionBoard=explosion.mBoard;
Require(!(bool)Invoke(typeof(ChompzillaExplosionDamagePatch),"Prefix",explosion)!&&explosion.mPlantHealth==2667,"Explosion removal must become capped damage");
PlayerPlantingScope.ConsumingIngredient=explosion.Pointer;
Require((bool)Invoke(typeof(ChompzillaExplosionDamagePatch),"Prefix",explosion)!,"Ingredient consumption must never become damage");
PlayerPlantingScope.ConsumingIngredient=IntPtr.Zero;ChomperFusions.PlantExplosionBoard=null;

var cooldown=PlantOf(FusionPlant.Chompzilla);cooldown.Target=new Zombie {mBodyHealth=10_000_000};
for(int i=0;i<93;i++) ChomperFusions.UpdateChompzilla(cooldown);
for(int i=0;i<3999;i++) ChomperFusions.UpdateChompzilla(cooldown);
Require(cooldown.Target.Damage<2_000_000,"Second million-damage bite must wait at least 40 seconds");
for(int i=0;i<176;i++) ChomperFusions.UpdateChompzilla(cooldown);
Require(cooldown.Target.Damage>=2_000_000,"Special bite must become available again");
var loaded=PlantOf(FusionPlant.Chompzilla);loaded.Target=new Zombie();
loaded.mLaunchCounter=83;loaded.mStateCountdown=3900;
ChomperFusions.UpdateChompzilla(loaded);
Require(loaded.Target.Hits==0,"Loading after bite event must not replay damage");
CherryBombPlants.DisposingBoard=loaded.mBoard.Pointer;
int savedPhase=loaded.mLaunchCounter;ChomperFusions.UpdateChompzilla(loaded);
Require(loaded.mLaunchCounter==savedPhase,"Board teardown must stop abilities");
CherryBombPlants.DisposingBoard=IntPtr.Zero;

var roller=PlantOf(FusionPlant.PeaNut);RollingProjectiles.Spawn(roller,false);
var projectile=roller.mBoard.Projectiles.Single();
Require(RollingProjectiles.TryShot(projectile,out var shot),"Rolling shot registration missing");
Require(projectile.mPosZ<0&&projectile.mVelX==2.5f,"Pea-nut must start above its ground baseline");
float initialHeight=projectile.mPosZ;
RollingProjectiles.Move(projectile,shot);
Require(projectile.mPosZ>initialHeight&&projectile.mPosZ<0,"Nut must fall towards ground");
var first=new Zombie { DataID=20,mPosX=projectile.mPosX+5 };
var second=new Zombie { DataID=21,mPosX=projectile.mPosX+6 };
var ally=new Zombie { DataID=22,mPosX=projectile.mPosX+5,mMindControlled=true };
var air=new Zombie { DataID=23,mPosX=projectile.mPosX+5,Eligible=false };
roller.mBoard.m_zombies.AddRange([first,second,ally,air]);
for(int i=0;i<10;i++) RollingProjectiles.Move(projectile,shot);
Require(first.Damage==20&&second.Damage==20,"Nut must pierce multiple targets exactly once each");
Require(ally.Hits==0&&air.Hits==0,"Must ignore allies and non-ground targets");
for(int i=0;i<50;i++) RollingProjectiles.Move(projectile,shot);
Require(projectile.mPosZ==0&&projectile.mVelZ==0&&projectile.mAccZ==0,"Nut must land without sinking or bouncing");
Require(projectile.mVelX==1.25f,"Landed nut must roll slower than its initial flight");
float landedX=projectile.mPosX;
RollingProjectiles.Move(projectile,shot);
Require(projectile.mPosX==landedX+1.25f&&projectile.mPosZ==0,"Rolling phase must stay on ground");
projectile.DataID++;
Require(!RollingProjectiles.TryShot(projectile,out _),"Pooled projectile must not inherit old shot");
projectile.DataID--;roller.mBoard.Ground=x=>-x;
RollingProjectiles.Move(projectile,shot);
Require(projectile.mDead,"Rolling shot must disappear uphill");
RollingProjectiles.ForgetBoard(roller.mBoard);
Require(!RollingProjectiles.TryShot(projectile,out _),"Board teardown must clear shots");
var chunkPlant=PlantOf(FusionPlant.ChompShooter);RollingProjectiles.Spawn(chunkPlant,false);
var chunk=chunkPlant.mBoard.Projectiles.Single();
Require(chunk.mPosZ==0&&chunk.mVelX==3.33f&&chunk.mAccZ==0,"Chomp-shooter movement must remain unchanged");

var weakFirst=PlantOf(FusionPlant.Chompzilla); weakFirst.mPlantHealth=4000;
weakFirst.mLaunchCounter=84; weakFirst.Target=new Zombie {mBodyHealth=500};
ChomperFusions.UpdateChompzilla(weakFirst);
Require(weakFirst.Target.Damage==500&&weakFirst.mStateCountdown==0,"Weak bite preserves ready special");
Require(weakFirst.mBoard.Projectiles.Count==0&&weakFirst.mTargetX==3,"Chunk waits for original spit event");
for(int i=0;i<52;i++) ChomperFusions.UpdateChompzilla(weakFirst);
var restoredShot=weakFirst.mBoard.Projectiles.Last();
Require(RollingProjectiles.Chunk(restoredShot)==3,"Weak bite uses original knockback chunk");
RollingProjectiles.Clear();
Require(RollingProjectiles.TryShot(restoredShot,out var restoredChunk)&&restoredChunk.Knockback,"Chunk restores after load");
weakFirst.mStateCountdown=4000; ChomperFusions.Fertilize(weakFirst);
Require(weakFirst.mStateCountdown==50,"Fertilizer caps digestion to half second");
weakFirst.mStateCountdown=12; ChomperFusions.Fertilize(weakFirst);
Require(weakFirst.mStateCountdown==12,"Fertilizer never lengthens digestion");

restoredShot.mProjectileType = ProjectileType.PeashooterPea;
Require(RollingProjectiles.Chunk(restoredShot)==3,"Native pea skin alias retains original zombie chunk identity");

var hungryPlant=PlantOf(FusionPlant.HungryShroom);
RollingProjectiles.Spawn(hungryPlant,false,1,hungry:true);
var hungryShot=hungryPlant.mBoard.Projectiles.Single();
Require(RollingProjectiles.Hungry(hungryShot)&&RollingProjectiles.Chunk(hungryShot)==1,"Hungry chunk has a distinct saved identity");
RollingProjectiles.Clear();
Require(RollingProjectiles.TryShot(hungryShot,out var hungryState)&&!hungryState.Knockback,"Hungry chunk reconstructs without Chompzilla knockback");
var firstHungryTarget=new Zombie{DataID=501,mPosX=60};
var secondHungryTarget=new Zombie{DataID=502,mPosX=65};
hungryPlant.mBoard.m_zombies.Add(firstHungryTarget);hungryPlant.mBoard.m_zombies.Add(secondHungryTarget);
RollingProjectiles.Move(hungryShot,hungryState);
Require(firstHungryTarget.Damage==40&&secondHungryTarget.Damage==40,"Hungry chunk pierces multiple enemies for forty each");
RollingProjectiles.Clear();RollingProjectiles.TryShot(hungryShot,out hungryState);
RollingProjectiles.Move(hungryShot,hungryState);
Require(firstHungryTarget.Hits==1&&secondHungryTarget.Hits==1,"Loading a chunk cannot clear saved target history");
hungryShot.mProjectileType=ProjectileType.PeashooterPea;
Require(RollingProjectiles.Chunk(hungryShot)==1,"Hungry chunk supports native pea skin alias");
Console.WriteLine($"Chomper fusion offline checks: {assertions} assertions passed; no game code loaded.");
// MoveWay_right2 uses landY+1 in source Y-up coordinates. The native board
// helper supplies the cell top; its ground point is80 pixels lower. Verify
// the actual production movement leaf rather than repeating a helper formula.
{
    var weed = PlantOf(FusionPlant.WeedSpreader);
    weed.mBoard.Ground = _ => 200;
    var saw = RollingProjectiles.SpawnWeed(weed, 1)!;
    Require(RollingProjectiles.TryShot(saw, out var sawState), "Weed reconstructs its rolling owner");
    saw.mPosY = 180;
    RollingProjectiles.Move(saw, sawState);
    Require(Math.Abs(saw.mPosY - 182.4444444f) < .0001f,
        "Adjacent saw approaches ground minus one source unit, never the ground point");
    var straight = RollingProjectiles.SpawnWeed(weed, weed.mRow)!;
    RollingProjectiles.TryShot(straight, out var straightState);
    straight.mPosY = 180;
    RollingProjectiles.Move(straight, straightState);
    Require(straight.mPosY == 180, "Own-lane saw keeps its original straight height");
}
Console.WriteLine($"Chomper and Weed geometry offline checks: {assertions} assertions passed; isolated canonical-step adapter, no game code loaded.");
{
    var weed = PlantOf(FusionPlant.WeedSpreader);
    var saw = RollingProjectiles.SpawnWeed(weed, weed.mRow)!;
    var firstSawTarget = new Zombie { DataID = 901, mBoard = weed.mBoard, mPosX = 100 };
    var secondSawTarget = new Zombie { DataID = 902, mBoard = weed.mBoard, mPosX = 110 };
    var alliedSawTarget = new Zombie { DataID = 903, mBoard = weed.mBoard, mPosX = 100, mMindControlled = true };
    weed.mBoard.m_zombies.AddRange([firstSawTarget, secondSawTarget, alliedSawTarget]);
    RollingProjectiles.TryShot(saw, out var owner);
    RollingProjectiles.Move(saw, owner);
    Require(firstSawTarget.Damage == 5 && secondSawTarget.Damage == 5 && alliedSawTarget.Damage == 0,
        "Original saw pierces two hostile targets for five each and ignores an ally");
    RollingProjectiles.Clear();
    Require(RollingProjectiles.TryShot(saw, out owner) && owner.Weed, "Saved marker reconstructs the saw after transient owner loss");
    RollingProjectiles.Move(saw, owner);
    Require(firstSawTarget.Hits == 1 && secondSawTarget.Hits == 1, "Owner reconstruction preserves board-owned once-per-target history");
    firstSawTarget.DataID += 0x10000; // Same pool slot, different generation; never reuse target902's ID.
    RollingProjectiles.Move(saw, owner);
    Require(firstSawTarget.Hits == 2, "New full target generation may be hit at the same coordinates");
    var vehicleWeed = PlantOf(FusionPlant.WeedSpreader);
    vehicleWeed.mBoard.Pointer = (IntPtr)910;
    var vehicleSaw = RollingProjectiles.SpawnWeed(vehicleWeed, vehicleWeed.mRow)!;
    var vehicle = new Zombie { DataID = 911, mBoard = vehicleWeed.mBoard, mPosX = 100,
        mZombieType = ZombieType.Zamboni, mBodyHealth = 1000, mBodyMaxHealth = 1000 };
    vehicleWeed.mBoard.m_zombies.Add(vehicle);
    RollingProjectiles.TryShot(vehicleSaw, out var vehicleOwner);
    RollingProjectiles.Move(vehicleSaw, vehicleOwner);
    Require(vehicle.Damage == 305 && vehicleSaw.mDead, "Vehicle receives five plus thirty percent maximum body health; saw is consumed");
}
Console.WriteLine($"Chomper and Weed owner offline checks: {assertions} assertions passed; clock/history are explicitly isolated adapters, no game code loaded.");
