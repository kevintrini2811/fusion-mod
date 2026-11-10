using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Source armor identities are mapped to native Replanted armor, never compared
// numerically across games. Lost armor no longer grants the damage multiplier.
internal static class MetalArmor
{
    internal static bool MetalEquipment(Zombie zombie) =>
        zombie.mHelmHealth > 0 && zombie.mHelmType is HelmType.Pail or HelmType.Football
        || zombie.mShieldHealth > 0 && zombie.mShieldType is ShieldType.Door or ShieldType.Ladder;

    internal static int Double(int damage, bool applies) => applies
        ? (int)Math.Min(int.MaxValue, Math.Max(0, damage) * 2L) : damage;

    internal static int FumeDamage(Zombie zombie, int damage) =>
        Double(damage, MetalEquipment(zombie) || zombie.mZombieType == ZombieType.Zamboni);

    internal static int RustDamage(Zombie zombie, int damage) => zombie.mHelmHealth > 0
        ? (int)Math.Min(int.MaxValue, Math.Max(0, damage) * 3L) : damage;

    internal static int IronStarDamage(Zombie zombie, int damage) =>
        (int)Math.Min(int.MaxValue, (long)Math.Max(0, damage) + Math.Max(0, zombie.mShieldHealth));

    internal static int StarDamage(Zombie zombie, int damage) => Double(damage, MetalEquipment(zombie));
    internal static int MelonDamage(Zombie zombie, int damage) => Double(damage, zombie.mHelmHealth > 0);
}
