using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class AimCrosshair : MonoBehaviour
{
    [SerializeField] private Texture2D crosshair;
    private GameObject overlay;
    private RectTransform marker;
    private bool previousCursorVisible;

    private void OnEnable()
    {
        previousCursorVisible = Cursor.visible;
        if (crosshair == null) return;
        overlay = new GameObject("Aim Crosshair", typeof(Canvas));
        Canvas canvas = overlay.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        GameObject image = new GameObject("Crosshair", typeof(RectTransform), typeof(RawImage));
        image.transform.SetParent(overlay.transform, false);
        RawImage graphic = image.GetComponent<RawImage>();
        graphic.texture = crosshair;
        graphic.raycastTarget = false;
        marker = image.GetComponent<RectTransform>();
        marker.sizeDelta = new Vector2(40, 40);
        marker.pivot = new Vector2(0.5f, 0.5f);
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (marker == null || Mouse.current == null) return;
        Vector2 position = Mouse.current.position.ReadValue();
        marker.position = position;
        bool inside = position.x >= 0 && position.y >= 0 && position.x <= Screen.width && position.y <= Screen.height;
        marker.gameObject.SetActive(inside && Application.isFocused);
    }

    private void OnDisable()
    {
        if (overlay != null) Destroy(overlay);
        Cursor.visible = previousCursorVisible;
    }
}
