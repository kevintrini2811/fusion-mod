using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native AnimateChewSound finds the actual bite target, consumes an awake
// Hypno-shroom and calls StartMindControlled. Reward only that committed path.
internal static class SilverHypnoShroom
{
    internal sealed record Bite(Board Board, Plant Plant, uint PlantId, uint ZombieId, float X, float Y);

    internal static Bite? Capture(Zombie zombie)
    {
        if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
            || zombie.mBoard == null || zombie.mBoard.mPaused) return null;
        var plant = zombie.FindPlantTarget(ZombieAttackType.Chew);
        return plant != null && FusionPlant.From(plant) == FusionPlant.SilverHypnoShroom
            && ChomperFusions.Active(plant) && !plant.mIsAsleep
            ? new(plant.mBoard, plant, plant.DataID, zombie.DataID, plant.mX, plant.mY) : null;
    }

    internal static void Reward(Zombie zombie, Bite? bite)
    {
        if (bite == null || zombie.DataID != bite.ZombieId || !zombie.mMindControlled
            || zombie.mBoard?.Pointer != bite.Board.Pointer || bite.Plant.DataID != bite.PlantId
            || !bite.Plant.mDead) return;
        // Source CoinMoney 34 pays 100. Two ordinary gold pickups pay exactly
        // 100 in Replanted and retain that value through native save/load.
        LevelMoney.Mark(bite.Board.AddCoin(bite.X - 6, bite.Y, CoinType.Gold, CoinMotion.Coin));
        LevelMoney.Mark(bite.Board.AddCoin(bite.X + 6, bite.Y, CoinType.Gold, CoinMotion.Coin));
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.AnimateChewSound))]
internal static class SilverHypnoBitePatch
{
    private static void Prefix(Zombie __instance, bool __runOriginal, out SilverHypnoShroom.Bite? __state) =>
        __state = __runOriginal ? SilverHypnoShroom.Capture(__instance) : null;

    private static void Postfix(Zombie __instance, SilverHypnoShroom.Bite? __state) =>
        SilverHypnoShroom.Reward(__instance, __state);
}
