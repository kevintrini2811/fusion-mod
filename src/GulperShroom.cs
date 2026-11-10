using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native Chomper retains acquisition, bite/chew/swallow state and saved
// digestion. Only the source SeaChomper growth and final giant meal differ.
internal static class GulperShroom
{
    internal static bool Owns(Plant plant) => FusionPlant.From(plant) == FusionPlant.GulperShroom;
    internal static int Growth(Plant plant) => Math.Clamp(plant.mTargetX, 0, 3);
    internal static bool Low(Plant plant) => Owns(plant)
        && Growth(plant) - (plant.mTargetY > 0 ? 1 : 0) < 2;
    internal static float Scale(Plant plant)
    {
        int growth = Growth(plant);
        // Native swallow lasts 2.25s (12/12 FPS) before Ready, longer than this
        // 1s wait + .5s tween even with fertilized digestion. Each new growth
        // therefore starts at the previous target; no scale accumulator needed.
        if (plant.mTargetY > 0) return (.2f + .1f * Math.Max(0, growth - 1)) / .22f;
        float remaining = Math.Clamp(plant.mLaunchRate, 0, 50) / 50f;
        float scale = growth == 0 ? .2f + .02f * remaining
            : .2f + .1f * growth - .1f * remaining;
        return scale / .22f;
    }
    internal static int Digestion(Plant plant) => (4 - Growth(plant)) * 1000;

    internal static void Initialize(Plant plant)
    {
        if (!Owns(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mTargetX = plant.mTargetY = 0;
        // Plant.UpdateAbilities calls UpdateShooting even for Chomper. Its
        // native positive countdown is not an available scale-storage field.
        plant.mShootingCounter = 0;
        plant.mLaunchRate = 50;
        plant.mIsAsleep = false;
    }

    internal static void AfterNative(Plant plant, PlantState before)
    {
        if (!Owns(plant) || !ChomperFusions.Active(plant)
            || !FusionTime.SourceStep(plant, typeof(GulperShroom))) return;
        if (plant.mTargetY > 0)
        {
            if (--plant.mTargetY == 0) plant.mLaunchRate = 50;
        }
        else if (plant.mLaunchRate > 0) --plant.mLaunchRate;
        if (before != PlantState.ChomperDigesting && plant.mState == PlantState.ChomperDigesting)
            plant.mStateCountdown = Digestion(plant);
        if (before == PlantState.ChomperDigesting && plant.mState == PlantState.ChomperSwallowing
            && Growth(plant) < 3)
        {
            // SeaChomper.Swallow increments immediately, waits 1000ms, then
            // updates digestion/short category and starts a .5s scale tween.
            // At the cap its original .5 -> .5 tween has no visible/state
            // effect: do not invent a previous-stage wait for another meal.
            plant.mTargetX = Growth(plant) + 1;
            plant.mTargetY = 100;
        }
    }

    internal static bool GiantMeal(Plant plant, out bool committed, int biteCountdown = 0)
    {
        committed = false;
        if (!Owns(plant) || Growth(plant) != 3 || !ChomperFusions.Active(plant)
            || plant.mState != PlantState.ChomperBiting || plant.mStateCountdown != biteCountdown) return false;
        uint plantId = plant.DataID;
        var board = plant.mBoard;
        bool SameOwner() => plant.DataID == plantId && plant.mBoard?.Pointer == board.Pointer && Owns(plant)
            && ChomperFusions.Active(plant) && plant.mPlantHealth > 0 && Growth(plant) == 3
            && plant.mState == PlantState.ChomperBiting && plant.mStateCountdown == biteCountdown;
        var zombie = plant.FindTargetZombie(plant.mRow, PlantWeapon.Primary);
        if (!SameOwner()) return true;
        if (zombie == null || zombie.mZombieType is not (ZombieType.Gargantuar or ZombieType.RedeyeGargantuar)
            || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) return false;
        uint zombieId = zombie.DataID;
        // Replace only the native giant-rejection branch at the bite event.
        // UpdateAbilities also inlines this branch in the installed build.
        zombie.DieNoLoot();
        if (!SameOwner() || zombie.DataID != zombieId
            || zombie.mBoard?.Pointer != board.Pointer || !zombie.mDead) return true;
        // Native successful devouring also clears the all-burned achievement
        // condition. Board+0x239 is mOnlyBurnedZombies, not an animation flag.
        board.mOnlyBurnedZombies = false;
        plant.mState = PlantState.ChomperBitingGotOne;
        board.mApp.SoundSystem.PlayFoley(FoleyType.Chomp);
        committed = true;
        return true;
    }

    internal static void Fertilize(Plant plant)
    {
        if (Owns(plant) && plant.mState == PlantState.ChomperDigesting && plant.mStateCountdown > 50)
            plant.mStateCountdown = 50;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class GulperInitializePatch
{ private static void Postfix(Plant __instance) => GulperShroom.Initialize(__instance); }

[HarmonyPatch]
internal static class GulperNativeChomperPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        typeof(Plant).GetMethod(nameof(Plant.UpdateAbilities))!,
        typeof(Plant).GetMethod(nameof(Plant.UpdateChomper))!
    };
    private static bool Prefix(Plant __instance, MethodBase __originalMethod, bool __runOriginal,
        out (PlantState State, uint Id, bool Run) __state)
    {
        __state = (__instance.mState, __instance.DataID, __runOriginal
            && GulperShroom.Owns(__instance) && FusionTime.IsSourcePulse(__instance.mBoard));
        if (!__state.Run) return true;
        var board = __instance.mBoard;
        // UpdateAbilities decrements +0xa0 before its INLINED Chomper branch:
        // its bite event enters with1, whereas standalone UpdateChomper sees0.
        int biteCountdown = __originalMethod.Name == nameof(Plant.UpdateAbilities)
            && __instance.mStateCountdown == 1 ? 1 : 0;
        if (!GulperShroom.GiantMeal(__instance, out bool committed, biteCountdown)) return true;
        // A committed meal changes only the bite branch. Keep other native
        // ability work (including the1->0 decrement); abort on callback damage
        // to the captured owner/phase rather than running its old native entry.
        // GotOne may also have been written by an aborted death callback.
        return committed && __instance.DataID == __state.Id && __instance.mBoard?.Pointer == board.Pointer
            && GulperShroom.Owns(__instance) && ChomperFusions.Active(__instance)
            && __instance.mPlantHealth > 0 && GulperShroom.Growth(__instance) == 3
            && __instance.mState == PlantState.ChomperBitingGotOne
            && __instance.mStateCountdown == biteCountdown;
    }
    private static void Postfix(Plant __instance, (PlantState State, uint Id, bool Run) __state)
    {
        if (__state.Run && __instance.DataID == __state.Id)
            GulperShroom.AfterNative(__instance, __state.State);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.GetPlantAttackRect))]
internal static class GulperRangePatch
{
    private static void Postfix(Plant __instance, ref UnityEngine.Rect __result)
    {
        if (!GulperShroom.Owns(__instance)) return;
        const float unit = 80f / 1.35f;
        __result.x = __instance.mX + __instance.mWidth / 2f + unit;
        __result.width = (2 + (GulperShroom.Growth(__instance) - 1) * .5f) * unit;
    }
}
