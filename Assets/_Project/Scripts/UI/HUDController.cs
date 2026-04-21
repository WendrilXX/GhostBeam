using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GhostBeam.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtHealth;
        [SerializeField] private TextMeshProUGUI txtBattery;
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtCoins;
        [SerializeField] private TextMeshProUGUI txtHighScore;
        [SerializeField] private TextMeshProUGUI txtTime;
        [SerializeField] private Slider batterySlider;
        [SerializeField] private Button pauseButton;

        private Gameplay.HealthSystem healthSystem;
        private Gameplay.BatterySystem batterySystem;

        private void Awake()
        {
            // Auto-find if not assigned
            if (healthSystem == null)
                healthSystem = FindAnyObjectByType<Gameplay.HealthSystem>();
            if (batterySystem == null)
                batterySystem = FindAnyObjectByType<Gameplay.BatterySystem>();
        }

        private void Start()
        {
            // Subscribe to events
            Gameplay.HealthSystem.onHealthChanged += UpdateHealth;
            Gameplay.BatterySystem.onBatteryChanged += UpdateBattery;

            if (Managers.ScoreManager.Instance != null)
            {
                Managers.ScoreManager.onScoreChanged += UpdateScore;
                Managers.ScoreManager.onCoinsChanged += UpdateCoins;
                Managers.ScoreManager.onHighScoreChanged += UpdateHighScore;
                Managers.ScoreManager.onSurvivalTimeChanged += UpdateTime;
            }

            if (pauseButton != null)
                pauseButton.onClick.AddListener(() => Managers.GameManager.TogglePause());

            // Initial sync
            if (healthSystem != null)
                UpdateHealth(healthSystem.CurrentHealth);
            if (batterySystem != null)
                UpdateBattery(batterySystem.CurrentBattery);
            if (Managers.ScoreManager.Instance != null)
            {
                UpdateScore(Managers.ScoreManager.Instance.CurrentScore);
                UpdateCoins(Managers.ScoreManager.Instance.Coins);
                UpdateHighScore(Managers.ScoreManager.Instance.HighScore);
            }
        }

        private void UpdateHealth(int health)
        {
            if (txtHealth != null)
                txtHealth.text = $"Health: {health}";
        }

        private void UpdateBattery(float battery)
        {
            if (txtBattery != null)
            {
                float percent = Mathf.Clamp01(battery / batterySystem.MaxBattery);
                txtBattery.text = $"Battery: {percent:P0}";
            }

            if (batterySlider != null)
            {
                batterySlider.value = Mathf.Clamp01(battery / batterySystem.MaxBattery);
            }
        }

        private void UpdateScore(int score)
        {
            if (txtScore != null)
                txtScore.text = $"Score: {score}";
        }

        private void UpdateCoins(int coins)
        {
            if (txtCoins != null)
                txtCoins.text = $"Coins: {coins}";
        }

        private void UpdateHighScore(int highScore)
        {
            if (txtHighScore != null)
                txtHighScore.text = $"Best: {highScore}";
        }

        private void UpdateTime(float seconds)
        {
            if (txtTime != null)
            {
                int minutes = (int)(seconds / 60);
                int secs = (int)(seconds % 60);
                txtTime.text = $"{minutes:D2}:{secs:D2}";
            }
        }

        private void OnDestroy()
        {
            Gameplay.HealthSystem.onHealthChanged -= UpdateHealth;
            Gameplay.BatterySystem.onBatteryChanged -= UpdateBattery;
            
            if (Managers.ScoreManager.Instance != null)
            {
                Managers.ScoreManager.onScoreChanged -= UpdateScore;
                Managers.ScoreManager.onCoinsChanged -= UpdateCoins;
                Managers.ScoreManager.onHighScoreChanged -= UpdateHighScore;
                Managers.ScoreManager.onSurvivalTimeChanged -= UpdateTime;
            }
        }
    }
}
