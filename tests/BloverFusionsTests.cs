using FusionMod;
using Il2CppReloaded.Gameplay;

int checks=0;
void Check(bool value,string message){++checks;if(!value)throw new Exception(message);}
Plant MakePlant(FusionPlant identity,Board? board=null){var p=new Plant{mBoard=board??new(),mSeedType=identity.NativeType,mImitaterType=identity.Marker};BloverFusions.Initialize(p);return p;}
Zombie MakeZombie(Board b,int x=200,bool friendly=false,ZombieType type=ZombieType.Normal){var z=new Zombie{mBoard=b,mPosX=x,mMindControlled=friendly,mZombieType=type};b.m_zombies.Add(z);return z;}
void Tick(Plant p,int n){for(int i=0;i<n&&!p.mDead;++i){BloverEffects.Restore(p.mBoard);BloverFusions.Tick(p);BloverEffects.Save(p.mBoard);}}
foreach(var (a,b,r) in new (FusionPlant,FusionPlant,FusionPlant)[]{
    (SeedType.Blover,SeedType.Blover,FusionPlant.Cyclover),(SeedType.Blover,SeedType.Garlic,FusionPlant.Disguster),
    (SeedType.Blover,SeedType.Iceshroom,FusionPlant.BreezeBlover),(SeedType.Blover,SeedType.Hypnoshroom,FusionPlant.MindBlover)}){
    Check(FusionRecipes.TryGet(a,b,out var actual)&&actual==r,"source recipe");
    Check(FusionRecipes.TryGet(b,a,out actual)&&actual==r,"reversed recipe");
    var p=MakePlant(r);Check(FusionPlant.From(p)==r&&FusionPlant.HasImportedVisuals(r),"saved identity and imported model");
    Check(p.mPlantHealth==300&&p.mDoSpecialCountdown==0,"native automatic special disabled");
    p.mBoard.mPaused=true;Tick(p,100);Check(p.mTargetX==0,"paused ability clock");
    p.mBoard.mPaused=false;p.OffGround=true;Tick(p,100);Check(p.mTargetX==0,"carried ability clock");
}
var poison=MakePlant(FusionPlant.Disguster);var enemy=MakeZombie(poison.mBoard);var ally=MakeZombie(poison.mBoard,friendly:true);
var balloon=MakeZombie(poison.mBoard,type:ZombieType.Balloon);balloon.Air=true;
var boss=MakeZombie(poison.mBoard,type:ZombieType.Boss);var underground=MakeZombie(poison.mBoard);underground.Excluded=true;
Tick(poison,48);Check(PoisonShooters.Level(enemy)==0,"no poison before source animation event");
Tick(poison,1);Check(PoisonShooters.Capture(enemy)==(6,500,1),"initial five levels followed by toxic effect");
Check(enemy.Redirects==1&&balloon.mBlowingAway&&balloon.Redirects==0,"garlic redirection and airborne removal");
Check(PoisonShooters.Level(underground)==6,"poison reaches underground enemies");
Check(PoisonShooters.Level(ally)==0&&PoisonShooters.Level(boss)==0,"friendly and boss exclusions");
PoisonShooters.States[enemy]=(9,700,43);BloverEffects.Save(poison.mBoard);
PoisonShooters.Forget(enemy);BloverEffects.Restore(poison.mBoard);
Check(PoisonShooters.Capture(enemy)==(9,700,43),"poison duration, levels and next damage survive restoration");
Tick(poison,199);Check(!poison.mDead,"Disguster lives until two seconds after blowing");
Tick(poison,1);Check(poison.mDead,"Disguster source disappearance");
var failed=MakePlant(FusionPlant.Disguster);failed.mBoard.m_gridItems.Fail=true;var safe=MakeZombie(failed.mBoard);
Tick(failed,49);Check(PoisonShooters.Level(safe)==0,"allocation failure creates no unsaved status");

var breeze=MakePlant(FusionPlant.BreezeBlover);enemy=MakeZombie(breeze.mBoard);ally=MakeZombie(breeze.mBoard,friendly:true);
underground=MakeZombie(breeze.mBoard);underground.Excluded=true;
var shot=new Projectile{mBoard=breeze.mBoard,mCobTargetX=-422040};breeze.mBoard.m_projectiles.Add(shot);
Tick(breeze,49);Check(enemy.Damage==0&&PolarProjectiles.IsPolar(shot),"conversion starts at blowing; cold pulse follows");
Check(shot.mCobTargetRow==40,"conversion preserves variant damage");
Tick(breeze,1);Check(enemy.Damage==1&&SnowPea.Capture(enemy)==(20,100),"first cold pulse and twenty cryo points");
Check(ally.Damage==0&&underground.Damage==0,"cold target categories");
Tick(breeze,909);Check(enemy.Damage==10&&breeze.mDead,"ten pulses include final pulse before disappearance");
Check(enemy.mIceTrapCounter>0,"accumulated cryo freezes");
var frozen=MakeZombie(shot.mBoard);frozen.mIceTrapCounter=400;
Check(PolarProjectiles.Impact(shot,frozen)&&shot.mDead&&frozen.Damage==160,"polar impact uses saved damage and frozen multiplier");
Check(PolarProjectiles.Impact(shot,frozen)&&frozen.Damage==160,"dead polar projectile cannot hit twice");
var bossShot=new Projectile{mBoard=breeze.mBoard};breeze.mBoard.m_projectiles.Add(bossShot);PolarProjectiles.Convert(breeze.mBoard);
boss=MakeZombie(breeze.mBoard,type:ZombieType.Boss);PolarProjectiles.Impact(bossShot,boss);
Check(boss.Damage==20,"polar projectile retains ordinary damage against bosses");
var record=breeze.mBoard.m_gridItems.First(i=>!i.mDead&&i.mGridX==unchecked((int)enemy.DataID));
uint oldId=enemy.DataID;enemy.DataID++;BloverEffects.Restore(breeze.mBoard);Check(record.mDead,"saved effect rejects recycled pool handle");enemy.DataID=oldId;

