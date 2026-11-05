using FusionMod;
using Il2CppReloaded.Gameplay;
using System.Reflection;
int checks=0;
void Check(bool value,string reason){++checks;if(!value)throw new Exception(reason);}
Plant Make(FusionPlant identity) {
    var p=new Plant{mSeedType=identity.NativeType,mImitaterType=identity.Marker};ShooterGrowth.Initialize(p);return p;
}
Projectile Fire(Plant plant) {
    BlazeFireScope.Prefix(plant,out var old);
    try {var p=new Projectile{mBoard=plant.mBoard};BlazeShooters.Track(p);return p;}
    finally {BlazeFireScope.Finalizer(old);}
}
var recipes=new (FusionPlant A,FusionPlant B,FusionPlant R)[]{
    (SeedType.Jalapeno,SeedType.Peashooter,FusionPlant.BlazePea),
    (FusionPlant.BlazePea,SeedType.Peashooter,FusionPlant.BlazeRepeater),
    (SeedType.Jalapeno,SeedType.Repeater,FusionPlant.BlazeRepeater),
    (FusionPlant.BlazePea,SeedType.Repeater,FusionPlant.SplitBlaze),
    (FusionPlant.BlazeRepeater,SeedType.Peashooter,FusionPlant.SplitBlaze),
    (SeedType.Jalapeno,SeedType.Splitpea,FusionPlant.SplitBlaze),
    (FusionPlant.BlazePea,SeedType.Splitpea,FusionPlant.GatlingBlaze),
    (FusionPlant.BlazeRepeater,SeedType.Repeater,FusionPlant.GatlingBlaze),
    (SeedType.Gatlingpea,SeedType.Jalapeno,FusionPlant.GatlingBlaze),
    (SeedType.Peashooter,FusionPlant.SplitBlaze,FusionPlant.GatlingBlaze)};
