using Il2CppReloaded.Gameplay;
using HarmonyLib;

namespace FusionMod;

internal static partial class DebugFusionIntegration
{
    // Read only: expose this exact owner's occupancy decision for the bounded
    // Day-placement investigation, without invoking placement or changing flags.
    internal static object? Slots(Board board, int col, int row)
    {
        if (board == null || !DebugMode.Owns(board) || col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows())
            return null;
        var occupants = ShroomSlots.Plants(board, col, row);
        var candidates = new List<object>();
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant == null || plant.mPlantCol != col || plant.mRow != row) continue;
            bool sameBoard = plant.mBoard?.Pointer == board.Pointer;
            int? slot = !plant.mDead && sameBoard ? ShroomSlots.Slot(plant) : null;
            candidates.Add(new {
                id = (uint)plant.DataID, type = plant.mSeedType.ToString(), marker = plant.mImitaterType.ToString(),
                onBoard = plant.mIsOnBoard, dead = plant.mDead, squished = plant.mSquished,
                onBungee = plant.mOnBungeeState.ToString(), notOnGround = plant.NotOnGround(),
                live = ShroomSlots.Live(plant), sameBoard, slot,
                sourceState = new { squishOwned = Squish.Owns(plant), squishAirborne = Squish.Airborne(plant),
                    nibbleActivated = NibbleShroom.Activated(plant), retained = PuffJumpers.Retained(plant),
                    detached = !plant.mIsOnBoard, lifecycleVeto = plant.mDead || plant.mSquished },
                occupant = occupants.Any(p => p.Pointer == plant.Pointer), x = plant.mX, y = plant.mY
            });
        }
        return new { count = occupants.Count, occupantIds = occupants.Select(p => (uint)p.DataID).ToArray(),
            gridX = board.GridToPixelX(col, row), gridY = board.GridToPixelY(col, row), candidates, notOnGroundPatches = GroundPatches() };
    }

    private static object[] GroundPatches()
    {
        var original = typeof(Plant).GetMethod(nameof(Plant.NotOnGround), Type.EmptyTypes)!;
        var info = HarmonyLib.Harmony.GetPatchInfo(original);
        if (info == null) return Array.Empty<object>();
        IEnumerable<object> Entries(string kind, IEnumerable<Patch> patches) => patches.Select(patch => (object)new {
            kind, owner = patch.owner, priority = patch.priority, index = patch.index,
            method = patch.PatchMethod.DeclaringType?.FullName + "." + patch.PatchMethod.Name,
            assembly = patch.PatchMethod.Module.Assembly.GetName().Name,
            assemblyVersion = patch.PatchMethod.Module.Assembly.GetName().Version?.ToString(),
            before = patch.before, after = patch.after
        });
        return Entries("prefix", info.Prefixes).Concat(Entries("postfix", info.Postfixes))
            .Concat(Entries("transpiler", info.Transpilers)).Concat(Entries("finalizer", info.Finalizers)).ToArray();
    }

}
