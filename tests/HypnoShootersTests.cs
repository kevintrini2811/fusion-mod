using FusionMod;
using Il2CppReloaded.Gameplay;
using System.Reflection;
int checks=0;
void Check(bool value,string reason){++checks;if(!value)throw new Exception(reason);}
Plant Make(FusionPlant identity) {
    var p=new Plant{mSeedType=identity.NativeType,mImitaterType=identity.Marker};ShooterGrowth.Initialize(p);return p;
}
Projectile Fire(Plant plant) {
    HypnoFireScope.Prefix(plant,out var old);
    try{return plant.mBoard.AddProjectile();}finally{HypnoFireScope.Finalizer(old);}
}
Zombie Enemy(Board board,int hp=100)=>new(){mBoard=board,Health=hp,mPosX=420,mRow=2};
Projectile AllyFire(Zombie ally) {
    HypnoAllyFireScope.Prefix(ally,out var old);
    try{return ally.mBoard.AddProjectile();}finally{HypnoAllyFireScope.Finalizer(old);}
}
var recipes=new (FusionPlant A,FusionPlant B,FusionPlant R)[]{
    (SeedType.Hypnoshroom,SeedType.Peashooter,FusionPlant.HypnoPea),
    (FusionPlant.HypnoPea,SeedType.Peashooter,FusionPlant.HypnoRepeater),
    (SeedType.Hypnoshroom,SeedType.Repeater,FusionPlant.HypnoRepeater),
    (FusionPlant.HypnoPea,SeedType.Repeater,FusionPlant.SplitHypno),
    (FusionPlant.HypnoRepeater,SeedType.Peashooter,FusionPlant.SplitHypno),
    (SeedType.Hypnoshroom,SeedType.Splitpea,FusionPlant.SplitHypno),
    (SeedType.Gatlingpea,SeedType.Hypnoshroom,FusionPlant.GatlingHypno),
    (FusionPlant.HypnoPea,SeedType.Splitpea,FusionPlant.GatlingHypno),
    (FusionPlant.HypnoRepeater,SeedType.Repeater,FusionPlant.GatlingHypno),
    (SeedType.Peashooter,FusionPlant.SplitHypno,FusionPlant.GatlingHypno)};
