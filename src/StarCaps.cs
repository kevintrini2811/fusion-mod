using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The native projectile moves the cap. Saved fields own all three contacts and
// the two previous handles, so loading never repeats a burst on the same target.
internal static class StarCaps
{
    private const int Marker = -556000;
    internal static bool Contains(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mCobTargetX is <= Marker and >= Marker - 2;

    internal static void Fire(Plant plant)
    {
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f + 20,
            plant.mY + plant.mHeight / 2f, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = Marker;
        shot.mCobTargetRow = 20;
        shot.mTargetZombieID = 0;
        shot.mHitTorchwoodGridX = 0;
        shot.mMotionType = ProjectileMotion.Star;
        shot.mVelX = 12 * (80f / 1.35f) / 100;
        shot.mVelY = shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        // Native UpdateNormalMotion hits land below 23px (Star) / 28px (Pea).
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground;
        ImportedPeaVisuals.EnsureStarCapProjectile(shot);
    }

    internal static void Collide(Projectile shot)
    {
        if (shot.mDead || shot.mBoard.mPaused) return;
        var board = shot.mBoard;
        uint shotId = shot.DataID;
        var targets = new List<(Zombie Zombie, uint Id, float X)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != shot.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            if (rect.xMin <= shot.mPosX + Math.Max(12, Math.Abs(shot.mVelX))
                && rect.xMax >= shot.mPosX - 12) targets.Add((zombie, zombie.DataID, rect.xMin));
        }
        foreach (var (zombie, id, _) in targets.OrderBy(target => target.X))
        {
            if (shot.mDead || shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer) return;
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)
                || (uint)shot.mTargetZombieID == id || unchecked((uint)shot.mHitTorchwoodGridX) == id) continue;
            // Commit contact before allocating children. No direct cap damage.
            int contacts = Marker - (int)shot.mCobTargetX + 1;
            shot.mHitTorchwoodGridX = unchecked((int)(uint)shot.mTargetZombieID);
            shot.mTargetZombieID = (ZombieID)id;
            float x = shot.mPosX, y = shot.mPosY;
            int row = shot.mRow, order = shot.mRenderOrder, damage = FumeProjectiles.Damage(shot, shot.mCobTargetRow);
            if (contacts == 3) shot.Die();
            else --shot.mCobTargetX;
            foreach (float angle in StarProjectiles.Angles)
                StarProjectiles.Spawn(board, x, y, order, row, 5, angle, 6 * (80f / 1.35f) / 100, damage);
            if (contacts == 3) return;
        }
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class StarCapCollisionPatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (!StarCaps.Contains(__instance)) return true;
        StarCaps.Collide(__instance); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class StarCapImpactPatch
{ private static bool Prefix(Projectile __instance) => !StarCaps.Contains(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class StarCapIgnitionPatch
{ private static bool Prefix(Projectile __instance) => !StarCaps.Contains(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class StarCapVisualPatch
{
    private static void Postfix(Projectile __instance)
    { if (!__instance.mDead && StarCaps.Contains(__instance)) ImportedPeaVisuals.EnsureStarCapProjectile(__instance); }
}
