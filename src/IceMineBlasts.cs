using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// IceMine.Freeze spreads between neighboring zombie cells every 150 ms.
// Each explosion has its own visited set: a zombie takes 600 damage at most
// once from that explosion, while separate mines remain independent.
internal static class IceMineBlasts
{
    private sealed class Wave
    {
        internal Board Board = null!;
        internal double NextTick;
        internal int Order;
        internal readonly HashSet<(IntPtr Pointer, uint Id)> Visited = new();
        internal List<(Zombie Zombie, uint Id)> Frontier = new();
    }
    private static readonly List<Wave> Waves = new();

    private static bool Eligible(Board board, Zombie zombie) => zombie.mBoard?.Pointer == board.Pointer
        && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled
        && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target)
        && zombie.EffectedByDamage(DamageRangeFlags.Ground | DamageRangeFlags.Flying
            | DamageRangeFlags.Submerged | DamageRangeFlags.Dog | DamageRangeFlags.OffGround
            | DamageRangeFlags.Underground);

    private static int Column(Board board, Zombie zombie) =>
        board.PixelToGridX((int)(zombie.GetZombieRect().xMin + zombie.GetZombieRect().xMax) / 2,
            (int)zombie.mPosY);

    internal static void Start(Board board, int col, int row, int order)
    {
        var wave = new Wave { Board = board, Order = order,
            NextTick = FusionTime.ElapsedSeconds(board) + .15 };
        wave.Frontier = Hit(wave, new[] { (col, row) });
        if (wave.Frontier.Count > 0) Waves.Add(wave);
    }

    private static List<(Zombie Zombie, uint Id)> Hit(Wave wave, IEnumerable<(int Col, int Row)> centers)
    {
        var cells = centers.Distinct().ToArray();
        var board = wave.Board;
        var targets = new List<(Zombie Zombie, uint Id)>();
        // Snapshot before any damage callback can mutate a native DataArray.
        var zombies = board.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || !Eligible(board, zombie)
                || wave.Visited.Contains((zombie.Pointer, zombie.DataID))) continue;
            int col = Column(board, zombie);
            if (cells.Any(cell => Math.Abs(cell.Col - col) <= 1 && Math.Abs(cell.Row - zombie.mRow) <= 1))
            {
                wave.Visited.Add((zombie.Pointer, zombie.DataID));
                targets.Add((zombie, zombie.DataID));
            }
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !Eligible(board, zombie)) continue;
            FusionStatuses.Freeze(zombie);
            if (zombie.DataID != id || !Eligible(board, zombie)) continue;
            zombie.TakeDamage(600, DamageFlags.HitsShieldAndBody);
        }
        foreach (var (col, row) in cells)
            board.mApp.AddTodParticle(board.GridToPixelX(col, row) + 40,
                board.GridToPixelY(col, row) + 40, wave.Order, ParticleEffect.IceSparkle);
        return targets;
    }

    internal static void Update(Board board)
    {
        if (board.mPaused || board.Pointer == CherryBombPlants.DisposingBoard
            || board.mApp == null || board.mApp.GameScene != GameScenes.Playing) return;
        // Hit can trigger another mine. Process a snapshot to defer new waves.
        foreach (var wave in Waves.Where(w => w.Board.Pointer == board.Pointer).ToArray())
        {
            if (FusionTime.ElapsedSeconds(board) + 1e-9 < wave.NextTick) continue;
            var centers = wave.Frontier.Where(t => t.Zombie.DataID == t.Id && Eligible(board, t.Zombie))
                .Select(t => (Column(board, t.Zombie), t.Zombie.mRow)).ToArray();
            wave.Frontier = Hit(wave, centers);
            wave.NextTick += .15;
            if (wave.Frontier.Count == 0) Waves.Remove(wave);
        }
    }

    internal static void ForgetBoard(Board board) => Waves.RemoveAll(w => w.Board.Pointer == board.Pointer);
    internal static void Clear() => Waves.Clear();
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class IceMineWavePatch
{
    private static void Postfix(Board __instance) => IceMineBlasts.Update(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class IceMineWaveCleanupPatch
{
    private static void Prefix(Board __instance) => IceMineBlasts.ForgetBoard(__instance);
}
