using HarmonyLib;
using System.Text.Json;
using Il2CppReloaded.Gameplay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    private static readonly Assets FumeSmokeAssets = new(FusionPlant.GutsyShroom, "FumeEffects");
    private sealed record SmokeProfile(int[] Frames, float Size, float[] Life, float[] Speed, float[] Color, float[][] Stops)
    {
        public float Emission { get; init; } = .6f;
        public float Rate { get; init; } = 60;
        public bool Radial { get; init; }
        public bool ConstantSpeed { get; init; }
        public int Streams { get; init; }
        public float OriginRadius { get; init; }
        public float Spread { get; init; } = .41074f;
        public Key[]? SpeedCurve { get; init; }
    }
    private static Dictionary<string, SmokeProfile>? smokeProfiles;
    private sealed record SmokeParticle(SpriteRenderer Renderer, float Birth, float Life, float Speed, float Y, Vector3 Direction);
    private sealed class SmokeView
    {
        internal readonly Board Board;
        internal readonly GameObject Root;
        internal readonly SmokeParticle[] Particles;
        internal readonly SmokeProfile Profile;
        internal int Tick, Age;
        internal readonly SpeedMod.VisualClock Clock = new();
        internal SmokeView(Board board, GameObject root, SmokeParticle[] particles, SmokeProfile profile)
        { Board = board; Root = root; Particles = particles; Profile = profile; Tick = board.mMainCounter; }
    }
    private static readonly List<SmokeView> SmokeViews = new();

    internal static void FumeSmoke(Plant plant, FusionPlant identity, int? row = null, float offset = 0, float degrees = 0, string? profileKey = null)
    {
        var owner = plant.mController;
        if (owner == null || !owner) return;
        FumeSmokeAssets.Load(plant.mBoard, null);
        if (smokeProfiles == null)
        {
            using var stream = FumeSmokeAssets.Resource("profiles.json");
            smokeProfiles = JsonSerializer.Deserialize<Dictionary<string, SmokeProfile>>(stream)
                ?? throw new InvalidDataException("Missing original fume profiles.");
        }
        string key = identity == FusionPlant.TriggerShroom ? "31975"
            : identity == FusionPlant.BelchShroom && plant.mTargetX > 0 ? "45293"
            : identity == FusionPlant.GloomShroom ? "38779"
            : identity == FusionPlant.FlameGloomShroom ? "38778"
            : identity == FusionPlant.FrostGloomShroom ? "38780"
            : identity == FusionPlant.SootShroom ? "71296"
            : identity == FusionPlant.PerfumeShroom ? "71301"
            : identity == FusionPlant.FrostShroom ? "71298"
            : identity == FusionPlant.StarchShroom ? "45295" : identity == FusionPlant.FoulShroom ? "71289"
            : identity == FusionPlant.FlameShroom || identity == FusionPlant.BlowtorchShroom ? "38446"
            : identity == FusionPlant.FumeSpreader ? "70217" : identity == FusionPlant.Cyclover ? "42479"
            : identity == FusionPlant.CaltropShroom ? "31972" : identity == FusionPlant.MorphShroom ? "38460"
            : identity == FusionPlant.RustShroom ? "41735" : "71299";
        var profile = smokeProfiles[profileKey ?? key];
        if (identity == FusionPlant.SharkShroom && plant.mTargetX > 0)
            profile = profile with { Speed = profile.Speed.Select(speed => speed * 2).ToArray() };
        if (identity == FusionPlant.BoostShroom) profile = profile with { Color = new[] { .16269517f, 1f, 0f, 1f } };
        var root = new GameObject("Fusion-" + FumeSmokeAssets.Name) { layer = owner.gameObject.layer };
        try
        {
            root.transform.SetParent(owner.transform.parent, false);
            // The controller origin includes native rendering offsets. Emit at
            // the imported animated muzzle, in the same space as the artwork.
            if (Views.TryGetValue(owner.Pointer, out var view) && view.IsValid && view.TryFumeOrigin(out var origin, identity == FusionPlant.Cyclover ? "Particle System" : "Shoot"))
                root.transform.position = origin;
            else
            {
                var ground = owner.m_skinController;
                var anchor = ground != null && ground ? ground.transform : owner.m_meshRenderer.transform;
                root.transform.position = anchor.TransformPoint(new Vector3(50 * 2.82f, 45 * 2.82f, 0));
            }
            float rowOffset = row is { } r ? plant.mBoard.GridToPixelY(plant.mPlantCol, r)
                - plant.mBoard.GridToPixelY(plant.mPlantCol, plant.mRow) : 0;
            root.transform.position += owner.transform.TransformVector(new Vector3(offset * 2.82f, -rowOffset * 2.82f, 0));
            root.transform.rotation = owner.transform.rotation * Quaternion.Euler(0, 0, degrees);
            AddSmoke(plant.mBoard, root, plant.mRenderOrder + 1, profile);
        }
        catch { Object.Destroy(root); throw; }
    }

    private static void AddSmoke(Board board, GameObject root, int order, SmokeProfile profile)
    {
        float unit = 80f / 1.35f * 2.82f;
        var random = new System.Random(board.mMainCounter ^ order);
        var particles = new SmokeParticle[(int)Math.Ceiling(profile.Emission * profile.Rate)];
        for (int i = 0; i < particles.Length; ++i)
        {
            var child = new GameObject("Smoke") { layer = root.layer };
            child.transform.SetParent(root.transform, false);
            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = FumeSmokeAssets.Sprites[profile.Frames[0]];
            renderer.sharedMaterial = FumeSmokeAssets.spriteMaterial;
            renderer.sortingOrder = order; renderer.enabled = false;
            child.transform.localScale = Vector3.one * (profile.Size * unit / renderer.sprite.bounds.size.x);
            float angle = profile.Streams > 0 ? i % profile.Streams * MathF.PI * 2 / profile.Streams
                : profile.Radial ? (float)random.NextDouble() * MathF.PI * 2 : 0;
            // All source Gloom emitters start together, each at its own angle.
            float birth = profile.Streams > 0 ? (i / profile.Streams) / (profile.Rate / profile.Streams) : i / profile.Rate;
            particles[i] = new(renderer, birth,
                profile.Life[0] + (float)random.NextDouble() * (profile.Life[1] - profile.Life[0]),
                (profile.Speed[0] + (float)random.NextDouble() * (profile.Speed[1] - profile.Speed[0])) * unit,
                ((float)random.NextDouble() - .5f) * profile.Spread * unit,
                new Vector3(MathF.Cos(angle), MathF.Sin(angle), 0));
        }
        SmokeViews.Add(new(board, root, particles, profile));
    }

    internal static void GarlicCloud(Board board, float x, float y, int order)
    {
        var reference = Enumerable.Range(0, board.m_zombies.Count).Select(i => board.m_zombies[i])
            .FirstOrDefault(z => z != null && z.mController != null);
        if (reference == null) return;
        var owner = reference.mController;
        FumeSmokeAssets.Load(board, null);
        if (smokeProfiles == null)
        {
            using var stream = FumeSmokeAssets.Resource("profiles.json");
            smokeProfiles = JsonSerializer.Deserialize<Dictionary<string, SmokeProfile>>(stream)!;
        }
        var root = new GameObject("Fusion-GarlicCloud") { layer = owner.gameObject.layer };
        try
        {
            root.transform.SetParent(owner.transform.parent, false);
            root.transform.rotation = owner.transform.rotation;
            root.transform.position = owner.m_transform.position + owner.m_transform.parent.TransformVector(
                new Vector3((x - reference.X) * 2.82f, -(y - reference.Y) * 2.82f, 0));
            AddSmoke(board, root, order, smokeProfiles["75459"]);
        }
        catch { Object.Destroy(root); throw; }
    }

    private sealed partial class View
    {
        internal bool TryFumeOrigin(out Vector3 position, string nodeName = "Shoot")
        {
            position = default;
            if (root == null || !root) return false;
            int index = Array.FindIndex(model.Nodes, node => node.Name == nodeName);
            if (index < 0 && assets.Identity == FusionPlant.CaltropShroom)
                index = Array.FindIndex(model.Nodes, node => node.Name == "FumeShroom_spout");
            if (index < 0) return false;
            position = transforms[index].position;
            return true;
        }
    }

    private static float IntegratedSpeed(Key[] keys, float time)
    {
        float total = 0;
        for (int i = 0; i < keys.Length && keys[i].Time < time; ++i)
        {
            float end = i + 1 < keys.Length ? Math.Min(time, keys[i + 1].Time) : time;
            float dt = end - keys[i].Time;
            var c = keys[i].Coefficients;
            total += dt * (c[3] + dt * (c[2] / 2 + dt * (c[1] / 3 + dt * c[0] / 4)));
        }
        return total;
    }

    internal static void UpdateFumeSmoke()
    {
        for (int i = SmokeViews.Count - 1; i >= 0; --i)
        {
            var view = SmokeViews[i];
            int ticks = Math.Max(0, view.Board.mMainCounter - view.Tick);
            view.Tick = view.Board.mMainCounter;
            if (!view.Root || !ScaredContent.Playing(view.Board)
                || view.Age >= (view.Profile.Emission + view.Profile.Life[1]) * 100)
            {
                if (view.Root) Object.Destroy(view.Root);
                SmokeViews.RemoveAt(i);
                continue;
            }
            if (view.Board.mPaused || ticks == 0) continue;
            view.Age += ticks;
            float visualAge = view.Clock.Sample("smoke", view.Age * .01f);
            foreach (var particle in view.Particles)
            {
                float age = visualAge - particle.Birth;
                particle.Renderer.enabled = age >= 0 && age < particle.Life;
                if (!particle.Renderer.enabled) continue;
                float normalized = age / particle.Life;
                float u = Math.Clamp((normalized - .4f) / .6f, 0, 1);
                // Integral of the original zero-tangent speed curve: 1 until
                // .4 life, then cubic easing to zero at the end of life.
                float distance = view.Profile.ConstantSpeed || normalized < .4f ? normalized
                    : .4f + .6f * (u - u * u * u + .5f * u * u * u * u);
                if (view.Profile.SpeedCurve is { } curve) distance = IntegratedSpeed(curve, normalized);
                float radius = view.Profile.OriginRadius * (80f / 1.35f * 2.82f);
                var lateral = view.Profile.Streams > 0
                    ? new Vector3(-particle.Direction.y, particle.Direction.x, 0) : Vector3.up;
                particle.Renderer.transform.localPosition = particle.Direction * (radius + particle.Speed * particle.Life * distance)
                    + lateral * particle.Y;
                var profile = view.Profile;
                particle.Renderer.sprite = FumeSmokeAssets.Sprites[profile.Frames[Math.Min(profile.Frames.Length - 1,
                    (int)(normalized * profile.Frames.Length))]];
                var stops = profile.Stops;
                int stop = 1;
                while (stop < stops.Length - 1 && normalized > stops[stop][0]) ++stop;
                var left = stops[stop - 1]; var right = stops[stop];
                float blend = right[0] > left[0] ? Mathf.Clamp01((normalized - left[0]) / (right[0] - left[0])) : 1;
                var color = profile.Color;
                particle.Renderer.color = new Color(
                    color[0] * Mathf.Lerp(left[1], right[1], blend), color[1] * Mathf.Lerp(left[2], right[2], blend),
                    color[2] * Mathf.Lerp(left[3], right[3], blend), color[3] * Mathf.Lerp(left[4], right[4], blend));
            }
        }
    }

    internal static void ClearFumeSmoke(Board? board = null)
    {
        for (int i = SmokeViews.Count - 1; i >= 0; --i)
            if (board == null || SmokeViews[i].Board.Pointer == board.Pointer)
            {
                if (SmokeViews[i].Root) Object.Destroy(SmokeViews[i].Root);
                SmokeViews.RemoveAt(i);
            }
        if (board == null) FumeSmokeAssets.ReleaseAssets();
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class FumeSmokeCleanupPatch
{
    private static void Prefix(Board __instance) => ImportedPeaVisuals.ClearFumeSmoke(__instance);
}
