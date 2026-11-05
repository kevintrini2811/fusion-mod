using FusionMod;
using Il2CppReloaded.Gameplay;

static class DoomShootersTests
{
    static void Require(bool value, string message) { if (!value) throw new Exception(message); }
    static Plant Plant(FusionPlant identity)
    {
        var plant = new Plant { mSeedType = identity.NativeType, mImitaterType = identity.Marker, mRow = 2 };
        DoomShooters.Initialize(plant); return plant;
    }
    static Zombie Enemy(Board board, uint id, float x, float y = 250)
    {
        var zombie = new Zombie { mBoard = board, DataID = id, X = x, Y = y, mRow = 2 };
        board.m_zombies.Add(zombie); return zombie;
    }
    static void Main()
    {
        var linked = Plant(FusionPlant.DoomPea);
        linked.mPlantCol = 3;
        var frenzy = Plant(FusionPlant.FrenzyShroom);
        frenzy.mBoard = linked.mBoard; frenzy.mPlantCol = 2; frenzy.mTargetX = 20;
        linked.mBoard.m_plants.Add(frenzy);
        DoomShooters.Shoot(linked, 2);
        Require(linked.mLaunchRate == 20 && frenzy.mShootingCounter == 41,
            "Left Frenzy-shroom supplies attack speed and receives current Doom damage");
        frenzy.mDead = true;
        DoomShooters.Shoot(linked, 2);
        Require(linked.mLaunchRate == 150 && linked.mTargetX == 2 && frenzy.mShootingCounter == 41,
            "A removed neighbour restores speed without resetting growth or writing stale damage");
        var pea = Plant(FusionPlant.DoomPea);
        for (int i = 0; i < 100; ++i) DoomShooters.Shoot(pea, 2);
        Require(pea.mBoard.Shots.First().mCobTargetRow == 41 && pea.mBoard.Shots.Last().mCobTargetRow == 100,
            "Damage grows before the first shot and stops at +60");
        int count = pea.mBoard.Shots.Count;
        pea.Carried = true; DoomShooters.Shoot(pea, 2);
        Require(pea.mBoard.Shots.Count == count && pea.mTargetX == 60, "Carried plants never shoot or advance growth");
        pea.Carried = false;
        pea.OnFire = () => throw new Exception("allocation callback");
        try { DoomShooters.Shoot(pea, 2); } catch (Exception) { }
        Require(DoomShooters.Firing == null && DoomShooters.Created == null, "Fire context restores on exceptions");

        var gatling = Plant(FusionPlant.GatlingDoom);
        for (int i = 0; i < 16; ++i) DoomShooters.Shoot(gatling, 2);
        Require(gatling.mBoard.Shots.Take(15).All(s => s.mCobTargetRow == 300)
            && DoomProjectiles.Family(gatling.mBoard.Shots[15]) == 3 && gatling.mLaunchCounter == 450,
            "Only every sixteenth shell is enlarged and starts rest");
        var shot = gatling.mBoard.Shots[15];
        var primary = Enemy(gatling.mBoard, 1, shot.mPosX);
        var nearby = Enemy(gatling.mBoard, 2, shot.mPosX + 100);
        var outside = Enemy(gatling.mBoard, 3, shot.mPosX + 300);
        var friendly = Enemy(gatling.mBoard, 4, shot.mPosX); friendly.mMindControlled = true;
        DoomProjectiles.Impact(shot, primary); DoomProjectiles.Impact(shot, primary);
        Require(primary.Damage == 3600 && nearby.Damage == 1800 && outside.Damage == 0 && friendly.Damage == 0,
            "Direct plus area damage occurs once, excluding distant and friendly targets");
        Require(primary.Flags == DamageFlags.BypassesShield, "Doom shells bypass shields");

        var three = Plant(FusionPlant.DoomSpreader);
        Enemy(three.mBoard, 1, 300);
        for (int i = 0; i < 1000; ++i) DoomShooters.Tick(three);
        Require(three.mBoard.Shots.Count == 21 && three.mBoard.Shots.TakeLast(3).All(s => s.mCobTargetRow == 120),
            "Seventh volley contains three large shells");
        var star = Plant(FusionPlant.DeathStar);
        star.mBoard.FailCreate = true; star.mLaunchCounter = 1;
        DoomShooters.Tick(star); Require(!star.mDead, "Meteor allocation failure retains plant");
        star.mLaunchCounter = 300; DoomShooters.Fertilize(star);
        Require(star.mLaunchCounter == 50, "Fertilizer caps Death Star timer");
        star.mBoard.FailCreate = false; star.mLaunchCounter = 1;
        DoomShooters.Tick(star);
        Require(star.mDead && star.mBoard.Shots.Count == 1, "Successful allocation consumes Death Star once");
        var meteor = star.mBoard.Shots.Single();
        star.mBoard.mPaused = true; DoomProjectiles.TickMeteor(meteor);
        Require(meteor.mProjectileAge == 0, "Meteor pauses");
        star.mBoard.mPaused = false;
        for (int i = 0; i < 300; ++i) DoomProjectiles.TickMeteor(meteor);
        Require(meteor.mDead && star.mBoard.Shots.Count == 181
            && star.mBoard.Shots.Skip(1).All(s => s.mCobTargetRow == 20), "Meteor splits exactly once into 180 stars");
        // Reconstructing only serialized fields retains identity and damage.
        var restored = new Projectile { mBoard = gatling.mBoard, mProjectileType = ProjectileType.Pea,
            mCobTargetX = gatling.mBoard.Shots[0].mCobTargetX, mCobTargetRow = 300 };
        Require(DoomProjectiles.Family(restored) == 2, "Projectile identity needs no transient state");
        Console.WriteLine("Doom shooter cases passed.");
    }
}
