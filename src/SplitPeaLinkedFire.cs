using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Couple target acquisition, not individual projectiles. Native UpdateShooter
// owns the single front shot, staggered rear pair and both head animations.
[HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetAndFire))]
internal static class SplitPeaTargetScope
{
    [ThreadStatic] internal static IntPtr ActivePlant;

    private static void Prefix(Plant __instance, out IntPtr __state)
    {
        __state = ActivePlant;
        ActivePlant = __instance.mSeedType == SeedType.Splitpea ? __instance.Pointer : IntPtr.Zero;
    }

    private static void Finalizer(IntPtr __state) => ActivePlant = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
internal static class SplitPeaLinkedTargetPatch
{
    [ThreadStatic] private static bool checkingOtherSide;

    private static void Postfix(Plant __instance, int theRow, PlantWeapon thePlantWeapon,
        ref Zombie __result)
    {
        if (__result != null || checkingOtherSide || __instance.mSeedType != SeedType.Splitpea
            || SplitPeaTargetScope.ActivePlant != __instance.Pointer) return;
        var other = thePlantWeapon == PlantWeapon.Primary ? PlantWeapon.Secondary : PlantWeapon.Primary;
        checkingOtherSide = true;
        try
        {
            // A target on either side allows the existing shooting schedule for
            // both heads. Outside FindTargetAndFire, directional queries retain
            // their native meaning. No recursive fallback or additional shots.
            __result = __instance.FindTargetZombie(theRow, other);
        }
        finally { checkingOtherSide = false; }
    }
}
