using FusionMod;
using Il2CppReloaded.Gameplay;

static class SeaSupportCases
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static Plant Make(Board board, FusionPlant identity)
    {
        var plant = new Plant { mBoard = board, mSeedType = identity.NativeType, mImitaterType = identity.Marker };
        board.m_plants.Add(plant); SeaSupportFusions.Initialize(plant); return plant;
    }
    static void Tick(Plant plant, int ticks) { for (int i = 0; i < ticks; ++i) SeaSupportFusions.Tick(plant); }
    static Plant Restore(Plant original)
    {
        var restored = new Plant();
        foreach (var field in typeof(Plant).GetFields()) if (!field.IsStatic) field.SetValue(restored, field.GetValue(original));
        return restored;
    }
    internal static void Run()
    {
        foreach (var (ingredient, result) in new[] {
            (SeedType.Plantern, FusionPlant.BeaconShroom), (SeedType.Hypnoshroom, FusionPlant.JellyShroom),
            (SeedType.Iceshroom, FusionPlant.SwellShroom), (SeedType.Scaredyshroom, FusionPlant.HermitShroom) })
        {
            Check(FusionRecipes.TryGet(SeedType.Seashroom, ingredient, out var first) && first == result, "aquatic recipe");
            Check(FusionRecipes.TryGet(ingredient, SeedType.Seashroom, out var second) && second == result, "reverse aquatic recipe");
        }
        var board = new Board(); var beacon = Make(board, FusionPlant.BeaconShroom);
        Tick(beacon, 2999); Check(beacon.mShootingCounter == 0, "light must not grow early");
        board.mPaused = true; Tick(beacon, 50); Check(beacon.mTargetX == 1, "pause preserves growth");
        board.mPaused = false; beacon = Restore(beacon); Tick(beacon, 1);
        Check(beacon.mShootingCounter == 1 && beacon.mTargetY > 0, "restored growth widens light once");
        Tick(beacon, 200); Check(beacon.mShootingCounter == 1 && board.m_projectiles.Count == 0, "mature light has no attack");
        beacon = Make(board, FusionPlant.BeaconShroom); beacon.mStartRow = 23;
        SeaSupportFusions.Fertilize(beacon); Check(beacon.mTargetX == 50 && beacon.mStartRow == 23, "fertilizer caps only attribute timer");
        beacon.mTargetX = 10; SeaSupportFusions.Fertilize(beacon); Check(beacon.mTargetX == 10, "fertilizer never extends short timer");

        board = new Board(); var jelly = Make(board, FusionPlant.JellyShroom); jelly.mPlantHealth = 17;
        Check(SeaFusions.CanFeed(jelly, SeedType.Seashroom), "shared card/glove feeding recognizes Jelly");
        SeaFusions.Feed(jelly); Tick(jelly, 83); Check(board.m_zombies.Count == 0, "no premature summon");
        jelly = Restore(jelly); Tick(jelly, 1);
        Check(board.m_zombies.Count == 1 && board.m_zombies[0].mZombieType == ZombieType.Normal
            && board.m_zombies[0].mMindControlled && jelly.mPlantHealth == 17, "saved event summons one native ally without healing");
        jelly = Restore(jelly); Tick(jelly, 200); Check(board.m_zombies.Count == 1, "load after event cannot replay summon");
        var eater = new Zombie { mBoard = board }; jelly.mIsAsleep = true;
        Check(!SeaSupportFusions.Bite(jelly, eater) && !eater.mMindControlled, "sleeping Jelly has no bite hypnosis");
        jelly.mIsAsleep = false;
        Check(SeaSupportFusions.Bite(jelly, eater) && eater.mMindControlled && jelly.mDead, "bite consumes Jelly and hypnotizes");

        board = new Board(); var swell = Make(board, FusionPlant.SwellShroom);
        var near = new Zombie { mBoard = board, X = 70, Y = 0, mIceTrapCounter = 700 };
        var far = new Zombie { mBoard = board, X = 400, Y = 0 };
        var friendly = new Zombie { mBoard = board, X = 70, Y = 0, mMindControlled = true };
        board.m_zombies.AddRange(new[] { near, far, friendly }); SeaFusions.Feed(swell);
        Tick(swell, 139); Check(near.Damage == 0, "no ice before original event");
        swell = Restore(swell); Tick(swell, 1);
        Check(near.Damage == 20 && near.mIceTrapCounter == 700 && far.Damage == 0 && friendly.Damage == 0,
            "ice retains longer freeze, rejects distant and friendly targets");
        swell = Restore(swell); Tick(swell, 200); Check(near.Damage == 20, "saved ice event never repeats");

        board = new Board(); var hermit = Make(board, FusionPlant.HermitShroom);
        board.m_zombies.Add(new Zombie { mBoard = board, X = 200 });
        Tick(hermit, 1000); Check(board.m_projectiles.Count == 0 && SeaSupportFusions.Low(hermit), "unfed Hermit stays hidden");
        SeaFusions.Feed(hermit); Check(hermit.mTargetX == 1000 && !SeaSupportFusions.Low(hermit), "feeding starts courage");
        Tick(hermit, 184); Check(board.m_projectiles.Count == 0, "feeding and emergence clips finish before shooting");
        Tick(hermit, 31); Check(board.m_projectiles.Count == 1 && SeaProjectiles.Family(board.m_projectiles[0]) == 1,
            "original event creates normal twenty-damage spore");
        Tick(hermit, 100); Check(board.m_projectiles.Count == 3, "Hermit shoots every half-second");
        hermit.Carried = true; int remaining = hermit.mTargetX; Tick(hermit, 100);
        Check(hermit.mTargetX == remaining, "carrying freezes courage and attacks"); hermit.Carried = false;
        SeaSupportFusions.Fertilize(hermit); Check(hermit.mTargetX == 50, "fertilizer shortens active source attribute");
        hermit = Restore(hermit); Tick(hermit, 50);
        Check(hermit.mShootingCounter == 4 && SeaSupportFusions.Low(hermit), "courage expiry starts withdrawal");
        int shots = board.m_projectiles.Count; Tick(hermit, 200); Check(board.m_projectiles.Count == shots, "withdrawn Hermit stops shooting");
        hermit.DataID += 100; SeaSupportFusions.Initialize(hermit);
        Check(hermit.mTargetX == 0 && hermit.mTargetY == 0 && hermit.mShootingCounter == 0, "pool reuse clears all phases");
        Check(!SeaFusions.CanFeed(hermit, SeedType.Peashooter), "unrelated ingredient never feeds Hermit");
        hermit.mDead = true; Check(!SeaFusions.CanFeed(hermit, SeedType.Seashroom), "dead recipients cannot consume food");
    }
}
