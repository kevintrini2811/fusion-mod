using FusionMod;
using Il2CppReloaded.Gameplay;
int checks=0;
void Check(bool condition,string message) {++checks;if(!condition)throw new Exception(message);}
Plant Make(FusionPlant id,int row=2) {var b=new Board();var p=new Plant {mBoard=b,mSeedType=id.NativeType,mImitaterType=id.Marker,mPlantCol=3,mRow=row,mX=240,mY=row*80};b.m_plants.Add(p);TorchFusions.Initialize(p);return p;}
Projectile Shot(Plant p,ProjectileType type=ProjectileType.Pea,float tag=0) => new() {mBoard=p.mBoard,mRow=p.mRow,mPosX=p.mX+32,mPosY=p.mY+40,mProjectileType=type,mCobTargetX=tag};
var tree=Make(FusionPlant.Torchthree);
for(int phase=0;phase<3;++phase) {var shot=Shot(tree);int before=tree.mBoard.Shots.Count;TorchFusions.Convert(shot);Check(shot.mDead,"input consumed once");Check(tree.mBoard.Shots.Count-before==phase+1,"1/2/3 ignition cycle");Check(tree.mTargetX==(phase+1)%3,"saved next phase");}
Check(tree.mBoard.Shots.All(s=>s.mDamageRangeFlags==DamageRangeFlags.Ground),"every split fireball can collide with ground zombies");
Check(tree.mBoard.Shots.Any(s=>s.mRow==1)&&tree.mBoard.Shots.Any(s=>s.mRow==3),"neighbor lanes");
tree=Make(FusionPlant.Torchthree,0);tree.mTargetX=2;TorchFusions.Convert(Shot(tree));Check(tree.mBoard.Shots.Count==3&&tree.mBoard.Shots.Count(s=>s.mRow==0)==2,"edge keeps all three peas");
var nut=Make(FusionPlant.Nutwood);Check(nut.mPlantMaxHealth==4000,"nut toughness");nut.mTargetY=49;nut.mBoard.Full=true;TorchFusions.Convert(Shot(nut));Check(nut.mTargetY==50,"full row retains summon credit");nut.mBoard.Full=false;TorchFusions.Convert(Shot(nut));Check(nut.mTargetY==0&&nut.mBoard.m_plants.Any(p=>p.mSeedType==SeedType.Wallnut),"free row creates roasted nut");
var hypno=Make(FusionPlant.Hypnowood);hypno.mTargetY=59;TorchFusions.Convert(Shot(hypno));Check(HypnoShooters.Summons==0&&hypno.mTargetY==60,"strict source threshold");TorchFusions.Convert(Shot(hypno));Check(HypnoShooters.Summons==1&&hypno.mTargetY==0,"61st credit summons");
hypno.mTargetY=10;var colored=Shot(hypno,tag:-30);TorchFusions.Convert(colored);Check(hypno.mTargetY==14&&HypnoShooters.IsFire(colored),"hypno pea credits four");TorchFusions.Convert(Shot(hypno,tag:-20));Check(hypno.mTargetY==14,"ally cannot farm credit");
var snow=Shot(hypno,ProjectileType.Snowpea);TorchFusions.Convert(snow);Check(snow.mProjectileType==ProjectileType.Pea&&hypno.mTargetY==14,"thaw gives no credit");hypno.mBoard.mPaused=true;TorchFusions.Convert(Shot(hypno));Check(hypno.mTargetY==14,"pause");hypno.mBoard.mPaused=false;
var attacker=new Zombie {mBoard=hypno.mBoard};hypno.mBoard.FailPlant=true;Check(!TorchFusions.Bite(hypno,attacker)&&!hypno.mDead&&!attacker.mMindControlled,"failed replacement retains plant and enemy");hypno.mBoard.FailPlant=false;Check(TorchFusions.Bite(hypno,attacker)&&hypno.mDead&&attacker.mMindControlled,"bite atomically restores native torchwood");
// A projectile-heavy board without converter hosts must scan its population
// once per tick, independent of projectile count or the three converter queries.
var crowded = new Board();
for(int i=0;i<35;++i) crowded.m_plants.Add(new Plant {mBoard=crowded,mSeedType=SeedType.Gatlingpea});
for(int shot=0;shot<1000;++shot)
    for(int converter=0;converter<3;++converter) Check(TorchPlantIndex.For(crowded).Length==0,"ordinary plants excluded");
