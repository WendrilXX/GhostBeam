using UnityEngine;
using TMPro;

public class GameOverPanelController : MonoBehaviour
{
    [Tooltip("Root do painel de game over que sera mostrado/ocultado.")]
    public GameObject panelRoot;
    public TMP_Text finalScoreText;
    public TMP_Text highscoreText;
    public TMP_Text survivalTimeText;
    public TMP_Text scoreComparisonText;

    private ScoreManager scoreManager;
    private int lastGameOverScore = 0;
    private int lastGameOverSurvivalTime = 0;

    private const string PanelObjectName = "PanelGameOver";
    private const string FinalScoreObjectName = "TxtScoreFinal";
    private const string HighscoreObjectName = "TxtRecordeFinal";

    private void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        AutoAssignIfMissing();

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameManager.onGameOver += HandleGameOver;
        GameManager.onMainMenuChanged += HandleMainMenuChanged;
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= HandleGameOver;
        GameManager.onMainMenuChanged -= HandleMainMenuChanged;
    }

    public void RestartGame()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.ReturnToMainMenu();
    }

    private void HandleGameOver()
    {
        SetPanelVisible(true);

        if (scoreManager != null && finalScoreText != null)
        {
            finalScoreText.text = $"<color=#FFD700>SCORE FINAL: {scoreManager.Score}</color>";
            finalScoreText.fontSize = 36;
        }

        if (scoreManager != null && highscoreText != null)
        {
            highscoreText.text = $"<color=#FFD700>RECORDE: {scoreManager.Highscore}</color>";
            highscoreText.fontSize = 32;
        }

        if (scoreManager != null && survivalTimeText != null)
        {
            int survivalSeconds = scoreManager.SurvivalSeconds;
            int minutes = survivalSeconds / 60;
            int seconds = survivalSeconds % 60;
            survivalTimeText.text = $"<color=#00D9FF>SOBREVIVENCIA: {minutes:00}:{seconds:00}</color>";
            survivalTimeText.fontSize = 28;
            lastGameOverSurvivalTime = survivalSeconds;
        }

        if (scoreManager != null && scoreComparisonText != null)
        {
            int scoreDiff = scoreManager.Score - lastGameOverScore;
            if (scoreDiff > 0)
            {
                scoreComparisonText.text = $"<color=#00FF00>+{scoreDiff} vs anterior!</color>";
                scoreComparisonText.fontSize = 24;
            }
            else if (scoreDiff < 0)
            {
                scoreComparisonText.text = $"<color=#FF6666>{scoreDiff} vs anterior</color>";
                scoreComparisonText.fontSize = 24;
            }
            else
            {
                scoreComparisonText.text = "<color=#CCCCCC>Mesmo score</color>";
                scoreComparisonText.fontSize = 24;
            }
            lastGameOverScore = scoreManager.Score;
        }
    }

    private void SetPanelVisible(bool visible)
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(visible);
        }
    }

    private void HandleMainMenuChanged(bool inMainMenu)
    {
        if (inMainMenu)
        {
            SetPanelVisible(false);
        }
    }

    private void AutoAssignIfMissing()
    {
        if (panelRoot == null)
        {
            Transform panelTransform = FindChildByName(transform, PanelObjectName);
            if (panelTransform != null)
            {
                panelRoot = panelTransform.gameObject;
            }
        }

        if (finalScoreText == null)
        {
            finalScoreText = FindTMPByName(FinalScoreObjectName);
        }

        if (highscoreText == null)
        {
            highscoreText = FindTMPByName(HighscoreObjectName);
        }

        // Auto-assign survival time e comparison se não existirem
        if (survivalTimeText == null)
        {
            survivalTimeText = FindTMPByName("TxtTempoSobrevivencia");
            if (survivalTimeText == null)
            {
                survivalTimeText = FindTMPByName("TxtTempo");
            }
        }

        if (scoreComparisonText == null)
        {
            scoreComparisonText = FindTMPByName("TxtComparacao");
            if (scoreComparisonText == null)
            {
                scoreComparisonText = FindTMPByName("TxtComparacaoScore");
            }
        }
    }

    private TMP_Text FindTMPByName(string objectName)
    {
        Transform target = FindChildByName(transform, objectName);
        if (target == null)
        {
            return null;
        }

        TMP_Text tmp = target.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            return tmp;
        }

        return target.GetComponentInChildren<TMP_Text>(true);
    }

    private Transform FindChildByName(Transform root, string targetName)
    {
        if (root == null || string.IsNullOrEmpty(targetName))
        {
            return null;
        }

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == targetName)
            {
                return child;
            }

            Transform nested = FindChildByName(child, targetName);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }
}
