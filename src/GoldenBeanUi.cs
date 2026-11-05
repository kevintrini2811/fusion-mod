using SpeedMod;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FusionMod;

internal sealed class GoldenBeanUi : IDisposable
{
    private readonly GameObject root;
    private readonly TMP_Text nativeBalance, balance;
    private readonly Image image;
    private readonly Texture2D texture;
    private readonly Sprite sprite;

    private GoldenBeanUi(TMP_Text nativeLabel, RectTransform anchor)
    {
        nativeBalance = nativeLabel;
        var staging = new GameObject("FusionGoldenBeanStaging");
        staging.SetActive(false);
        root = new GameObject("FusionGoldenBean");
        var rect = root.AddComponent<RectTransform>();
        rect.SetParent(staging.transform, false);
        root.AddComponent<LayoutElement>().ignoreLayout = true;
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        try
        {
            using var stream = typeof(GoldenBeanUi).Assembly.GetManifestResourceStream("FusionMod.GoldenBean.icon.png")
                ?? throw new InvalidDataException("Missing original golden bean icon.");
            using var bytes = new MemoryStream(); stream.CopyTo(bytes);
            if (!ImageConversion.LoadImage(texture, bytes.ToArray(), true)) throw new InvalidDataException("Invalid golden bean icon.");
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
            // The slowdown anchor is a layout wrapper, not a Button. Borrow
            // the native speed button as artwork; anchor only owns position.
            var plate = NativeUi.Clone(BoardSpeed.Cycler!.m_button.gameObject, staging.transform, rect, "GoldenBeanButton");
            var button = plate.GetComponent<Button>();
            var background = button.targetGraphic
                ?? throw new InvalidOperationException("Native speed button background is unavailable.");
            foreach (var graphic in plate.GetComponentsInChildren<Graphic>(true))
                graphic.enabled = graphic.Pointer == background.Pointer;
            for (Transform part = background.transform; part != plate.transform; part = part.parent)
                part.gameObject.SetActive(true);
            background.enabled = true; background.color = Color.white;
            foreach (var group in plate.GetComponentsInChildren<CanvasGroup>(true)) group.alpha = 1;
            var plateRect = plate.GetComponent<RectTransform>();
            plateRect.anchorMin = Vector2.zero; plateRect.anchorMax = Vector2.one;
            plateRect.offsetMin = plateRect.offsetMax = Vector2.zero;
            plateRect.localScale = Vector3.one;
            var colors = button.colors;
            colors.normalColor = colors.highlightedColor = colors.selectedColor = Color.white;
            colors.pressedColor = new Color(.8f, .8f, .8f, 1); colors.colorMultiplier = 1;
            button.colors = colors;
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener((UnityAction)GoldenBean.Select);
            plate.SetActive(true);
            var icon = new GameObject("Icon").AddComponent<RectTransform>();
            icon.SetParent(rect, false);
            icon.anchorMin = new Vector2(.15f, .15f); icon.anchorMax = new Vector2(.85f, .85f);
            icon.offsetMin = icon.offsetMax = Vector2.zero;
            image = icon.gameObject.AddComponent<Image>(); image.sprite = sprite; image.preserveAspect = true;
            image.raycastTarget = false;
            balance = NativeUi.CloneCaption(BoardSpeed.Cycler!.m_textMeshProUGUI, rect, "LevelMoney");
            rect.SetParent(anchor.parent, false);
            Position(anchor);
        }
        catch { Object.Destroy(root); Object.Destroy(texture); if (sprite != null) Object.Destroy(sprite); throw; }
        finally { Object.Destroy(staging); }
    }

    internal static GoldenBeanUi? TryCreate()
    {
        var label = Object.FindObjectsOfType<TMP_Text>()
            .FirstOrDefault(text => text.name == "CoinBank_Label" && text.gameObject.activeInHierarchy);
        var speed = BoardSpeed.Cycler;
        if (label == null || speed == null) return null;
        var anchor = BoardSpeed.SlowdownAnchor;
        if (anchor == null) return null;
        return anchor.rect.width > 0 && anchor.rect.height > 0 ? new GoldenBeanUi(label, anchor) : null;
    }

    private void Position(RectTransform anchor)
    {
        var rect = root.GetComponent<RectTransform>();
        rect.SetParent(anchor.parent, false);
        rect.localScale = anchor.localScale;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, anchor.rect.width);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, anchor.rect.height);
        rect.position = anchor.TransformPoint(new Vector3(anchor.rect.center.x,
            anchor.rect.yMax + anchor.rect.height / 2 + 12, 0));
    }

    internal void Update(bool visible, bool selected, bool debug, string money)
    {
        if (!root) return;
        var speed = BoardSpeed.Cycler;
        root.SetActive(visible && speed != null);
        if (speed != null)
        {
            if (BoardSpeed.SlowdownAnchor is { } anchor) Position(anchor);
            NativeUi.MatchCaption(balance, speed.m_textMeshProUGUI);
        }
        if (debug && nativeBalance != null) nativeBalance.text = "∞";
        image.color = selected ? new Color(1, .8f, .25f) : Color.white;
        balance.text = money;
    }

    public void Dispose()
    {
        if (root) { root.SetActive(false); Object.Destroy(root); }
        Object.Destroy(sprite); Object.Destroy(texture);
    }
}
