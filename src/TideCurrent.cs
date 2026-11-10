using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Source current advances in centiseconds; native walking remains on its actual
// tick. Cancel only the forward displacement that the real walking method made,
// so chill, freeze, butter and native movement guards keep their own behavior.
internal static class TideCurrent
{
    private static bool Active(Plant plant) => FusionPlant.From(plant) == FusionPlant.TideShroom
        && BeamShooters.Active(plant) && !plant.NotOnGround();
    private static bool Target(Board board, int row, Zombie zombie) => zombie.mBoard?.Pointer == board.Pointer
        && zombie.mRow == row && zombie.mInPool && !zombie.mMindControlled
        && BloverFusions.Live(board, zombie) && zombie.mPosX < 900;

    internal static void Tick(Plant plant)
    {
        if (!Active(plant) || !FusionTime.SourceStep(plant, typeof(TideCurrent))) return;
        if (plant.mTargetX < 100) ++plant.mTargetX;
        if (plant.mStartRow == 0) { plant.mStartRow = 1; BloverFusions.Blow(plant); }
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && Target(plant.mBoard, plant.mRow, zombie))
                BloverFusions.Move(zombie, Math.Min(900 - zombie.mPosX, .1f));
        }
    }

    private static bool Controlled(Zombie zombie)
    {
        var board = zombie.mBoard;
        if (board == null || board.mPaused || !Target(board, zombie.mRow, zombie)) return false;
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant != null && plant.mBoard?.Pointer == board.Pointer
                && plant.mRow == zombie.mRow && Active(plant)) return true;
        }
        return false;
    }

    internal sealed record Walking(IntPtr Board, IntPtr Pointer, uint Id, float X, float DisplayX)
    { internal bool Completed; }
    internal static Walking? Capture(Zombie zombie) => Controlled(zombie)
        ? new(zombie.mBoard.Pointer, zombie.Pointer, zombie.DataID, zombie.mPosX, zombie.mX) : null;
    internal static void Finish(Zombie zombie, Walking? state)
    {
        if (state == null || state.Completed) return;
        state.Completed = true;
        if (zombie.Pointer != state.Pointer || zombie.DataID != state.Id || zombie.mBoard?.Pointer != state.Board
            || !Controlled(zombie) || zombie.mPosX >= state.X) return;
        zombie.mPosX = state.X;
        zombie.mX = state.DisplayX;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieWalking))]
internal static class TideCurrentWalkingPatch
{
    private static void Prefix(Zombie __instance, out TideCurrent.Walking? __state) => __state = TideCurrent.Capture(__instance);
    private static void Postfix(Zombie __instance, TideCurrent.Walking? __state) => TideCurrent.Finish(__instance, __state);
    private static void Finalizer(Zombie __instance, TideCurrent.Walking? __state) => TideCurrent.Finish(__instance, __state);
}
