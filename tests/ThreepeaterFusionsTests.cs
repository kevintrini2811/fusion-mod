using FusionMod;
using Il2CppReloaded.Gameplay;
int checks=0;
void Check(bool condition,string reason) {++checks;if(!condition)throw new Exception(reason);}
Plant Make(FusionPlant identity,int row=2) {
    var p=new Plant {mSeedType=identity.NativeType,mImitaterType=identity.Marker,mRow=row};
    ThreepeaterFusions.Initialize(p);p.mBoard.m_plants.Add(p);return p;
}
void Enemy(Plant p,int row,float x) => p.mBoard.m_zombies.Add(new Zombie {mBoard=p.mBoard,mRow=row,X=x});
void Ticks(Plant p,int count) {for(int i=0;i<count;++i)ThreepeaterFusions.Tick(p);}
var recipes=new (FusionPlant A,FusionPlant B,FusionPlant Result)[] {
    (SeedType.Garlic,SeedType.Threepeater,FusionPlant.StenchSpreader),
    (SeedType.Cherrybomb,SeedType.Threepeater,FusionPlant.CherrySpreader),
    (SeedType.Iceshroom,SeedType.Threepeater,FusionPlant.SnowThreepeater),
    (SeedType.Jalapeno,SeedType.Threepeater,FusionPlant.ScorchedThreepeater),
    (SeedType.Jalapeno,FusionPlant.ScorchedThreepeater,FusionPlant.CharredThreepeater)};
