using Il2CppReloaded.Input;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FusionMod;

internal static class NativeUi
{
    internal static Il2CppTMPro.TMP_Text CloneCaption(Il2CppTMPro.TMP_Text source, Transform parent, string name)
    {
        var staging = new GameObject(name + "Staging");
        staging.SetActive(false);
        try
        {
            var clone = Clone(source.gameObject, staging.transform, parent, name);
            var label = clone.GetComponent<Il2CppTMPro.TMP_Text>();
            var rect = label.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0, .5f);
            rect.pivot = new Vector2(1, .5f);
            rect.anchoredPosition = new Vector2(-12, 0);
            (clone.GetComponent<LayoutElement>() ?? clone.AddComponent<LayoutElement>()).ignoreLayout = true;
            label.raycastTarget = false;
            MatchCaption(label, source);
            clone.SetActive(true);
            return label;
        }
        finally { Object.Destroy(staging); }
    }

    internal static void MatchCaption(Il2CppTMPro.TMP_Text label, Il2CppTMPro.TMP_Text source)
    {
        // Match rendered size, including different parent scales. Keep the
        // cloned font material, outline and weight instead of rebuilding them.
        label.enableAutoSizing = false;
        label.fontSize = source.fontSize;
        label.alignment = Il2CppTMPro.TextAlignmentOptions.MidlineRight;
        var parentScale = label.transform.parent.lossyScale;
        var sourceScale = source.transform.lossyScale;
        label.transform.localScale = new Vector3(sourceScale.x / parentScale.x, sourceScale.y / parentScale.y, 1);
        label.rectTransform.sizeDelta = new Vector2(Math.Max(source.rectTransform.rect.width, source.fontSize * 8),
            Math.Max(source.rectTransform.rect.height, source.fontSize * 1.5f));
    }

    // Always stage below an inactive transform. Remove native binding, profile
    // transitions and animation writers before the owned view can activate.
    internal static GameObject Clone(GameObject source, Transform staging, Transform parent, string name)
    {
        var clone = Object.Instantiate(source, staging);
        clone.name = name;
        clone.SetActive(false);
        foreach (var animator in clone.GetComponentsInChildren<Animator>(true)) Object.DestroyImmediate(animator);
        foreach (var behaviour in clone.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (behaviour.TryCast<Graphic>() != null || behaviour.TryCast<Button>() != null
                || behaviour.TryCast<LayoutGroup>() != null || behaviour.TryCast<LayoutElement>() != null
                || behaviour.TryCast<ContentSizeFitter>() != null || behaviour.TryCast<ScrollRect>() != null
                || behaviour.TryCast<Mask>() != null || behaviour.TryCast<RectMask2D>() != null
                || behaviour.TryCast<GamepadScroll>() != null || behaviour.TryCast<CanvasScaler>() != null
                || behaviour.TryCast<GraphicRaycaster>() != null) continue;
            Object.DestroyImmediate(behaviour);
        }
        clone.transform.SetParent(parent, false);
        return clone;
    }
}
