using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool ok, string message) { ++checks; if (!ok) throw new Exception(message); }
uint next = 10;
Plant Nut(FusionPlant identity, Board? board = null) {
    var p = new Plant { mSeedType=identity.NativeType, mImitaterType=identity.Marker,
        mBoard=board ?? new Board(), DataID=next++, Pointer=(IntPtr)next };
    ElementalNuts.Initialize(p); p.mBoard.m_plants.Add(p); return p;
}
Zombie Enemy(Board board, int health = 10000, float x = 240, int row = 2) {
    var z = new Zombie { mBoard=board, DataID=next++, Pointer=(IntPtr)next,
        Health=health, Rect=new(x-10, 220, x+10, 260), mRow=row };
    board.m_zombies.Add(z); return z;
}
var recipes = new (FusionPlant A, FusionPlant B, FusionPlant R)[] {
    (SeedType.Jalapeno, SeedType.Wallnut, FusionPlant.FlameWallNut),
    (SeedType.Iceshroom, SeedType.Wallnut, FusionPlant.FrostWallNut),
    (FusionPlant.FlameWallNut, SeedType.Tallnut, FusionPlant.FlameTallNut),
    (SeedType.Tallnut, SeedType.Jalapeno, FusionPlant.FlameTallNut),
    (FusionPlant.FrostWallNut, SeedType.Tallnut, FusionPlant.FrostTallNut),
    (SeedType.Tallnut, SeedType.Iceshroom, FusionPlant.FrostTallNut)
};
foreach (var (first,second,r) in recipes) {
    Check(FusionRecipes.TryGet(first,second,out var result) && result==r, "recipe");
    Check(FusionRecipes.TryGet(second,first,out result) && result==r, "reverse recipe");
    var p=Nut(r);
    Check(FusionPlant.From(p)==r && FusionPlant.HasImportedVisuals(r), "saved identity and original assets");
    Check(p.mPlantHealth==(r.NativeType==SeedType.Tallnut?8000:4000), "source HP");
    Check(p.mPlantMaxHealth==p.mPlantHealth, "max HP");
}
foreach(var r in new[]{FusionPlant.FlameWallNut,FusionPlant.FlameTallNut}) {
    Overheat.Clear(); var p=Nut(r);var z=Enemy(p.mBoard);
    for(int i=0;i<13;i++)z.EatPlant(p);
    Check(p.mPlantHealth==p.mPlantMaxHealth-52 && z.Damage==20 && Overheat.Has(z), "actual bite retaliates");
    for(int i=0;i<12;i++)z.EatPlant(p);Check(z.Damage==50, "later retaliation amplified");
    z.SkipBite=true;z.EatPlant(p);Check(z.Damage==50, "native skipped bite has no retaliation");
    z.SkipBite=false;p.mPlantHealth=4;z.EatPlant(p);
    Check(p.mDead && z.Damage==50, "partial lethal bite does not invent a source bite");
}
foreach(var r in new[]{FusionPlant.FrostWallNut,FusionPlant.FrostTallNut}) {
    var p=Nut(r);var z=Enemy(p.mBoard);Overheat.Apply(z);
    for(int i=0;i<112;i++)z.EatPlant(p);
    Check(z.mChilledCounter==1000 && z.mIceTrapCounter==0 && z.Damage==0 && !Overheat.Has(z), "cold clears heat, nine bites no freeze");
    for(int i=0;i<13;i++)z.EatPlant(p);Check(z.mIceTrapCounter==400 && z.Damage==0, "tenth bite freezes without trap damage");
    z.EatPlant(p);z.RemoveIceTrap();
    for(int i=0;i<248;i++)z.EatPlant(p);
    Check(z.mIceTrapCounter==0, "next threshold 200");
    z.EatPlant(p);Check(z.mIceTrapCounter==400, "twentieth bite freezes");
    var immune=Enemy(p.mBoard);immune.ImmuneToFreeze=true;immune.EatPlant(p);
    Check(immune.mIceTrapCounter==0 && immune.mChilledCounter==0, "native cold immunity");
}
foreach(var mode in new[]{"pause","dead","carried","squished","bungee","loading","garden","dispose","ally","foreign"}) {
    var p=Nut(FusionPlant.FlameWallNut);var z=Enemy(p.mBoard);
    if(mode=="pause")p.mBoard.mPaused=true;
    if(mode=="dead")p.mDead=true;
    if(mode=="carried")p.OffGround=true;
    if(mode=="squished")p.mSquished=true;
    if(mode=="bungee")p.mOnBungeeState=PlantOnBungeeState.OnBungee;
    if(mode=="loading")p.mApp.GameScene=GameScenes.Loading;
    if(mode=="garden")p.mApp.GameMode=GameMode.ChallengeZenGarden;
    if(mode=="dispose")CherryBombPlants.DisposingBoard=p.mBoard.Pointer;
    if(mode=="ally")z.mMindControlled=true;
    if(mode=="foreign")z.mBoard=new Board{Pointer=(IntPtr)9};
    Check(ElementalNuts.BeforeBite(z,p)==null, "inactive "+mode);
    CherryBombPlants.DisposingBoard=IntPtr.Zero;
}
var poolNut=Nut(FusionPlant.FlameWallNut);var poolZombie=Enemy(poolNut.mBoard);
var bite=ElementalNuts.BeforeBite(poolZombie,poolNut);poolNut.mPlantHealth-=4;poolZombie.DataID++;
ElementalNuts.AfterBite(poolZombie,poolNut,bite);Check(poolZombie.Damage==0,"pooled zombie rejects stale bite");
bite=ElementalNuts.BeforeBite(poolZombie,poolNut);poolNut.mPlantHealth-=4;poolNut.DataID++;
ElementalNuts.AfterBite(poolZombie,poolNut,bite);Check(poolZombie.Damage==0,"pooled plant rejects stale bite");
var plain=new Plant{mSeedType=SeedType.Wallnut,mPlantHealth=123};ElementalNuts.Initialize(plain);
Check(plain.mPlantHealth==123 && ElementalNuts.BeforeBite(poolZombie,plain)==null,"ordinary nut unchanged");

