using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class LumosAngler
{
    private const float SourceUnit = 80f / 1.35f;

    internal static void AfterNative(Plant plant, PlantState before)
    {
        if (FusionPlant.From(plant) != FusionPlant.LumosAngler || !ChomperFusions.Active(plant)
            || !FusionTime.IsSourcePulse(plant.mBoard) || before == plant.mState) return;
        if (plant.mState == PlantState.ChomperBitingGotOne)
        {
            // Native digestion starts after the bite animation finishes.
            // Preserve the light at the bite across the animation and a save.
            plant.mTargetX = 4000 - 1000 * Math.Clamp(PumpkinFusions.LightLevel(plant), 0, 3);
        }
        else if (plant.mState == PlantState.ChomperDigesting && plant.mTargetX is > 0 and <= 4000)
        {
            plant.mStateCountdown = plant.mTargetX;
            plant.mTargetX = 0;
        }
        else if (before == PlantState.ChomperSwallowing && plant.mState == PlantState.Ready)
            Pull(plant);
    }

    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.LumosAngler) return;
        if (plant.mState == PlantState.ChomperBitingGotOne && plant.mTargetX > 50)
            plant.mTargetX = 50;
        if (plant.mState == PlantState.ChomperDigesting && plant.mStateCountdown > 50)
            plant.mStateCountdown = 50;
    }

    private static bool Eligible(Board board, Zombie zombie) =>
        zombie.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Dog);

    private static void Pull(Plant plant)
    {
        var board = plant.mBoard;
        int row = plant.mRow;
        float x = plant.mX + plant.mWidth * .5f;
        float y = plant.mY + plant.mHeight - SourceUnit * .5f;
        Zombie? candidate = null;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || !Eligible(board, zombie)) continue;
            if (zombie.mRow == row) return;
            if (candidate != null || Math.Abs(zombie.mRow - row) != 1 || !board.RowCanHaveZombies(zombie.mRow)
                || board.mPlantRow[zombie.mRow] != board.mPlantRow[row]
                || !ChomperVariants.CanSwallow(zombie) || !BloverFusions.CanRedirect(zombie)
                || Math.Abs(zombie.mPosY - zombie.GetPosYBasedOnRow(zombie.mRow)) > 1f) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
            float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= 9 * SourceUnit * SourceUnit) candidate = zombie;
        }
        if (candidate == null) return;
        candidate.StopEating();
        // Native SetRow updates lane and render order. UpdateZombiePosition
        // moves the body toward the new lane using its normal terrain offset.
        candidate.SetRow(row);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class LumosAnglerAbilitiesPatch
{
    private static void Prefix(Plant __instance, bool __runOriginal, out (PlantState State, uint Id)? __state) =>
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.LumosAngler
            ? (__instance.mState, __instance.DataID) : null;

    private static void Postfix(Plant __instance, (PlantState State, uint Id)? __state)
    {
        if (__state is { } state && __instance.DataID == state.Id)
            LumosAngler.AfterNative(__instance, state.State);
    }
}
