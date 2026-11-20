using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class FrostburnWood
{
    internal static bool Cross(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || shot.mPosZ != 0
            || shot.mMotionType is not (ProjectileMotion.Straight or ProjectileMotion.Threepeater)
            || shot.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea
                or ProjectileType.Snowpea or ProjectileType.Fireball or ProjectileType.PeashooterFireball)
            || shot.mCobTargetX < 0 && shot.mCobTargetX != SupportTorchFusions.Scorch
            || BlazeShooters.Contains(shot)) return false;

        float left = Math.Min(shot.mPosX, shot.mPosX + shot.mVelX);
        float right = Math.Max(shot.mPosX, shot.mPosX + shot.mVelX);
        Plant? crossing = null;
        foreach (var entry in TorchPlantIndex.For(board))
        {
            var plant = entry.Plant;
            if (!entry.IsCurrent(board) || plant.mRow != shot.mRow || plant.mPlantCol == shot.mHitTorchwoodGridX
                || FusionPlant.From(plant) != FusionPlant.FrostburnWood || !BeamShooters.Active(plant)) continue;
            float x = plant.mX + plant.mWidth * .5f;
            if (left > x + 16 || right < x - 16 || Math.Abs(shot.mPosY - plant.mY - plant.mHeight * .5f) > 60) continue;
            if (crossing == null || (shot.mVelX >= 0 ? plant.mPlantCol < crossing.mPlantCol : plant.mPlantCol > crossing.mPlantCol))
                crossing = plant;
        }
        if (crossing == null) return false;
        int col = crossing.mPlantCol;
        if (shot.mProjectileType == ProjectileType.Snowpea)
        {
            int damage = FumeProjectiles.Damage(shot, ShootingPots.ProjectileDamage(shot, 20));
            if (FumeProjectiles.Record(shot, false) is { } boost)
            {
                boost.mGridItemCounter = 0;
                boost.mSeedType = SeedType.None;
            }
            shot.mHitTorchwoodGridX = col;
            PolarProjectiles.Promote(shot, (int)Math.Min(int.MaxValue, 2L * damage));
            return false; // The promoted projectile retains native flight and collision.
        }
        return Replace(shot, col);
    }

    private static bool Replace(Projectile shot, int col)
    {
        var board = shot.mBoard;
        uint id = shot.DataID;
        bool fire = shot.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball;
        var motion = shot.mMotionType;
        float x = shot.mPosX, y = shot.mPosY, vx = shot.mVelX, vy = shot.mVelY, shadow = shot.mShadowY;
        var flags = shot.mDamageRangeFlags;
        bool highGround = shot.mOnHighGround;
        int age = shot.mProjectileAge, modifier = shot.mCobTargetRow, previousPortal = shot.mLastPortalX;
        var boost = fire ? null : FumeProjectiles.Record(shot, false);
        var replacement = board.AddProjectile(x, y, shot.mRenderOrder, shot.mRow,
            fire ? ProjectileType.Pea : ProjectileType.Snowpea);
        if (replacement == null || replacement.mDead) return false;
        if (shot.DataID != id || shot.mDead || shot.mBoard?.Pointer != board.Pointer)
        {
            replacement.Die();
            return true;
        }
        replacement.mMotionType = motion;
        replacement.mPosX = replacement.mX = x;
        replacement.mPosY = replacement.mY = y;
        replacement.mVelX = vx;
        replacement.mVelY = vy;
        replacement.mShadowY = shadow;
        replacement.mDamageRangeFlags = flags;
        replacement.mOnHighGround = highGround;
        replacement.mProjectileAge = age;
        replacement.mHitTorchwoodGridX = col;
        replacement.mLastPortalX = previousPortal;
        if (!fire) replacement.mCobTargetRow = modifier;
        shot.Die();
        if (shot.DataID == id && !shot.mDead)
        {
            replacement.Die();
            return false;
        }
        if (boost is { mDead: false } && unchecked((uint)boost.mGridX) == id)
            boost.mGridX = unchecked((int)replacement.DataID);
        return true;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class FrostburnCrossingPatch
{
    [HarmonyPriority(Priority.First + 950)]
    private static bool Prefix(Projectile __instance, bool __runOriginal) =>
        !__runOriginal || !FrostburnWood.Cross(__instance);
}

[HarmonyPatch]
internal static class FrostburnNativeTorchPatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => new[]
    {
        typeof(Plant).GetMethod(nameof(Plant.UpdateAbilities))!,
        typeof(Plant).GetMethod(nameof(Plant.UpdateTorchwood))!
    };
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.FrostburnWood;
}
