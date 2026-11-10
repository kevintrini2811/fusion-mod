using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppSource.Controllers;
using UnityEngine;

namespace FusionMod;

// DrawReanim writes native color after UpdateReanim. Compose at that final
// controller writer so all body/skin materials receive the same status tint.
[HarmonyPatch(typeof(ReloadedController), nameof(ReloadedController.SetColorOverride))]
internal static class ZombieStatusColorPatch
{
    private static void Prefix(ReloadedController __instance, ref Color color)
    {
        var zombie = __instance.m_obj?.TryCast<Zombie>();
        if (zombie == null || zombie.mBoard == null) return;
        if (EmberEffects.Has(zombie)) color *= new Color(.65f, .35f, .8f, 1f);
        else if (Overheat.Has(zombie)) color *= new Color(1f, .55f, .2f, 1f);
        else if (PoisonShooters.Level(zombie) > 0) color *= new Color(.58f, 1f, .56f, 1f);
        // The argument is native base color, never a previously tinted result.
        // No persistent material override or restoration cache survives pooling.
    }
}
