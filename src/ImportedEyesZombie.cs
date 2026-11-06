using ZombieLibrary;
using Il2CppReloaded.Gameplay;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSpine;
using Il2CppSpine.Unity;
using Il2CppSource.Controllers;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    private static readonly Assets EyesZombieAssets = new(FusionPlant.EyesZombieHead, "EyesZombie");
    private static readonly Dictionary<IntPtr, EyesView> EyesViews = new();
    private static readonly Dictionary<string, Sprite> EyesArmor = new();
    private static Dictionary<string, float[]>? eyesArmorFits;
    private static float[] ArmorFit(string name)
    {
        if (eyesArmorFits == null)
        {
            using var stream = typeof(ImportedPeaVisuals).Assembly.GetManifestResourceStream("FusionMod.EyesArmor.placement.json")
                ?? throw new InvalidDataException("Missing Eyes armor placement.");
            eyesArmorFits = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, float[]>>(stream)
                ?? throw new InvalidDataException("Invalid Eyes armor placement.");
        }
        return eyesArmorFits[name];
    }
    private static Sprite ArmorSprite(string name)
    {
        if (EyesArmor.TryGetValue(name, out var sprite)) return sprite;
        using var stream = typeof(ImportedPeaVisuals).Assembly.GetManifestResourceStream("FusionMod.EyesArmor." + name + ".png")
            ?? throw new InvalidDataException(name);
        using var bytes = new MemoryStream(); stream.CopyTo(bytes);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(texture, bytes.ToArray(), true))
        { Object.Destroy(texture); throw new InvalidDataException(name); }
        texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, 0), 100);
        sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        sprite.name = "EyesArmor." + name;
        EyesArmor.Add(name, sprite); return sprite;
    }

    internal static void EnsureEyesZombie(Zombie zombie)
    {
        if (zombie.mZombiePhase is ZombiePhase.ZombieBurned or ZombiePhase.ZombieMowered) return;
        if (EyesViews.TryGetValue(zombie.Pointer, out var old))
        {
            if (old.Valid) return;
            old.Dispose();
        }
        EyesViews[zombie.Pointer] = new EyesView(zombie);
    }
    internal static bool PlaceEyesPoolDuck(Zombie zombie, SpriteRenderer duck) =>
        EyesViews.TryGetValue(zombie.Pointer, out var view) && view.PlacePoolDuck(duck);

    private static readonly Dictionary<IntPtr, EyesHeadEffect> EyesHeadEffects = new();
    internal static void ReplaceEyesHead(Zombie zombie, TodParticleSystem particle)
    {
        var controller = particle.mController;
        if (controller == null || !controller) return;
        if (EyesHeadEffects.TryGetValue(controller.Pointer, out var current) && current.Valid) return;
        ResetEyesHead(controller);
        EyesHeadEffects[controller.Pointer] = new EyesHeadEffect(zombie, particle);
    }
    internal static void ResetEyesHead(EffectController controller)
    {
        if (EyesHeadEffects.Remove(controller.Pointer, out var effect)) effect.Dispose();
    }
    internal static void UpdateEyesZombies()
    {
        foreach (var (pointer, effect) in EyesHeadEffects.ToArray())
            if (!effect.Valid) { effect.Dispose(); EyesHeadEffects.Remove(pointer); }
        foreach (var (pointer, view) in EyesViews.ToArray())
        {
            if (!view.Valid) { view.Dispose(); EyesViews.Remove(pointer); continue; }
            try { view.Update(); }
            catch (Exception error)
            {
                view.Dispose(); view.Failed = true;
                FusionMod.Instance.LoggerInstance.Error($"Eyes zombie artwork failed: {error}");
            }
        }
    }
    internal static void ClearEyesZombies(Board? board = null)
    {
        foreach (var (pointer, effect) in EyesHeadEffects.ToArray())
            if (board == null || effect.Board == board.Pointer)
            { effect.Dispose(); EyesHeadEffects.Remove(pointer); }
        foreach (var (pointer, view) in EyesViews.ToArray())
            if (board == null || view.Board == board.Pointer)
            { view.Dispose(); EyesViews.Remove(pointer); }
        if (board == null)
        {
            EyesZombieAssets.ReleaseAssets();
            foreach (var sprite in EyesArmor.Values) { Object.Destroy(sprite.texture); Object.Destroy(sprite); }
            EyesArmor.Clear();
        }
    }

    private sealed class EyesHeadEffect : IDisposable
    {
        private readonly TodParticleSystem particle;
        private readonly EffectController controller;
        private readonly uint id;
        internal readonly IntPtr Board;
        private readonly GameObject root;
        private readonly List<(Renderer Renderer, bool Enabled)> native = new();
        internal bool Valid => !particle.mDead && particle.DataID == id && controller &&
            particle.mController?.Pointer == controller.Pointer && controller.m_obj?.Pointer == particle.Pointer;
        internal EyesHeadEffect(Zombie zombie, TodParticleSystem particle)
        {
            this.particle = particle; controller = particle.mController;
            id = particle.DataID; Board = zombie.mBoard.Pointer;
            var assets = Models.First(a => a.Identity == FusionPlant.EyesZombieHead);
            assets.Load(zombie.mBoard, null);
            var body = controller.GetComponentInChildren<Renderer>(true)
                ?? throw new InvalidOperationException("Native zombie head effect has no renderer.");
            root = new GameObject("FusionEyesHeadEffect") { layer = body.gameObject.layer };
            // The native mesh follows the effect's rigidbody. Borrow that exact
            // transform so native throwing, rotation, pause and pooling still own it.
            var physics = controller.m_PhysicsRoot;
            root.transform.SetParent(physics != null && physics ? physics.transform : body.transform, false);
            var renderer = root.AddComponent<SpriteRenderer>();
            renderer.sprite = assets.Sprites[DropSprite(assets)];
            renderer.sharedMaterial = assets.spriteMaterial;
            renderer.sortingLayerID = body.sortingLayerID; renderer.sortingOrder = body.sortingOrder;
            root.transform.position = body.bounds.center;
            float width = body.bounds.size.x;
            float sourceWidth = root.transform.TransformVector(new Vector3(renderer.sprite.bounds.size.x, 0, 0)).magnitude;
            if (width > 0 && sourceWidth > 0) root.transform.localScale *= .75f * width / sourceWidth;
            foreach (var part in controller.GetComponentsInChildren<Renderer>(true))
                if (part.Pointer != renderer.Pointer) { native.Add((part, part.enabled)); part.enabled = false; }
        }
        public void Dispose()
        {
            if (root) { root.SetActive(false); Object.Destroy(root); }
            foreach (var (renderer, enabled) in native) if (renderer) renderer.enabled = enabled;
            native.Clear();
        }
    }

    private sealed class EyesView : IDisposable
    {
        private readonly Zombie zombie;
        private readonly uint id;
        internal readonly IntPtr Board;
        private IntPtr controller;
        internal bool Failed;
        private GameObject? root;
        private SpriteRenderer? nativeShadow, armor;
        private bool facingRight;
        private float nativeFootprint;
        private readonly MaterialPropertyBlock colorEffects = new();
        private readonly Dictionary<int, PoolSpriteClip> poolClips = new();
        private Transform[] transforms = Array.Empty<Transform>();
        private SpriteRenderer?[] renderers = Array.Empty<SpriteRenderer?>();
        private readonly List<(Renderer Renderer, bool Enabled)> native = new();
        internal EyesView(Zombie zombie) { this.zombie = zombie; id = zombie.DataID; Board = zombie.mBoard.Pointer; }
        private Vector3 GroundPosition(ReloadedController owner)
        {
            // Swimming selects ShadowType.None. Its pooled shadow can have no
            // sprite and retains another actor's offset and position. Anchor to
            // the live controller with the Browncoat's native (23, 92) footprint.
            if (zombie.mInPool || nativeShadow == null || nativeShadow.sprite == null)
                return owner.m_transform.TransformPoint(new Vector3(23 * 2.82f, -92 * 2.82f, 0));
            return nativeShadow.transform.TransformPoint(nativeShadow.sprite.bounds.center);
        }
        internal bool Valid => zombie.DataID == id && zombie.mBoard?.Pointer == Board && !zombie.mDead
            && zombie.mZombiePhase is not (ZombiePhase.ZombieBurned or ZombiePhase.ZombieMowered)
            && ScaredContent.Eyes(zombie) && (controller == IntPtr.Zero || zombie.mController?.Pointer == controller);

        internal bool PlacePoolDuck(SpriteRenderer duck)
        {
            if (!Valid || Failed || root == null || !root) return false;
            if (duck.transform.parent?.Pointer != root.transform.Pointer)
                duck.transform.SetParent(root.transform, true);
            var model = EyesZombieAssets.model!;
            // The visible imported torso owns tube size. Hidden native duck
            // bones can retain a different pose during pool entry or reuse.
            var torso = renderers.FirstOrDefault(renderer => renderer != null
                && renderer.name == "Zombie_body-sprite" && renderer.sprite != null);
            if (torso == null) return false;
            var bounds = torso.sprite.bounds;
            var center = root.transform.InverseTransformPoint(torso.transform.TransformPoint(bounds.center));
            var xAxis = root.transform.InverseTransformVector(torso.transform.TransformVector(new Vector3(bounds.size.x, 0, 0)));
            var yAxis = root.transform.InverseTransformVector(torso.transform.TransformVector(new Vector3(0, bounds.size.y, 0)));
            float width = xAxis.magnitude * (390f / 203.37f);
            if (!float.IsFinite(width) || width <= .000001f || yAxis.sqrMagnitude <= .000001f) return false;
            duck.transform.localPosition = center - xAxis * (51.37f / 203.37f) - yAxis * (67.73f / 250f);
            duck.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(-yAxis.x, yAxis.y) * Mathf.Rad2Deg);
            float sign = xAxis.x * yAxis.y - xAxis.y * yAxis.x < 0 ? -1 : 1;
            duck.transform.localScale = new Vector3(sign * width / duck.sprite.bounds.size.x,
                width * (235f / 390f) / duck.sprite.bounds.size.y, 1);
            int middle = model.Nodes.Max(node => node.Order) + 1;
            duck.sortingLayerID = 0; // Same internal layer as the imported sprites.
            duck.sortingOrder = middle;
            for (int i = 0; i < renderers.Length; ++i)
                if (renderers[i] is { } renderer && PoolDuckVisuals.LeftArm(model.Nodes[i].Name))
                    renderer.sortingOrder = middle + 1 + model.Nodes[i].Order;
            return true;
        }

        private void UpdateArmor()
        {
            bool visible = zombie.mHasHead && zombie.mHelmHealth > 0 && zombie.mZombieType is ZombieType.TrafficCone or ZombieType.Pail;
            if (!visible) { if (armor != null) armor.enabled = false; return; }
            var head = renderers.FirstOrDefault(renderer => renderer != null && renderer.name == "anim_head1-sprite" && renderer.sprite != null);
            if (head == null) return;
            if (armor == null)
            {
                var go = new GameObject("EyesArmor") { layer = head.gameObject.layer };
                go.transform.SetParent(head.transform, false);
                armor = go.AddComponent<SpriteRenderer>();
                armor.sharedMaterial = head.sharedMaterial;
                armor.sortingOrder = EyesZombieAssets.model!.Nodes.Max(node => node.Order) + 10;
            }
            string kind = zombie.mZombieType == ZombieType.TrafficCone ? "cone" : "bucket";
            int stage = zombie.mHelmHealth * 3L > zombie.mHelmMaxHealth * 2L ? 1
                : zombie.mHelmHealth * 3L > zombie.mHelmMaxHealth ? 2 : 3;
            string name = kind + (stage == 1 ? "" : stage.ToString());
            armor.sprite = ArmorSprite(name);
            var fit = ArmorFit(name);
            var bounds = head.sprite.bounds;
            float width = bounds.size.x * fit[0];
            armor.transform.localScale = Vector3.one * (width / armor.sprite.bounds.size.x);
            armor.transform.localPosition = new Vector3(bounds.center.x + bounds.size.x * fit[1],
                bounds.min.y + bounds.size.y * fit[2], 0);
            armor.color = head.color;
            armor.enabled = head.enabled && head.gameObject.activeInHierarchy;
        }

        private void HideMissingParts(Model model)
        {
            for (int i = 0; i < model.Nodes.Length; ++i)
                if ((!zombie.mHasHead && model.Nodes[i].Name is "anim_head1" or "anim_head2" or "anim_head3" or "anim_hair")
                    || (!zombie.mHasArm && model.Nodes[i].Name is "Zombie_outerarm_hand" or "Zombie_outerarm_lower"))
                    transforms[i].gameObject.SetActive(false);
        }

        private float NativeBodyHeight(MeshRenderer reference, out int skeletonCount, out int headParts)
        {
            float min = float.MaxValue, max = float.MinValue;
            int measuredHeadParts = 0;
            var seen = new HashSet<IntPtr>();
            var vertices = new Il2CppStructArray<float>(8);
            foreach (var (renderer, enabled) in native)
            {
                if (!enabled || !renderer.gameObject.activeInHierarchy || renderer.Pointer == nativeShadow?.Pointer) continue;
                // Unity's generic parent helper fails its Cast<T> constraint in
                // the shipped IL2CPP bridge. Use the native Type overload.
                var component = renderer.GetComponentInParent(Il2CppType.Of<SkeletonRenderer>(), true);
                var skin = component == null ? null : new SkeletonRenderer(component.Pointer);
                if (skin == null || !skin || skin.skeleton == null || !seen.Add(skin.Pointer)) continue;
                var normalHead = skin.skeleton.Data.FindSkin("Head");
                var slots = skin.skeleton.DrawOrder;
                for (int index = 0; index < slots.Count; ++index)
                {
                    var slot = slots.Items[index];
                    if (slot == null || !slot.Bone.Active || slot.Data.Name is "anim_cone" or "anim_bucket") continue;
                    // The first native pose can still have only the body skin.
                    // Measure the ordinary head from the same loaded SkeletonData,
                    // on its current bones, without changing the live attachments.
                    string? headName = zombie.mHasHead ? slot.Data.Name switch
                    {
                        "anim_head1" => "Zombie_head",
                        "anim_head2" => "Zombie_jaw",
                        "anim_hair" => "Zombie_hair",
                        _ => null
                    } : null;
                    if (headName == null && slot.A <= 0) continue;
                    var attachment = headName == null ? slot.Attachment
                        : normalHead?.GetAttachment(slot.Data.Index, headName);
                    if (attachment == null) continue;
                    bool measured = false;
                    int count;
                    Il2CppStructArray<int>? triangles = null;
                    if (attachment.TryCast<RegionAttachment>() is { } region)
                    {
                        if (region.A <= 0) continue;
                        count = 8;
                        region.ComputeWorldVertices(slot, vertices, 0, 2);
                    }
                    else if (attachment.TryCast<MeshAttachment>() is { } mesh)
                    {
                        if (mesh.A <= 0) continue;
                        count = mesh.WorldVerticesLength;
                        if (count > vertices.Length) vertices = new Il2CppStructArray<float>(count);
                        mesh.ComputeWorldVertices(slot, 0, count, vertices, 0, 2);
                        triangles = mesh.Triangles;
                    }
                    else continue; // Points, bounds and clipping are not body artwork.

                    void Triangle(int ai, int bi, int ci)
                    {
                        if (ai < 0 || bi < 0 || ci < 0 || ai >= count / 2 || bi >= count / 2 || ci >= count / 2) return;
                        var a = new Vector3(vertices[ai * 2], vertices[ai * 2 + 1], 0);
                        var b = new Vector3(vertices[bi * 2], vertices[bi * 2 + 1], 0);
                        var c = new Vector3(vertices[ci * 2], vertices[ci * 2 + 1], 0);
                        float area = Vector3.Cross(b - a, c - a).sqrMagnitude;
                        if (!float.IsFinite(area) || area <= .00000001f) return;
                        for (int corner = 0; corner < 3; ++corner)
                        {
                            var point = corner == 0 ? a : corner == 1 ? b : c;
                            float y = reference.transform.InverseTransformPoint(skin.transform.TransformPoint(point)).y;
                            if (!float.IsFinite(y)) continue;
                            min = Math.Min(min, y); max = Math.Max(max, y);
                            measured = true;
                        }
                    }
                    if (triangles == null) { Triangle(0, 1, 2); Triangle(2, 3, 0); }
                    else for (int i = 0; i + 2 < triangles.Length; i += 3)
                        Triangle(triangles[i], triangles[i + 1], triangles[i + 2]);
                    if (headName != null && measured) ++measuredHeadParts;
                }
            }
            skeletonCount = seen.Count;
            headParts = measuredHeadParts;
            // Never calibrate a complete imported zombie against a headless
            // native initialization pose. Retry while its head bones are pending.
            if (zombie.mHasHead && measuredHeadParts < 3) return 0;
            // ComputeWorldVertices already includes the SkeletonDataAsset
            // scale (Browncoat: .564), the active skin and the current pose.
            // It remains available after Unity discards the CPU mesh buffers.
            return max > min ? reference.transform.TransformVector(new Vector3(0, max - min, 0)).magnitude : 0;
        }

        private readonly FusionVisualClock visualClock = new();

        internal void Update()
        {
            if (Failed) return;
            var owner = zombie.mController;
            if (owner == null || !owner || !ScaredContent.Playing(zombie.mBoard)) return;
            var assets = EyesZombieAssets;
            if (root == null)
            {
                var body = owner.GetComponentInChildren<MeshRenderer>(true);
                if (body == null || !body) return;
                nativeShadow = owner.m_shadowController?.m_spriteRenderer;
                if (nativeShadow == null || !nativeShadow) return;
                assets.Load(zombie.mBoard, null);
                var model = assets.model!;
                // Capture before adding imported renderers. Never hide native art
                // until every source sprite, curve and footprint is available.
                var butter = zombie.mButterReanim?.mController;
                foreach (var renderer in owner.GetComponentsInChildren<MeshRenderer>(true))
                    if (butter == null || !butter || !renderer.transform.IsChildOf(butter.transform))
                        native.Add((renderer, renderer.enabled));
                native.Add((nativeShadow, nativeShadow.enabled));
                float nativeHeight = NativeBodyHeight(body, out int skeletonCount, out int headParts);
                if (!float.IsFinite(nativeHeight) || nativeHeight <= 0)
                {
                    // The native renderer may precede its initialized pose.
                    // Retry next frame, before creating or hiding any artwork.
                    native.Clear();
                    return;
                }
                controller = owner.Pointer;
                root = new GameObject("Fusion-EyesZombie") { layer = body.gameObject.layer };
                root.transform.SetParent(owner.transform, false);
                root.transform.rotation = body.transform.rotation;
                var group = root.AddComponent<SortingGroup>();
                group.sortingLayerID = body.sortingLayerID; group.sortingOrder = body.sortingOrder;
                transforms = new Transform[model.Nodes.Length];
                renderers = new SpriteRenderer?[model.Nodes.Length];
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    var node = model.Nodes[i];
                    var go = new GameObject(node.Name) { layer = root.layer };
                    transforms[i] = go.transform;
                    go.transform.SetParent(node.Parent < 0 ? root.transform : transforms[node.Parent], false);
                    if (node.Sprite < 0) continue;
                    var renderer = go.AddComponent<SpriteRenderer>(); renderers[i] = renderer;
                    renderer.sprite = assets.Sprites[node.Sprite]; renderer.sharedMaterial = assets.spriteMaterial;
                    renderer.sortingOrder = node.Order; renderer.flipX = node.FlipX; renderer.flipY = node.FlipY;
                }
                Pose(model, transforms, 0, null, renderers, assets.Sprites, "walk");
                HideMissingParts(model);
                var shadow = renderers[model.GroundNode]!;
                float width = shadow.transform.TransformVector(new Vector3(shadow.sprite.bounds.size.x, 0, 0)).magnitude;
                float targetWidth = nativeShadow.sprite == null ? 0
                    : nativeShadow.transform.TransformVector(new Vector3(nativeShadow.sprite.bounds.size.x, 0, 0)).magnitude;
                if (!float.IsFinite(width) || width <= 0)
                    throw new InvalidDataException("Eyes zombie has no source footprint.");
                // Measure the actual normal zombie renderer in this scene.
                // Compare complete bodies in the same world coordinate space.
                float min = float.MaxValue, max = float.MinValue;
                for (int i = 0; i < renderers.Length; ++i)
                {
                    var part = renderers[i];
                    // Pose keeps disabled effect/head nodes active in the hierarchy.
                    // Match native attachment visibility when measuring body artwork.
                    if (i == model.GroundNode || part == null || !part.enabled
                        || !part.gameObject.activeInHierarchy || part.sprite == null
                        || AnimatedAlpha(model, i, "walk", 0, "", null) <= 0) continue;
                    var bounds = part.sprite.bounds;
                    for (int corner = 0; corner < 4; ++corner)
                    {
                        var point = bounds.center + Vector3.Scale(bounds.extents,
                            new Vector3((corner & 1) == 0 ? -1 : 1, (corner & 2) == 0 ? -1 : 1, 0));
                        float y = root.transform.InverseTransformPoint(part.transform.TransformPoint(point)).y;
                        min = Math.Min(min, y); max = Math.Max(max, y);
                    }
                }
                // Sprite-local Y follows the body even when the board is rotated.
                // World AABB Y can be almost zero (edge-on) or include depth.
                float sourceHeight = root.transform.TransformVector(new Vector3(0, max - min, 0)).magnitude;
                float bodyScale = nativeHeight / sourceHeight;
                if (!float.IsFinite(nativeHeight) || nativeHeight <= 0 || !float.IsFinite(sourceHeight)
                    || sourceHeight <= 0 || !float.IsFinite(bodyScale) || bodyScale <= 0)
                    throw new InvalidDataException("Eyes zombie has no finite visible body geometry.");
                root.transform.localScale = Vector3.one * bodyScale;
                FusionMod.Instance.LoggerInstance.Msg($"Eyes zombie scale: nativeHeight={nativeHeight}, sourceHeight={sourceHeight}, scale={bodyScale}, nativeSkeletons={skeletonCount}, nativeHeadParts={headParts}, source=native-spine.");
                nativeFootprint = targetWidth;
                // The controller's skin origin is not its ground position.
                // Anchor to the actual native shadow, never the animated skin.
                facingRight = false;
            }
            if (facingRight != zombie.mMindControlled)
            {
                var scale = root.transform.localScale; scale.x = -scale.x;
                var position = root.transform.localPosition; position.x = -position.x;
                root.transform.localScale = scale; root.transform.localPosition = position;
                facingRight = zombie.mMindControlled;
            }
            var data = assets.model!;
            float time = ScaredContent.AnimationTime(zombie);
            string clip = "walk";
            if (zombie.mZombiePhase == ZombiePhase.ZombieDying) { clip = "death"; time = Math.Min(time * 2.25f, data.Clips[clip].Duration - .00001f); }
            else if (ScaredContent.Awakened(zombie))
            {
                if (ScaredContent.Raging(zombie)) clip = "rage";
                else { clip = zombie.mIsEating ? "eat" : "rage_walk";
                    time *= zombie.mIsEating ? ScaredContent.EatAnimationRate : ScaredContent.WalkAnimationRate; }
            }
            // Source rendering is separate from native Spine scaling. Apply
            // later shrink operations too, keeping the original ground anchor.
            float current = Math.Abs(root.transform.localScale.x);
            if (nativeShadow != null && nativeShadow && nativeShadow.sprite != null)
            {
                float target = nativeShadow!.transform.TransformVector(new Vector3(nativeShadow.sprite.bounds.size.x, 0, 0)).magnitude;
                if (nativeFootprint > 0 && target > 0)
                {
                    root.transform.localScale = new Vector3(Math.Sign(root.transform.localScale.x) * current * target / nativeFootprint,
                        current * target / nativeFootprint, current * target / nativeFootprint);
                    nativeFootprint = target;
                }
                else if (target > 0) nativeFootprint = target;
            }
            var order = root.GetComponent<SortingGroup>();
            var bodyRenderer = zombie.mController.GetComponentInChildren<MeshRenderer>(true);
            if (bodyRenderer != null) { order.sortingLayerID = bodyRenderer.sortingLayerID; order.sortingOrder = bodyRenderer.sortingOrder; }
            time = visualClock.Sample(clip, time);
            Pose(data, transforms, time, null, renderers, assets.Sprites, clip);
            HideMissingParts(data);
            UpdateArmor();
            if (nativeShadow != null && nativeShadow)
            {
                var ground = renderers[data.GroundNode]!;
                root.transform.position += GroundPosition(owner)
                    - ground.transform.TransformPoint(ground.sprite.bounds.center);
                // Imported artwork anchors to the ground shadow. Native pool
                // entry lowers only the actor, so transfer that saved height.
                Zombies.ApplyBodyDisplacement(zombie, root.transform);
            }
            var effects = owner.m_materialEffectController;
            bool cold = zombie.mIceTrapCounter > 0 || zombie.mChilledCounter > 0;
            // Native cold uses a solid additive blue material. Multiplying the
            // source texture preserves eyes, clothes, outlines and shading.
            Color tint = cold ? new Color(.65f, .8f, 1f, 1f)
                : effects != null && effects ? effects.m_overrideColor : Color.white;
            if (!cold && effects != null && effects && effects.m_enableOverlayColor)
            {
                var overlay = effects.m_overlayColor;
                tint = Color.Lerp(tint, overlay, overlay.a);
            }
            if (!cold && zombie.mMindControlled && effects != null && effects && effects.m_enableAdditiveColor)
                tint *= Color.Lerp(Color.white, effects.m_additiveColor, effects.m_additiveColor.a);
            Color additive = !cold && !zombie.mMindControlled && effects != null && effects && effects.m_enableAdditiveColor
                ? effects.m_additiveColor : Color.clear;
            for (int i = 0; i < data.Nodes.Length; ++i)
            {
                var node = data.Nodes[i];
                if (renderers[i] is { } renderer)
                {
                    Color color = i == data.GroundNode ? Color.white : tint;
                    renderer.color = new Color(node.Color[0] * color.r, node.Color[1] * color.g,
                        node.Color[2] * color.b, AnimatedAlpha(data, i, clip, time, "", null) * color.a);
                    renderer.GetPropertyBlock(colorEffects);
                    colorEffects.SetColor("_BlendColor", additive);
                    colorEffects.SetFloat("_BlendStrength", i == data.GroundNode ? 0 : additive.a);
                    renderer.SetPropertyBlock(colorEffects);
                }
            }
            var groundPosition = GroundPosition(owner);
            bool inWater = Zombies.BodyClipPlane(zombie, root.transform, groundPosition, out var water, out var up);
            for (int i = 0; i < renderers.Length; ++i)
            {
                if (renderers[i] is not { } renderer) continue;
                // A ground shadow must not cover the pool surface.
                if (i == data.GroundNode && zombie.mInPool) { renderer.enabled = false; continue; }
                if (!poolClips.TryGetValue(i, out var poolClip)) poolClips[i] = poolClip = new PoolSpriteClip();
                poolClip.Apply(renderer, water, up, inWater);
            }
            if (armor != null && armor.enabled)
            {
                if (!poolClips.TryGetValue(-1, out var armorClip)) poolClips[-1] = armorClip = new PoolSpriteClip();
                armorClip.Apply(armor, water, up, inWater);
            }
            var butterController = zombie.mButterReanim?.mController;
            foreach (var (renderer, _) in native)
                if (renderer && (butterController == null || !butterController
                    || !renderer.transform.IsChildOf(butterController.transform))) renderer.enabled = false;
            if (butterController != null && butterController)
                foreach (var renderer in butterController.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.enabled = zombie.mButteredCounter > 0 && zombie.mHasHead;
                    renderer.sortingLayerID = root!.GetComponent<SortingGroup>().sortingLayerID;
                    renderer.sortingOrder = root.GetComponent<SortingGroup>().sortingOrder + 1;
                }
        }
        public void Dispose()
        {
            if (root != null && root) Object.Destroy(root);
            root = null;
            foreach (var (renderer, enabled) in native) if (renderer) renderer.enabled = enabled;
            native.Clear();
            foreach (var clip in poolClips.Values) clip.Dispose();
            poolClips.Clear();
        }
    }
}

