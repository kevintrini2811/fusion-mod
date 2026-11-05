using Il2CppReloaded.Gameplay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    private static readonly Assets FalloutSplatAssets = new(FusionPlant.FalloutBlover, "FalloutSplat");
    private sealed record FalloutSplat(Board Board, GameObject Root, SpriteRenderer[] Parts, Vector3[] Velocity, int Born);
    private static readonly List<FalloutSplat> FalloutSplats = new();

    internal static void ShowFalloutSplat(Zombie zombie)
    {
        var owner = zombie.mController;
        if (owner == null || !owner) return;
        FalloutSplatAssets.Load(zombie.mBoard, null);
        var root = new GameObject("Fusion-DoomSplat") { layer = owner.gameObject.layer };
        try
        {
            root.transform.SetParent(owner.transform.parent, false);
            root.transform.position = owner.transform.position;
            root.transform.rotation = owner.transform.rotation;
            root.transform.localScale = Vector3.one * .5f; // Original DoomSplat prefab.
            float unit = 80f / 1.35f * 2.82f;
            root.transform.position += owner.transform.TransformVector(new Vector3(0, .6f * unit, 0));
            var parts = new SpriteRenderer[8];
            var velocity = new Vector3[8];
            var random = new System.Random(unchecked((int)zombie.DataID ^ zombie.mBoard.mMainCounter));
            var native = owner.GetComponentInChildren<Renderer>(true);
            for (int i = 0; i < parts.Length; ++i)
            {
                var child = new GameObject("DoomSplat") { layer = root.layer };
                child.transform.SetParent(root.transform, false);
                var part = child.AddComponent<SpriteRenderer>(); parts[i] = part;
                part.sharedMaterial = FalloutSplatAssets.spriteMaterial;
                part.sprite = FalloutSplatAssets.Sprites[16294];
                child.transform.localScale = Vector3.one * (unit / part.sprite.bounds.size.x);
                if (native != null) part.sortingLayerID = native.sortingLayerID;
                part.sortingOrder = zombie.mRenderOrder + 1;
                float angle = (float)random.NextDouble() * MathF.PI * 2;
                velocity[i] = new Vector3(MathF.Cos(angle), MathF.Sin(angle), 0) * (3 * unit);
            }
            FalloutSplats.Add(new(zombie.mBoard, root, parts, velocity, zombie.mBoard.mMainCounter));
        }
        catch { Object.Destroy(root); throw; }
    }
    private static void UpdateFalloutSplats()
    {
        for (int i = FalloutSplats.Count - 1; i >= 0; --i)
        {
            var effect = FalloutSplats[i];
            float age = (effect.Board.mMainCounter - effect.Born) * .01f;
            if (age >= .5f || effect.Board.Pointer == CherryBombPlants.DisposingBoard)
            { Object.Destroy(effect.Root); FalloutSplats.RemoveAt(i); continue; }
            if (effect.Board.mPaused) continue;
            for (int p = 0; p < effect.Parts.Length; ++p)
            {
                var part = effect.Parts[p];
                part.transform.localPosition = effect.Velocity[p] * age;
                // Original SizeModule shrinks linearly from one to zero.
                part.transform.localScale = Vector3.one * ((1 - age * 2) * 80f / 1.35f * 2.82f / part.sprite.bounds.size.x);
            }
        }
    }
    private static void ClearFalloutSplats(Board? board = null)
    {
        for (int i = FalloutSplats.Count - 1; i >= 0; --i)
            if (board == null || FalloutSplats[i].Board.Pointer == board.Pointer)
            { Object.Destroy(FalloutSplats[i].Root); FalloutSplats.RemoveAt(i); }
        if (board == null) FalloutSplatAssets.ReleaseAssets();
    }
}
