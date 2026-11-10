// Library boundary double. Summoning behavior is supplied through the library.
namespace FusionMod
{
    internal static class FusionZombies { internal static readonly object Hypno = new(); }
}
namespace ZombieLibrary
{
    internal static class Zombies
    {
        internal static Il2CppReloaded.Gameplay.Zombie? Spawn(object definition, Il2CppReloaded.Gameplay.Board board, int row)
        {
            var zombie = board.AddZombieInRow(Il2CppReloaded.Gameplay.ZombieType.PeaHead, row, -1, false);
            if (zombie == null || zombie.mDead) return zombie;
            zombie.mBodyHealth = zombie.mBodyMaxHealth = 270;
            zombie.mSummonCounter = FusionMod.HypnoShooters.Summoned;
            zombie.mPhaseCounter = 50 + board.Rand(101);
            zombie.StartMindControlled();
            return zombie;
        }
    }
}
