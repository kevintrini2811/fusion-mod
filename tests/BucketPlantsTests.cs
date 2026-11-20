using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;

int assertions=0;
void Check(bool condition,string message) { ++assertions; if(!condition) throw new Exception(message); }
object? Call(Type type,string name,params object?[] args)=>type.GetMethod(name,BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,args);
Plant Make(FusionPlant identity,Board? board=null,int col=1)=>new(){mSeedType=identity.NativeType,
    mImitaterType=identity.Marker,mBoard=board??new(),mPlantCol=col,mRow=1};
foreach(var (ingredient,result) in new[]{(SeedType.Peashooter,FusionPlant.Buckshooter),(SeedType.Wallnut,FusionPlant.BuckNut)})
{
    Check(FusionRecipes.TryGet(ingredient,FusionPlant.Bucket,out var f)&&f==result,"Forward recipe");
    Check(FusionRecipes.TryGet(FusionPlant.Bucket,ingredient,out f)&&f==result,"Reverse recipe");
    Check(FusionPlant.From(Make(result))==result,"Saved fusion identity");
}
Check(FusionPlant.From(Make(FusionPlant.Bucket))==FusionPlant.Bucket,"Saved bucket identity");
foreach(var (identity,health) in new[]{(FusionPlant.Bucket,2000),(FusionPlant.BuckNut,8000),(FusionPlant.Buckshooter,2000)})
{
    var p=Make(identity); Call(typeof(BucketInitializePatch),"Postfix",p);
    Check(p.mPlantHealth==health&&p.mPlantMaxHealth==health,"Source toughness");
    if(identity==FusionPlant.Buckshooter) Check(p.mLaunchRate==150,"Source fire interval");
    if(identity==FusionPlant.Bucket) Check(p.mState==PlantState.Notready&&p.mStateCountdown==0,"Low inert bucket");
    p.mPlantHealth=health-100; BucketPlants.Feed(p); Check(p.mPlantHealth==health,"Healing cap");
    p.mPlantHealth=1; BucketPlants.Feed(p); Check(p.mPlantHealth==501,"500 healing");
    p.mPlantHealth=health+50; BucketPlants.Feed(p); Check(p.mPlantHealth==health+50,"Preserve excess health");
}
var board=new Board();
var magnet=Make(SeedType.Magnetshroom,board);board.Plants[(1,1)]=magnet;
magnet.mMagnetItems[0].ItemType=MagnetItemType.Pail2;
Check(!BucketPlants.MouseDown(board,1,1,1,0)&&BucketPlants.Holding(magnet),"Select captured pail");
Check(!BucketPlants.MouseDown(board,10,1,1,0)&&board.Created==0,"Outside board preserves pail");
board.Terrain=PlantingReason.NotHere;
Check(!BucketPlants.MouseDown(board,2,1,1,0)&&board.Created==0&&BucketPlants.Holding(magnet),"Invalid terrain preserves pail");
board.Terrain=PlantingReason.Ok;board.FailCreation=true;
BucketPlants.MouseDown(board,2,1,1,0);
Check(magnet.mMagnetItems[0].ItemType==MagnetItemType.Pail2&&BucketPlants.Holding(magnet),"Allocation failure retains ingredient");
board.FailCreation=false;
BucketPlants.MouseDown(board,2,1,1,0);
Check(FusionPlant.From(board.LastCreated!)==FusionPlant.Bucket,"Place standalone bucket");
Check(magnet.mMagnetItems[0].ItemType==MagnetItemType.None&&board.mCursorObject.CursorType==CursorType.Normal,"Successful placement consumes once");
magnet.mMagnetItems[0].ItemType=MagnetItemType.Pail1;
BucketPlants.MouseDown(board,1,1,1,0);
board.mPaused=true;BucketPlants.UpdateCursor();
Check(board.mCursorObject.CursorType==CursorType.Glove,"Pause preserves selection");
BucketPlants.MouseDown(board,3,1,1,0);
Check(board.Created==1,"Pause cannot place");
board.mPaused=false;
BucketPlants.MouseDown(board,0,0,-1,0);
Check(magnet.mMagnetItems[0].ItemType==MagnetItemType.Pail1&&board.mCursorObject.CursorType==CursorType.Normal,"Cancel preserves pail");
var pea=Make(SeedType.Peashooter,board,3);board.Plants[(3,1)]=pea;
BucketPlants.MouseDown(board,1,1,1,0);BucketPlants.MouseDown(board,3,1,1,0);
Check(pea.mDead&&FusionPlant.From(board.LastCreated!)==FusionPlant.Buckshooter,"Direct item fusion consumes only after creation");
var iron=board.LastCreated!;iron.mPlantMaxHealth=2000;iron.mPlantHealth=1000;
Check(!BucketPlants.CanFeed(Make(FusionPlant.Bucket),FusionPlant.Bucket),"Standalone bucket is not an iron fusion");
magnet.mMagnetItems[0].ItemType=MagnetItemType.Pail3;
BucketPlants.MouseDown(board,1,1,1,0);BucketPlants.MouseDown(board,3,1,1,0);
Check(iron.mPlantHealth==1500&&board.Created==2,"Feed existing iron plant without replacement");
var carried=Make(FusionPlant.Bucket,board,4);
Check(FusionService.CanPlaceCarried(board,carried,3,1),"Glove supports feeding");
Check(FusionService.TryPlaceCarried(board,carried,3,1)&&carried.mDead&&iron.mPlantHealth==2000,"Glove feeding consumes bucket");
var native=Make(SeedType.Peashooter);Call(typeof(BucketInitializePatch),"Postfix",native);
Check(native.mPlantHealth==300,"Native plant unchanged");
magnet.mMagnetItems[0].ItemType=MagnetItemType.Pail1;
BucketPlants.MouseDown(board,1,1,1,0);magnet.DataID++;
BucketPlants.UpdateCursor();
Check(!BucketPlants.Holding(magnet),"Pool reuse invalidates selection");
magnet.mMagnetItems[0].ItemType=MagnetItemType.Pail1;
BucketPlants.MouseDown(board,1,1,1,0);
Check(BucketPlants.IsTarget(iron),"Held pail highlights valid feeding target");
Call(typeof(BucketGloveToolChangePatch),"Prefix");
Check(magnet.mMagnetItems[0].ItemType==MagnetItemType.Pail1
    &&board.mCursorObject.CursorType==CursorType.Normal,"Same-enum glove switch releases selection without consumption");
