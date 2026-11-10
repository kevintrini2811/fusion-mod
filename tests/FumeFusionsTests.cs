using FusionMod;
using Il2CppReloaded.Gameplay;
int checks=0; void Check(bool ok,string why) { ++checks; if(!ok)throw new Exception(why); }
uint next=1;
Plant Make(FusionPlant f) { var p=new Plant {mSeedType=f.NativeType,mImitaterType=f.Marker}; FumeFusions.Initialize(p); return p; }
Zombie Enemy(Plant p,int row=2,float x=400) {var z=new Zombie {mZombieType=ZombieType.Normal,mBoard=p.mBoard,DataID=next++,Pointer=(IntPtr)next,mRow=row,X=x};p.mBoard.m_zombies.Add(z);return z;}
void Tick(Plant p,int ticks) { for(int i=0;i<ticks;++i) Check(!FumeTickPatch.Prefix(p),"native shooter suppressed"); }
foreach(var (ingredient,result) in new[]{(SeedType.Iceshroom,FusionPlant.FrostShroom),(SeedType.Jalapeno,FusionPlant.FlameShroom),(SeedType.Doomshroom,FusionPlant.SootShroom),(SeedType.Threepeater,FusionPlant.FumeSpreader)}) {
 Check(FusionRecipes.TryGet(SeedType.Fumeshroom,ingredient,out var f)&&f==result,"recipe");
 Check(FusionRecipes.TryGet(ingredient,SeedType.Fumeshroom,out f)&&f==result,"reverse recipe");
 var p=Make(result);Check(FusionPlant.From(p)==result&&FusionPlant.HasImportedVisuals(result),"saved identity/visuals");
 Check(p.mPlantMaxHealth==300,"health");Check(!FumeNativeFirePatch.Prefix(p),"no native duplicate damage");
}
var frost=Make(FusionPlant.FrostShroom);var target=Enemy(frost);
Tick(frost,35);Check(target.Hits==0,"original emit delay");Tick(frost,1);Check(target.Damage==40&&target.FreezePoints==5&&target.Chills==1,"frost source damage and five freeze points");
Tick(frost,150);Check(target.Hits==2,"150 tick cadence");
var flame=Make(FusionPlant.FlameShroom);target=Enemy(flame);Tick(flame,100);
Check(target.Hits==33&&target.Damage==33&&target.Heats==33&&target.ColdClears==33,"33 sustained fire hits/overheat");
var soot=Make(FusionPlant.SootShroom);target=Enemy(soot,x:2000);Tick(soot,50);Check(target.Hits==0,"manual only");
Check(FumeFusions.Click(soot,1,0),"manual click claimed");Tick(soot,36);Check(target.Damage==1800,"entire forward row");
int cooldown=soot.mLaunchCounter;Check(FumeFusions.Click(soot,1,0)&&soot.mLaunchCounter==cooldown,"cannot bypass reload");Tick(soot,3000);Check(target.Hits==1,"no automatic repeat");
Check(FumeFusions.Click(soot,1,0)&&soot.mLaunchCounter==3000,"ready after 30 seconds");
foreach(var rows in new[]{5,6}) foreach(var row in new[]{0,2,rows-1}) {
 var p=Make(FusionPlant.FumeSpreader);p.mBoard.Rows=rows;p.mRow=row;
 var center=Enemy(p,row,2000);var up=Enemy(p,row-1,2000);var down=Enemy(p,row+1,2000);
 // Exclude impossible board rows from the fixture.
 if(row==0)up.mDead=true;if(row==rows-1)down.mDead=true;
 Tick(p,55);Check(center.Hits==0,"200 ms delayed penetrating wave");Tick(p,1);
 Check(center.Damage==(row==0||row==rows-1?80:40),"edge lane doubles center damage");
 Check(up.Damage==(up.mDead?0:40)&&down.Damage==(down.mDead?0:40),"adjacent lanes");
}
foreach(var state in new[]{"pause","sleep","carry","dead","squished","bungee"}) {
 var p=Make(FusionPlant.FlameShroom);target=Enemy(p);Tick(p,36);int phase=p.mStartRow,launch=p.mLaunchCounter,hits=target.Hits;
 if(state=="pause")p.mBoard.mPaused=true;if(state=="sleep")p.mIsAsleep=true;if(state=="carry")p.mIsOnBoard=false;
 if(state=="dead")p.mDead=true;if(state=="squished")p.mSquished=true;if(state=="bungee")p.Bungee=true;
 Tick(p,50);Check(p.mStartRow==phase&&p.mLaunchCounter==launch&&target.Hits==hits,"inactive state: "+state);
}
var filter=Make(FusionPlant.FrostShroom);var behind=Enemy(filter,x:100);var far=Enemy(filter,x:655);var flying=Enemy(filter);flying.Category=DamageRangeFlags.Flying;
var ally=Enemy(filter);ally.mMindControlled=true;var other=Enemy(filter);other.mBoard=new Board{Pointer=(IntPtr)2};var rowTarget=Enemy(filter,3);
FumeFusions.Hit(filter,FusionPlant.FrostShroom);Check(new[]{behind,far,flying,ally,other,rowTarget}.All(z=>z.Hits==0),"target boundaries");
var near=Enemy(filter,x:654);FumeFusions.Hit(filter,FusionPlant.FrostShroom);Check(near.Hits==1&&far.Hits==0,"source seven units / 1.35 units per column");
var saved=Make(FusionPlant.FlameShroom);target=Enemy(saved);Tick(saved,62);int previous=target.Hits;
var loaded=Make(FusionPlant.FlameShroom);loaded.mStartRow=saved.mStartRow;loaded.mLaunchCounter=saved.mLaunchCounter;var afterLoad=Enemy(loaded);
Tick(loaded,38);Check(previous+afterLoad.Hits==33,"save/load mid-flame neither loses nor duplicates hits");
var reentrant=Make(FusionPlant.FrostShroom);var a=Enemy(reentrant);var b=Enemy(reentrant);a.OnDamage=()=>++b.DataID;
FumeFusions.Hit(reentrant,FusionPlant.FrostShroom);Check(a.Hits==1&&b.Hits==0,"pool reuse during damage callback");
var cursor=Make(FusionPlant.SootShroom);cursor.mBoard.mCursorObject.CursorType=CursorType.Shovel;FumeFusions.Click(cursor,1,0);Check(cursor.mStartRow==0,"shovel does not fire");
cursor.mBoard.mCursorObject.CursorType=CursorType.Normal;cursor.mBoard.Carried=cursor;FumeFusions.Click(cursor,1,0);Check(cursor.mStartRow==0,"glove does not fire");
Check(FumeTickPatch.Prefix(new Plant()),"ordinary native plant unchanged");
foreach(var (ingredient,result) in new[]{(SeedType.Garlic,FusionPlant.FoulShroom),(SeedType.Potatomine,FusionPlant.StarchShroom)}) {
 Check(FusionRecipes.TryGet(SeedType.Fumeshroom,ingredient,out var f)&&f==result,"new fume recipe");
 Check(FusionRecipes.TryGet(ingredient,SeedType.Fumeshroom,out f)&&f==result,"reverse new fume recipe");
 var p=Make(result);Check(FusionPlant.From(p)==result&&FusionPlant.HasImportedVisuals(result),"new fume saved identity and artwork");
}
var foul=Make(FusionPlant.FoulShroom);target=Enemy(foul);Tick(foul,36);
Check(target.Damage==30&&target.PoisonLevels==1,"foul fume damages before adding one poison level");
Tick(foul,150);Check(target.Damage==60&&target.PoisonLevels==2,"foul cadence");
var starch=Make(FusionPlant.StarchShroom);target=Enemy(starch);
Tick(starch,36);Check(target.Damage==30&&MineFusions.Blasts.Count==0,"first attack ordinary; charge starts at 15 seconds");
Tick(starch,1464);Check(starch.mTargetX==0,"charge completes after 1500 active ticks");
Tick(starch,36);Check(MineFusions.Blasts.Count==1,"next wave becomes potato explosion");
Check(MineFusions.Blasts.Single() is {Radius:0,Damage:300,Row:2},"source point overlap and same-row 300 damage");
Check(target.Damage==300,"charged wave does not add an ordinary 30-damage hit");
// 10 ordinary waves at ticks 36..1386 precede the charged wave at 1536.
Check(starch.mTargetX==1500,"charge resets on emitting empowered wave");
MineFusions.Blasts.Clear();starch.mTargetX=0;var second=Enemy(starch,x:500);
FumeFusions.Hit(starch,FusionPlant.StarchShroom);
Check(MineFusions.Blasts.Count==2&&MineFusions.Blasts.Select(b=>b.X).Distinct().Count()==2,"one independent explosion per hit zombie");
MineFusions.Blasts.Clear();starch.mTargetX=0;starch.mBoard.m_zombies.Clear();FumeFusions.Hit(starch,FusionPlant.StarchShroom);
Check(starch.mTargetX==1500&&MineFusions.Blasts.Count==0,"empty wave still consumes charge");
var original=Make(FusionPlant.StarchShroom);Tick(original,700);
var restored=Make(FusionPlant.StarchShroom);restored.mTargetX=original.mTargetX;restored.mTargetY=original.mTargetY;
restored.mShootingCounter=original.mShootingCounter;restored.mStartRow=original.mStartRow;restored.mLaunchCounter=original.mLaunchCounter;
Tick(original,800);Tick(restored,800);Check(restored.mTargetX==0&&original.mTargetX==0&&restored.mTargetY==original.mTargetY,"charge and flash save/load");
foreach(var mode in new[]{"pause","sleep","carry","dead","bungee"}) {
 var p=Make(FusionPlant.StarchShroom);Tick(p,70);int charge=p.mTargetX,flash=p.mTargetY,phase=p.mShootingCounter;
 if(mode=="pause")p.mBoard.mPaused=true;if(mode=="sleep")p.mIsAsleep=true;if(mode=="carry")p.mIsOnBoard=false;
 if(mode=="dead")p.mDead=true;if(mode=="bungee")p.Bungee=true;
 Tick(p,100);Check(p.mTargetX==charge&&p.mTargetY==flash&&p.mShootingCounter==phase,"inactive charge/flash: "+mode);
}
// Source Gloom clips use different playback rates. Exercise real simulation,
// saved phases and exact recipient validation, not a duplicate timing model.
foreach (var identity in new[]{FusionPlant.GloomShroom,FusionPlant.FrostGloomShroom,FusionPlant.FlameGloomShroom}) {
 var p=Make(identity);GloomFusions.Initialize(p);var victim=Enemy(p,x:300);
 Check(FusionPlant.From(p)==identity&&FusionPlant.HasImportedVisuals(identity),"gloom identity/art");
 Check(p.mPlantHealth==300&&p.mLaunchRate==190,"gloom health/cadence");
 Check(!GloomNativeFirePatch.Prefix(p),"gloom native callback suppressed");
 int first=identity==FusionPlant.GloomShroom?43:47;
 for(int i=0;i<first;++i)Check(!GloomTickPatch.Prefix(p),"gloom owns shooter");
 Check(victim.Hits==0,"gloom waits for original first clip event");
 GloomTickPatch.Prefix(p);Check(victim.Hits==1,"gloom first pulse");
 Check(ImportedPeaVisuals.LastSmokeIdentity==identity,"gloom pulse requests the exact original smoke variant");
 var copy=Make(identity);GloomFusions.Initialize(copy);copy.mStartRow=p.mStartRow;copy.mLaunchCounter=p.mLaunchCounter;
 var restoredVictim=Enemy(copy,x:300);
 for(int i=0;i<150;++i)GloomTickPatch.Prefix(copy);
 Check(restoredVictim.Hits==3,"load after first pulse preserves remaining three pulses");
 Check(restoredVictim.Damage==(identity==FusionPlant.GloomShroom?60:120),"gloom penetrating damage");
 if(identity==FusionPlant.FrostGloomShroom)Check(restoredVictim.Chills==3&&restoredVictim.FreezePoints==15,"gloom cold and five freeze points per pulse");
 if(identity==FusionPlant.FlameGloomShroom)Check(restoredVictim.Heats==3&&restoredVictim.ColdClears==3,"gloom fire and overheat each pulse");
 p.mBoard.mPaused=true;int phase=p.mStartRow,reload=p.mLaunchCounter;GloomTickPatch.Prefix(p);
 Check(p.mStartRow==phase&&p.mLaunchCounter==reload,"gloom pause preserves phase/cooldown");
}
var around=Make(FusionPlant.FrostGloomShroom);GloomFusions.Initialize(around);
var back=Enemy(around,x:150);var side=Enemy(around,row:1,x:240);var distant=Enemy(around,x:500);
var rowFar=Enemy(around,row:0,x:240);var friendly=Enemy(around,x:240);friendly.mMindControlled=true;
GloomFusions.Hit(around,FusionPlant.FrostGloomShroom);
Check(back.Hits==1&&side.Hits==1&&distant.Hits==0&&rowFar.Hits==0&&friendly.Hits==0,"gloom rear/adjacent/radius/row/friendly target rules");
var reuse=Make(FusionPlant.FlameGloomShroom);GloomFusions.Initialize(reuse);var firstVictim=Enemy(reuse,x:280);var recycled=Enemy(reuse,x:300);
firstVictim.OnDamage=()=>++recycled.DataID;GloomFusions.Hit(reuse,FusionPlant.FlameGloomShroom);
Check(firstVictim.Hits==1&&recycled.Hits==0,"gloom target snapshot rejects recycled zombie");
foreach(var (left,right,result) in new[]{
 ((FusionPlant)SeedType.Fumeshroom,FusionPlant.GloomShroom,FusionPlant.GloomShroom),
 (FusionPlant.FrostShroom,FusionPlant.GloomShroom,FusionPlant.FrostGloomShroom),
 (FusionPlant.GloomShroom,(FusionPlant)SeedType.Iceshroom,FusionPlant.FrostGloomShroom),
 (FusionPlant.FlameShroom,FusionPlant.GloomShroom,FusionPlant.FlameGloomShroom),
 (FusionPlant.GloomShroom,(FusionPlant)SeedType.Jalapeno,FusionPlant.FlameGloomShroom),
 ((FusionPlant)SeedType.Fumeshroom,(FusionPlant)SeedType.Scaredyshroom,FusionPlant.GutsyShroom)}) {
 Check(FusionRecipes.TryGet(left,right,out var f)&&f==result,"new source recipe");
 Check(FusionRecipes.TryGet(right,left,out f)&&f==result,"reverse new source recipe");
}
var gutsy=Make(FusionPlant.GutsyShroom);var close=Enemy(gutsy,x:400);var farBehind=Enemy(gutsy,x:750);
Tick(gutsy,75);Check(close.Hits==0,"gutsy smoke original event delay");Tick(gutsy,1);
Check(close.Damage==30&&farBehind.Hits==0&&gutsy.mBoard.Projectiles.Count==0,"gutsy chooses penetrating smoke for nearby target");
var farGutsy=Make(FusionPlant.GutsyShroom);var farEnemy=Enemy(farGutsy,x:750);
Tick(farGutsy,38);Check(farGutsy.mBoard.Projectiles.Count==0,"gutsy spores wait for clip event at 0.375 seconds");
Tick(farGutsy,1);Check(farGutsy.mBoard.Projectiles.Count==1,"gutsy first spore");Tick(farGutsy,17);
Check(farGutsy.mBoard.Projectiles.Count==2&&farEnemy.Hits==0,"gutsy two staggered collision projectiles");
var spore=farGutsy.mBoard.Projectiles[0];spore.Pointer=(IntPtr)77;farEnemy.Pointer=(IntPtr)88;
Check(GutsyShroom.IsShot(spore)&&spore.mMotionType==ProjectileMotion.Star&&spore.mVelX>0&&spore.mVelY==0,"saved spore marker/forward motion");
Check(spore.mShadowY-spore.mPosY==40,"spore clears native ground-impact cutoff");
spore.mShadowY=spore.mPosY;
GutsyFlightHeightPatch.Prefix(spore);
Check(spore.mShadowY-spore.mPosY==40,"movement restores spore height before native collision, including loaded projectiles");
Check(!GutsyTorchwoodPatch.Prefix(spore),"spore cannot ignite");
GutsyImpactPatch.Prefix(spore,farEnemy,out var scope);GutsyDamageTargetPatch.Postfix(spore,farEnemy);
int damage=20;GutsyDamagePatch.Prefix(farEnemy,ref damage);Check(damage==30,"native impact adjusted to source thirty damage");
damage=20;GutsyDamagePatch.Prefix(farEnemy,ref damage);Check(damage==20,"spore damage adjustment consumed exactly once");GutsyImpactPatch.Finalizer(scope);
GutsyImpactPatch.Prefix(spore,farEnemy,out scope);GutsyDamageTargetPatch.Postfix(spore,farEnemy);++farEnemy.DataID;
damage=20;GutsyDamagePatch.Prefix(farEnemy,ref damage);Check(damage==20,"recycled impact recipient rejected");GutsyImpactPatch.Finalizer(scope);
var sporeLoaded=new Projectile{mProjectileType=spore.mProjectileType,mCobTargetX=spore.mCobTargetX,mMotionType=spore.mMotionType};
Check(GutsyShroom.IsShot(sporeLoaded),"spore behavior recovers entirely from native saved fields");sporeLoaded.mCobTargetX=0;
Check(!GutsyShroom.IsShot(sporeLoaded)&&GutsyTorchwoodPatch.Prefix(sporeLoaded),"pool reset removes marker");
foreach(var mode in new[]{"pause","sleep","carry","dead","bungee"}) {
 var p=Make(FusionPlant.GutsyShroom);Enemy(p,x:750);Tick(p,39);int phase=p.mStartRow,reload=p.mLaunchCounter,count=p.mBoard.Projectiles.Count;
 if(mode=="pause")p.mBoard.mPaused=true;if(mode=="sleep")p.mIsAsleep=true;if(mode=="carry")p.mIsOnBoard=false;
 if(mode=="dead")p.mDead=true;if(mode=="bungee")p.Bungee=true;
 Tick(p,100);Check(p.mStartRow==phase&&p.mLaunchCounter==reload&&p.mBoard.Projectiles.Count==count,"gutsy inactive: "+mode);
}
var mid=Make(FusionPlant.GutsyShroom);Enemy(mid,x:750);Tick(mid,39);
var midRestored=Make(FusionPlant.GutsyShroom);midRestored.mTargetX=mid.mTargetX;midRestored.mStartRow=mid.mStartRow;midRestored.mLaunchCounter=mid.mLaunchCounter;
Tick(midRestored,30);Check(midRestored.mBoard.Projectiles.Count==1,"load mid-salvo produces remaining spore once even after target disappears");

