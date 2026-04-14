using UnityEngine;
using TMPro;

/// <summary>
/// Exibe o recorde no menu principal para motivar o jogador
/// </summary>
public class MenuHighscoreDisplay : MonoBehaviour
{
    private TMP_Text highscoreText;
    
    private const string HighscoreDisplayObjectName = "TxtRecordeMenu";

    private void Start()
    {
        AutoAssignIfMissing();
        RefreshHighscoreDisplay();
    }

    private void OnEnable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.onHighscoreChanged += RefreshHighscoreDisplay;
        }
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.onHighscoreChanged -= RefreshHighscoreDisplay;
        }
    }

    private void RefreshHighscoreDisplay(int newHighscore = 0)
    {
        if (highscoreText == null)
        {
            AutoAssignIfMissing();
        }

        if (highscoreText == null)
        {
            return;
        }

        int highscore = ScoreManager.Instance != null ? ScoreManager.Instance.Highscore : 0;
        highscoreText.text = $"<color=#FFD700>MELHOR SCORE: {highscore}</color>";
        highscoreText.fontSize = 32;
    }

    private void AutoAssignIfMissing()
    {
        if (highscoreText != null)
        {
            return;
        }

        Transform searchRoot = transform;
        while (searchRoot.parent != null)
        {
            searchRoot = searchRoot.parent;
        }

        Transform target = FindChildByName(searchRoot, HighscoreDisplayObjectName);
        if (target != null)
        {
            highscoreText = target.GetComponent<TextMeshProUGUI>();
        }

        if (highscoreText == null)
        {
            highscoreText = GetComponentInChildren<TextMeshProUGUI>(true);
        }
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
