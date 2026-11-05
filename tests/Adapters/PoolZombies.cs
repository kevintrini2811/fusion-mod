using Il2CppReloaded.Gameplay;
namespace ZombieLibrary;
// This suite exercises pool patches with Fusion's registered native rig set.
internal static class Zombies
{
    internal sealed record Definition(bool RubberDuck);
    internal static Definition? Find(Zombie zombie) => null;
    internal static bool HasPoolType(ZombieType type) => HasNativeDuck(type);
    internal static bool HasNativeDuck(ZombieType type) => type is ZombieType.Football or ZombieType.JackInTheBox
        or ZombieType.Dancer or ZombieType.BackupDancer or ZombieType.Yeti or ZombieType.Gargantuar
        or ZombieType.RedeyeGargantuar or ZombieType.SquashHead;
}
