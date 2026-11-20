using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class Levitater
{
    private const int RechargeTicks = 1500;

    internal sealed record Detonation(Board Board, int Col, int Row, uint MineId, FusionPlant Mine,
        Plant Lifter, uint LifterId);

    internal static Detonation? Capture(Plant mine)
    {
        if (!MineFusions.Active(mine)) return null;
        var board = mine.mBoard;
        int col = mine.mPlantCol, row = mine.mRow;
        var lifter = board.GetPlantsOnLawn(col, row).FlyingPlant;
        if (lifter == null || FusionPlant.From(lifter) != FusionPlant.Levitater || lifter.mLaunchCounter > 0
            || !ChomperFusions.Active(lifter)) return null;
        return new(board, col, row, mine.DataID, FusionPlant.From(mine), lifter, lifter.DataID);
    }

    internal static void Rearm(Plant mine, Detonation? detonation)
    {
        if (detonation == null || !mine.mDead || mine.DataID != detonation.MineId
            || mine.mBoard?.Pointer != detonation.Board.Pointer) return;
        var lifter = detonation.Lifter;
        if (lifter.DataID != detonation.LifterId || lifter.mBoard?.Pointer != detonation.Board.Pointer
            || lifter.mPlantCol != detonation.Col || lifter.mRow != detonation.Row || lifter.mLaunchCounter > 0
            || !ChomperFusions.Active(lifter)) return;

        lifter.mLaunchCounter = RechargeTicks;
        if (DoomFusions.Spawn(detonation.Board, detonation.Col, detonation.Row, detonation.Mine) == null
            && lifter.DataID == detonation.LifterId && lifter.mLaunchCounter == RechargeTicks)
            lifter.mLaunchCounter = 0;
    }
}

[HarmonyPatch]
internal static class LevitaterNativeMinePatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => new[]
    {
        AccessTools.Method(typeof(Plant), nameof(Plant.UpdatePotato)),
        AccessTools.Method(typeof(Plant), nameof(Plant.UpdateAbilities))
    };

    private static void Prefix(Plant __instance, bool __runOriginal, out Levitater.Detonation? __state) =>
        __state = __runOriginal && FusionPlant.From(__instance) == (FusionPlant)SeedType.Potatomine
            && __instance.mState is PlantState.PotatoArmed or PlantState.PotatoMashed
            ? Levitater.Capture(__instance) : null;

    private static void Postfix(Plant __instance, Levitater.Detonation? __state) => Levitater.Rearm(__instance, __state);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class LevitaterFusionMinePatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out Levitater.Detonation? __state) =>
        __state = __runOriginal && __instance.mState == PlantState.PotatoMashed
            && MineFusions.CurrentDetonation == (__instance.mBoard?.Pointer, __instance.DataID)
            ? Levitater.Capture(__instance) : null;

    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Plant __instance, Levitater.Detonation? __state) => Levitater.Rearm(__instance, __state);
}
