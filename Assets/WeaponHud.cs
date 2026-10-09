using UnityEngine;

[DisallowMultipleComponent]
public sealed class WeaponHud : MonoBehaviour
{
    private TankFire weapon;
    private UnityEngine.UI.Image normal;
    private UnityEngine.UI.Image missile;
    private UnityEngine.UI.Image acid;
    private GameObject canvasObject;
    private TMPro.TMP_FontAsset font;
    private readonly Color selected = new Color(1f, 0.78f, 0.15f);
    private readonly Color unselected = new Color(0.25f, 0.3f, 0.38f);

    private void Awake()
    {
        weapon = GetComponent<TankFire>();
        font = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        canvasObject = new GameObject("Weapon HUD", typeof(RectTransform), typeof(Canvas),
            typeof(UnityEngine.UI.CanvasScaler));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        var row = new GameObject("Weapon Slots", typeof(RectTransform), typeof(UnityEngine.UI.HorizontalLayoutGroup));
        row.transform.SetParent(canvasObject.transform, false);
        var rect = row.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 24f);
        rect.sizeDelta = new Vector2(240f, 72f);
        var layout = row.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        layout.spacing = 12f;
        layout.childControlWidth = layout.childControlHeight = false;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;
        normal = CreateSlot(row.transform, "Normal", "N");
        missile = CreateSlot(row.transform, "Missile", "M");
        acid = CreateSlot(row.transform, "Acid", "A");
        Refresh();
    }

    private UnityEngine.UI.Image CreateSlot(Transform parent, string name, string label)
    {
        var slot = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image));
        slot.transform.SetParent(parent, false);
        slot.GetComponent<RectTransform>().sizeDelta = new Vector2(72f, 72f);
        var border = slot.GetComponent<UnityEngine.UI.Image>();
        border.raycastTarget = false;
        var inner = new GameObject("Background", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        inner.transform.SetParent(slot.transform, false);
        var innerRect = inner.GetComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(4f, 4f);
        innerRect.offsetMax = new Vector2(-4f, -4f);
        var background = inner.GetComponent<UnityEngine.UI.Image>();
        background.color = new Color(0.06f, 0.08f, 0.12f, 0.95f);
        background.raycastTarget = false;
        var textObject = new GameObject("Label", typeof(RectTransform), typeof(TMPro.TextMeshProUGUI));
        textObject.transform.SetParent(inner.transform, false);
        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        var text = textObject.GetComponent<TMPro.TextMeshProUGUI>();
        text.font = font;
        text.text = label;
        text.fontSize = 36f;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        return border;
    }

    private void LateUpdate() => Refresh();
    private void Refresh()
    {
        int current = weapon != null ? weapon.SelectedWeapon : 1;
        normal.color = current == 1 ? selected : unselected;
        missile.color = current == 2 ? selected : unselected;
        acid.color = current == 3 ? selected : unselected;
    }

    private void OnDestroy()
    {
        if (canvasObject != null) Destroy(canvasObject);
    }
}