foreach(var (a,b,result) in recipes) {
    Check(FusionRecipes.TryGet(a,b,out var r)&&r==result,"forward recipe");
    Check(FusionRecipes.TryGet(b,a,out r)&&r==result,"reverse recipe");
    var p=Make(result);Check(FusionPlant.From(p)==result&&FusionPlant.HasImportedVisuals(result),"persisted identity and model");
    Check(p.mPlantHealth==300&&p.mStartRow==0,"fresh initialization");
}
var stench=Make(FusionPlant.StenchSpreader);Enemy(stench,2,400);
Ticks(stench,50);Check(stench.mBoard.Shots.Count==0,"no shot before original .5-second event");
Ticks(stench,1);Check(stench.mBoard.Shots.Count==3,"three source rows");
Check(stench.mBoard.Shots.All(PoisonShooters.Contains),"all three poison projectiles tracked");
var victim=new Zombie{mBoard=stench.mBoard,mRow=1};stench.mBoard.Shots[0].DoImpact(victim);
Check(victim.Damage==30&&PoisonShooters.Level(victim)==1,"poison ability reaches actual impact");
Check(PoisonShooters.RowDamage(stench.mBoard,2)==60&&PoisonShooters.RowDamage(stench.mBoard,1)==0,"Miasma counts three bullets in planted row only");
Ticks(stench,150);Check(stench.mBoard.Shots.Count==6,"1.5-second cadence");
var edge=Make(FusionPlant.StenchSpreader,0);Enemy(edge,1,300);Ticks(edge,51);
Check(edge.mBoard.Shots.Select(p=>p.mRow).SequenceEqual(new[]{0,1}),"edge has two initial lanes");
Ticks(edge,20);Check(edge.mBoard.Shots.Count==3&&edge.mBoard.Shots[2].mRow==0,"missing edge shot after .2 seconds");
var idle=Make(FusionPlant.StenchSpreader);Enemy(idle,4,200);Ticks(idle,200);
Check(idle.mBoard.Shots.Count==0,"ordinary spreader ignores distant rows");
var snow=Make(FusionPlant.SnowThreepeater);Enemy(snow,4,400);Enemy(snow,0,200);Enemy(snow,1,300);Enemy(snow,3,500);
Check(ThreepeaterFusions.Rows(snow).SequenceEqual(new[]{0,1,4}),"three most threatened distinct rows");
Ticks(snow,51);Check(snow.mBoard.Shots.Count==3&&snow.mBoard.Shots.All(p=>p.mProjectileType==ProjectileType.Snowpea),"all snow bullets use native ice type");
Check(snow.mBoard.Shots[0].mVelY==-6&&snow.mBoard.Shots[2].mVelY==6,"distant row motion scales native velocity");
Ticks(snow,100);Check(snow.mBoard.Shots.Count==6,"one-second snow cadence");
var fill=Make(FusionPlant.SnowThreepeater);Enemy(fill,4,100);
Check(ThreepeaterFusions.Rows(fill).SequenceEqual(new[]{4,2,1}),"vacant slots fill own/previous/next rows");
fill.mBoard.mPaused=true;Ticks(fill,200);Check(fill.mStartRow==0&&fill.mBoard.Shots.Count==0,"pause holds native attack fields");
fill.mBoard.mPaused=false;fill.mIsOnBoard=false;Ticks(fill,100);Check(fill.mBoard.Shots.Count==0,"carried plant cannot attack");
var charred=Make(FusionPlant.CharredThreepeater);Enemy(charred,2,100);Ticks(charred,300);
Check(charred.mBoard.Shots.Count==0&&!ThreepeaterNativeFirePatch.Prefix(charred),"charred has no attack, including animation callback");
var scorched=Make(FusionPlant.ScorchedThreepeater);Enemy(scorched,2,500);Ticks(scorched,51);
var shot=scorched.mBoard.Shots[0];victim=new Zombie{mBoard=scorched.mBoard};shot.DoImpact(victim);
Check(victim.Damage==40,"scorched plain pea actual damage");
shot.mProjectileType=ProjectileType.Fireball;shot.DoImpact(victim);Check(victim.Damage==100,"ordinary Torchwood adds twenty, source fire damage sixty");
ScorchedImpactPatch.Prefix(shot,victim,out var splashOld);
try {
    var neighbor=new Zombie{mBoard=scorched.mBoard};
    ScorchedDamageTargetPatch.Postfix(shot,neighbor);
    neighbor.TakeDamage(13,DamageFlags.HitsShieldAndBody);
    Check(neighbor.Damage==20,"fire splash recipient uses source sixty-damage budget");
    neighbor.TakeDamage(13,DamageFlags.HitsShieldAndBody);
    Check(neighbor.Damage==33,"unrelated damage cannot inherit consumed projectile bonus");
} finally {ScorchedImpactPatch.Finalizer(splashOld);}
var loaded=new Projectile {mBoard=scorched.mBoard,mProjectileType=ProjectileType.Pea,mCobTargetX=shot.mCobTargetX};
Check(ThreepeaterFusions.IsScorchedShot(loaded),"damage identity reconstructs entirely from saved projectile fields");
loaded.Reset();Check(!ThreepeaterFusions.IsScorchedShot(loaded),"native pool reset removes damage identity");
DoomEffects.Rows.Clear();ThreepeaterFusions.OnFusion(scorched);Check(DoomEffects.Rows.SequenceEqual(new[]{1,2,3}),"successful scorched fusion burns three rows");
DoomEffects.Rows.Clear();ThreepeaterFusions.OnFusion(charred);Check(DoomEffects.Rows.Count==0,"charred does not replay fusion blast");
var native=Make(SeedType.Threepeater);Check(ThreepeaterNativeFirePatch.Prefix(native),"native threepeater untouched");
Check(ThreepeaterFusions.Firing==null&&ThreepeaterFusions.Created==null,"fire context restored");

