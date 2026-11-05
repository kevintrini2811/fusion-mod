using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Wall-nut and Garlic retain targeting, biting, terrain and saves.
// Silver Sunflower shares the existing saved producer implementation.
internal static class SilverPlants
{
    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsSilver(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.SilverUmbrella ? 1000
            : identity == FusionPlant.SilverNut ? 6000
            : identity == FusionPlant.SilverGarlic ? 600 : 300;
        if (identity == FusionPlant.SilverUmbrella || identity == FusionPlant.SilverKernel)
            plant.mTargetY = 0;
        if (identity == FusionPlant.SilverDoomShroom)
            plant.mDoSpecialCountdown = 0;
        if (identity != FusionPlant.SilverIceShroom) return;
        // Source IceShroom.Awake gives an attribute countdown of 1.5 seconds.
        // Replacing awake UpdateAbilities prevents native ice/countdown writers.
        plant.mDoSpecialCountdown = 0;
        plant.mLaunchCounter = 150;
        plant.mStartRow = 0;
    }

    internal static void Tick(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.SilverIceShroom
            || !FumeFusions.Active(plant) || plant.mWakeUpCounter > 0 || plant.mStartRow != 0) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mLaunchCounter == 0) Explode(plant);
    }

    internal static void Explode(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.SilverIceShroom
            || !ChomperFusions.Active(plant) || plant.mStartRow != 0) return;
        var board = plant.mBoard;
        uint id = plant.DataID;
        // Commit before death callbacks; shovel, fusion consumption and board
        // teardown never enter this path. Source Crashed also explodes once.
        plant.mStartRow = 1;
        plant.Die();
        if (!plant.mDead || plant.DataID != id || plant.mBoard?.Pointer != board.Pointer) return;
        if (!IceInstantFusions.Ice(board, 500, plant.mPlantCol, plant.mRow)) return;
        ElementalNuts.Heal(board, false);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SilverInitializePatch
{
    private static void Postfix(Plant __instance) => SilverPlants.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SilverIceAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (identity != FusionPlant.SilverIceShroom && identity != FusionPlant.SilverDoomShroom) return true;
        // Native common ability work owns coffee's saved wake countdown.
        // Let it run while asleep/waking; DoSpecial remains suppressed.
        if (__instance.mIsAsleep || __instance.mWakeUpCounter > 0) return true;
        if (identity == FusionPlant.SilverIceShroom) SilverPlants.Tick(__instance);
        return false;
    }

    private static void Postfix(Plant __instance)
    {
        // Coffee's native wake path can inline Doom's countdown initialization.
        if (FusionPlant.From(__instance) == FusionPlant.SilverDoomShroom)
            __instance.mDoSpecialCountdown = 0;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class SilverIceSpecialPatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.SilverIceShroom
        && FusionPlant.From(__instance) != FusionPlant.SilverDoomShroom;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateDoomShroom))]
internal static class SilverDoomAbilityPatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.SilverDoomShroom;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class SilverDoomCrushPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || FusionPlant.From(__instance) != FusionPlant.SilverDoomShroom
            || __instance.mSquished || __instance.NotOnGround()
            || !CherryBombPlants.CanTriggerDeath(__instance)) return true;
        // Native Squish tail-calls DoSpecial for Doom. Its normal explosion
        // also removes it; this stable variant must still die when crushed.
        __instance.Die();
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class SilverIceCrushPatch
{
    [HarmonyPriority(Priority.Last)]
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || FusionPlant.From(__instance) != FusionPlant.SilverIceShroom
            || !CherryBombPlants.CanTriggerDeath(__instance)) return true;
        SilverPlants.Explode(__instance);
        return false;
    }
}