Check(crowded.m_plants.Reads==35,"three thousand queries perform only one population scan");
++crowded.mMainCounter;TorchPlantIndex.For(crowded);
Check(crowded.m_plants.Reads==70,"next simulation tick refreshes population exactly once");

var late = new Plant {mBoard=crowded,mSeedType=SeedType.Torchwood,mImitaterType=SeedType.None,mPlantCol=3,mRow=2,mX=240,mY=160};
crowded.m_plants.Add(late);
var oldSnapshot=TorchPlantIndex.For(crowded);
Check(oldSnapshot.Length==1,"same-tick population growth invalidates an empty index");
late.mImitaterType=FusionPlant.Nutwood.Marker;
var crossing=Shot(late);TorchFusions.Convert(crossing);
Check(crossing.mProjectileType==ProjectileType.Fireball,"identity assigned after indexing is read live");

crossing=Shot(late);late.mRow=3;TorchFusions.Convert(crossing);
Check(crossing.mProjectileType==ProjectileType.Pea,"same-tick row movement cannot use stale coordinates");
late.mRow=2;crossing=Shot(late);late.mX=600;TorchFusions.Convert(crossing);
Check(crossing.mProjectileType==ProjectileType.Pea,"same-tick horizontal movement is read live");
late.mX=240;late.mDead=true;crossing=Shot(late);TorchFusions.Convert(crossing);
Check(crossing.mProjectileType==ProjectileType.Pea,"dead converter is rejected without waiting for index refresh");
late.mDead=false;++late.DataID;crossing=Shot(late);TorchFusions.Convert(crossing);
Check(crossing.mProjectileType==ProjectileType.Pea&&!oldSnapshot[0].IsCurrent(crowded),"pooled generation cannot inherit cached membership");
TorchPlantIndex.Invalidate();crossing=Shot(late);TorchFusions.Convert(crossing);
Check(crossing.mProjectileType==ProjectileType.Fireball,"lifecycle invalidation refreshes current generation in the same tick");
Check(oldSnapshot.Length==1,"invalidating and rebuilding never mutate an in-flight snapshot");

var replacement=new Plant {mBoard=crowded,mSeedType=SeedType.Potatomine,mImitaterType=SeedType.Torchwood};
crowded.m_plants[^1]=replacement;
var postfix=typeof(TorchIndexInitializePatch).GetMethod("Postfix",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic)!;
postfix.Invoke(null,null);
Check(TorchPlantIndex.For(crowded).Single().Plant==replacement,"initialize postfix catches same-count replacement in the same tick");
var otherBoard=new Board();otherBoard.m_plants.Add(late);
Check(!TorchPlantIndex.For(otherBoard)[0].IsCurrent(otherBoard),"foreign board candidate is rejected");
var disposal=typeof(TorchIndexDisposePatch).GetMethod("Prefix",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic)!;
disposal.Invoke(null,null);
Check(TorchPlantIndex.For(crowded).Single().Plant==replacement,"board disposal releases the prior board snapshot");