var shooter=Make(FusionPlant.Buckshooter,board);
var shot=new Projectile(){mBoard=board};
IronProjectiles.Firing=shooter;IronProjectiles.Track(shot);IronProjectiles.Firing=null;
Check(IronProjectiles.Contains(shot),"Track scoped iron shot");
shot.DataID++;Check(!IronProjectiles.Contains(shot),"Pool reuse cannot inherit iron origin");
IronProjectiles.Track(shot);Check(!IronProjectiles.Contains(shot),"Unscoped shots remain native");
IronProjectiles.Firing=shooter;IronProjectiles.Track(shot);IronProjectiles.Firing=null;
var zombie=new Zombie(){mShieldHealth=1100,mPosX=400};
object?[] impact=[shot,zombie,null];Call(typeof(IronImpactScope),"Prefix",impact);
object?[] damage=[zombie,20,null];Call(typeof(IronDamagePatch),"Prefix",damage);
Check((int)damage[1]! == 1180,"80 plus current shield health");
Call(typeof(IronDamagePatch),"Postfix",zombie,damage[2]);Check(zombie.mPosX==416,"Original .2-unit knockback");
object?[] second=[zombie,20,null];Call(typeof(IronDamagePatch),"Prefix",second);
Check((int)second[1]! == 20,"Scoped damage once only");
Call(typeof(IronImpactScope),"Finalizer",impact[2]);
zombie.mZombieType=ZombieType.Gargantuar;IronProjectiles.Knockback(zombie);
Check(zombie.mPosX==416,"Do not displace heavy targets");
zombie.mZombieType=ZombieType.Normal;zombie.mDead=true;IronProjectiles.Knockback(zombie);
Check(zombie.mPosX==416,"Do not displace dead targets");
Check(!(bool)Call(typeof(IronTorchwoodPatch),"Prefix",shot)!,"Iron stays iron through Torchwood");
IronProjectiles.ForgetBoard(board);Check(!IronProjectiles.Contains(shot),"Board cleanup");
var smallIron=Make(FusionPlant.BuckShroom,board);smallIron.mPlantMaxHealth=1000;smallIron.mPlantHealth=700;
Check(BucketPlants.IsIron(FusionPlant.From(smallIron)) && BucketPlants.CanFeed(smallIron,FusionPlant.Bucket),"Small iron supports source bucket healing");
Call(typeof(BucketInitializePatch),"Postfix",smallIron);
Check(smallIron.mPlantMaxHealth==1000 && smallIron.mPlantHealth==700,"Bucket initialization cannot replace Puff toughness");
BucketPlants.Feed(smallIron);Check(smallIron.mPlantHealth==1000,"Small iron healing capped at source toughness");
var smallShot=new Projectile(){mBoard=board,mCobTargetX=IronProjectiles.SmallShot};
Check(IronProjectiles.Contains(smallShot)&&IronProjectiles.IsSmall(smallShot),"Small iron projectile survives origin registry loss");
zombie.mDead=false;zombie.mShieldHealth=400;
object?[] smallImpact=[smallShot,zombie,null];Call(typeof(IronImpactScope),"Prefix",smallImpact);
object?[] smallDamage=[zombie,20,null];Call(typeof(IronDamagePatch),"Prefix",smallDamage);
Check((int)smallDamage[1]! ==480,"Small iron uses 80 plus current shield HP");
Call(typeof(IronImpactScope),"Finalizer",smallImpact[2]);
Check(!(bool)Call(typeof(IronTorchwoodPatch),"Prefix",smallShot)!,"Small iron cannot ignite");
BucketPlants.Clear();IronProjectiles.Clear();
// The chance is decided once after a complete damaging hit, never by death
// or generic DropHelm callbacks (which also run for magnet extraction).
BucketDrops.Clear();
var dropBoard=new Board { Pointer=(IntPtr)22 };
Zombie Armoured()=>new(){mBoard=dropBoard,mPosX=100,mPosY=100};
var bucketZombie=Armoured();
var hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
bucketZombie.mHelmHealth=0;
BucketDrops.AfterDamage(bucketZombie,hit);
Check(BucketDrops.Count==1,"Successful half of the 50 percent roll creates a drop");
int randomCalls=dropBoard.RandomCalls;
BucketDrops.AfterDamage(bucketZombie,hit);
Check(BucketDrops.Count==1&&dropBoard.RandomCalls==randomCalls,"Same hit cannot drop twice");
Check(BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None)==null,"Later damage on bare head cannot reroll");
BucketDrops.Clear();
bucketZombie=Armoured();hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
bucketZombie.mHelmHealth=0;dropBoard.RandomValues.Enqueue(1);
BucketDrops.AfterDamage(bucketZombie,hit);
Check(BucketDrops.Count==0,"Other half of the 50 percent roll produces no item");
bucketZombie=Armoured();hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
bucketZombie.mHelmHealth=500;BucketDrops.AfterDamage(bucketZombie,hit);
Check(BucketDrops.Count==0,"Partial helm damage cannot drop");
bucketZombie=Armoured();hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
bucketZombie.mHelmHealth=0;bucketZombie.mBodyHealth=0;
BucketDrops.AfterDamage(bucketZombie,hit);
Check(BucketDrops.Count==0,"Lethal overkill destroying helm and body produces no drop");
bucketZombie=Armoured();
Check(BucketDrops.BeforeDamage(bucketZombie,DamageFlags.DoesntLeaveBody)==null,"Instant-kill damage is excluded");
bucketZombie.mHelmType=HelmType.Football;
Check(BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None)==null,"Other helmet types cannot produce buckets");
bucketZombie=Armoured();hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
bucketZombie.DataID++;bucketZombie.mHelmHealth=0;BucketDrops.AfterDamage(bucketZombie,hit);
Check(BucketDrops.Count==0,"Pooled zombie reuse cannot inherit a drop");
bucketZombie=Armoured();bucketZombie.mMindControlled=true;
Check(BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None)==null,"Allies cannot create enemy bucket drops");
bucketZombie=Armoured();hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
CherryBombPlants.DisposingBoard=dropBoard.Pointer;bucketZombie.mHelmHealth=0;
BucketDrops.AfterDamage(bucketZombie,hit);CherryBombPlants.DisposingBoard=IntPtr.Zero;
Check(BucketDrops.Count==0,"Board teardown cannot create a drop");
// Native magnet extraction directly clears the helmet, without a damage hit.
bucketZombie=Armoured();bucketZombie.mHelmHealth=0;bucketZombie.mHelmType=HelmType.None;
Check(BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None)==null&&BucketDrops.Count==0,
    "Magnet-captured item has no second ground drop");
