using FusionMod;
using Il2CppReloaded.Gameplay;

internal static class CactusNewCases
{
    private static void Check(bool value, string message)
    { if (!value) throw new Exception(message); }
    private static Plant Make(Board board, FusionPlant identity)
    {
        var plant = board.AddPlant(0, 0, identity.NativeType, identity.Marker)!;
        CactusFusions.Initialize(plant);
        return plant;
    }
    private static Zombie Enemy(Board board, bool air = false, int row = 0)
    {
        var zombie = new Zombie { mBoard = board, Flying = air, mRow = row };
        board.m_zombies.Add(zombie);
        return zombie;
    }
    internal static void Run()
    {
        foreach (var (ingredient, result) in new[] {
            (SeedType.Starfruit, FusionPlant.Starthorn),
            (SeedType.Doomshroom, FusionPlant.DoomCactus),
            (SeedType.Plantern, FusionPlant.LumosCactus) })
        {
            Check(FusionRecipes.TryGet(SeedType.Cactus, ingredient, out var a) && a == result, "cactus recipe");
            Check(FusionRecipes.TryGet(ingredient, SeedType.Cactus, out a) && a == result, "reverse cactus recipe");
        }
        var board = new Board { RandomValue = 1 };
        var doom = Make(board, FusionPlant.DoomCactus);
        for (int i = 0; i < 20; ++i) CactusFusions.Fire(doom);
        Check(doom.mShootingCounter == 1 && doom.mDoSpecialCountdown == 0, "twenty failures leave guaranteed next draw");
        CactusFusions.Fire(doom);
        Check(doom.mShootingCounter == 21 && doom.mDoSpecialCountdown == 184, "success resets original 21-entry bag");
        int shots = board.m_projectiles.Count;
        board.mPaused = true;
        CactusFusions.Tick(doom);
        Check(doom.mDoSpecialCountdown == 184, "paused ultimate does not advance");
        board.mPaused = false;
        for (int i = 0; i < 184; ++i) CactusFusions.Tick(doom);
        Check(board.m_projectiles.Count - shots == 90, "complete ultimate emits ninety saved thorns");
        Check(board.m_projectiles.Skip(shots).All(p => p.mVelX > 0
            && Math.Abs(p.mVelY) <= p.mVelX + .001f
            && p.mDamageRangeFlags == (DamageRangeFlags.Ground | DamageRangeFlags.Flying)), "forward cone targets ground and air");
        var first = Enemy(board); var second = Enemy(board); var third = Enemy(board);
        var shot = CactusProjectiles.Spawn(doom)!;
        CactusProjectiles.Impact(shot, first);
        CactusProjectiles.Impact(shot, first);
        Check(first.Damage == 40 && EmberEffects.Has(first) && CactusProjectiles.Remaining(shot) == 2, "one damage and status per target");
        CactusProjectiles.Impact(shot, second);
        CactusProjectiles.Impact(shot, first);
        CactusProjectiles.Impact(shot, third);
        Check(shot.mDead && third.Damage == 40, "third different ground hit consumes thorn");
        shot = CactusProjectiles.Spawn(doom, 45, true)!;
        shot.mPosX = 10; shot.mPosY = 10; shot.mVelX = 20; shot.mVelY = 20;
        Check(CactusProjectiles.Swept(shot, 0, 0, 1, 1), "swept diagonal crosses narrow rectangle");
        Check(!CactusProjectiles.Swept(shot, 40, 0, 42, 2), "diagonal cannot hit distant rectangle");

        board = new(); var light = Make(board, FusionPlant.LumosCactus);
        shot = CactusProjectiles.Spawn(light)!;
        Check(CactusProjectiles.Family(shot) == 6 && CactusProjectiles.Remaining(shot) == 1, "dark spike has separate saved identity");
        Make(board, SeedType.Plantern);
        shot = CactusProjectiles.Spawn(light)!;
        Check(CactusProjectiles.Family(shot) == 4 && CactusProjectiles.Remaining(shot) == 3, "one light adds two pierces");
        Make(board, SeedType.Plantern);
        var ground = Enemy(board); var air = Enemy(board, true);
        light.mTargetX = 1; CactusFusions.Laser(light);
        Check(air.Damage == 90 && ground.Damage == 0, "air laser respects altitude");
        Make(board, SeedType.Plantern);
        CactusFusions.Laser(light);
        Check(air.Damage == 270, "third light doubles laser damage");
        light.mTargetX = 0; CactusFusions.Laser(light);
        Check(ground.Damage == 60 && air.Damage == 270, "ground laser cannot hit air");

        board = new(); var star = Make(board, FusionPlant.Starthorn);
        var balloon = Enemy(board, true, 4); var land = Enemy(board);
        for (int i = 0; i < 43; ++i) CactusFusions.Tick(star);
        Check(board.m_projectiles.Count == 5, "all-board air target triggers five original stars");
        shot = board.m_projectiles[0];
        CactusProjectiles.BeforeMove(shot);
        Check((uint)shot.mTargetZombieID == balloon.DataID, "homing retains complete target handle");
        balloon.Flying = false;
        var replacement = Enemy(board, true, 1);
        CactusProjectiles.BeforeMove(shot);
        Check((uint)shot.mTargetZombieID == replacement.DataID, "landed target is reacquired");
        CactusProjectiles.Impact(shot, land);
        Check(!shot.mDead && land.Damage == 0, "star ignores ground impact");
        CactusProjectiles.Impact(shot, replacement);
        Check(shot.mDead && replacement.Damage == 20, "star damages airborne target exactly once");
        var trapBoard = new Board(); var trap = Make(trapBoard, FusionPlant.Thornweed);
        var nearbyGround = Enemy(trapBoard); nearbyGround.X = 40;
        var nearbyAir = Enemy(trapBoard, true); nearbyAir.X = 40;
        var otherLane = Enemy(trapBoard, true, 1); otherLane.X = 40;
        for (int i = 0; i < 35; ++i) CactusFusions.Tick(trap);
        Check(nearbyGround.Damage == 20 && nearbyAir.Damage == 20 && otherLane.Damage == 0,
            "contact includes ground and air only in native own-row rectangle");
        Check(trapBoard.m_projectiles.Count == 3 && trapBoard.m_projectiles.All(p => p.mVelY < 0),
            "each contact event emits three upward original spikes");
        var trapShot = trapBoard.m_projectiles[1];
        Check(CactusProjectiles.Family(trapShot) == 7 && Math.Abs(trapShot.mVelX) < .001f,
            "middle spike uses saved identity and vertical flight");
        CactusProjectiles.Impact(trapShot, otherLane);
        Check(trapShot.mDead && otherLane.Damage == 20, "upward spike can hit another row once");
        trapBoard.mPaused = true; int phase = trap.mStartRow;
        CactusFusions.Tick(trap);
        Check(trap.mStartRow == phase, "pause preserves trap attack phase");
        board.FailAllocation = true;
        Check(CactusProjectiles.Star(star, 0) == null, "failed star allocation remains safe");
        var ballBoard = new Board(); var ball = Make(ballBoard, FusionPlant.BallCactus);
        foreach (bool flying in new[] { false, true })
        {
            var victim = Enemy(ballBoard, flying);
            var thorn = CactusProjectiles.Spawn(ball)!;
            Check(CactusProjectiles.Family(thorn) == 8 && CactusProjectiles.Remaining(thorn) == 1,
                "Ball Cactus has distinct saved projectile and one impact");
            int effects = ballBoard.mApp.Effects;
            CactusProjectiles.Impact(thorn, victim);
            Check(thorn.mDead && victim.Damage == 30 && victim.Shrinks == 1,
                "Ball Cactus deals thirty damage then shrinks both ground and flying victims");
            Check(victim.Flying == flying && ballBoard.mApp.Effects == effects + 1,
                "Ball Cactus preserves flight and emits one captured impact");
            CactusProjectiles.Impact(thorn, victim);
            Check(victim.Damage == 30 && victim.Shrinks == 1 && ballBoard.mApp.Effects == effects + 1,
                "Reentrant dead projectile cannot repeat damage, shrink or VFX");
        }
        var recycledTarget = Enemy(ballBoard);
        var recycledShot = CactusProjectiles.Spawn(ball)!;
        float hitX = recycledShot.mPosX, hitY = recycledShot.mPosY;
        recycledTarget.OnDamage = () => { ++recycledTarget.DataID; recycledShot.mPosX = 999; recycledShot.mPosY = 999; };
        CactusProjectiles.Impact(recycledShot, recycledTarget);
        Check(recycledTarget.Shrinks == 0, "Damage callback cannot shrink a recycled target generation");
        Check(PlantLibrary.Projectiles.LastImpact!.X == hitX && PlantLibrary.Projectiles.LastImpact.Y == hitY,
            "Impact effect uses pre-damage snapshot even when callbacks recycle the shot");
    }
}
