using FusionMod;
using Il2CppReloaded.Gameplay;

internal static class BoltPultCases
{
    private static void Check(bool condition, string message)
    { if (!condition) throw new Exception("Bolt-pult: " + message); }

    internal static void Run()
    {
        Plant Make(Board board) => new() { mBoard = board, mSeedType = FusionPlant.BoltPult.NativeType,
            mImitaterType = FusionPlant.BoltPult.Marker };
        Zombie Enemy(Board board, int armor = 0)
        {
            var zombie = new Zombie { mBoard = board, mHelmHealth = armor };
            board.m_zombies.Add(zombie); return zombie;
        }
        var board = new Board();
        var plant = Make(board);
        LobberFusions.Initialize(plant);
        Check(plant.mPlantHealth == 2000 && plant.mPlantMaxHealth == 2000, "source toughness");
        Check(FusionRecipes.TryGet(FusionPlant.KernelMorph, FusionPlant.Bucket, out var result)
            && result == FusionPlant.BoltPult, "bucket upgrade recipe");
        Check(FusionRecipes.TryGet(FusionPlant.Bucket, FusionPlant.KernelMorph, out result)
            && result == FusionPlant.BoltPult, "reverse resolver");
        Check(!MixedLobberNativeFirePatch.Prefix(plant) && !MixedLobberShootingPatch.Prefix(plant),
            "single shooting owner");
        var target = Enemy(board, 1100);
        foreach (bool butter in new[] { false, true })
        {
            plant.mShootingCounter = butter ? 1 : 0;
            LobberProjectiles.Fire(plant, plant.mRow, target, false, 0);
            var shot = board.Shots[^1];
            Check(LobberProjectiles.Family(shot) == (butter ? 17 : 16), "saved family");
            Check(shot.mMotionType == ProjectileMotion.Lobbed, "native saved lob motion");
            int before = target.Damage;
            LobberProjectiles.Impact(shot, target);
            Check(target.Damage - before == (butter ? 1200 : 80), "kernel versus armor-breaking butter");
            Check(shot.mDead, "consumed once");
            LobberProjectiles.Impact(shot, target);
            Check(target.Damage - before == (butter ? 1200 : 80), "no repeated impact");
        }
        Check(target.mButteredCounter == 400, "four-second butter");
        plant.mShootingCounter = 1;
        var unarmored = Enemy(board);
        LobberProjectiles.Fire(plant, plant.mRow, unarmored, false, 0);
        var saved = board.Shots[^1];
        plant.mDead = true;
        LobberProjectiles.Impact(saved, unarmored);
        Check(unarmored.Damage == 100 && unarmored.mButteredCounter == 400,
            "saved projectile independent of dead producer");

        plant = Make(board);
        plant.mShootingCounter = 1;
        var longButter = Enemy(board);
        longButter.mButteredCounter = 700;
        LobberProjectiles.Fire(plant, plant.mRow, longButter, false, 0);
        LobberProjectiles.Impact(board.Shots[^1], longButter);
        Check(longButter.mButteredCounter == 700, "longer butter retained");
        var recycled = Enemy(board, 900);
        recycled.OnDamage = () => ++recycled.DataID;
        LobberProjectiles.Fire(plant, plant.mRow, recycled, false, 0);
        LobberProjectiles.Impact(board.Shots[^1], recycled);
        Check(recycled.mButteredCounter == 0, "recycled target cannot inherit butter");
        var friendly = Enemy(board);
        friendly.mMindControlled = true;
        LobberProjectiles.Fire(plant, plant.mRow, friendly, false, 0);
        LobberProjectiles.Impact(board.Shots[^1], friendly);
        Check(friendly.Damage == 0 && !board.Shots[^1].mDead, "friendly target rejected");
        LobberProjectiles.Impact(board.Shots[^1], null);
        Check(board.Shots[^1].mDead, "ground miss releases projectile");

        board = new Board(); plant = Make(board); Enemy(board);
        LobberFusions.Initialize(plant);
        board.Rolls.Enqueue(15); board.Rolls.Enqueue(0);
        for (int i = 0; i < 34; ++i) LobberFusions.Tick(plant);
        Check(board.Shots.Count == 0 && plant.mShootingCounter == 1, "one butter choice before source event");
        int phase = plant.mStartRow, countdown = plant.mLaunchCounter;
        board.mPaused = true; LobberFusions.Tick(plant); board.mPaused = false;
        plant.mIsAsleep = true; LobberFusions.Tick(plant); plant.mIsAsleep = false;
        Check(plant.mStartRow == phase && plant.mLaunchCounter == countdown, "pause and sleep retain saved phase");
        LobberFusions.Tick(plant);
        Check(board.Shots.Count == 1 && LobberProjectiles.Family(board.Shots[0]) == 17,
            "single butter at source animation event");
        for (int i = 0; i < 65; ++i) LobberFusions.Tick(plant);
        Check(board.Shots.Count == 1 && plant.mShootingCounter == 0, "native duplicate shoot suppressed");
        board.FailAllocation = true;
        plant.mShootingCounter = 1;
        LobberProjectiles.Fire(plant, plant.mRow, null, false, 0);
        Check(board.Shots.Count == 1 && !plant.mDead, "allocation failure leaves producer intact");
    }
}