Projectile CherryShot(Plant owner,Zombie target) {
    var p=new Projectile {mBoard=owner.mBoard,mProjectileType=ProjectileType.Pea,mRow=target.mRow};
    owner.mBoard.Shots.Add(p); CherrySpreader.Track(p); return p;
}
var cherry=Make(FusionPlant.CherrySpreader);Enemy(cherry,2,400);
victim=cherry.mBoard.m_zombies[0];
Ticks(cherry,50);Check(cherry.mBoard.Shots.Count==0,"cherry spreader preserves original shot event");
Ticks(cherry,1);
Check(cherry.mBoard.Shots.Count==3&&cherry.mBoard.Shots.All(CherrySpreader.IsFlying),"three distinct native cherry projectiles");
Check(cherry.mLaunchRate==150,"cherry spreader uses 1.5-second source interval");
var bomb=cherry.mBoard.Shots.Single(p=>p.mRow==2);bomb.DoImpact(victim);
Check(CherrySpreader.IsAttached(bomb)&&!bomb.mDead&&victim.Damage==0,"first impact attaches without an extra direct hit");
Check(bomb.mCobTargetRow==20&&(uint)bomb.mTargetZombieID==victim.DataID,"attachment stores source damage and actual target ID");
Check(CherrySpreader.AttachedTo(cherry.mBoard,victim.DataID)==bomb,"one attachment found through native projectile array");
var repeated=CherryShot(cherry,victim);repeated.DoImpact(victim);
Check(repeated.mDead&&victim.Damage==20&&bomb.mCobTargetRow==25,"second semi-ripe cherry charges five and deals twenty");
Check(cherry.mBoard.Shots.Count(p=>!p.mDead&&CherrySpreader.IsAttached(p))==1,"repeated hits never attach a second bomb");
Check(!CherrySpreaderTorchwoodPatch.Prefix(bomb),"attached cherry cannot convert at Torchwood");

var ordinaryCherry=new Projectile {mBoard=cherry.mBoard,mProjectileType=ProjectileType.Pea};
var previousFiring=CherryShooter.FiringPlant;
CherryShooter.FiringPlant=new Plant {mBoard=cherry.mBoard,mSeedType=SeedType.Peashooter,mImitaterType=SeedType.Cherrybomb};
try {CherryShooter.Track(ordinaryCherry);} finally {CherryShooter.FiringPlant=previousFiring;}
ordinaryCherry.DoImpact(victim);
Check(victim.Damage==60&&bomb.mCobTargetRow==35,"existing pink cherry keeps forty damage and contributes ten");
Check(CherrySpreader.CherryDamageBoard==null,"projectile provenance restored after impact");
CherrySpreaderExplosionDamageScope.Prefix(new object[]{new CherryBombPlants.Death(cherry.mBoard)},out var explosionOld);
try {victim.TakeDamage(1800,DamageFlags.HitsShieldAndBody);} finally {CherrySpreaderExplosionDamageScope.Finalizer(explosionOld);}
Check(bomb.mCobTargetRow==485,"actual shared cherry explosion contributes one quarter");
CherrySpreaderExplosionDamageScope.Prefix(new object[]{new CherryBombPlants.Death(cherry.mBoard)},out explosionOld);
try {victim.TakeDamage(3,DamageFlags.HitsShieldAndBody);victim.TakeDamage(7,DamageFlags.HitsShieldAndBody);}
finally {CherrySpreaderExplosionDamageScope.Finalizer(explosionOld);}
Check(bomb.mCobTargetRow==486,"quarter contribution truncates separately per hit");
int beforeCharge=bomb.mCobTargetRow;
victim.TakeDamage(100,DamageFlags.HitsShieldAndBody);
Check(bomb.mCobTargetRow==beforeCharge,"unrelated damage does not charge cherries");

