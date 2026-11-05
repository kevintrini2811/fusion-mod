using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool ok, string reason) { ++checks; if (!ok) throw new Exception(reason); }
uint next = 10;
Plant Make(FusionPlant identity) {
    var p = new Plant { mSeedType=identity.NativeType, mImitaterType=identity.Marker,
        Pointer=(IntPtr)next, DataID=next++ };
    p.mBoard.m_plants.Add(p); ElementalNuts.Initialize(p); return p;
}
Zombie Enemy(Plant p) {
    var z = new Zombie { Pointer=(IntPtr)next, DataID=next++, mBoard=p.mBoard };
    p.mBoard.m_zombies.Add(z); return z;
}
var recipes = new (SeedType A, SeedType B, FusionPlant Result)[] {
    (SeedType.Spikeweed, SeedType.Sunflower, FusionPlant.Solarweed),
    (SeedType.Spikeweed, SeedType.Iceshroom, FusionPlant.Iceweed),
    (SeedType.Spikeweed, SeedType.Jalapeno, FusionPlant.Scorchweed),
    (SeedType.Spikeweed, SeedType.Wallnut, FusionPlant.SpikeNut)
};
foreach (var (a,b,r) in recipes) {
    Check(FusionRecipes.TryGet(a,b,out var result) && result==r,"recipe");
    Check(FusionRecipes.TryGet(b,a,out result) && result==r,"reverse recipe");
    var p=Make(r);
    Check(FusionPlant.From(p)==r && FusionPlant.HasImportedVisuals(r),"saved marker and assets");
    Check(p.mSeedType==(r==FusionPlant.SpikeNut?SeedType.Wallnut:SeedType.Spikeweed),"native target category");
}
foreach(var r in new[]{FusionPlant.Solarweed,FusionPlant.Iceweed,FusionPlant.Scorchweed}) {
    Overheat.Clear(); SolarShooter.Suns.Clear(); var p=Make(r);var a=Enemy(p);var b=Enemy(p);
    var ally=Enemy(p);ally.mMindControlled=true;
    var air=Enemy(p);air.Category=DamageRangeFlags.Flying;
    var row=Enemy(p);row.mRow++;
    var far=Enemy(p);far.Rect=new(500,200,520,280);
    p.DoRowAreaDamage(20,DamageFlags.BypassesShield|DamageFlags.Spike);
    Check(a.Damage==(r==FusionPlant.Solarweed?20:40) && b.Damage==a.Damage,"source contact damage");
    Check(ally.Damage==0 && air.Damage==0 && row.Damage==0 && far.Damage==0,"native contact restrictions");
    Check(SolarShooter.Suns.Count==(r==FusionPlant.Solarweed?1:0),"one pickup per attack, not victim");
    Check(SolarShooter.Suns.All(s=>s.Value==5),"source five-sun pickup");
    Check(a.mChilledCounter==(r==FusionPlant.Iceweed?1000:0),"ice chill");
    Check(Overheat.Has(a)==(r==FusionPlant.Scorchweed),"overheat");
    Check(WeedFusions.CurrentAttack==null,"attack scope restored");
}
Overheat.Clear();var ice=Make(FusionPlant.Iceweed);var target=Enemy(ice);
for(int i=0;i<10;i++)ice.DoRowAreaDamage(20,DamageFlags.Spike);
Check(target.Damage==400 && target.FreezeCalls==1,"ten hits freeze without extra damage");
Check(target.mIceTrapCounter==400 && target.mChilledCounter==1400,"four-second freeze plus chill");
target=Enemy(ice);target.ImmuneToFreeze=true;ice.DoRowAreaDamage(20,DamageFlags.Spike);
Check(target.Damage==40 && target.mChilledCounter==0 && target.FreezeCalls==0,"native immunity");
var fire=Make(FusionPlant.Scorchweed);target=Enemy(fire);
fire.DoRowAreaDamage(20,DamageFlags.Spike);fire.DoRowAreaDamage(20,DamageFlags.Spike);
Check(target.Damage==100,"overheat amplifies later hits");
foreach(string mode in new[]{"pause","dead","carry","bungee","loading","garden","dispose"}) {
    var p=Make(FusionPlant.Solarweed);target=Enemy(p);SolarShooter.Suns.Clear();
    if(mode=="pause")p.mBoard.mPaused=true;
    if(mode=="dead")p.mDead=true;
    if(mode=="carry")p.OffGround=true;
    if(mode=="bungee")p.mOnBungeeState=PlantOnBungeeState.OnBungee;
    if(mode=="loading")p.mApp.GameScene=GameScenes.Loading;
    if(mode=="garden")p.mApp.GameMode=GameMode.ChallengeZenGarden;
    if(mode=="dispose")CherryBombPlants.DisposingBoard=p.mBoard.Pointer;
    p.DoRowAreaDamage(20,DamageFlags.Spike);
    Check(target.Damage==0 && SolarShooter.Suns.Count==0,"inactive "+mode);
    CherryBombPlants.DisposingBoard=IntPtr.Zero;
}
var plain=Make(SeedType.Spikeweed);target=Enemy(plain);SolarShooter.Suns.Clear();
plain.DoRowAreaDamage(20,DamageFlags.Spike);
Check(target.Damage==20 && SolarShooter.Suns.Count==0,"ordinary weed unchanged");
var nut=Make(FusionPlant.SpikeNut);target=Enemy(nut);
Check(nut.mPlantHealth==4000,"source nut health");
target.EatPlant(nut);Check(target.Damage==50,"successful bite retaliates");
target.SkipBite=true;target.EatPlant(nut);Check(target.Damage==50,"skipped bite no retaliation");
target.SkipBite=false;nut.mPlantHealth=4;target.EatPlant(nut);
Check(nut.mDead && target.Damage==100,"lethal bite retaliates");
foreach(var type in new[]{ZombieType.Zamboni,ZombieType.Catapult}) {
    Overheat.Clear();nut=Make(FusionPlant.SpikeNut);target=Enemy(nut);target.mZombieType=type;target.Health=1350;
    target.SquishAllInSquare(nut.mPlantCol,nut.mRow,1);
    Check(nut.mPlantHealth==3500 && !nut.mSquished,"tire costs 500 HP");
    Check(target.mDead && target.Damage==1800,"vehicle popped");
    Check(WeedFusions.CurrentCrush==null,"vehicle scope restored");
    nut=Make(FusionPlant.SpikeNut);nut.mPlantHealth=500;target=Enemy(nut);target.mZombieType=type;
    target.SquishAllInSquare(nut.mPlantCol,nut.mRow,1);
    Check(nut.mDead,"eighth tire is lethal");
}
nut=Make(FusionPlant.SpikeNut);target=Enemy(nut);target.mZombieType=ZombieType.Gargantuar;
target.SquishAllInSquare(nut.mPlantCol,nut.mRow,0);
Check(nut.mSquished,"giant crush stays native");
nut=Make(FusionPlant.SpikeNut);nut.Squish();Check(nut.mSquished,"unscoped removal stays native");
// Both synchronous contexts restore on failures, nesting and unrelated calls.
ice=Make(FusionPlant.Iceweed);target=Enemy(ice);target.OnDamage=()=>throw new InvalidOperationException("damage failure");
try{ice.DoRowAreaDamage(20,DamageFlags.Spike);}catch(InvalidOperationException){}
Check(WeedFusions.CurrentAttack==null,"throw restores contact context");
var outer=new WeedFusions.Attack(ice.mBoard,ice.DataID,FusionPlant.Iceweed);WeedFusions.CurrentAttack=outer;
plain.DoRowAreaDamage(20,DamageFlags.Spike);Check(WeedFusions.CurrentAttack==outer,"nested unrelated attack restores outer");
WeedFusions.CurrentAttack=null;
var hit=new WeedFusions.Attack(ice.mBoard,ice.DataID,FusionPlant.Iceweed);uint old=target.DataID++;target.OnDamage=null;
WeedFusions.AfterHit(target,hit,old);Check(target.mChilledCounter==0,"pool reuse rejects old hit");
SolarShooter.Suns.Clear();var solar=Make(FusionPlant.Solarweed);target=Enemy(solar);target.mZombieType=ZombieType.Zamboni;
solar.DoRowAreaDamage(20,DamageFlags.Spike);
Check(solar.mDead && target.Damage==1800 && SolarShooter.Suns.Count==1,"native tire death still produces one sun");
Check(WeedFusions.RockDamageStage(301,450)==0&&WeedFusions.RockDamageStage(300,450)==1
 &&WeedFusions.RockDamageStage(151,450)==1&&WeedFusions.RockDamageStage(150,450)==2,"source rock damage-stage thresholds");