foreach(var(a,b,r) in recipes){
    Check(FusionRecipes.TryGet(a,b,out var result)&&result==r,"source recipe");
    Check(FusionRecipes.TryGet(b,a,out result)&&result==r,"reverse recipe");
}
foreach(var identity in new[]{FusionPlant.BlazePea,FusionPlant.BlazeRepeater,FusionPlant.SplitBlaze,FusionPlant.GatlingBlaze}){
    var p=Make(identity);Check(FusionPlant.From(p)==identity&&FusionPlant.HasImportedVisuals(identity),"saved identity and model");
    var shot=Fire(p);Check(BlazeShooters.Contains(shot),"own shot");
    var z=new Zombie{mBoard=p.mBoard,mChilledCounter=100,mIceTrapCounter=50};
    shot.DoImpact(z);Check(z.Damage==45&&Overheat.Has(z),"heat before first thirty-damage hit");
    Check(z.mChilledCounter==0&&z.mIceTrapCounter==0,"heat removes cold");
    Check(!BlazeTorchwoodPatch.Prefix(shot),"ordinary Torchwood cannot ignite source bullet 175");
    Check(BlazeShooters.CurrentImpact==null&&!BlazeShooters.DamagePending,"impact scope restored");
    BlazeShooters.Forget(shot);Check(!BlazeShooters.Contains(shot),"death cleanup");
}
var plain=Make(SeedType.Peashooter);var ordinary=Fire(plain);
Check(!BlazeShooters.Contains(ordinary)&&BlazeTorchwoodPatch.Prefix(ordinary),"native pea unchanged");
var plant=Make(FusionPlant.BlazePea);var bullet=Fire(plant);
var enemy=new Zombie{mBoard=plant.mBoard};var ally=new Zombie{mBoard=plant.mBoard,mMindControlled=true};
Check(BlazeShooters.BeginImpact(bullet,ally)==null&&!Overheat.Has(ally),"ally excluded");
var boss=new Zombie{mBoard=plant.mBoard,mZombieType=ZombieType.Boss};bullet.DoImpact(boss);
Check(boss.Damage==30&&!Overheat.Has(boss),"boss retains base damage and native status exclusion");
var foreign=new Zombie{mBoard=new Board{Pointer=(IntPtr)90}};
Check(BlazeShooters.BeginImpact(bullet,foreign)==null,"foreign board excluded");
bullet.DataID++;Check(!BlazeShooters.Contains(bullet),"pooled projectile cannot inherit origin");
bullet=Fire(plant);BlazeShooters.ForgetBoard(plant.mBoard);Check(!BlazeShooters.Contains(bullet),"board cleanup");
bullet=Fire(plant);BlazeImpactScope.Prefix(bullet,enemy,out var outer);
try {
    Check(BlazeShooters.CurrentImpact?.Zombie==enemy,"outer context");
    var other=new Projectile{Pointer=(IntPtr)50,mBoard=plant.mBoard};BlazeImpactScope.Prefix(other,enemy,out var nested);
    try {Check(BlazeShooters.CurrentImpact==null,"unrelated nested impact clears context");}
    finally {BlazeImpactScope.Finalizer(nested);}
    int damage=20;BlazeDamagePatch.Prefix(enemy,ref damage);Check(damage==30,"nested context restored");
    damage=100;BlazeDamagePatch.Prefix(enemy,ref damage);Check(damage==100,"only one direct damage overridden");
}finally{BlazeImpactScope.Finalizer(outer);}
BlazeImpactScope.Prefix(bullet,enemy,out outer);enemy.DataID++;int changed=20;BlazeDamagePatch.Prefix(enemy,ref changed);
Check(changed==20,"pooled victim excluded");BlazeImpactScope.Finalizer(outer);
var priority=typeof(BlazeDamagePatch).GetMethod("Prefix",BindingFlags.NonPublic|BindingFlags.Static)!.GetCustomAttribute<HarmonyLib.HarmonyPriority>();
Check(priority?.Value==HarmonyLib.Priority.First,"base damage runs before shared multiplier");
foreach(var identity in new[]{FusionPlant.BlazeRepeater,FusionPlant.SplitBlaze}){
    plant=Make(identity);Check(plant.mStartRow==1500,"fifteen-second saved countdown");
    Check(!ShooterGrowth.Grow(plant,1,0),"early click denied");
    for(int i=0;i<1499;i++)ShooterGrowth.Tick(plant);
    Check(plant.mStartRow==1&&!ShooterGrowth.Grow(plant,1,0),"boundary before readiness");
    // Native serialization persists mStartRow. Restore fields without initializing.
    var restored=new Plant{mSeedType=plant.mSeedType,mImitaterType=plant.mImitaterType,mStartRow=plant.mStartRow};
    ShooterGrowth.Tick(restored);Check(restored.mStartRow==0,"saved countdown resumes");
    plant.mStartRow=0;plant.mPlantHealth=121;
    Check(ShooterGrowth.Grow(plant,1,0)&&plant.mDead,"ready click replaces original");
    var grown=plant.mBoard.Created!;
    Check(FusionPlant.From(grown)==(identity==FusionPlant.BlazeRepeater?FusionPlant.SplitBlaze:FusionPlant.GatlingBlaze),"correct upgrade");
    Check(grown.mPlantHealth==121,"growth preserves damage");
    Check(grown.mStartRow==(identity==FusionPlant.BlazeRepeater?1500:0),"next growth timer independent");
}
foreach(string mode in new[]{"pause","dead","carry","bungee","loading","garden","dispose","cursor","held","blocked","allocation","right","player"}){
    plant=Make(FusionPlant.BlazeRepeater);plant.mStartRow=0;
    if(mode=="pause")plant.mBoard.mPaused=true;
    if(mode=="dead")plant.mDead=true;
    if(mode=="carry")plant.OffGround=true;
    if(mode=="bungee")plant.mOnBungeeState=PlantOnBungeeState.OnBungee;
    if(mode=="loading")plant.mApp.GameScene=GameScenes.Loading;
    if(mode=="garden")plant.mApp.GameMode=GameMode.ChallengeZenGarden;
    if(mode=="dispose")CherryBombPlants.DisposingBoard=plant.mBoard.Pointer;
    if(mode=="cursor")plant.mBoard.mCursorObject.CursorType=CursorType.Shovel;
    if(mode=="held")plant.mBoard.Carried=plain;
    if(mode=="blocked")plant.mBoard.BlockPlacement=true;
    if(mode=="allocation")plant.mBoard.FailCreate=true;
    Check(!ShooterGrowth.Grow(plant,mode=="right"?-1:1,mode=="player"?1:0),"growth denial "+mode);
    Check(plant.mBoard.Created==null,"failed growth retains original "+mode);
    if(mode is "pause" or "dead" or "carry" or "bungee" or "loading" or "garden" or "dispose"){
        plant.mStartRow=100;ShooterGrowth.Tick(plant);Check(plant.mStartRow==100,"inactive timer "+mode);
    }
    CherryBombPlants.DisposingBoard=IntPtr.Zero;
}
Check(!ShooterGrowth.Grow(Make(FusionPlant.BlazePea),1,0),"single has no click upgrade");
Check(!ShooterGrowth.Grow(Make(FusionPlant.GatlingBlaze),1,0),"gatling has no click upgrade");
Console.WriteLine($"Blaze shooters: {checks} assertions passed; no game executed.");
