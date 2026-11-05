using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.U2D;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    private sealed record Skin(int Node, int Sprite, int[] Bones, float[][] Vertices,
        float[][] Weights, int[][] Indices, float[][] BindPoses, ushort[] Triangles);

    private static void PrepareSkinGeometry(Model model, Dictionary<int, Sprite> sprites)
    {
        foreach (var skin in model.Skins ?? Array.Empty<Skin>())
        {
            if (skin.Node < 0 || skin.Node >= model.Nodes.Length || model.Nodes[skin.Node].Sprite != skin.Sprite
                || skin.Vertices.Length != skin.Weights.Length || skin.Vertices.Length != skin.Indices.Length
                || skin.Bones.Length != skin.BindPoses.Length || skin.Bones.Any(i => i < 0 || i >= model.Nodes.Length)
                || skin.Triangles.Any(i => i >= skin.Vertices.Length))
                throw new InvalidDataException("Invalid original SpriteSkin geometry");
            // OverrideGeometry takes rectangle pixels, whereas SpriteSkin and
            // SetDeformableBuffer use local Unity units. Keep the skin data intact.
            var sprite = sprites[skin.Sprite];
            var rect = sprite.rect;
            var pivot = sprite.pivot;
            var vertices = skin.Vertices.Select(v => new Vector2(
                Math.Clamp(v[0] * sprite.pixelsPerUnit + pivot.x, 0, rect.width),
                Math.Clamp(v[1] * sprite.pixelsPerUnit + pivot.y, 0, rect.height))).ToArray();
            sprite.OverrideGeometry(vertices, skin.Triangles);
        }
    }

    private sealed partial class View
    {
        private readonly List<(SpriteRenderer Renderer, IntPtr Buffer, Matrix4x4[] BindPoses)> skinBuffers = new();
        private Vector3[]? blendPositions, blendScales;
        private Quaternion[]? blendRotations;

        private void BlendGrapple(string? previous, float previousTime, string current, float currentTime, float weight)
        {
            if (previous == null || weight >= 1) return;
            blendPositions ??= new Vector3[transforms.Length];
            blendScales ??= new Vector3[transforms.Length];
            blendRotations ??= new Quaternion[transforms.Length];
            ImportedPeaVisuals.Pose(model, transforms, Math.Min(previousTime, model.Clips[previous].Duration - .00001f), null,
                renderers, assets.Sprites, previous);
            for (int i = 0; i < transforms.Length; ++i)
            {
                blendPositions[i] = transforms[i].localPosition;
                blendScales[i] = transforms[i].localScale;
                blendRotations[i] = transforms[i].localRotation;
            }
            ImportedPeaVisuals.Pose(model, transforms, currentTime, null, renderers, assets.Sprites, current);
            for (int i = 0; i < transforms.Length; ++i)
            {
                transforms[i].localPosition = Vector3.Lerp(blendPositions[i], transforms[i].localPosition, weight);
                transforms[i].localScale = Vector3.Lerp(blendScales[i], transforms[i].localScale, weight);
                transforms[i].localRotation = Quaternion.Slerp(blendRotations[i], transforms[i].localRotation, weight);
            }
        }

        private unsafe void UpdateSkins()
        {
            var skins = model.Skins;
            if (skins == null || skins.Length == 0) return;
            for (int s = 0; s < skins.Length; ++s)
            {
                var skin = skins[s];
                var renderer = renderers[skin.Node]!;
                if (s >= skinBuffers.Count)
                {
                    var bind = skin.BindPoses.Select(values =>
                    {
                        var matrix = new Matrix4x4();
                        for (int row = 0; row < 4; ++row)
                            for (int col = 0; col < 4; ++col) matrix[row, col] = values[row * 4 + col];
                        return matrix;
                    }).ToArray();
                    var buffer = Marshal.AllocHGlobal(checked(skin.Vertices.Length * sizeof(Vector3)));
                    skinBuffers.Add((renderer, buffer, bind));
                }
                var state = skinBuffers[s];
                var output = new Span<Vector3>((void*)state.Buffer, skin.Vertices.Length);
                Bounds bounds = default;
                for (int v = 0; v < output.Length; ++v)
                {
                    var vertex = Vector(skin.Vertices[v]);
                    Vector3 deformed = default;
                    for (int w = 0; w < skin.Weights[v].Length; ++w)
                    {
                        float weight = skin.Weights[v][w];
                        if (weight == 0) continue;
                        int bone = skin.Indices[v][w];
                        var position = state.BindPoses[bone].MultiplyPoint3x4(vertex);
                        position = transforms[skin.Bones[bone]].TransformPoint(position);
                        deformed += transforms[skin.Node].InverseTransformPoint(position) * weight;
                    }
                    output[v] = deformed;
                    if (v == 0) bounds = new Bounds(deformed, Vector3.zero);
                    else bounds.Encapsulate(deformed);
                }
                // This native API changes positions while retaining the original
                // mesh UVs and SpriteMask interaction. Buffer lives with the view.
                SpriteRendererDataAccessExtensions.SetDeformableBuffer(renderer, (void*)state.Buffer, output.Length);
                SpriteRendererDataAccessExtensions.SetLocalAABB(renderer, bounds);
            }
        }
        private void ReleaseSkins()
        {
            foreach (var state in skinBuffers)
            {
                if (state.Renderer) SpriteRendererDataAccessExtensions.DeactivateDeformableBuffer(state.Renderer);
                Marshal.FreeHGlobal(state.Buffer);
            }
            skinBuffers.Clear();
        }
    }
}
