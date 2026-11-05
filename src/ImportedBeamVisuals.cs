using UnityEngine;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    private sealed partial class View
    {
        private Sprite? beamSprite;
        private SpriteRenderer CreateBeam(string name)
        {
            FumeSmokeAssets.Load(Plant.mBoard, null);
            beamSprite = FumeSmokeAssets.Sprites[1160];
            var go = new GameObject(name) { layer = root!.layer };
            go.transform.SetParent(root.transform, false);
            var beam = go.AddComponent<SpriteRenderer>();
            beam.sharedMaterial = assets.spriteMaterial;
            beam.sprite = beamSprite;
            beam.sortingLayerID = sorting!.sortingLayerID;
            beam.sortingOrder = 100;
            return beam;
        }
        private static void PositionBeam(SpriteRenderer beam, Vector3 start, Vector3 end, float width)
        {
            var parent = beam.transform.parent;
            var a = parent.InverseTransformPoint(start);
            var b = parent.InverseTransformPoint(end);
            var delta = b - a;
            beam.transform.localPosition = (a + b) * .5f;
            beam.transform.localRotation = Quaternion.Euler(0, 0, MathF.Atan2(delta.y, delta.x) * 180 / MathF.PI);
            beam.transform.localScale = new Vector3(delta.magnitude / beam.sprite.bounds.size.x, width / beam.sprite.bounds.size.y, 1);
        }

    }
}
