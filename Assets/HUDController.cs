using UnityEngine;
using TMPro;
using System.Collections;

public class HUDController : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text scoreText;
    public TMP_Text highscoreText;
    public TMP_Text batteryText;
    public TMP_Text coinsText;
    public TMP_Text survivalTimeText;
    public TMP_Text stageText;
    public TMP_Text performanceText;

    private HealthSystem healthSystem;
    private ScoreManager scoreManager;
    private BatterySystem batterySystem;

    private const string HealthObjectName = "TxtVida";
    private const string ScoreObjectName = "TxtScore";
    private const string HighscoreObjectName = "TxtRecorde";
    private const string BatteryObjectName = "TxtBateria";
    private const string CoinsObjectName = "TxtMoedas";
    private const string SurvivalTimeObjectName = "TxtTempo";
    private const string StageObjectName = "TxtFase";
    private const string PerformanceObjectName = "TxtPerf";

    private void OnEnable()
    {
        HealthSystem.onHealthChanged += UpdateHealth;
        ScoreManager.onScoreChanged += UpdateScore;
        ScoreManager.onHighscoreChanged += UpdateHighscore;
        ScoreManager.onCoinsChanged += UpdateCoins;
        ScoreManager.onSurvivalTimeChanged += UpdateSurvivalTime;
        BatterySystem.onBatteryChanged += UpdateBattery;
        SpawnManager.onDifficultyStageChanged += UpdateDifficultyStage;
        CoinPickup.onCoinCollected += HandleCoinCollected;
        BatteryPickup.onBatteryCollected += HandleBatteryCollected;

        if (SettingsManager.Instance != null)
        {
            SettingsManager.onSettingsChanged += ApplyTimerVisibility;
        }
    }

    private void OnDisable()
    {
        HealthSystem.onHealthChanged -= UpdateHealth;
        ScoreManager.onScoreChanged -= UpdateScore;
        ScoreManager.onHighscoreChanged -= UpdateHighscore;
        ScoreManager.onCoinsChanged -= UpdateCoins;
        ScoreManager.onSurvivalTimeChanged -= UpdateSurvivalTime;
        BatterySystem.onBatteryChanged -= UpdateBattery;
        SpawnManager.onDifficultyStageChanged -= UpdateDifficultyStage;
        CoinPickup.onCoinCollected -= HandleCoinCollected;
        BatteryPickup.onBatteryCollected -= HandleBatteryCollected;

        if (SettingsManager.Instance != null)
        {
            SettingsManager.onSettingsChanged -= ApplyTimerVisibility;
        }
    }

    private void Start()
    {
        AutoAssignIfMissing();

        if (healthText != null) healthText.text = "Vida: -";
        if (scoreText != null) scoreText.text = "Score: 0";
        if (highscoreText != null) highscoreText.text = "Recorde: 0";
        if (batteryText != null) batteryText.text = "Bateria: -";
        if (coinsText != null) coinsText.text = "Moedas: 0";
        if (survivalTimeText != null) survivalTimeText.text = "Tempo: 00:00";
        if (stageText != null) stageText.gameObject.SetActive(false);


        healthSystem = FindAnyObjectByType<HealthSystem>();
        scoreManager = FindAnyObjectByType<ScoreManager>();
        batterySystem = FindAnyObjectByType<BatterySystem>();

        if (healthSystem != null)
        {
            UpdateHealth(healthSystem.CurrentHealth, healthSystem.maxHealth);
        }

        if (scoreManager != null)
        {
            UpdateScore(scoreManager.Score);
            UpdateHighscore(scoreManager.Highscore);
            UpdateCoins(scoreManager.Coins);
            UpdateSurvivalTime(scoreManager.SurvivalSeconds);
        }

        if (batterySystem != null)
        {
            UpdateBattery(batterySystem.CurrentBattery, batterySystem.maxBattery, batterySystem.IsDisabled);
        }

        ApplyTimerVisibility();
    }

    private void UpdateHealth(int current, int max)
    {
        if (healthText == null) return;
        healthText.text = $"<color=#FF4444>VIDA: {current}/{max}</color>";
        healthText.fontSize = 32;
    }

    private void UpdateScore(int score)
    {
        if (scoreText == null) return;
        scoreText.text = $"<color=#FFD700>SCORE: {score}</color>";
        scoreText.fontSize = 30;
    }

    private void UpdateHighscore(int highscore)
    {
        if (highscoreText == null) return;
        highscoreText.text = $"<color=#FFD700>RECORDE: {highscore}</color>";
        highscoreText.fontSize = 28;
    }

    private void UpdateBattery(float current, float max, bool isDisabled)
    {
        if (batteryText == null) return;
        if (max <= 0f)
        {
            batteryText.text = "<color=#00D9FF>BATERIA: -</color>";
            return;
        }

        int batteryPercent = Mathf.RoundToInt((current / max) * 100f);
        string batteryColor = isDisabled ? "#FF3333" : "#00D9FF";
        batteryText.text = $"<color={batteryColor}>BATERIA: {batteryPercent}%</color>";
        if (isDisabled) batteryText.text += " <color=#FF3333>(APAGADA)</color>";
        batteryText.fontSize = 32;
    }

    private void UpdateCoins(int coins)
    {
        if (coinsText == null) return;
        coinsText.text = "Moedas: " + coins;
    }

    private void UpdateSurvivalTime(int seconds)
    {
        if (survivalTimeText == null) return;
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        survivalTimeText.text = "Tempo: " + minutes.ToString("00") + ":" + remainingSeconds.ToString("00");
    }

    private void UpdateDifficultyStage(DifficultyStage stage)
    {
        if (stageText == null)
        {
            return;
        }

        switch (stage)
        {
            case DifficultyStage.Presentation:
                stageText.text = "Fase: Apresentacao";
                break;
            case DifficultyStage.Test:
                stageText.text = "Fase: Teste";
                break;
            default:
                stageText.text = "Fase: Climax";
                break;
        }
    }

    private void ApplyTimerVisibility()
    {
        if (survivalTimeText == null)
        {
            return;
        }

        bool visible = SettingsManager.Instance == null || SettingsManager.Instance.ShowHudTimer;
        survivalTimeText.gameObject.SetActive(visible);
    }

    private void HandleCoinCollected(int value, Vector3 worldPosition)
    {
        // Mostra quantidade verdadeira de moedas (1-20)
        PickupFloatingText.ShowCoinText(worldPosition, value);
    }

    private void HandleBatteryCollected(float value, Vector3 worldPosition)
    {
        // Bateria dropada é coletada automaticamente, sem feedback visual
    }

    private void ShowFloatingPickupFeedback(string message, Color color, Vector3 worldPosition)
    {
        Camera cam = Camera.main;
        RectTransform hudRect = transform as RectTransform;
        if (cam == null || hudRect == null)
        {
            return;
        }

        Vector3 screen = cam.WorldToScreenPoint(worldPosition + new Vector3(0f, 0.35f, 0f));
        if (screen.z < 0f)
        {
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(hudRect, screen, null, out Vector2 localPoint))
        {
            return;
        }

        GameObject feedbackObj = new GameObject("TxtPickupFeedback", typeof(RectTransform), typeof(TextMeshProUGUI));
        feedbackObj.transform.SetParent(transform, false);

        RectTransform rect = feedbackObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(340f, 56f);
        rect.anchoredPosition = localPoint;

        TMP_Text text = feedbackObj.GetComponent<TMP_Text>();
        text.text = message;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        text.enableAutoSizing = true;
        text.fontSizeMin = 16;
        text.fontSizeMax = 36;
        text.color = color;
        if (coinsText != null)
        {
            text.font = coinsText.font;
        }

        StartCoroutine(AnimateAndDisposePickupFeedback(rect, text));
    }

    private IEnumerator AnimateAndDisposePickupFeedback(RectTransform rect, TMP_Text text)
    {
        const float duration = 0.65f;
        float t = 0f;
        Vector2 start = rect.anchoredPosition;
        Vector2 end = start + new Vector2(0f, 62f);

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            rect.anchoredPosition = Vector2.Lerp(start, end, p);

            Color c = text.color;
            c.a = 1f - p;
            text.color = c;

            yield return null;
        }

        if (rect != null)
        {
            Destroy(rect.gameObject);
        }
    }

    private void AutoAssignIfMissing()
    {
        if (healthText == null)
        {
            healthText = FindTMPByName(HealthObjectName);
        }

        if (scoreText == null)
        {
            scoreText = FindTMPByName(ScoreObjectName);
        }

        if (highscoreText == null)
        {
            highscoreText = FindTMPByName(HighscoreObjectName);
        }

        if (batteryText == null)
        {
            batteryText = FindTMPByName(BatteryObjectName);
        }

        if (coinsText == null)
        {
            coinsText = FindTMPByName(CoinsObjectName);
        }

        if (survivalTimeText == null)
        {
            survivalTimeText = FindTMPByName(SurvivalTimeObjectName);
        }

        if (stageText == null)
        {
            stageText = FindTMPByName(StageObjectName);
        }

        if (performanceText == null)
        {
            performanceText = FindTMPByName(PerformanceObjectName);
        }
    }

    private TMP_Text FindTMPByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return null;
        }

        Transform target = transform.Find(objectName);
        if (target == null)
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] != null && texts[i].name == objectName)
                {
                    return texts[i];
                }
            }

            return null;
        }

        return target.GetComponent<TMP_Text>();
    }
}