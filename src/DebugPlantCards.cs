using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedModUi;
namespace FusionMod;

// Fusion owns fusion placement. DebugMod owns catalogs, chooser and bank state.
internal static class DebugPlantCards
{
    private static readonly Func<CursorObject, (bool Found, SeedType NativeType, SeedType Marker)>? cursorIdentity =
        OptionalModApi.Bind<Func<CursorObject, (bool, SeedType, SeedType)>>("DebugMod", "DebugMod.DebugPlantCards", "CursorIdentity");
    internal static bool TryCursorIdentity(CursorObject cursor, out FusionPlant identity)
    {
        var result = cursorIdentity?.Invoke(cursor) ?? default;
        identity = new FusionPlant(result.NativeType, result.Marker);
        return result.Found;
    }
    internal static PlantingReason Placement(Board board, FusionPlant incoming, int col, int row)
    {
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return PlantingReason.NotHere;
        var plans = FusionService.ResolveAll(board, incoming, col, row);
        if (plans.Count > 0)
        {
            foreach (var plan in plans)
            {
                var reason = FusionService.CheckFusionPlacement(board, plan.Target, plan.Result);
                if (reason != PlantingReason.Ok) return reason;
            }
            return PlantingReason.Ok;
        }
        if (incoming == FusionPlant.EyesZombieHead) return PlantingReason.NotHere;
        if (ShroomSlots.IsSmall(incoming))
            return ShroomSlots.TryPlacementSlot(board, incoming, col, row, out _) ? PlantingReason.Ok : PlantingReason.NotHere;
        return FusionPlant.IsPumpkinFusion(incoming) ? FusionService.ShellTerrain(board, col, row, incoming)
            : AirPlants.Owns(incoming) ? AirPlants.Terrain(board, col, row)
            : FusionService.CheckNativePlacement(board, col, row,
                incoming == FusionPlant.Splash || incoming == FusionPlant.SeaNut || incoming == FusionPlant.SeaMine || incoming == FusionPlant.GulperShroom
                    ? SeedType.Seashroom : LawnGlove.PlantPlacementRules.TerrainType(incoming.NativeType));
    }


    internal static bool Place(Board value, SeedType type, SeedType marker, bool ingredient, int col, int row)
    {
        var incoming = new FusionPlant(type, marker);
        var target = value.GetPlantsOnLawn(col, row).NormalPlant;
        if (ingredient && target != null && BucketPlants.CanFeed(target, incoming)) BucketPlants.Feed(target);
        else
        {
            var plans = FusionService.ResolveAll(value, incoming, col, row);
            if (plans.Count > 0)
            {
                if (plans.Any(plan => FusionService.CheckFusionPlacement(value, plan.Target, plan.Result) != PlantingReason.Ok)) return false;
                FusionService.CompleteBatch(value, incoming, plans);
            }
            else
            {
                if (Placement(value, incoming, col, row) != PlantingReason.Ok) return false;
                int slot = -1;
                if (ShroomSlots.IsSmall(incoming))
                    ShroomSlots.TryPlacementSlot(value, incoming, col, row, out slot);
                var created = value.AddPlant(col, row, incoming.NativeType, incoming.Marker);
                if (created == null || created.mDead || !created.mIsOnBoard) return false;
                if (slot >= 0) ShroomSlots.Position(created, col, row, slot);
                if (incoming == FusionPlant.BlackCherry) IceInstantFusions.OnFusion(created);
                ThreepeaterFusions.OnFusion(created);
            }
        }
        return true;
    }
}