bucketZombie=Armoured();hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
bucketZombie.mHelmHealth=0;BucketDrops.AfterDamage(bucketZombie,hit);
Check(BucketDrops.TryPick(dropBoard,140,140,out var loose)&&loose!=null,"Loose original bucket can be clicked");
int age=loose!.Age;
dropBoard.mPaused=true;BucketDrops.Tick(dropBoard);
Check(loose.Age==age,"Pause freezes lifetime and physics");dropBoard.mPaused=false;
Check(!BucketPlants.MouseDown(dropBoard,140,140,1,0)&&BucketPlants.HoldingDrop(loose),"Ground pickup enters shared bucket selection");
for(int i=0;i<1000;i++) BucketDrops.Tick(dropBoard);
Check(BucketDrops.Count==1&&loose.Age==age,"Held bucket does not expire");
BucketPlants.MouseDown(dropBoard,0,0,-1,0);
Check(BucketDrops.Contains(loose)&&!BucketPlants.HoldingDrop(loose),"Cancel returns the same drop without duplication");
BucketPlants.MouseDown(dropBoard,140,140,1,0);dropBoard.FailCreation=true;
BucketPlants.MouseDown(dropBoard,2,1,1,0);
Check(BucketDrops.Contains(loose)&&BucketPlants.HoldingDrop(loose),"Failed planting preserves held ground drop");
dropBoard.FailCreation=false;BucketPlants.MouseDown(dropBoard,2,1,1,0);
Check(BucketDrops.Count==0&&FusionPlant.From(dropBoard.LastCreated!)==FusionPlant.Bucket,"Successful planting consumes ground drop once");
bucketZombie=Armoured();hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
bucketZombie.mHelmHealth=0;BucketDrops.AfterDamage(bucketZombie,hit);
BucketDrops.TryPick(dropBoard,140,140,out loose);
for(int i=0;i<800;i++) BucketDrops.Tick(dropBoard);
Check(BucketDrops.Count==1&&loose!.Height==0,"Bucket lands and remains until eight seconds");
BucketDrops.Tick(dropBoard);Check(BucketDrops.Count==0,"Bucket expires after eight seconds");
bucketZombie=Armoured();hit=BucketDrops.BeforeDamage(bucketZombie,DamageFlags.None);
bucketZombie.mHelmHealth=0;BucketDrops.AfterDamage(bucketZombie,hit);
BucketDrops.BeforeSave();Check(BucketDrops.Count==1,"Saving removes only transient visuals");
BucketDrops.Clear(dropBoard);Check(BucketDrops.Count==0,"Board cleanup removes owned drops");

