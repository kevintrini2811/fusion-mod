using FusionMod;
using Il2CppReloaded.Gameplay;
using LawnGlove;

int checks=0;
void Check(bool ok,string message){++checks;if(!ok)throw new Exception(message);}
Plant Add(Board b,FusionPlant type,int col=2,int row=1)=>b.AddPlant(col,row,type.NativeType,type.Marker)!;
void Complete(Board b){foreach(var e in b.mApp.Effects)e.mController.Loops=1;foreach(var t in b.m_gridItems.Items.ToArray())Fertilizer.UpdateFeeding(t);}

var b=new Board();
var shrooms=Enumerable.Range(0,3).Select(_=>Add(b,SeedType.Sunshroom)).ToArray();
var pumpkin=Add(b,SeedType.Pumpkinshell);var lily=Add(b,SeedType.Lilypad);
var other=Add(b,SeedType.Sunshroom,3);
Check(Fertilizer.Apply(b,2,1),"Occupied tile accepts one fertilizer");
Check(shrooms.All(p=>p.mPlantHealth==300)&&pumpkin.mPlantHealth==300&&lily.mPlantHealth==300,"All three shrooms, pumpkin and support heal");
Check(other.mPlantHealth==10,"Other tiles remain unchanged");
Check(b.m_gridItems.Items.Count==3,"Every young native Sun-shroom receives saved growth");
foreach(var t in b.m_gridItems.Items)Check(!Fertilizer.UpdateFeeding(t)&&!t.mDead,"Growth waits for animation completion");
Check(shrooms.All(p=>p.mState!=PlantState.SunshroomBig),"No premature maturation");
Complete(b);
Check(shrooms.All(p=>p.mState==PlantState.SunshroomBig),"All three Sun-shrooms mature");
Check(!lily.mDead&&b.m_plants.All(p=>p.mSeedType!=SeedType.Cattail),"Occupied lily retains its occupant");

var saved=new Board();var original=Add(saved,SeedType.Sunshroom);
Fertilizer.Apply(saved,2,1);var oldTool=saved.m_gridItems.Items.Single();
var restored=new GridItem {mBoard=saved,mGridItemType=oldTool.mGridItemType,mGridItemState=oldTool.mGridItemState,
 mGridX=oldTool.mGridX,mGridY=oldTool.mGridY,mSeedType=oldTool.mSeedType,mGridItemCounter=oldTool.mGridItemCounter,
 mGridItemReanimID=oldTool.mGridItemReanimID};
saved.mApp.Effects.Single().mController.Loops=1;
Fertilizer.UpdateFeeding(restored);
Check(original.mState==PlantState.SunshroomBig,"Saved feeding fields retain exact recipient");

var removed=new Board();var gone=Add(removed,SeedType.Sunshroom);Fertilizer.Apply(removed,2,1);
gone.Die();var replacement=Add(removed,SeedType.Sunshroom);Complete(removed);
Check(replacement.mState!=PlantState.SunshroomBig,"Replacement in same slot cannot inherit feeding");
var moved=new Board();var moving=Add(moved,SeedType.Sunshroom);Fertilizer.Apply(moved,2,1);moving.mPlantCol=4;Complete(moved);
Check(moving.mState!=PlantState.SunshroomBig,"Moved recipient cannot grow elsewhere");
var carried=new Board();var held=Add(carried,SeedType.Sunshroom);var neighbour=Add(carried,SeedType.Sunshroom);
LawnGloveMod.Carried=held;Fertilizer.Apply(carried,2,1);Complete(carried);
Check(held.mPlantHealth==10&&held.mState!=PlantState.SunshroomBig&&neighbour.mState==PlantState.SunshroomBig,"Carried plant excluded; neighbour still fed");
LawnGloveMod.Carried=null;
var failed=new Board();var unchanged=Add(failed,SeedType.Sunshroom);Add(failed,SeedType.Sunshroom);failed.m_gridItems.FailAfter=1;
Check(!Fertilizer.Apply(failed,2,1)&&unchanged.mPlantHealth==10,"Partial allocation failure does not consume healing");
Check(failed.m_gridItems.Items.All(t=>t.mDead)&&failed.mApp.Effects.All(e=>e.mDead),"Failed reservation releases tools and animation");
var empty=new Board();Check(!Fertilizer.Apply(empty,2,1),"Empty tile rejects fertilizer");
var nuts=new Board();var nut=Add(nuts,SeedType.Wallnut);var shell=Add(nuts,SeedType.Pumpkinshell);Fertilizer.Apply(nuts,2,1);Complete(nuts);
Check(nut.mDead&&nuts.m_plants.Any(p=>p.mSeedType==SeedType.Tallnut&&!p.mDead)&&!shell.mDead&&shell.mPlantHealth==300,"Nut upgrades inside healed pumpkin");
var mines=new Board();var potatoes=new[]{Add(mines,FusionPlant.PeatatoMine),Add(mines,FusionPlant.IceMine),Add(mines,FusionPlant.HypnoMine)};
foreach(var p in potatoes)p.mState=PlantState.Notready;
Fertilizer.Apply(mines,2,1);Check(potatoes.All(p=>p.mState==PlantState.PotatoArmed),"All buried mines arm immediately");

