using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Thrower owns each projectile's native saved arc. The plant owns its saved
// attack phase, once-per-cycle butter choice and non-butter series.
internal static class TriLobbers
{
    internal const int ShootTicks = 50;
    private const float Kernel = -510054, Butter = -510055, Melon = -510080;
    internal static int Family(Projectile shot) => shot.mProjectileType switch
    {
        ProjectileType.Kernel when shot.mCobTargetX == Kernel => 1,
        ProjectileType.Butter when shot.mCobTargetX == Butter => 2,
        ProjectileType.Melon when shot.mCobTargetX == Melon => 3,
        _ => 0
    };
    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsTriLobber(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = 150;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = plant.mTargetX = plant.mTargetY = 0;
    }
    internal static Zombie? Target(Plant plant, int row) => PuffLobbers.Target(plant, row);
    private static bool HasTarget(Plant plant) => Enumerable.Range(Math.Max(0, plant.mRow - 1),
        Math.Min(plant.mBoard.GetNumRows() - 1, plant.mRow + 1) - Math.Max(0, plant.mRow - 1) + 1)
        .Any(row => Target(plant, row) != null);

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(TriLobbers))) return;
        var identity = FusionPlant.From(plant);
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || !HasTarget(plant)) return;
            plant.mLaunchCounter = 143 + plant.mBoard.Rand(15);
            plant.mStartRow = ShootTicks;
            plant.mShootingCounter = identity == FusionPlant.KernelTriPult
                && plant.mBoard.Rand(PotBoosts.ButterRange(plant)) == 0 ? 1 : 0;
        }
        if (ShootTicks - plant.mStartRow == 25) Shoot(plant);
        --plant.mStartRow;
    }

    internal static void Shoot(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.CabbageTriPult)
        {
            foreach (int requested in new[] { plant.mRow - 1, plant.mRow + 1, plant.mRow })
            {
                int row = Math.Clamp(requested, 0, plant.mBoard.GetNumRows() - 1);
                LobberProjectiles.Fire(plant, row, Target(plant, row), requested != row,
                    requested == plant.mRow ? 2 : 1);
            }
            return;
        }
        bool kernel = FusionPlant.From(plant) == FusionPlant.KernelTriPult;
        bool butter = kernel && plant.mShootingCounter == 1;
        if (kernel && !butter && plant.mTargetY < int.MaxValue) ++plant.mTargetY;
        foreach (int requested in new[] { plant.mRow - 1, plant.mRow + 1, plant.mRow })
        {
            int row = Math.Clamp(requested, 0, plant.mBoard.GetNumRows() - 1);
            Fire(plant, row, Target(plant, row), requested != row);
        }
        if (kernel)
        {
            // The source increments before Shoot1's extras, but Shoot2 uses
            // the previous count and resets after its complete butter volley.
            int extra = Math.Max(0, plant.mTargetY - 1);
            var targets = new List<Zombie>();
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            {
                var zombie = plant.mBoard.m_zombies[i];
                if (zombie != null && Math.Abs(zombie.mRow - plant.mRow) <= 1
                    && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
                    && zombie.GetZombieRect().xMax >= plant.mX + plant.mWidth / 2f
                    && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) targets.Add(zombie);
            }
            for (int i = 0; i < extra && targets.Count > 0; ++i)
            {
                var target = targets[plant.mBoard.Rand(targets.Count)];
                Fire(plant, target.mRow, target, false);
            }
            if (butter) plant.mTargetY = 0;
        }
    }

    private static void Fire(Plant plant, int row, Zombie? target, bool edge)
    {
        bool kernel = FusionPlant.From(plant) == FusionPlant.KernelTriPult;
        bool butter = kernel && plant.mShootingCounter == 1;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f
            + (edge ? .5f * ScaredyFusions.SourceUnit : 0), plant.mY + plant.mHeight / 2f - 10,
            plant.mRenderOrder + 1, row, kernel ? butter ? ProjectileType.Butter : ProjectileType.Kernel : ProjectileType.Melon);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = kernel ? butter ? Butter : Kernel : Melon;
        int damage = ShootingPots.ThreeDamage(plant, kernel ? butter ? 40 : 20 : 80);
        shot.mCobTargetRow = !kernel && PotBoosts.HasSupport(plant, FusionPlant.MelonPot) ? -damage : damage;
        LobberFlight.Launch(shot, target, PuffLobbers.FlightTicks);
        ImportedPeaVisuals.EnsureTriLobProjectile(shot);
    }

    internal static void Impact(Projectile shot, Zombie? target)
    {
        int family = Family(shot);
        if (family == 0) return;
        LobberImpact.Apply(shot, target, Math.Abs(shot.mCobTargetRow), family == 3,
            family == 2, 0, family == 3 && shot.mCobTargetRow < 0);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TriLobberInitializePatch
{
    internal static void Postfix(Plant __instance) => TriLobbers.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class TriLobberTickPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsTriLobber(FusionPlant.From(__instance))) return true;
        TriLobbers.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class TriLobberShootingPatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsTriLobber(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class TriLobberNativeFirePatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsTriLobber(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class TriLobberImpactPatch
{
    internal static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (TriLobbers.Family(__instance) == 0) return true;
        TriLobbers.Impact(__instance, theZombie); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class TriLobberVisualPatch
{
    internal static void Postfix(Projectile __instance)
    { if (!__instance.mDead && TriLobbers.Family(__instance) != 0) ImportedPeaVisuals.EnsureTriLobProjectile(__instance); }
}