var board=new Board();var fire=Nut(FusionPlant.FlameTallNut,board);var ice=Nut(FusionPlant.FrostTallNut,board);
var shortFire=Nut(FusionPlant.FlameWallNut,board);var otherRow=Nut(FusionPlant.FlameTallNut,board);otherRow.mRow=3;
foreach(var p in board.m_plants)p.mPlantHealth=100;
ElementalNuts.Heal(board,true,2);
Check(fire.mPlantHealth==3100 && ice.mPlantHealth==100 && shortFire.mPlantHealth==100 && otherRow.mPlantHealth==100,"fire heals only tall variant in row");
ElementalNuts.Heal(board,false);Check(ice.mPlantHealth==1100 && fire.mPlantHealth==3100,"ice heal");
fire.mPlantHealth=7900;ElementalNuts.Heal(board,true,2);Check(fire.mPlantHealth==8000,"heal caps HP");
ice.mDead=true;ElementalNuts.Heal(board,false);Check(ice.mPlantHealth==1100,"no dead revival");ice.mDead=false;
var shroom=new Plant{mBoard=board,mSeedType=SeedType.Iceshroom};
FrostTallNutSpecialHealPatch.Prefix(shroom,true,out var frozenBoard);shroom.mDead=true;
FrostTallNutSpecialHealPatch.Postfix(frozenBoard);Check(ice.mPlantHealth==2100,"inlined ice explosion heals after source death");
FrostTallNutSpecialHealPatch.Prefix(fire,true,out frozenBoard);Check(frozenBoard==null,"non-ice special no heal");
Check(ElementalNuts.DamageStage(2667,4000)==0 && ElementalNuts.DamageStage(2665,4000)==1
    && ElementalNuts.DamageStage(1332,4000)==2,"original damage bodies");

Overheat.Clear();board=new Board();var a=Enemy(board);var b=Enemy(board,10000,310);
var far=Enemy(board,10000,500);var ally=Enemy(board,10000,250);ally.mMindControlled=true;
var air=Enemy(board,10000,260);air.Category=DamageRangeFlags.Flying;
Overheat.Apply(a);a.TakeDamage(20,0);Check(a.Damage==30,"50 percent incoming damage");
Check(Overheat.Damage(a,int.MaxValue)==int.MaxValue && Overheat.Damage(a,-1)==-1,"damage bounds");
a.DieWithLoot();
Check(b.Damage==100 && Overheat.Has(b) && air.Damage==100 && Overheat.Has(air),"death burst hits ground and air, then heats");
Check(far.Damage==0 && ally.Damage==0,"radius and friendly exclusions");
Check(board.mApp.Particles==1 && !Overheat.Has(a),"nested death explodes once");
b.ApplyChill(false);Check(!Overheat.Has(b),"cold cures heat");
Overheat.Apply(b);Check(b.mChilledCounter==0 && b.mIceTrapCounter==0,"heat thaws cold");
b.ApplyChill(false);b.TakeDamage(20,0);Check(b.Damage==120,"cured damage normal");
var boss=Enemy(board);boss.mZombieType=ZombieType.Boss;Overheat.Apply(boss);Check(!Overheat.Has(boss),"boss status exclusion");

