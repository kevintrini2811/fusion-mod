using Il2CppReloaded.Gameplay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    private sealed partial class View
    {
        private sealed record LanternParticle(SpriteRenderer Renderer, double Born, Vector3 Origin,
            Vector3 Velocity, Vector3 Scale, Quaternion Rotation);
        private readonly List<LanternParticle> lanternParticles = new();
        private double nextLanternParticle = double.NaN;

        private void UpdateLanternShine()
        {
            // Prefab45021, ParticleSystem127356: local-scaled world-space
            // sphere,15/s,1s lifetime,0.4 speed,0.1–0.2 size, white sprite15653.
            // Replanted strips most ParticleSystem module setters. Reconstruct
            // this one profile with the existing sprite owner and source clock.
            double now = FusionTime.ElapsedSeconds(Plant.mBoard);
            const double period = 1d / 15;
            if (double.IsNaN(nextLanternParticle)) nextLanternParticle = now + period;
            if (MagnetFusions.StoredLight(Plant) < 2) nextLanternParticle = now + period;
            else
            {
                // Hidden views need no burst of particles that already expired.
                if (nextLanternParticle < now - 1)
                    nextLanternParticle += Math.Ceiling((now - 1 - nextLanternParticle) / period) * period;
                var source = transforms[Array.FindIndex(model.Nodes, node => node.Path == "LanternShine")];
                while (nextLanternParticle <= now)
                {
                    float z = 2 * System.Random.Shared.NextSingle() - 1;
                    float angle = 2 * MathF.PI * System.Random.Shared.NextSingle();
                    float planar = MathF.Sqrt(1 - z * z);
                    var direction = new Vector3(planar * MathF.Cos(angle), planar * MathF.Sin(angle), 0);
                    float radius = .68031716f * MathF.Cbrt(System.Random.Shared.NextSingle());
                    var sprite = assets.Sprites[15653];
                    var go = new GameObject("LanternShine-Particle") { layer = root!.layer };
                    go.transform.SetParent(root.transform, false);
                    var renderer = go.AddComponent<SpriteRenderer>();
                    renderer.sharedMaterial = assets.spriteMaterial;
                    renderer.sprite = sprite;
                    renderer.sortingOrder = 0;
                    float size = (.1f + .1f * System.Random.Shared.NextSingle()) / sprite.bounds.size.x;
                    // Source Local scaling ignores the prefab's0.25 parent.
                    // Our root converts source world units to Replanted once.
                    Vector3 Scale(Vector3 value) => root.transform.TransformVector(Vector3.Scale(source.localScale, value));
                    lanternParticles.Add(new(renderer, nextLanternParticle,
                        source.position + Scale(direction * radius), Scale(direction * .4f),
                        Vector3.Scale(root.transform.lossyScale, source.localScale) * size, source.rotation));
                    nextLanternParticle += period;
                }
            }
            var parentScale = root!.transform.lossyScale;
            for (int i = lanternParticles.Count - 1; i >= 0; --i)
            {
                var particle = lanternParticles[i];
                float age = (float)Math.Max(0, now - particle.Born);
                if (age >= 1)
                {
                    Object.Destroy(particle.Renderer.gameObject);
                    lanternParticles.RemoveAt(i);
                    continue;
                }
                var transform = particle.Renderer.transform;
                transform.position = particle.Origin + particle.Velocity * age;
                transform.rotation = particle.Rotation;
                transform.localScale = new Vector3(particle.Scale.x / parentScale.x,
                    particle.Scale.y / parentScale.y, particle.Scale.z / parentScale.z) * (1 - age);
                const float fade = 32768f / 65535;
                particle.Renderer.color = new Color(1, 1, 1, age <= fade ? 1 : (1 - age) / (1 - fade));
            }
        }
    }

    // The original MagnetLight prefab is one stationary, white, fading sprite.
    // It is presentation only and shares the existing board visual lifecycle.
    private sealed record MagnetLight(Board Board, GameObject Root, SpriteRenderer Sprite, double Born, float Unit);
    private static readonly List<MagnetLight> MagnetLights = new();
    internal static void ShowMagnetLight(Plant plant, float x, float y)
    {
        var owner = plant.mController;
        if (owner == null || !owner) return;
        var assets = Models.Single(a => a.Identity == FusionPlant.LumosMagnet);
        assets.Load(plant.mBoard, null);
        var root = new GameObject("Fusion-MagnetLight") { layer = owner.gameObject.layer };
        try
        {
            root.transform.SetParent(owner.transform.parent, false);
            root.transform.position = owner.transform.TransformPoint(new Vector3((x - plant.mX) * 2.82f,
                -(y - plant.mY) * 2.82f, 0));
            root.transform.rotation = owner.transform.rotation;
            var renderer = root.AddComponent<SpriteRenderer>();
            renderer.sprite = assets.Sprites[15653];
            renderer.sharedMaterial = assets.spriteMaterial;
            var native = owner.GetComponentInChildren<Renderer>(true);
            if (native != null) renderer.sortingLayerID = native.sortingLayerID;
            renderer.sortingOrder = (native != null ? native.sortingOrder : 0) + 1;
            root.transform.localScale = Vector3.zero;
            MagnetLights.Add(new(plant.mBoard, root, renderer, FusionTime.ElapsedSeconds(plant.mBoard),
                80f / 1.35f * 2.82f));
        }
        catch { Object.Destroy(root); throw; }
    }
    private static void UpdateMagnetLights()
    {
        for (int i = MagnetLights.Count - 1; i >= 0; --i)
        {
            var effect = MagnetLights[i];
            float age = (float)Math.Max(0, FusionTime.ElapsedSeconds(effect.Board) - effect.Born);
            if (age >= .3f || effect.Board.Pointer == CherryBombPlants.DisposingBoard)
            { Object.Destroy(effect.Root); MagnetLights.RemoveAt(i); continue; }
            float t = age / .3f;
            float q = Math.Clamp(t / .1f, 0, 1);
            float u = (t - .1f) / .9f;
            float size = t < .1f ? 2 * q * q * (3 - 2 * q) : 2 * (1 - .2f * u * u + .1f * u * u * u);
            effect.Root.transform.localScale = Vector3.one * (size * effect.Unit / effect.Sprite.sprite.bounds.size.x);
            effect.Sprite.color = new Color(1, 1, 1, t <= 49151f / 65535 ? 1 : (1 - t) * 65535 / 16384);
        }
    }
    private static void ClearMagnetLights(Board? board = null)
    {
        for (int i = MagnetLights.Count - 1; i >= 0; --i)
            if (board == null || MagnetLights[i].Board.Pointer == board.Pointer)
            { Object.Destroy(MagnetLights[i].Root); MagnetLights.RemoveAt(i); }
    }
}
