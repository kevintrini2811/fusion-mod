using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.TreeStateActivities;

namespace FusionMod;

// The retired Puffshroom/Sunflower pair remains reserved for old saves only.
// Deserialize restores plant fields before calling CreatePlantController
// (Plant.Deserialize RVA 0x6aca44..0x6aca60). Migrate before choosing its skin.
internal static class NativeSunShroom
{
    internal static bool Migrate(Plant plant)
    {
        if (plant.mSeedType != SeedType.Puffshroom || plant.mImitaterType != SeedType.Sunflower) return false;
        bool mature = plant.mTargetX == 0;
        plant.mSeedType = SeedType.Sunshroom;
        plant.mImitaterType = SeedType.None;
        plant.mState = mature ? PlantState.SunshroomBig : PlantState.SunshroomSmall;
        // Identity conversion relinquishes Fusion clock ownership. Only this
        // source maturity remainder crosses into the ordinary native domain;
        // subsequent native timers and rate changes retain vanilla behavior.
        plant.mStateCountdown = FusionTime.NativeTicks(Math.Max(0, plant.mTargetX));
        plant.mTargetX = plant.mTargetY = plant.mStartRow = plant.mShootingCounter = 0;
        return true;
    }
}

[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.CreatePlantController))]
internal static class NativeSunShroomControllerPatch
{
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Plant plant, ref SeedType type)
    {
        if (plant != null && NativeSunShroom.Migrate(plant)) type = SeedType.Sunshroom;
    }
}
