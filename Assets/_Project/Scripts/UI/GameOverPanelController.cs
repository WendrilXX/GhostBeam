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
            CacheComponentReferences();
            ApplyVisualStyle();
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

            if (gameOverCanvas != null)
                gameOverCanvas.sortingOrder = 80;

            var scoreManager = Managers.ScoreManager.Instance;
            if (scoreManager != null)
            {
                if (txtScoreFinal != null)
                    txtScoreFinal.text = $"Pontuação: {scoreManager.CurrentScore}";
                if (txtHighScoreFinal != null)
                    txtHighScoreFinal.text = $"Recorde: {scoreManager.HighScore}";
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
                gameOverCanvas.sortingOrder = 80;
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
        }

        private void CacheComponentReferences()
        {
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

        private void ApplyVisualStyle()
        {
            if (gameOverPanel == null)
                return;

            var rootImg = gameOverPanel.GetComponent<Image>();
            if (rootImg != null)
            {
                rootImg.color = new Color(
                    MenuVisualTheme.PanelBackdrop.r,
                    MenuVisualTheme.PanelBackdrop.g,
                    MenuVisualTheme.PanelBackdrop.b,
                    0.94f);
            }

            foreach (var tmp in gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (tmp == null)
                    continue;
                tmp.color = tmp.name.Contains("Title") || tmp.name.Contains("GameOver")
                    ? MenuVisualTheme.TextPrimary
                    : MenuVisualTheme.TextMuted;
                tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
                tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
                if (tmp.name.Contains("Title") || tmp.name.Contains("TxtGameOver"))
                {
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.fontSize = Mathf.Max(tmp.fontSize, 52f);
                    tmp.outlineWidth = 0.2f;
                    tmp.outlineColor = MenuVisualTheme.TitleOutline;
                    tmp.color = MenuVisualTheme.TextPrimary;
                }
            }

            foreach (var button in gameOverPanel.GetComponentsInChildren<Button>(true))
                StyleGameOverButton(button);
        }

        private static void StyleGameOverButton(Button button)
        {
            if (button == null)
                return;

            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = MenuVisualTheme.ButtonFill;
                image.type = Image.Type.Simple;
            }

            var shadow = button.GetComponent<Shadow>();
            if (shadow == null)
                shadow = button.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.65f);
            shadow.effectDistance = new Vector2(3f, -3f);

            var outline = button.GetComponent<Outline>();
            if (outline == null)
                outline = button.gameObject.AddComponent<Outline>();
            outline.useGraphicAlpha = true;
            outline.effectColor = MenuVisualTheme.OutlineIdle;
            outline.effectDistance = new Vector2(1f, -1f);

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.05f, 1.06f, 1.08f, 1f);
            colors.pressedColor = new Color(0.82f, 0.84f, 0.88f, 1f);
            colors.selectedColor = Color.white;
            button.colors = colors;
            button.transition = Selectable.Transition.ColorTint;

            var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                label.color = MenuVisualTheme.TextPrimary;
                label.enableAutoSizing = true;
                label.fontSizeMin = 18;
                label.fontSizeMax = 30;
            }
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

            Transform direct = gameOverPanel.transform.Find(name);
            if (direct != null)
                return direct.GetComponent<TextMeshProUGUI>();

            foreach (Transform t in gameOverPanel.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == name)
                    return t.GetComponent<TextMeshProUGUI>();
            }

            return null;
        }

        private Button FindButtonInPanel(string name)
        {
            if (gameOverPanel == null)
                return null;

            Transform direct = gameOverPanel.transform.Find(name);
            if (direct != null)
                return direct.GetComponent<Button>();

            foreach (Transform t in gameOverPanel.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == name)
                    return t.GetComponent<Button>();
            }

            return null;
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
            panelImage.color = new Color(
                MenuVisualTheme.PanelBackdrop.r,
                MenuVisualTheme.PanelBackdrop.g,
                MenuVisualTheme.PanelBackdrop.b,
                0.94f);
            panelImage.raycastTarget = true;

            var card = new GameObject("ContentCard");
            card.transform.SetParent(panelObj.transform, false);
            var cardRt = card.AddComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(560f, 520f);
            cardRt.anchoredPosition = Vector2.zero;

            var cardImg = card.AddComponent<Image>();
            cardImg.color = new Color(0.04f, 0.06f, 0.1f, 0.92f);
            cardImg.raycastTarget = false;

            var cardOutline = card.AddComponent<Outline>();
            cardOutline.effectColor = MenuVisualTheme.OutlineIdle;
            cardOutline.effectDistance = new Vector2(1.5f, -1.5f);

            var vlg = card.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(32, 32, 28, 28);
            vlg.spacing = 12f;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;

            CreateLayoutTitle(card.transform, "TxtGameOverTitle", "Game Over", 56f);
            CreateLayoutStat(card.transform, "TxtScoreFinal", "Pontuação: 0", 34f);
            CreateLayoutStat(card.transform, "TxtHighScoreFinal", "Recorde: 0", 30f);
            CreateLayoutStat(card.transform, "TxtCoinsFinal", "Moedas: 0", 30f);

            var spacer = new GameObject("Spacer");
            spacer.transform.SetParent(card.transform, false);
            var spLe = spacer.AddComponent<LayoutElement>();
            spLe.minHeight = 8f;
            spLe.preferredHeight = 12f;

            CreateLayoutButton(card.transform, "BtnRestart", "Jogar de novo");
            CreateLayoutButton(card.transform, "BtnMenu", "Voltar ao menu");

            return panelObj;
        }

        private static void CreateLayoutTitle(Transform parent, string name, string content, float fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 64f;
            le.preferredHeight = 72f;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = MenuVisualTheme.TextPrimary;
            tmp.outlineWidth = 0.22f;
            tmp.outlineColor = MenuVisualTheme.TitleOutline;
        }

        private static void CreateLayoutStat(Transform parent, string name, string content, float fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 36f;
            le.preferredHeight = 40f;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = MenuVisualTheme.TextMuted;
        }

        private static void CreateLayoutButton(Transform parent, string name, string label)
        {
            var buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            var le = buttonObj.AddComponent<LayoutElement>();
            le.minHeight = 52f;
            le.preferredHeight = 54f;
            le.minWidth = 320f;
            le.preferredWidth = 480f;

            var image = buttonObj.AddComponent<Image>();
            image.color = MenuVisualTheme.ButtonFill;
            image.type = Image.Type.Simple;

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
            buttonText.fontSize = 28;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = MenuVisualTheme.TextPrimary;
        }

        private void OnDestroy()
        {
            if (Managers.GameManager.Instance != null)
                Managers.GameManager.onGameOver -= ShowGameOver;
        }
    }
}