var ironSquash = Make(FusionPlant.IronSquash);
ironSquash.mPlantHealth = 100; ironSquash.mPlantMaxHealth = 1000;
Check(BucketPlants.CanFeed(ironSquash, FusionPlant.Bucket), "Iron Squash accepts native bucket feeding");
BucketPlants.Feed(ironSquash);
Check(ironSquash.mPlantHealth == 600, "Iron Squash gains 500 health");
BucketPlants.Feed(ironSquash);
Check(ironSquash.mPlantHealth == 1000, "Iron Squash feeding respects maximum health");
Console.WriteLine($"{assertions} offline assertions passed. No game loaded or started.");

foreach(var type in new[]{FusionPlant.RustShroom,FusionPlant.MetalPult,FusionPlant.BoltPult,FusionPlant.IronStar}) {
 var upgrade=Make(type);upgrade.mPlantMaxHealth=2000;upgrade.mPlantHealth=1200;upgrade.mStartRow=42;
 Check(BucketPlants.CanFeed(upgrade,FusionPlant.Bucket),"metal upgrades accept repair buckets");
 Check(!BucketPlants.CanFeed(upgrade,FusionPlant.FootballHelmet),"unimplemented helmet upgrade not treated as repair");
 BucketPlants.Feed(upgrade);Check(upgrade.mPlantHealth==1700&&upgrade.mStartRow==42,"repair preserves attack phase");
 BucketPlants.Feed(upgrade);Check(upgrade.mPlantHealth==2000,"metal repair caps health");
}

