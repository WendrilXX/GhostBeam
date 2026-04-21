using UnityEngine;
using System;

namespace GhostBeam.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        private int currentScore = 0;
        private int highScore = 0;
        private int coins = 0;
        private float survivalSeconds = 0f;

        public int CurrentScore => currentScore;
        public int HighScore => highScore;
        public int Coins => coins;
        public float SurvivalSeconds => survivalSeconds;

        public static event Action<int> onScoreChanged;
        public static event Action<int> onHighScoreChanged;
        public static event Action<int> onCoinsChanged;
        public static event Action<float> onSurvivalTimeChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            LoadData();
        }

        private void Start()
        {
            GameManager.onGameOver += OnGameOver;
        }

        private void Update()
        {
            if (Time.timeScale > 0)
            {
                survivalSeconds += Time.deltaTime;
                onSurvivalTimeChanged?.Invoke(survivalSeconds);
                
                // Score por tempo: +5 pontos por segundo
                int newScore = currentScore + (int)(Time.deltaTime * 5);
                if (newScore != currentScore)
                {
                    currentScore = newScore;
                    onScoreChanged?.Invoke(currentScore);
                }
            }
        }

        public void AddScore(int amount)
        {
            currentScore += amount;
            onScoreChanged?.Invoke(currentScore);
        }

        public void AddCoins(int amount)
        {
            coins += amount;
            onCoinsChanged?.Invoke(coins);
            SaveData();
        }

        public bool TrySpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                onCoinsChanged?.Invoke(coins);
                SaveData();
                return true;
            }
            return false;
        }

        private void OnGameOver()
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
                onHighScoreChanged?.Invoke(highScore);
                SaveData();
            }
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt("Highscore", highScore);
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.Save();
        }

        private void LoadData()
        {
            highScore = PlayerPrefs.GetInt("Highscore", 0);
            coins = PlayerPrefs.GetInt("Coins", 0);
        }

        public void ResetForNewGame()
        {
            currentScore = 0;
            survivalSeconds = 0f;
            onScoreChanged?.Invoke(currentScore);
            onSurvivalTimeChanged?.Invoke(survivalSeconds);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.onGameOver -= OnGameOver;
            }
        }
    }
}
