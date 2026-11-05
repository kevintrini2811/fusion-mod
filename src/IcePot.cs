using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The four Roasted forms are original, non-fusible summons. Native plants own
// their combat behavior; Ice Pot replaces only one qualifying occupant per cycle.
internal static class IcePot
{
    internal static SeedType Base(FusionPlant identity) => identity == FusionPlant.RoastedSquash ? SeedType.Squash
        : identity == FusionPlant.RoastedWallNut ? SeedType.Wallnut
        : identity == FusionPlant.RoastedPumpkin ? SeedType.Pumpkinshell
        : identity == FusionPlant.RoastedPotato ? SeedType.Potatomine : SeedType.None;
    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.RoastedPotato) return;
        plant.mState = PlantState.PotatoArmed;
        plant.mStateCountdown = 0;
    }
    internal static void Tick(Plant pot)
    {
        if (FusionPlant.From(pot) != FusionPlant.IcePot || !ChomperFusions.Active(pot) || pot.NotOnGround()) return;
        var board = pot.mBoard;
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant == null || plant.mPlantCol != pot.mPlantCol || plant.mRow != pot.mRow
                || !ChomperFusions.Active(plant) || plant.NotOnGround()) continue;
            var type = Base(FusionPlant.From(plant));
            if (type == SeedType.None) continue;
            if (++pot.mTargetY <= 1000) return;
            // Reserve the replacement before consuming the summon. Health is
            // preserved only for the Roasted Potato, as in the original method.
            var result = board.AddPlant(pot.mPlantCol, pot.mRow, type, SeedType.None);
            if (result == null || result.mDead || !result.mIsOnBoard) return;
            if (type == SeedType.Potatomine)
            { MineFusions.GrowImmediately(result); result.mPlantHealth = Math.Min(result.mPlantMaxHealth, plant.mPlantHealth); }
            PlayerPlantingScope.Consume(plant);
            pot.mTargetY = 0;
            return;
        }
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class RoastedPotatoInitializePatch
{ private static void Postfix(Plant __instance) => IcePot.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class IcePotTickPatch
{ private static void Postfix(Plant __instance) => IcePot.Tick(__instance); }
