using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Replacement ability drivers must retain the common native coffee lifecycle.
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class MushroomWakePatch
{
    private static void Postfix(Plant __instance, bool __runOriginal)
    {
        if (FusionPlant.IsPuffJumper(FusionPlant.From(__instance)) && !FusionTime.IsSourcePulse(__instance.mBoard)) return;
        if (__runOriginal || !ChomperFusions.Active(__instance) || __instance.mWakeUpCounter <= 0) return;
        if (--__instance.mWakeUpCounter == 60)
            __instance.mApp.SoundSystem.PlayFoley(FoleyType.Wakeup);
        if (__instance.mWakeUpCounter == 0) __instance.SetSleeping(false);
    }
}

// Hail has no native shooter. Contact belongs to the ability tick, including
// ticks with no eligible shooting target or while the launch timer is running.
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class HailContactPatch
{
    private static void Postfix(Plant __instance)
    {
        if (FusionPlant.From(__instance) == FusionPlant.HailShroom
            && ChomperFusions.Active(__instance) && !__instance.mIsAsleep)
            PuffSpecials.ContactFreeze(__instance);
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateReanim))]
internal static class SmallZombieGroundPatch
{
    private static void Postfix(Zombie __instance)
    {
        if (!PuffSupport.IsSmall(__instance) || __instance.mController == null) return;
        // Native UpdateReanim writes the overlay afresh each tick. Its 120px
        // bitmap compensation translates a bottom-anchored Spine skeleton
        // downwards after shrinking. Preserve native imp/digger base scales.
        float original = __instance.mScaleZombie / .7f;
        var matrix = __instance.mController.m_overlayMatrix;
        matrix.m12 -= 120 * (original - __instance.mScaleZombie);
        __instance.mController.m_overlayMatrix = matrix;
    }
}