var other=new Zombie{mBoard=cherry.mBoard,mRow=2,X=700};cherry.mBoard.m_zombies.Add(other);
var otherBomb=CherryShot(cherry,other);otherBomb.DoImpact(other);
victim.OnDamage=()=>other.TakeDamage(1000,DamageFlags.HitsShieldAndBody);
CherrySpreaderExplosionDamageScope.Prefix(new object[]{new CherryBombPlants.Death(cherry.mBoard)},out explosionOld);
try {victim.TakeDamage(40,DamageFlags.HitsShieldAndBody);} finally {CherrySpreaderExplosionDamageScope.Finalizer(explosionOld);victim.OnDamage=null;}
Check(bomb.mCobTargetRow==496&&otherBomb.mCobTargetRow==20,"nested unrelated damage cannot inherit cherry provenance");
Check(CherrySpreader.CherryDamageBoard==null,"nested damage restores outer ownership exactly");
CherrySpreader.CherryDamageBoard=new Board {Pointer=(IntPtr)99};
try {victim.TakeDamage(40,DamageFlags.HitsShieldAndBody);} finally {CherrySpreader.CherryDamageBoard=null;}
Check(bomb.mCobTargetRow==496,"another board cannot contribute cherry damage");

var reentrant=CherryShot(cherry,victim);
int directBefore=victim.Damage;
victim.OnDamage=()=>reentrant.DoImpact(victim);
reentrant.DoImpact(victim);victim.OnDamage=null;
Check(victim.Damage==directBefore+20&&bomb.mCobTargetRow==501,"reentrant impact cannot repeat a consumed cherry");
var throwing=CherryShot(cherry,victim);
victim.OnDamage=()=>throw new InvalidOperationException("damage callback failed");
try {throwing.DoImpact(victim);throw new Exception("Expected callback failure");}
catch(InvalidOperationException e) {Check(e.Message=="damage callback failed","damage failure propagated");}
finally {victim.OnDamage=null;}
Check(throwing.mDead&&CherrySpreader.CherryDamageBoard==null,"exception cannot retain an impact or damage context");

int age=bomb.mProjectileAge;cherry.mBoard.mPaused=true;bomb.Update();cherry.mBoard.mPaused=false;
Check(bomb.mProjectileAge==age,"pause freezes the ripening countdown");
victim.X+=20;bomb.Update();
Check(bomb.mPosX==victim.X+10&&bomb.mRow==victim.mRow,"attached original sprite follows its live target");
var near=new Zombie{mBoard=cherry.mBoard,mRow=2,X=bomb.mPosX+CherrySpreader.BlastRadius-1};
var far=new Zombie{mBoard=cherry.mBoard,mRow=2,X=bomb.mPosX+CherrySpreader.BlastRadius+1};
var distantRow=new Zombie{mBoard=cherry.mBoard,mRow=4,X=victim.X};
var ally=new Zombie{mBoard=cherry.mBoard,mRow=2,X=victim.X,mMindControlled=true};
cherry.mBoard.m_zombies.AddRange(new[]{near,far,distantRow,ally});
while(bomb.mProjectileAge<CherrySpreader.RipeningTicks)bomb.Update();
Check(!bomb.mDead&&near.Damage==0,"six-second fuse does not explode early");
int blast=bomb.mCobTargetRow, victimBefore=victim.Damage;
bomb.Update();
Check(bomb.mDead&&victim.Damage-victimBefore==blast&&near.Damage==blast,"physical explosion uses accumulated damage within original source radius");
Check(far.Damage==0&&distantRow.Damage==0&&ally.Damage==0,"explosion preserves distance, row and friendly-target exclusions");
bomb.Update();Check(victim.Damage-victimBefore==blast,"dead attachment cannot explode twice");
Check(CherrySpreader.AttachedTo(cherry.mBoard,victim.DataID)==null,"dead attachment leaves target available for a fresh cherry");
bomb.Reset();Check(!CherrySpreader.IsAttached(bomb)&&bomb.mTargetZombieID==0&&bomb.mCobTargetRow==0,"native pool reset clears attachment identity and target");

