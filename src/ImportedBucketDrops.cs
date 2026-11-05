using Il2CppReloaded.Gameplay;
using Il2CppSource.Controllers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    // Original loose Bucket prefab 59960 uses the same sprite 15928 as the
    // planted Bucket body. No substitute icon or foreign script is required.
    internal sealed class BucketDropView : IDisposable
    {
        private readonly Board board;
        private readonly FusionPlant identity;
        private readonly ReloadedObject anchor;
        private PreviewController? controller;
        private SpriteRenderer? native;
        private bool nativeEnabled;
        private GameObject? root;
        private SpriteRenderer? renderer;
        internal BucketDropView(Board board, FusionPlant identity = default)
        {
            this.board = board;
            this.identity = identity == default ? FusionPlant.Bucket : identity;
            anchor = new ReloadedObject { mBoard = board, mApp = board.mApp, mVisible = true, mRenderOrder = 400000 };
        }
        internal void Update(float x, float y, float alpha)
        {
            anchor.mX = x;
            anchor.mY = y;
            if (controller == null || !controller)
            {
                controller = board.mApp.CreatePreviewController(FusionPlant.Bucket.NativeType, anchor);
                if (controller == null || !controller) return;
                native = controller.m_previewDrawer?.m_previewSprite;
                if (native == null || !native) throw new InvalidOperationException("Bucket drop preview has no sprite anchor.");
                nativeEnabled = native.enabled;
                var assets = Models.First(a => a.Identity == identity);
                assets.Load(board, controller);
                root = new GameObject("Fusion-BucketDrop") { layer = native.gameObject.layer };
                root.transform.SetParent(controller.transform, false);
                renderer = root.AddComponent<SpriteRenderer>();
                renderer.sprite = assets.Sprites[DropSprite(assets)];
                renderer.sharedMaterial = assets.spriteMaterial;
                renderer.sortingOrder = 32750;
                root.transform.localScale = Vector3.one * (55f * 2.82f / renderer.sprite.bounds.size.x);
                controller.SetPosition(0, 0);
                controller.ShowPreview();
                native.enabled = false;
            }
            controller.UpdatePosition();
            if (renderer != null && renderer) renderer.color = new Color(1, 1, 1, alpha);
        }
        public void Dispose()
        {
            if (root != null && root) Object.Destroy(root);
            root = null;
            if (native != null && native) native.enabled = nativeEnabled;
            native = null;
            if (controller != null && controller) controller.Die();
            controller = null;
        }
    }
}