var footballRepair=Make(FusionPlant.FootballShroom);footballRepair.mPlantMaxHealth=3000;footballRepair.mPlantHealth=100;
Check(BucketPlants.CanFeed(footballRepair,FusionPlant.Bucket),"Football accepts repair Bucket");
Check(!BucketPlants.CanFeed(footballRepair,FusionPlant.FootballHelmet),"Football Helmet does not become repair Bucket");
BucketPlants.Feed(footballRepair);Check(footballRepair.mPlantHealth==600,"Football Bucket repairs500");
footballRepair.mPlantHealth=2900;BucketPlants.Feed(footballRepair);Check(footballRepair.mPlantHealth==3000,"Football repair caps at3000");
footballRepair.mPlantHealth=3100;BucketPlants.Feed(footballRepair);Check(footballRepair.mPlantHealth==3100,"Football repair preserves existing excesshealth");
Console.WriteLine($"Football repair included: {assertions} checks");

// Captured physical items retain one owner and their paused age.
BucketDrops.Clear();
var capturing = Make(FusionPlant.LumosMagnet);
BucketDrops.Add(capturing.mBoard, capturing.mRow, capturing.mX + 30, capturing.mY + 20, FusionPlant.Bucket);
Check(BucketDrops.TakeForMagnet(capturing, 1000, 5, out _), "physical bucket transferred to native slot");
Check(BucketDrops.Count == 0 && capturing.mTargetY % 4 == 1, "one owner after capture");
var physical = BucketDrops.CaptureMagnet(capturing)!;
Check(physical != null && physical.Kind == 1, "pre-death physical snapshot");
capturing.mMagnetItems[0].ItemType = MagnetItemType.None;
BucketDrops.ReleaseFromMagnet(capturing, physical);
Check(BucketDrops.Count == 1 && capturing.mTargetY == 0, "snapshot restores item despite native slot cleanup");
BucketDrops.Clear();
BucketDrops.Add(capturing.mBoard, capturing.mRow, capturing.mX + 30, capturing.mY + 20, FusionPlant.FootballHelmet);
Check(BucketDrops.TakeForMagnet(capturing, 1000, 5, out _), "physical helmet transferred");
physical = BucketDrops.CaptureMagnet(capturing)!;
PlayerPlantingScope.ConsumingIngredient = capturing.Pointer;
BucketDrops.ReleaseFromMagnet(capturing, physical);
PlayerPlantingScope.ConsumingIngredient = IntPtr.Zero;
Check(BucketDrops.Count == 0, "ingredient consumption cannot duplicate physical item");
Check(BucketDrops.CaptureMagnet(capturing) == null, "processed ownership cannot be captured again");
BucketDrops.Add(capturing.mBoard, capturing.mRow, capturing.mX + 30, capturing.mY + 20, FusionPlant.EyesZombieHead);
Check(!BucketDrops.TakeForMagnet(capturing, 1000, 5, out _), "nonmetal head excluded");
BucketDrops.Clear();
Console.WriteLine($"Physical magnet transfers included: {assertions} checks");

