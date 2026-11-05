using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Plantern already clears the original 5x7 fog footprint without corners.
// PumpkinFusions.LightLevel already supplies its additive 3x3 level-one light.
// Source Fusion 4.0.5.2 differs in its 15-second recharge (25 sun is native).
[HarmonyPatch(typeof(Plant), nameof(Plant.GetRefreshTime))]
internal static class PlanternRechargePatch
{
    private static void Postfix(SeedType theSeedType, SeedType theImitaterType, ref int __result)
    {
        if (theSeedType == SeedType.Plantern || theSeedType == SeedType.Imitater && theImitaterType == SeedType.Plantern)
            __result = 1500;
    }
}