foreach(var r in new[]{FusionPlant.Icerock,FusionPlant.Hearthrock}) {
 var ingredient=r==FusionPlant.Icerock?SeedType.Iceshroom:SeedType.Jalapeno;
 var weed=r==FusionPlant.Icerock?FusionPlant.Iceweed:FusionPlant.Scorchweed;
 foreach(var pair in new (FusionPlant,FusionPlant)[]{(SeedType.Spikerock,ingredient),(ingredient,SeedType.Spikerock),(weed,SeedType.Spikerock),(SeedType.Spikerock,weed)})
  Check(FusionRecipes.TryGet(pair.Item1,pair.Item2,out var result)&&result==r,"all rock recipe orders");
 var p=Make(r);Check(FusionPlant.From(p)==r&&FusionPlant.HasImportedVisuals(r),"saved rock identity and original assets");
 Check(p.mSeedType==SeedType.Spikerock&&FusionPlant.IsWeed(r),"native rock lifecycle");
 Overheat.Clear();var z=Enemy(p);
 p.DoRowAreaDamage(20,DamageFlags.Spike);
 Check(z.Damage==40,"first rock contact");
 p.DoRowAreaDamage(20,DamageFlags.Spike);
 Check(z.Damage==(r==FusionPlant.Icerock?80:100),"second contact and heat amplification");
 Check(z.mChilledCounter==(r==FusionPlant.Icerock?1500:0),"rock chill");
 Check(Overheat.Has(z)==(r==FusionPlant.Hearthrock),"rock overheat");
}
var coldRock=Make(FusionPlant.Icerock);var frozenTarget=Enemy(coldRock);
for(int i=0;i<19;i++)coldRock.DoRowAreaDamage(20,DamageFlags.Spike);
Check(frozenTarget.FreezeCalls==0,"Icerock adds exactly five freeze points per hit");
coldRock.DoRowAreaDamage(20,DamageFlags.Spike);Check(frozenTarget.FreezeCalls==1,"Icerock freezes after twenty contacts");
// New contacts retain independent butter/attack timers and saved flight ownership.
var butterweed = Make(FusionPlant.Butterweed);
GroundWeeds.Initialize(butterweed);var butterTarget = Enemy(butterweed);
Check(GroundWeeds.Slowed(butterTarget), "butterweed slows within live contact");
for (int i=0;i<299;i++) GroundWeeds.Tick(butterweed);
Check(butterTarget.Damage==0 && butterTarget.mButteredCounter==0, "butterweed never damages; full first interval");
GroundWeeds.Tick(butterweed);
Check(butterTarget.mButteredCounter==100, "one-second butter after three seconds");
butterweed.mBoard.mPaused=true;int butterTimer=butterweed.mTargetY;
GroundWeeds.Tick(butterweed);Check(butterweed.mTargetY==butterTimer,"pause preserves butter timer");
butterweed.mBoard.mPaused=false;butterweed.mDead=true;
Check(!GroundWeeds.Slowed(butterTarget),"removal immediately removes passive slow");
var springweed=Make(FusionPlant.Springweed);GroundWeeds.Initialize(springweed);var springTarget=Enemy(springweed);
for(int i=0;i<42;i++) GroundWeeds.Tick(springweed);
Check(springTarget.Damage==80 && SpringEffects.Airborne(springTarget),"original event damages then launches once");
var flight=springweed.mBoard.m_gridItems.Single();
var position=SpringEffects.Capture(springTarget);
springTarget.mPosX=99;springTarget.mPosY=88;springTarget.mAltitude=77;
SpringEffects.Restore(position);
Check(springTarget.mPosX==position!.X && springTarget.mPosY==position.Y && springTarget.mAltitude==position.Altitude,
    "inlined terrain movement cannot overwrite flight position");