var metalBoard = new Board();
Plant MetalPlant(FusionPlant identity, int col) => metalBoard.Plants[(col, 1)] = new Plant
{
    mBoard = metalBoard, mPlantCol = col, mRow = 1, Pointer = (IntPtr)(20 + col),
    mSeedType = identity.NativeType, mImitaterType = identity.Marker
};
var solar = MetalPlant(FusionPlant.SolarMagnet, 1);
var grenadier = MetalPlant(FusionPlant.GrenadierMagnet, 2);
metalBoard.RandomValues.Enqueue(2);
BucketDrops.ProduceMetal(solar);
Check(BucketDrops.Count == 0 && grenadier.mMagnetItems[0].ItemType == MagnetItemType.PickAxe
    && grenadier.mLaunchCounter == 1500, "right-hand ready Grenadier receives the generated item directly");
metalBoard.RandomValues.Enqueue(3);
BucketDrops.ProduceMetal(solar);
var metalDrop = BucketDrops.Snapshot(metalBoard).Single();
Check(metalBoard.m_gridItems.Any(i => !i.mDead && BucketDrops.IsSavedDrop(i)),
    "busy automatic recipient leaves one persisted loose item");
Check(BucketPlants.TrySelect(metalBoard, (int)metalDrop.X, (int)metalDrop.Y), "generic generated metal is selectable");
BucketPlants.MouseDown(metalBoard, 2, 1, 1, 0);
Check(BucketDrops.Count == 1 && BucketPlants.SelectedOn(metalBoard), "busy manual recipient preserves the selection");
BucketPlants.MouseDown(metalBoard, 3, 1, 1, 0);
Check(metalBoard.Created == 0 && BucketDrops.Count == 1, "generic metal cannot be planted as a bucket");
BucketPlants.MouseDown(metalBoard, 1, 1, 1, 0);
Check(BucketDrops.Count == 0 && metalBoard.Suns == 1 && !BucketPlants.SelectedOn(metalBoard),
    "Solar consumes one generic metal item for one full sun");

// The real controller boundary must retain the item across native seed cursor
// state and force-clear on completion. Default ClearCursor is a controller no-op.
BucketPlants.Clear();
var controllerBoard = new Board { ControllerMode=true, mX=40, mY=30 };
var controllerMagnet = new Plant { mBoard=controllerBoard, mPlantCol=2, mRow=1,
    mSeedType=FusionPlant.LumosMagnet.NativeType, mImitaterType=FusionPlant.LumosMagnet.Marker };
