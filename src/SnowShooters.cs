using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.TreeStateActivities;

namespace FusionMod;

// Native Repeater/Splitpea/Gatlingpea own targeting, staggered shots and launch counters.
// Its hidden animation clock drives the imported Fusion visuals.
internal static class SnowShooters
{
    [ThreadStatic] internal static Board? FiringBoard;
}

[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.CreatePlantController))]
internal static class SnowShootersControllerPatch
{
    private static void Postfix(Plant plant, Il2CppSource.Controllers.PlantController __result)
    {
        // Keep a native controller as the simulation/save adapter, but replace
        // its rendering with the actual Fusion prefab sprites and clip curves.
        if (plant != null && FusionPlant.HasImportedVisuals(FusionPlant.From(plant)))
            ImportedPeaVisuals.Register(plant, __result);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SnowShootersFireScope
{
    private static void Prefix(Plant __instance, out Board? __state)
    {
        __state = SnowShooters.FiringBoard;
        SnowShooters.FiringBoard = FusionPlant.IsSnowShooter(FusionPlant.From(__instance))
            ? __instance.mBoard : null;
    }

    private static void Finalizer(Board? __state) => SnowShooters.FiringBoard = __state;
}

[HarmonyPatch]
internal static class SnowShootersProjectilePatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        typeof(Board).GetMethods().Where(method => method.Name == nameof(Board.AddProjectile));

    private static void Prefix(Board __instance, ref ProjectileType projectileType)
    {
        // Choose ice before initialization, so the native projectile controller,
        // effects, damage, save data and Torchwood conversion all agree.
        if (SnowShooters.FiringBoard?.Pointer == __instance.Pointer
            && projectileType == ProjectileType.Pea)
            projectileType = ProjectileType.Snowpea;
    }
}
