using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UIBuilder - Constrói toda a interface do jogo automaticamente
/// Cria: Menu Principal, HUD, GameOver, Settings, Skin Shop
/// </summary>
public class UIBuilder : MonoBehaviour
{
    private void Awake()
    {
        BuildCompleteUI();
    }

    private void BuildCompleteUI()
    {
        // Main Menu
        BuildMainMenuCanvas();

        // Gameplay HUD
        BuildGameplayHUD();

        // Game Over
        BuildGameOverCanvas();

        // Settings Menu
        BuildSettingsCanvas();
        
        // Shop Screen
        BuildShopCanvas();
        
        // Leaderboard Screen
        BuildLeaderboardCanvas();
        
        // Daily Quests Screen
        BuildDailyQuestsCanvas();

        Debug.Log("✅ Interface completa construída com Shop, Ranking e Desafios!");
    }

    private void BuildMainMenuCanvas()
    {
        GameObject canvasGO = new GameObject("CanvasMainMenu");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        canvasGO.AddComponent<GraphicRaycaster>();

        // Background com gradiente
        Image bgImage = canvasGO.AddComponent<Image>();
        bgImage.color = new Color(0.08f, 0.08f, 0.12f, 1f);

        // SafeArea para notch
        SafeAreaFitter safeArea = canvasGO.AddComponent<SafeAreaFitter>();
        
        // Título com styling
        CreateTitle(canvasGO.transform, "GHOST BEAM", new Vector2(0, 350));

        // Botão JOGAR (destaque verde)
        Button playBtn = CreateMainMenuButton(canvasGO.transform, "Play", new Vector2(0, 200), new Color(0.2f, 1f, 0.2f, 1), 120);
        if (playBtn != null)
        {
            playBtn.onClick.AddListener(() =>
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.StartGameplayFromMenu();
            });
        }

        // Layout para 3 botões em linha
        // LOJA (Laranja)
        Button shopBtn = CreateMainMenuButton(canvasGO.transform, "LOJA", new Vector2(-270, 50), new Color(1f, 0.65f, 0.2f, 1), 90);
        if (shopBtn != null)
        {
            shopBtn.onClick.AddListener(() =>
            {
                MainMenuController menu = FindObjectOfType<MainMenuController>();
                if (menu != null) menu.OpenShop();
            });
        }

        // RANKING (Ciano)
        Button rankBtn = CreateMainMenuButton(canvasGO.transform, "RANKING", new Vector2(0, 50), new Color(0.2f, 1f, 1f, 1), 90);
        if (rankBtn != null)
        {
            rankBtn.onClick.AddListener(() =>
            {
                MainMenuController menu = FindObjectOfType<MainMenuController>();
                if (menu != null) menu.OpenLeaderboard();
            });
        }

        // DESAFIOS (Magenta)
        Button questsBtn = CreateMainMenuButton(canvasGO.transform, "DESAFIOS", new Vector2(270, 50), new Color(1f, 0.2f, 1f, 1), 90);
        if (questsBtn != null)
        {
            questsBtn.onClick.AddListener(() =>
            {
                MainMenuController menu = FindObjectOfType<MainMenuController>();
                if (menu != null) menu.OpenDailyQuests();
            });
        }

        // CONFIGURAÇÕES (Amarelo)
        Button settingsBtn = CreateMainMenuButton(canvasGO.transform, "CONFIG", new Vector2(0, -80), new Color(1f, 1f, 0.2f, 1), 90);
        if (settingsBtn != null)
        {
            settingsBtn.onClick.AddListener(() =>
            {
                MainMenuController menu = FindObjectOfType<MainMenuController>();
                if (menu != null) menu.OpenSettings();
            });
        }

