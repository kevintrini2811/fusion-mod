using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;

int assertions = 0;
void Check(bool condition, string message)
{ ++assertions; if (!condition) throw new Exception(message); }
object? Call(Type type, string name, params object?[] args) =>
    type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, args);
Plant Make(FusionPlant identity, Board board) => new() { mSeedType=identity.NativeType,
    mImitaterType=identity.Marker, mBoard=board, mRow=1, mPlantCol=2 };
var board = new Board();
CherryBombPlants.DisposingBoard=IntPtr.Zero;
foreach (var (identity, health) in new[] { (FusionPlant.FootballHelmet,4000),
    (FusionPlant.FootballNut,16000), (FusionPlant.FootballGatling,3000) })
{
    var plant = Make(identity,board);
    Check(FusionPlant.From(plant)==identity,"Saved identity reconstructs");
    FootballPlants.Initialize(plant);
    Check(plant.mPlantHealth==health && plant.mPlantMaxHealth==health,"Original health");
    Check(FusionPlant.HasImportedVisuals(identity),"Original graphics registered");
    if (identity==FusionPlant.FootballHelmet)
        Check(plant.mState==PlantState.Notready && !(bool)Call(typeof(BucketPotatoPatch),"Prefix",plant)!,"Helmet cannot explode as a mine");
}
foreach (var (ingredient,result) in new[] { (SeedType.Tallnut,FusionPlant.FootballNut),
    (SeedType.Gatlingpea,FusionPlant.FootballGatling) })
{
    Check(FusionRecipes.TryGet(ingredient,FusionPlant.FootballHelmet,out var found)&&found==result,"Forward recipe");
    Check(FusionRecipes.TryGet(FusionPlant.FootballHelmet,ingredient,out found)&&found==result,"Reverse recipe");
}
var football = new Zombie { mBoard=board,mZombieType=ZombieType.Football,
    mHelmType=HelmType.Football,mPosX=100,mPosY=100 };
Check(BucketDrops.BeforeDamage(football,DamageFlags.DoesntLeaveBody)==null,"Instant kill cannot drop");
var hit=BucketDrops.BeforeDamage(football,DamageFlags.None);
football.mHelmHealth=0; BucketDrops.AfterDamage(football,hit);
Check(BucketDrops.TryPick(board,140,140,out var drop)&&drop!.Identity==FusionPlant.FootballHelmet,"Helmet loss produces original helmet");
int randomCalls=board.RandomCalls;
BucketDrops.AfterDamage(football,hit);
Check(BucketDrops.Count==1 && board.RandomCalls==randomCalls,"No duplicate roll");
BucketPlants.MouseDown(board,140,140,1,0);
board.FailCreation=true; BucketPlants.MouseDown(board,2,1,1,0);
Check(BucketPlants.HoldingDrop(drop!)&&BucketDrops.Contains(drop),"Failed allocation retains helmet");
board.FailCreation=false; BucketPlants.MouseDown(board,2,1,1,0);
Check(FusionPlant.From(board.LastCreated!)==FusionPlant.FootballHelmet && BucketDrops.Count==0,"Plant loose helmet");
BucketDrops.Clear();
football.mHelmHealth=1400; board.RandomValues.Enqueue(1);
hit=BucketDrops.BeforeDamage(football,DamageFlags.None);football.mHelmHealth=0;BucketDrops.AfterDamage(football,hit);
Check(BucketDrops.Count==0,"Other half of roll gives no helmet");
football.mHelmHealth=1400; hit=BucketDrops.BeforeDamage(football,DamageFlags.None);
football.mHelmHealth=0;football.mBodyHealth=0;football.mDead=true;BucketDrops.AfterDamage(football,hit);
Check(BucketDrops.Count==1,"Ordinary lethal damage is not an instant kill");
BucketDrops.Clear();
var magnet=Make(SeedType.Magnetshroom,board);magnet.mPlantCol=1;
magnet.mMagnetItems[0].ItemType=MagnetItemType.FootballHelmet2;board.Plants[(1,1)]=magnet;
var nut=Make(SeedType.Tallnut,board);board.Plants[(2,1)]=nut;
BucketPlants.MouseDown(board,1,1,1,0);
Check(BucketPlants.IsTarget(nut),"Helmet highlights Tall-nut");
BucketPlants.MouseDown(board,2,1,1,0);
Check(nut.mDead && FusionPlant.From(board.LastCreated!)==FusionPlant.FootballNut
    && magnet.mMagnetItems[0].ItemType==MagnetItemType.None,"Captured helmet fuses before consumption");