var savedPlant=Make(FusionPlant.CherrySpreader);Enemy(savedPlant,2,200);var savedTarget=savedPlant.mBoard.m_zombies[0];
var saved=CherryShot(savedPlant,savedTarget);saved.DoImpact(savedTarget);
for(int i=0;i<250;++i)saved.Update();CherrySpreader.Charge(savedTarget,40);
var restoredBoard=new Board {Pointer=(IntPtr)77};
var restoredTarget=new Zombie{mBoard=restoredBoard,mRow=2,X=250,DataID=savedTarget.DataID};restoredBoard.m_zombies.Add(restoredTarget);
var restored=new Projectile {mBoard=restoredBoard,mProjectileType=saved.mProjectileType,
    mCobTargetX=saved.mCobTargetX,mCobTargetRow=saved.mCobTargetRow,mTargetZombieID=saved.mTargetZombieID,
    mProjectileAge=saved.mProjectileAge,mClickBackoffCounter=saved.mClickBackoffCounter};restoredBoard.Shots.Add(restored);
restored.Update();
Check(CherrySpreader.IsAttached(restored)&&restored.mCobTargetRow==30&&restored.mProjectileAge==251,"serialized fields reconstruct target, power and fuse without global state");
Check(restored.mPosX==260&&CherrySpreader.AttachedTo(restoredBoard,restoredTarget.DataID)==restored,"loaded attachment follows target in its own board");
restoredTarget.DataID++;restored.Update();
Check(restored.mDead&&restoredTarget.Damage==0,"pooled target cannot inherit an old attachment or explosion");
var deadOwner=CherryShot(savedPlant,savedTarget);saved.mDead=true;deadOwner.DoImpact(savedTarget);savedTarget.mDead=true;deadOwner.Update();
Check(deadOwner.mDead,"attachment is removed when its owner dies");
var cherryEdge=Make(FusionPlant.CherrySpreader,0);Enemy(cherryEdge,1,300);Ticks(cherryEdge,71);
Check(cherryEdge.mBoard.Shots.Select(p=>p.mRow).SequenceEqual(new[]{0,1,0})&&cherryEdge.mBoard.Shots.All(CherrySpreader.IsFlying),"edge replacement shot retains cherry identity");
var airPlant=Make(FusionPlant.CherrySpreader);Enemy(airPlant,2,200);var airOwner=airPlant.mBoard.m_zombies[0];
var airBomb=CherryShot(airPlant,airOwner);airBomb.DoImpact(airOwner);airOwner.Flying=true;
var airNeighbor=new Zombie{mBoard=airPlant.mBoard,mRow=2,X=220,Flying=true};
var groundNeighbor=new Zombie{mBoard=airPlant.mBoard,mRow=2,X=220};airPlant.mBoard.m_zombies.AddRange(new[]{airNeighbor,groundNeighbor});
airBomb.mProjectileAge=CherrySpreader.RipeningTicks;airBomb.Update();
Check(airOwner.Damage==20&&airNeighbor.Damage==20&&groundNeighbor.Damage==0,"airborne owner preserves source separation of flying and ground explosions");
// Phoenix projectiles keep three distinct hits, independent of their owner.
var phoenix=Make(FusionPlant.PhoenixThreepeater);Enemy(phoenix,2,400);Ticks(phoenix,51);
Check(phoenix.mBoard.Shots.Count==3&&phoenix.mBoard.Shots.All(PhoenixProjectiles.Contains),"phoenix source volley");
var fire=phoenix.mBoard.Shots[0];
var fireVictims=Enumerable.Range(0,3).Select(_=>new Zombie{mBoard=phoenix.mBoard}).ToArray();
PhoenixProjectiles.Impact(fire,fireVictims[0]);PhoenixProjectiles.Impact(fire,fireVictims[0]);
Check(fireVictims[0].Damage==160&&!fire.mDead,"one hit per zombie, two penetrations remain");
PhoenixProjectiles.Impact(fire,fireVictims[1]);PhoenixProjectiles.Impact(fire,fireVictims[2]);
Check(fire.mDead&&fireVictims.All(z=>z.Damage==160),"third distinct hit consumes phoenix pea");
var revived=Make(FusionPlant.CharredThreepeater);PhoenixProjectiles.IgniteRow(revived.mBoard,revived.mRow);
Check(revived.mDead&&FusionPlant.From(revived.mBoard.Created!)==FusionPlant.PhoenixThreepeater,"Jalapeno lane revives ash");
var retained=Make(FusionPlant.CharredThreepeater);retained.mBoard.FailCreate=true;PhoenixProjectiles.IgniteRow(retained.mBoard,retained.mRow);
Check(!retained.mDead,"failed revival preserves original");
var sunBoard=new Board();SolarShooter.DropSun(sunBoard,0,0,5);var pickup=sunBoard.Coins.Single();
SolarShooter.Clear();Check(SolarShooter.CustomSunValue(pickup)==5,"small Sun value survives cache loss");
var loadedSun=new Coin {mBoard=sunBoard,mType=pickup.mType,mUsableSeedType=pickup.mUsableSeedType};
Check(SolarShooter.CustomSunValue(loadedSun)==5,"serialized small Sun reconstructs five-sun value");
Check(SolarShooter.CustomSunValue(new Coin {mBoard=sunBoard,mType=CoinType.SmallSun})==0,"ordinary small Sun remains native");
var multi = Make(FusionPlant.Multipeater);
Enemy(multi, 2, 450);
Ticks(multi, 100);
Check(multi.mBoard.Shots.Count == 6, "Multipeater standard mode emits two peas in each adjacent row");
multi.mBoard.Shots.Clear(); multi.mLaunchCounter = 0;
ThreepeaterFusions.ClickMulti(multi, 1, 0);
Check(multi.mTargetY == 1, "click selects coverage mode");
Ticks(multi, 50);
Check(multi.mBoard.Shots.Count == 5 && multi.mBoard.Shots.Select(s => s.mRow).Distinct().Count() == 5,
    "coverage fires into every row");
