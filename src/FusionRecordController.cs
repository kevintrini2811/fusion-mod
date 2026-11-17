using HarmonyLib;
using Il2CppSource.Controllers;
using Il2CppReloaded.Gameplay;
using UnityEngine;
using R = FusionMod.FusionNativeTimeRecords;

namespace FusionMod;

// These private data carriers are created during a synchronous save. Unity's
// deferred Start cannot initialize their animation before that same write.
internal static class FusionRecordController
{
    private static int unityThread;

    internal static void Initialize()
    {
        unityThread = Environment.CurrentManagedThreadId;
        R.InitializeController = Prepare;
    }

    internal static void Dispose()
    {
        R.InitializeController = null;
        unityThread = 0;
    }

    private static void Owned(GridItem item)
    {
        if (unityThread == 0 || Environment.CurrentManagedThreadId != unityThread)
            throw new InvalidOperationException("Fusion record initialization requires its registered Unity thread.");
        if (item == null || item.mDead || item.DataID == 0 || item.mBoard == null
            || item.mBoard.m_gridItems.DataArrayTryToGet(item.DataID)?.Pointer != item.Pointer)
            throw new InvalidDataException("Fusion record controller belongs to a stale native item.");
        if (!Candidate(item)) throw new InvalidDataException("Cannot initialize a foreign GridItem controller.");
    }

    private static bool Candidate(GridItem item) => R.IsRecord(item)
        || FusionProjectileRecords.Loading && FusionProjectileLoad.LegacyNonce(item);

    // Native SpriteGroup.Serialize writes scalars only; its reader additionally
    // requires scene references. Complete only our private carrier's references,
    // preserving group count and the native wire format, including older saves.
    private static void PrepareGroups(GridItemController controller)
    {
        var groups = controller.m_spriteGroups
            ?? throw new IOException("Fusion record sprite groups are unavailable.");
        for (int i = 0; i < groups.Count; ++i)
        {
            var group = groups[i];
            group.sprites ??= new Il2CppSystem.Collections.Generic.List<Sprite>();
            if (group.groupTransform == null || !group.groupTransform)
            {
                var child = new GameObject("FusionRecordSpriteGroup");
                child.transform.SetParent(controller.transform, false);
                group.groupTransform = child.transform;
            }
            if (group.sprites.Count > 0 && (group.spriteRenderer == null || !group.spriteRenderer))
                group.spriteRenderer = group.groupTransform.gameObject.AddComponent<SpriteRenderer>();
            groups[i] = group;
        }
    }

    internal static void PrepareLoaded(GridItemController controller)
    {
        var item = controller.m_gridItem;
        if (item == null || !Candidate(item)) return;
        Owned(item);
        // CreateGridItemController invokes OnInit before assigning mController.
        PrepareGroups(controller);
    }

    internal static void Prepare(GridItem item)
    {
        Owned(item);
        var controller = item.mController;
        if (controller == null || !controller || controller.m_gridItem?.Pointer != item.Pointer)
            throw new IOException("Fusion record controller is unavailable or bound to another item.");
        PrepareGroups(controller);
        var animation = controller.m_animationController;
        // Native ReloadedCharacterController.Serialize permits no animation.
        if (animation == null || !animation) return;
        if (!animation.m_initialized) animation.Start();
        Owned(item);
        if (!controller || !animation || item.mController?.Pointer != controller.Pointer
            || controller.m_gridItem?.Pointer != item.Pointer || controller.m_animationController?.Pointer != animation.Pointer
            || !animation.m_initialized || animation.m_animationState == null
            || animation.m_skeletonAnimation == null || animation.m_skeletonAnimation.Skeleton == null
            || animation.m_attachmentBones == null || animation.m_previousAnimations == null
            || animation.m_currentAnimations == null)
            throw new IOException("Fusion record animation is not ready for native serialization.");
    }
}

[HarmonyPatch(typeof(GridItemController), "OnInit")]
internal static class FusionRecordControllerLoadPatch
{
    private static void Postfix(GridItemController __instance) => FusionRecordController.PrepareLoaded(__instance);
}
