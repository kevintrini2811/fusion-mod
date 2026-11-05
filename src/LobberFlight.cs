using Il2CppReloaded.Gameplay;

namespace FusionMod;

// One native ballistic solver serves every imported lobber family.
internal static class LobberFlight
{
    internal static void Launch(Projectile shot, Zombie? target, int ticks)
    {
        float x = shot.mBoard.GridToPixelX(8, shot.mRow) + 80;
        float y = shot.mBoard.GridToPixelY(0, shot.mRow) + 30;
        if (target != null)
        {
            var rect = target.GetZombieRect();
            float velocity = target.mButteredCounter > 0 || target.mIceTrapCounter > 0 || target.mIsEating ? 0
                : target.mVelX * (target.IsWalkingBackwards() ? 1 : -1);
            if (target.IsMovingAtChilledSpeed()) velocity *= .5f;
            x = (rect.xMin + rect.xMax) / 2 + velocity * ticks;
            y = (rect.yMin + rect.yMax) / 2;
        }
        LaunchAt(shot, x, y, ticks);
    }

    internal static void LaunchAt(Projectile shot, float x, float y, int ticks)
    {
        shot.mMotionType = ProjectileMotion.Lobbed;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground;
        shot.mTargetZombieID = 0;
        // These Cabbage/Butter/Melon shots never use native pea conversion.
        // Preserve the second previous handle as well as the most recent one.
        shot.mHitTorchwoodGridX = 0;
        shot.mVelX = (x - shot.mPosX) / ticks;
        shot.mVelY = (y - shot.mPosY) / ticks;
        shot.mAccZ = PuffLobbers.Gravity;
        shot.mPosZ = -PuffShooters.FlightHeight;
        // Native Cabbage/Kernel/Butter/Melon impact is tested at -35/-30
        // (and -32 for the special melon type). Solve the actual collision
        // height instead of hitting zero after the target has been passed.
        float impactZ = shot.mProjectileType is ProjectileType.Cabbage or ProjectileType.Butter ? -35 : -30;
        shot.mVelZ = (PuffShooters.FlightHeight + impactZ) / ticks
            - PuffLobbers.Gravity * (ticks + 1) / 2;
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
    }

}
