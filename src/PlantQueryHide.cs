using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Synchronous native query only. Never keep these flags across gameplay writes.
// Each scope owns its snapshots, so nested queries preserve the outer hide.
internal sealed class PlantQueryHide : IDisposable
{
    private sealed record State(Plant Plant, IntPtr Pointer, uint Id, IntPtr BoardPointer,
        FusionPlant Identity, bool OnBoard, bool Squished);
    private readonly List<State> states = new();
    private bool disposed;

    internal PlantQueryHide(Board board, IEnumerable<Plant> hidden)
    {
        try
        {
            foreach (var plant in hidden)
            {
                if (plant == null || plant.mDead || plant.mPlantHealth <= 0 || plant.mBoard?.Pointer != board.Pointer
                    || states.Any(s => s.Pointer == plant.Pointer)) continue;
                states.Add(new(plant, plant.Pointer, plant.DataID, board.Pointer, FusionPlant.From(plant),
                    plant.mIsOnBoard, plant.mSquished));
                plant.mIsOnBoard = false;
                plant.mSquished = true;
            }
        }
        catch { Dispose(); throw; }
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        for (int i = states.Count - 1; i >= 0; --i)
        {
            var state = states[i];
            var plant = state.Plant;
            // Never revive an expired generation, a replacement or a foreign board.
            if (plant.Pointer != state.Pointer || plant.DataID != state.Id || plant.mDead || plant.mPlantHealth <= 0
                || plant.mBoard?.Pointer != state.BoardPointer || FusionPlant.From(plant) != state.Identity) continue;
            plant.mIsOnBoard = state.OnBoard;
            plant.mSquished = state.Squished;
        }
    }
}
