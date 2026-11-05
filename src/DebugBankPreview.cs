using Il2CppReloaded.Gameplay;
using Il2CppSource.Controllers;

namespace FusionMod;

// Cursor controllers may be reused for variants with the same native SeedType.
// Synchronize the borrowed controllers by both pointer and exact bank identity.
internal static class DebugBankPreviews
{
    private static readonly Dictionary<IntPtr, (PreviewController Controller, FusionPlant Identity)> attached = new();

    internal static void Update(Board? board)
    {
        var active = new HashSet<IntPtr>();
        if (board != null && DebugMode.Owns(board) && board.mApp.GameScene == GameScenes.Playing)
        {
            var cursor = board.mCursorObject;
            if (DebugPlantCards.TryCursorIdentity(cursor, out var identity) && FusionPlant.HasImportedVisuals(identity))
            {
                void sync(PreviewController? controller)
                {
                    if (controller == null || !controller) return;
                    active.Add(controller.Pointer);
                    if (attached.TryGetValue(controller.Pointer, out var previous) && previous.Identity == identity) return;
                    ImportedPeaVisuals.RegisterDebugPreview(controller, board, identity);
                    attached[controller.Pointer] = (controller, identity);
                }
                sync(cursor.mPreviewController);
                var grid = board.mCursorPreview;
                if (grid != null)
                {
                    sync(grid.mController);
                    if (grid.mColumnControllers != null)
                        for (int i = 0; i < grid.mColumnControllers.Count; ++i) sync(grid.mColumnControllers[i]);
                }
            }
        }
        foreach (var pair in attached.Where(pair => !active.Contains(pair.Key)).ToArray())
        {
            ImportedPeaVisuals.ForgetController(pair.Value.Controller);
            attached.Remove(pair.Key);
        }
    }

    internal static void Clear()
    {
        foreach (var item in attached.Values) ImportedPeaVisuals.ForgetController(item.Controller);
        attached.Clear();
    }
}
