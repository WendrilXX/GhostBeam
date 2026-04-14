using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const string HighscoreKey = "Highscore";
    private const string CoinsKey = "Coins";

    public static ScoreManager Instance { get; private set; }

    public static System.Action<int> onScoreChanged;
    public static System.Action<int> onHighscoreChanged;
    public static System.Action<int> onCoinsChanged;
    public static System.Action<int> onSurvivalTimeChanged;

    public int scorePerSecond = 5;
    public int scorePerKill = 15;
    public int coinsPerSecond = 0;
    public int coinsPerKill = 0;

    public int Score { get; private set; }
    public int Highscore { get; private set; }
    public int Coins { get; private set; }
    public int SurvivalSeconds { get; private set; }

    private float scoreTickTimer;
    private bool highscoreSaved;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        EnemyController.onEnemyKilled += AddKillScore;
        GameManager.onGameOver += SaveHighscoreIfNeeded;
    }

    private void OnDisable()
    {
        EnemyController.onEnemyKilled -= AddKillScore;
        GameManager.onGameOver -= SaveHighscoreIfNeeded;
    }

    private void Start()
    {
        Highscore = PlayerPrefs.GetInt(HighscoreKey, 0);
        Coins = PlayerPrefs.GetInt(CoinsKey, 0);
        onScoreChanged?.Invoke(Score);
        onHighscoreChanged?.Invoke(Highscore);
        onCoinsChanged?.Invoke(Coins);
        onSurvivalTimeChanged?.Invoke(SurvivalSeconds);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        scoreTickTimer += Time.deltaTime;
        if (scoreTickTimer >= 1f)
        {
            int ticks = Mathf.FloorToInt(scoreTickTimer);
            scoreTickTimer -= ticks;
            AddScore(ticks * scorePerSecond);
            if (coinsPerSecond > 0)
            {
                AddCoins(ticks * coinsPerSecond);
            }
            AddSurvivalSeconds(ticks);
        }
    }

    private void AddKillScore()
    {
        AddScore(scorePerKill);
        if (coinsPerKill > 0)
        {
            AddCoins(coinsPerKill);
        }
    }

    private void AddScore(int value)
    {
        Score += value;
        onScoreChanged?.Invoke(Score);
    }

    private void AddCoins(int value)
    {
        if (value <= 0)
        {
            return;
        }

        Coins += value;
        SaveCoins();
        onCoinsChanged?.Invoke(Coins);
    }

    public bool TrySpendCoins(int value)
    {
        if (value <= 0)
        {
            return true;
        }

        if (Coins < value)
        {
            return false;
        }

        Coins -= value;
        SaveCoins();
        onCoinsChanged?.Invoke(Coins);
        return true;
    }

    public void CollectCoins(int value)
    {
        AddCoins(value);
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsKey, Coins);
        PlayerPrefs.Save();
    }

    private void AddSurvivalSeconds(int value)
    {
        if (value <= 0)
        {
            return;
        }

        SurvivalSeconds += value;
        onSurvivalTimeChanged?.Invoke(SurvivalSeconds);
    }

    private void SaveHighscoreIfNeeded()
    {
        if (highscoreSaved)
        {
            return;
        }

        highscoreSaved = true;

        if (Score <= Highscore)
        {
            return;
        }

        Highscore = Score;
        PlayerPrefs.SetInt(HighscoreKey, Highscore);
        PlayerPrefs.Save();
        onHighscoreChanged?.Invoke(Highscore);
    }
}