SpringEffects.Launch(springTarget);Check(springweed.mBoard.m_gridItems.Count==1,"launch cannot stack");
springweed.mBoard.mPaused=true;SpringEffects.Tick(flight);
Check(flight.mGridItemCounter==0,"flight pauses");springweed.mBoard.mPaused=false;
for(int i=0;i<100 && !flight.mDead;i++)SpringEffects.Tick(flight);
Check(flight.mDead && springTarget.mAltitude==0,"flight lands and restores original altitude");
var imp=Enemy(springweed);imp.mZombieType=ZombieType.Imp;SpringEffects.Launch(imp);
Check(imp.mDead,"launch instantly kills imp");
var boss=Enemy(springweed);boss.mZombieType=ZombieType.Boss;SpringEffects.Launch(boss);
Check(!SpringEffects.Airborne(boss),"boss cannot launch");
var reused=Enemy(springweed);SpringEffects.Launch(reused);var stale=springweed.mBoard.m_gridItems.Last();
reused.DataID++;SpringEffects.Tick(stale);Check(stale.mDead,"pool reuse does not inherit flight");
var seaNut=Make(FusionPlant.SeaNut);SeaNut.Initialize(seaNut);
seaNut.mPlantHealth=1500;
for(int i=0;i<1000;i++)SeaNut.Tick(seaNut);
Check(seaNut.mStartRow==1 && seaNut.mPlantMaxHealth==4000 && seaNut.mPlantHealth==3500,"growth preserves existing damage");
SeaNut.Fertilize(seaNut);Check(seaNut.mLaunchCounter==50,"fertilizer accelerates next growth only");
for(int i=0;i<50;i++)SeaNut.Tick(seaNut);
Check(seaNut.mPlantMaxHealth==6000 && seaNut.mStartRow==2,"second growth");
for(int i=0;i<1000;i++)SeaNut.Tick(seaNut);
Check(seaNut.mStartRow==3,"third event starts rolling");
float rollX=seaNut.mX;SeaNut.Tick(seaNut);Check(seaNut.mX>rollX,"rolling moves saved plant anchor");
SeaNut.Fertilize(seaNut);Check(seaNut.mStartRow==3,"fertilizer does not restart rolling");
bool canTarget=true;GroundFusionTargetPatch.Postfix(Enemy(seaNut),seaNut,ref canTarget);
Check(!canTarget,"rolling nut cannot become native bite/crush target");
Console.WriteLine($"Weed fusions: {checks} assertions passed; offline doubles only.");

var landKelpWeed = Make(FusionPlant.TangleWeed); var landKelpTarget = Enemy(landKelpWeed);
KelpEffects.Calls = 0;
landKelpWeed.DoRowAreaDamage(20, DamageFlags.Spike);
Check(landKelpTarget.Damage == 20 && KelpEffects.Calls == 1 && KelpEffects.Ticks == 200 && KelpEffects.Land,
    "Tangle Weed applies 20 damage and exactly one two-second land kelp hit");