// New summons retain each source's different threshold and full-row behavior.
var squashwood=Make(FusionPlant.Squashwood); squashwood.mTargetY=49;
TorchFusions.Convert(Shot(squashwood));Check(squashwood.mTargetY==50&&squashwood.mBoard.m_plants.Count==1,"fiftieth conversion does not summon");
squashwood.mBoard.Full=true;TorchFusions.Convert(Shot(squashwood,ProjectileType.Snowpea));Check(squashwood.mTargetY==51,"thaw counts and full row preserves Squashwood credit");
squashwood.mBoard.Full=false;TorchFusions.Convert(Shot(squashwood));Check(squashwood.mTargetY==0&&squashwood.mBoard.m_plants.Any(p=>FusionPlant.From(p)==FusionPlant.RoastedSquash),"next legal conversion summons original Roasted Squash");
var weed=Make(FusionPlant.Torchweed);TorchweedFusions.Initialize(weed);weed.mTargetX=99;
var small=Shot(weed,tag:-460010);TorchweedFusions.Convert(small);Check(small.mCobTargetX==ChargeTorchFusions.SmallFire&&small.mCobTargetRow==40&&weed.mTargetX==100,"small pea ignites and strict hundredth threshold");
TorchweedFusions.Convert(small);Check(weed.mTargetX==100,"same shot cannot count twice");
weed.mBoard.Full=true;TorchweedFusions.Convert(Shot(weed,tag:-460010));Check(weed.mTargetX==0&&weed.mBoard.m_plants.Count==1,"Torchweed loses charge when row is full");
weed.mBoard.Full=false;weed.mTargetX=100;TorchweedFusions.Convert(Shot(weed,tag:-460010));Check(weed.mTargetX==0&&weed.mBoard.m_plants.Any(p=>FusionPlant.From(p)==FusionPlant.RoastedSpikeweed),"Torchweed summons separate non-fusible marker");
var rolling=Shot(weed);RollingProjectiles.Track(rolling,true);rolling.mCobTargetRow=20;TorchweedFusions.Convert(rolling);Check(rolling.mCobTargetX==RollingProjectiles.IgnitedNut&&rolling.mCobTargetRow==40,"ordinary rolling nut ignites once");int credit=weed.mTargetX;rolling.mHitTorchwoodGridX=-1;TorchweedFusions.Convert(rolling);Check(rolling.mCobTargetRow==40&&weed.mTargetX==credit,"already ignited source232 is not accepted by another Torchweed");
var plain=Shot(weed);TorchweedFusions.Convert(plain);Check(plain.mProjectileType==ProjectileType.Pea&&weed.mTargetX==credit,"ordinary pea is not a small pea");
weed.mBoard.mPaused=true;TorchweedFusions.Convert(Shot(weed,tag:-460010));int timer=weed.mLaunchCounter;TorchweedFusions.Tick(weed);Check(weed.mTargetX==credit&&weed.mLaunchCounter==timer,"pause stops both converter and fire clock");weed.mBoard.mPaused=false;
var normal=new Zombie {mBoard=weed.mBoard,mRow=weed.mRow,X=weed.mX+35};var balloon=new Zombie {mBoard=weed.mBoard,mRow=weed.mRow,X=weed.mX+35,mZombieType=ZombieType.Balloon,Flying=true};var ally=new Zombie {mBoard=weed.mBoard,mRow=weed.mRow,X=weed.mX+35,mMindControlled=true};weed.mBoard.m_zombies.AddRange(new[]{normal,balloon,ally});weed.mLaunchCounter=1;TorchweedFusions.Tick(weed);Check(normal.Health==960&&balloon.Health==1000&&balloon.mFlyingHealth==0&&balloon.Popping&&balloon.Landings==1&&ally.Health==1000&&normal.ColdClears==1,"one fire pulse damages ground, pops balloon without body overflow, thaws, excludes allies");
Check(weed.mLaunchCounter==120,"independent fire clock resets to one native second");
Check(TorchweedFusions.Target(weed,balloon),"popping balloon remains a target despite native OffGround classification");
var otherOffGround=new Zombie{mBoard=weed.mBoard,mRow=weed.mRow,X=weed.mX+35,OffGround=true};Check(!TorchweedFusions.Target(weed,otherOffGround),"unrelated OffGround target remains excluded");
var airborneOtherBalloon=new Zombie{mBoard=weed.mBoard,mRow=weed.mRow,X=weed.mX+35,mZombieType=ZombieType.Balloon,OffGround=true};Check(!TorchweedFusions.Target(weed,airborneOtherBalloon),"OffGround addition is restricted to actual BalloonPopping phase");
weed.mLaunchCounter=1;TorchweedFusions.Tick(weed);Check(balloon.Health==960&&balloon.Landings==1,"popping balloon receives ordinary fire damage without another pop");
var clock=Make(FusionPlant.Torchweed);TorchweedFusions.Initialize(clock);var clockTarget=new Zombie{mBoard=clock.mBoard,mRow=clock.mRow,X=clock.mX+35};clock.mBoard.m_zombies.Add(clockTarget);
for(int i=0;i<119;i++)TorchweedFusions.Tick(clock);Check(clockTarget.Health==1000&&clock.mLaunchCounter==1,"no source fire before one native second");
TorchweedFusions.Tick(clock);Check(clockTarget.Health==960,"120th native tick deals exactly40");
for(int i=0;i<120;i++)TorchweedFusions.Tick(clock);Check(clockTarget.Health==920,"second native second deals exactly40 again");
var helper=Make(FusionPlant.RoastedSpikeweed);TorchweedFusions.Initialize(helper);var helperTarget=new Zombie{mBoard=helper.mBoard,mRow=helper.mRow,X=helper.mX+35};helper.mBoard.m_zombies.Add(helperTarget);
for(int i=0;i<119;i++)TorchweedFusions.Tick(helper);Check(helper.mStartRow==0&&helperTarget.Health==1000,"helper waits one source second before attack trigger");
TorchweedFusions.Tick(helper);Check(helper.mStartRow==80&&helperTarget.Health==1000,"helper starts original two-thirds-second clip without immediate damage");
for(int i=0;i<39;i++)TorchweedFusions.Tick(helper);Check(helperTarget.Health==1000,"helper waits until original one-third-second event");
TorchweedFusions.Tick(helper);Check(helperTarget.Health==980&&helper.mStartRow==40,"helper damages20 at exact AnimAttack event");
for(int i=0;i<120;i++)TorchweedFusions.Tick(helper);Check(helperTarget.Health==960,"helper steady-state twenty per source second at zero jitter");
helper.mBoard.RandomValue=0;Check(TorchweedFusions.RoastedInterval(helper)==108,"source negative0.1second jitter quantizes to108ticks");helper.mBoard.RandomValue=200000;Check(TorchweedFusions.RoastedInterval(helper)==132,"source positive0.1second jitter quantizes to132ticks");
helper.mBoard.mPaused=true;int helperClock=helper.mLaunchCounter,helperPhase=helper.mStartRow;TorchweedFusions.Tick(helper);Check(helperClock==helper.mLaunchCounter&&helperPhase==helper.mStartRow,"helper pause preserves both clocks");
var spikeCar=Make(FusionPlant.RoastedSpikeweed);TorchweedFusions.Initialize(spikeCar);var car=new Zombie{mBoard=spikeCar.mBoard,mRow=spikeCar.mRow,X=spikeCar.mX+35,mZombieType=ZombieType.Zamboni};var neighbor=new Zombie{mBoard=spikeCar.mBoard,mRow=spikeCar.mRow,X=spikeCar.mX+35};spikeCar.mBoard.m_zombies.AddRange(new[]{car,neighbor});TorchweedFusions.Tick(spikeCar);Check(spikeCar.mStartRow==80&&!car.mDead,"vehicle contact requests original clip without immediate kill");for(int i=0;i<40;i++)TorchweedFusions.Tick(spikeCar);Check(car.VehicleDeaths==1&&car.LootDrops==1&&spikeCar.mDead&&neighbor.Health==980,"AnimAttack preserves vehicle-specific death then ordinary neighbors before plant removal");
var fireCar=Make(FusionPlant.Torchweed);TorchweedFusions.Initialize(fireCar);var catapult=new Zombie{mBoard=fireCar.mBoard,mRow=fireCar.mRow,X=fireCar.mX+35,mZombieType=ZombieType.Catapult};var fireNeighbor=new Zombie{mBoard=fireCar.mBoard,mRow=fireCar.mRow,X=fireCar.mX+35};fireCar.mBoard.m_zombies.AddRange(new[]{catapult,fireNeighbor});fireCar.mLaunchCounter=1;TorchweedFusions.Tick(fireCar);Check(catapult.VehicleDeaths==1&&catapult.LootDrops==1&&fireCar.mDead&&fireNeighbor.Health==960,"fire pulse completes ordinary damage after its vehicle-special branch");
var armoredBalloon=new Zombie{mBoard=weed.mBoard,mZombieType=ZombieType.Balloon,Flying=true,mFlyingHealth=100};
TorchweedFusions.FireDamage(armoredBalloon);Check(armoredBalloon.mFlyingHealth==60&&armoredBalloon.Flying&&armoredBalloon.Health==1000,"modified100HP armor receives first40 without forced pop");
TorchweedFusions.FireDamage(armoredBalloon);TorchweedFusions.FireDamage(armoredBalloon);Check(armoredBalloon.mFlyingHealth==0&&armoredBalloon.Landings==1&&armoredBalloon.Health==1000,"armor-breaking hit discards only overflow after native armor consumption");
TorchweedFusions.FireDamage(armoredBalloon);Check(armoredBalloon.Health==960&&armoredBalloon.Landings==1,"popping phase with exhausted armor takes40 body and never repops");
var extremeBalloon=new Zombie{mBoard=weed.mBoard,mZombieType=ZombieType.Balloon,Flying=true,mFlyingHealth=100,DamageDivisor=2};for(int i=0;i<4;i++)TorchweedFusions.FireDamage(extremeBalloon);Check(extremeBalloon.mFlyingHealth==20&&extremeBalloon.Landings==0,"native receiver modifier halves armor damage before distribution");TorchweedFusions.FireDamage(extremeBalloon);TorchweedFusions.FireDamage(extremeBalloon);Check(extremeBalloon.mFlyingHealth==0&&extremeBalloon.Health==980&&extremeBalloon.Landings==1,"receiver modifier applies once to later body hit");
var helmetBalloon=new Zombie{mBoard=weed.mBoard,mZombieType=ZombieType.Balloon,Flying=true,mHelmHealth=100};TorchweedFusions.FireDamage(helmetBalloon);Check(helmetBalloon.mFlyingHealth==0&&helmetBalloon.mHelmHealth==100&&helmetBalloon.Health==1000,"first flying-armor hit cannot overflow into additional helmet");TorchweedFusions.FireDamage(helmetBalloon);Check(helmetBalloon.mHelmHealth==60,"later hit retains ordinary helmet routing");
var nestedBalloon=new Zombie{mBoard=weed.mBoard,mZombieType=ZombieType.Balloon,Flying=true};nestedBalloon.ArmorCallback=()=>nestedBalloon.TakeDamage(5,DamageFlags.BypassesShield);TorchweedFusions.FireDamage(nestedBalloon);Check(nestedBalloon.Health==995&&TorchweedBalloonArmor.Current==null&&TorchweedBalloonArmor.Depth==0,"recursive unrelated damage is preserved and both synchronous contexts restore");
var failedBalloon=new Zombie{mBoard=weed.mBoard,mZombieType=ZombieType.Balloon,Flying=true};failedBalloon.ArmorCallback=()=>throw new InvalidOperationException("armor callback");try{TorchweedFusions.FireDamage(failedBalloon);}catch(InvalidOperationException){}Check(TorchweedBalloonArmor.Current==null&&TorchweedBalloonArmor.Depth==0,"exception restores armor scope and recursion depth");
// Compile the real movement owner and both conversion implementations. Calling
// Move is essential: custom rolling motion never enters native CheckForCollision.
var movingHost=Make(FusionPlant.Torchweed);TorchweedFusions.Initialize(movingHost);var movingShot=Shot(movingHost);RollingProjectiles.Track(movingShot,true);Check(RollingProjectiles.TryShot(movingShot,out var movingState),"real rolling state exists before custom movement");var priorVictim=new Zombie{mBoard=movingHost.mBoard,mRow=movingHost.mRow,X=movingHost.mX+38};movingHost.mBoard.m_zombies.Add(priorVictim);movingState.Hit.Add(priorVictim.DataID);RollingProjectiles.Move(movingShot,movingState);Check(movingShot.mCobTargetX==RollingProjectiles.IgnitedNut&&movingShot.mCobTargetRow==40&&movingHost.mTargetX==1,"actual Move reaches Torchweed conversion and saves fire damage marker");Check(priorVictim.Health==960,"actual Move can hit a previous target again after ignition clears history");RollingProjectiles.Move(movingShot,movingState);Check(priorVictim.Health==960&&movingHost.mTargetX==1,"actual Move does not reignite or duplicate same-target damage on later tick");
var originalHot=Make(FusionPlant.HotPotato);ChargeTorchFusions.Initialize(originalHot);var originalRoll=Shot(originalHot);RollingProjectiles.Track(originalRoll,true);RollingProjectiles.TryShot(originalRoll,out var originalState);RollingProjectiles.Move(originalRoll,originalState);Check(originalRoll.mCobTargetX==RollingProjectiles.IgnitedNut&&originalRoll.mCobTargetRow==40&&originalHot.mTargetX==1,"existing Hot Potato conversion remains in the actual movement path");
var pausedHost=Make(FusionPlant.Torchweed);TorchweedFusions.Initialize(pausedHost);var pausedRoll=Shot(pausedHost);RollingProjectiles.Track(pausedRoll,true);float pausedX=pausedRoll.mPosX;pausedHost.mBoard.mPaused=true;var motion=typeof(RollingMotionPatch).GetMethod("Prefix",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!;Check(!(bool)motion.Invoke(null,new object[]{pausedRoll})!&&pausedRoll.mPosX==pausedX&&pausedRoll.mCobTargetX==0,"real motion hook pauses before Move and converter");pausedHost.mBoard.mPaused=false;motion.Invoke(null,new object[]{pausedRoll});Check(pausedRoll.mCobTargetX==RollingProjectiles.IgnitedNut&&pausedHost.mTargetX==1,"real motion hook resumes through Move and converter");
var context=new object();PlayerPlantingScope.Current=context;weed.mBoard.FailPlant=true;Check(!TorchweedFusions.SummonAhead(weed,FusionPlant.RoastedSpikeweed)&&ReferenceEquals(context,PlayerPlantingScope.Current),"failed summon restores outer planting transaction");

Console.WriteLine($"Torch fusion and index contracts: {checks} checks passed.");
