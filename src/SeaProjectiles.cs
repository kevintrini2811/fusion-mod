using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native arbitrary Star motion, collision, shield damage and balloon popping
// remain responsible. The saved marker identifies spores versus air spikes.
internal static class SeaProjectiles
{
    private const int Marker = -560000;
    internal static int Family(Projectile shot) => shot.mCobTargetX switch
    {
        Marker - 1 when shot.mProjectileType == ProjectileType.Pea => 1,
        Marker - 2 when shot.mProjectileType == ProjectileType.Pea => 2,
        Marker - 3 when shot.mProjectileType == ProjectileType.Spike => 3,
        _ => 0
    };
    internal static Projectile? Spawn(Plant plant, int family, float offset)
    {
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f + 15,
            plant.mY + plant.mHeight / 2f + offset, plant.mRenderOrder + 1, plant.mRow,
            family == 3 ? ProjectileType.Spike : ProjectileType.Pea);
        if (shot == null || shot.mDead) return null;
        shot.mCobTargetX = Marker - family;
        shot.mCobTargetRow = 20;
        shot.mHitTorchwoodGridX = plant.mRow;
        shot.mMotionType = ProjectileMotion.Star;
        shot.mVelX = 6 * SeaFusions.Unit / 100;
        shot.mVelY = shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
        shot.mDamageRangeFlags = family == 3 ? DamageRangeFlags.Flying : DamageRangeFlags.Ground;
        shot.mTargetZombieID = (ZombieID)(SeaFusions.Target(plant, family == 3)?.DataID ?? 0);
        ImportedPeaVisuals.EnsureSeaProjectile(shot);
        return shot;
    }

    internal static void BeforeMove(Projectile shot)
    {
        int family = Family(shot);
        if (family == 0 || shot.mDead || shot.mBoard.mPaused) return;
        if (family == 2)
        {
            Zombie? target = null;
            float nearest = float.MaxValue;
            for (int i = 0; i < shot.mBoard.m_zombies.Count; ++i)
            {
                var zombie = shot.mBoard.m_zombies[i];
                if (zombie == null || zombie.mRow != shot.mHitTorchwoodGridX || zombie.IsFlying()
                    || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                    || !SquashFusions.Eligible(shot.mBoard, zombie, DamageRangeFlags.Ground)) continue;
                var rect = zombie.GetZombieRect();
                float d = (rect.xMin + rect.xMax) / 2 - shot.mPosX;
                if (d >= -20 && d < nearest) { target = zombie; nearest = d; }
                if (zombie.DataID == (uint)shot.mTargetZombieID) { target = zombie; break; }
            }
            if (target != null)
            {
                shot.mTargetZombieID = (ZombieID)target.DataID;
                var rect = target.GetZombieRect();
                float dx = (rect.xMin + rect.xMax) / 2 - shot.mPosX;
                float dy = (rect.yMin + rect.yMax) / 2 - shot.mPosY;
                float length = MathF.Sqrt(dx * dx + dy * dy);
                if (length > .001f)
                {
                    float speed = 6 * SeaFusions.Unit / 100;
                    shot.mVelX = dx / length * speed;
                    shot.mVelY = dy / length * speed;
                }
            }
        }
        // Native Star motion uses this height for collision, not a second
        // visual motion owner. Keep small spores above the pea ground cutoff.
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class SeaProjectileMotionPatch
{ private static void Prefix(Projectile __instance) => SeaProjectiles.BeforeMove(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class SeaProjectileVisualPatch
{
    private static void Postfix(Projectile __instance)
    { if (!__instance.mDead && SeaProjectiles.Family(__instance) != 0) ImportedPeaVisuals.EnsureSeaProjectile(__instance); }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class SeaProjectileIgnitionPatch
{ private static bool Prefix(Projectile __instance) => SeaProjectiles.Family(__instance) == 0; }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToPea))]
internal static class SeaProjectileThawPatch
{ private static bool Prefix(Projectile __instance) => SeaProjectiles.Family(__instance) == 0; }
