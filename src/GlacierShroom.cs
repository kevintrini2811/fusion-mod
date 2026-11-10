using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Fume owns sleep, carrying and animation. Native projectiles own flight/pools;
// saved markers and two full target handles own the three distinct contacts.
internal static class GlacierShroom
{
    internal const int Marker = -614000;
    internal static bool Contains(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mCobTargetX is <= Marker and >= Marker - 2;
    internal static void Tick(Plant plant)
    {
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || !FumeFusions.HasTarget(plant, FusionPlant.GlacierShroom)) return;
            plant.mStartRow = 100;
            plant.mLaunchCounter = 300;
        }
        // Four original AnimShoot events, rounded up to simulation ticks.
        if (100 - plant.mStartRow is 42 or 55 or 69 or 84) Fire(plant);
        --plant.mStartRow;
    }
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
        shot.mVelX = 6 * FumeVariants.Unit / 100;
        shot.mVelY = shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground;
        ImportedPeaVisuals.EnsureGlacierProjectile(shot);
    }
    private static bool Eligible(Board board, Zombie zombie) => zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground);
    internal static void Collide(Projectile shot)
    {
        if (shot.mDead || shot.mBoard.mPaused) return;
        if (shot.mPosX > 900 || shot.mPosX < -80) { shot.Die(); return; }
        var board = shot.mBoard;
        uint shotId = shot.DataID;
        var targets = new List<(Zombie Zombie, uint Id, float X)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != shot.mRow || !Eligible(board, zombie)) continue;
            var rect = zombie.GetZombieRect();
            if (rect.xMin <= shot.mPosX + Math.Max(12, Math.Abs(shot.mVelX)) && rect.xMax >= shot.mPosX - 12)
                targets.Add((zombie, zombie.DataID, rect.xMin));
        }
        foreach (var (direct, id, _) in targets.OrderBy(target => target.X))
        {
            if (shot.mDead || shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer) return;
            if (direct.DataID != id || !Eligible(board, direct) || (uint)shot.mTargetZombieID == id
                || unchecked((uint)shot.mHitTorchwoodGridX) == id) continue;
            int contacts = Marker - (int)shot.mCobTargetX + 1;
            shot.mHitTorchwoodGridX = unchecked((int)(uint)shot.mTargetZombieID);
            shot.mTargetZombieID = (ZombieID)id;
            var impact = PlantLibrary.ProjectileImpact.Capture(shot);
            int damage = FumeProjectiles.Damage(shot, shot.mCobTargetRow);
            float x = shot.mPosX, y = shot.mPosY;
            if (contacts == 3) shot.Die(); else --shot.mCobTargetX;
            // The executable uses a 1.5-unit radius, unlike its almanac's one.
            // Freeze points precede splash; splash includes the direct target.
            SnowPea.Hit(direct, 5);
            var splash = new List<(Zombie Zombie, uint Id)>();
            for (int i = 0; i < board.m_zombies.Count; ++i)
            {
                var zombie = board.m_zombies[i];
                if (zombie == null || !Eligible(board, zombie)) continue;
                var rect = zombie.GetZombieRect();
                float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
                if (dx * dx + dy * dy <= 2.25f * FumeVariants.Unit * FumeVariants.Unit)
                    splash.Add((zombie, zombie.DataID));
            }
            foreach (var (zombie, targetId) in splash)
                if (zombie.DataID == targetId && Eligible(board, zombie))
                { FusionStatuses.Chill(zombie); zombie.TakeDamage(damage / 2, DamageFlags.BypassesShield); }
            if (direct.DataID == id && Eligible(board, direct))
            {
                int multiplier = direct.mIceTrapCounter > 0 ? 4 : 1;
                direct.TakeDamage(damage * multiplier, DamageFlags.BypassesShield);
                if (direct.DataID == id && Eligible(board, direct)) FusionStatuses.Chill(direct);
            }
            PlantLibrary.Projectiles.EmitImpactEffect("fusion:glacier-impact", impact);
            PlantLibrary.Projectiles.PlaySound("fusion:Splat", board);
            if (contacts == 3) return;
        }
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class GlacierCollisionPatch
{
    private static bool Prefix(Projectile __instance)
    { if (!GlacierShroom.Contains(__instance)) return true; GlacierShroom.Collide(__instance); return false; }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class GlacierImpactPatch
{ private static bool Prefix(Projectile __instance) => !GlacierShroom.Contains(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class GlacierIgnitionPatch
{ private static bool Prefix(Projectile __instance) => !GlacierShroom.Contains(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class GlacierVisualPatch
{
    private static void Postfix(Projectile __instance)
    { if (!__instance.mDead && GlacierShroom.Contains(__instance)) ImportedPeaVisuals.EnsureGlacierProjectile(__instance); }
}
