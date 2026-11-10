using Il2CppReloaded.Gameplay;
namespace FusionMod;
// Mod air-layer and terrain behavior requires the real board implementation.
internal static class AirPlants
{
    internal static bool Owns(FusionPlant plant) => plant.NativeType == SeedType.Blover
        || plant == FusionPlant.CamoShroom || plant == FusionPlant.Flytrapper;
    internal static bool Owns(Plant plant) => Owns(FusionPlant.From(plant));
    internal static IEnumerable<Plant> At(Board board, int col, int row) => Array.Empty<Plant>();
    internal static PlantingReason Terrain(Board board, int col, int row, Plant? replacing = null) => PlantingReason.Ok;
    internal static void Move(Board board, Plant plant, int col, int row)
    { plant.mPlantCol = col; plant.mRow = row; plant.mX = board.GridToPixelX(col, row); plant.mY = board.GridToPixelY(col, row); }
}
