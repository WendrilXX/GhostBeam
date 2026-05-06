using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GhostBeam.UI
{
    public class GameOverPanelController : MonoBehaviour
    {
        [SerializeField] private Canvas gameOverCanvas;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI txtScoreFinal;
        [SerializeField] private TextMeshProUGUI txtHighScoreFinal;
        [SerializeField] private TextMeshProUGUI txtCoinsFinal;
        [SerializeField] private Button btnRestart;
        [SerializeField] private Button btnMenu;

        private void Awake()
        {
            EnsureUIReferences();
            SetGameOverVisible(false);
        }

        private void Start()
        {
            Managers.GameManager.onGameOver += ShowGameOver;
            
            if (btnRestart != null)
            {
                btnRestart.onClick.RemoveAllListeners();
                btnRestart.onClick.AddListener(() => Managers.GameManager.RestartScene());
            }

            if (btnMenu != null)
            {
                btnMenu.onClick.RemoveAllListeners();
                btnMenu.onClick.AddListener(() => Managers.GameManager.ReturnToMainMenu());
            }
        }

        private void ShowGameOver()
        {
            SetGameOverVisible(true);

            var scoreManager = Managers.ScoreManager.Instance;
            if (scoreManager != null)
            {
                if (txtScoreFinal != null)
                    txtScoreFinal.text = $"Score Final: {scoreManager.CurrentScore}";
                if (txtHighScoreFinal != null)
                    txtHighScoreFinal.text = $"Best: {scoreManager.HighScore}";
                if (txtCoinsFinal != null)
                    txtCoinsFinal.text = $"Moedas: {scoreManager.Coins}";
            }
        }

        private void EnsureUIReferences()
        {
            if (gameOverCanvas == null)
                gameOverCanvas = GetComponent<Canvas>();

            if (gameOverCanvas == null)
                gameOverCanvas = GetComponentInParent<Canvas>();

            if (gameOverCanvas == null)
            {
                var canvasObj = new GameObject("CanvasGameOver");
                gameOverCanvas = canvasObj.AddComponent<Canvas>();
                gameOverCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<GraphicRaycaster>();

                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }

            EventSystemUtility.EnsureEventSystem();

            if (gameOverPanel == null)
            {
                Transform existingPanel = gameOverCanvas.transform.Find("GameOverPanel");
                if (existingPanel != null)
                {
                    gameOverPanel = existingPanel.gameObject;
                }
                else
                {
                    gameOverPanel = CreateDefaultPanel(gameOverCanvas.transform);
                }
            }

            if (txtScoreFinal == null)
                txtScoreFinal = FindTextInPanel("TxtScoreFinal");
            if (txtHighScoreFinal == null)
                txtHighScoreFinal = FindTextInPanel("TxtHighScoreFinal");
            if (txtCoinsFinal == null)
                txtCoinsFinal = FindTextInPanel("TxtCoinsFinal");
            if (btnRestart == null)
                btnRestart = FindButtonInPanel("BtnRestart");
            if (btnMenu == null)
                btnMenu = FindButtonInPanel("BtnMenu");
        }

        private void SetGameOverVisible(bool visible)
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(visible);
        }

        private TextMeshProUGUI FindTextInPanel(string name)
        {
            if (gameOverPanel == null)
                return null;

            Transform child = gameOverPanel.transform.Find(name);
            return child != null ? child.GetComponent<TextMeshProUGUI>() : null;
        }

        private Button FindButtonInPanel(string name)
        {
            if (gameOverPanel == null)
                return null;

            Transform child = gameOverPanel.transform.Find(name);
            return child != null ? child.GetComponent<Button>() : null;
        }

        private GameObject CreateDefaultPanel(Transform parent)
        {
            var panelObj = new GameObject("GameOverPanel");
            panelObj.transform.SetParent(parent, false);

            var panelRect = panelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.85f);

            CreateLabel(panelObj.transform, "TxtGameOverTitle", "GAME OVER", new Vector2(0f, 220f), 96);
            CreateLabel(panelObj.transform, "TxtScoreFinal", "Score Final: 0", new Vector2(0f, 80f), 52);
            CreateLabel(panelObj.transform, "TxtHighScoreFinal", "Best: 0", new Vector2(0f, 20f), 42);
            CreateLabel(panelObj.transform, "TxtCoinsFinal", "Moedas: 0", new Vector2(0f, -35f), 42);

            CreateButton(panelObj.transform, "BtnRestart", "Reiniciar", new Vector2(0f, -130f));
            CreateButton(panelObj.transform, "BtnMenu", "Voltar ao Menu", new Vector2(0f, -220f));

            return panelObj;
        }

        private void CreateLabel(Transform parent, string name, string content, Vector2 anchoredPosition, float fontSize)
        {
            var labelObj = new GameObject(name);
            labelObj.transform.SetParent(parent, false);

            var rect = labelObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(900f, 90f);
            rect.anchoredPosition = anchoredPosition;

            var text = labelObj.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
        }

        private void CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            var buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            var rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(420f, 70f);
            rect.anchoredPosition = anchoredPosition;

            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.18f, 0.18f, 0.2f, 0.95f);

            buttonObj.AddComponent<Button>();

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = label;
            buttonText.fontSize = 34;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
        }

        private void OnDestroy()
        {
            if (Managers.GameManager.Instance != null)
                Managers.GameManager.onGameOver -= ShowGameOver;
        }
    }
}
