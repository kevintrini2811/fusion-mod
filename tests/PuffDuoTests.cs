using FusionMod;
using Il2CppReloaded.Gameplay;
int checks=0;
void Check(bool condition,string message) { ++checks;if(!condition)throw new Exception(message); }
Plant Make(FusionPlant identity) { var plant=new Plant {mSeedType=identity.NativeType,mImitaterType=identity.Marker};PuffSupport.Initialize(plant);PuffContactFusions.Initialize(plant);return plant; }
foreach(var (ingredient,result) in new[] {(SeedType.Cherrybomb,FusionPlant.CherryPop),(SeedType.Cactus,FusionPlant.BallCactus)}) {
 Check(FusionRecipes.TryGet(SeedType.Puffshroom,ingredient,out var value)&&value==result,"Source recipe");
 Check(FusionRecipes.TryGet(ingredient,SeedType.Puffshroom,out value)&&value==result,"Reverse recipe");
 var plant=Make(result);Check(FusionPlant.From(plant)==result&&plant.mPlantHealth==300,"Saved identity and health");
 Check(FusionPlant.IsPuffVariant(result)&&FusionPlant.HasImportedVisuals(result),"Small plant art and native shot suppression");
}
var ball=Make(FusionPlant.BallCactus);
var ground=new Zombie {mBoard=ball.mBoard,DataID=1,X=600,Category=DamageRangeFlags.Ground};ball.mBoard.m_zombies.Add(ground);
var air=new Zombie {mBoard=ball.mBoard,DataID=2,X=600,Category=DamageRangeFlags.Flying};
Check(PuffContactFusions.BallTarget(ball,ground)&&PuffContactFusions.BallTarget(ball,air),"Full lane ground and air acquisition");
ground.X=200;Check(!PuffContactFusions.BallTarget(ball,ground),"Targets behind the muzzle cannot start a forward shot");ground.X=600;
ground.mMindControlled=true;Check(!PuffContactFusions.BallTarget(ball,ground),"Friendly targets cannot start attack");ground.mMindControlled=false;
air.mRow=1;Check(!PuffContactFusions.BallTarget(ball,air),"Other row excluded");
ball.mIsAsleep=true;PuffContactFusions.TickBall(ball);Check(ball.mStartRow==0,"Sleep blocks attack");ball.mIsAsleep=false;
for(int i=0;i<25;++i)PuffContactFusions.TickBall(ball);Check(ball.mBoard.Projectiles.Count==0,"No shot before source animation event");
PuffContactFusions.TickBall(ball);Check(ball.mBoard.Projectiles.Count==1,"Exactly one shot at .25 seconds");
int phase=ball.mStartRow;ball.mBoard.mPaused=true;PuffContactFusions.TickBall(ball);Check(ball.mStartRow==phase,"Pause preserves saved phase");ball.mBoard.mPaused=false;
for(int i=0;i<100;++i)PuffContactFusions.TickBall(ball);Check(ball.mBoard.Projectiles.Count==2,"One second cadence without animation duplicates");
var pop=Make(FusionPlant.CherryPop);var enemy=new Zombie {mBoard=pop.mBoard,DataID=3,X=240,Category=DamageRangeFlags.Ground};pop.mBoard.m_zombies.Add(enemy);
PuffContactFusions.Explode(IceInstantFusions.Capture(pop));Check(CherrySpreader.Bombs[enemy]==900&&CherryBombPlants.Blasts.Last()==900,"Attach base damage and initial blast");
CherrySpreader.Bombs.Clear();enemy.Category=DamageRangeFlags.Flying;PuffContactFusions.Explode(IceInstantFusions.Capture(pop));Check(CherrySpreader.Bombs[enemy]==900,"The explosion callback can attach to a flying victim; only contact requires ground");
Check(!PuffContactFusions.Contact(pop,enemy)&&!pop.mDead,"Flying bite cannot trigger contact");enemy.Category=DamageRangeFlags.Ground;
Check(PuffContactFusions.Contact(pop,enemy)&&pop.mDead,"Ground contact consumes Cherry Pop");
pop=Make(FusionPlant.CherryPop);pop.mIsAsleep=true;Check(!PuffContactFusions.Contact(pop,enemy),"Sleeping Cherry Pop waits");
pop=Make(FusionPlant.CherryPop);enemy=new Zombie {mBoard=pop.mBoard,DataID=4,Category=DamageRangeFlags.Ground};pop.mBoard.m_zombies.Add(enemy);
enemy.X=pop.mX+pop.mWidth/2f+PuffContactFusions.CherryContactOffset+PuffContactFusions.CherryContactHalfWidth+20+.1f;
Check(!PuffContactFusions.Contact(pop),"Outside the actual callback collider does not detonate");
enemy.X-=.2f;Check(PuffContactFusions.Contact(pop),"Entering the original asymmetric callback collider detonates");
var prefix=typeof(CherryPopDeathPatch).GetMethod("Prefix",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!;
var postfix=typeof(CherryPopDeathPatch).GetMethod("Postfix",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!;
pop=Make(FusionPlant.CherryPop);object?[] state={pop,true,null};prefix.Invoke(null,state);Check(state[2]!=null,"Ordinary death captures explosion");
int blasts=CherryBombPlants.Blasts.Count;pop.mDead=true;++pop.DataID;postfix.Invoke(null,new[]{(object)pop,state[2]});Check(CherryBombPlants.Blasts.Count==blasts,"Recycled plant cannot emit old death effect");
pop=Make(FusionPlant.CherryPop);PlayerPlantingScope.ConsumingIngredient=pop.Pointer;state=new object?[]{pop,true,null};prefix.Invoke(null,state);Check(state[2]==null,"Ingredient consumption cannot explode");PlayerPlantingScope.ConsumingIngredient=IntPtr.Zero;
CherryBombPlants.DisposingBoard=pop.mBoard.Pointer;prefix.Invoke(null,state);Check(state[2]==null,"Board disposal cannot explode");CherryBombPlants.DisposingBoard=IntPtr.Zero;
Check(FusionRecipes.IsPuffSpreading(SeedType.Puffshroom,SeedType.Fumeshroom)&&!FusionRecipes.IsPuffSpreading(SeedType.Fumeshroom,SeedType.Puffshroom),"Existing spread direction retained at recipe owner");
var balloonArmor=new Zombie {mScaleZombie=1,mBodyHealth=201,mBodyMaxHealth=270,mFlyingHealth=5,mFlyingMaxHealth=20};
PuffSupport.Shrink(balloonArmor);Check(balloonArmor.mBodyHealth==140&&balloonArmor.mBodyMaxHealth==189&&balloonArmor.mFlyingHealth==3&&balloonArmor.mFlyingMaxHealth==14,"Shrink scales native balloon armor and body separately");
PuffSupport.Shrink(balloonArmor);Check(balloonArmor.mFlyingHealth==3&&balloonArmor.mFlyingMaxHealth==14,"Repeated shrink cannot scale balloon armor twice");
Console.WriteLine($"Puff duo: {checks} checks passed.");