// New fume contracts: production sources run against offline native doubles.
foreach(var (ingredient,result) in new[]{(SeedType.Peashooter,FusionPlant.BoostShroom),
 (SeedType.Torchwood,FusionPlant.BlowtorchShroom),(SeedType.Wallnut,FusionPlant.WallopShroom),
 (SeedType.Spikeweed,FusionPlant.CaltropShroom)}) {
 Check(FusionRecipes.TryGet(SeedType.Fumeshroom,ingredient,out var f)&&f==result,"variant recipe");
 Check(FusionRecipes.TryGet(ingredient,SeedType.Fumeshroom,out f)&&f==result,"variant reverse recipe");
 var p=Make(result);Check(FusionPlant.From(p)==result&&FusionPlant.HasImportedVisuals(result),"variant identity/art");
 Check(p.mPlantMaxHealth==(result==FusionPlant.WallopShroom?4000:300),"variant health");
}
var boost=Make(FusionPlant.BoostShroom);var boostEnemy=Enemy(boost);
var pea=boost.mBoard.AddProjectile(300,240,1,2,ProjectileType.Pea);
Tick(boost,35);Check(FumeProjectiles.Damage(pea,20)==20,"no boost before original StartShooting");
Tick(boost,10);Check(FumeProjectiles.Damage(pea,20)>20&&FumeProjectiles.Speed(pea)>3.33f,"stacked damage and speed");
int boostDamage=FumeProjectiles.Damage(pea,20);float speed=FumeProjectiles.Speed(pea);
boost.mBoard.mPaused=true;Tick(boost,10);Check(FumeProjectiles.Damage(pea,20)==boostDamage,"pause stops boost");
boost.mBoard.mPaused=false;boost.mIsOnBoard=false;Tick(boost,10);
Check(FumeProjectiles.Damage(pea,20)==boostDamage&&FumeProjectiles.Speed(pea)==speed,"leaving plant preserves projectile bonus");
boost.mIsOnBoard=true;boost.mBoard.m_gridItems.Full=true;
var unbacked=boost.mBoard.AddProjectile(320,240,1,2,ProjectileType.Pea);FumeProjectiles.Boost(boost);
Check(FumeProjectiles.Damage(unbacked,20)==20,"allocation failure grants no unsaved modifier");
boost.mBoard.m_gridItems.Full=false;
var effect=boost.mBoard.m_gridItems.Single(i=>FumeProjectiles.IsBoost(i));
int bonus=FumeProjectiles.Damage(pea,0);
FumeProjectiles.CurrentHit=new(pea,boostEnemy,bonus,false);int direct=20;
FumeProjectiles.ApplyBonus(boostEnemy,ref direct);Check(direct==20+bonus,"boost reaches health writer");
FumeProjectiles.ApplyBonus(boostEnemy,ref direct);Check(direct==20+bonus,"bonus applied once per target");
FumeProjectiles.CurrentHit=new(pea,boostEnemy,bonus,false);
Check(FumeProjectiles.ModifiedDamage(pea,20)==20+bonus&&!FumeProjectiles.CurrentHit.Pending,"polar owns pre-multiplier bonus");
FumeProjectiles.CurrentHit=null;pea.Die();FumeProjectiles.TickRecord(effect);Check(effect.mDead,"dead projectile cleans saved bonus");
var torch=Make(FusionPlant.BlowtorchShroom);target=Enemy(torch);Tick(torch,200);
Check(target.Hits==0,"blowtorch has no autonomous damage");
float mouth=torch.mX+torch.mWidth/2;
var snowShot=torch.mBoard.AddProjectile(mouth,240,1,2,ProjectileType.Snowpea);Tick(torch,2);
Check(!snowShot.mDead&&snowShot.mProjectileType==ProjectileType.Pea&&torch.mTargetX==0,"thawed pea passes same torch");
for(int i=0;i<20;++i)torch.mBoard.AddProjectile(mouth,240,1,2,ProjectileType.Pea);
Tick(torch,1);Check(torch.mTargetX==300,"absorbed energy capped at 300");
Tick(torch,49);Check(target.Hits==0,"flame waits through 150 ms coroutine delay");
Tick(torch,1);Check(target.Damage==300&&torch.mTargetX==0&&target.ColdClears==1,"flame releases energy and thaws targets");
var wallop=Make(FusionPlant.WallopShroom);target=Enemy(wallop);Tick(wallop,400);
Check(wallop.mBoard.Projectiles.Count==0,"wallop needs ammunition");
var ammo=new Plant{mBoard=wallop.mBoard,mSeedType=FusionPlant.NutShroom.NativeType,mImitaterType=FusionPlant.NutShroom.Marker,
 mPlantHealth=700,mPlantCol=wallop.mPlantCol+1,mRow=wallop.mRow};wallop.mBoard.m_plants.Add(ammo);
