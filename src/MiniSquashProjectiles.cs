using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Cob marker, damage, velocity, last DataID and contact timeout are native
// serialized projectile fields. No transient gameplay collection is needed.
internal static class MiniSquashProjectiles
{
    private const int Marker = -540500;
    internal static bool Contains(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mCobTargetX == Marker;
    internal static void Spawn(Plant plant, int row)
    {
        var board = plant.mBoard;
        var shot = board.AddProjectile(plant.mX + 45,
            board.GetPosYBasedOnRow(plant.mX + 45, row) + 60,
            plant.mRenderOrder + 1, row, ProjectileType.Pea);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = Marker;
        shot.mCobTargetRow = 40;
        shot.mTargetZombieID = 0;
        shot.mHitTorchwoodGridX = 0;
        shot.mMotionType = ProjectileMotion.Straight;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground;
        shot.mVelX = (1.8f + board.Rand(201) * .001f) * ScaredyFusions.SourceUnit * .01f;
        shot.mVelY = shot.mVelZ = 0;
        shot.mPosZ = -30;
        shot.mAccZ = .001f * ScaredyFusions.SourceUnit;
        ImportedPeaVisuals.EnsureMiniSquashProjectile(shot);
    }

    internal static void Area(Board board, int row, float x, float y, int damage, int order, float sourceRadius = .5f)
    {
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != row
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Submerged)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            float radius = sourceRadius * ScaredyFusions.SourceUnit;
            if (dx * dx + dy * dy <= radius * radius)
                targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Submerged))
                zombie.TakeDamage(damage, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
        // Mini-squash landings cause damage without a full-screen explosion sprite.
    }

    internal static void Move(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || !KelpEffects.Playing(board)) return;
        uint ownerId = shot.DataID;
        float oldX = shot.mPosX;
        ++shot.mProjectileAge;
        shot.mPosX += shot.mVelX;
        shot.mPosY += board.GetPosYBasedOnRow(shot.mPosX, shot.mRow) - board.GetPosYBasedOnRow(oldX, shot.mRow);
        shot.mVelZ += shot.mAccZ;
        shot.mPosZ += shot.mVelZ;
        shot.mX = (int)shot.mPosX;
        shot.mY = (int)(shot.mPosY + shot.mPosZ);
        shot.mShadowY = shot.mPosY + 10;
        if (shot.mPosX > 900 || shot.mPosX < -80) { shot.Die(); return; }
        if (shot.mPosZ >= 0 && shot.mVelZ > 0)
        {
            shot.mPosZ = 0;
            int col = board.PixelToGridX(shot.mPosX, shot.mPosY);
            if (col >= 0 && col < 9 && board.IsPoolSquare(col, shot.mRow)) { shot.Die(); return; }
            // JumpUpdate resets upward velocity before JumpLand/AttackZombie.
            shot.mVelZ = -.04f * ScaredyFusions.SourceUnit;
            Area(board, shot.mRow, shot.mPosX, shot.mPosY, 40, shot.mRenderOrder);
            if (shot.DataID != ownerId || shot.mDead || shot.mBoard?.Pointer != board.Pointer) return;
        }
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != shot.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float y = shot.mPosY + shot.mPosZ;
            if (rect.xMax < oldX - 8 || rect.xMin > shot.mPosX + 8 || y < rect.yMin - 8 || y > rect.yMax + 8) continue;
            if (!NutProjectiles.Remember(shot, zombie, 15)) continue;
            // Commit the filter and bounce before invoking damage callbacks.
            shot.mTargetZombieID = (ZombieID)zombie.DataID;
            shot.mHitTorchwoodGridX = shot.mProjectileAge + 15;
            shot.mVelX = .025f * ScaredyFusions.SourceUnit;
            shot.mVelZ = 0;
            zombie.TakeDamage(40, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
            break;
        }
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class MiniSquashUpdatePatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (!MiniSquashProjectiles.Contains(__instance)) return true;
        MiniSquashProjectiles.Move(__instance);
        if (!__instance.mDead) ImportedPeaVisuals.EnsureMiniSquashProjectile(__instance);
        return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class MiniSquashCollisionPatch
{ private static bool Prefix(Projectile __instance) => !MiniSquashProjectiles.Contains(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class MiniSquashTorchPatch
{ private static bool Prefix(Projectile __instance) => !MiniSquashProjectiles.Contains(__instance); }
