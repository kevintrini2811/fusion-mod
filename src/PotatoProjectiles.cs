using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The source uses the same potato pea for ordinary fire and all 36 radial
// directions. Keep the native projectile pool, age and rendering lifecycle.
internal static class PotatoProjectiles
{
    private const float Marker = -5331240;
    [ThreadStatic] internal static bool NativeLaneMotion;
    internal static bool Contains(Projectile projectile) => projectile.mProjectileType == ProjectileType.Pea
        && projectile.mCobTargetX == Marker;

    internal static void Spawn(Board board, float x, float y, int row, int order, int angle, int? originRow = null)
    {
        var projectile = board.AddProjectile(x, y, order + 1, row, ProjectileType.Pea);
        if (projectile == null || projectile.mDead) return;
        Forget(projectile);
        projectile.mCobTargetX = Marker;
        float radians = angle * MathF.PI / 180;
        projectile.mMotionType = ProjectileMotion.Straight;
        projectile.mVelX = 4.8f * MathF.Cos(radians);
        projectile.mVelY = 4.8f * MathF.Sin(radians);
        projectile.mPosZ = projectile.mVelZ = projectile.mAccZ = 0;
        // ProjectileInitialize already supplies the native target-lane shadow.
        // Preserve it for Threepeater motion; low custom shadows trigger native
        // pea ground impact before the transition has finished.
        if (originRow.HasValue)
        {
            // Keep the native Threepeater transition and its saved velocity.
            // Physical birth stays at the plant; mRow is the destination lane.
            projectile.mMotionType = ProjectileMotion.Threepeater;
            projectile.mVelY = Math.Sign(row - originRow.Value) * 3;
            projectile.mShadowY -= board.GridToPixelY(0, row) - board.GridToPixelY(0, originRow.Value);
        }
        else projectile.mShadowY = y + 12;
        projectile.mDamageRangeFlags = DamageRangeFlags.Ground;
        ImportedPeaVisuals.RegisterPotatoProjectile(projectile);
    }

    internal static void Move(Projectile projectile)
    {
        var board = projectile.mBoard;
        if (projectile.mDead || board.mPaused) return;
        // Potato motion replaces native collision, so its owner must offer
        // the swept crossing before moving or checking for a zombie hit.
        ChargeTorchFusions.Convert(projectile);
        SupportTorchFusions.Convert(projectile);
        if (!Contains(projectile)) return;
        ImportedPeaVisuals.EnsurePotatoProjectile(projectile);
        float oldX = projectile.mPosX, oldY = projectile.mPosY;
        float x, y;
        if (projectile.mMotionType == ProjectileMotion.Threepeater)
        {
            bool previous = NativeLaneMotion;
            NativeLaneMotion = true;
            try { projectile.UpdateNormalMotion(); }
            finally { NativeLaneMotion = previous; }
            if (projectile.mDead) return;
            x = projectile.mPosX; y = projectile.mPosY;
        }
        else
        {
            x = oldX + projectile.mVelX;
            float slope = board.GetPosYBasedOnRow(x, projectile.mRow) - board.GetPosYBasedOnRow(oldX, projectile.mRow);
            y = oldY + projectile.mVelY + slope;
            projectile.mPosX = x;
            projectile.mPosY = y;
            projectile.mShadowY = y + 12;
        }
        if (x < -80 || x > 900 || y < -80 || y > 700 || projectile.mProjectileAge > 1000)
        { projectile.Die(); return; }
        // A radial pea crosses rows. Test the swept segment against eligible
        // zombie rectangles, then select the first contact along its direction.
        Zombie? target = null;
        float first = float.MaxValue;
        var zombies = board.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
                || projectile.mMotionType == ProjectileMotion.Threepeater && zombie.mRow != projectile.mRow
                || !zombie.EffectedByDamage(DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            if (!Intersection(oldX, oldY, x, y, rect.xMin - 6, rect.yMin - 6,
                rect.xMax + 6, rect.yMax + 6, out float time) || time >= first) continue;
            first = time;
            target = zombie;
        }
        if (target == null) return;
        // Unconverted source BulletType 7 has no piercing or splash.
        var impact = PlantLibrary.ProjectileImpact.Capture(projectile);
        projectile.Die();
        target.TakeDamage(30, (DamageFlags)0);
        PlantLibrary.Projectiles.PlaySound("fusion:Splat", impact.Board);
        PlantLibrary.Projectiles.EmitImpactEffect("fusion:potato-impact", impact);
    }

    internal static bool Intersection(float x, float y, float endX, float endY,
        float left, float top, float right, float bottom, out float time)
    {
        float start = 0, end = 1;
        bool Axis(float origin, float delta, float min, float max)
        {
            if (Math.Abs(delta) < .00001f) return origin >= min && origin <= max;
            float a = (min - origin) / delta, b = (max - origin) / delta;
            start = Math.Max(start, Math.Min(a, b));
            end = Math.Min(end, Math.Max(a, b));
            return start <= end;
        }
        bool hit = Axis(x, endX - x, left, right) && Axis(y, endY - y, top, bottom);
        time = start;
        return hit;
    }

    internal static void Forget(Projectile projectile)
    {
        // Keep the saved marker through Die: native Projectile.Update can
        // still call CheckForCollision after the custom motion killed it.
        // Native Reset clears the marker before the next pool generation.
        ImportedPeaVisuals.ForgetProjectile(projectile);
    }
    internal static void ForgetBoard(Board board)
    {
        for (int i = 0; i < board.m_projectiles.Count; ++i)
            if (board.m_projectiles[i] is { } projectile && Contains(projectile)) Forget(projectile);
    }

}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class PotatoMotionPatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (!PotatoProjectiles.Contains(__instance) || PotatoProjectiles.NativeLaneMotion) return true;
        PotatoProjectiles.Move(__instance);
        return !PotatoProjectiles.Contains(__instance);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class PotatoCollisionPatch
{
    private static bool Prefix(Projectile __instance) => !PotatoProjectiles.Contains(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class PotatoTorchwoodPatch
{
    private static bool Prefix(Projectile __instance) => !PotatoProjectiles.Contains(__instance);
}

[HarmonyPatch]
internal static class PotatoCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), "Die"), AccessTools.Method(typeof(Projectile), "Reset") };
    private static void Prefix(Projectile __instance) => PotatoProjectiles.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class PotatoBoardCleanupPatch
{
    private static void Prefix(Board __instance) => PotatoProjectiles.ForgetBoard(__instance);
}
