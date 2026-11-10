using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class FootballPlants
{
    // Football never summons allies. This native saved/reset field records
    // flight without changing faction, target eligibility or zombie identity.
    private const int Fleeing = -450168;
    internal static bool IsFleeing(Zombie zombie) => zombie.mZombieType == ZombieType.Football
        && zombie.mSummonCounter == Fleeing && !zombie.mMindControlled;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsFootball(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.FootballHelmet ? 4000
            : identity == FusionPlant.FootballNut ? 16000 : 3000;
        if (identity == FusionPlant.FootballHelmet)
        { plant.mState = PlantState.Notready; plant.mStateCountdown = 0; }
        if (identity == FusionPlant.FootballGatling)
        {
            plant.mLaunchRate = 200;
            // Gatling has no captured zombie target; Fire receives its target
            // separately. Preserve shot progress in its saved, unused target ID.
            plant.mTargetZombieID = 0;
        }
    }
    internal static void Scare(Zombie zombie)
    {
        if (zombie.mZombieType != ZombieType.Football || zombie.mDead || zombie.IsDeadOrDying()
            || zombie.mMindControlled) return;
        zombie.mSummonCounter = Fleeing;
        // Source Scared doubles speed only while below 3 world units/sec.
        if (zombie.mVelX < 3f * 80f / 1.4168f / 100f) zombie.mVelX *= 2;
        zombie.StopEating();
        zombie.StartWalkAnim(0);
    }
    internal static bool Walk(Zombie zombie)
    {
        if (!IsFleeing(zombie) || zombie.mDead || zombie.IsDeadOrDying()) return true;
        var board = zombie.mBoard;
        if (board == null || board.mPaused || board.Pointer == CherryBombPlants.DisposingBoard) return false;
        if (zombie.mButteredCounter > 0 || zombie.mIceTrapCounter > 0) return false;
        zombie.mPosX += zombie.mVelX * (zombie.mChilledCounter > 0 ? .5f : 1);
        zombie.mX = (int)zombie.mPosX;
        if (zombie.mPosX > 900) zombie.DieNoLoot();
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class FootballInitializePatch
{
    private static void Postfix(Plant __instance) => FootballPlants.Initialize(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieWalking))]
internal static class FootballFleeWalkPatch
{
    private static bool Prefix(Zombie __instance) => FootballPlants.Walk(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.FindPlantTarget))]
internal static class FootballFleeTargetPatch
{
    private static bool Prefix(Zombie __instance, ref Plant __result)
    {
        if (!FootballPlants.IsFleeing(__instance)) return true;
        __result = null!;
        return false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.IsWalkingBackwards))]
internal static class FootballFleeDirectionPatch
{
    private static void Postfix(Zombie __instance, ref bool __result)
    { if (FootballPlants.IsFleeing(__instance)) __result = true; }
}