Check(multi.mBoard.Shots.All(s => ThreepeaterFusions.MultiDamage(s, 20) == 24), "five rows preserve 120 base damage");
var coverage = multi.mBoard.Shots[0];
coverage.mProjectileType = ProjectileType.Fireball;
Check(ThreepeaterFusions.MultiDamage(coverage, 40) == 44 && ThreepeaterFusions.MultiDamage(coverage, 13) == 14,
    "Torchwood retains its flat twenty damage and native splash fraction");
multi.mBoard.Shots.Clear(); multi.mLaunchCounter = 0;
ThreepeaterFusions.ClickMulti(multi, 1, 0);
Ticks(multi, 1);
ThreepeaterFusions.ClickMulti(multi, 1, 0);
Ticks(multi, 99);
Check(multi.mTargetY == 0 && multi.mTargetX == 2 && multi.mBoard.Shots.Count == 6
    && multi.mBoard.Shots.All(s => s.mRow == multi.mRow), "mid-burst mode changes affect only the next attack");
multi.mBoard.mPaused = true;
ThreepeaterFusions.ClickMulti(multi, 1, 0);
Check(multi.mTargetY == 0, "pause prevents changing mode");
var staggered = Make(FusionPlant.Multipeater);
Enemy(staggered, staggered.mRow, 450);
staggered.mTargetY = 2;
var emissionTicks = new List<int>();
for (int tick = 0; tick < 100; ++tick)
{
    int before = staggered.mBoard.Shots.Count;
    Ticks(staggered, 1);
    int emitted = staggered.mBoard.Shots.Count - before;
    Check(emitted <= 1, "single-row mode never stacks peas on the same tick");
    if (emitted != 0) emissionTicks.Add(tick);
}
Check(emissionTicks.SequenceEqual(new[] { 42, 50, 58, 67, 75, 83 }),
    "single-row mode spreads the original six peas across two quick bursts");
Check(staggered.mBoard.Shots.All(shot => shot.mRow == staggered.mRow), "staggered peas retain own-row collision");
Console.WriteLine($"Threepeater fusions: {checks} checks passed.");
