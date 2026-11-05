using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Both pinned games deal 20 damage per ordinary pea. Keep one conversion at
// the final damage boundary, after all family, armor and projectile modifiers.
// A universal boundary also covers poison, splash, beams and native carriers.
// Percentage health changes and instant kills are their own source mechanics.
internal static class FusionDamageScale
{
    internal const int FusionPeaDamage = 20;
    internal const int ReplantedPeaDamage = 20;

    internal static int Apply(int damage) => damage <= 0 ? damage
        : (int)Math.Min(int.MaxValue, (long)damage * ReplantedPeaDamage / FusionPeaDamage);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class FusionDamageScalePatch
{
    // Run after the existing Last-priority boost and resistance modifiers.
    [HarmonyPriority(Priority.Last - 100)]
    private static void Prefix(ref int theDamage) => theDamage = FusionDamageScale.Apply(theDamage);
}
