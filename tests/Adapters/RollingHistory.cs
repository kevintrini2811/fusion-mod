using Il2CppReloaded.Gameplay;
namespace FusionMod;
// Stand-in for the board-owned history; clearing transient rolling views must
// not erase it. ActiveNutsTests compiles the real saved GridItem implementation.
internal static class NutProjectiles
{
    private static readonly HashSet<(IntPtr Board, uint Shot, uint Zombie)> History = new();
    internal static bool Remember(Projectile shot, Zombie zombie) =>
        History.Add((shot.mBoard.Pointer, shot.DataID, zombie.DataID));
}
