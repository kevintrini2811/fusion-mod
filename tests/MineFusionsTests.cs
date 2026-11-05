using FusionMod;
using Il2CppReloaded.Gameplay;

int assertions = 0;
void Check(bool condition, string message)
{ ++assertions; if (!condition) throw new Exception(message); }
Plant Mine(FusionPlant identity)
{
    var plant = new Plant { mSeedType=identity.NativeType, mImitaterType=identity.Marker };
    MineFusions.Initialize(plant);
    return plant;
}
Zombie Target(Plant plant, float x=100, int row=0, float y=0)
{
    var zombie = new Zombie { mBoard=plant.mBoard, mPosX=x, mPosY=y, mRow=row,
        DataID=(uint)(plant.mBoard.m_zombies.Count+1), mBodyHealth=10000 };
    plant.mBoard.m_zombies.Add(zombie);
    return zombie;
}
CherryBombPlants.Death Death(Plant plant) => new(plant.mBoard, FusionPlant.From(plant), plant.DataID,
    0, plant.mRow, plant.mX+40, plant.mY+40, 0);

foreach (var (a,b,result) in new[] {
    ((FusionPlant)SeedType.Peashooter,(FusionPlant)SeedType.Potatomine,FusionPlant.PeatatoMine),
    ((FusionPlant)SeedType.Sunflower,(FusionPlant)SeedType.Potatomine,FusionPlant.SolarMine),
    ((FusionPlant)SeedType.Wallnut,(FusionPlant)SeedType.Potatomine,FusionPlant.MineNut) })
{
    Check(FusionRecipes.TryGet(a,b,out var one) && one==result,"forward recipe");
    Check(FusionRecipes.TryGet(b,a,out var two) && two==result,"reverse recipe");
    var p=Mine(result);
    Check(FusionPlant.From(p)==result && p.mSeedType==SeedType.Potatomine,"saved low native identity");
    Check(!FusionRecipes.TryGet(result,a,out _),"marker must not become base ingredient");
}
Check(MineFusions.ShotInterval(600)==150 && MineFusions.ShotInterval(480)==150,"far interval");
Check(MineFusions.ShotInterval(280)==100,"middle interval");
Check(MineFusions.ShotInterval(80)==50 && MineFusions.ShotInterval(-20)==50,"near interval");
var pea=Mine(FusionPlant.PeatatoMine);
MineFusions.OnFusion(pea);
Check(pea.mBoard.SunMoney==50,"peatato refund");
pea.mLaunchCounter=0;
var target=Target(pea,500);
MineFusions.Update(pea);
Check(pea.mState==PlantState.Notready && pea.mBoard.Projectiles.Count==1,"shoot underground");
Check(pea.mLaunchCounter==145,"source distance interval");
pea.mBoard.mPaused=true;
int countdown=pea.mStateCountdown;
MineFusions.Update(pea);
Check(pea.mStateCountdown==countdown && pea.mBoard.Projectiles.Count==1,"pause timers");
pea.mBoard.mPaused=false;
pea.OffGround=true;
MineFusions.Update(pea);
Check(pea.mStateCountdown==countdown,"carried mine does not arm");
pea.OffGround=false;
target.mMindControlled=true;
Check(MineFusions.Nearest(pea,false)==null,"friendly target excluded");
target.mMindControlled=false; target.Eligible=false;
Check(MineFusions.Nearest(pea,false)==null,"ineligible target excluded");
target.Eligible=true; target.mRow=1;
Check(MineFusions.Nearest(pea,false)==null,"ordinary fire stays in row");
target.mRow=0; target.mPosX=50;
pea.mStateCountdown=1;
MineFusions.Update(pea);
Check(pea.mState==PlantState.PotatoRising && !pea.mDead,"start original rise");
for(int i=0;i<MineFusions.RiseTicks;i++) MineFusions.Update(pea);
Check(pea.mDead && target.Damage==1800,"single armed contact explosion");
MineFusions.OnDeath(Death(pea));
Check(pea.mBoard.Projectiles.Count>=37,"36 radial death peas");
var radial=pea.mBoard.Projectiles.TakeLast(36).ToArray();
Check(Math.Abs(radial[0].mVelX-4.8f)<.001 && Math.Abs(radial[9].mVelY-4.8f)<.001,"radial cardinal vectors");
Check(Math.Abs(radial[18].mVelX+4.8f)<.001 && Math.Abs(radial[27].mVelY+4.8f)<.001,"backward and upward peas");
var solar=Mine(FusionPlant.SolarMine);
MineFusions.OnFusion(solar);
Check(solar.mBoard.SunMoney==25,"solar refund");
for(int i=0;i<1499;i++) MineFusions.Update(solar);
Check(solar.mBoard.Coins==0,"production waits full initial interval");
MineFusions.Update(solar);
Check(solar.mBoard.Coins==1 && solar.mLaunchCounter==1300,"production without targets and random interval");
MineFusions.OnDeath(Death(solar));
Check(solar.mBoard.Coins==3,"solar death gives two normal suns");
var nut=Mine(FusionPlant.MineNut);
var victim=Target(nut,70);
Check(nut.mPlantHealth==4000 && nut.mState==PlantState.PotatoArmed,"mine nut health");
MineFusions.Update(nut);
Check(victim.Damage==0 && !nut.mDead,"mine nut does not detonate on contact");
nut.mPlantHealth=2666; MineFusions.Update(nut);
Check(victim.Damage==0,"exact first boundary does not fire");
nut.mPlantHealth=2665; MineFusions.Update(nut);
Check(victim.Damage==500 && nut.mShootingCounter==1,"first damage band");
nut.mPlantHealth=4000; MineFusions.Update(nut);
nut.mPlantHealth=2000; MineFusions.Update(nut);
Check(victim.Damage==500,"healing does not reset first band");
nut.mPlantHealth=1333; MineFusions.Update(nut);
Check(victim.Damage==500,"exact second boundary");
nut.mPlantHealth=1332; MineFusions.Update(nut);
Check(victim.Damage==1000 && nut.mShootingCounter==3,"second band once");
MineFusions.Update(nut);
Check(victim.Damage==1000,"no repeated band explosion");
MineFusions.OnDeath(Death(nut));
Check(victim.Damage==2800,"death explosion 1800");
var jump=Mine(FusionPlant.MineNut); var jumpVictim=Target(jump,70);
jump.mPlantHealth=1000; MineFusions.Update(jump);
Check(jumpVictim.Damage==500 && jump.mShootingCounter==2,"large hit follows current source band");
var restored=Mine(FusionPlant.MineNut);
restored.mPlantHealth=1000; restored.mShootingCounter=jump.mShootingCounter;
var restoredVictim=Target(restored,70); MineFusions.Update(restored);
Check(restoredVictim.Damage==0,"saved flags prevent repeat after load");
var firing=Mine(FusionPlant.PeatatoMine);
var shotTarget=Target(firing,50);
PotatoProjectiles.Spawn(firing.mBoard,40,40,0,0,0);
var shot=firing.mBoard.Projectiles[0];
PotatoProjectiles.Move(shot);
Check(shotTarget.Damage==30 && shot.mDead,"potato pea deals 30 then dies");
PotatoProjectiles.Forget(shot);
Check(PotatoProjectiles.Contains(shot) && shot.mDead && !(bool)typeof(PotatoCollisionPatch).GetMethod("Prefix", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.Invoke(null, new object[] { shot })!,
    "dead potato retains collision suppression until native Reset");
PotatoProjectiles.Spawn(firing.mBoard,40,40,0,0,90);
var crossing=firing.mBoard.Projectiles[^1];
shotTarget.Eligible=false;
var otherRow=Target(firing,40,1,46);
PotatoProjectiles.Move(crossing);
Check(otherRow.Damage==30,"radial pea hits another row");
PotatoProjectiles.Spawn(firing.mBoard,40,40,0,0,0);
var reused=firing.mBoard.Projectiles[^1]; reused.Reset(); reused.DataID++;
Check(!PotatoProjectiles.Contains(reused),"pool identity");
PotatoProjectiles.Spawn(firing.mBoard,40,40,0,0,0);
var cleared=firing.mBoard.Projectiles[^1];
PotatoProjectiles.ForgetBoard(firing.mBoard);
Check(PotatoProjectiles.Contains(cleared),"board visual cleanup preserves saved ownership until native Reset");
Check(PotatoProjectiles.Intersection(0,0,100,0,40,-5,50,5,out var t) && Math.Abs(t-.4f)<.001,"swept collision");
Check(!PotatoProjectiles.Intersection(0,10,100,10,40,-5,50,5,out _),"parallel miss");
Check(!PotatoProjectiles.Intersection(100,0,110,0,40,-5,50,5,out _),"target behind segment");
// Reproduce the real native immunity before checking the scoped correction.
var eater = new Zombie();
var reproduction = Mine(FusionPlant.MineNut);
eater.EatPlant(reproduction);
Check(reproduction.mPlantHealth==4000,"native armed-potato guard reproduces infinite bites");
foreach (var identity in new[] { FusionPlant.PeatatoMine, FusionPlant.SolarMine, FusionPlant.MineNut })
    foreach (var state in new[] { PlantState.Notready, PlantState.PotatoRising, PlantState.PotatoArmed })
    {
        var plant=Mine(identity); plant.mState=state;
        int health=plant.mPlantHealth, timer=plant.mStateCountdown;
        MineBiteScope.Prefix(plant,out var scope);
        try { eater.EatPlant(plant); }
        finally { MineBiteScope.Finalizer(scope); }
        Check(plant.mPlantHealth==health-4,"native bite damages each mine in every living state");
        Check(plant.mState==state && plant.mStateCountdown==timer,"bite restores ability state without resetting timer");
        Check(FusionPlant.From(plant)==identity && plant.mSeedType==SeedType.Potatomine,"bite retains identity and low projectile category");
    }
var ordinary=new Plant { mSeedType=SeedType.Potatomine,mState=PlantState.PotatoArmed };
MineBiteScope.Prefix(ordinary,out var ordinaryScope); eater.EatPlant(ordinary); MineBiteScope.Finalizer(ordinaryScope);
Check(ordinaryScope==null && ordinary.mPlantHealth==300,"ordinary armed potato keeps native behavior");
var dying=Mine(FusionPlant.MineNut); dying.mPlantHealth=4;
MineBiteScope.Prefix(dying,out var deathScope); eater.EatPlant(dying); MineBiteScope.Finalizer(deathScope);
Check(dying.mDead && dying.mPlantHealth==0 && FusionPlant.From(dying)==FusionPlant.MineNut,"lethal bite uses native death with fusion identity");
var exceptional=Mine(FusionPlant.SolarMine); exceptional.mState=PlantState.PotatoRising;
MineBiteScope.Prefix(exceptional,out var exceptionScope);
try { throw new InvalidOperationException("simulated native failure"); }
catch (InvalidOperationException) { }
finally { MineBiteScope.Finalizer(exceptionScope); }
Check(exceptional.mState==PlantState.PotatoRising,"finalizer restores state after exception");
var nested=Mine(FusionPlant.MineNut);
MineBiteScope.Prefix(nested,out var outer); MineBiteScope.Prefix(nested,out var inner);
MineBiteScope.Finalizer(inner);
Check(nested.mState==PlantState.Notready,"nested finalizer keeps outer bite scope");
MineBiteScope.Finalizer(outer);
Check(nested.mState==PlantState.PotatoArmed,"outer finalizer restores armed state");
var recycled=Mine(FusionPlant.MineNut); MineBiteScope.Prefix(recycled,out var recycledScope);
recycled.DataID++; recycled.mState=PlantState.Ready; MineBiteScope.Finalizer(recycledScope);
Check(recycled.mState==PlantState.Ready,"pool reuse does not restore stale state");
var moved=Mine(FusionPlant.SolarMine); moved.mState=PlantState.PotatoRising;
MineBiteScope.Prefix(moved,out var movedScope); moved.mBoard=new Board { Pointer=(IntPtr)999 }; moved.mState=PlantState.Ready;
MineBiteScope.Finalizer(movedScope);
Check(moved.mState==PlantState.Ready,"board change does not restore stale state");
var bittenNut=Mine(FusionPlant.MineNut); var biteVictim=Target(bittenNut,70);
bittenNut.mPlantHealth=2666;
MineBiteScope.Prefix(bittenNut,out var bandScope); eater.EatPlant(bittenNut); MineBiteScope.Finalizer(bandScope);
MineFusions.Update(bittenNut);
Check(bittenNut.mPlantHealth==2662 && biteVictim.Damage==500,"native bite reaches mine-nut damage explosion");
// New variants use all source recipes and retain their saved native category.
foreach (var (ingredient, identity) in new[] {
    (SeedType.Iceshroom, FusionPlant.IceMine),
    (SeedType.Hypnoshroom, FusionPlant.HypnoMine),
    (SeedType.Jalapeno, FusionPlant.ChipMine) })
{
    Check(FusionRecipes.TryGet(ingredient, SeedType.Potatomine, out var forward) && forward == identity, "new forward recipe");
    Check(FusionRecipes.TryGet(SeedType.Potatomine, ingredient, out var reverse) && reverse == identity, "new reverse recipe");
    Check(FusionPlant.From(Mine(identity)) == identity && FusionPlant.HasImportedVisuals(identity), "new saved identity/assets");
}
var chip = Mine(FusionPlant.ChipMine); var chipVictim = Target(chip, 50);
chip.mStateCountdown = 1;
for (int tick = 0; tick < 200; ++tick) MineFusions.Update(chip);
Check(!chip.mDead && chip.mState == PlantState.Notready && chipVictim.Damage == 0 && chip.mBoard.Projectiles.Count == 0,
    "Chip Mine stays edible without rise, contact attack or projectiles");
int scatters = chip.mBoard.m_plants.Count;
chip.Die();
Check(chipVictim.Damage == 1800 && chip.mBoard.m_plants.Count == scatters + 7, "chip death explosion and scatter dispatch");
var hypno = Mine(FusionPlant.HypnoMine); var convert = Target(hypno, 50);
var bossTarget = Target(hypno, 60); bossTarget.mZombieType = ZombieType.Boss;
var resist = Target(hypno, 70); resist.FailControl = true;
var nextRow = Target(hypno, 50, 1); nextRow.mPosY = 80;
hypno.mState = PlantState.PotatoArmed;
MineFusions.Update(hypno);
Check(hypno.mDead && convert.mMindControlled && convert.Damage == 0, "successful hypno replaces explosion damage");
Check(bossTarget.Damage == 1800 && !bossTarget.mMindControlled && resist.Damage == 1800, "hypno immunity falls back to damage");
Check(nextRow.Damage == 0 && !nextRow.mMindControlled, "hypno explosion restricted to same row");
    Check(MineFusions.CurrentDetonation == null, "detonation context restored");
var buried = Mine(FusionPlant.HypnoMine); var buriedVictim = Target(buried, 50); buried.Die();
Check(buriedVictim.Damage == 0 && !buriedVictim.mMindControlled, "ordinary buried death does not explode");
var crushed = Mine(FusionPlant.HypnoMine); var crushedVictim = Target(crushed, 50);
crushed.mState = PlantState.PotatoArmed;
BurstMineCrushScope.Prefix(crushed, out var crushing);
try { crushed.Die(); } finally { BurstMineCrushScope.Finalizer(crushing); }
Check(crushedVictim.mMindControlled && MineFusions.CurrentDetonation == null, "armed crash detonates with restored context");
IceMineBlasts.Clear();
var iceMine = Mine(FusionPlant.IceMine);
var firstIce = Target(iceMine, 70); var chainedIce = Target(iceMine, 170); var farIce = Target(iceMine, 410);
var allyIce = Target(iceMine, 60); allyIce.mMindControlled = true;
var immuneIce = Target(iceMine, 60); immuneIce.FreezeImmune = true;
IceMineBlasts.Start(iceMine.mBoard, 0, 0, 0);
Check(firstIce.Damage == 600 && firstIce.mIceTrapCounter == 400 && chainedIce.Damage == 0, "first ice wave uses cells, 600 damage and four second freeze");
Check(allyIce.Damage == 0 && immuneIce.Damage == 600 && immuneIce.mIceTrapCounter == 0, "ice immunity and friendly exclusion");
iceMine.mBoard.mMainCounter += 14; IceMineBlasts.Update(iceMine.mBoard);
Check(chainedIce.Damage == 0, "chain waits 150 ms");
iceMine.mBoard.mPaused = true; iceMine.mBoard.mMainCounter++; IceMineBlasts.Update(iceMine.mBoard);
Check(chainedIce.Damage == 0, "paused chain does not advance");
iceMine.mBoard.mPaused = false; IceMineBlasts.Update(iceMine.mBoard);
Check(chainedIce.Damage == 600 && firstIce.Damage == 600 && farIce.Damage == 0, "chain spreads to neighboring cell without duplicate damage");
iceMine.mBoard.mMainCounter += 15; IceMineBlasts.Update(iceMine.mBoard);
Check(firstIce.Damage == 600 && chainedIce.Damage == 600, "visited targets stop cycles");
IceMineBlasts.Start(iceMine.mBoard, 0, 0, 0); chainedIce.DataID++;
iceMine.mBoard.mMainCounter += 15; IceMineBlasts.ForgetBoard(iceMine.mBoard); IceMineBlasts.Update(iceMine.mBoard);
Check(chainedIce.Damage == 600, "board teardown cancels pending waves");
IceMineBlasts.Clear();
var fertilizedIce = Mine(FusionPlant.IceMine); var growthTarget = Target(fertilizedIce, 70);
MineFusions.GrowImmediately(fertilizedIce);
Check(fertilizedIce.mState == PlantState.PotatoArmed && growthTarget.Damage == 600, "fertilizer also triggers ice rise effect");
MineFusions.GrowImmediately(fertilizedIce);
Check(growthTarget.Damage == 600, "already armed ice mine does not repeat rise effect");
IceMineBlasts.Clear();
var scatterBoard = new Board();
var scatterDeath = new CherryBombPlants.Death(scatterBoard, FusionPlant.ChipMine, 1, 4, 2, 360, 200, 0);
Check(MineFusions.ScatterCells(scatterDeath, true).Select(c => c.Col).SequenceEqual(new[]{5,3,6,2,7,1,8,0}),
    "chip source alternating scatter order, no origin");
scatterBoard.BlockedCells.Add((5,2));
scatterBoard.m_plants.Add(new Plant {mPlantCol=3,mRow=2,mBoard=scatterBoard});
var plantingContext = new object(); PlayerPlantingScope.Current = plantingContext;
MineFusions.ScatterMines(scatterDeath, true);
Check(scatterBoard.m_plants.Count == 7 && scatterBoard.m_plants.Skip(1).All(p => p.mStateCountdown == 500
    && p.mState == PlantState.Notready && p.mSeedType == SeedType.Potatomine && p.mImitaterType == SeedType.None),
    "scatter skips blocked/occupied cells and uses native five-second mines");
Check(!scatterBoard.SawPlayerContext && ReferenceEquals(PlayerPlantingScope.Current, plantingContext),
    "scripted scatter disables recipe context then restores it");
scatterBoard = new Board { FailSpawn=true };
try { MineFusions.ScatterMines(scatterDeath with { Board=scatterBoard }, true); }
catch (InvalidOperationException) { }
Check(ReferenceEquals(PlayerPlantingScope.Current, plantingContext), "scatter exception restores planting context");
PlayerPlantingScope.Current = null;
var clusterBoard = new Board();
MineFusions.ScatterMines(scatterDeath with {Board=clusterBoard});
Check(clusterBoard.m_plants.Count == 9 && clusterBoard.m_plants.Any(p => p.mPlantCol == 4 && p.mRow == 2),
    "existing Cluster Mine retains complete three-by-three scatter");
var risingIce = Mine(FusionPlant.IceMine); var riseVictim = Target(risingIce, 70);
risingIce.mStateCountdown = 1; MineFusions.Update(risingIce);
Check(risingIce.mState == PlantState.PotatoRising && risingIce.mStateCountdown == 100 && riseVictim.Damage == 0,
    "ice rise uses complete one second source animation");
for (int tick=0; tick<49; ++tick) MineFusions.Update(risingIce);
Check(riseVictim.Damage == 0, "ice rise waits first 500 ms");
MineFusions.Update(risingIce);
Check(riseVictim.Damage == 600 && risingIce.mState == PlantState.PotatoRising, "ice pulse occurs during source rise");
IceMineBlasts.Clear();
Check(MineFusions.FlashInterval(40)==20 && MineFusions.FlashInterval(80)==50
    && MineFusions.FlashInterval(480)==300 && MineFusions.FlashInterval(481)==600,
    "original distance dependent flashing boundaries");
// SeaMine begins armed, grows twice, then stops changing its saved stage.
var land = Mine(FusionPlant.LandMine);
Check(land.mState == PlantState.PotatoArmed && land.mStartRow == 0
    && land.mStateCountdown == 2900 && MineFusions.LandMineScale(land) == 1, "Land-mine immediate arming");
for (int tick = 0; tick < 2899; ++tick) MineFusions.Update(land);
Check(land.mStartRow == 0 && land.mStateCountdown == 1, "no early growth");
MineFusions.Update(land);
Check(land.mStartRow == 1 && land.mTargetX == 25 && MineFusions.LandMineScale(land) == 1,
    "first growth starts without scale jump");
for (int tick=0; tick<25; ++tick) MineFusions.Update(land);
Check(Math.Abs(MineFusions.LandMineScale(land) - 4f/3) < .0001f, "first scale transition completes");
land.mStateCountdown = 1; MineFusions.Update(land);
Check(land.mStartRow == 2 && land.mTargetX == 25, "second growth starts");
for (int tick=0; tick<40; ++tick) MineFusions.Update(land);
Check(land.mStartRow == 2 && land.mStateCountdown == 0
    && Math.Abs(MineFusions.LandMineScale(land) - 5f/3) < .0001f, "final stage remains stable");
land.mBoard.mPaused = true; land.mLaunchCounter=99; MineFusions.Update(land);
Check(land.mLaunchCounter == 99, "pause stops Land-mine timers");
land.mBoard.mPaused = false;
MineFusions.GrowImmediately(land);
Check(land.mStartRow == 2 && land.mStateCountdown == 0, "fertilizer does not reset ready mine growth");
foreach (int stage in new[] { 0, 1, 2 }) {
    var mine = Mine(FusionPlant.LandMine); mine.mStartRow = stage;
    var landVictim = Target(mine, 40);
    var landOtherRow = Target(mine, 40, 1, 80);
    var farVictim = Target(mine, 400);
    var twoRows = Target(mine, 40, 2, 80);
    MineFusions.Update(mine);
    Check(mine.mDead && landVictim.Damage == (stage==0 ? 900 : 1800), "stage damage " + stage);
    Check(landOtherRow.Damage == (stage==2 ? 1800 : 0) && farVictim.Damage == 0, "stage radius and row filter " + stage);
    Check(twoRows.Damage == 0, "final source blast is limited to adjacent rows");
    Check(MineFusions.CurrentDetonation == null, "detonation context restored");
}
var smallContact = Mine(FusionPlant.LandMine); var contactVictim = Target(smallContact, 75);
MineFusions.Update(smallContact);
Check(!smallContact.mDead && contactVictim.Damage == 0, "small contact collider excludes distance 35");
smallContact.mStartRow=2;
MineFusions.Update(smallContact);
Check(smallContact.mDead && contactVictim.Damage==1800, "grown contact collider includes distance 35");
var untriggered = Mine(FusionPlant.LandMine); var noBlast = Target(untriggered, 70);
untriggered.Die();
Check(noBlast.Damage == 0, "ordinary death does not detonate Land-mine");
Check(FusionRecipes.TryGet(SeedType.Seashroom, SeedType.Potatomine, out var landRecipe)
    && landRecipe == FusionPlant.SeaMine
    && FusionRecipes.TryGet(SeedType.Potatomine, SeedType.Seashroom, out landRecipe)
    && landRecipe == FusionPlant.LandMine, "directional land and aquatic mine recipes");

var torch = new Plant { mSeedType=SeedType.Sunflower, mImitaterType=SeedType.Torchwood, mLaunchCounter=2 };
Torchflower.Initialize(torch);
Projectile Shot(float x, float y, ProjectileType type=ProjectileType.Pea) {
    var shot = torch.mBoard.AddProjectile(x,y,0,0,type); shot.mProjectileType=type; return shot;
}
var inside=Shot(40,40); var boundary=Shot(200,40); var outside=Shot(200.1f,40);
var hostile=Shot(40,40,ProjectileType.ZombiePea); var ball=Shot(40,40,ProjectileType.Basketball);
var foreign=Shot(40,40); foreign.mBoard=new Board{Pointer=(IntPtr)900};
var alreadyDead=Shot(40,40); alreadyDead.mDead=true;
var high=Shot(40,40); high.mPosZ=-200;
SolarShooter.CustomSun=0;
TorchflowerProductionPatch.Prefix(torch);
Check(torch.mBoard.Coins==0 && !inside.mDead, "production waits for saved native delay");
TorchflowerProductionPatch.Prefix(torch);
for (int tick=0; tick<49; ++tick) TorchflowerProductionPatch.Prefix(torch);
Check(torch.mBoard.Coins==0 && !inside.mDead, "source waits half second before production");
TorchflowerProductionPatch.Prefix(torch);
Check(torch.mBoard.Coins==1 && SolarShooter.CustomSun==10 && inside.mDead && boundary.mDead,
    "25 sun plus five sun for each friendly projectile in actual source radius");
Check(!outside.mDead && !hostile.mDead && !ball.mDead && !foreign.mDead && !high.mDead,
    "exclude distant, hostile, foreign-board and high projectiles");
Check(torch.mLaunchCounter==2375-50 && Torchflower.Brightness(torch)==4,
    "source 0.95 production jitter and peak brightness after half second");
torch.mBoard.mPaused=true; torch.mLaunchCounter=1; TorchflowerProductionPatch.Prefix(torch);
Check(torch.mLaunchCounter==1 && torch.mBoard.Coins==1, "pause blocks production");
torch.mBoard.mPaused=false; torch.OffGround=true; TorchflowerProductionPatch.Prefix(torch);
Check(torch.mLaunchCounter==1, "carrying stops production");
var ordinaryFlower = new Plant { mSeedType=SeedType.Sunflower };
Check(TorchflowerProductionPatch.Prefix(ordinaryFlower), "ordinary sunflower retains native production");
Check(FusionRecipes.TryGet(SeedType.Sunflower, SeedType.Torchwood, out var torchRecipe)
    && torchRecipe==FusionPlant.Torchflower
    && FusionRecipes.TryGet(SeedType.Torchwood, SeedType.Sunflower, out torchRecipe)
    && torchRecipe==FusionPlant.Torchflower, "both Torchflower recipes");
var point=Mine(FusionPlant.LandMine);var primary=Target(point,100);var overlap=Target(point,110);
var outsidePoint=Target(point,116);var adjacentRow=Target(point,100,1);var friendlyPoint=Target(point,100);friendlyPoint.mMindControlled=true;
var foreignPoint=Target(point,100);foreignPoint.mBoard=new Board{Pointer=(IntPtr)12345};
MineFusions.Blast(point.mBoard,115,40,0,300,0,row:0);
Check(primary.Damage==300&&overlap.Damage==300,"Starch-shroom point overlap damages intersecting colliders");
Check(outsidePoint.Damage==0&&adjacentRow.Damage==0&&friendlyPoint.Damage==0&&foreignPoint.Damage==0,
 "Starch-shroom zero radius excludes separated colliders, other rows, allies and foreign boards");
Console.WriteLine($"Mine fusions: {assertions} assertions passed; offline doubles only.");


// Directional source pairs must never fall back to the opposite result.
Check(FusionRecipes.TryGet(SeedType.Potatomine, SeedType.Tanglekelp, out var landKelp)
    && landKelp == FusionPlant.TangleMine, "kelp onto potato creates land grabber");
Check(FusionRecipes.TryGet(SeedType.Tanglekelp, SeedType.Potatomine, out var waterKelp)
    && waterKelp == FusionPlant.KelpMine, "potato onto kelp creates aquatic grabber");
Check(FusionRecipes.TryGet(SeedType.Seashroom, SeedType.Potatomine, out var aquaticMine)
    && aquaticMine == FusionPlant.SeaMine, "potato onto sea creates aquatic mine");
Check(FusionRecipes.TryGet(SeedType.Potatomine, SeedType.Seashroom, out var landSea)
    && landSea == FusionPlant.LandMine, "existing land recipe stays distinct");
var newSea = Mine(FusionPlant.SeaMine);
Check(newSea.mState == PlantState.PotatoArmed && !newSea.mIsAsleep, "aquatic mine starts armed and awake");
UtilityMines.Fertilize(newSea);
Check(newSea.mStateCountdown == 50 && newSea.mStartRow == 0, "feeding shortens one growth stage");
newSea.mBoard.mPaused = true;
MineFusions.Update(newSea);
Check(newSea.mStateCountdown == 50, "paused aquatic growth stays fixed");
newSea.mBoard.mPaused = false;
for (int i = 0; i < 50; ++i) MineFusions.Update(newSea);
Check(newSea.mStartRow == 1, "aquatic mine grows after feeding countdown");
var masher = Mine(FusionPlant.PotatoMasher);
UtilityMines.Initialize(masher);
var approaching = Target(masher, 70);
var utilityPlantingContext = new object();
PlayerPlantingScope.Current = utilityPlantingContext;
UtilityMines.Tick(masher);
Check(masher.mBoard.m_plants.Count == 1
    && FusionPlant.From(masher.mBoard.m_plants[0]) == FusionPlant.PotatoMash,
    "producer creates native squash child with original variant");
Check(masher.mState == PlantState.Notready && masher.mStateCountdown == 1500
    && !masher.mBoard.SawPlayerContext && ReferenceEquals(PlayerPlantingScope.Current, utilityPlantingContext),
    "child allocation commits cooldown and restores scripted context");
PlayerPlantingScope.Current = null;
UtilityMines.Fertilize(masher);
Check(masher.mState == PlantState.Notready && masher.mStateCountdown == 50 && masher.mBoard.m_plants.Count == 1,
    "feeding shortens burial without removing existing child");
masher.mState = PlantState.PotatoArmed;
masher.mBoard.FailSpawn = true;
try { UtilityMines.Tick(masher); } catch (InvalidOperationException) { }
Check(masher.mState == PlantState.PotatoArmed && PlayerPlantingScope.Current == null,
    "failed allocation preserves armed producer and context");
var grabber = Mine(FusionPlant.TangleMine);
UtilityMines.Initialize(grabber);
grabber.mState = PlantState.TanglekelpGrabbing;
grabber.mStateCountdown = 70;
UtilityMines.Fertilize(grabber);
Check(grabber.mStateCountdown == 70 && grabber.mState == PlantState.TanglekelpGrabbing,
    "feeding preserves active grab");
var utilityVictim = Target(grabber, 50); grabber.mTargetZombieID = utilityVictim.DataID;
var interrupted = UtilityMines.Capture(grabber, true)!;
UtilityMines.Death(interrupted);
Check(utilityVictim.mDead && utilityVictim.Damage == 0, "interrupted grab finishes utilityVictim without explosion");
Check(UtilityMines.Capture(grabber, false) == null, "cancelled death produces no effect");
Console.WriteLine("Utility mine cases compiled; execution belongs to the user.");

var three = Mine(FusionPlant.ThreetatoMine);
Check(three.mTargetY == 0 && three.mTargetX == 0 && three.mStateCountdown == 1500,
    "Threetato initializes saved sentinels and exact 15-second burial");
Check(ThreetatoMine.Interval(0) == 30 && ThreetatoMine.Interval(10000) == 150,
    "Threetato attack interval is bounded at both source limits");
three.mState = PlantState.PotatoArmed; three.mStateCountdown = 0; three.mRow = 1;
Target(three, 400, row: 2);
for (int i = 0; i < 110; ++i) ThreetatoMine.Tick(three);
Check(three.mBoard.Projectiles.Count == 3 && three.mBoard.Projectiles.Select(p => p.mRow).Distinct().Count() == 3,
    "Threetato shoots all lanes when only an adjacent lane has a target");
var savedShot = three.mBoard.Projectiles[0];
var loadedShot = new Projectile { mBoard = three.mBoard, mProjectileType = savedShot.mProjectileType,
    mCobTargetX = savedShot.mCobTargetX, DataID = 999 };
Check(PotatoProjectiles.Contains(loadedShot), "saved projectile identity survives a new pool object");
loadedShot.Reset();
Check(!PotatoProjectiles.Contains(loadedShot), "native Reset prevents marker inheritance");
var threeContact = Mine(FusionPlant.ThreetatoMine);
threeContact.mState = PlantState.PotatoArmed; threeContact.mStateCountdown = 0;
var threeVictim = Target(threeContact, 40);
ThreetatoMine.Tick(threeContact);
Check(threeVictim.Damage == 5400 && threeVictim.mPosX == 120 && threeContact.mState == PlantState.Notready,
    "Threetato contact damages, knocks back and commits reburial once");
ThreetatoMine.Tick(threeContact);
Check(threeVictim.Damage == 5400 && threeContact.mStateCountdown == 1499,
    "burial prevents repeated contact explosion");
var vehicleMine = Mine(FusionPlant.ThreetatoMine);
vehicleMine.mState = PlantState.PotatoArmed; vehicleMine.mStateCountdown = 0;
var vehicle = Target(vehicleMine, 40); vehicle.mZombieType = ZombieType.Zamboni;
ThreetatoMine.Tick(vehicleMine);
Check(vehicle.Damage == 0 && vehicleMine.mState == PlantState.PotatoArmed,
    "vehicle contact leaves mine armed for native crush without a duplicate explosion");
ThreetatoCrushPatch.Prefix(vehicleMine, true);
Check(vehicle.Damage == 5400, "native crush triggers one armed Threetato explosion");
Console.WriteLine($"Mine regressions: {assertions} assertions passed; offline doubles only.");

var departing = three.mBoard.Projectiles;
Check(departing.All(p => p.mPosY == departing[1].mPosY),
    "all three potato projectiles are born at the plant, not in their destination lanes");
var upperShot = departing.Single(p => p.mRow == 0);
var middleShot = departing.Single(p => p.mRow == 1);
var lowerShot = departing.Single(p => p.mRow == 2);
float birthY = middleShot.mPosY;
PotatoProjectiles.Move(upperShot); PotatoProjectiles.Move(middleShot); PotatoProjectiles.Move(lowerShot);
Check(upperShot.mPosY < birthY && lowerShot.mPosY > birthY && middleShot.mPosY == birthY,
    "native Threepeater motion moves outer projectiles progressively into adjacent lanes");
Check(Math.Abs(upperShot.mPosY - birthY) < 10 && Math.Abs(lowerShot.mPosY - birthY) < 10,
    "first transition tick cannot teleport a whole lane");
Console.WriteLine($"Mine and lane regressions: {assertions} assertions passed; offline doubles only.");

var edgeMine = Mine(FusionPlant.ThreetatoMine);
edgeMine.mState = PlantState.PotatoArmed; edgeMine.mStateCountdown = 0;
Target(edgeMine, 400);
for (int i = 0; i < 100; ++i) ThreetatoMine.Tick(edgeMine);
Check(edgeMine.mBoard.Projectiles.Count == 3 && edgeMine.mBoard.Projectiles.Count(p => p.mRow == 0) == 2
    && edgeMine.mBoard.Projectiles.Count(p => p.mRow == 1) == 1,
    "missing edge lane returns as a delayed third projectile in the own row");
Console.WriteLine($"Final mine regressions: {assertions} assertions passed; offline doubles only.");

// A converted potato transfers movement to the native owner in the same tick.
var transferred = new Projectile { mBoard = edgeMine.mBoard, mProjectileType = ProjectileType.Pea,
    mCobTargetX = middleShot.mCobTargetX, mPosX = 100, mPosY = 100 };
SupportTorchFusions.OnConvert = shot => { shot.mCobTargetX = -622001; };
PotatoProjectiles.Move(transferred);
Check(transferred.mPosX == 100 && !PotatoProjectiles.Contains(transferred),
    "converted potato does not receive a second custom movement or collision");
SupportTorchFusions.OnConvert = null;
