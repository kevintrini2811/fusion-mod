using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native projectile fields serialize the entire flight. Only this owner moves
// kelp bullets; native pea collision and Torchwood conversion are suppressed.
internal static class KelpProjectiles
{
    private const int Marker = -540020, StraightMarker = -540021;
    internal static bool Contains(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && (shot.mCobTargetX == Marker || shot.mCobTargetX == StraightMarker);
    internal static void Shoot(Plant plant, int row)
    {
        var board = plant.mBoard;
        var shot = board.AddProjectile(plant.mX + 45, board.GetPosYBasedOnRow(plant.mX + 45, row) + 60,
            plant.mRenderOrder + 1, row, ProjectileType.Pea);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = StraightMarker;
        shot.mCobTargetRow = 30;
        shot.mMotionType = ProjectileMotion.Straight;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground | DamageRangeFlags.Submerged;
        shot.mVelX = .06f * ScaredyFusions.SourceUnit;
        float targetY = shot.mPosY;
        shot.mPosY = board.GetPosYBasedOnRow(shot.mPosX, plant.mRow) + 60;
        shot.mVelY = (targetY - shot.mPosY) * .03f;
        shot.mVelZ = shot.mAccZ = 0;
        shot.mPosZ = -20;
        ImportedPeaVisuals.EnsureKelpProjectile(shot);
    }
    internal static void Burst(Plant plant)
    {
        var board = plant.mBoard;
        for (int i = 0; i < 30; ++i)
        {
            var shot = board.AddProjectile(plant.mX + plant.mWidth * .5f
                + (board.Rand(201) - 100) * .001f * ScaredyFusions.SourceUnit,
                plant.mY + plant.mHeight - 10, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
            if (shot == null || shot.mDead) continue;
            shot.mCobTargetX = Marker;
            shot.mCobTargetRow = 20;
            shot.mMotionType = ProjectileMotion.Straight;
            shot.mDamageRangeFlags = DamageRangeFlags.Ground | DamageRangeFlags.Submerged;
            shot.mVelX = (board.Rand(4001) - 2000) * .00001f * ScaredyFusions.SourceUnit;
            shot.mVelY = 0;
            shot.mPosZ = -(board.Rand(201) - 100) * .001f * ScaredyFusions.SourceUnit;
            shot.mVelZ = -(board.Rand(5001) - 2000) * .00001f * ScaredyFusions.SourceUnit;
            shot.mAccZ = .001f * ScaredyFusions.SourceUnit;
            ImportedPeaVisuals.EnsureKelpProjectile(shot);
        }
    }
    internal static void Move(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || !KelpEffects.Playing(board)) return;
        if (!FusionTime.SourceStep(shot, typeof(KelpProjectiles))) return;
        ++shot.mProjectileAge;
        float oldX = shot.mPosX;
        shot.mPosX += shot.mVelX;
        shot.mPosY += board.GetPosYBasedOnRow(shot.mPosX, shot.mRow) - board.GetPosYBasedOnRow(oldX, shot.mRow);
        if (shot.mCobTargetX == StraightMarker)
        {
            shot.mPosY += shot.mVelY;
            shot.mVelY *= .97f;
        }
        shot.mPosZ += shot.mVelZ;
        shot.mVelZ += shot.mAccZ;
        shot.mShadowY = shot.mPosY + 10;
        shot.mX = (int)shot.mPosX;
        shot.mY = (int)(shot.mPosY + shot.mPosZ);
        if (shot.mPosX < -80 || shot.mPosX > 900 || shot.mPosZ > 0) { shot.Die(); return; }
        Zombie? target = null;
        float distance = float.MaxValue;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != shot.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, shot.mDamageRangeFlags)) continue;
            var rect = zombie.GetZombieRect();
            float y = shot.mPosY + shot.mPosZ;
            if (rect.xMax < Math.Min(oldX, shot.mPosX) - 4 || rect.xMin > Math.Max(oldX, shot.mPosX) + 4
                || y < rect.yMin - 4 || y > rect.yMax + 4) continue;
            float next = Math.Abs((rect.xMin + rect.xMax) * .5f - oldX);
            if (next < distance) { target = zombie; distance = next; }
        }
        if (target != null) Impact(shot, target);
    }
    internal static void Impact(Projectile shot, Zombie zombie)
    {
        var board = shot.mBoard;
        if (board == null || shot.mDead || !Contains(shot) || zombie.mBoard?.Pointer != board.Pointer) return;
        uint id = zombie.DataID;
        // Retire first: damage callbacks cannot hit twice with this bullet.
        int damage = shot.mCobTargetX == StraightMarker ? 30 : 20;
        shot.Die();
        zombie.TakeDamage(damage, (DamageFlags)0);
        if (zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer && zombie.mInPool)
            KelpEffects.Add(zombie, 200);
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class KelpProjectileTickPatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (!KelpProjectiles.Contains(__instance)) return true;
        KelpProjectiles.Move(__instance);
        if (!__instance.mDead) ImportedPeaVisuals.EnsureKelpProjectile(__instance);
        return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class KelpProjectileCollisionPatch
{
    private static bool Prefix(Projectile __instance) => !KelpProjectiles.Contains(__instance);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class KelpProjectileTorchPatch
{
    private static bool Prefix(Projectile __instance) => !KelpProjectiles.Contains(__instance);
}
