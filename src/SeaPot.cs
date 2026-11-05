using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Moisture is derived from live pots and terrain. No permanent modifier can
// survive moving, removing, recycling or loading a pot. Multiple pots do not stack.
internal static class SeaPot
{
    internal static bool Wet(Plant pot)
    {
        // Moisture is a spatial state, so pausing must not hide its artwork.
        if (pot.mBoard == null || pot.mDead || !pot.mIsOnBoard || pot.NotOnGround()) return false;
        var board = pot.mBoard;
        for (int col = Math.Max(0, pot.mPlantCol - 1); col <= Math.Min(8, pot.mPlantCol + 1); ++col)
        for (int row = Math.Max(0, pot.mRow - 1); row <= Math.Min(board.GetNumRows() - 1, pot.mRow + 1); ++row)
            if (board.IsPoolSquare(col, row)) return true;
        return false;
    }

    internal static bool Boosted(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.NotOnGround() || plant.mIsAsleep) return false;
        var identity = FusionPlant.From(plant);
        // These are the available terrestrial kelp hybrids. Aquatic grabbers
        // and the unrelated Sea-shroom family are not moisture recipients.
        if (identity != FusionPlant.GrappleKelp && identity != FusionPlant.SpikeKelp
            && identity != FusionPlant.TingleKelp && identity != FusionPlant.TangleWeed
            && identity != FusionPlant.TangleNut) return false;
        for (int i = 0; i < plant.mBoard.m_plants.Count; ++i)
        {
            var pot = plant.mBoard.m_plants[i];
            if (pot != null && pot.mBoard?.Pointer == plant.mBoard.Pointer
                && Math.Abs(pot.mPlantCol - plant.mPlantCol) <= 1 && Math.Abs(pot.mRow - plant.mRow) <= 1
                && FusionPlant.From(pot) == FusionPlant.SeaPot && ChomperFusions.Active(pot) && Wet(pot)) return true;
        }
        return false;
    }

    internal static int Steps(Plant plant) => Boosted(plant) && (plant.mBoard.mMainCounter & 1) == 0 ? 2 : 1;

    internal static void Advance(Plant plant, Action<Plant> tick)
    {
        int steps = Steps(plant);
        uint id = plant.DataID;
        var board = plant.mBoard;
        for (int i = 0; i < steps; ++i)
        {
            if (plant.DataID != id || plant.mBoard?.Pointer != board.Pointer) break;
            tick(plant);
        }
    }
}

// The native Spikeweed owner waits for the body clip's loop count as well as
// its saved hit countdown. Scale both clocks, otherwise only the first hit
// becomes earlier while subsequent attacks retain the old cadence.
[HarmonyPatch(typeof(Plant), nameof(Plant.PlayBodyReanim))]
internal static class SeaPotWeedAnimationPatch
{
    private static void Prefix(Plant __instance, ref float theAnimRate)
    {
        if (FusionPlant.From(__instance) == FusionPlant.TangleWeed && SeaPot.Boosted(__instance))
            theAnimRate *= 1.5f;
    }
}

// UpdateAbilities owns the native Spikeweed attack countdown, including its
// inlined UpdateSpikeweed path. Custom kelp owners advance their own clocks.
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SeaPotWeedSpeedPatch
{
    [ThreadStatic] private static bool advancing;
    private static void Prefix(Plant __instance, out uint __state) => __state = __instance.DataID;
    private static void Postfix(Plant __instance, uint __state)
    {
        if (advancing || __instance.DataID != __state || FusionPlant.From(__instance) != FusionPlant.TangleWeed
            || SeaPot.Steps(__instance) != 2) return;
        advancing = true;
        try { __instance.UpdateAbilities(); }
        finally { advancing = false; }
    }
}
