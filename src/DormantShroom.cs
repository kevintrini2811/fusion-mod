using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The original MagnetDoomPrefab has only Plant, not DoomShroom. Preserve native
// mushroom sleep/coffee and serialization, but never schedule a Doom explosion.
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class DormantShroomInitializePatch
{
    internal static void Postfix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.DormantShroom) return;
        __instance.mPlantHealth = __instance.mPlantMaxHealth = 300;
        __instance.mDoSpecialCountdown = 0;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class DormantShroomAbilityPatch
{
    internal static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.DormantShroom;
}
[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class DormantShroomSpecialPatch
{
    internal static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.DormantShroom;
}
