using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Original full-size clips emit at .316667/.333333 seconds. Thrower flight
// lasts 1.5 seconds; Puff's later animation event does not apply here.
// Native plant counters and projectile fields serialize the complete attack.
internal static class SilverLobbers
{
    internal const int ShootTicks = 100;
    internal const float CabbageShot = -491000, MelonShot = -492000,
        KernelShot = -493000, ButterShot = -494000;
    internal const float SplashRadius = LobberImpact.SplashRadius;
    internal static int EmitTick(FusionPlant identity) => identity == FusionPlant.SilverCabbage ? 31 : 33;

    internal static int Family(Projectile shot) => shot.mProjectileType switch
    {
        ProjectileType.Cabbage when shot.mCobTargetX == CabbageShot => 1,
        ProjectileType.Melon when shot.mCobTargetX == MelonShot => 2,
        ProjectileType.Kernel when shot.mCobTargetX == KernelShot => 3,
        ProjectileType.Butter when shot.mCobTargetX == ButterShot => 4,
        _ => 0
    };

    internal static Zombie? Target(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.SilverKernel) return PuffLobbers.Target(plant);
        Zombie? sameRow = null, otherRow = null;
        float sameDistance = float.MaxValue, otherDistance = float.MaxValue;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target) continue;
            var rect = zombie.GetZombieRect();
            float dx = (rect.xMin + rect.xMax) / 2 - (plant.mX + plant.mWidth / 2f);
            float dy = (rect.yMin + rect.yMax) / 2 - (plant.mY + plant.mHeight / 2f);
            if (rect.xMin > 900) continue;
            float distance = dx * dx + dy * dy;
            if (zombie.mRow == plant.mRow && distance < sameDistance)
            { sameRow = zombie; sameDistance = distance; }
            if (distance < otherDistance) { otherRow = zombie; otherDistance = distance; }
        }
        return sameRow ?? otherRow;
    }

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsSilverLobber(FusionPlant.From(plant))) return;
        plant.mLaunchRate = 300;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = 0;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(SilverLobbers))) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || Target(plant) == null) return;
            plant.mLaunchCounter = 285 + plant.mBoard.Rand(31);
            plant.mStartRow = ShootTicks;
            if (FusionPlant.From(plant) == FusionPlant.SilverKernel)
                plant.mShootingCounter = plant.mBoard.Rand(PotBoosts.ButterRange(plant)) == 0 ? 1 : 0;
        }
        if (ShootTicks - plant.mStartRow == EmitTick(FusionPlant.From(plant))
            && Target(plant) is { } target)
            Fire(plant, target);
        --plant.mStartRow;
    }

    internal static void Fire(Plant plant, Zombie target)
    {
        bool melon = FusionPlant.From(plant) == FusionPlant.SilverMelon;
        bool kernel = FusionPlant.From(plant) == FusionPlant.SilverKernel;
        bool butter = kernel && plant.mShootingCounter == 1;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, kernel ? target.mRow : plant.mRow,
            kernel ? butter ? ProjectileType.Butter : ProjectileType.Kernel
                : melon ? ProjectileType.Melon : ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = kernel ? butter ? ButterShot : KernelShot : melon ? MelonShot : CabbageShot;
        if (melon && PotBoosts.HasSupport(plant, FusionPlant.MelonPot)) shot.mCobTargetRow = MelonPot.ShotMarker;
        if (butter) shot.mCobTargetRow = SilverMoney.MoneyUnits(plant);
        LobberFlight.Launch(shot, target, PuffLobbers.FlightTicks);
        ImportedPeaVisuals.EnsureSilverProjectile(shot);
    }

    internal static int SplashDamage(int count) => Math.Min(33, Math.Max(1, 800 / Math.Max(1, count)));

    internal static void Impact(Projectile shot, Zombie? direct)
    {
        int family = Family(shot);
        if (family == 0) return;
        int money = family == 4 ? Math.Clamp(shot.mCobTargetRow, 2, 3) : 0;
        LobberImpact.Apply(shot, direct, family is 1 or 4 ? 60 : family == 3 ? 40 : 100,
            family == 2, family == 4, money, family == 2 && MelonPot.Boosted(shot));
    }

}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SilverLobberInitializePatch
{
    private static void Postfix(Plant __instance) => SilverLobbers.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class SilverLobberTickPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsSilverLobber(FusionPlant.From(__instance))) return true;
        SilverLobbers.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class SilverLobberShootingPatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsSilverLobber(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SilverLobberFirePatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsSilverLobber(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class SilverLobberImpactPatch
{
    private static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (SilverLobbers.Family(__instance) == 0) return true;
        SilverLobbers.Impact(__instance, theZombie);
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class SilverProjectileRestorePatch
{
    private static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && SilverLobbers.Family(__instance) != 0)
            ImportedPeaVisuals.EnsureSilverProjectile(__instance);
    }
}
