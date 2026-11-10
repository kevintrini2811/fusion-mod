using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool value, string reason) { ++checks; if (!value) throw new Exception(reason); }
Plant Make(FusionPlant identity, Board? board = null)
{
    board ??= new();
    var plant = new Plant { mBoard = board, mPlantHealth = 300, mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    board.m_plants.Add(plant);
    SunflowerFusions.Initialize(plant);
    return plant;
}
void Ticks(Plant plant, int count) { for (int i = 0; i < count; ++i) SunflowerFusions.Tick(plant); }
Zombie Enemy(Board board, int row = 0)
{
    var zombie = new Zombie { mBoard = board, DataID = (uint)board.m_zombies.Count + 1, mRow = row };
    board.m_zombies.Add(zombie);
    return zombie;
}
var recipes = new (SeedType Ingredient, FusionPlant Result)[] {
    (SeedType.Flowerpot, FusionPlant.SolarPot), (SeedType.Seashroom, FusionPlant.SunsetShroom),
    (SeedType.Jalapeno, FusionPlant.SolarFlare), (SeedType.Blover, FusionPlant.SolarBlover)
};
foreach (var (ingredient, result) in recipes)
{
    Check(FusionRecipes.TryGet(ingredient, SeedType.Sunflower, out var found) && found == result, "source recipe");
    Check(FusionRecipes.TryGet(SeedType.Sunflower, ingredient, out found) && found == result, "reverse recipe");
    var plant = Make(result);
    Check(FusionPlant.From(plant) == result && FusionPlant.HasImportedVisuals(result), "persisted identity and imported art");
    Check(plant.mPlantHealth == 300 && plant.mPlantMaxHealth == 300, "original health");
    plant.mImitaterType = SeedType.None;
    Check(FusionPlant.From(plant) == (FusionPlant)ingredient, "native plant remains native");
    foreach (string stopped in new[] { "pause", "dead", "carry", "squish", "bungee", "sleep", "loading", "garden", "dispose" })
    {
        plant = Make(result);
        plant.mLaunchCounter = 250; plant.mShootingCounter = 75;
        if (stopped == "pause") plant.mBoard.mPaused = true;
        if (stopped == "dead") plant.mDead = true;
        if (stopped == "carry") plant.Airborne = true;
        if (stopped == "squish") plant.mSquished = true;
        if (stopped == "bungee") plant.mOnBungeeState = PlantOnBungeeState.OnBungee;
        if (stopped == "sleep") plant.mIsAsleep = true;
        if (stopped == "loading") plant.mApp.GameScene = GameScenes.Loading;
        if (stopped == "garden") plant.mApp.GameMode = GameMode.ChallengeZenGarden;
        if (stopped == "dispose") CherryBombPlants.DisposingBoard = plant.mBoard.Pointer;
        Ticks(plant, 3000);
        Check(plant.mLaunchCounter == 250 && plant.mShootingCounter == 75 && plant.mTargetY == 0
            && plant.mBoard.Coins.Count == 0 && plant.mBoard.Shots.Count == 0, "inactive timers: " + stopped);
        CherryBombPlants.DisposingBoard = IntPtr.Zero;
    }
}
var pot = Make(FusionPlant.SolarPot);
Ticks(pot, 50);
Check(pot.mBoard.Coins.Count == 0, "pot waits half second before payout");
Ticks(pot, 1);
Check(pot.mBoard.Coins.SequenceEqual(new[] { CoinType.SmallSun }), "pot produces fifteen sun");
Check(pot.mLaunchCounter == 2200, "pot production delay is sampled from 25 seconds");
pot.mBoard.Roll = 500; pot.mLaunchCounter = 1; Ticks(pot, 1);
Check(pot.mLaunchCounter == 2750, "production upper jitter bound");

var sea = Make(FusionPlant.SunsetShroom);
Ticks(sea, 51);
Check(sea.mBoard.Coins.SequenceEqual(new[] { CoinType.SmallSun }), "young aquatic sun");
Ticks(sea, 1948);
Check(sea.mStartRow == 0 && sea.mTargetX == 1, "first growth waits twenty seconds");
Ticks(sea, 1);
Check(sea.mStartRow == 1 && sea.mTargetX == 4000 && sea.mTargetY == 100, "first source growth and clip");
sea.mShootingCounter = 51; Ticks(sea, 1);
Check(sea.mBoard.Coins.Last() == CoinType.Sun, "middle stage produces twenty-five");
Ticks(sea, 3999);
Check(sea.mStartRow == 2 && sea.mTargetY == 100, "second growth after another forty seconds");
sea.mShootingCounter = 51; Ticks(sea, 1);
Check(sea.mBoard.Coins.Last() == CoinType.LargeSun, "mature stage produces fifty");
Ticks(sea, 10000);
Check(sea.mStartRow == 2 && sea.mTargetX == 0, "growth is capped");
Check(!SunsetNativeShooterPatch.Prefix(sea) && !SunsetNativeFirePatch.Prefix(sea), "no ordinary sea spores");
Check(SunsetNativeShooterPatch.Prefix(Make(SeedType.Seashroom)), "native sea shooter preserved");

var feeding = Make(FusionPlant.SunsetShroom);
Check(Fertilizer.Apply(feeding.mBoard, 0, 0), "fertilizer accepts aquatic variant");
Check(feeding.mTargetX == 2000 && feeding.mBoard.m_gridItems.Items.Count == 1, "growth waits for feeding animation");
var tool = feeding.mBoard.m_gridItems.Items.Single();
Fertilizer.UpdateFeeding(tool);
Check(feeding.mTargetX == 2000 && !tool.mDead, "unfinished animation does not accelerate");
feeding.mBoard.mApp.Effects.Single().mController.Loops = 1;
Fertilizer.UpdateFeeding(tool);
Check(feeding.mTargetX == 50 && tool.mDead, "completed animation leaves half a second");
Ticks(feeding, 49); Check(feeding.mStartRow == 0, "fertilizer does not skip remaining half second");
Ticks(feeding, 1); Check(feeding.mStartRow == 1 && feeding.mTargetX == 4000, "one application grows once");
SunflowerFusions.Fertilize(feeding); Ticks(feeding, 50);
Check(feeding.mStartRow == 2, "second application grows mature stage");
SunflowerFusions.Fertilize(feeding); Ticks(feeding, 100);
Check(feeding.mStartRow == 2, "fertilizer cannot add a third growth");
var removed = Make(FusionPlant.SunsetShroom);
Fertilizer.Apply(removed.mBoard, 0, 0);
removed.mBoard.mApp.Effects.Single().mController.Loops = 1;
var replacement = Make(FusionPlant.SunsetShroom, removed.mBoard); removed.mDead = true;
Fertilizer.UpdateFeeding(removed.mBoard.m_gridItems.Items.Single());
Check(replacement.mTargetX == 2000, "feeding DataID prevents replacement inheritance");

var blover = Make(FusionPlant.SolarBlover);
var balloon = Enemy(blover.mBoard); balloon.mZombieType = ZombieType.Balloon;
var friend = Enemy(blover.mBoard); friend.mZombieType = ZombieType.Balloon; friend.mMindControlled = true;
Ticks(blover, 48); Check(blover.mBoard.Shots.Count == 0 && !balloon.mDead, "wait for blow event");
Ticks(blover, 1);
Check(blover.mBoard.Shots.Count == 1 && balloon.mDead && !friend.mDead, "first solar shot and inherited balloon removal");
Check(blover.mBoard.mFogBlownCountDown == 3000, "thirty-second fog clear");
Ticks(blover, 142);
Check(blover.mBoard.Shots.Count == 72 && !blover.mDead, "seventy-two shots before final coroutine yield");
Check(blover.mBoard.Shots.First().Angle == -10 && blover.mBoard.Shots.Last().Angle == -720, "two complete source spirals");
Check(Math.Abs(blover.mBoard.Shots[0].Speed - 5 * (80f / 1.35f) / 100) < .000001, "source speed conversion");
Ticks(blover, 2); Check(blover.mDead, "death after final fixed yield");
Ticks(blover, 1000); Check(blover.mBoard.Shots.Count == 72, "dead blover cannot repeat");
var shot = blover.mBoard.Shots.First(); var zombie = Enemy(blover.mBoard);
Check(SunflowerFusions.IsShot(shot) && !BloverSunTorchwoodPatch.Prefix(shot), "saved solar provenance excludes Torchwood");
Check(!BloverSunImpactPatch.Prefix(shot, zombie), "solar impact replaces native damage");
Check(zombie.Damage == 10 && shot.mDead && blover.mBoard.SmallSuns.SequenceEqual(new[] { 5 }), "ten damage and five physical sun");
SunflowerFusions.Impact(shot, zombie);
Check(zombie.Damage == 10 && blover.mBoard.SmallSuns.Count == 1, "no duplicate impact");

var flare = Make(FusionPlant.SolarFlare);
var nut = Make(SeedType.Wallnut, flare.mBoard); nut.mPlantHealth = 4000;
var burnt = Make(SeedType.Peashooter, flare.mBoard);
var spared = Make(SeedType.Sunflower, flare.mBoard);
var exact = Make(SeedType.Garlic, flare.mBoard); exact.mPlantHealth = 1800;
var otherRow = Make(SeedType.Peashooter, flare.mBoard); otherRow.mRow = 1;
var enemy = Enemy(flare.mBoard); var ally = Enemy(flare.mBoard); ally.mMindControlled = true;
var otherEnemy = Enemy(flare.mBoard, 1);
flare.mBoard.Rolls = new(new[] { 2, 2, 0, 0 });
Ticks(flare, 66); Check(enemy.Damage == 0 && !flare.mDead, "flare waits original explode event");
Ticks(flare, 1);
Check(flare.mDead && enemy.Damage == 3800 && enemy.Thaws == 1, "1800 plus five hundred per plant, clears cold");
Check(ally.Damage == 0 && otherEnemy.Damage == 0 && otherRow.mPlantHealth == 300, "row and enemy targeting");
Check(nut.mPlantHealth == 2200 && burnt.mDead && spared.mPlantHealth == 1 && !spared.mDead
    && exact.mDead, "plant damage, one-third rescue, strict negative boundary");
Check(flare.mBoard.Coins.Count(c => c == CoinType.LargeSun) == 4
    && flare.mBoard.Coins.Count(c => c == CoinType.Sun) == 1, "sun rewards count plants and zombies once");
SunflowerFusions.Flare(flare);
Check(enemy.Damage == 3800, "completed explosion is not replayed");
var eaten = Make(FusionPlant.SolarFlare); var eatingEnemy = Enemy(eaten.mBoard);
SolarFlareDeathPatch.Prefix(eaten, true, out var death); eaten.Die(); SolarFlareDeathPatch.Postfix(eaten, death);
Check(eatingEnemy.Damage == 1800, "destruction before fuse also explodes");
var consumedFlare = Make(FusionPlant.SolarFlare);
PlayerPlantingScope.ConsumingIngredient = consumedFlare.Pointer;
SolarFlareDeathPatch.Prefix(consumedFlare, true, out death);
Check(death == null, "fusion consumption never explodes");
PlayerPlantingScope.ConsumingIngredient = IntPtr.Zero;
var reusedFlare = Make(FusionPlant.SolarFlare);
SolarFlareDeathPatch.Prefix(reusedFlare, true, out death); ++reusedFlare.DataID; reusedFlare.Die();
SolarFlareDeathPatch.Postfix(reusedFlare, death);
Check(reusedFlare.mTargetY == 0, "pooled replacement cannot inherit death ability");

foreach (var identity in recipes.Select(r => r.Result))
{
    var original = Make(identity);
    Ticks(original, identity == FusionPlant.SolarFlare ? 30 : identity == FusionPlant.SolarBlover ? 85 : 35);
    var restored = Make(identity);
    restored.mLaunchCounter = original.mLaunchCounter; restored.mShootingCounter = original.mShootingCounter;
    restored.mTargetX = original.mTargetX; restored.mTargetY = original.mTargetY; restored.mStartRow = original.mStartRow;
    Ticks(original, 250); Ticks(restored, 250);
    Check(original.mStartRow == restored.mStartRow && original.mTargetX == restored.mTargetX
        && original.mTargetY == restored.mTargetY && original.mShootingCounter == restored.mShootingCounter
        && original.mLaunchCounter == restored.mLaunchCounter, "restored native fields continue same phase");
}
var silverFlower = Make(FusionPlant.SilverSunflower);
Ticks(silverFlower, 50);
Check(silverFlower.mBoard.Coins.Count == 0, "silver sunflower waits for production event");
Ticks(silverFlower, 1);
Check(silverFlower.mBoard.Coins.SequenceEqual(new[] { CoinType.Sun }), "silver sunflower produces 25, not 15 or 50");
Check(silverFlower.mLaunchCounter == 1375, "silver sunflower uses 15 seconds with five percent jitter");
silverFlower.mBoard.Roll = 150; silverFlower.mLaunchCounter = 1; Ticks(silverFlower, 1);
Check(silverFlower.mLaunchCounter == 1575, "silver sunflower production upper bound");
var fedSpicyStar = Make(FusionPlant.SpicyStarfruit);
fedSpicyStar.mPlantHealth = 10; fedSpicyStar.mLaunchCounter = 90;
Check(Fertilizer.Apply(fedSpicyStar.mBoard, 0, 0) && fedSpicyStar.mPlantHealth == 300
    && fedSpicyStar.mLaunchCounter == 50, "Spicy Starfruit fertilizer heals and caps positive fuse at half a second");
fedSpicyStar.mLaunchCounter = 12; Fertilizer.Apply(fedSpicyStar.mBoard, 0, 0);
Check(fedSpicyStar.mLaunchCounter == 12, "Spicy Starfruit fertilizer never lengthens a short fuse");
foreach (var identity in new[] { FusionPlant.Starlic, FusionPlant.WinterStarfruit, FusionPlant.Starshooter })
{
    var fedStar = Make(identity); fedStar.mLaunchCounter = 90; fedStar.mStartRow = 20;
    Fertilizer.Apply(fedStar.mBoard, 0, 0);
    Check(fedStar.mLaunchCounter == 90 && fedStar.mStartRow == 20, "Shooting star fertilizer preserves attack progress");
}

var fedSilverIce = Make(FusionPlant.SilverIceShroom); fedSilverIce.mLaunchCounter = 150;
Check(Fertilizer.Apply(fedSilverIce.mBoard, 0, 0) && fedSilverIce.mLaunchCounter == 50,
    "fertilizer caps silver ice delay at half a second");
fedSilverIce.mLaunchCounter = 10; Fertilizer.Apply(fedSilverIce.mBoard, 0, 0);
Check(fedSilverIce.mLaunchCounter == 10, "fertilizer does not extend short silver ice fuse");

var silverPot = Make(FusionPlant.SilverPot);
PotFusions.Initialize(silverPot);
silverPot.mLaunchCounter = 100; silverPot.mShootingCounter = 51;
PotFusions.Tick(silverPot);
Check(silverPot.mBoard.Coins.Count == 2 && silverPot.mTargetY == 5,
    "Silver pot pays at brightening midpoint and saves remainder");
silverPot.mShootingCounter = 51;
PotFusions.Tick(silverPot);
Check(silverPot.mBoard.Coins.Count == 5 && silverPot.mTargetY == 0,
    "Two silver pot productions pay fifty coins");
silverPot.mBoard.mPaused = true;
int potCounter = silverPot.mLaunchCounter;
PotFusions.Tick(silverPot);
Check(silverPot.mLaunchCounter == potCounter, "Paused support stops production");
var fogMemory = System.Runtime.InteropServices.Marshal.AllocHGlobal(0x20 + 12 * 7 * 4);
try {
    for (int i = 0; i < 84; ++i) System.Runtime.InteropServices.Marshal.WriteInt32(fogMemory, 0x20 + i * 4, 255);
    PotFusions.FadeFog(fogMemory, 12, 7, 4, 2);
    for (int col = 0; col < 12; ++col)
        for (int row = 0; row < 7; ++row)
            Check(System.Runtime.InteropServices.Marshal.ReadInt32(fogMemory, 0x20 + 4 * (col * 7 + row))
                == (col == 4 && (row == 2 || row == 3) ? 249 : 255),
                "Lumos pot changes only its own two fog edges");
    PotFusions.FadeFog(fogMemory, 11, 7, 4, 2);
    Check(System.Runtime.InteropServices.Marshal.ReadInt32(fogMemory, 0x20 + 4 * 30) == 249,
        "Unexpected fog dimensions cannot write native memory");
    foreach (int radius in new[] {1, 2}) {
        for (int i=0;i<84;++i) System.Runtime.InteropServices.Marshal.WriteInt32(fogMemory,0x20+i*4,255);
        PotFusions.FadeFog(fogMemory,12,7,4,2,radius);
        for(int col=0;col<12;++col) for(int row=0;row<7;++row)
            Check(System.Runtime.InteropServices.Marshal.ReadInt32(fogMemory,0x20+4*(col*7+row))
                == (Math.Abs(col-4)<=radius && row>=2-radius && row<=3+radius ? 249 : 255),
                "Beacon light covers all edges of its original square");
    }
    for(int i=0;i<84;++i)System.Runtime.InteropServices.Marshal.WriteInt32(fogMemory,0x20+i*4,255);
    PotFusions.FadeFog(fogMemory,12,7,0,0,2);
    Check(System.Runtime.InteropServices.Marshal.ReadInt32(fogMemory,0x20)==249,
        "Light at board boundary clips memory accesses");
} finally { System.Runtime.InteropServices.Marshal.FreeHGlobal(fogMemory); }

foreach (var potIdentity in new[] { FusionPlant.SilverPot, FusionPlant.LumosPot }) {
    var protectedPot = Make(potIdentity); PotFusions.Initialize(protectedPot);
    Check(PotFusions.Protected(protectedPot) && !PotBiteProtectionPatch.Prefix(protectedPot)
        && !PotCrushProtectionPatch.Prefix(protectedPot), "Half-second source protection blocks bites and crushing");
    Plant? collided = protectedPot;
    var hostileShot = new Projectile { mBoard = protectedPot.mBoard };
    PotProjectileProtectionPatch.Postfix(hostileShot, ref collided);
    Check(hostileShot.mDead && collided == null, "Protected pot consumes enemy projectile before native health write");
    var priorDamageBoard = RetaliationNuts.RadiusBoard;
    RetaliationNuts.RadiusBoard = protectedPot.mBoard.Pointer;
    Check(!PotDeathProtectionPatch.Prefix(protectedPot), "Initial protection blocks native radius removal");
    RetaliationNuts.RadiusBoard = priorDamageBoard;
    Check(PotDeathProtectionPatch.Prefix(protectedPot), "Shovel removal remains possible outside damage scope");
    PlayerPlantingScope.ConsumingIngredient = protectedPot.Pointer;
    Check(!PotFusions.Protected(protectedPot), "Fusion consumption bypasses initial protection");
    PlayerPlantingScope.ConsumingIngredient = IntPtr.Zero;
    protectedPot.mBoard.mPaused = true;
    PotFusions.Tick(protectedPot);
    Check(protectedPot.mTargetX == 50, "Paused grace duration remains saved");
    protectedPot.mBoard.mPaused = false;
    for (int tick = 0; tick < 50; ++tick) PotFusions.Tick(protectedPot);
    Check(protectedPot.mTargetX == 0 && !PotFusions.Protected(protectedPot)
        && PotBiteProtectionPatch.Prefix(protectedPot), "Protection expires after fifty active simulation ticks");
}
foreach(var identity in new[]{FusionPlant.CabbagePot,FusionPlant.ButterPot,FusionPlant.UmbrellaPot,
    FusionPlant.NutPot,FusionPlant.GarlicPot,FusionPlant.SpikePot,FusionPlant.MelonPot})
{
    var newSupport=Make(identity); PotFusions.Initialize(newSupport);
    Check(newSupport.mPlantHealth==300&&newSupport.mPlantMaxHealth==300&&newSupport.mTargetX==50,"New supports inherit source initial protection");
    Check(PotFusions.Protected(newSupport),"Fresh support blocks combat damage");
    newSupport.mBoard.mPaused=true; PotFusions.Tick(newSupport);
    Check(newSupport.mTargetX==50,"Pause retains source protection period");
    newSupport.mBoard.mPaused=false;
    for(int tick=0;tick<50;++tick) PotFusions.Tick(newSupport);
    Check(!PotFusions.Protected(newSupport),"Protection ends after fifty simulation ticks");
    newSupport.mPlantHealth=1;
    Check(Fertilizer.Apply(newSupport.mBoard,newSupport.mPlantCol,newSupport.mRow)&&newSupport.mPlantHealth==300,
        "Fertilizer heals new supports");
    Check(newSupport.mTargetX==0&&newSupport.mBoard.m_gridItems.Items.Count==0,"Fertilizer does not restart protection or create evolution");
}

Console.WriteLine($"PASS: {checks} sunflower fusion checks; no game code executed.");

foreach (var identity in new[] { FusionPlant.SolarPot, FusionPlant.SilverSunflower, FusionPlant.SunsetShroom })
{
    var producer = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker, mShootingCounter = 1 };
    Check(!SunflowerProducerShootingPatch.Prefix(producer) && !SunsetNativeFirePatch.Prefix(producer),
        "production animation cannot enter native projectile path");
    Check(producer.mShootingCounter == 1, "guard preserves production counter for its owner");
}