Tick(wallop,90);Check(ammo.mDead&&wallop.mTargetX==700&&wallop.mBoard.Projectiles.Count==0,"ammo consumed and saved before event");
wallop.mBoard.SpawnFails=true;Tick(wallop,1);Check(wallop.mTargetX==700,"failed projectile allocation retains saved ammunition");
wallop.mBoard.SpawnFails=false;Tick(wallop,400);
var nutShot=wallop.mBoard.Projectiles.Single();Check(FumeProjectiles.IsNut(nutShot)&&nutShot.mCobTargetRow==720,"original nut projectile receives current ammo health");
var nutBoard=nutShot.mBoard;nutBoard.m_zombies.Clear();
var armored=new Zombie{mBoard=nutBoard,DataID=next++,mRow=nutShot.mRow,X=300,mBodyHealth=100,mHelmHealth=20,mShieldHealth=1000};nutBoard.m_zombies.Add(armored);
for(int i=0;i<20;++i)FumeProjectiles.MoveNut(nutShot);
Check(armored.Hits==1&&nutShot.mCobTargetRow==600,"piercing spends body and helmet, ignores shield, hits once");
Check(nutBoard.m_gridItems.Any(NutProjectiles.IsHistory),"piercing history uses native saved records");
var blocked=Make(FusionPlant.WallopShroom);FumeProjectiles.LaunchNut(blocked,100);var blockedShot=blocked.mBoard.Projectiles.Single();
var blockedVictim=Enemy(blocked,x:300);blocked.mBoard.m_gridItems.Full=true;
for(int i=0;i<20&&!blockedShot.mDead;++i)FumeProjectiles.MoveNut(blockedShot);
Check(blockedShot.mDead&&blockedVictim.Hits==0,"history allocation failure never grants unsaved piercing");
var caltrop=Make(FusionPlant.CaltropShroom);var driver=Enemy(caltrop,x:500);driver.mZombieType=ZombieType.Zamboni;driver.mBodyMaxHealth=1000;
for(int i=0;i<57;++i)FumeVariants.Tick(caltrop,FusionPlant.CaltropShroom);
Check(driver.Hits==0,"caltrop waits for AnimAttack");FumeVariants.Tick(caltrop,FusionPlant.CaltropShroom);
Check(driver.Damage==340,"caltrop smoke and thirty percent max vehicle health");
driver.X=caltrop.mX+caltrop.mWidth/2;FumeVariants.Tick(caltrop,FusionPlant.CaltropShroom);
Check(caltrop.mDead&&driver.Damage==2140,"vehicle contact punctures tires and consumes caltrop");
var rollingSeaNut=Make(FusionPlant.SeaNut);
rollingSeaNut.mStartRow=2;
Check(!ReactiveFumeDamage.Invulnerable(rollingSeaNut),"Sea-nut before rolling remains vulnerable");
rollingSeaNut.mStartRow=3;
Check(ReactiveFumeDamage.Invulnerable(rollingSeaNut),"Sea-nut rolling reuses actual health-writer immunity");
rollingSeaNut.mBoard.mPaused=true;
Check(!ReactiveFumeDamage.Invulnerable(rollingSeaNut),"paused board does not enter active damage interception");

