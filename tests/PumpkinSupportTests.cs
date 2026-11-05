using FusionMod;
using Il2CppReloaded.Gameplay;
static void Check(bool value,string name) { if(!value)throw new Exception(name); }
static Plant MakePlant(Board board,FusionPlant identity,int col=2,int row=1)
{
    var p=new Plant {mBoard=board,mSeedType=identity.NativeType,mImitaterType=identity.Marker,mPlantCol=col,mRow=row};
    p.Pointer=(IntPtr)p.DataID;board.m_plants.Add(p);PumpkinSupport.Initialize(p);return p;
}
static Zombie Enemy(Board board,int row=1)
{ var z=new Zombie {mBoard=board,mRow=row};z.Pointer=(IntPtr)z.DataID;board.m_zombies.Add(z);return z; }
var board=new Board();var cryo=MakePlant(board,FusionPlant.CryoPumpkin);
Check(PumpkinSupport.StoreIce(board,3,1)&&PumpkinSupport.StoreIce(board,3,1)&&PumpkinSupport.StoreIce(board,3,1),"forward tile charges");
Check(cryo.mTargetX==3&&!PumpkinSupport.StoreIce(board,3,1)&&!PumpkinSupport.StoreIce(board,2,1),"three-charge cap and rear rejection");
cryo.mLaunchCounter=1;PumpkinSupport.Tick(cryo);
Check(cryo.mTargetX==2&&IceInstantFusions.Bursts==1,"automatic release above one charge");
cryo.mTargetX=1;cryo.mLaunchCounter=1;PumpkinSupport.Tick(cryo);Check(cryo.mTargetX==1,"automatic release retains final charge");
board.mPaused=true;cryo.mLaunchCounter=1;PumpkinSupport.Tick(cryo);Check(cryo.mLaunchCounter==1,"pause preserves timer");board.mPaused=false;
PumpkinSupport.ReleaseIce(cryo);Check(cryo.mTargetX==0,"manual final charge");
var pyro=MakePlant(board,FusionPlant.PyroPumpkin,4);PumpkinSupport.FireEnergy(board,1,1800);Check(pyro.mTargetX==180,"ten percent fire energy");
PumpkinSupport.HeatedDeath(board,1);Check(pyro.mTargetX==240,"heated death energy");
pyro.mTargetX=999999;PumpkinSupport.FireEnergy(board,1,1800);Check(pyro.mTargetX==1000000,"ordinary heat cap");
PumpkinSupport.HeatedDeath(board,1);Check(pyro.mTargetX==1000060,"source death bonus directly adds beyond ordinary cap");
pyro.mTargetX=1800;PumpkinSupport.ReleaseFire(pyro);Check(DoomEffects.DamageReleased==1800&&pyro.mTargetX==0,"release consumes all, cannot recharge itself during callbacks");
var pod=MakePlant(board,FusionPlant.PumpkinPod,6);var pea=MakePlant(board,SeedType.Splitpea,6);Enemy(board);pod.mLaunchCounter=0;
for(int i=0;i<126;i++)PumpkinSupport.Tick(pod);
Check(pea.Shots.SequenceEqual(new[]{PlantWeapon.Primary,PlantWeapon.Secondary,PlantWeapon.Secondary}),"copies native split pattern");
Check(pea.mLaunchCounter==0,"copied attacks preserve original cooldown");
pod.mLaunchCounter=0;PumpkinSupport.Tick(pod);pea.DataID+=100;
for(int i=0;i<42;i++)PumpkinSupport.Tick(pod);
Check(pea.Shots.Count==3,"generation change cancels copied volley");
var z=Enemy(board);Overheat.Apply(z);Check(Overheat.Has(z)&&board.m_gridItems.Count==1,"overheat stored in native pool");
var item=board.m_gridItems[0];z.DataID++;Check(!Overheat.Has(z),"saved status never binds pooled generation");SavedOverheat.Tick(item);Check(item.mDead,"orphan status removed");
z.DataID--;Overheat.Apply(z);Overheat.Clear();Check(Overheat.Has(z),"clearing transient state does not erase serialized status");
Overheat.Forget(z);Check(!Overheat.Has(z),"cold/removal clears saved status");
board.m_gridItems.Fail=true;Overheat.Apply(z);Check(!Overheat.Has(z),"allocation failure cannot create unsaved status");
board.m_gridItems.Fail=false;
var lobber=MakePlant(board,SeedType.Cabbagepult,2);cryo.mDead=true;var shell=MakePlant(board,FusionPlant.MelonShell,2);
var frame=MelonShell.Begin(lobber)!;frame.First=board.AddProjectile();MelonShell.Finish(frame);Check(LobberProjectiles.ShellShots==1,"one successful attack yields one extra melon");
frame=MelonShell.Begin(lobber)!;MelonShell.Finish(frame);Check(LobberProjectiles.ShellShots==1,"failed attack yields no extra melon");
Console.WriteLine("Pumpkin support cases passed.");

// Shields use the real saved item owner. A recycled plant cannot inherit points.
var shieldBoard=new Board();var recipient=MakePlant(shieldBoard,SeedType.Peashooter);
PlantShields.Add(recipient,50);PlantShields.Add(recipient,50);
Check(PlantShields.Amount(recipient)==100,"shield production stacks");
Check(PlantShields.Absorb(recipient,60)==0&&PlantShields.Amount(recipient)==40,"shield absorbs a partial hit");
Check(PlantShields.Absorb(recipient,70)==30&&PlantShields.Amount(recipient)==0,"shield exhaustion passes exactly the remaining damage");
PlantShields.Add(recipient,50);var shieldRecord=shieldBoard.m_gridItems.Last();recipient.DataID++;
Check(PlantShields.Amount(recipient)==0,"shield rejects pool reuse");PlantShields.Tick(shieldRecord);
Check(shieldRecord.mDead,"orphan shield is released");
shieldBoard.m_gridItems.Fail=true;PlantShields.Add(recipient,50);
Check(PlantShields.Amount(recipient)==0,"allocation failure cannot grant unsaved shield");
Console.WriteLine("Shield lifetime cases passed.");
