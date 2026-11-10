using FusionMod;
using Il2CppReloaded.Gameplay;
using System.Reflection;

int assertions = 0;
void Check(bool ok, string message) { ++assertions; if (!ok) throw new Exception(message); }
Plant NewPlant(FusionPlant identity)
{
    var p = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    SolarChomperPlants.Initialize(p);
    p.mBoard.m_plants.Add(p);
    return p;
}
foreach (var (a,b,r) in new[] {
    ((FusionPlant)SeedType.Chomper,(FusionPlant)SeedType.Potatomine,FusionPlant.ChompMine),
    ((FusionPlant)SeedType.Chomper,(FusionPlant)SeedType.Sunflower,FusionPlant.SolarChomper),
    ((FusionPlant)SeedType.Sunflower,(FusionPlant)SeedType.Wallnut,FusionPlant.SolarNut),
    (FusionPlant.SolarNut,(FusionPlant)SeedType.Wallnut,FusionPlant.TwinSolarNut) })
{
    Check(FusionRecipes.TryGet(a,b,out var one) && one==r,"forward recipe");
    Check(FusionRecipes.TryGet(b,a,out var two) && two==r,"reverse recipe");
    Check(FusionPlant.From(NewPlant(r))==r,"saved identity");
}
var chomper=NewPlant(FusionPlant.ChompMine);
chomper.mState=PlantState.ChomperDigesting;
SolarChomperPlants.AfterNative(chomper,PlantState.Ready);
Check(chomper.mStateCountdown==2000,"twenty-second digestion");
chomper.mState=PlantState.Ready;
var context=new object(); PlayerPlantingScope.Current=context;
SolarChomperPlants.AfterNative(chomper,PlantState.ChomperDigesting);
var mine=chomper.mBoard.m_plants.Last();
Check(mine.mSeedType==SeedType.Potatomine && mine.mState==PlantState.PotatoArmed,"spawn armed ordinary mine");
Check(mine.mPlantCol==1 && ReferenceEquals(PlayerPlantingScope.Current,context),"front cell and scope restoration");
chomper.mPlantCol=8; int count=chomper.mBoard.m_plants.Count;
SolarChomperPlants.CreateMine(chomper);
Check(count==chomper.mBoard.m_plants.Count,"right boundary");
var solar=NewPlant(FusionPlant.SolarChomper);solar.mState=PlantState.ChomperDigesting;
SolarChomperPlants.AfterNative(solar,PlantState.Ready);
Check(solar.mStateCountdown==4000,"forty-second digestion");
solar.mState=PlantState.Ready;SolarChomperPlants.AfterNative(solar,PlantState.ChomperDigesting);
Check(solar.mBoard.Coins==4,"100 sun after digestion");
SolarChomperPlants.AfterNative(solar,PlantState.Ready);
Check(solar.mBoard.Coins==4,"no repeat sun");
foreach(var identity in new[]{FusionPlant.SolarNut,FusionPlant.TwinSolarNut})
{
    var nut=NewPlant(identity);
    Check(nut.mPlantHealth==4000,"nut health");
    nut.mBoard.mPaused=true;SolarChomperPlants.UpdateNut(nut);
    Check(nut.mLaunchCounter==1500,"paused production");nut.mBoard.mPaused=false;
    nut.mLaunchCounter=1;SolarChomperPlants.UpdateNut(nut);
    Check(nut.mBoard.Coins==1,"production");
    nut.mPlantHealth=2665;SolarChomperPlants.UpdateNut(nut);
    Check(nut.mBoard.Coins==2,"first damage band");
    nut.mPlantHealth=4000;SolarChomperPlants.UpdateNut(nut);
    nut.mPlantHealth=2665;SolarChomperPlants.UpdateNut(nut);
    Check(nut.mBoard.Coins==2,"healing never rearms reward");
    nut.mPlantHealth=1332;SolarChomperPlants.UpdateNut(nut);
    Check(nut.mBoard.Coins==3 && nut.mShootingCounter==3,"second saved band");
}
foreach(int health in new[]{4000,100,50,3})
{
    var nut=NewPlant(FusionPlant.TwinSolarNut);nut.mPlantHealth=health;
    var saved=SolarChomperPlants.BeginDamage(nut);
    nut.mPlantHealth-=10000; // Native damage would otherwise kill before return.
    SolarChomperPlants.Finish(saved);SolarChomperPlants.Restore(saved);
    Check(nut.mPlantHealth==health-50 && nut.mDead==(health<=50),"fixed damage including lethal hits");
}
var twin=NewPlant(FusionPlant.TwinSolarNut);
var observation=SolarChomperPlants.BeginDamage(twin);SolarChomperPlants.Finish(observation);
Check(twin.mPlantHealth==4000,"miss does not damage");
observation=SolarChomperPlants.BeginDamage(twin);SolarChomperPlants.Restore(observation);
Check(twin.mPlantHealth==4000,"exception restores reserved health");
observation=SolarChomperPlants.BeginDamage(twin);twin.DataID++;twin.mPlantHealth=123;SolarChomperPlants.Restore(observation);
Check(twin.mPlantHealth==123,"pool reuse protected");
Check(RollingProjectiles.ChargeSolarBall(twin),"create charged ball");
var ball=twin.mBoard.Projectiles.Single();RollingProjectiles.TryShot(ball,out var shot);
float x=ball.mPosX;
for(int i=0;i<100;i++)RollingProjectiles.Move(ball,shot);
Check(ball.mPosX==x,"one-second charge");
Check(shot.AnimationTime==0 && shot.AnimationSpeed==2,"roll clock excludes charge and uses source Speed");
RollingProjectiles.Move(ball,shot);Check(ball.mPosX>x,"roll after charge");
Check(Math.Abs(shot.AnimationTime-.02f)<.00001 && ball.mRotation==0,"roll begins upright without second rotation");
float animationTime=shot.AnimationTime;
ball.mProjectileAge=100000; // Render age must not become the roll phase.
RollingProjectiles.Move(ball,shot);
Check(Math.Abs(shot.AnimationTime-animationTime-.02f)<.00001,"roll clock independent of native projectile age");
float shadowDistance=ball.mShadowY-ball.mPosY;
ball.mBoard.Ground=position=>position*.2f;
RollingProjectiles.Move(ball,shot);
Check(Math.Abs(ball.mShadowY-ball.mPosY-shadowDistance)<.00001,"solar ball ground anchor follows terrain");
ball.mBoard.Ground=_=>0;
Check(RollingProjectiles.ChargeSolarBall(twin),"new ball after launch");
var charging=twin.mBoard.Projectiles.Last();RollingProjectiles.TryShot(charging,out var power);
Check(RollingProjectiles.ChargeSolarBall(twin) && twin.mBoard.Projectiles.Count==2 && power.Damage==1200 && Math.Abs(power.Scale-(.3f+.1f*MathF.Exp(-.6f)))<.00001,"charge existing ball");
Check(Math.Abs(power.AnimationSpeed-1.9f)<.00001 && power.AnimationTime==0,"reinforcement updates source Speed before roll");
int savedPower=power.Power;power.Power=100;
Check(power.AnimationSpeed==1,"source animation Speed has lower bound");power.Power=savedPower;
power.ChargeTicks=0;
var zombie=new Zombie {mBoard=twin.mBoard,mPosX=charging.mPosX,mBodyHealth=10000,DataID=77};
twin.mBoard.m_zombies.Add(zombie);
RollingProjectiles.Move(charging,power);RollingProjectiles.Move(charging,power);RollingProjectiles.Move(charging,power);
Check(zombie.Hits==2 && zombie.Damage==2400,"damage each 0.02 seconds");
var low=NewPlant(FusionPlant.ChompMine);
var pea=new Projectile {mBoard=low.mBoard,mProjectileType=ProjectileType.ZombiePea};
var prefix=typeof(ChompMineLowCollisionScope).GetMethod("Prefix",BindingFlags.Static|BindingFlags.NonPublic)!;
var finalizer=typeof(ChompMineLowCollisionScope).GetMethod("Finalizer",BindingFlags.Static|BindingFlags.NonPublic)!;
object?[] invocation={pea,null};prefix.Invoke(null,invocation);
Check(low.mSeedType==SeedType.Potatomine,"native low filter during lookup");
finalizer.Invoke(null,new[]{invocation[1]});
Check(FusionPlant.From(low)==FusionPlant.ChompMine,"restore chomper after lookup");
foreach(var identity in new[]{(FusionPlant)SeedType.Potatomine,FusionPlant.PeatatoMine,FusionPlant.SolarMine})
{
    var potato=NewPlant(identity);potato.mState=PlantState.Notready;potato.mStateCountdown=1400;
    MineFusions.GrowImmediately(potato);
    Check(potato.mState==PlantState.PotatoArmed && potato.mStateCountdown==0 && FusionPlant.From(potato)==identity,"fertilizer immediate growth preserves variant");
}
Console.WriteLine($"Solar/chomper offline checks: {assertions} assertions passed; no game code loaded.");
