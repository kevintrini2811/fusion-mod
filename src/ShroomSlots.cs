using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Positions, rather than a spare Plant field or a transient registry, own slot
// identity. ReloadedObject already serializes mX/mY; cancellation restores them.
internal static class ShroomSlots
{
    internal static bool IsSmall(FusionPlant identity) =>
        identity.NativeType is SeedType.Puffshroom or SeedType.Sunshroom;

    internal static bool Live(Plant plant) => !plant.mDead && plant.mIsOnBoard && !plant.mSquished
        && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee && !plant.NotOnGround();

    internal static List<Plant> Plants(Board board, int col, int row)
    {
        var result = new List<Plant>();
        var normal = board.GetPlantsOnLawn(col, row).NormalPlant;
        if (normal != null && Live(normal)) result.Add(normal);
        var plants = board.m_plants;
        for (int i = 0; i < plants.Count; ++i)
        {
            var plant = plants[i];
            if (plant != null && plant.mPlantCol == col && plant.mRow == row && Live(plant)
                && IsSmall(FusionPlant.From(plant)) && result.All(p => p.Pointer != plant.Pointer)) result.Add(plant);
        }
        return result;
    }

    // Back centre, front right, front left. Adapt Fusion's triangle to native
    // sprite sizes; keep a centred legacy plant closest to slot zero.
    internal static (float X, float Y) Offset(int slot) => slot switch
    {
        1 => (24, 12), 2 => (-24, 12), _ => (0, -16)
    };

    internal static int Nearest(float x, float y)
    {
        int selected = 0;
        float distance = float.MaxValue;
        for (int slot = 0; slot < 3; ++slot)
        {
            var offset = Offset(slot);
            float d = (x - offset.X) * (x - offset.X) + (y - offset.Y) * (y - offset.Y);
            if (d < distance) { selected = slot; distance = d; }
        }
        return selected;
    }

    internal static int Slot(Plant plant) => Nearest(
        plant.mX - plant.mBoard.GridToPixelX(plant.mPlantCol, plant.mRow),
        plant.mY - plant.mBoard.GridToPixelY(plant.mPlantCol, plant.mRow));

    internal static int Hover(Board board, int col, int row)
    {
        var (x, y) = PlayerCursorScope.Position(board);
        return Hover(board, col, row, x, y);
    }

    internal static int Hover(Board board, int col, int row, int x, int y)
    {
        // Native plant coordinates describe the upper-left corner of an 80px
        // cell; mouse selection uses the visual centre of each subfield.
        return Nearest(x - board.GridToPixelX(col, row) - 40, y - board.GridToPixelY(col, row) - 40);
    }

    internal static void Position(Plant plant, int col, int row, int slot)
    {
        var offset = Offset(slot);
        plant.mPlantCol = col;
        plant.mRow = row;
        plant.mX = plant.mBoard.GridToPixelX(col, row) + offset.X;
        plant.mY = plant.mBoard.GridToPixelY(col, row) + offset.Y;
        int order = plant.CalcRenderOrder();
        plant.mShadowRenderOrder += order - plant.mRenderOrder;
        plant.mRenderOrder = order;
        plant.UpdateReanim();
    }

    internal static bool CanAdd(Board board, FusionPlant incoming, int col, int row, Plant? source = null)
        => TryPlacementSlot(board, incoming, col, row, out _, source);

    internal static bool TryPlacementSlot(Board board, FusionPlant incoming, int col, int row,
        out int slot, Plant? source = null)
    {
        slot = -1;
        if (!IsSmall(incoming) || col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()
            || board.mApp.GameScene != GameScenes.Playing || board.mApp.GameMode == GameMode.ChallengeZenGarden) return false;
        var plants = Plants(board, col, row).Where(p => p.Pointer != source?.Pointer).ToList();
        if (plants.Count >= 3 || plants.Any(p => !IsSmall(FusionPlant.From(p)))
            || CheckTerrain(board, col, row, incoming.NativeType, plants) != PlantingReason.Ok) return false;
        int hover = Hover(board, col, row);
        slot = plants.All(p => Slot(p) != hover) ? hover
            : Enumerable.Range(0, 3).First(candidate => plants.All(p => Slot(p) != candidate));
        return true;
    }

    // Scripted reproduction uses a free slot, independent of the mouse. Reuse
    // the same occupant and native terrain rules as cards and the glove.
    internal static bool TrySpawn(Board board, FusionPlant identity, int col, int row)
        => TrySpawn(board, identity, col, row, out _);

    internal static bool TrySpawn(Board board, FusionPlant identity, int col, int row, out Plant? spawned)
    {
        spawned = null;
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return false;
        var plants = Plants(board, col, row);
        if (!IsSmall(identity) || plants.Count >= 3 || plants.Any(p => !IsSmall(FusionPlant.From(p)))
            || CheckTerrain(board, col, row, identity.NativeType, plants) != PlantingReason.Ok) return false;
        int slot = Enumerable.Range(0, 3).First(s => plants.All(p => Slot(p) != s));
        spawned = board.AddPlant(col, row, identity.NativeType, identity.Marker);
        if (spawned == null || spawned.mDead || !spawned.mIsOnBoard) return false;
        Position(spawned, col, row, slot);
        return true;
    }

    internal static PlantingReason CheckTerrain(Board board, int col, int row, SeedType type, IEnumerable<Plant> hidden)
    {
        var states = hidden.Select(p => (Plant: p, OnBoard: p.mIsOnBoard, Squished: p.mSquished)).ToArray();
        try
        {
            foreach (var state in states) { state.Plant.mIsOnBoard = false; state.Plant.mSquished = true; }
            return FusionService.CheckNativePlacement(board, col, row, LawnGlove.PlantPlacementRules.TerrainType(type));
        }
        finally
        {
            foreach (var state in states) { state.Plant.mIsOnBoard = state.OnBoard; state.Plant.mSquished = state.Squished; }
        }
    }
}

// CalcRenderOrder sorts native plants by horizontal position. All three small
// slots share one tile depth; the two lower slots must sort in front, including
// after loading, carrying and native render-order recalculation. Native sorting
// remaps 800000 logical steps into 65525 Unity steps: +1 can become a tie.
[HarmonyLib.HarmonyPatch(typeof(Plant), nameof(Plant.CalcRenderOrder))]
internal static class ShroomSlotRenderOrderPatch
{
    private static void Postfix(Plant __instance, ref int __result)
    {
        if (__instance.mBoard == null || !ShroomSlots.IsSmall(FusionPlant.From(__instance))) return;
        __result += (int)(__instance.mX - __instance.mBoard.GridToPixelX(__instance.mPlantCol, __instance.mRow))
            + (ShroomSlots.Slot(__instance) == 0 ? 0 : 16);
    }
}
