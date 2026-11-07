using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The native lobber owns terrain and saved fields. One owner advances the
// attack clip and selects butter once, including across save/load.
internal static class LobberFusions
{
    internal const int ShootTicks = 100;

    internal static int Duration(FusionPlant identity) => identity == FusionPlant.SaladPult ? 117 : ShootTicks;

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsMixedLobber(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = (FusionPlant.From(plant) == FusionPlant.MetalPult || FusionPlant.From(plant) == FusionPlant.BoltPult) ? 2000 : 300;
        plant.mLaunchRate = FusionPlant.From(plant) == FusionPlant.DamagePult ? 1500
            : FusionPlant.From(plant) == FusionPlant.SaladPult || FusionPlant.From(plant) == FusionPlant.SolarPult ? 500 : 300;
        plant.mTargetX = 0;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = 0;
        SquashLobbers.Initialize(plant);
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        var identity = FusionPlant.From(plant);
        if (SquashLobbers.Owns(identity)) { SquashLobbers.Tick(plant); return; }
        if (ElementalLobbers.Owns(identity)) { ElementalLobbers.Tick(plant); return; }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || PuffLobbers.Target(plant) == null) return;
            plant.mLaunchCounter = identity == FusionPlant.DamagePult ? 1425 + plant.mBoard.Rand(151)
                : (identity == FusionPlant.SaladPult || identity == FusionPlant.SolarPult)
                ? 475 + plant.mBoard.Rand(51) : 285 + plant.mBoard.Rand(31);
            plant.mStartRow = Duration(identity);
            plant.mShootingCounter = identity != FusionPlant.DamagePult && identity != FusionPlant.OverripeMelon && identity != FusionPlant.SaladPult && identity != FusionPlant.MelonMorph && identity != FusionPlant.MetalPult
                && plant.mBoard.Rand(PotBoosts.ButterRange(plant)) == 0 ? 1 : 0;
        }
        // First simulation tick at or after the original animation event.
        int eventTick = identity == FusionPlant.SaladPult ? 45 : identity == FusionPlant.TacoPult || identity == FusionPlant.DamagePult ? 32 : 34;
        if (Duration(identity) - plant.mStartRow == eventTick)
        {
            if (identity == FusionPlant.SaladPult)
            {
                LobberProjectiles.Fire(plant, plant.mRow, PuffLobbers.Target(plant), false, 0);
                if (plant.mBoard.Rand(100) < 42) plant.mLaunchCounter = 10;
            }
            else LobberProjectiles.Fire(plant, plant.mRow, PuffLobbers.Target(plant), false, 0);
        }
        if (--plant.mStartRow == 0) plant.mShootingCounter = 0;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class MixedLobberInitializePatch
{
    internal static void Postfix(Plant __instance) => LobberFusions.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class MixedLobberTickPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsMixedLobber(FusionPlant.From(__instance))) return true;
        LobberFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class MixedLobberShootingPatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsMixedLobber(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class MixedLobberNativeFirePatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsMixedLobber(FusionPlant.From(__instance));
}
