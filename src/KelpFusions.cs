using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native Tanglekelp owns aquatic placement, acquisition, grabbing, drowning,
// death and serialization. Only the two source death effects differ.
internal static class KelpFusions
{
    internal static CherryBombPlants.Death? Capture(Plant plant, bool runOriginal)
    {
        var identity = FusionPlant.From(plant);
        if (!runOriginal || !FusionPlant.IsKelp(identity) || !CherryBombPlants.CanTriggerDeath(plant)) return null;
        return new(plant.mBoard, identity, plant.DataID, plant.mPlantCol, plant.mRow,
            plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight / 2f, plant.mRenderOrder);
    }

    internal static void OnDeath(CherryBombPlants.Death death)
    {
        var board = death.Board;
        bool fire = death.Identity == FusionPlant.SpicyKelp;
        if (fire) ElementalNuts.Heal(board, true, death.Row);
        DoomEffects.Damage(board, death.Row, death.X, death.Y, death.Order, fire);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class KelpFusionDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state) =>
        __state = KelpFusions.Capture(__instance, __runOriginal);

    internal static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer) KelpFusions.OnDeath(__state);
    }
}
