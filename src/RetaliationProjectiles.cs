using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Star motion already owns arbitrary planar velocity, row changes,
// collision, armor damage, age and serialization. No parallel simulation or
// transient damage table is needed for these ordinary 20-damage projectiles.
internal static class RetaliationProjectiles
{
    internal static bool IsPea(Projectile projectile) =>
        projectile.mMotionType == ProjectileMotion.Star && ScaredyProjectiles.Family(projectile) == 0
        && !GutsyShroom.IsShot(projectile) && !PuffShooters.Contains(projectile)
        && !GlacierShroom.Contains(projectile)
        && !SunflowerFusions.IsShot(projectile) && !PuffSpecials.IsNugget(projectile) && !IronProjectiles.IsSmall(projectile)
        && projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea;
    internal static bool IsStar(Projectile projectile) =>
        projectile.mMotionType == ProjectileMotion.Star && projectile.mProjectileType == ProjectileType.Star;

    internal static Projectile? Spawn(Board board, float x, float y, int order, int row,
        bool star, float degrees, float speed)
    {
        var projectile = board.AddProjectile(x, y, order + 1, row,
            star ? ProjectileType.Star : ProjectileType.Pea);
        if (projectile == null || projectile.mDead) return null;
        float radians = degrees * MathF.PI / 180;
        projectile.mMotionType = ProjectileMotion.Star;
        projectile.mVelX = MathF.Cos(radians) * speed;
        projectile.mVelY = -MathF.Sin(radians) * speed;
        projectile.mVelZ = projectile.mAccZ = projectile.mPosZ = 0;
        // Native UpdateNormalMotion hits land below 23px (Star) / 28px (Pea).
        projectile.mShadowY = projectile.mPosY + PuffShooters.FlightHeight;
        projectile.mDamageRangeFlags = DamageRangeFlags.Ground;
        ImportedPeaVisuals.EnsureRetaliationProjectile(projectile);
        return projectile;
    }

    internal static void Stars(RetaliationNuts.Hit hit) => Stars(hit.Board, hit.X, hit.Y, hit.Order, hit.Row);

    internal static void Stars(Board board, float x, float y, int order, int row)
    {
        // StarNut.TakeDamage/Shoot: angles 30, -30, 180, 90, -90;
        // source speeds 5, 5, 6, 5, 5 cells/s, at 100 native ticks/s.
        foreach (float angle in new[] { 30f, -30f, 180f, 90f, -90f })
            Spawn(board, x, y, order, row, true, angle, angle == 180 ? 4.8f : 4);
    }

    internal static void Burst(SquashFusions.Landing landing)
    {
        // PeaSquash.AttackZombie: 36 angles, three speeds per angle, and
        // 1800 / 90 = 20 damage per pea. Native Pea supplies exactly 20.
        for (int angle = -90; angle < 270; angle += 10)
            for (int ring = 0; ring < 3; ++ring)
                Spawn(landing.Board, landing.X, landing.Y, landing.Order, landing.Row,
                    false, angle, 4 + ring * .8f);
    }

    internal static void AfterCollision(Projectile projectile)
    {
        // Native vertical screen bounds apply only to ProjectileType.Star.
        // Radial peas need those bounds too, especially the vertical rays.
        if (!projectile.mDead && projectile.mMotionType == ProjectileMotion.Star
            && projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
                or ProjectileType.Fireball or ProjectileType.PeashooterFireball
            && (projectile.mPosY < -80 || projectile.mPosY > 650)) projectile.Die();
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class RetaliationProjectileVisualPatch
{
    private static void Prefix(Projectile __instance)
    {
        if (!__instance.mDead && __instance.mMotionType == ProjectileMotion.Star && __instance.mPosZ == 0)
            __instance.mShadowY = __instance.mPosY + PuffShooters.FlightHeight;
    }
    private static void Postfix(Projectile __instance) => ImportedPeaVisuals.EnsureRetaliationProjectile(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class RetaliationProjectileBoundsPatch
{
    internal static void Postfix(Projectile __instance) => RetaliationProjectiles.AfterCollision(__instance);
}
