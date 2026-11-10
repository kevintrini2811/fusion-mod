using Il2CppReloaded.Gameplay;
using Il2CppReloaded.TreeStateActivities;
using ReplantedModUi;
using UnityEngine;

namespace FusionMod;

// No DebugMod AssemblyRef: ordinary Fusion gameplay remains independent.
internal static class DebugMode
{
    private static readonly Func<Board?, bool>? ownsBoard = OptionalModApi.Bind<Func<Board?, bool>>("DebugMod", "DebugMod.DebugMode", "Owns");
    private static readonly Func<GameplayActivity, bool>? ownsApp = OptionalModApi.Bind<Func<GameplayActivity, bool>>("DebugMod", "DebugMod.DebugMode", "Owns");
    private static readonly Action? cancel = OptionalModApi.Bind<Action>("DebugMod", "DebugMod.DebugMode", "CancelZombiePlacement");
    internal static bool Owns(Board? board) => ownsBoard?.Invoke(board) == true;
    internal static bool Owns(GameplayActivity app) => ownsApp?.Invoke(app) == true;
    internal static void CancelZombiePlacement() => cancel?.Invoke();
    internal static void TraceShootingPot(Plant plant, string message)
    { if (Owns(plant.mBoard)) FusionMod.Instance.LoggerInstance.Msg($"TriPot plant={(uint)plant.DataID}: {message}"); }

}

internal static partial class DebugFusionIntegration
{
    internal static object[] MetalDrops(Board board) => board != null && DebugMode.Owns(board)
        ? BucketDrops.Snapshot(board).Select(drop => (object)new {
            type = drop.Identity.NativeType.ToString(), marker = drop.Identity.Marker.ToString(),
            row = drop.Row, x = drop.X, y = drop.Y, age = drop.Age,
            spawnX = drop.SpawnX, spawnGroundY = drop.SpawnGroundY
        }).ToArray()
        : Array.Empty<object>();

    internal static string? PacketResource(SeedType type, SeedType marker) =>
        FusionAlmanac.DebugCatalog(false).FirstOrDefault(item => item.Plant == new FusionPlant(type, marker)).Portrait;
    internal static bool PlaceApi(Board board, SeedType type, SeedType marker, bool ingredient, int col, int row, int offsetY)
    {
        var previous = PlayerCursorScope.Current;
        try
        {
            PlayerCursorScope.Current = new(board, 0, board.GridToPixelX(col, row) + 40, board.GridToPixelY(col, row) + offsetY);
            return DebugPlantCards.Place(board, type, marker, ingredient, col, row);
        }
        finally { PlayerCursorScope.Current = previous; }
    }
    internal static bool GetSuppressReplacement() => ScaredContent.SuppressReplacement;
    internal static void SetSuppressReplacement(bool value) => ScaredContent.SuppressReplacement = value;
    internal static bool Place(Board board, SeedType type, SeedType marker, bool ingredient, int col, int row) =>
        DebugPlantCards.Place(board, type, marker, ingredient, col, row);
    internal static IEnumerable<(SeedType NativeType, SeedType Marker, string Name, string Portrait, int Cost)> Ingredients(Board board)
    {
        var costs = PlantValue.Calculate(board);
        return FusionAlmanac.DebugCatalog(true).Select(item =>
            (item.Plant.NativeType, item.Plant.Marker, item.Name, item.Portrait, costs[item.Plant]));
    }
}
