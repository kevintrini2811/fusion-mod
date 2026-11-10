using FusionMod;
using Il2CppReloaded.Gameplay;

internal static class CactusCases
{
    private static void Check(bool value, string message)
    { if (!value) throw new Exception(message); }
    private static Plant Plant(Board board, FusionPlant identity)
    {
        var plant = new Plant { mBoard = board, mSeedType = identity.NativeType, mImitaterType = identity.Marker };
        board.m_plants.Add(plant);
        CactusFusions.Initialize(plant);
        IllumiNut.Initialize(plant);
        return plant;
    }
    private static Zombie Zombie(Board board, float x, bool air = false)
    {
        var zombie = new Zombie { mBoard = board, X = x, Flying = air };
        board.m_zombies.Add(zombie); return zombie;
    }
    private static Projectile Restore(Projectile old)
    {
        var copy = new Projectile();
        foreach (var field in typeof(Projectile).GetFields())
            if (!field.IsStatic) field.SetValue(copy, field.GetValue(old));
        return copy;
    }
    internal static void Run()
    {
        foreach (var (a, b, result) in new[] {
            (SeedType.Fumeshroom, SeedType.Cactus, FusionPlant.SaguaroShroom),
            (SeedType.Iceshroom, SeedType.Cactus, FusionPlant.IceCactus),
            (SeedType.Magnetshroom, SeedType.Cactus, FusionPlant.MagnetCactus),
            (SeedType.Plantern, SeedType.Wallnut, FusionPlant.IllumiNut) })
        {
            Check(FusionRecipes.TryGet(a, b, out var forward) && forward == result, "new forward recipe");
            Check(FusionRecipes.TryGet(b, a, out var reverse) && reverse == result, "new reverse recipe");
            Check(FusionPlant.From(Plant(new(), result)) == result, "native save identity");
        }
        var board = new Board(); var plant = Plant(board, FusionPlant.SaguaroShroom);
        var ground = Zombie(board, 100); var air = Zombie(board, 120, true);
        var far = Zombie(board, 600);
        CactusFusions.Fire(plant);
        Check(ground.Damage == 40 && air.Damage == 40 && far.Damage == 0, "low fumes pierce both target kinds within source range");
        Check(CactusFusions.Click(plant, 1, 0) && plant.mTargetX == 1, "click selects high mode");
        int transition = plant.mTargetY;
        board.mPaused = true; CactusFusions.Tick(plant);
        Check(plant.mTargetY == transition, "pause preserves transition"); board.mPaused = false;
        CactusFusions.Fire(plant);
        Check(ground.Damage == 40 && air.Damage == 200, "high fumes hit only air at quadruple damage");
        plant.mIsAsleep = true; CactusFusions.Fire(plant);
        Check(air.Damage == 200, "sleep blocks source ability"); plant.mIsAsleep = false;
        plant.Carried = true; CactusFusions.Click(plant, 1, 0);
        Check(plant.mTargetX == 1, "carrying prevents mode changes"); plant.Carried = false;

        board = new(); plant = Plant(board, FusionPlant.IceCactus);
        ground = Zombie(board, 100); air = Zombie(board, 200, true);
        Check(CactusFusions.Target(plant, true) == air, "air priority despite nearer ground target");
        plant.mTargetX = 1;
        var shot = CactusProjectiles.Spawn(plant)!;
        air.Frost = 90;
        CactusProjectiles.Impact(shot, air);
        Check(air.Damage == 320 && air.Chilled && CactusProjectiles.Remaining(shot) == 2, "threshold frost and air bonuses stack");
        shot = Restore(shot);
        CactusProjectiles.Impact(shot, air);
        Check(air.Damage == 320, "restored pierce cannot hit previous target again");
        var second = Zombie(board, 220, true); CactusProjectiles.Impact(shot, second);
        shot = Restore(shot); CactusProjectiles.Impact(shot, air);
        Check(air.Damage == 320, "both previous handles survive save");
        CactusProjectiles.Impact(shot, Zombie(board, 240, true));
        Check(shot.mDead, "third airborne target consumes spike");
        plant.mTargetX = 0; shot = CactusProjectiles.Spawn(plant)!;
        CactusProjectiles.Impact(shot, ground);
        Check(shot.mDead && ground.Damage == 20, "ground impact consumes ice spike");

        board = new(); plant = Plant(board, FusionPlant.MagnetCactus);
        Check(CactusFusions.TransitionTicks(plant) == 69, "magnet lowering preserves full 0.6833 second source clip");
        plant.mTargetX = 1;
        Check(CactusFusions.TransitionTicks(plant) == 67, "magnet rising uses 0.6667 second source clip");
        plant.mTargetX = 0;
        var metal = Zombie(board, 100); metal.mHelmType = HelmType.Pail; metal.mHelmHealth = 100;
        CactusProjectiles.Impact(CactusProjectiles.Spawn(plant)!, metal);
        Check(metal.Damage == 80, "present metal armor doubles magnetic hit");
        metal.mHelmHealth = 0;
        CactusProjectiles.Impact(CactusProjectiles.Spawn(plant)!, metal);
        Check(metal.Damage == 120, "lost armor removes bonus");
        board.FailAllocation = true;
        Check(CactusProjectiles.Spawn(plant) == null, "failed projectile allocation is safe");

        board = new(); plant = Plant(board, FusionPlant.IllumiNut);
        plant.mPlantHealth = 1000; plant.mLaunchCounter = 1; IllumiNut.Tick(plant);
        Check(plant.mPlantHealth == 1000, "own light alone never heals");
        var light = Plant(board, SeedType.Plantern); light.mPlantCol = 1;
        plant.mLaunchCounter = 1; IllumiNut.Tick(plant);
        Check(plant.mPlantHealth == 1150, "nearby light adds 150 health");
        light.Carried = true; plant.mLaunchCounter = 1; IllumiNut.Tick(plant);
        Check(plant.mPlantHealth == 1150, "carried light is not a provider");
        plant.mLaunchCounter = 1500; IllumiNut.Fertilize(plant);
        Check(plant.mLaunchCounter == 50, "fertilizer caps positive attribute timer");
        plant.mLaunchCounter = 0; IllumiNut.Fertilize(plant);
        Check(plant.mLaunchCounter == 0, "fertilizer preserves ready timer");
        board.mPaused = true; IllumiNut.Tick(plant);
        Check(plant.mLaunchCounter == 0, "pause preserves ready light timer");
    }
}
