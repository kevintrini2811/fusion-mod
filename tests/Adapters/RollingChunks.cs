using Il2CppReloaded.Gameplay;
namespace FusionMod;
// Actual rolling reconstruction is covered by ChomperFusionsTests.
internal static class RollingProjectiles
{
    internal static int Chunk(Projectile shot) => shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
        && shot.mCobTargetX is >= -31103 and <= -31101 ? -(int)shot.mCobTargetX - 31100 : 0;
    internal static bool TryShot(Projectile shot, out object? state) { state = null; return false; }
}