foreach (var identity in new[]{FusionPlant.BoostShroom,FusionPlant.BlowtorchShroom,FusionPlant.WallopShroom,FusionPlant.CaltropShroom}) {
 var variantSaved=Make(identity); variantSaved.mStartRow=17;variantSaved.mLaunchCounter=23;variantSaved.mTargetX=91;variantSaved.mTargetY=1;
 variantSaved.mBoard.mPaused=true;FumeVariants.Tick(variantSaved,identity);
 Check(variantSaved.mStartRow==17&&variantSaved.mLaunchCounter==23&&variantSaved.mTargetX==91&&variantSaved.mTargetY==1,"new phases pause without reset");
 var variantLoaded=Make(identity);variantLoaded.mStartRow=variantSaved.mStartRow;variantLoaded.mLaunchCounter=variantSaved.mLaunchCounter;
 variantLoaded.mTargetX=variantSaved.mTargetX;variantLoaded.mTargetY=variantSaved.mTargetY;
 Check(variantLoaded.mTargetX==91&&variantLoaded.mStartRow==17&&variantLoaded.mLaunchCounter==23,"variantSaved variant counters retain energy/ammunition and phase");
}

// Morph-shroom must classify the current armor on every actual penetrating hit.
var morph=Make(FusionPlant.MorphShroom);var metal=Enemy(morph);metal.mZombieType=ZombieType.Normal;metal.mHelmType=HelmType.Pail;metal.mHelmHealth=100;
var cone=Enemy(morph);cone.mZombieType=ZombieType.Normal;cone.mHelmType=HelmType.TrafficCone;cone.mHelmHealth=100;
var metalDriver=Enemy(morph);metalDriver.mZombieType=ZombieType.Zamboni;
FumeFusions.Hit(morph,FusionPlant.MorphShroom);
Check(metal.Damage==80&&cone.Damage==40&&metalDriver.Damage==80,"metal equipment/body classifier");
metal.mHelmHealth=0;FumeFusions.Hit(morph,FusionPlant.MorphShroom);
Check(metal.Damage==120,"lost bucket removes bonus");
var door=Enemy(morph);door.mShieldType=ShieldType.Door;door.mShieldHealth=100;
FumeFusions.Hit(morph,FusionPlant.MorphShroom);Check(door.Damage==80,"door counts even though fume bypasses shield");

