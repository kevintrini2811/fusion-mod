using FusionMod;
using Il2CppReloaded.Gameplay;

internal static class OffensivePotsCases
{
    private static void Check(bool condition, string message)
    { if (!condition) throw new Exception(message); }

    private static Plant Add(Board board, FusionPlant identity, int col = 0)
    {
        var plant = board.AddPlant(col, 0, identity.NativeType, identity.Marker)!;
        plant.mX = col * 80;
        return plant;
    }

    internal static void Run()
    {
        var board = new Board();
        var pot = Add(board, FusionPlant.BloverPot);
        var pea = Add(board, SeedType.Peashooter);
        var flyer = Add(board, FusionPlant.CamoShroom);
        pea.mPlantHealth = 100;
        board.m_gridItems.Fail = true;
        OffensivePots.Tick(pot);
        Check(!flyer.mDead && pea.mPlantHealth == 100, "failed reservation must preserve flyer and recipient");
        board.m_gridItems.Fail = false;
        pot.mLaunchCounter = 0;
        OffensivePots.Tick(pot);
        Check(flyer.mDead && pea.mPlantHealth == 250, "successful sacrifice heals exactly 150");
        Check(OffensivePots.Bonus(pea) == 20000, "recipient receives 20 percent");
        pot.Die();
        Check(OffensivePots.Bonus(pea) == 20000, "recipient bonus survives pot removal");
        var effect = board.m_gridItems.First(item => !item.mDead && item.mGridX == unchecked((int)pea.DataID));
        int before = effect.mGridItemCounter;
        board.mPaused = true;
        OffensivePots.TickEffect(effect);
        Check(effect.mGridItemCounter == before, "paused recipient clock does not advance");
        board.mPaused = false;
        for (int i = 0; i < 500; ++i) OffensivePots.TickEffect(effect);
        Check(effect.mDead && OffensivePots.Bonus(pea) == 0, "recipient bonus expires after five simulation seconds");

        var scary = Add(board, FusionPlant.ScaredyPot, 1);
        var shroom = Add(board, SeedType.Scaredyshroom, 1);
        scary.mTargetY = 0;
        scary.mLaunchCounter = 100;
        Check(OffensivePots.Bonus(shroom) == 0, "scaredy pot starts with zero efficiency");
        for (int i = 0; i < 100; ++i) OffensivePots.Tick(scary);
        Check(scary.mTargetY == 30000, "recovery gains thirty percentage points each second");
        Check(OffensivePots.Bonus(shroom) == 6600, "25-sun scaredy mushroom receives 22 percent at full efficiency");
        scary.mTargetY = OffensivePots.Full;
        scary.mLaunchCounter = 0;
        OffensivePots.Tick(scary);
        Check(OffensivePots.Bonus(shroom) == 22000, "scaredy multiplier uses twice base plus ten percent");
        board.m_zombies.Add(new Zombie { mBoard = board, X = 80 });
        OffensivePots.Tick(scary);
        Check(scary.mTargetY == 0 && OffensivePots.Bonus(shroom) == 22000, "fear resets efficiency before the next source check");
        for (int i = 0; i < 100; ++i) OffensivePots.Tick(scary);
        Check(OffensivePots.Bonus(shroom) == 0, "one-second check removes frightened recipient bonus");
        board.m_zombies.Clear();

        var shot = new Projectile { mBoard = board };
        board.m_projectiles.Add(shot);
        OffensivePots.Current = new(board, 120000);
        OffensivePots.Capture(shot);
        OffensivePots.Capture(shot);
        OffensivePots.Current = null;
        var saved = board.m_gridItems.Single(item => !item.mDead && item.mGridX == unchecked((int)shot.DataID));
        Check(OffensivePots.ProjectileAttack(shot)?.Factor == 120000, "nested factory hooks never compound projectile bonus");
        // Simulate deserialization by restoring only native saved fields into a
        // fresh backing item. No managed attack context is copied.
        int marker = saved.mSunCount, multiplier = saved.mGridY;
        saved.GridItemDie();
        var loaded = board.m_gridItems.DataArrayAlloc()!;
        loaded.Init(board.mApp, GridItemType.ZenTool);
        loaded.mGridItemState = GridItemState.Normal;
        loaded.mSunCount = marker;
        loaded.mGridX = unchecked((int)shot.DataID);
        loaded.mGridY = multiplier;
        Check(OffensivePots.ProjectileAttack(shot)?.Factor == 120000, "saved projectile bonus needs no managed cache");
        var target = new Zombie { mBoard = board };
        shot.mProjectileType = ProjectileType.Fireball;
        var fireAttack = new OffensivePots.Attack(board, 120000, shot, target);
        Check(OffensivePots.HitDamage(new(target, target.DataID, fireAttack), 40) == 44,
            "Torchwood adds twenty after the attack bonus");
        ++shot.DataID;
        Check(OffensivePots.ProjectileAttack(shot) == null, "recycled projectile does not inherit old bonus");
        Check(OffensivePots.Scale(20, 120000) == 24 && OffensivePots.Scale(int.MaxValue, 160000) == int.MaxValue,
            "damage bonus preserves normal scaling and saturates without overflow");
    }
}