var mind=MakePlant(FusionPlant.MindBlover);ally=MakeZombie(mind.mBoard,600,true);enemy=MakeZombie(mind.mBoard,600);
var left=MakeZombie(mind.mBoard,40,true);var giant=MakeZombie(mind.mBoard,600,true,ZombieType.Gargantuar);
Tick(mind,49);Check(ally.Damage==37&&left.Damage==0&&enemy.Damage==0,"single ten-percent payment excludes leftmost column and enemies");
float x=ally.mPosX,gx=giant.mPosX;Tick(mind,1);
Check(ally.mPosX<x&&Math.Abs((gx-giant.mPosX)/(x-ally.mPosX)-.4)<.0001,"friendly drag and giant speed factor");
Tick(mind,499);Check(mind.mDead&&ally.Damage==37,"five-second lifetime never repeats payment");

var cyclone=MakePlant(FusionPlant.Cyclover);var near=MakeZombie(cyclone.mBoard,160);var second=MakeZombie(cyclone.mBoard,240);
var third=MakeZombie(cyclone.mBoard,320);var fourth=MakeZombie(cyclone.mBoard,400);var behind=MakeZombie(cyclone.mBoard,0);behind.mRow=1;
Tick(cyclone,50);Check(near.mPosX==160,"no push before animation event");
Tick(cyclone,1);Check(near.mPosX>160&&second.mPosX>240&&third.mPosX>320&&fourth.mPosX==400,"only nearest three receive wind");
Check(cyclone.mBoard.mFogBlownCountDown==3000,"first blow clears fog");
Tick(cyclone,49);float stopped=near.mPosX;Tick(cyclone,20);Check(near.mPosX==stopped,"push ends at original clip event");
Check(!cyclone.mDead&&cyclone.mLaunchCounter==181,"persistent plant preserves remaining attack interval");
cyclone.mBoard.mFogBlownCountDown=1;Tick(cyclone,231);Check(cyclone.mBoard.mFogBlownCountDown==1,"later attacks do not replay first-use balloon/fog effect");
var heatBoard=new Board();var heated=MakeZombie(heatBoard);
Check(BloverEffects.ReserveOverheat(heated),"reserve persistent overheat before application");
Overheat.Apply(heated);BloverEffects.Save(heatBoard);Overheat.Forget(heated);BloverEffects.Restore(heatBoard);
Check(Overheat.Has(heated),"overheat restored after load");
Overheat.Forget(heated);BloverEffects.Save(heatBoard);BloverEffects.Restore(heatBoard);
Check(!Overheat.Has(heated),"cold-cleared overheat does not resurrect");
heatBoard.m_gridItems.Fail=true;Check(!BloverEffects.ReserveOverheat(MakeZombie(heatBoard)),"overheat reservation failure");
Console.WriteLine($"Blover fusions: {checks} assertions passed; no game executed.");

// Glowver keeps each temporary light application in a saved native record.
var glow = MakePlant(FusionPlant.Glowver);
Check(FusionRecipes.TryGet(SeedType.Blover,SeedType.Plantern,out var glowResult) && glowResult==FusionPlant.Glowver,
    "Glowver source recipe");
Check(FusionRecipes.TryGet(SeedType.Plantern,SeedType.Blover,out glowResult) && glowResult==FusionPlant.Glowver,
    "Glowver reverse recipe");
Tick(glow,48); Check(Glowver.Level(glow.mBoard)==0,"No light before animation event");
Tick(glow,1); Check(Glowver.Level(glow.mBoard)==1,"Animation event grants one global light level");
var firstLight=glow.mBoard.m_gridItems.Single(i=>Glowver.IsEffect(i));
Check(!GlowverEffectTickPatch.Prefix(firstLight)&&firstLight.mGridItemCounter==2999,"Effect owns exactly one tick");
glow.mBoard.mPaused=true;GlowverEffectTickPatch.Prefix(firstLight);
Check(firstLight.mGridItemCounter==2999,"Pause preserves saved remaining light time");
glow.mBoard.mPaused=false;Check(Glowver.Grant(glow.mBoard)&&Glowver.Level(glow.mBoard)==2,"Independent light applications stack");
firstLight.mGridItemCounter=1;GlowverEffectTickPatch.Prefix(firstLight);
Check(firstLight.mDead&&Glowver.Level(glow.mBoard)==1,"One expiry does not remove another application's light");
var restoredLight=new GridItem{mBoard=glow.mBoard,mGridItemType=GridItemType.ZenTool,mGridItemState=GridItemState.Normal,
    mSunCount=glow.mBoard.m_gridItems.Single(i=>!i.mDead&&Glowver.IsEffect(i)).mSunCount,mGridItemCounter=1};
glow.mBoard.m_gridItems.Add(restoredLight);Check(Glowver.Level(glow.mBoard)==2,"Loaded fields reconstruct light without replaying grant");
GlowverEffectTickPatch.Prefix(restoredLight);Check(restoredLight.mDead,"Loaded light expires normally");
var failedGlow=MakePlant(FusionPlant.Glowver);failedGlow.mBoard.m_gridItems.Fail=true;
Tick(failedGlow,49);Check(failedGlow.mStartRow==0&&Glowver.Level(failedGlow.mBoard)==0,"Allocation failure does not commit blow event");
failedGlow.mBoard.m_gridItems.Fail=false;Tick(failedGlow,1);
Check(Glowver.Level(failedGlow.mBoard)==1,"Failed grant retries once storage becomes available");