Overheat.Clear();board=new Board();a=Enemy(board);b=Enemy(board,120,300);var c=Enemy(board,10000,360);
Overheat.Apply(a);Overheat.Apply(b);a.DieNoLoot();
Check(b.mDead && b.Damage==150 && c.Damage==100 && Overheat.Has(c),"existing heat chains through lethal victim");
Check(board.mApp.Particles==2,"one burst per heated death");
Overheat.Clear();board=new Board();a=Enemy(board);b=Enemy(board,80,300);Overheat.Apply(a);a.DieNoLoot();
Check(b.mDead && board.mApp.Particles==1,"newly killed unheated victim does not acquire heat retroactively");

var pooled=Enemy(new Board());Overheat.Apply(pooled);pooled.DataID++;
Check(!Overheat.Has(pooled) && Overheat.Damage(pooled,20)==20,"pool identity");
Overheat.Apply(pooled);pooled.mBoard=new Board{Pointer=(IntPtr)13};Check(!Overheat.Has(pooled),"board identity");
Overheat.Apply(pooled);pooled.Reset();Check(!Overheat.Has(pooled),"reset cleanup");
Overheat.Apply(pooled);Overheat.ForgetBoard(pooled.mBoard);
Check(Overheat.Has(pooled),"transient board cleanup preserves native saved heat items");
pooled.mBoard.DisposeBoard();Check(!Overheat.Has(pooled),"native board disposal releases saved heat items");
Overheat.Apply(pooled);CherryBombPlants.DisposingBoard=pooled.mBoard.Pointer;
Check(Overheat.BeginDeath(pooled)==null,"teardown no explosion");CherryBombPlants.DisposingBoard=IntPtr.Zero;
Overheat.Clear();
var failed=Enemy(new Board());Overheat.Apply(failed);
OverheatDeathPatch.Prefix(failed,out var failedDeath);
Check(!Overheat.Has(failed),"death capture prevents nested duplicate");
OverheatDeathPatch.Finalizer(failed,failedDeath);
Check(Overheat.Has(failed),"failed native death restores live status");
OverheatDeathPatch.Prefix(failed,out failedDeath);failed.DataID++;
OverheatDeathPatch.Finalizer(failed,failedDeath);
Check(!Overheat.Has(failed),"failed death never restores pooled identity");
Overheat.Clear();
var thorn = Nut(FusionPlant.ThornNut); var thornBiter = Enemy(thorn.mBoard);
Check(FusionRecipes.TryGet(SeedType.Cactus, SeedType.Wallnut, out var thornRecipe)
    && thornRecipe == FusionPlant.ThornNut && FusionRecipes.TryGet(SeedType.Wallnut, SeedType.Cactus, out thornRecipe), "both thorn recipes");
