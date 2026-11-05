using FusionMod;
using Il2CppReloaded.Gameplay;
int checks=0;
void Check(bool condition,string message) {++checks;if(!condition)throw new Exception(message);}
Plant Make(FusionPlant id,int row=2) {var b=new Board();var p=new Plant {mBoard=b,mSeedType=id.NativeType,mImitaterType=id.Marker,mPlantCol=3,mRow=row,mX=240,mY=row*80};b.m_plants.Add(p);TorchFusions.Initialize(p);return p;}
Projectile Shot(Plant p,ProjectileType type=ProjectileType.Pea,float tag=0) => new() {mBoard=p.mBoard,mRow=p.mRow,mPosX=p.mX+32,mPosY=p.mY+40,mProjectileType=type,mCobTargetX=tag};
var tree=Make(FusionPlant.Torchthree);
for(int phase=0;phase<3;++phase) {var shot=Shot(tree);int before=tree.mBoard.Shots.Count;TorchFusions.Convert(shot);Check(shot.mDead,"input consumed once");Check(tree.mBoard.Shots.Count-before==phase+1,"1/2/3 ignition cycle");Check(tree.mTargetX==(phase+1)%3,"saved next phase");}
Check(tree.mBoard.Shots.Any(s=>s.mRow==1)&&tree.mBoard.Shots.Any(s=>s.mRow==3),"neighbor lanes");
tree=Make(FusionPlant.Torchthree,0);tree.mTargetX=2;TorchFusions.Convert(Shot(tree));Check(tree.mBoard.Shots.Count==3&&tree.mBoard.Shots.Count(s=>s.mRow==0)==2,"edge keeps all three peas");
var nut=Make(FusionPlant.Nutwood);Check(nut.mPlantMaxHealth==4000,"nut toughness");nut.mTargetY=49;nut.mBoard.Full=true;TorchFusions.Convert(Shot(nut));Check(nut.mTargetY==50,"full row retains summon credit");nut.mBoard.Full=false;TorchFusions.Convert(Shot(nut));Check(nut.mTargetY==0&&nut.mBoard.m_plants.Any(p=>p.mSeedType==SeedType.Wallnut),"free row creates roasted nut");
var hypno=Make(FusionPlant.Hypnowood);hypno.mTargetY=59;TorchFusions.Convert(Shot(hypno));Check(HypnoShooters.Summons==0&&hypno.mTargetY==60,"strict source threshold");TorchFusions.Convert(Shot(hypno));Check(HypnoShooters.Summons==1&&hypno.mTargetY==0,"61st credit summons");
hypno.mTargetY=10;var colored=Shot(hypno,tag:-30);TorchFusions.Convert(colored);Check(hypno.mTargetY==14&&HypnoShooters.IsFire(colored),"hypno pea credits four");TorchFusions.Convert(Shot(hypno,tag:-20));Check(hypno.mTargetY==14,"ally cannot farm credit");
var snow=Shot(hypno,ProjectileType.Snowpea);TorchFusions.Convert(snow);Check(snow.mProjectileType==ProjectileType.Pea&&hypno.mTargetY==14,"thaw gives no credit");hypno.mBoard.mPaused=true;TorchFusions.Convert(Shot(hypno));Check(hypno.mTargetY==14,"pause");hypno.mBoard.mPaused=false;
var attacker=new Zombie {mBoard=hypno.mBoard};hypno.mBoard.FailPlant=true;Check(!TorchFusions.Bite(hypno,attacker)&&!hypno.mDead&&!attacker.mMindControlled,"failed replacement retains plant and enemy");hypno.mBoard.FailPlant=false;Check(TorchFusions.Bite(hypno,attacker)&&hypno.mDead&&attacker.mMindControlled,"bite atomically restores native torchwood");
Console.WriteLine($"Torch fusion contracts: {checks} checks passed.");