foreach(int maturity in new[]{0,1234}){
 var legacy=Add(new Board(),new FusionPlant(SeedType.Puffshroom,SeedType.Sunflower));
 legacy.mTargetX=maturity;legacy.mTargetY=10;legacy.mStartRow=5;legacy.mShootingCounter=311;legacy.mLaunchCounter=900;
 Check(NativeSunShroom.Migrate(legacy),"Legacy port migrates");
 Check(legacy.mSeedType==SeedType.Sunshroom&&legacy.mImitaterType==SeedType.None,"Migration selects native identity");
 Check(legacy.mState==(maturity==0?PlantState.SunshroomBig:PlantState.SunshroomSmall)&&legacy.mStateCountdown==maturity,"Migration retains maturity");
 Check(legacy.mLaunchCounter==900&&legacy.mShootingCounter==0&&legacy.mTargetX==0&&legacy.mTargetY==0,"Native production retains interval, clears port-only phases");
 Check(!NativeSunShroom.Migrate(legacy)&&!FusionPlant.HasImportedVisuals(FusionPlant.From(legacy)),"Migration is idempotent; native graphics remain");
}
var ordinary=Add(new Board(),SeedType.Puffshroom);Check(!NativeSunShroom.Migrate(ordinary),"Ordinary Puff remains native Puff");
var roller=Add(new Board(),SeedType.GiantWallnut);roller.mController.Clip="_ground";roller.mController.Rate=12;
GiantWallNut.Update(roller);
Check(roller.mController.Size==1.5f&&roller.mWidth==120&&roller.mHeight==120,"Smaller Giant Wall-Nut and matching contact box");
Check(Math.Abs(roller.mController.Rate-5.4f)<.0001,"Existing ground clip also receives slower rotation");
Check(Math.Abs(roller.mX-1.8f)<.0001,"Translation remains independent from rotation and scale");
GiantWallNut.Update(roller);Check(roller.mController.Size==1.5f&&Math.Abs(roller.mX-3.6f)<.0001,"Absolute scale and steady motion across ticks");
roller.mBoard.mPaused=true;GiantWallNut.Update(roller);Check(Math.Abs(roller.mX-3.6f)<.0001,"Pause stops rolling");
foreach (var identity in new[] { FusionPlant.Cyclover, FusionPlant.Disguster, FusionPlant.BreezeBlover, FusionPlant.MindBlover,
 FusionPlant.BoostShroom, FusionPlant.BlowtorchShroom, FusionPlant.WallopShroom, FusionPlant.CaltropShroom })
{
    var windBoard = new Board();
    var wind = Add(windBoard, identity);
    wind.mTargetX = 73; wind.mTargetY = 3; wind.mStartRow = 1; wind.mLaunchCounter = 81;
    Check(Fertilizer.Apply(windBoard, 2, 1) && wind.mPlantHealth == 300, "Wind fusion receives ordinary healing");
    Complete(windBoard);
    Check(FusionPlant.From(wind) == identity && wind.mTargetX == 73 && wind.mTargetY == 3
        && wind.mStartRow == 1 && wind.mLaunchCounter == 81 && windBoard.m_gridItems.Items.Count == 0,
        "Wind fertilizer neither evolves nor resets attack, wind lifetime or pulse phase");
}
Console.WriteLine($"{checks} tile, migration and rolling checks passed; no game code loaded.");
