using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Explicit saved Fusion identities, never all native projectiles of a type.
internal static class FusionProjectileDomains
{
    internal static bool CanonicalAge(Projectile shot) => MiniSquashProjectiles.Contains(shot)
        || KelpProjectiles.Contains(shot) || DoomProjectiles.Family(shot) == 4 || CherrySpreader.IsAttached(shot);

    internal static bool CustomMotion(Projectile shot) => PotatoProjectiles.Contains(shot)
        || PotatoSeedProjectiles.Contains(shot) || FumeProjectiles.IsNut(shot) || NutProjectiles.Family(shot) == 3
        || RollingProjectiles.TryShot(shot, out _)
        || SquashLobbers.Family(shot) is not (0 or 7);

    internal static bool TryFlags(Projectile shot, out int flags)
    {
        flags = FusionProjectileTime.Age;
        if (CanonicalAge(shot)) return false;
        if (CustomMotion(shot) || StarProjectiles.Family(shot) == 2) return true;
        bool marked = StarProjectiles.Family(shot) != 0 || SeaProjectiles.Family(shot) != 0
            || CactusProjectiles.Family(shot) != 0 || CattailFusions.Family(shot) != 0
            || PuffLobbers.Family(shot) != 0 || SilverLobbers.Family(shot) != 0 || TriLobbers.Family(shot) != 0
            || LobberProjectiles.Family(shot) != 0 || ElementalLobbers.Family(shot) != 0
            || SquashLobbers.Family(shot) != 0 || NutProjectiles.Family(shot) != 0
            || ScaredyProjectiles.Family(shot) != 0 || DoomProjectiles.Family(shot) != 0
            || AirLobbers.IsShot(shot) || GlacierShroom.Contains(shot) || StarCaps.Contains(shot)
            || PuffShooters.Contains(shot) || PuffSpecials.IsIcicle(shot) || PuffSpecials.IsNugget(shot)
            || SunflowerFusions.IsShot(shot) || GutsyShroom.IsShot(shot) || IronProjectiles.Contains(shot)
            || PolarProjectiles.IsPolar(shot) || SupportFusions.IsSolarShot(shot) || MelonPot.Boosted(shot)
            || CherrySpreader.IsFlying(shot) || CherryShooter.TryShot(shot) || PhoenixProjectiles.Contains(shot)
            || HypnoShooters.Contains(shot) || PoisonShooters.Contains(shot) || BlazeShooters.Contains(shot)
            || ChargeTorchFusions.IsFireCherry(shot) || ChargeTorchFusions.IsLowFire(shot)
            || shot.mCobTargetX is SupportTorchFusions.Scorch or SupportTorchFusions.Garlic or SupportTorchFusions.Stench
            || UmbrellaVariants.ReflectedPea(shot) || UmbrellaVariants.ReflectedMelon(shot);
        if (!marked)
        {
            var saved = FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID);
            if (saved == null) return false;
            if (saved.Flags != 0) { flags = saved.Flags; return true; }
        }
        flags |= shot.mMotionType == ProjectileMotion.Lobbed
            ? FusionProjectileTime.Planar | FusionProjectileTime.Vertical | FusionProjectileTime.Gravity
            : shot.mMotionType is ProjectileMotion.Star or ProjectileMotion.Homing ? FusionProjectileTime.Planar
            : shot.mMotionType == ProjectileMotion.Threepeater ? FusionProjectileTime.FixedStep | FusionProjectileTime.Planar
            // Even literal-X native motion retains velocity vectors for torch
            // crossing and later Star handoffs. Keep those saved fields native.
            : shot.mMotionType == ProjectileMotion.Straight ? FusionProjectileTime.FixedStep | FusionProjectileTime.Planar
            : 0;
        return true;
    }
}