thornBiter.EatPlant(thorn);
Check(thornBiter.Damage == 4 && thorn.mPlantMaxHealth == 4000, "thorn source HP and retaliation follows actual four-HP native bite");
thornBiter.SkipBite = true; thornBiter.EatPlant(thorn);
Check(thornBiter.Damage == 4, "skipped thorn bite has no retaliation");
thornBiter.SkipBite = false; thorn.mPlantHealth = 4; thornBiter.EatPlant(thorn);
Check(thorn.mDead && thornBiter.Damage == 8, "lethal thorn bite retaliates actual HP loss");
thorn = Nut(FusionPlant.ThornNut);
var fullBiter = Enemy(thorn.mBoard); fullBiter.BiteDamage = 50; fullBiter.EatPlant(thorn);
Check(fullBiter.Damage == 50, "thorn also reflects a full fifty-HP native bite without a fixed multiplier");
thorn = Nut(FusionPlant.ThornNut); var balloon = Enemy(thorn.mBoard, x: 330);
balloon.mZombieType = ZombieType.Balloon;
ElementalNuts.UpdateThorn(thorn);
Check(thorn.mLaunchCounter == 1 && thorn.mShootingCounter == 58 && balloon.Flying, "balloon approach starts original rise");
balloon.Rect = new(230, 220, 250, 260); ElementalNuts.UpdateThorn(thorn);
Check(!balloon.Flying, "contact pops balloon using native landing");
ElementalNuts.UpdateThorn(thorn);
Check(thorn.mLaunchCounter == 0 && thorn.mShootingCounter == 50, "balloon gone starts original lowering");
balloon.Flying = true; thorn.mBoard.mPaused = true; ElementalNuts.UpdateThorn(thorn);
Check(balloon.Flying && thorn.mShootingCounter == 50, "pause stops thorn ability and animation timer");
thorn.mBoard.mPaused = false; balloon.mMindControlled = true; ElementalNuts.UpdateThorn(thorn);
Check(balloon.Flying, "friendly balloon preserved");
balloon.mMindControlled = false; balloon.mRow++; ElementalNuts.UpdateThorn(thorn);
Check(balloon.Flying, "other row balloon preserved");
// Butter-nut reacts to actual HP loss, including the final bite.
foreach (bool lethal in new[] { false, true }) {
    var nut = Nut(FusionPlant.ButterNut); var eater = Enemy(nut.mBoard);
    for (int biteIndex = 0; biteIndex < 12; ++biteIndex) eater.EatPlant(nut);
    Check(eater.mButteredCounter == 0, "Butter-nut waits for fifty accumulated HP loss");
    if (lethal) nut.mPlantHealth = 4;
    eater.EatPlant(nut);
    Check(eater.mButteredCounter == 200, "Butter-nut two-second source bite after thirteen native bites, lethal=" + lethal);
    Check(nut.mPlantMaxHealth == 4000, "Butter-nut source health");
}
var skippedNut = Nut(FusionPlant.ButterNut); var skippedEater = Enemy(skippedNut.mBoard);
skippedEater.SkipBite = true; skippedEater.EatPlant(skippedNut);
Check(skippedEater.mButteredCounter == 0, "skipped bite cannot apply butter");
var butterNut = Nut(FusionPlant.ButterNut); var butterEater = Enemy(butterNut.mBoard);
butterEater.BiteDamage = 50;
butterEater.mButteredCounter = 350; butterEater.EatPlant(butterNut);
Check(butterEater.mButteredCounter == 350, "existing longer butter duration survives");
butterEater.mButteredCounter = 0; butterEater.ButterImmune = true; butterEater.EatPlant(butterNut);
Check(butterEater.mButteredCounter == 0, "native butter immunity survives");
Check(FusionRecipes.TryGet(SeedType.Kernelpult, SeedType.Wallnut, out var butterRecipe)
    && butterRecipe == FusionPlant.ButterNut
    && FusionRecipes.TryGet(SeedType.Wallnut, SeedType.Kernelpult, out butterRecipe)
    && butterRecipe == FusionPlant.ButterNut, "both Butter-nut recipes");
// Exercise the actual Snowpea impact and health writer with the Icicle marker.
foreach (var (marker, frozen, immune, expected) in new[] {
    (-470018, false, false, 20), (-470018, true, false, 80),
    (-470018, false, true, 80), (0, false, true, 20) })
{
    var iceBoard = new Board(); var target = Enemy(iceBoard);
    target.ImmuneToChill = immune; target.ImmuneToFreeze = immune;
    if (frozen) target.mIceTrapCounter = 400;
    var shot = new Projectile { mProjectileType = ProjectileType.Snowpea, mCobTargetX = marker };
    object?[] impactArgs = [shot, target, null];
    typeof(SnowPeaImpactPatch).GetMethod("Prefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!.Invoke(null, impactArgs);
    try { target.TakeDamage(20, 0); }
    finally {
        typeof(SnowPeaImpactPatch).GetMethod("Finalizer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!.Invoke(null, [impactArgs[2]]);
    }
    Check(target.Damage == expected, "Icicle frozen/immune multiplier remains variant-specific");
    Check(SnowPea.CurrentImpact == null, "Impact context restored after hit");
}
var cryoBoard = new Board();
var cryoSaved = Enemy(cryoBoard);
SnowPea.Restore(cryoSaved, 70, 300);
Check(SnowPea.Capture(cryoSaved) == (70, 300), "saved cryo retains points and increasing threshold");
SnowPea.Restore(cryoSaved, 0, 100);
Check(SnowPea.Capture(cryoSaved) == (70, 300), "loaded backing never replaces a live cryo state");
SnowPea.Forget(cryoSaved);
SnowPea.Restore(cryoSaved, -20, 9000);
Check(SnowPea.Capture(cryoSaved) == (0, 1000), "cryo restore validates serialized bounds");
Console.WriteLine($"Elemental nuts: {checks} offline assertions passed.");
