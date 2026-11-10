using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Source SpreadFume.Start / SpreadScaredyShroom.Start are instantaneous recipe
// operations. Their final plants and all shots remain native Replanted objects.
internal static class PuffSpreading
{
    internal static IEnumerable<(int Col, int Row, int Count)> Cells(int col, int row, int count, bool fume)
    {
        if (fume)
        {
            for (int distance = 1; distance <= count; ++distance)
                yield return (col + distance, row, count - distance + 1);
        }
        else
        {
            // Binary source: 1 = front; 2 = front/up/down once;
            // 3 = front/up/down twice. Blocked cells do not redistribute.
            yield return (col + 1, row, count == 1 ? 1 : count - 1);
            if (count > 1)
            {
                yield return (col, row - 1, count - 1);
                yield return (col, row + 1, count - 1);
            }
        }
    }

    internal static void Create(Board board, FusionService.Plan plan, List<Plant> created)
    {
        foreach (var cell in Cells(plan.Target.mPlantCol, plan.Target.mRow,
            plan.PuffIngredients!.Count, plan.Result == (FusionPlant)SeedType.Fumeshroom))
        {
            for (int i = 0; i < cell.Count; ++i)
            {
                // Occupancy and terrain are checked separately from allocation:
                // unavailable cells are skipped; failed allocation rolls back
                // every new object before any ingredient is consumed.
                if (!ShroomSlots.TryPlacementSlot(board, SeedType.Puffshroom, cell.Col, cell.Row, out _)) break;
                if (!ShroomSlots.TrySpawn(board, SeedType.Puffshroom, cell.Col, cell.Row, out var puff))
                    throw new InvalidOperationException("Puff spread allocation failed; ingredients retained.");
                created.Add(puff!);
            }
        }
    }
}