// DropHead changes native gameplay state normally. OverrideParticleScale runs
// after its head effect is created; only that effect receives the imported art.
[HarmonyPatch]
internal static class EyesHeadDropScopePatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.DropHead)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.TakeDamage)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.TakeBodyDamage)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateDeath)) };
    [ThreadStatic] internal static Zombie? Current;
    private static void Prefix(Zombie __instance, out Zombie? __state)
    { __state = Current; Current = ScaredContent.Eyes(__instance) ? __instance : null; }
    private static void Finalizer(Zombie? __state) => Current = __state;
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.OverrideParticleScale))]
internal static class EyesHeadEffectPatch
{
    private static void Postfix(Zombie __instance, TodParticleSystem aParticle)
    {
        if (ScaredContent.Eyes(__instance) && aParticle != null
            && aParticle.mEffectType is ParticleEffect.ZombieHead or ParticleEffect.ZombieHeadPool
                or ParticleEffect.ZombieHeadRetro or ParticleEffect.ZombieHeadRetroPool)
            ImportedPeaVisuals.ReplaceEyesHead(__instance, aParticle);
    }
}
[HarmonyPatch(typeof(EffectController), nameof(EffectController.OnControllerReset))]
internal static class EyesHeadEffectResetPatch
{
    private static void Prefix(EffectController __instance) => ImportedPeaVisuals.ResetEyesHead(__instance);
}

[HarmonyPatch]
internal static class EyesHeadCreatedPatch
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => typeof(Il2CppReloaded.TreeStateActivities.GameplayActivity)
        .GetMethods().Where(method => method.Name == "AddTodParticle");
    private static void Postfix(TodParticleSystem __result)
    {
        if (EyesHeadDropScopePatch.Current is { } zombie && __result != null
            && __result.mEffectType is ParticleEffect.ZombieHead or ParticleEffect.ZombieHeadPool
                or ParticleEffect.ZombieHeadRetro or ParticleEffect.ZombieHeadRetroPool)
            ImportedPeaVisuals.ReplaceEyesHead(zombie, __result);
    }
}
