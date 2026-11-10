using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool ok, string message) { ++checks; if (!ok) throw new Exception(message); }
Plant Shell() {
    var p = new Plant { mSeedType=SeedType.Umbrella, mImitaterType=SeedType.Wallnut };
    UmbrellaShell.Initialize(p); return p;
}
Zombie Target(Plant p, float x=100, int row=0, float y=0) {
    var z = new Zombie { mBoard=p.mBoard, mPosX=x, mPosY=y, mRow=row,
        DataID=(uint)(p.mBoard.m_zombies.Count+1) };
    p.mBoard.m_zombies.Add(z); return z;
}
Check(FusionRecipes.TryGet(SeedType.Umbrella, SeedType.Wallnut, out var a) && a==FusionPlant.UmbrellaShell,"forward recipe");
Check(FusionRecipes.TryGet(SeedType.Wallnut, SeedType.Umbrella, out var b) && b==a,"reverse recipe");
var p=Shell();
Check(FusionPlant.From(p)==a && p.mSeedType==SeedType.Umbrella,"saved native protection identity");
Check(p.mPlantHealth==4000 && p.mPlantMaxHealth==4000,"health");
Check(!FusionRecipes.TryGet(a,SeedType.Wallnut,out _),"marker is not base ingredient");
var z=Target(p); var adjacent=Target(p,100,1,80); var far=Target(p,400);
UmbrellaShell.Update(p);
Check(z.Damage==40 && adjacent.Damage==40 && far.Damage==0,"radius pulse");
Check(z.mPosX==260 && adjacent.mPosX==260,"two-unit immediate knockback without pot");
Check(p.mPlantHealth==3850,"two targets cost 150 health");
Check(p.mLaunchCounter==100 && p.mShootingCounter==84,"saved cooldown and clip time");
z.mPosX=100;
for (int i=0;i<99;i++) UmbrellaShell.Update(p);
Check(z.Hits==1 && p.mLaunchCounter==1,"cooldown is full second");
UmbrellaShell.Update(p);
Check(z.Hits==2 && p.mPlantHealth==3750,"next pulse costs 100 for single target");
foreach (int n in new[] {1,2,3,4,5,8}) {
    var q=Shell(); for(int i=0;i<n;i++) Target(q);
    UmbrellaShell.Update(q);
    Check(q.mPlantHealth==4000-(n==1?100:n<=4?150:250),"self damage count "+n);
}
foreach (string mode in new[]{"pause","carried","bungee","dead","squished","loading","garden","board-end"}) {
    var q=Shell();var t=Target(q);
    if(mode=="pause") q.mBoard.mPaused=true;
    if(mode=="carried") q.OffGround=true;
    if(mode=="bungee") q.mOnBungeeState=PlantOnBungeeState.OnBungee;
    if(mode=="dead") q.mDead=true;
    if(mode=="squished") q.mSquished=true;
    if(mode=="loading") q.mApp.GameScene=GameScenes.Loading;
    if(mode=="garden") q.mApp.GameMode=GameMode.ChallengeZenGarden;
    if(mode=="board-end") CherryBombPlants.DisposingBoard=q.mBoard.Pointer;
    UmbrellaShell.Update(q);
    Check(t.Hits==0 && q.mLaunchCounter==0,"inactive "+mode);
    CherryBombPlants.DisposingBoard=IntPtr.Zero;
}
foreach(string mode in new[]{"friendly","dead","dying","airborne","other-board","vehicle","boss"}) {
    var q=Shell();var t=Target(q);
    if(mode=="friendly")t.mMindControlled=true;
    if(mode=="dead")t.mDead=true;
    if(mode=="dying")t.mBodyHealth=0;
    if(mode=="airborne")t.Eligible=false;
    if(mode=="other-board")t.mBoard=new Board{Pointer=(IntPtr)99};
    if(mode=="vehicle")t.mZombieType=ZombieType.Zamboni;
    if(mode=="boss")t.mZombieType=ZombieType.Boss;
    UmbrellaShell.Update(q);Check(t.Hits==0 && q.mPlantHealth==4000,"excluded "+mode);
}
var q2=Shell();Target(q2,100,1,80);UmbrellaShell.Update(q2);
Check(q2.mLaunchCounter==0,"adjacent row alone cannot trigger contact");
var lethal=Shell();lethal.mPlantHealth=100;Target(lethal);UmbrellaShell.Update(lethal);
Check(lethal.mDead,"self damage can kill");
var normal=new Plant{mSeedType=SeedType.Umbrella};Target(normal);
UmbrellaShell.Initialize(normal);UmbrellaShell.Update(normal);
Check(normal.mPlantHealth==300 && normal.mBoard.m_zombies[0].Hits==0,"normal umbrella untouched");
var restored=Shell();restored.mLaunchCounter=50;restored.mShootingCounter=34;Target(restored);
UmbrellaShell.Update(restored);
Check(restored.mLaunchCounter==49 && restored.mShootingCounter==33 && restored.mBoard.m_zombies[0].Hits==0,"restored timers continue");
var husk = new Plant { mSeedType=SeedType.Umbrella, mImitaterType=SeedType.Kernelpult };
UmbrellaShell.Initialize(husk);
var huskTarget = Target(husk);
UmbrellaShell.Update(husk);
Check(FusionPlant.From(husk) == FusionPlant.UmbrellaHusk && husk.mPlantMaxHealth == 1000,
    "Husk source health and saved native protection identity");
Check(huskTarget.Damage == 40 && huskTarget.mButteredCounter == 200 && huskTarget.mPosX == 260,
    "Husk contact pulse damages, butters and knocks back");
