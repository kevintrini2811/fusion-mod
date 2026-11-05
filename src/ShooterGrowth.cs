using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

// These source families use the same saved fifteen-second click-growth rule.
internal static class ShooterGrowth
{
    private static bool CanGrow(FusionPlant identity) => identity == FusionPlant.BlazeRepeater
        || identity == FusionPlant.SplitBlaze || identity == FusionPlant.PoisonRepeater
        || identity == FusionPlant.SplitPoison || identity == FusionPlant.HypnoRepeater
        || identity == FusionPlant.SplitHypno;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsBlazeShooter(identity) && !FusionPlant.IsPoisonShooter(identity) && !FusionPlant.IsHypnoShooter(identity)) return;
        // mStartRow is saved/reset natively and unused by these shooters.
        // The +0xd4 writer in Fire addresses a projectile, not this Plant.
        plant.mStartRow = CanGrow(identity) ? 1500 : 0;
    }

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (CanGrow(identity)
            && ChomperFusions.Active(plant) && plant.mStartRow > 0) --plant.mStartRow;
    }

    internal static bool Grow(Plant plant, int clicks, int player)
    {
        var identity = FusionPlant.From(plant);
        if (!CanGrow(identity)) return false;
        var board = plant.mBoard;
        if (clicks < 0 || player != 0 || !ChomperFusions.Active(plant) || plant.mStartRow > 0
            || board.mCursorObject.CursorType != CursorType.Normal
            || LawnGloveMod.GetCarriedPlant(board) != null) return false;
        var result = identity == FusionPlant.BlazeRepeater ? FusionPlant.SplitBlaze
            : identity == FusionPlant.SplitBlaze ? FusionPlant.GatlingBlaze
            : identity == FusionPlant.PoisonRepeater ? FusionPlant.SplitPoison
            : identity == FusionPlant.SplitPoison ? FusionPlant.GatlingPoison
            : identity == FusionPlant.HypnoRepeater ? FusionPlant.SplitHypno : FusionPlant.GatlingHypno;
        if (FusionService.CheckFusionPlacement(board, plant, result) != PlantingReason.Ok) return false;
        var grown = board.AddPlant(plant.mPlantCol, plant.mRow, result.NativeType, result.Marker);
        if (grown == null || grown.mDead || !grown.mIsOnBoard) return false;
        grown.mPlantHealth = Math.Min(grown.mPlantMaxHealth, plant.mPlantHealth);
        PlayerPlantingScope.Consume(plant);
        return true;
    }

}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ShooterGrowthInitializePatch
{
    internal static void Postfix(Plant __instance) => ShooterGrowth.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ShooterGrowthTickPatch
{
    internal static void Postfix(Plant __instance) => ShooterGrowth.Tick(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.MouseDown))]
internal static class ShooterGrowthClickPatch
{
    internal static bool Prefix(Plant __instance, int theClickCount, int playerIndex) =>
        !ShooterGrowth.Grow(__instance, theClickCount, playerIndex);
}

