using UnityEngine;

namespace FusionMod;

internal static class PuffArtworkSizing
{
    // The sprite path is known to render correctly. Enlarge all Puff imports
    // consistently, preserving their aspect ratio, bottom and horizontal anchor.
    internal static Bounds Target(Bounds sprite, float cachedScale, bool preview)
    {
        var size = sprite.size * (1.5f * (preview ? 1 : cachedScale));
        var center = preview ? new Vector3(sprite.center.x, sprite.min.y + size.y * .5f, 0)
            : new Vector3(0, size.y * .5f, 0);
        return new Bounds(center, size);
    }
}
