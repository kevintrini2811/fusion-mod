using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Registered fusion plants retain canonical source-centisecond save counters.
// Each real native entry consumes its own pulse: UpdateAbilities includes inline
// branches and also calls standalone owners. Animation advances separately with
// the actual native delta and must not share this counter gate.
internal static class NativePlantClocks
{
    internal static bool Owns(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        return FusionPlant.HasImportedVisuals(identity)
            && identity != FusionPlant.Torchweed && identity != FusionPlant.RoastedSpikeweed;
    }

    internal static bool Allow(Plant plant, MethodBase entry)
    {
        if (!Owns(plant) || !ScaredContent.Playing(plant.mBoard)) return true;
        if (FusionPlant.IsPuffJumper(FusionPlant.From(plant)) && entry.Name == nameof(Plant.UpdateAbilities))
            return true; // The leaf consumes actual native deltas through NativeStep.
        if (SeaPot.AdvancingNative && entry.Name == nameof(Plant.UpdateAbilities))
            return FusionTime.IsSourcePulse(plant.mBoard);
        return FusionTime.SourceStep(plant, entry);
    }
}

[HarmonyPatch]
internal static class NativePlantClockPatch
{
    internal static IEnumerable<MethodBase> TargetMethods() => new[] {
        nameof(Plant.UpdateAbilities), nameof(Plant.UpdateShooter),
        nameof(Plant.UpdateShooting), nameof(Plant.UpdateProductionPlant),
        nameof(Plant.UpdateChomper), nameof(Plant.UpdateTanglekelp),
        nameof(Plant.UpdateScaredyShroom)
    }.Select(name => typeof(Plant).GetMethod(name)!);

    [HarmonyPriority(Priority.First + 3000)]
    internal static bool Prefix(Plant __instance, MethodBase __originalMethod) =>
        NativePlantClocks.Allow(__instance, __originalMethod);
}
