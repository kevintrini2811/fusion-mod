using Il2CppReloaded.Gameplay;

namespace FusionMod;

// These suites isolate rolling, chomper and mine behavior with no torch plants present.
// TorchFusionsTests compiles the real converters and verifies their integration
// with RollingProjectiles.Move, including ignition and saved damage markers.
internal static class TorchweedFusions
{
    internal static void Convert(Projectile projectile) { }
}

internal static class ChargeTorchFusions
{
    internal static void Convert(Projectile projectile) { }
}
