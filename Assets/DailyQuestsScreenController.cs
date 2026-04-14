using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// DailyQuestsScreenController - Tela de Desafios Diários integrada no Menu Principal
/// </summary>
public class DailyQuestsScreenController : MonoBehaviour
{
    [System.Serializable]
    public class DailyQuest
    {
        public string title;
        public string description;
        public int reward;
        public bool completed;
        public float progress; // 0-1
    }

    public GameObject questsPanelRoot;
    public Transform questsContent;
    public TextMeshProUGUI resetTimeText;
    public Button closeButton;

    private DailyQuest[] dailyQuests = new DailyQuest[3];
    private const string QuestsPanelObjectName = "PanelDailyQuests";

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
        gameObject.SetActive(visible);
        
        if (questsPanelRoot != null)
        {
            questsPanelRoot.SetActive(visible);

            if (visible)
            {
                InitializeQuests();
                PopulateQuests();

                if (closeButton != null)
                {
                    closeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.AddListener(() =>
                    {
                        MainMenuController menu = FindAnyObjectByType<MainMenuController>();
                        if (menu != null) menu.CloseDailyQuests();
                    });
                }
            }
        }
    }

    private void InitializeQuests()
    {
        // Mock quests - em produção seria do servidor
        dailyQuests[0] = new DailyQuest
        {
            title = "Mate 10 Inimigos",
            description = "Mate 10 inimigos em uma sessão",
            reward = 50,
            completed = false,
            progress = 0.3f
        };

        dailyQuests[1] = new DailyQuest
        {
            title = "Atinja Stage 2",
            description = "Chegue ao Stage 2 (35+ segundos)",
            reward = 75,
            completed = false,
            progress = 0f
        };

        dailyQuests[2] = new DailyQuest
        {
            title = "Combo de 3x",
            description = "Faça um combo de 3 kills",
            reward = 100,
            completed = false,
            progress = 0f
        };

        // Atualizar tempo de reset
        System.DateTime nextReset = System.DateTime.Now.AddDays(1).Date;
        System.TimeSpan timeUntilReset = nextReset - System.DateTime.Now;
        if (resetTimeText != null)
        {
            resetTimeText.text = $"RESET: {timeUntilReset.Hours:D2}:{timeUntilReset.Minutes:D2}";
        }
    }

    private void PopulateQuests()
    {
        if (questsContent != null)
        {
            foreach (Transform child in questsContent)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var quest in dailyQuests)
        {
            if (questsContent != null)
            {
                GameObject itemGO = new GameObject("QuestItem");
                itemGO.transform.SetParent(questsContent, false);

                // Texto com título e recompensa
                TextMeshProUGUI questTM = itemGO.AddComponent<TextMeshProUGUI>();
                questTM.text = $"{quest.title} (+{quest.reward})";
                questTM.fontSize = 28;
                questTM.color = new Color(1, 0.8f, 0.2f, 1);

                // Progress bar
                Image progressBar = itemGO.AddComponent<Image>();
                progressBar.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);
                progressBar.fillAmount = quest.progress;

                LayoutElement layout = itemGO.AddComponent<LayoutElement>();
                layout.preferredHeight = 60;
            }
        }
    }

    private void HandleMainMenuChanged(bool isVisible)
    {
        if (!isVisible && questsPanelRoot != null)
        {
            questsPanelRoot.SetActive(false);
        }
    }

    private void AutoAssignIfMissing()
    {
        if (questsPanelRoot == null)
        {
            Transform found = transform.Find(QuestsPanelObjectName);
            if (found != null)
                questsPanelRoot = found.gameObject;
        }
    }
}
