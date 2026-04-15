using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// HUD em tempo real com vida, score, tempo de jogo e dificuldade.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("Health UI")]
    public Image healthBar;
    public TMP_Text healthText;

    [Header("Score UI")]
    public TMP_Text scoreText;
    public TMP_Text highscoreText;

    [Header("Time UI")]
    public TMP_Text survivalTimeText;

    [Header("Difficulty UI")]
    public TMP_Text waveText;

    [Header("Items UI")]
    public TMP_Text coinsText;
    public TMP_Text batteryText;

    [Header("Buttons")]
    public Button pauseButton;

    private float survivalTime = 0f;
    private HealthSystem healthSystem;
    private ScoreManager scoreManager;
    private BatterySystem batterySystem;

    private void OnEnable()
    {
        if (healthSystem != null)
            HealthSystem.onHealthChanged += UpdateHealth;
        if (scoreManager != null)
            ScoreManager.onScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        if (healthSystem != null)
            HealthSystem.onHealthChanged -= UpdateHealth;
        if (scoreManager != null)
            ScoreManager.onScoreChanged -= UpdateScore;
    }

    private void Start()
    {
        healthSystem = FindAnyObjectByType<HealthSystem>();
        scoreManager = FindAnyObjectByType<ScoreManager>();
        batterySystem = FindAnyObjectByType<BatterySystem>();

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        // Inicializar valores
        if (healthSystem != null)
            UpdateHealth((int)healthSystem.CurrentHealth, healthSystem.maxHealth);
        if (scoreManager != null)
        {
            UpdateScore(scoreManager.Score);
            UpdateHighscore(scoreManager.Highscore);
            UpdateCoins(scoreManager.Coins);
        }
    }

    private void Update()
    {
        survivalTime += Time.deltaTime;
        UpdateSurvivalTime(survivalTime);
        UpdateWave();

        // Atualizar bateria em tempo real
        if (batterySystem != null && batteryText != null)
        {
            batteryText.text = $"Bateria: {batterySystem.CurrentBattery:F0}%";
        }
    }

    private void UpdateHealth(int current, int max)
    {
        if (healthText != null)
            healthText.text = $"Vida: {current}/{max}";

        if (healthBar != null)
        {
            healthBar.fillAmount = (float)current / max;
            // Mudar cor conforme vida diminui
            if (current / max > 0.5f)
                healthBar.color = Color.green;
            else if (current / max > 0.25f)
                healthBar.color = Color.yellow;
            else
                healthBar.color = Color.red;
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    private void UpdateHighscore(int highscore)
    {
        if (highscoreText != null)
            highscoreText.text = $"Recorde: {highscore}";
    }

    private void UpdateSurvivalTime(float seconds)
    {
        int minutes = (int)(seconds / 60f);
        int secs = (int)(seconds % 60f);

        if (survivalTimeText != null)
            survivalTimeText.text = $"Tempo: {minutes:D2}:{secs:D2}";
    }

    private void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = $"Moedas: {coins}";
    }

    private void UpdateWave()
    {
        if (waveText != null)
        {
            SpawnManager spawn = FindAnyObjectByType<SpawnManager>();
            if (spawn != null)
            {
                waveText.text = $"Wave: {spawn.CurrentDifficultyStage + 1}";
            }
        }
    }

    private void PauseGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
    }
}