controllerBoard.Plants[(2,1)]=controllerMagnet;
controllerMagnet.mMagnetItems[0].ItemType=MagnetItemType.Pail1;
controllerBoard.mCursorObject.CursorType=CursorType.PlantFromBank;
controllerBoard.mCursorObject.mType=SeedType.Peashooter;
var controller=new Il2CppSource.Controllers.GamepadCursorController { Board=controllerBoard };
controllerBoard.ClearCursor();
Check(controllerBoard.mCursorObject.CursorType==CursorType.PlantFromBank,"native default clear leaves controller seed cursor");
Check(!BucketController.Confirm(controller)&&BucketPlants.Holding(controllerMagnet),"controller confirm selects captured metal from bank cursor");
Check(controllerBoard.mCursorObject.CursorType==CursorType.Glove&&controllerMagnet.mMagnetItems[0].ItemType==MagnetItemType.Pail1,"pickup retains native item ownership");
controller.X=3;controller.Y=0;
Check(!BucketController.Confirm(controller)&&controllerBoard.Created==1,"controller confirm places one bucket");
Check(controllerBoard.mCursorObject.CursorType==CursorType.Normal&&controllerMagnet.mMagnetItems[0].ItemType==MagnetItemType.None,"controller placement consumes metal and force clears selection");
controller.X=2;controller.Y=1;
controllerMagnet.mMagnetItems[0].ItemType=MagnetItemType.FootballHelmet1;
Check(!BucketController.Confirm(controller),"controller can select helmet");
controller.m_secondaryAction.Pressed=true;
Check(!BucketController.Cancel(controller)&&!BucketPlants.SelectedOn(controllerBoard),"controller cancel releases only selection");
Check(controllerMagnet.mMagnetItems[0].ItemType==MagnetItemType.FootballHelmet1&&controllerBoard.mCursorObject.CursorType==CursorType.Normal,"cancel retains metal and force clears cursor");
Check(!BucketController.Tool(controller),"shared cancel tool press cannot shovel");
controller.m_secondaryAction.Pressed=false;
Check(!BucketController.Tool(controller),"shared cancel tool release cannot shovel");
BucketController.ToolUpdated(controller);
Check(BucketController.Tool(controller),"later unrelated tool gesture is restored");
Check(!BucketController.Confirm(controller)&&!BucketController.Tool(controller),"tool-only binding cancels owned metal instead of digging");
Check(controllerMagnet.mMagnetItems[0].ItemType==MagnetItemType.FootballHelmet1,"tool cancellation retains item");
controllerBoard.mCursorObject.CursorType=CursorType.PlantFromBank;
controllerBoard.mCursorObject.mType=SeedType.Pumpkinshell;
controllerBoard.AllowedPlacement=SeedType.Pumpkinshell;
Check(BucketController.Confirm(controller)&&!BucketPlants.SelectedOn(controllerBoard),"valid native planting takes priority over pickup");
controllerBoard.AllowedPlacement=null;
controllerMagnet.mSeedType=SeedType.Magnetshroom;controllerMagnet.mImitaterType=SeedType.None;
controllerBoard.mCursorObject.mType=SeedType.Plantern;
Check(BucketController.Confirm(controller)&&!BucketPlants.SelectedOn(controllerBoard),"valid source recipe takes priority over pickup");
controllerBoard.Terrain=PlantingReason.NotHere;
Check(!BucketController.Confirm(controller)&&BucketPlants.Holding(controllerMagnet),"terrain-invalid recipe permits metal pickup fallback");
BucketPlants.Clear();controllerBoard.Terrain=PlantingReason.Ok;
controllerBoard.mCursorObject.mType=SeedType.Peashooter;
controller.PlayerIndex=1;Check(BucketController.Confirm(controller),"secondary player is untouched");controller.PlayerIndex=0;
controller.m_playerInput.Paired=false;Check(BucketController.Confirm(controller),"unpaired controller is untouched");controller.m_playerInput.Paired=true;
controllerBoard.mPaused=true;Check(BucketController.Confirm(controller),"paused board is untouched");controllerBoard.mPaused=false;
controller.gameObject.activeInHierarchy=false;Check(BucketController.Confirm(controller),"inactive controller is untouched");controller.gameObject.activeInHierarchy=true;
UnityEngine.Application.isFocused=false;Check(BucketController.Confirm(controller),"unfocused input is untouched");UnityEngine.Application.isFocused=true;
controller.m_playerInput.CurrentControlType=Il2CppReloaded.Input.ControlType.Mouse;
Check(BucketController.Confirm(controller),"mouse mode retains existing path");
controller.m_playerInput.CurrentControlType=Il2CppReloaded.Input.ControlType.Controller;
controllerBoard.mCursorObject.CursorType=CursorType.Shovel;
Check(BucketController.Confirm(controller),"another selected tool is untouched");
BucketController.Forget(controller);BucketPlants.Clear();
Console.WriteLine($"Controller boundaries included: {assertions} checks");
