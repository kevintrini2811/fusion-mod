using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Puff owns small slots, coffee, terrain and saving. These counters replace
// its shooter, just as for the other small imported shooters.
internal static class PuffLobbers
{
    internal const int ShootTicks = 100;
    internal const int CabbageShot = -481000, KernelShot = -482000,
        ButterShot = -483000, MelonShot = -484000;
    internal const float Gravity = 9.8f * ScaredyFusions.SourceUnit / 10000;
    internal const int FlightTicks = 150;

    internal static int Family(Projectile shot) => shot.mProjectileType switch
    {
        ProjectileType.Cabbage when shot.mCobTargetX <= CabbageShot && shot.mCobTargetX >= CabbageShot - 5 => 1,
        ProjectileType.Kernel when shot.mCobTargetX == KernelShot => 2,
        ProjectileType.Butter when shot.mCobTargetX == ButterShot => 3,
        // Cabbage collision supplies a single direct hit, without melon splash.
        ProjectileType.Cabbage when shot.mCobTargetX == MelonShot => 4,
        _ => 0
    };

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsPuffLobber(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = 300;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = 0;
    }

    internal static Zombie? Target(Plant plant, int? row = null)
    {
        Zombie? nearest = null, unshrunk = null;
        float distance = float.MaxValue, unshrunkDistance = float.MaxValue;
        float x = plant.mX + plant.mWidth / 2f;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != (row ?? plant.mRow)
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float d = (rect.xMin + rect.xMax) / 2 - x;
            if (d < 0) continue;
            if (d < distance) { nearest = zombie; distance = d; }
            if (!PuffSupport.IsSmall(zombie) && d < unshrunkDistance)
            { unshrunk = zombie; unshrunkDistance = d; }
        }
        return FusionPlant.From(plant) == FusionPlant.BabyCorn ? unshrunk ?? nearest : nearest;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(PuffLobbers))) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || Target(plant) == null) return;
            plant.mLaunchCounter = 285 + plant.mBoard.Rand(31);
            plant.mStartRow = ShootTicks;
            // Cornpult selects the butter animation once per attack. Preserve
            // that selection in a serialized field, including during loads.
            plant.mShootingCounter = FusionPlant.From(plant) == FusionPlant.BabyCorn
                && plant.mBoard.Rand(PotBoosts.ButterRange(plant)) == 0 ? 1 : 0;
        }
        if (ShootTicks - plant.mStartRow == 61)
        {
            var target = Target(plant);
            if (target != null) Fire(plant, target);
        }
        if (--plant.mStartRow == 0) plant.mShootingCounter = 0;
    }

    internal static void Fire(Plant plant, Zombie target)
    {
        var identity = FusionPlant.From(plant);
        bool corn = identity == FusionPlant.BabyCorn;
        bool butter = corn && plant.mShootingCounter == 1;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, plant.mRow,
            corn ? butter ? ProjectileType.Butter : ProjectileType.Kernel : ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = corn ? butter ? ButterShot : KernelShot
            : identity == FusionPlant.SlicePult ? MelonShot : CabbageShot;
        LobberFlight.Launch(shot, target, PuffLobbers.FlightTicks);
        ImportedPeaVisuals.EnsurePuffLobProjectile(shot);
    }

    internal static void Impact(Projectile shot, Zombie? zombie)
    {
        int family = Family(shot);
        var board = shot.mBoard;
        if (family == 0 || shot.mDead || board == null) return;
        uint shotId = shot.DataID;
        if (zombie == null) { shot.Die(); return; }
        if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mBoard?.Pointer != shot.mBoard?.Pointer) return;
        if (family == 1 && (uint)shot.mTargetZombieID == zombie.DataID) return;
        bool small = PuffSupport.IsSmall(zombie);
        uint targetId = zombie.DataID;
        int state = family == 1 ? CabbageShot - (int)shot.mCobTargetX : 0;
        bool spread = state >= 3;
        int hits = state % 3;
        if (family == 1)
        {
            if (small) spread = true;
            else if (spread) Shrink(zombie);
        }
        if (zombie.DataID != targetId || zombie.mBoard?.Pointer != board.Pointer) return;
        int damage = family == 2 ? 20 : family == 4 ? small ? 160 : 80 : 40;
        // Source thrown damage bypasses hand-held shields; native body/helmet
        // damage, death and rewards still run through the native health writer.
        zombie.TakeDamage(damage, DamageFlags.BypassesShield);
        if (shot.mDead || shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer) return;
        if (family == 3 && zombie.DataID == targetId && zombie.mBoard?.Pointer == board.Pointer
            && !zombie.mDead && !zombie.IsDeadOrDying())
        {
            if (!small) Shrink(zombie);
            else
            {
                FusionStatuses.Butter(zombie, 150);
            }
        }
        board.mApp.AddTodParticle(shot.mPosX, shot.mPosY + shot.mPosZ,
            shot.mRenderOrder, family == 4 ? ParticleEffect.MelonSplash
                : family == 1 ? ParticleEffect.CabbageSplat : ParticleEffect.ButterSplat);
        if (family != 1 || hits == 2) { shot.Die(); return; }
        shot.mCobTargetX = CabbageShot - (hits + 1) - (spread ? 3 : 0);
        shot.mTargetZombieID = (ZombieID)targetId;
        // Original Bullet_cabbage_small: horizontal speed * .4, reflected
        // vertical speed * .5, three total hits; landing ends the projectile.
        shot.mVelX *= .4f;
        shot.mVelZ = -Math.Max(FusionProjectileTime.SourceVelocity(Gravity), Math.Abs(shot.mVelZ) * .5f);
    }

    internal static void Shrink(Zombie zombie)
    {
        // Actual ImpZombie.BeSmall overrides the size/health operation with
        // immediate death. Both butter and an infected cabbage dispatch it.
        if (zombie.mZombieType == ZombieType.Imp) zombie.DieWithLoot();
        else PuffSupport.Shrink(zombie);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PuffLobInitializePatch
{
    internal static void Postfix(Plant __instance) => PuffLobbers.Initialize(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class PuffLobImpactPatch
{
    internal static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (PuffLobbers.Family(__instance) == 0) return true;
        PuffLobbers.Impact(__instance, theZombie);
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.FindCollisionTarget))]
internal static class PuffLobCollisionPatch
{
    internal static void Postfix(Projectile __instance, ref Zombie __result)
    {
        if (PuffLobbers.Family(__instance) == 1 && __result != null
            && (uint)__instance.mTargetZombieID == __result.DataID) __result = null!;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class PuffLobProjectilePatch
{
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && PuffLobbers.Family(__instance) != 0)
            ImportedPeaVisuals.EnsurePuffLobProjectile(__instance);
    }
}