var rust=Make(FusionPlant.RustShroom);var rustTarget=Enemy(rust);rustTarget.mHelmHealth=100;
Check(rust.mPlantMaxHealth==2000,"rust source toughness");
FumeFusions.Hit(rust,FusionPlant.RustShroom);Check(rustTarget.Damage==150,"rust triples damage with type 1 armor");
rustTarget.mHelmHealth=0;rustTarget.mShieldHealth=500;
FumeFusions.Hit(rust,FusionPlant.RustShroom);Check(rustTarget.Damage==200,"type 2 shield alone does not triple rust damage");

// A cap emits no direct fume hit. Its three contacts survive a saved-field copy.
var starburst=Make(FusionPlant.StarburstShroom);var starTarget=Enemy(starburst);
Tick(starburst,34);Check(starburst.mBoard.Projectiles.Count==0,"star cap original emit delay");
Tick(starburst,1);Check(starTarget.Damage==0&&starburst.mBoard.Projectiles.Count==1,"star cap replaces direct fume damage");
var cap=starburst.mBoard.Projectiles.Single();cap.mPosX=starTarget.X;
StarProjectiles.Children.Clear();StarCaps.Collide(cap);
Check(StarProjectiles.Children.Count==5&&StarProjectiles.Children.All(c=>c.Damage==20),"first cap contact emits five twenty-damage stars");
StarCaps.Collide(cap);Check(StarProjectiles.Children.Count==5,"overlapping owner cannot repeat contact");
var secondCapTarget=Enemy(starburst,x:500);cap.mPosX=500;StarCaps.Collide(cap);
Check(!cap.mDead&&StarProjectiles.Children.Count==10,"cap survives second distinct contact");
var restoredCap=new Projectile {mBoard=cap.mBoard,mProjectileType=cap.mProjectileType,mCobTargetX=cap.mCobTargetX,
 mCobTargetRow=cap.mCobTargetRow,mTargetZombieID=cap.mTargetZombieID,mHitTorchwoodGridX=cap.mHitTorchwoodGridX,
 mRow=cap.mRow,mPosX=500};
StarCaps.Collide(restoredCap);Check(StarProjectiles.Children.Count==10,"loaded contact history excludes existing targets");
Enemy(starburst,x:600);restoredCap.mPosX=600;StarCaps.Collide(restoredCap);
Check(restoredCap.mDead&&StarProjectiles.Children.Count==15,"third contact consumes cap");
var pausedCap=new Projectile{mBoard=cap.mBoard,mProjectileType=cap.mProjectileType,mCobTargetX=-556000,mRow=2,mPosX=400};
pausedCap.mBoard.mPaused=true;StarCaps.Collide(pausedCap);
Check(StarProjectiles.Children.Count==15,"pause blocks bursts");

// Source-based cases for the four support fusions. The user runs this suite.
Plant SupportMake(FusionPlant f) {var p=Make(f);FumeSupport.Initialize(p);return p;}
foreach(var (ingredient,result) in new[]{(SeedType.Plantern,FusionPlant.RayShroom),(SeedType.Kernelpult,FusionPlant.BlubberShroom),(SeedType.Sunflower,FusionPlant.SunriseShroom),(SeedType.Umbrella,FusionPlant.BushShroom)}) {
 Check(FusionRecipes.TryGet(SeedType.Fumeshroom,ingredient,out var f)&&f==result,"support recipe");
 Check(FusionRecipes.TryGet(ingredient,SeedType.Fumeshroom,out f)&&f==result,"reverse support recipe");
 var p=SupportMake(result);Check(FusionPlant.From(p)==result,"support saved identity");
}
Check(FumeSupport.RayDamage(0,0)==90&&FumeSupport.RayDamage(5,0)==10,"laser distance falloff clamps");
Check(FumeSupport.RayDamage(5,3)==65&&FumeSupport.RayDamage(10,3)==25,"level three doubles falloff distance");
var ray=SupportMake(FusionPlant.RayShroom);target=Enemy(ray,x:2000);Tick(ray,25);Check(target.Hits==0,"laser original emit time");Tick(ray,1);Check(target.Damage==10,"laser reaches distant row target");
var blubber=SupportMake(FusionPlant.BlubberShroom);target=Enemy(blubber,x:300);Tick(blubber,100);Check(blubber.mStartRow==0,"butter has no automatic targeting");
Check(FumeSupport.Trigger(blubber),"manual or damage trigger accepts nearby target");
Tick(blubber,25);Check(LobberProjectiles.Butters==0,"butter waits for original event");Tick(blubber,1);
Check(LobberProjectiles.Butters==1&&blubber.mPlantHealth==270&&blubber.mPlantMaxHealth==270,"one butter and thirty current/max HP per event");
var bush=SupportMake(FusionPlant.BushShroom);var damageHit=new ReactiveFumeDamage.Hit(bush,true);bush.mPlantHealth-=1200;damageHit.Apply();
Check(bush.mPlantHealth==880&&bush.mTargetX==600,"lethal raw damage reduces before native death");damageHit.Apply();Check(bush.mTargetX==600,"one energy credit per native writer");
var reused=new ReactiveFumeDamage.Hit(bush,false);bush.DataID++;bush.mPlantHealth-=100;reused.Apply();Check(bush.mTargetX==600,"pool reuse cannot credit energy");
target=Enemy(bush);FumeFusions.Hit(bush,FusionPlant.BushShroom);Check(target.Damage==640&&bush.mTargetX==0,"smoke consumes energy once");
var bank=SupportMake(FusionPlant.SunriseShroom);bank.mBoard.m_plants.Add(bank);bank.mTargetX=25;FumeSupport.Produce(bank);
Check(bank.mBoard.Coins.Count==1&&bank.mTargetX==25,"exactly 25 savings stay until above source threshold");
bank.mTargetX=65;FumeSupport.Produce(bank);Check(bank.mBoard.Coins.Count==4&&bank.mTargetX==15,"production releases base sun and whole stored batches");
var sun=new Coin{mBoard=bank.mBoard,mPosX=240,mPosY=240,Value=25};Check(FumeSupport.Sun(sun),"new nearby sun enters bank");
Check(sun.mDead&&bank.mTargetX==47,"25 sun yields truncated 32 savings");
var farSun=new Coin{mBoard=bank.mBoard,mPosX=800,mPosY=500};Check(!FumeSupport.Sun(farSun),"sun outside five by five keeps native behavior");
var paid=bank.mBoard.Coins[0];Check(!FumeSupport.Sun(paid),"bank payouts cannot compound themselves");
var flyingSun=new Coin{mBoard=bank.mBoard,mPosX=360,mPosY=200,Value=15};FumeSupport.Sun(flyingSun);
Check(flyingSun.mUsableSeedType==SeedType.Fumeshroom&&flyingSun.mDisappearCounter==15,"in-flight sun stores custom value and full destination");
var restoredSun=new Coin{mBoard=bank.mBoard,mUsableSeedType=flyingSun.mUsableSeedType,mTimesDropped=flyingSun.mTimesDropped,mDisappearCounter=flyingSun.mDisappearCounter,mPosX=240,mPosY=240};
FumeSupport.Sun(restoredSun);Check(restoredSun.mDead&&bank.mTargetX==66,"loaded sun retains value and credits once");
bank.mLaunchCounter=1000;FumeSupport.Fertilize(bank);Check(bank.mLaunchCounter==50&&bank.mTargetX==66,"fertilizer accelerates production without clearing savings");
var pausedBank=SupportMake(FusionPlant.SunriseShroom);pausedBank.mBoard.mPaused=true;int savedTimer=pausedBank.mLaunchCounter;FumeSupport.TickSunrise(pausedBank);Check(pausedBank.mLaunchCounter==savedTimer,"paused production stays paused");