var footballNut=board.LastCreated!;FootballPlants.Initialize(footballNut);footballNut.mPlantHealth=15500;
Check(BucketPlants.CanFeed(footballNut,FusionPlant.FootballHelmet),"Helmet heals Football-nut");
BucketPlants.Feed(footballNut);Check(footballNut.mPlantHealth==16000,"Helmet healing capped");
footballNut.mPlantHealth=1;BucketPlants.Feed(footballNut);Check(footballNut.mPlantHealth==5001,"Original 5000 healing");
Check(!BucketPlants.CanFeed(footballNut,FusionPlant.Bucket),"Bucket cannot substitute helmet");
var gatling=Make(FusionPlant.FootballGatling,board);FootballPlants.Initialize(gatling);
Check(gatling.mLaunchRate==200,"Original two second burst interval");
Projectile? red=null;
for(int i=1;i<=24;++i)
{
    var shot=new Projectile { mBoard=board,Pointer=(IntPtr)(100+i),DataID=(uint)i };
    IronProjectiles.Firing=gatling;IronProjectiles.Track(shot);IronProjectiles.Track(shot);IronProjectiles.Firing=null;
    Check(IronProjectiles.IsRed(shot)==(i%12==0),"Every twelfth actual projectile is red, no duplicate counting");
    if(i==12) red=shot;
}
// Counter and normal iron origin survive reconstruction independently.
var resumed=Make(FusionPlant.FootballGatling,board);resumed.mTargetZombieID=11;
var resumedShot=new Projectile {mBoard=board,Pointer=(IntPtr)500,DataID=500};
IronProjectiles.Firing=resumed;IronProjectiles.Track(resumedShot);IronProjectiles.Firing=null;
Check(IronProjectiles.IsRed(resumedShot)&&(int)resumed.mTargetZombieID==0,"Reload at shot eleven preserves next special shot");
var ironRestored=new Projectile {mBoard=board,mCobTargetX=-450011,Pointer=(IntPtr)501,DataID=501};
Check(IronProjectiles.Contains(ironRestored)&&!IronProjectiles.IsRed(ironRestored),"Normal football iron shot survives reload");
var shielded=new Zombie {mBoard=board,mPosX=400,mShieldHealth=600};
object?[] ironImpact=[ironRestored,shielded,null];Call(typeof(IronImpactScope),"Prefix",ironImpact);
object?[] ironDamage=[shielded,20,null];Call(typeof(IronDamagePatch),"Prefix",ironDamage);
Check((int)ironDamage[1]! == 680,"Restored iron projectile retains shield damage");
Call(typeof(IronDamagePatch),"Postfix",shielded,ironDamage[2]);Check(shielded.mPosX==416,"Restored iron projectile retains knockback");
Call(typeof(IronImpactScope),"Finalizer",ironImpact[2]);
// Reconstruct a projectile using only fields saved by native serialization.
var restored=new Projectile { mBoard=board,mCobTargetX=red!.mCobTargetX,Pointer=(IntPtr)300,DataID=300 };
Check(IronProjectiles.Contains(restored)&&IronProjectiles.IsRed(restored),"Special projectile survives reload");
football=new Zombie {mBoard=board,mZombieType=ZombieType.Football,mPosX=400,mShieldHealth=1000};
object?[] impact=[restored,football,null];Call(typeof(IronImpactScope),"Prefix",impact);
Check(FootballPlants.IsFleeing(football)&&football.mVelX==2&&!football.mMindControlled,"Red hit doubles speed without hypnosis");
object?[] damage=[football,20,null];Call(typeof(IronDamagePatch),"Prefix",damage);
Check((int)damage[1]! == 80,"Red projectile has direct 80 damage, no iron shield bonus");
Call(typeof(IronDamagePatch),"Postfix",football,damage[2]);Check(football.mPosX==400,"Red projectile has no iron knockback");
Call(typeof(IronImpactScope),"Finalizer",impact[2]);
FootballPlants.Scare(football);Check(football.mVelX==2,"Repeated red hit never doubles speed again");
FootballPlants.Walk(football);Check(football.mPosX==402,"Flee right");
board.mPaused=true;FootballPlants.Walk(football);Check(football.mPosX==402,"Pause freezes fleeing");board.mPaused=false;
football.mButteredCounter=10;FootballPlants.Walk(football);Check(football.mPosX==402,"Butter freezes fleeing");football.mButteredCounter=0;
football.mChilledCounter=10;FootballPlants.Walk(football);Check(football.mPosX==403,"Cold slows fleeing");
object?[] target=[football,Make(SeedType.Wallnut,board)];Check(!(bool)Call(typeof(FootballFleeTargetPatch),"Prefix",target)!&&target[1]==null,"Fleeing zombie cannot eat plants");
var loaded=new Zombie {mBoard=board,mZombieType=ZombieType.Football,mSummonCounter=football.mSummonCounter,mVelX=football.mVelX};
Check(FootballPlants.IsFleeing(loaded),"Saved native flight state survives reload");
football.mPosX=901;FootballPlants.Walk(football);Check(football.mDead,"Exit removes fleeing zombie");
var normal=new Zombie {mZombieType=ZombieType.Normal};FootballPlants.Scare(normal);Check(!FootballPlants.IsFleeing(normal)&&normal.mVelX==1,"Other zombies do not flee");
var ally=new Zombie {mZombieType=ZombieType.Football,mMindControlled=true};FootballPlants.Scare(ally);Check(!FootballPlants.IsFleeing(ally)&&ally.mVelX==1,"Friendly football unaffected");
BucketPlants.Clear();BucketDrops.Clear();IronProjectiles.Clear();
Console.WriteLine($"{assertions} offline assertions passed. No game loaded or started.");
