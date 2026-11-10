using FusionMod;
using Il2CppReloaded.Gameplay;

internal static class SquashLobberCases
{
    private static void Check(bool value, string message)
    { if (!value) throw new Exception(message); }
    private static Projectile Shot(Board board, int marker, ProjectileType type, int damage = 60) =>
        new() { mBoard = board, mProjectileType = type, mCobTargetX = marker,
            mCobTargetRow = damage, mPosX = 400, mPosY = 230, mRow = 2 };
    internal static void Run()
    {
        foreach (var (a, b, result) in new (FusionPlant, FusionPlant, FusionPlant)[] {
            (SeedType.Cabbagepult, SeedType.Squash, FusionPlant.LettucePult),
            (SeedType.Kernelpult, SeedType.Squash, FusionPlant.ButternutPult),
            (SeedType.Melonpult, SeedType.Squash, FusionPlant.GourdPult),
            (SeedType.Melonpult, SeedType.Fumeshroom, FusionPlant.SpringMelon) })
        {
            Check(FusionRecipes.TryGet(a, b, out var forward) && forward == result, "new recipe");
            Check(FusionRecipes.TryGet(b, a, out var reverse) && reverse == result, "reverse new recipe");
        }
        Check(SquashLobbers.ChargedDamage(3000) == 1920, "maximum Spring charge");
        var board = new Board();
        var plant = new Plant { mBoard = board, mSeedType = FusionPlant.SpringMelon.NativeType,
            mImitaterType = FusionPlant.SpringMelon.Marker, mTargetX = 2000, mTargetY = 100 };
        SquashLobbers.Fertilize(plant);
        Check(plant.mTargetY == 50 && plant.mTargetX == 2000, "fertilizer preserves charge, caps attribute timer");
        var enemy = new Zombie { mBoard = board }; board.m_zombies.Add(enemy);
        var shot = Shot(board, SquashLobbers.Lettuce, ProjectileType.Cabbage);
        SquashLobbers.Land(shot);
        Check(enemy.Damage == 60 && SquashLobbers.Family(shot) == 2, "landing crush and phase");
        board.mPaused = true; SquashLobbers.Motion(shot);
        Check(shot.mHitTorchwoodGridX == 0, "pause holds saved action"); board.mPaused = false;
        for (int i = 0; i < 160; ++i) SquashLobbers.Motion(shot);
        Check(board.Shots.Count == 2 && board.Shots.All(s => s.mCobTargetRow == 30
            && SquashLobbers.Generation(s) == 1), "two first-generation children");
        var final = Shot(board, SquashLobbers.Lettuce - 2, ProjectileType.Cabbage, 15);
        SquashLobbers.Land(final);
        Check(final.mDead && enemy.Damage == 75, "final generation crushes then stops");
        var gourd = Shot(board, SquashLobbers.Gourd, ProjectileType.Melon, 120);
        SquashLobbers.Land(gourd);
        int before = enemy.Damage;
        for (int i = 0; i < 99; ++i) SquashLobbers.Motion(gourd);
        Check(enemy.Damage == before, "Gourd splash waits one second");
        SquashLobbers.Motion(gourd);
        Check(gourd.mDead && enemy.Damage == before + 40, "Gourd deferred splash without direct hit");
        var reused = Shot(board, SquashLobbers.Lettuce, ProjectileType.Cabbage);
        enemy.OnDamage = () => reused.DataID++;
        SquashLobbers.Land(reused);
        Check(board.Shots.Count == 2, "reused slot never emits children during damage");
        enemy.OnDamage = null;
        board.Water = true;
        var water = Shot(board, SquashLobbers.Lettuce, ProjectileType.Cabbage);
        before = enemy.Damage; SquashLobbers.Land(water);
        Check(water.mDead && enemy.Damage == before, "lettuce sinks before crush");
    }
}
