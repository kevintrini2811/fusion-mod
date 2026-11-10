using Il2CppReloaded.Gameplay;

namespace FusionMod;

// These suites isolate the ordinary Puff/Scaredy families. Hungry-shroom's
// swallow and firing implementation is exercised by FumeFusionsTests instead.
// Fail explicitly if a future test accidentally enters that excluded owner.
internal static class EatingShrooms
{
    internal static void Fear(Plant plant) => throw new NotSupportedException("Use the real EatingShrooms owner in FumeFusionsTests.");
    internal static void ShootHungry(Plant plant) => throw new NotSupportedException("Use the real EatingShrooms owner in FumeFusionsTests.");
}

// Shared projectile marker contract: Glacier projectiles must never receive
// the ordinary retaliation modifier. Its behavior owner remains outside here.
internal static class GlacierShroom
{
    internal static bool Contains(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mCobTargetX is <= -614000 and >= -614002;
}