// The existing native damage observer also owns ultimate cactus immunity.
var protectedCactus = SupportMake(FusionPlant.DoomCactus);
protectedCactus.mPlantHealth = protectedCactus.mPlantMaxHealth = 300;
protectedCactus.mDoSpecialCountdown = 184;
var immuneHit = ReactiveFumeDamage.Capture(protectedCactus, true)!;
protectedCactus.mPlantHealth = -1000;
immuneHit.Apply();
Check(protectedCactus.mPlantHealth == 300, "ultimate restores lethal native bite before death");
protectedCactus.mDoSpecialCountdown = 0;
Check(ReactiveFumeDamage.Capture(protectedCactus, true) == null, "ordinary cactus has no damage immunity");

Check(PerfumeShroom.Chance(50, 100) == 0 && PerfumeShroom.Chance(100, 100) == 0,
    "Perfume cannot hypnotize targets at or above half health");
Check(Math.Abs(PerfumeShroom.Chance(28, 100) - .25) < 1e-12,
    "Perfume chance caps at 25 percent");
var perfume = Make(FusionPlant.PerfumeShroom);
var perfumeTarget = Enemy(perfume); perfumeTarget.mBodyHealth = 100; perfumeTarget.mBodyMaxHealth = 100;
FumeFusions.Hit(perfume, FusionPlant.PerfumeShroom);
Check(perfumeTarget.Damage == 40 && !perfumeTarget.mMindControlled, "Healthy targets receive ordinary perfume damage");
perfumeTarget.mBodyHealth = 20; perfume.mBoard.RandomValue = 0;
FumeFusions.Hit(perfume, FusionPlant.PerfumeShroom);
Check(perfumeTarget.mMindControlled && perfumeTarget.Damage == 40, "Successful hypnosis replaces damage");
var bittenPerfume = Make(FusionPlant.PerfumeShroom); var perfumeBiter = Enemy(bittenPerfume);
bittenPerfume.mBoard.SpawnFails = true;
Check(PerfumeShroom.Bite(bittenPerfume, perfumeBiter) && !bittenPerfume.mDead && !perfumeBiter.mMindControlled,
    "Failed replacement retains plant and enemy without consuming either");
bittenPerfume.mBoard.SpawnFails = false;
Check(PerfumeShroom.Bite(bittenPerfume, perfumeBiter) && bittenPerfume.mDead && perfumeBiter.mMindControlled
    && bittenPerfume.mBoard.m_plants.Any(p => p.mSeedType == SeedType.Fumeshroom && p.mImitaterType == SeedType.None),
    "Bite hypnosis leaves one ordinary Fume-shroom");

foreach (var identity in new[] { FusionPlant.UmbrellaShell, FusionPlant.UmbrellaHusk, FusionPlant.Torchflower,
    FusionPlant.FrostShroom, FusionPlant.SquashNut, FusionPlant.PyroPumpkin, FusionPlant.StenchSpreader })
{
    var custom = Make(identity); custom.mShootingCounter = 84;
    Check(!CustomShootingCounterPatch.Prefix(custom) && custom.mShootingCounter == 84,
        "Native shooting cannot consume custom action or production counters");
}
foreach (var identity in new[] { FusionPlant.UmbrellaShell, FusionPlant.UmbrellaHusk, FusionPlant.Torchflower })
    Check(!CustomNonShooterFirePatch.Prefix(Make(identity)), "Non-shooting carriers never request native projectiles");
Check(CustomShootingCounterPatch.Prefix(Make(SeedType.Peashooter))
    && CustomShootingCounterPatch.Prefix(Make(FusionPlant.PoisonPea)), "Ordinary and native-backed pea clocks remain native");

