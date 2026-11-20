using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native projectile fields serialize the entire flight. Only this owner moves
// kelp bullets; native pea collision and Torchwood conversion are suppressed.
internal static class KelpProjectiles
{
    private const int Marker = -540020, StraightMarker = -540021, FireMarker = -540022, FireStraightMarker = -540023;
    internal static bool Contains(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mCobTargetX is Marker or StraightMarker or FireMarker or FireStraightMarker;
    internal static bool IsFire(Projectile shot) => shot.mCobTargetX is FireMarker or FireStraightMarker;
    private static bool IsStraight(Projectile shot) => shot.mCobTargetX is StraightMarker or FireStraightMarker;
    internal static void Ignite(Projectile shot, int col)
    {
        shot.mCobTargetX = IsStraight(shot) ? FireStraightMarker : FireMarker;
        shot.mCobTargetRow += 20;
        shot.mHitTorchwoodGridX = col;
    }
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
        TorchFusions.IgniteKelp(shot);
        ++shot.mProjectileAge;
        float oldX = shot.mPosX;
        shot.mPosX += shot.mVelX;
        shot.mPosY += board.GetPosYBasedOnRow(shot.mPosX, shot.mRow) - board.GetPosYBasedOnRow(oldX, shot.mRow);
        if (IsStraight(shot))
        {
            shot.mPosY += shot.mVelY;
            shot.mVelY *= .97f;
        }
        shot.mPosZ += shot.mVelZ;
        shot.mVelZ += shot.mAccZ;
        shot.mShadowY = shot.mPosY + 10;
        shot.mX = (int)shot.mPosX;
        shot.mY = (int)(shot.mPosY + shot.mPosZ);
        if (shot.mPosX < -80 || shot.mPosX > 900 || shot.mPosZ > 0)
        {
            if (IsFire(shot) && shot.mPosZ > 0) ShowFireImpact(shot);
            shot.Die();
            return;
        }
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
        bool fire = IsFire(shot);
        var targets = fire ? SplashTargets(shot, zombie) : null;
        uint id = zombie.DataID;
        // Retire first: damage callbacks cannot hit twice with this bullet.
        int damage = shot.mCobTargetRow > 0 ? shot.mCobTargetRow : IsStraight(shot) ? 30 : 20;
        if (fire) ShowFireImpact(shot);
        shot.Die();
        Hit(board, zombie, id, damage, fire);
        if (targets is not { Count: > 0 }) return;
        int splash = Math.Min(damage / 3, Math.Max(1, damage / targets.Count));
        foreach (var (target, generation) in targets)
            if (!target.mDead && !target.IsDeadOrDying()) Hit(board, target, generation, splash, fire: true);
    }

    private static List<(Zombie Zombie, uint Id)> SplashTargets(Projectile shot, Zombie direct)
    {
        var board = shot.mBoard;
        var targets = new List<(Zombie, uint)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.Pointer == direct.Pointer || zombie.mRow != shot.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target || zombie.IsFlying()
                || !SquashFusions.Eligible(board, zombie, shot.mDamageRangeFlags)) continue;
            var rect = zombie.GetZombieRect();
            float dx = shot.mPosX - Math.Clamp(shot.mPosX, rect.xMin, rect.xMax);
            float y = shot.mPosY + shot.mPosZ;
            float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= ScaredyFusions.SourceUnit * ScaredyFusions.SourceUnit) targets.Add((zombie, zombie.DataID));
        }
        return targets;
    }

    private static void Hit(Board board, Zombie zombie, uint id, int damage, bool fire)
    {
        if (zombie.DataID != id || zombie.mBoard?.Pointer != board.Pointer) return;
        zombie.TakeDamage(damage, (DamageFlags)0);
        if (zombie.DataID != id || zombie.mBoard?.Pointer != board.Pointer) return;
        if (zombie.mInPool) KelpEffects.Add(zombie, 200);
        if (fire && zombie.DataID == id && !zombie.mDead) zombie.RemoveColdEffects();
    }

    private static void ShowFireImpact(Projectile shot)
    {
        shot.mBoard.mApp.AddTodParticle(shot.mPosX, shot.mPosY + shot.mPosZ, shot.mRenderOrder, ParticleEffect.FireballDeath);
        shot.mBoard.mApp.SoundSystem.PlayFoley(FoleyType.Splat);
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
