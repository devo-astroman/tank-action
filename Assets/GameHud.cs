using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public sealed class GameHud : MonoBehaviour
{
    private static GameHud instance;
    public int Score { get; private set; }
    public float ElapsedTime { get; private set; }
    private TankHealth tank;
    private EnemyWaves waves;
    private GameObject canvasObject;
    private TMP_FontAsset font;
    private TextMeshProUGUI waveText, scoreText, healthText;
    private Image healthFill;
    public static void AddScore(int amount)
    {
        if (instance != null && amount > 0) instance.Score += amount;
    }

    private void Awake()
    {
        instance = this;
        tank = GetComponent<TankHealth>();
        waves = Object.FindFirstObjectByType<EnemyWaves>();
        font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        canvasObject = new GameObject("Game HUD", typeof(Canvas), typeof(CanvasScaler));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        waveText = Panel("Wave", new Vector2(0, 1), new Vector2(20, -20), new Vector2(210, 52));
        scoreText = Panel("Score and Time", Vector2.one, new Vector2(-20, -20), new Vector2(230, 82));
        healthText = Panel("Health", Vector2.zero, new Vector2(20, 20), new Vector2(250, 80));
        healthText.alignment = TextAlignmentOptions.Top;
        healthText.margin = new Vector4(8, 6, 8, 0);
        var bar = new GameObject("Health Fill", typeof(RectTransform), typeof(Image));
        bar.transform.SetParent(healthText.transform.parent, false);
        var rect = bar.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = Vector2.zero;
        rect.offsetMin = new Vector2(12, 12);
        rect.offsetMax = new Vector2(-12, 36);
        healthFill = bar.GetComponent<Image>();
        healthFill.raycastTarget = false;
    }

    private TextMeshProUGUI Panel(string name, Vector2 anchor, Vector2 offset, Vector2 size)
    {
        var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasObject.transform, false);
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = anchor;
        rect.anchoredPosition = offset;
        rect.sizeDelta = size;
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        panel.GetComponent<Image>().raycastTarget = false;
        var label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        label.transform.SetParent(panel.transform, false);
        var text = label.GetComponent<TextMeshProUGUI>();
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = text.rectTransform.offsetMax = Vector2.zero;
        text.font = font;
        text.fontSize = 20.8f;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        return text;
    }

    private void LateUpdate()
    {
        if (tank.IsAlive) ElapsedTime += Time.deltaTime;
        int seconds = Mathf.FloorToInt(ElapsedTime);
        string time = $"{seconds / 60:00}:{seconds % 60:00}";
        waveText.text = "Wave " + (waves != null ? waves.WaveNumber : 0);
        scoreText.text = "Time " + time + "\nScore " + Score;
        healthText.text = "Health " + tank.CurrentHealth + " / " + tank.MaxHealth;
        float fraction = (float)tank.CurrentHealth / tank.MaxHealth;
        healthFill.rectTransform.localScale = new Vector3(fraction, 1, 1);
        healthFill.color = Color.Lerp(Color.red, Color.green, Mathf.Clamp01((fraction - 0.15f) / 0.55f));
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
        if (canvasObject != null) Destroy(canvasObject);
    }
}
