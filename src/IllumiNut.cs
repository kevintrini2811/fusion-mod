using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Wall-nut owns health, blocking, terrain and serialization. Its unused launch
// counter stores the source attribute timer, including across saves and moves.
internal static class IllumiNut
{
    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.IllumiNut) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 4000;
        plant.mLaunchCounter = 1500;
    }
    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.IllumiNut && plant.mLaunchCounter > 50)
            plant.mLaunchCounter = 50;
    }
    internal static void Tick(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.IllumiNut || !ChomperFusions.Active(plant)) return;
        if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
        plant.mLaunchCounter = 1500;
        ReactivePots.Heal(plant, Math.Max(0, PumpkinFusions.LightLevel(plant) - 1) * 150);
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class IllumiNutInitializePatch
{ private static void Postfix(Plant __instance) => IllumiNut.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class IllumiNutAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.IllumiNut) return true;
        IllumiNut.Tick(__instance); return false;
    }
}
