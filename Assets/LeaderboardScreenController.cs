using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// LeaderboardScreenController - Tela de Ranking integrada no Menu Principal
/// </summary>
public class LeaderboardScreenController : MonoBehaviour
{
    public GameObject leaderboardPanelRoot;
    public Transform leaderboardContent;
    public TextMeshProUGUI playerRankText;
    public Button closeButton;

    private const string LeaderboardPanelObjectName = "PanelLeaderboard";

    private void Awake()
    {
        AutoAssignIfMissing();
    }

    private void OnEnable()
    {
        GameManager.onMainMenuChanged += HandleMainMenuChanged;
    }

    private void OnDisable()
    {
        GameManager.onMainMenuChanged -= HandleMainMenuChanged;
    }

    public void SetVisible(bool visible)
    {
        if (leaderboardPanelRoot != null)
        {
            leaderboardPanelRoot.SetActive(visible);

            if (visible)
            {
                PopulateLeaderboard();

                if (closeButton != null)
                {
                    closeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.AddListener(() =>
                    {
                        MainMenuController menu = FindObjectOfType<MainMenuController>();
                        if (menu != null) menu.CloseLeaderboard();
                    });
                }
            }
        }
    }

    private void PopulateLeaderboard()
    {
        // Limpar lista anterior
        if (leaderboardContent != null)
        {
            foreach (Transform child in leaderboardContent)
            {
                Destroy(child.gameObject);
            }
        }

        // Mock data - em produção seria do servidor
        int[] mockScores = { 5420, 4890, 4230, 3950, 3720, 3450, 3210, 2980, 2750, 2520 };
        string[] mockNames = { "Ninja", "Shadow", "Ghost", "Phoenix", "Reaper", "Blaze", "Storm", "Nova", "Echo", "Void" };

        int playerHighscore = PlayerPrefs.GetInt("Highscore", 0);
        int playerRank = 11;

        for (int i = 0; i < mockScores.Length; i++)
        {
            if (mockScores[i] <= playerHighscore && i < playerRank)
            {
                playerRank = i + 1;
            }
        }

        // Criar items do ranking
        for (int i = 0; i < mockScores.Length; i++)
        {
            if (leaderboardContent != null)
            {
                GameObject itemGO = new GameObject($"RankItem_{i + 1}");
                itemGO.transform.SetParent(leaderboardContent, false);

                TextMeshProUGUI rankTM = itemGO.AddComponent<TextMeshProUGUI>();
                rankTM.text = $"#{i + 1} {mockNames[i].PadRight(12)} {mockScores[i]}";
                rankTM.fontSize = 24;

                LayoutElement layout = itemGO.AddComponent<LayoutElement>();
                layout.preferredHeight = 50;
            }
        }

        if (playerRankText != null)
        {
            playerRankText.text = $"SEU RANK: #{playerRank}";
        }
    }

    private void HandleMainMenuChanged(bool isVisible)
    {
        if (!isVisible && leaderboardPanelRoot != null)
        {
            leaderboardPanelRoot.SetActive(false);
        }
    }

    private void AutoAssignIfMissing()
    {
        if (leaderboardPanelRoot == null)
        {
            Transform found = transform.Find(LeaderboardPanelObjectName);
            if (found != null)
                leaderboardPanelRoot = found.gameObject;
        }
    }
}
