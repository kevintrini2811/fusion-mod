using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// These owners use ShootingCounter for their own clips, shields or production.
// Native UpdateShooting must never decrement that field or create a shot from it.
internal static class CustomShooterClocks
{
    internal static bool Owns(FusionPlant identity) => FusionPlant.IsFume(identity) || FusionPlant.IsGloom(identity)
        || FusionPlant.IsContactUmbrella(identity) || FusionPlant.IsActiveNut(identity)
        || FusionPlant.IsPumpkinFusion(identity) || FusionPlant.IsThreepeater(identity)
        || identity == FusionPlant.GrappleKelp || identity == FusionPlant.Torchflower;
    internal static bool HasNoNativeWeapon(FusionPlant identity) => FusionPlant.IsContactUmbrella(identity)
        || identity == FusionPlant.Torchflower;
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class CustomShootingCounterPatch
{ internal static bool Prefix(Plant __instance) => !CustomShooterClocks.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class CustomNonShooterFirePatch
{ internal static bool Prefix(Plant __instance) => !CustomShooterClocks.HasNoNativeWeapon(FusionPlant.From(__instance)); }
