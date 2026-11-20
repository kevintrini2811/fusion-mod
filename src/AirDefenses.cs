using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class AirDefenses
{
    internal const int BlockTicks = 84;
    internal const int RecoveryDelay = 1002;

    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.Hoverleaf || identity == FusionPlant.DroneNut;
    internal static bool IsDrone(Plant plant) => FusionPlant.From(plant) == FusionPlant.DroneNut;

    internal static void Initialize(Plant plant, FusionPlant identity)
    {
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.DroneNut ? 4000 : 300;
        plant.mDoSpecialCountdown = plant.mLaunchCounter = plant.mShootingCounter = 0;
    }

    internal static void Tick(Plant plant, FusionPlant identity)
    {
        if (!ChomperFusions.Active(plant) || !FusionTime.SourceStep(plant, typeof(AirDefenses))) return;
        if (identity == FusionPlant.Hoverleaf)
        {
            if (plant.mShootingCounter > 0 && --plant.mShootingCounter == 0) plant.mState = PlantState.Ready;
            return;
        }
        if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
        if (plant.mPlantHealth >= plant.mPlantMaxHealth) return;
        // The source waits strictly more than ten seconds, then restores one
        // health per 20 ms. Both countdowns stay in saved source ticks.
        ++plant.mPlantHealth;
        plant.mLaunchCounter = 2;
        plant.UpdateReanimColor();
    }

    internal static void TakeRedirectedDamage(Plant drone, int damage)
    {
        drone.mLaunchCounter = RecoveryDelay;
        drone.mPlantHealth -= damage;
        if (drone.mPlantHealth <= 0) drone.Die();
        else drone.UpdateReanimColor();
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class AirDefenseInitializePatch
{
    private static void Postfix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (AirDefenses.Owns(identity)) AirDefenses.Initialize(__instance, identity);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class AirDefenseAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (!AirDefenses.Owns(identity)) return true;
        AirDefenses.Tick(__instance, identity);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class AirDefenseSpecialPatch
{
    private static bool Prefix(Plant __instance) => !AirDefenses.Owns(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class AirDefenseShootingPatch
{
    private static bool Prefix(Plant __instance) => !AirDefenses.Owns(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Board), nameof(Board.FindUmbrellaPlant))]
internal static class HoverleafUmbrellaPatch
{
    internal static void Postfix(Board __instance, int theGridX, int theGridY, bool __runOriginal, ref Plant? __result)
    {
        if (!__runOriginal || __result != null || !ScaredContent.Playing(__instance) || __instance.mPaused
            || theGridX < 0 || theGridX >= 9 || theGridY < 0 || theGridY >= __instance.GetNumRows()) return;
        var leaf = __instance.GetPlantsOnLawn(theGridX, theGridY).FlyingPlant;
        if (leaf == null || FusionPlant.From(leaf) != FusionPlant.Hoverleaf || !ChomperFusions.Active(leaf)) return;
        // The native caller still deflects the basketball or turns the bungee
        // away. Only the eligible protector gains a new exact-tile fallback.
        leaf.mShootingCounter = AirDefenses.BlockTicks;
        __result = leaf;
    }
}
