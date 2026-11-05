using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Passive support effects are derived from live board occupants. Moving,
// removing or loading a pot cannot leave a permanent modifier on its occupant.
internal static class PotBoosts
{
    internal static bool HasSupport(Plant plant, FusionPlant support) => FindSupport(plant, support) != null;

    internal static Plant? FindSupport(Plant plant, FusionPlant support)
    {
        if (!ChomperFusions.Active(plant) || plant.NotOnGround()) return null;
        var plants = plant.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
        {
            var pot = plants[i];
            if (pot != null && pot.mPlantCol == plant.mPlantCol && pot.mRow == plant.mRow
                && pot.mBoard?.Pointer == plant.mBoard.Pointer && !pot.NotOnGround()
                && ChomperFusions.Active(pot) && FusionPlant.From(pot) == support) return pot;
        }
        return null;
    }

    internal static int ButterRange(Plant plant) => HasSupport(plant, FusionPlant.ButterPot) ? 2 : 4;

    internal static bool FasterCabbage(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.Coldslaw && plant.mTargetX >= 150) return false;
        return (identity == (FusionPlant)SeedType.Cabbagepult
            || identity == FusionPlant.SproutPult || identity == FusionPlant.SilverCabbage
            || identity == FusionPlant.LettucePult || identity == FusionPlant.TacoPult || identity == FusionPlant.OverripeMelon
            || identity == FusionPlant.CabbageTriPult || identity == FusionPlant.Coldslaw
            || identity == FusionPlant.KimchiPult || identity == FusionPlant.SaladPult
            || identity == FusionPlant.GarbagePult)
            && !plant.mIsAsleep && HasSupport(plant, FusionPlant.CabbagePot);
    }

    [ThreadStatic] internal static bool ChoosingButter;
}

[HarmonyPatch(typeof(Plant), "UpdateShooter")]
internal static class CabbagePotIntervalPatch
{
    // Postfixes also run when a custom shooter's prefix owns the whole tick.
    // Scale each new interval once; never compound a persistent launch rate.
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Plant __instance, out int __state) => __state = __instance.mLaunchCounter;

    internal static void Postfix(Plant __instance, int __state)
    {
        if (__instance.mLaunchCounter > Math.Max(0, __state) && PotBoosts.FasterCabbage(__instance))
            __instance.mLaunchCounter = Math.Max(1, (int)Math.Round(__instance.mLaunchCounter * (2d / 3)));
    }
}

[HarmonyPatch(typeof(Plant), "FindTargetAndFire")]
internal static class ButterPotChoiceScope
{
    private static void Prefix(Plant __instance, out bool __state)
    {
        __state = PotBoosts.ChoosingButter;
        PotBoosts.ChoosingButter = FusionPlant.From(__instance) == (FusionPlant)SeedType.Kernelpult
            && PotBoosts.HasSupport(__instance, FusionPlant.ButterPot);
    }

    private static void Finalizer(bool __state) => PotBoosts.ChoosingButter = __state;
}

[HarmonyPatch]
internal static class ButterPotNativeRandomPatch
{
    private static MethodBase TargetMethod() => typeof(Common).GetMethod(nameof(Common.Rand), new[] { typeof(int) })!;

    // Native FindTargetAndFire calls Common.Rand(4) immediately before choosing
    // its saved butter state and matching animation. Preserve that entire path.
    internal static void Prefix(ref int range)
    {
        if (range == 4 && PotBoosts.ChoosingButter) range = 2;
    }
}