foreach (var (left,right,result) in new[] {
    (SeedType.Fumeshroom,SeedType.Chomper,FusionPlant.BelchShroom),
    (SeedType.Scaredyshroom,SeedType.Chomper,FusionPlant.HungryShroom),
    (SeedType.Fumeshroom,SeedType.Cherrybomb,FusionPlant.TriggerShroom),
    (SeedType.Iceshroom,SeedType.Jalapeno,FusionPlant.Spicicle) })
{
    Check(FusionRecipes.TryGet(left,right,out var result1)&&result1==result,"new recipe");
    Check(FusionRecipes.TryGet(right,left,out result1)&&result1==result,"new reverse recipe");
    Check(FusionPlant.From(Make(result))==result&&FusionPlant.HasImportedVisuals(result),"new saved identity and model");
}
var belch=Make(FusionPlant.BelchShroom); var belchClose=Enemy(belch,x:300);
EatingShrooms.TickBelch(belch);Check(belch.mTargetY==75&&!belchClose.mDead,"chomp starts without early swallow");
for(int i=0;i<55;++i)EatingShrooms.TickBelch(belch);
Check(!belchClose.mDead,"bite waits for 0.55-second original event");
EatingShrooms.TickBelch(belch);Check(belchClose.mDead&&belch.mTargetX==4000,"bite commits digestion and swallows");
var belchFar=Enemy(belch,x:500); FumeFusions.Hit(belch,FusionPlant.BelchShroom);
Check(belchFar.Damage==40&&belchFar.mPosX>400,"digesting belch doubles damage and pushes");
EatingShrooms.Fertilize(belch);Check(belch.mTargetX==50,"fertilizer caps digestion");
belch.mTargetX=0; FumeFusions.Hit(belch,FusionPlant.BelchShroom);Check(belchFar.Damage==60,"ordinary belch damage is twenty");
var hungry=Make(FusionPlant.HungryShroom);
for(int i=0;i<4;++i)Enemy(hungry,x:280+i);
EatingShrooms.Fear(hungry);Check(hungry.mBoard.m_zombies.Count(z=>z.mDead)==3&&hungry.mTargetX==4000,"fear swallows at most three");
EatingShrooms.Fear(hungry);Check(hungry.mBoard.m_zombies.Count(z=>z.mDead)==3,"cooldown prevents second swallow");
EatingShrooms.ShootHungry(hungry);Check(RollingProjectiles.HungryShots==1,"digesting hungry shoots chunks");
EatingShrooms.Fertilize(hungry);Check(hungry.mTargetX==50,"hungry fertilizer cap");
var emptyHungry=Make(FusionPlant.HungryShroom);EatingShrooms.Fear(emptyHungry);
Check(emptyHungry.mTargetX==4000,"empty fear still charges cooldown");
var trigger=Make(FusionPlant.TriggerShroom);
for(int i=0;i<6;++i)trigger.mBoard.AddProjectile(260+i,240,0,2,ProjectileType.Pea).mCobTargetX=-41001;
Check(TriggerShroom.HasCherry(trigger),"cherries trigger targetless attack");
TriggerShroom.Convert(trigger);Check(trigger.mBoard.Projectiles.Count(x=>x.mCobTargetX==-41011)==4,"one emission converts at most four");
trigger.mBoard.Projectiles.ForEach(x=>x.mRow=1);Check(!TriggerShroom.HasCherry(trigger),"conversion stays in row");
var guard=Make(FusionPlant.Spicicle);Spicicle.Initialize(guard);guard.mBoard.m_plants.Add(guard);
var protectedPlant=guard.mBoard.AddPlant(4,2,SeedType.Peashooter,SeedType.None)!;protectedPlant.mPlantHealth=protectedPlant.mPlantMaxHealth=300;
Check(guard.mPlantHealth==32000&&Spicicle.Guard(protectedPlant)==guard,"global guard owns damage");
var guardedHit=ReactiveFumeDamage.Capture(protectedPlant,true)!;protectedPlant.mPlantHealth=-100;
guardedHit.Apply();Check(protectedPlant.mPlantHealth==300&&guard.mPlantHealth==31600,"lethal damage restores target before charging guard");
guardedHit.Apply();Check(guard.mPlantHealth==31600,"same damage snapshot transfers once");
Spicicle.Tick(guard);Check(guard.mPlantHealth==31600,"decay waits full second");
for(int i=1;i<100;++i)Spicicle.Tick(guard);Check(guard.mPlantHealth==25200,"decay is twenty percent of max each second");
Spicicle.Fertilize(guard);Check(guard.mTargetX==50,"guard fertilizer caps next decay");
guard.mBoard.mPaused=true;Spicicle.Tick(guard);Check(guard.mTargetX==50,"pause stops guard clock");
guard.mBoard.mPaused=false;guard.mIsAsleep=true;Check(Spicicle.Guard(protectedPlant)==null,"sleep disables global guard");
guard.mIsAsleep=false;Check(Spicicle.Guard(guard)==null,"guard cannot redirect its own damage");
protectedPlant.mIsAsleep=true;
var sleepingHit=ReactiveFumeDamage.Capture(protectedPlant,true);
Check(sleepingHit!=null,"global guard protects sleeping recipients");
protectedPlant.mPlantHealth-=50;sleepingHit!.Apply();
Check(protectedPlant.mPlantHealth==300,"sleeping recipient retains health");
protectedPlant.mIsAsleep=false;
var immuneGuarded=Make(FusionPlant.DoomCactus);immuneGuarded.mBoard=guard.mBoard;
immuneGuarded.mDoSpecialCountdown=184;immuneGuarded.mPlantHealth=300;
var guardBeforeImmune=guard.mPlantHealth;var immuneGuardHit=ReactiveFumeDamage.Capture(immuneGuarded,true)!;
immuneGuarded.mPlantHealth-=100;immuneGuardHit.Apply();
Check(immuneGuarded.mPlantHealth==300&&guard.mPlantHealth==guardBeforeImmune,"existing immunity does not charge global guard");


var relayOwner = Make(FusionPlant.FrostShroom);
var relayShot = relayOwner.mBoard.AddProjectile(300, 300, 1, 2, ProjectileType.Cabbage);
var relayRecord = FumeProjectiles.Record(relayShot, true)!;
relayRecord.mSeedType = SeedType.Umbrella;
Check(FumeProjectiles.Damage(relayShot, 40) == 40, "held relay has no early amplification");
relayRecord.mSeedType = SeedType.Cobcannon;
Check(FumeProjectiles.Damage(relayShot, 40) == 70, "relay damage factor is seven quarters");
relayRecord.mGridItemCounter = 5;
Check(FumeProjectiles.Damage(relayShot, 40) == 78, "relay amplifies the existing saved damage bonus");
var relayVictim = Enemy(relayOwner);
FumeProjectiles.CurrentHit = new(relayShot, relayVictim, FumeProjectiles.Damage(relayShot, 0), false);
int relayDamage = 40;
FumeProjectiles.ApplyBonus(relayVictim, ref relayDamage);
Check(relayDamage == 78, "native impact applies relay amplification at damage writer");
FumeProjectiles.ApplyBonus(relayVictim, ref relayDamage);
Check(relayDamage == 78, "same target snapshot cannot apply amplification twice");
FumeProjectiles.CurrentHit = null;
++relayShot.DataID;
Check(FumeProjectiles.Damage(relayShot, 40) == 40, "reused projectile cannot inherit an old relay record");

Console.WriteLine($"PASS: {checks} fume/gloom/gutsy checks; no game code loaded.");
Console.WriteLine($"Fume variant contracts prepared: {checks} checks.");

// Football-shroom independently damages each layer; armor overflow never adds
// a second body hit. These are isolated behavior fixtures, not native damage proof.
Check(FusionRecipes.TryGet(FusionPlant.RustShroom,FusionPlant.FootballHelmet,out var footballResult)
    && footballResult==FusionPlant.FootballShroom,"directed Football recipe");
Check(!FusionRecipes.TryGet(FusionPlant.FootballHelmet,FusionPlant.RustShroom,out _),"Football reverse unavailable");
Check(FusionRecipes.TryGet(FusionPlant.DormantShroom,FusionPlant.Bucket,out var doomplateResult)
    && doomplateResult==FusionPlant.DoomplateShroom,"directed Doomplate recipe");