        // SAIR (Vermelho)
        Button quitBtn = CreateMainMenuButton(canvasGO.transform, "SAIR", new Vector2(0, -150), new Color(1f, 0.3f, 0.3f, 1), 80);
        if (quitBtn != null)
        {
            quitBtn.onClick.AddListener(() =>
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            });
        }

        // Melhor Score (em baixo)
        GameObject bestScoreGO = new GameObject("BestScore");
        bestScoreGO.transform.SetParent(canvasGO.transform);
        TextMeshProUGUI bestScoreTM = bestScoreGO.AddComponent<TextMeshProUGUI>();
        bestScoreTM.text = "MELHOR: 0";
        bestScoreTM.fontSize = 48;
        bestScoreTM.fontStyle = FontStyles.Bold;
        bestScoreTM.alignment = TextAlignmentOptions.Bottom;
        bestScoreTM.color = new Color(1f, 0.84f, 0, 1);

        RectTransform bestScoreRect = bestScoreGO.GetComponent<RectTransform>();
        bestScoreRect.anchorMin = new Vector2(0.5f, 0);
        bestScoreRect.anchorMax = new Vector2(0.5f, 0);
        bestScoreRect.sizeDelta = new Vector2(900, 120);
        bestScoreRect.anchoredPosition = new Vector2(0, 40);
    }

    private void BuildGameplayHUD()
    {
        GameObject canvasGO = new GameObject("CanvasHUD");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Top Left: Vida e Moedas
        CreateHUDText(canvasGO.transform, "TxtVida", new Vector2(30, -30), TextAnchor.UpperLeft, "VIDA: 3/3", 36, new Color(1, 0.3f, 0.3f, 1));
        CreateHUDText(canvasGO.transform, "TxtMoedas", new Vector2(30, -80), TextAnchor.UpperLeft, "MOEDAS: 0", 32, new Color(1, 0.84f, 0, 1));

        // Top Center: Score
        CreateHUDText(canvasGO.transform, "TxtScore", new Vector2(0, -30), TextAnchor.UpperCenter, "SCORE: 0", 40, new Color(1, 1, 1, 1));

        // Top Right: Bateria
        CreateHUDText(canvasGO.transform, "TxtBateria", new Vector2(-30, -30), TextAnchor.UpperRight, "BATERIA: 100%", 36, new Color(0, 1, 1, 1));
        CreateHUDText(canvasGO.transform, "TxtTempo", new Vector2(-30, -80), TextAnchor.UpperRight, "TEMPO: 00:00", 32, new Color(0.8f, 0.8f, 0.8f, 1));

        // Bottom: Estágio e Performance
        CreateHUDText(canvasGO.transform, "TxtFase", new Vector2(30, 30), TextAnchor.LowerLeft, "STAGE: 1", 28, new Color(0.7f, 1, 0.7f, 1));
        CreateHUDText(canvasGO.transform, "TxtPerf", new Vector2(-30, 30), TextAnchor.LowerRight, "FPS: 60", 24, new Color(0.6f, 0.6f, 0.6f, 1));

        // Hidden for now
        CreateHUDText(canvasGO.transform, "TxtRecorde", new Vector2(30, -130), TextAnchor.UpperLeft, "RECORDE: 0", 28, new Color(1, 0.84f, 0, 0.7f));
    }

    private void BuildGameOverCanvas()
    {
        GameObject canvasGO = new GameObject("CanvasGameOver");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Dark overlay
        GameObject panelGO = new GameObject("Overlay");
        panelGO.transform.SetParent(canvasGO.transform);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.85f);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Panel de conteúdo
        GameObject contentPanelGO = new GameObject("PanelGameOver");
        contentPanelGO.transform.SetParent(canvasGO.transform);

        Image contentImage = contentPanelGO.AddComponent<Image>();
        contentImage.color = new Color(0.15f, 0.15f, 0.25f, 0.95f);

        RectTransform contentRect = contentPanelGO.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(800, 600);

        // Título GAME OVER
        CreateGameOverText(contentPanelGO.transform, "Title", "GAME OVER", new Vector2(0, 200), 60, Color.red);

        // Score Final
        CreateGameOverText(contentPanelGO.transform, "TxtScoreFinal", "SCORE FINAL: 0", new Vector2(0, 80), 48, Color.white);

        // Recorde
        CreateGameOverText(contentPanelGO.transform, "TxtRecordeFinal", "RECORDE: 0", new Vector2(0, 0), 44, new Color(1, 0.84f, 0, 1));

        // Tempo
        CreateGameOverText(contentPanelGO.transform, "TxtTempo", "SOBREVIVÊNCIA: 00:00", new Vector2(0, -80), 36, new Color(0.7f, 1, 0.7f, 1));

        // Botão Reiniciar
        CreateGameOverButton(contentPanelGO.transform, "BtnReiniciar", "REINICIAR", new Vector2(0, -200), Color.green);

        canvasGO.SetActive(false);
    }

    private void BuildSettingsCanvas()
    {
        GameObject canvasGO = new GameObject("CanvasSettings");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 150;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel
        GameObject panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.95f);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Título
        CreateGameOverText(panelGO.transform, "Title", "CONFIGURAÇÕES", new Vector2(0, 250), 50, Color.yellow);

        // Volume label
        CreateGameOverText(panelGO.transform, "VolumeLabel", "VOLUME", new Vector2(-200, 100), 36, Color.white);

        // Volume slider (placeholder)
        GameObject volumeSliderGO = new GameObject("VolumeSlider");
        volumeSliderGO.transform.SetParent(panelGO.transform);
        Slider volumeSlider = volumeSliderGO.AddComponent<Slider>();
        volumeSlider.direction = Slider.Direction.LeftToRight;

        RectTransform volumeSliderRect = volumeSliderGO.GetComponent<RectTransform>();
        volumeSliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        volumeSliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        volumeSliderRect.sizeDelta = new Vector2(400, 50);
        volumeSliderRect.anchoredPosition = new Vector2(0, 100);

        // Vibração toggle
        CreateGameOverText(panelGO.transform, "VibraLabel", "VIBRAÇÃO", new Vector2(-200, -50), 36, Color.white);

        GameObject vibraToggleGO = new GameObject("VibraToggle");
        vibraToggleGO.transform.SetParent(panelGO.transform);
        Toggle vibraToggle = vibraToggleGO.AddComponent<Toggle>();

        RectTransform vibraToggleRect = vibraToggleGO.GetComponent<RectTransform>();
        vibraToggleRect.anchorMin = new Vector2(0.5f, 0.5f);
        vibraToggleRect.anchorMax = new Vector2(0.5f, 0.5f);
        vibraToggleRect.sizeDelta = new Vector2(60, 60);
        vibraToggleRect.anchoredPosition = new Vector2(150, -50);

        // Botão Voltar
        Button backBtn = CreateGameOverButton(panelGO.transform, "BtnBack", "VOLTAR", new Vector2(0, -200), Color.red);
        backBtn.onClick.AddListener(() =>
        {
            MainMenuController menu = FindObjectOfType<MainMenuController>();
            if (menu != null) menu.CloseSettings();
        });

        canvasGO.SetActive(false);
    }

    private void BuildSkinShopCanvas()
    {
        GameObject canvasGO = new GameObject("CanvasSkins");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 140;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel
        GameObject panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Título
        CreateGameOverText(panelGO.transform, "Title", "SKINS", new Vector2(0, 250), 50, Color.white);

        // Luna Skin
        CreateGameOverText(panelGO.transform, "LunaSkinLabel", "Luna Skin - 40 Moedas", new Vector2(-300, 50), 32, new Color(0.7f, 1, 1, 1));
        CreateGameOverButton(panelGO.transform, "BtnLunaSkin", "DESBLOQUEAR", new Vector2(-300, -50), new Color(0, 0.8f, 1, 1));

        // Flash Skin
        CreateGameOverText(panelGO.transform, "FlashSkinLabel", "Flash Skin - 45 Moedas", new Vector2(300, 50), 32, new Color(1, 0.7f, 0, 1));
        CreateGameOverButton(panelGO.transform, "BtnFlashSkin", "DESBLOQUEAR", new Vector2(300, -50), new Color(1, 0.6f, 0, 1));

        // Botão Voltar
        CreateGameOverButton(panelGO.transform, "BtnBack", "VOLTAR", new Vector2(0, -200), Color.red);

        canvasGO.SetActive(false);
    }

    private void CreateTitle(Transform parent, string text, Vector2 position)
    {
        GameObject go = new GameObject("Title");
        go.transform.SetParent(parent);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 90;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.2f, 1f, 1f, 1);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(1000, 200);
        rect.anchoredPosition = position;
    }

    private Button CreateMainMenuButton(Transform parent, string label, Vector2 position, Color color, int fontSize = 80)
    {
        GameObject btnGO = new GameObject($"Btn{label}");
        btnGO.transform.SetParent(parent);

        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = color;

        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        // Adicionar ColorTransition para feedback visual
        ColorBlock colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(color.r * 1.3f, color.g * 1.3f, color.b * 1.3f, 1f);
        colors.pressedColor = new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f, 1f);
        colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        btn.colors = colors;

        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(480, 100);
        btnRect.anchoredPosition = position;

        // Texto do botão
        GameObject txtGO = new GameObject("Text");
        txtGO.transform.SetParent(btnGO.transform);

        TextMeshProUGUI txt = txtGO.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = fontSize;
        txt.fontStyle = FontStyles.Bold;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;

        RectTransform txtRect = txtGO.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        return btn;
    }

    private void CreateHUDText(Transform parent, string name, Vector2 position, TextAnchor alignment, string text, int fontSize, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = position;
        rect.sizeDelta = new Vector2(500, 100);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment == TextAnchor.UpperLeft ? TextAlignmentOptions.TopLeft : 
                        alignment == TextAnchor.UpperCenter ? TextAlignmentOptions.Top :
                        alignment == TextAnchor.UpperRight ? TextAlignmentOptions.TopRight :
                        alignment == TextAnchor.LowerLeft ? TextAlignmentOptions.BottomLeft : TextAlignmentOptions.BottomRight;
    }

    private void CreateGameOverText(Transform parent, string name, string text, Vector2 position, int fontSize, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(700, 100);
        rect.anchoredPosition = position;
    }

    private void BuildShopCanvas()
    {
        GameObject canvasGO = new GameObject("CanvasShop");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 160;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Adicionar ShopScreenController
        ShopScreenController shopController = canvasGO.AddComponent<ShopScreenController>();

        // Panel
        GameObject panelGO = new GameObject("PanelLoja");
        panelGO.transform.SetParent(canvasGO.transform);
        shopController.shopPanelRoot = panelGO;

        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.95f);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Título
        CreateGameOverText(panelGO.transform, "Title", "LOJA", new Vector2(0, 250), 50, new Color(1f, 0.65f, 0.2f, 1));

        // Moedas
        GameObject coinsGO = new GameObject("CoinsDisplay");
        coinsGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI coinsTM = coinsGO.AddComponent<TextMeshProUGUI>();
        coinsTM.text = "MOEDAS: 0";
        coinsTM.fontSize = 36;
        coinsTM.fontStyle = FontStyles.Bold;
        coinsTM.alignment = TextAlignmentOptions.Center;
        coinsTM.color = new Color(1f, 0.84f, 0, 1);
        shopController.shopCoinsText = coinsTM;

        RectTransform coinsRect = coinsGO.GetComponent<RectTransform>();
        coinsRect.anchorMin = new Vector2(0.5f, 0.5f);
        coinsRect.anchorMax = new Vector2(0.5f, 0.5f);
        coinsRect.sizeDelta = new Vector2(600, 100);
        coinsRect.anchoredPosition = new Vector2(0, 180);

        // Items da loja (3 botões) + guardar referências
        Color[] itemColors = { new Color(0, 0.8f, 1, 1), new Color(1, 0.6f, 0, 1), new Color(0.6f, 0.2f, 1, 1) };
        string[] itemNames = { "Item 1\n100 coins", "Item 2\n150 coins", "Item 3\n200 coins" };

        Button[] shopButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            shopButtons[i] = CreateGameOverButton(panelGO.transform, $"BtnItem{i}", itemNames[i], new Vector2((i - 1) * 250, 50), itemColors[i]);
        }
        shopController.buyButtons = shopButtons;

        // Botão Voltar
        Button closeBtn = CreateGameOverButton(panelGO.transform, "BtnClose", "VOLTAR", new Vector2(0, -200), Color.red);
        closeBtn.onClick.AddListener(() =>
        {
            MainMenuController menu = FindObjectOfType<MainMenuController>();
            if (menu != null) menu.CloseShop();
        });
        shopController.closeButton = closeBtn;

        canvasGO.SetActive(false);
    }

    private void BuildLeaderboardCanvas()
    {
        GameObject canvasGO = new GameObject("CanvasLeaderboard");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 155;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Adicionar LeaderboardScreenController
        LeaderboardScreenController leaderboardController = canvasGO.AddComponent<LeaderboardScreenController>();

        // Panel
        GameObject panelGO = new GameObject("PanelLeaderboard");
        panelGO.transform.SetParent(canvasGO.transform);
        leaderboardController.leaderboardPanelRoot = panelGO;

        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.95f);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Título
        CreateGameOverText(panelGO.transform, "Title", "RANKING", new Vector2(0, 250), 50, new Color(0.2f, 1f, 1f, 1));

        // Rank do jogador
        GameObject rankGO = new GameObject("PlayerRank");
        rankGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI rankTM = rankGO.AddComponent<TextMeshProUGUI>();
        rankTM.text = "SEU RANK: #1";
        rankTM.fontSize = 32;
        rankTM.fontStyle = FontStyles.Bold;
        rankTM.alignment = TextAlignmentOptions.Center;
        rankTM.color = new Color(1f, 0.84f, 0, 1);
        leaderboardController.playerRankText = rankTM;

        RectTransform rankRect = rankGO.GetComponent<RectTransform>();
        rankRect.anchorMin = new Vector2(0.5f, 0.5f);
        rankRect.anchorMax = new Vector2(0.5f, 0.5f);
        rankRect.sizeDelta = new Vector2(600, 100);
        rankRect.anchoredPosition = new Vector2(0, 180);

        // Content area para ranking (será populado dinamicamente)
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(panelGO.transform);
        leaderboardController.leaderboardContent = contentGO.transform;

        RectTransform contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(700, 300);
        contentRect.anchoredPosition = new Vector2(0, 0);

        VerticalLayoutGroup layout = contentGO.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.spacing = 10;

        // Botão Voltar
        Button closeBtn = CreateGameOverButton(panelGO.transform, "BtnClose", "VOLTAR", new Vector2(0, -200), Color.red);
        closeBtn.onClick.AddListener(() =>
        {
            MainMenuController menu = FindObjectOfType<MainMenuController>();
            if (menu != null) menu.CloseLeaderboard();
        });
        leaderboardController.closeButton = closeBtn;

        canvasGO.SetActive(false);
    }

    private void BuildDailyQuestsCanvas()
    {
        GameObject canvasGO = new GameObject("CanvasDailyQuests");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 150;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Adicionar DailyQuestsScreenController
        DailyQuestsScreenController questsController = canvasGO.AddComponent<DailyQuestsScreenController>();

        // Panel
        GameObject panelGO = new GameObject("PanelDailyQuests");
        panelGO.transform.SetParent(canvasGO.transform);
        questsController.questsPanelRoot = panelGO;

        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.95f);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Título
        CreateGameOverText(panelGO.transform, "Title", "DESAFIOS", new Vector2(0, 250), 50, new Color(1f, 0.2f, 1f, 1));

        // Reset time
        GameObject timeGO = new GameObject("ResetTime");
        timeGO.transform.SetParent(panelGO.transform);
        TextMeshProUGUI timeTM = timeGO.AddComponent<TextMeshProUGUI>();
        timeTM.text = "RESET: 23:45";
        timeTM.fontSize = 28;
        timeTM.fontStyle = FontStyles.Bold;
        timeTM.alignment = TextAlignmentOptions.Center;
        timeTM.color = new Color(0.7f, 0.7f, 0.7f, 1);
        questsController.resetTimeText = timeTM;

        RectTransform timeRect = timeGO.GetComponent<RectTransform>();
        timeRect.anchorMin = new Vector2(0.5f, 0.5f);
        timeRect.anchorMax = new Vector2(0.5f, 0.5f);
        timeRect.sizeDelta = new Vector2(600, 80);
        timeRect.anchoredPosition = new Vector2(0, 180);

        // Content area para quests
        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(panelGO.transform);
        questsController.questsContent = contentGO.transform;

        RectTransform contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(700, 300);
        contentRect.anchoredPosition = new Vector2(0, 20);

        VerticalLayoutGroup layout = contentGO.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.spacing = 15;

        // Botão Voltar
        Button closeBtn = CreateGameOverButton(panelGO.transform, "BtnClose", "VOLTAR", new Vector2(0, -200), Color.red);
        closeBtn.onClick.AddListener(() =>
        {
            MainMenuController menu = FindObjectOfType<MainMenuController>();
            if (menu != null) menu.CloseDailyQuests();
        });
        questsController.closeButton = closeBtn;

        canvasGO.SetActive(false);
    }

    private Button CreateGameOverButton(Transform parent, string name, string label, Vector2 position, Color color)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent);

        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = color;

        Button btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        // Adicionar ColorTransition para feedback visual
        ColorBlock colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(color.r * 1.3f, color.g * 1.3f, color.b * 1.3f, 1f);
        colors.pressedColor = new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f, 1f);
        colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        btn.colors = colors;

        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(350, 70);
        btnRect.anchoredPosition = position;

        // Texto
        GameObject txtGO = new GameObject("Text");
        txtGO.transform.SetParent(btnGO.transform);

        TextMeshProUGUI txt = txtGO.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 32;
        txt.fontStyle = FontStyles.Bold;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;

        RectTransform txtRect = txtGO.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;
        
        return btn;
    }
}