foreach(var(a,b,r) in recipes){
    Check(FusionRecipes.TryGet(a,b,out var result)&&result==r,"source recipe");
    Check(FusionRecipes.TryGet(b,a,out result)&&result==r,"reverse recipe");
}
foreach(var identity in new[]{FusionPlant.HypnoPea,FusionPlant.HypnoRepeater,FusionPlant.SplitHypno,FusionPlant.GatlingHypno}) {
    var p=Make(identity);Check(FusionPlant.From(p)==identity&&FusionPlant.HasImportedVisuals(identity),"persisted plant identity and model");
    var b=p.mBoard;var z=Enemy(b);var shot=Fire(p);
    Check(HypnoShooters.Contains(shot),"plant shot tracked");shot.DoImpact(z);
    Check(z.Damage==30&&b.Summoned.Count==0,"thirty damage without premature summon");
    z=Enemy(b,30);Fire(p).DoImpact(z);Check(b.Summoned.Count==1,"lethal hit summons once");
    var ally=b.Summoned[0];Check(HypnoShooters.IsSummoned(ally)&&ally.Health==270&&ally.mBodyMaxHealth==270,"friendly PeaHead with source health");
    Check(ally.mRow==2&&ally.mPosX==420&&ally.mX==420,"summon at victim position");
    z=Enemy(b);AllyFire(ally).DoImpact(z);Check(z.Damage==20,"ally projectile uses twenty damage");
    z=Enemy(b,20);AllyFire(ally).DoImpact(z);Check(b.Summoned.Count==2,"ally projectile retains recursive summoning");
    Check(HypnoShooters.CurrentImpact==null&&HypnoShooters.FiringPlant==null&&HypnoShooters.FiringZombie==null,"contexts restored");
    shot=Fire(p);Check(!HypnoTorchwoodPatch.Prefix(shot),"ordinary Torchwood leaves hypno bullet unchanged");
    var loaded=new Projectile{mBoard=b,mProjectileType=shot.mProjectileType,mCobTargetX=shot.mCobTargetX};
    z=Enemy(b,30);loaded.DoImpact(z);Check(b.Summoned.Count==3,"loaded shot retains damage and summon");
    var loadedAlly=new Zombie{mBoard=b,mZombieType=ally.mZombieType,mMindControlled=ally.mMindControlled,mSummonCounter=ally.mSummonCounter};
    Check(HypnoShooters.Contains(AllyFire(loadedAlly)),"loaded ally retains special ammunition");
}
var plant=Make(FusionPlant.HypnoPea);var board=plant.mBoard;
var shielded=Enemy(board,30);shielded.Armor=30;Fire(plant).DoImpact(shielded);
Check(shielded.Armor==0&&shielded.Health==30&&board.Summoned.Count==0,"shield absorption cannot summon");
Fire(plant).DoImpact(shielded);Check(board.Summoned.Count==1,"body kill after shield summons");
var native=Fire(Make(SeedType.Peashooter));Check(!HypnoShooters.Contains(native)&&HypnoTorchwoodPatch.Prefix(native),"native pea unchanged");
var ordinaryAlly=new Zombie{mBoard=board,mZombieType=ZombieType.PeaHead,mMindControlled=true};
Check(!HypnoShooters.Contains(AllyFire(ordinaryAlly)),"ordinary hypnotized PeaHead unchanged");
foreach(var excluded in new[]{Enemy(board),Enemy(board),Enemy(new Board{Pointer=(IntPtr)99})}) {
    if(excluded.mBoard==board) {if(!excluded.mMindControlled)excluded.mMindControlled=true;}
    Check(HypnoShooters.BeginImpact(Fire(plant),excluded)==null,"friendly/foreign target excluded");
}
foreach(var type in new[]{ZombieType.Boss,ZombieType.Gravestone,ZombieType.Target}) {
    var z=Enemy(board,30);z.mZombieType=type;int before=board.Summoned.Count;Fire(plant).DoImpact(z);
    Check(z.Damage==30&&board.Summoned.Count==before,"non-zombie targets cannot create allies");
}
var victim=Enemy(board,30);var bullet=Fire(plant);var impact=HypnoShooters.BeginImpact(bullet,victim)!;
HypnoShooters.CurrentImpact=impact;victim.TakeDamage(20,0);int count=board.Summoned.Count;
HypnoShooters.FinishImpact(impact);HypnoShooters.FinishImpact(impact);
Check(board.Summoned.Count==count+1,"duplicate finish cannot duplicate ally");HypnoShooters.CurrentImpact=null;
victim=Enemy(board,30);impact=HypnoShooters.BeginImpact(Fire(plant),victim)!;impact.Hit=true;victim.Health=0;victim.DataID++;
count=board.Summoned.Count;HypnoShooters.FinishImpact(impact);Check(board.Summoned.Count==count,"pooled victim rejected");
victim=Enemy(board);bullet=Fire(plant);HypnoImpactScope.Prefix(bullet,victim,out var outer);
try {
    var plain=new Projectile{mBoard=board};HypnoImpactScope.Prefix(plain,victim,out var inner);
    try {int d=99;HypnoShooters.AdjustDamage(victim,ref d);Check(d==99,"nested native impact clears outer context");}
    finally{HypnoImpactScope.Finalizer(inner);}
    int damage=20;HypnoShooters.AdjustDamage(victim,ref damage);Check(damage==30,"outer context restored");
    damage=123;HypnoShooters.AdjustDamage(victim,ref damage);Check(damage==123,"secondary damage unchanged");
} finally {HypnoImpactScope.Finalizer(outer);}
victim=Enemy(board,30);victim.OnDamage=()=>throw new InvalidOperationException("damage failure");
try{Fire(plant).DoImpact(victim);}catch(InvalidOperationException){}
Check(HypnoShooters.CurrentImpact==null,"exception restores impact context");
board.FailSummon=true;Fire(plant).DoImpact(Enemy(board,30));board.FailSummon=false;
Check(HypnoShooters.CurrentImpact==null,"failed allocation retains valid scope");
CherryBombPlants.DisposingBoard=board.Pointer;count=board.Summoned.Count;Fire(plant).DoImpact(Enemy(board,30));
Check(board.Summoned.Count==count,"board disposal cannot summon");CherryBombPlants.DisposingBoard=IntPtr.Zero;
foreach(var identity in new[]{FusionPlant.HypnoRepeater,FusionPlant.SplitHypno}) {
    var p=Make(identity);Check(p.mStartRow==1500&&!ShooterGrowth.Grow(p,1,0),"growth waits fifteen seconds");
    p.mBoard.mPaused=true;ShooterGrowth.Tick(p);Check(p.mStartRow==1500,"pause stops growth");p.mBoard.mPaused=false;
    p.mOnBungeeState=PlantOnBungeeState.OnBungee;ShooterGrowth.Tick(p);Check(p.mStartRow==1500,"bungee stops growth");p.mOnBungeeState=0;
    for(int i=0;i<1500;i++)ShooterGrowth.Tick(p);
    Check(p.mStartRow==0,"growth uses simulation ticks");p.mBoard.FailCreate=true;
    Check(!ShooterGrowth.Grow(p,1,0)&&!p.mDead,"failed growth retains plant");p.mBoard.FailCreate=false;
    p.mBoard.BlockPlacement=true;Check(!ShooterGrowth.Grow(p,1,0),"growth validates terrain");p.mBoard.BlockPlacement=false;
    p.mBoard.mCursorObject.CursorType=CursorType.Glove;Check(!ShooterGrowth.Grow(p,1,0),"glove click cannot grow");p.mBoard.mCursorObject.CursorType=CursorType.Normal;
    p.mPlantHealth=123;Check(ShooterGrowth.Grow(p,1,0)&&p.mDead,"successful click consumes old plant");
    var grown=p.mBoard.Created!;Check(FusionPlant.From(grown)==(identity==FusionPlant.HypnoRepeater?FusionPlant.SplitHypno:FusionPlant.GatlingHypno)&&grown.mPlantHealth==123,"correct next stage preserves damage");
}
var specialAlly=board.Summoned[0];
HypnoAllyFireScope.Prefix(specialAlly,out var allyOuter);
try {
    Check(HypnoShooters.Contains(board.AddProjectile()),"ally fire scope active");
    HypnoFireScope.Prefix(Make(SeedType.Peashooter),out var ordinaryInner);
    try {Check(!HypnoShooters.Contains(board.AddProjectile()),"nested ordinary plant cannot inherit allied ammunition");}
    finally {HypnoFireScope.Finalizer(ordinaryInner);}
    Check(HypnoShooters.Contains(board.AddProjectile()),"allied context restored after nested plant fire");
}finally{HypnoAllyFireScope.Finalizer(allyOuter);}
HypnoFireScope.Prefix(plant,out var plantOuter);
try {
    HypnoAllyFireScope.Prefix(ordinaryAlly,out var ordinaryInner);
    try {Check(!HypnoShooters.Contains(board.AddProjectile()),"nested ordinary zombie cannot inherit plant ammunition");}
    finally {HypnoAllyFireScope.Finalizer(ordinaryInner);}
    Check(HypnoShooters.Contains(board.AddProjectile()),"plant context restored after nested zombie fire");
}finally{HypnoFireScope.Finalizer(plantOuter);}
Check(specialAlly.mPhaseCounter>=50&&specialAlly.mPhaseCounter<=150,"source randomized startup interval");
var split=Fire(Make(FusionPlant.SplitHypno));split.mMotionType=ProjectileMotion.Backwards;split.mPosX=-3;
SplitPeaProjectiles.Reflect(split);Check(split.mMotionType==ProjectileMotion.Straight&&split.mPosX==3&&HypnoShooters.Contains(split),"loaded rear shot rebounds and retains hypno identity");
var targets=(IEnumerable<MethodBase>)typeof(HypnoAllyFireScope).GetMethod("TargetMethods",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,null)!;
Check(targets.Any(m=>m.Name==nameof(Zombie.UpdateActions))&&targets.Any(m=>m.Name==nameof(Zombie.UpdateZombiePeaHead)),"inlined and direct ally firing paths hooked");
// Fire conversion preserves the lethal-hit contract with the native Gatling AI.
var fireInput=Fire(Make(FusionPlant.HypnoPea));
HypnoShooters.Ignite(fireInput,3);
Check(HypnoShooters.Contains(fireInput)&&HypnoShooters.IsFire(fireInput)&&fireInput.mCobTargetRow==60,"ignited plant hypno pea carries 60 saved damage");
Check(!HypnoShooters.FromAlly(fireInput),"plant ignition counts toward Hypnowood credit");
var fireAlly=HypnoShooters.SummonFire(board,2,420)!;
Check(fireAlly.mZombieType==ZombieType.GatlingHead&&HypnoShooters.IsSummoned(fireAlly)&&fireAlly.mPosX==420,"fire summon uses native Gatling and a saved marker");
HypnoAllyFireScope.Prefix(fireAlly,out var fireScope);
try {
    var child=board.AddProjectile();
    Check(HypnoShooters.IsFire(child)&&child.mCobTargetRow==40&&HypnoShooters.FromAlly(child),"fire ally shots remain recognizable and cannot farm ignition credit");
} finally { HypnoAllyFireScope.Finalizer(fireScope); }
board.FailSummon=true;
Check(HypnoShooters.SummonFire(board,0,100)==null,"summon allocation failure stays safe");
board.FailSummon=false;
Console.WriteLine($"Hypno shooters: {checks} checks passed.");