Check(!FusionRecipes.TryGet(FusionPlant.Bucket,FusionPlant.DormantShroom,out _),"Doomplate reverse unavailable");
foreach(var armor in new[]{0,50,100,300}) {
 var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);z.mHelmHealth=z.mShieldHealth=armor;
 Check(p.mPlantMaxHealth==3000&&p.mLaunchRate==150,"Football original health/cadence");
 Tick(p,35);Check(z.Hits==0&&z.HelmHits==0&&z.ShieldHits==0,"Football no early hit");
 Tick(p,1);Check(z.Damage==100&&z.Hits==1,"Football body100 once");
 Check(z.mHelmHealth==Math.Max(0,armor-100)&&z.HelmHits==(armor>0?1:0),"Football helmet100 independently");
 Check(z.mShieldHealth==Math.Max(0,armor-100)&&z.ShieldHits==(armor>0?1:0),"Football shield100 independently");
 Check((z.LastLayerFlags&DamageFlags.Freeze)==0,"Football never maps source6 to native Freeze");
 Tick(p,150);Check(z.Hits==2&&z.Damage==200,"Football source150 cadence");
}
foreach(var stage in new[]{1,2,3}) foreach(var mutation in new[]{"zombie generation","zombie board","plant generation","plant board","plant dead"}) {
 var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);z.mShieldHealth=z.mHelmHealth=500;int calls=0;
 z.OnDamage=()=>{if(++calls!=stage)return;switch(mutation){case "zombie generation":++z.DataID;break;case "zombie board":z.mBoard=new Board{Pointer=(IntPtr)2};break;case "plant generation":++p.DataID;break;case "plant board":p.mBoard=new Board{Pointer=(IntPtr)2};break;default:p.mDead=true;break;}};
 FootballFumeDamage.Hit(p,z,z.DataID);
 Check(calls==stage,"Football stops after native callback: "+stage+" / "+mutation);
 Check(z.ShieldHits==1&&z.HelmHits==(stage>=2?1:0)&&z.Hits==(stage>=3?1:0),"Football callback layer order");
}
Console.WriteLine($"Football-shroom included; total {checks} checks");

foreach(var divisor in new[]{1,2}) {
 var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);z.mHelmHealth=z.mShieldHealth=500;z.DamageDivisor=divisor;
 FootballFumeDamage.Hit(p,z,z.DataID);
 Check(z.EntryHits==1&&z.Damage==100/divisor&&z.mHelmHealth==500-100/divisor&&z.mShieldHealth==500-100/divisor,"outer damage entry/scaling once");
}
{
 var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);z.mHelmHealth=z.mShieldHealth=500;z.Immune=true;FootballFumeDamage.Hit(p,z,z.DataID);
 Check(z.Hits+z.HelmHits+z.ShieldHits==0&&FootballFumeDamage.Current==null,"outer immunity gate retained");
 z.Immune=false;z.DamageThrows=true;try{FootballFumeDamage.Hit(p,z,z.DataID);throw new Exception("exception not propagated");}catch(InvalidOperationException){}
 Check(FootballFumeDamage.Current==null,"exception clears scope");
 z.DamageThrows=false;z.CancelHelmet=true;FootballFumeDamage.Hit(p,z,z.DataID);
 Check(z.Hits==0&&z.HelmHits==0,"canceled helmet original does not invent body hit");
}
{
 var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);z.mHelmHealth=z.mShieldHealth=500;
 bool nested=false;z.OnEntry=()=>{if(nested)return;nested=true;z.TakeDamage(30,DamageFlags.BypassesShield);};
 FootballFumeDamage.Hit(p,z,z.DataID);
 Check(z.EntryHits==2&&z.HelmHits==2&&z.mHelmHealth==370&&z.mShieldHealth==400&&z.Damage==100,"nested same-target hit does not inherit Football helmet override");
 Check(FootballFumeDamage.Current==null,"nested entry restores scope");
}
Console.WriteLine($"Football scoped native-entry fixtures complete; {checks} total checks");

{
 // These fixtures reproduce the Football side of the integration contract.
 // They do not compile or simulate the whole Zombie Library native pipeline.
 var failures=new List<string>();
 foreach(var divisor in new[]{1,2}) foreach(var hat in new[]{30,200}) {
  var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);z.mHelmHealth=500;z.AdditionalHatHealth=hat;z.DamageDivisor=divisor;
  FootballFumeDamage.Hit(p,z,z.DataID);int expected=100/divisor;
  if(z.Damage!=expected||z.EntryHits!=1)
   failures.Add($"extra hat{hat}, outer divisor{divisor}: body={z.Damage}, expected={expected}, entries={z.EntryHits}");
 }
 if(failures.Count>0)throw new Exception("Football additional-hat integration reproducer: "+string.Join("; ",failures));
 Check(true,"Football additional-hat independent body damage");
}

{
 var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);z.mHelmHealth=500;z.AdditionalHatHealth=200;
 int calls=ZombieLibrary.Zombies.Calls;bool nested=false;
 z.OnEntry=()=>{if(nested)return;nested=true;z.TakeDamage(30,DamageFlags.BypassesShield);};
 FootballFumeDamage.Hit(p,z,z.DataID);
 Check(z.EntryHits==2&&z.AdditionalHatHealth==70&&z.mHelmHealth==400&&z.Damage==100,"ordinary nested hit keeps hat absorption; outer hits native helmet/body independently");
 Check(ZombieLibrary.Zombies.Calls==calls+1,"Football opens exactly one public armor API scope");
}
Console.WriteLine($"Football added-armor API isolation complete: {checks} checks");

{
 var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);z.mHelmHealth=z.mShieldHealth=1000;
 Tick(p,20);int clip=p.mStartRow,footballCooldown=p.mLaunchCounter;
 var footballLoaded=Make(FusionPlant.FootballShroom);footballLoaded.mStartRow=clip;footballLoaded.mLaunchCounter=footballCooldown;
 var footballRestored=Enemy(footballLoaded);footballRestored.mHelmHealth=footballRestored.mShieldHealth=1000;
 Tick(p,16);Tick(footballLoaded,16);
 Check(z.Damage==100&&footballRestored.Damage==100&&p.mStartRow==footballLoaded.mStartRow&&p.mLaunchCounter==footballLoaded.mLaunchCounter,"Football footballRestored clip emits once at original35 phase");
 Tick(p,150);Tick(footballLoaded,150);
 Check(z.Damage==200&&footballRestored.Damage==200,"Football footballRestored footballCooldown keeps source150 cadence");
}
foreach(var state in new[]{"pause","sleep","carry","dead","squished","bungee"}) {
 var p=Make(FusionPlant.FootballShroom);var z=Enemy(p);Tick(p,20);int clip=p.mStartRow,footballCooldown=p.mLaunchCounter;
 if(state=="pause")p.mBoard.mPaused=true;if(state=="sleep")p.mIsAsleep=true;if(state=="carry")p.mIsOnBoard=false;
 if(state=="dead")p.mDead=true;if(state=="squished")p.mSquished=true;if(state=="bungee")p.Bungee=true;
 Tick(p,40);Check(z.Damage==0&&clip==p.mStartRow&&footballCooldown==p.mLaunchCounter,"Football inactive clip/footballCooldown: "+state);
}
Console.WriteLine($"Football final owner-isolation suite: {checks} checks");
