using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppSource.Controllers;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    private static readonly Dictionary<IntPtr, HypnoHeadView> HypnoHeads = new();
    private static readonly Dictionary<IntPtr, (uint Id, IntPtr Head)> FailedHypnoHeads = new();

    internal static void HypnoHead(ZombieController controller)
    {
        var zombie = controller.m_zombie;
        var head = controller.m_zombotanyHead;
        bool valid = zombie != null && !zombie.mDead && zombie.mHasHead && HypnoShooters.IsSummoned(zombie)
            && head != null && !head.mDead && head.mController != null;
        if (valid && FailedHypnoHeads.TryGetValue(controller.Pointer, out var failed)
            && failed.Id == zombie!.DataID && failed.Head == head!.Pointer) return;
        FailedHypnoHeads.Remove(controller.Pointer);
        if (HypnoHeads.TryGetValue(controller.Pointer, out var previous))
        {
            if (valid && previous.Valid(zombie!, head!)) { previous.Update(); return; }
            previous.Dispose(); HypnoHeads.Remove(controller.Pointer);
        }
        if (!valid) return;
        var view = new HypnoHeadView(zombie!, head!);
        try { view.Update(); HypnoHeads[controller.Pointer] = view; }
        catch { view.Dispose(); throw; }
    }

    private sealed class HypnoHeadView : IDisposable
    {
        private readonly FusionVisualClock visualClock = new();
        private readonly Zombie zombie;
        private readonly uint id;
        private readonly Reanimation head;
        private readonly IntPtr controller;
        private readonly Assets assets;
        private GameObject? root;
        private Transform[] transforms = Array.Empty<Transform>();
        private SpriteRenderer?[] renderers = Array.Empty<SpriteRenderer?>();
        private readonly List<(Renderer Renderer, bool Enabled)> native = new();
        internal HypnoHeadView(Zombie zombie, Reanimation head)
        { this.zombie = zombie; id = zombie.DataID; this.head = head; controller = head.mController.Pointer;
            assets = Models.First(model => model.Identity == (zombie.mSummonCounter == HypnoShooters.SummonedFire ? FusionPlant.GatlingHypno : FusionPlant.HypnoPea)); }
        internal bool Valid(Zombie value, Reanimation reanimation) => value.DataID == id
            && value.Pointer == zombie.Pointer && reanimation.Pointer == head.Pointer
            && reanimation.mController?.Pointer == controller;

        internal void Update()
        {
            var owner = head.mController;
            if (owner == null || !owner) return;
            assets.Load(zombie.mBoard, null);
            var model = assets.model!;
            if (root == null)
            {
                var mesh = owner.GetComponentInChildren<MeshRenderer>(true);
                if (mesh == null || !mesh) return;
                var target = VisibleMeshBounds(mesh, owner.transform);
                if (target == null) return;
                root = new GameObject("FusionHypnoPeaZombieHead") { layer = mesh.gameObject.layer };
                root.transform.SetParent(owner.transform, false);
                var sorting = root.AddComponent<SortingGroup>();
                sorting.sortingLayerID = mesh.sortingLayerID; sorting.sortingOrder = mesh.sortingOrder;
                transforms = new Transform[model.Nodes.Length];
                renderers = new SpriteRenderer?[model.Nodes.Length];
                int headIndex = Array.FindIndex(model.Nodes, node => node.Name == (zombie.mSummonCounter == HypnoShooters.SummonedFire ? "GatlingPea_head" : "PeaShooter_Head"));
                if (headIndex < 0) throw new InvalidDataException("Hypno Pea head is missing.");
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    var node = model.Nodes[i];
                    var part = new GameObject(node.Name) { layer = root.layer };
                    transforms[i] = part.transform;
                    part.transform.SetParent(node.Parent < 0 ? root.transform : transforms[node.Parent], false);
                    int ancestor = i;
                    while (ancestor >= 0 && ancestor != headIndex) ancestor = model.Nodes[ancestor].Parent;
                    // The original nozzle is a root sibling named Shoot.
                    if ((ancestor < 0 && node.Name != "Shoot") || node.Sprite < 0) continue;
                    var renderer = part.AddComponent<SpriteRenderer>();
                    renderers[i] = renderer;
                    renderer.sprite = assets.Sprites[node.Sprite]; renderer.sharedMaterial = assets.spriteMaterial;
                    renderer.sortingOrder = node.Order; renderer.flipX = node.FlipX; renderer.flipY = node.FlipY;
                }
                Pose(model, transforms, 0, null, renderers, assets.Sprites);
                Bounds? source = null;
                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;
                    var bounds = renderer.sprite.bounds;
                    for (int corner = 0; corner < 4; ++corner)
                    {
                        var point = root.transform.InverseTransformPoint(renderer.transform.TransformPoint(
                            bounds.center + Vector3.Scale(bounds.extents, new Vector3((corner & 1) == 0 ? -1 : 1,
                                (corner & 2) == 0 ? -1 : 1, 0))));
                        if (source == null) source = new Bounds(point, Vector3.zero);
                        else { var box = source.Value; box.Encapsulate(point); source = box; }
                    }
                }
                if (source == null || source.Value.size.x <= 0 || source.Value.size.y <= 0)
                    throw new InvalidDataException("Hypno Pea head has no artwork bounds.");
                float scale = Math.Min(target.Value.size.x / source.Value.size.x, target.Value.size.y / source.Value.size.y);
                // The planted model faces opposite to the attached native head.
                // Mirror once locally; native attachment still owns faction turns.
                var headScale = new Vector3(-scale, scale, scale);
                root.transform.localScale = headScale;
                root.transform.localPosition = target.Value.center - Vector3.Scale(source.Value.center, headScale);
                foreach (var renderer in owner.GetComponentsInChildren<MeshRenderer>(true))
                    native.Add((renderer, renderer.enabled));
            }
            float idle = zombie.mBoard.mMainCounter * .01f;
            float? shoot = zombie.mPhaseCounter >= 120 ? (150 - zombie.mPhaseCounter) * .01f : null;
            idle = visualClock.Sample("idle", idle);
            if (shoot.HasValue) shoot = visualClock.Sample("shoot", shoot.Value);
            Pose(model, transforms, idle, shoot, renderers, assets.Sprites);
            foreach (var (renderer, _) in native) if (renderer) renderer.enabled = false;
        }

        public void Dispose()
        {
            if (root != null && root) { root.SetActive(false); Object.Destroy(root); }
            foreach (var (renderer, enabled) in native) if (renderer) renderer.enabled = enabled;
            native.Clear();
        }
    }

    internal static void ClearHypnoHeads()
    {
        foreach (var view in HypnoHeads.Values) view.Dispose();
        HypnoHeads.Clear();
        FailedHypnoHeads.Clear();
    }

    internal static void FailHypnoHead(ZombieController controller)
    {
        if (HypnoHeads.Remove(controller.Pointer, out var view)) view.Dispose();
        if (controller.m_zombie is { } zombie && controller.m_zombotanyHead is { } head)
            FailedHypnoHeads[controller.Pointer] = (zombie.DataID, head.Pointer);
    }
}

[HarmonyPatch(typeof(ZombieController), "Update")]
internal static class HypnoZombieHeadPatch
{
    private static void Postfix(ZombieController __instance)
    {
        try { ImportedPeaVisuals.HypnoHead(__instance); }
        catch (Exception exception)
        {
            ImportedPeaVisuals.FailHypnoHead(__instance);
            FusionMod.Instance.LoggerInstance.Error($"Hypno Pea zombie head: {exception}");
        }
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class HypnoHeadBoardCleanupPatch
{
    private static void Prefix() => ImportedPeaVisuals.ClearHypnoHeads();
}
