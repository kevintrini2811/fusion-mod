using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Chomper owns search, valid swallow targets, missed/large-target bites,
// the 40-second saved digestion countdown and all placement/save behavior.
// IL2CPP inlines UpdateChomper inside UpdateAbilities; Plant.Update calls that
// owner directly. Patching UpdateChomper cannot intercept actual swallowing.
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class CherryChomperSwallowPatch
{
    private static void Prefix(Plant __instance, out PlantState __state) => __state = __instance.mState;

    private static void Postfix(Plant __instance, PlantState __state)
    {
        var board = __instance.mBoard;
        if (__state != PlantState.ChomperBiting || __instance.mState != PlantState.ChomperBitingGotOne
            || FusionPlant.From(__instance) != FusionPlant.CherryChomper
            || __instance.mDead || !__instance.mIsOnBoard || __instance.mSquished
            || board == null || board.Pointer == CherryBombPlants.DisposingBoard
            || __instance.mOnBungeeState != PlantOnBungeeState.NotOnBungee || __instance.NotOnGround()
            || board.mApp == null || board.mApp.GameScene != GameScenes.Playing
            || board.mApp.GameMode == GameMode.ChallengeZenGarden) return;
        // Chomp in the source places BombCherry one cell ahead. Reuse the
        // shared cherry explosion (1800, radius two cells), including nut heal.
        CherryBombPlants.Explode(new(board, FusionPlant.CherryChomper, __instance.DataID,
            __instance.mPlantCol + 1, __instance.mRow,
            __instance.mX + __instance.mWidth / 2f + 80,
            __instance.mY + __instance.mHeight / 2f, __instance.mRenderOrder));
    }
}
