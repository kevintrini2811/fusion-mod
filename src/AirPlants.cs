using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Mod flight is independent of native IsFlying, which identifies coffee only.
internal static class AirPlants
{
    internal static void Move(Board board, Plant plant, int col, int row)
    {
        float dx = board.GridToPixelX(col, row) - board.GridToPixelX(plant.mPlantCol, plant.mRow);
        float dy = board.GridToPixelY(col, row) - board.GridToPixelY(plant.mPlantCol, plant.mRow);
        plant.mPlantCol = col; plant.mRow = row; plant.mX += dx; plant.mY += dy;
        int order = plant.CalcRenderOrder();
        plant.mShadowRenderOrder += order - plant.mRenderOrder; plant.mRenderOrder = order;
        plant.UpdateReanim();
    }
    internal static bool Owns(FusionPlant identity) => identity.NativeType == SeedType.Blover
        || identity == FusionPlant.CamoShroom || identity == FusionPlant.Flytrapper;
    internal static bool Owns(Plant plant) => Owns(FusionPlant.From(plant));
    internal static IEnumerable<Plant> At(Board board, int col, int row) => Enumerable.Range(0, board.m_plants.Count)
        .Select(i => board.m_plants[i]).Where(p => p != null && p.mPlantCol == col && p.mRow == row
            && ShroomSlots.Live(p) && Owns(p));
    internal static PlantingReason Terrain(Board board, int col, int row, Plant? replacing = null)
    {
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return PlantingReason.NotHere;
        if (At(board, col, row).Any(p => p.Pointer != replacing?.Pointer)) return PlantingReason.NotHere;
        // Ignore the independent ground layer only during this synchronous
        // query. Air needs neither a water support nor a roof pot. Native
        // obstacle and challenge restrictions still own every other rejection.
        var hidden = ShroomSlots.Plants(board, col, row);
        if (replacing != null) hidden.Add(replacing);
        var reason = ShroomSlots.CheckTerrain(board, col, row,
            board.IsPoolSquare(col, row) ? SeedType.Seashroom : SeedType.Blover, hidden.DistinctBy(p => p.Pointer));
        return reason == PlantingReason.NeedsPot ? PlantingReason.Ok : reason;
    }
}
[HarmonyPatch(typeof(Board), nameof(Board.GetPlantsOnLawn))]
internal static class AirPlantLayerPatch
{
    private static void Postfix(Board __instance, int theGridX, int theGridY, ref PlantsOnLawn __result)
    {
        var air = AirPlants.At(__instance, theGridX, theGridY).FirstOrDefault();
        if (air == null) return;
        __result.FlyingPlant = air;
        if (__result.NormalPlant?.Pointer != air.Pointer) return;
        __result.NormalPlant = null;
        for (int i = 0; i < __instance.m_plants.Count; ++i)
        {
            var plant = __instance.m_plants[i];
            if (plant != null && plant.mPlantCol == theGridX && plant.mRow == theGridY && ShroomSlots.Live(plant)
                && !AirPlants.Owns(plant) && plant.mSeedType is not (SeedType.Pumpkinshell or SeedType.Flowerpot
                    or SeedType.Lilypad or SeedType.InstantCoffee)) __result.NormalPlant = plant;
        }
    }
}
[HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
internal static class AirPlantPlacementPatch
{
    private static void Postfix(Board __instance, int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
    {
        if (FusionService.CheckingNativePlacement) return;
        var carried = LawnGlove.LawnGloveMod.GetCarriedPlant(__instance);
        bool flying = carried != null ? theType == carried.mSeedType && AirPlants.Owns(carried)
            : AirPlants.Owns((FusionPlant)theType);
        // Ingredient recipes retain priority over moving/planting an air occupant.
        if (flying && FusionService.ResolveAll(__instance, carried == null ? theType : FusionPlant.From(carried),
                theGridX, theGridY, carried).Count == 0)
            __result = AirPlants.Terrain(__instance, theGridX, theGridY, carried);
    }
}

// Filter each candidate before native FindPlantTarget chooses the ground plant.
// Rejecting only its final result would hide edible plants below the flyer.
[HarmonyPatch(typeof(Zombie), nameof(Zombie.CanTargetPlant))]
internal static class AirPlantTargetPatch
{
    private static void Postfix(Plant thePlant, ref bool __result)
    {
        if (thePlant != null && (AirPlants.Owns(thePlant) || Squish.Airborne(thePlant))) __result = false;
    }
}
