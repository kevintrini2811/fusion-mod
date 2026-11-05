using HarmonyLib;
using UnityEngine;

namespace FusionMod;

// Scene changes release renderer assets before Application.quitting. Enforce
// the camera/texture invariant at the actual native release boundary as well.
[HarmonyPatch(typeof(RenderTexture), nameof(RenderTexture.Release))]
internal static class RenderTextureReleasePatch
{
    private static void Prefix(RenderTexture __instance) => FusionMod.DetachCameraTarget(__instance);
}

[HarmonyPatch(typeof(RenderTexture), nameof(RenderTexture.ReleaseTemporary))]
internal static class RenderTextureTemporaryReleasePatch
{
    private static void Prefix(RenderTexture __0)
    { if (__0 != null) FusionMod.DetachCameraTarget(__0); }
}