Check(husk.mPlantHealth == 900 && husk.mLaunchCounter == 100, "Husk inherits self-damage and cooldown");
Check(FusionRecipes.TryGet(SeedType.Umbrella, SeedType.Kernelpult, out var huskRecipe)
    && huskRecipe == FusionPlant.UmbrellaHusk
    && FusionRecipes.TryGet(SeedType.Kernelpult, SeedType.Umbrella, out huskRecipe)
    && huskRecipe == FusionPlant.UmbrellaHusk, "both Husk recipe directions");

var silver = new Plant { mSeedType = SeedType.Umbrella, mImitaterType = SeedType.Marigold };
UmbrellaShell.Initialize(silver);
var contact = Target(silver);
UmbrellaShell.Update(silver);
Check(contact.Damage == 40 && silver.mPlantHealth == 1000 && silver.mBoard.Coins == 0,
    "Silver contact deals damage and knockback without health cost or reward");
silver.mState = PlantState.UmbrellaReflecting;
var ball = new Projectile { mBoard = silver.mBoard, DataID = 123, mProjectileType = ProjectileType.Basketball };
var reflection = new SilverUmbrella.Reflection(ball);
SilverUmbrella.Current = reflection;
SilverUmbrella.Found(silver);
SilverUmbrella.Current = null;
SilverUmbrella.Finish(reflection);
Check(silver.mBoard.Coins == 0, "Live basketball cannot pay before native reflection");
ball.mDead = true;
SilverUmbrella.Finish(reflection);
Check(silver.mBoard.Coins == 2 && silver.mTargetY == 5,
    "Reflected basketball pays and retains remainder");
var recycledBall = new Projectile { mBoard = silver.mBoard, DataID = 456 };
reflection = new SilverUmbrella.Reflection(recycledBall);
SilverUmbrella.Current = reflection;
SilverUmbrella.Found(silver);
SilverUmbrella.Current = null;
recycledBall.mDead = true; ++recycledBall.DataID;
SilverUmbrella.Finish(reflection);
Check(silver.mBoard.Coins == 2, "Recycled projectile cannot inherit reward");
// The support modifies contact effects without becoming a second umbrella.
foreach (var identity in new[] {FusionPlant.UmbrellaShell,FusionPlant.UmbrellaHusk,FusionPlant.SilverUmbrella})
{
    var top=Shell(); top.mImitaterType=identity.Marker; UmbrellaShell.Initialize(top);
    var support=new Plant {mBoard=top.mBoard,mPlantCol=top.mPlantCol,mRow=top.mRow,
        mSeedType=SeedType.Flowerpot,mImitaterType=SeedType.Umbrella};
    top.mBoard.m_plants.Add(support);
    var enemy=Target(top);
    UmbrellaShell.Update(top);
    Check(enemy.mPosX==340,"Umbrella Pot increases full-size knockback from 160 to 240 board pixels");
    if(identity==FusionPlant.UmbrellaHusk)
        Check(enemy.mButteredCounter==400,"Umbrella Pot adds two seconds to Husk butter");
    Check(top.mPlantHealth==(identity==FusionPlant.SilverUmbrella?1000:identity==FusionPlant.UmbrellaShell?3900:900),
        "Support retains existing contact health cost");
}


var clove=new Plant{mSeedType=SeedType.Umbrella,mImitaterType=SeedType.Garlic};
UmbrellaShell.Initialize(clove);var cloveTarget=Target(clove);var cloveOtherRow=Target(clove,60,1,20);UmbrellaShell.Update(clove);
Check(cloveOtherRow.Damage==0,"clove source pulse stays in its own lane");
Check(cloveTarget.Damage==40&&cloveTarget.mYuckyFace&&cloveTarget.mYuckyFaceCounter==169,"clove damage and native lane-change entry");
Check(Math.Abs(cloveTarget.mPosX-(100+2*80f/1.35f))<.01f&&clove.mPlantHealth==900,"clove original push and self damage");
var thorn=new Plant{mSeedType=SeedType.Umbrella,mImitaterType=SeedType.Cactus};
UmbrellaShell.Initialize(thorn);var groundThorn=Target(thorn);UmbrellaShell.Update(thorn);
Check(thorn.mPlantHealth==1000&&thorn.mLaunchCounter==0,"thorn ignores ground contact");
var airThorn=Target(thorn,60);airThorn.Flying=true;airThorn.mZombieType=ZombieType.Balloon;
UmbrellaShell.Update(thorn);
Check(!airThorn.Flying&&airThorn.Damage==0&&groundThorn.Damage==0,"thorn pops balloon without damage");
Check(thorn.mPlantHealth==900&&thorn.mLaunchCounter==100,"thorn charges contact once");
Check(Math.Abs(airThorn.mPosX-(60+2*80f/1.35f))<.01f,"thorn source knockback");

var light = new Plant { mSeedType = FusionPlant.LumosUmbrella.NativeType, mImitaterType = FusionPlant.LumosUmbrella.Marker, mTargetX = 2000 };
Check(LumosEnergy.Absorb(light, 100) == 0 && light.mTargetX == 1500, "five energy absorb one HP");
Check(LumosEnergy.Absorb(light, 400) == 100 && light.mTargetX == 0 && light.mTargetY == 500, "partial shield and five-second exhaustion");
light.mTargetX = 4;
Check(LumosEnergy.Absorb(light, 1) == 1 && light.mTargetX == 0, "fractional shield cannot absorb a full HP");
var ordinary = Shell(); ordinary.mTargetX = 2000;
Check(LumosEnergy.Absorb(ordinary, 100) == 100 && ordinary.mTargetX == 2000, "energy belongs only to Lumos Umbrella");

Console.WriteLine($"Umbrella Shell: {checks} offline assertions passed.");
