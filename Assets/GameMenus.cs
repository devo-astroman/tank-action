using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using TMPro;

[DefaultExecutionOrder(-1000)]
public sealed class GameMenus : MonoBehaviour
{
    public static bool IsPlaying { get; private set; }
    private static bool startOnLoad;
    private static GameMenus instance;
    private GameObject canvasObject;
    private GameObject main, credits, destroyed;
    private TextMeshProUGUI finalScore;
    private TMP_FontAsset font;

    private void Awake()
    {
        instance = this;
        IsPlaying = false;
        Time.timeScale = 1;
        font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("Menu Event System", typeof(EventSystem), typeof(InputSystemUIInputModule));
        canvasObject = new GameObject("Menus", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.GetComponent<Canvas>().sortingOrder = 200;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        main = Screen("Main Menu");
        Label(main, "Tank in action", 200, 52);
        Button(main, "Start", 20, StartGame);
        Button(main, "Credits", -65, ShowCredits);
        credits = Screen("Credits");
        Label(credits, "Credits", 200, 44);
        Label(credits, "Created by @DevoAstroman\n\nThanks for playing", 45, 30);
        Button(credits, "Back", -130, ShowMain);
        destroyed = Screen("Restart Menu");
        Label(destroyed, "You have been destroyed", 170, 44);
        finalScore = Label(destroyed, "Score 0", 65, 32);
        Button(destroyed, "Restart", -30, Restart);
        Button(destroyed, "Main menu", -115, MainMenu);
    }

    private void Start()
    {
        bool automatic = startOnLoad;
        startOnLoad = false;
        if (automatic) StartGame(); else ShowMain();
    }

    private GameObject Screen(string name)
    {
        var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasObject.transform, false);
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.025f, 0.035f, 0.025f, 0.94f);
        panel.SetActive(false);
        return panel;
    }

    private TextMeshProUGUI Label(GameObject parent, string text, float y, float size)
    {
        var go = new GameObject(text, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent.transform, false);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(900, 140);
        rect.anchoredPosition = new Vector2(0, y);
        var label = go.GetComponent<TextMeshProUGUI>();
        label.font = font; label.text = text; label.fontSize = size;
        label.alignment = TextAlignmentOptions.Center; label.color = Color.white;
        label.raycastTarget = false;
        return label;
    }

    private void Button(GameObject parent, string text, float y, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject(text + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent.transform, false);
        var rect = go.GetComponent<RectTransform>(); rect.sizeDelta = new Vector2(320, 64);
        rect.anchoredPosition = new Vector2(0, y);
        go.GetComponent<Image>().color = new Color(0.18f, 0.24f, 0.12f);
        go.GetComponent<Button>().onClick.AddListener(action);
        var label = Label(go, text, 0, 28);
        label.rectTransform.sizeDelta = new Vector2(320, 64);
    }

    private void SetGameplay(bool playing)
    {
        IsPlaying = playing;
        var tank = Object.FindFirstObjectByType<TankHealth>();
        tank.GetComponent<CubeMovement>().enabled = playing;
        tank.GetComponent<TankFire>().enabled = playing;
        tank.GetComponent<WeaponHud>().enabled = playing;
        tank.GetComponent<AimCrosshair>().enabled = playing;
        var weaponCanvas = GameObject.Find("Weapon HUD");
        if (weaponCanvas != null) weaponCanvas.GetComponent<Canvas>().enabled = playing;
        Cursor.visible = !playing;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideScreens() { main.SetActive(false); credits.SetActive(false); destroyed.SetActive(false); }
    public void StartGame() { HideScreens(); SetGameplay(true); Time.timeScale = 1; }
    public void ShowMain() { HideScreens(); SetGameplay(false); main.SetActive(true); }
    public void ShowCredits() { HideScreens(); credits.SetActive(true); }
    public void Restart() { startOnLoad = true; Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void MainMenu() { startOnLoad = false; Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public static void ShowDestroyed()
    {
        if (instance == null) return;
        instance.HideScreens(); instance.SetGameplay(false);
        instance.finalScore.text = "Score " + Object.FindFirstObjectByType<GameHud>().Score;
        instance.destroyed.SetActive(true);
        Time.timeScale = 0;
    }
    private void OnDestroy() { if (instance == this) instance = null; if (canvasObject != null) Destroy(canvasObject); }
}
