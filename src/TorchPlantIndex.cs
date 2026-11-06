using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Crossing rules still inspect live identity, position and eligibility. Only
// the potential host population is indexed, once per board tick. In particular,
// marker writes after PlantInitialize do not affect this coarse native-type index.
internal static class TorchPlantIndex
{
    internal readonly record struct Candidate(Plant Plant, uint Id)
    {
        internal bool IsCurrent(Board board) => (uint)Plant.DataID == Id && Plant.mBoard?.Pointer == board.Pointer;
    }

    private static Candidate[] plants = Array.Empty<Candidate>();
    private static IntPtr owner;
    private static int tick, count;
    private static bool valid;

    internal static Candidate[] For(Board board)
    {
        var source = board.m_plants;
        int currentCount = source.Count, currentTick = board.mMainCounter;
        if (valid && owner == board.Pointer && tick == currentTick && count == currentCount) return plants;
        List<Candidate>? candidates = null;
        for (int i = 0; i < currentCount; ++i)
        {
            var plant = source[i];
            if (plant != null && plant.mSeedType is SeedType.Torchwood or SeedType.Potatomine)
                (candidates ??= new()).Add(new(plant, (uint)plant.DataID));
        }
        // Replace the array instead of mutating it: a nested conversion or
        // plant callback may invalidate/rebuild while an older query is active.
        plants = candidates?.ToArray() ?? Array.Empty<Candidate>();
        owner = board.Pointer; tick = currentTick; count = currentCount; valid = true;
        return plants;
    }

    internal static void Invalidate()
    {
        valid = false;
        plants = Array.Empty<Candidate>();
        owner = IntPtr.Zero;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TorchIndexInitializePatch
{
    private static void Prefix() => TorchPlantIndex.Invalidate();
    private static void Postfix() => TorchPlantIndex.Invalidate();
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class TorchIndexDeathPatch
{
    private static void Prefix() => TorchPlantIndex.Invalidate();
    private static void Postfix() => TorchPlantIndex.Invalidate();
}
[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class TorchIndexDisposePatch
{ private static void Prefix() => TorchPlantIndex.Invalidate(); }
