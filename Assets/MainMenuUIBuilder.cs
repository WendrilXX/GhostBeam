using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// MEGA Script - Cria e gerencia TODA a UI do jogo em um único arquivo.
/// - Cria 5 Canvases automáticos
/// - Gerencia navegação de menus
/// - Exibe HUD em tempo real
/// - Detecta Game Over
/// - Controla Pausa com ESC
/// </summary>
public class MainMenuUIBuilder : MonoBehaviour
{
    [Header("Canvas References")]
    private Canvas mainMenuCanvas;
    private Canvas pauseMenuCanvas;
    private Canvas settingsCanvas;
    private Canvas shopCanvas;
    private Canvas gameOverCanvas;
    private Canvas hudCanvas;
    private Canvas tutorialCanvas;

    [Header("Button References")]
    private Button playButton, settingsButton, shopButton, quitButton;
    private Button resumeButton, pauseSettingsButton, pauseQuitButton;
    private Button settingsBackButton, restartButton, menuButton;
    private Button shopBackButton, buyUpgrade1, buyUpgrade2, buyUpgrade3;

    [Header("HUD References")]
    private TextMeshProUGUI healthText, scoreText, highscoreText, survivalTimeText, waveText, coinsText, batteryText;
    private Image healthBar;
    private Button pauseInGameButton;

    [Header("Settings")]
    private Slider volumeSlider;
    private Toggle vibrationToggle;

    [Header("Game Over Texts")]
    private TextMeshProUGUI gameOverScoreText, gameOverHighscoreText;

    // ===== ESTADO DO JOGO =====
    private float survivalTime = 0f;
    private bool isInGameplay = false;
    private bool isPaused = false;

    private void Start()
    {
        // Verificar se tem EventSystem (necessário para UI ser clicável)
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("[MainMenuUIBuilder] ✅ EventSystem criado automaticamente");
        }

        if (transform.Find("MainMenuCanvas") == null)
        {
            Debug.Log("[MainMenuUIBuilder] 🎮 Criando UI completa...");
            CreateCompleteUI();
            SetupEventListeners();
            ShowMainMenu();
        }
    }

    private void Update()
    {
        // Atualizar HUD em tempo real
        if (isInGameplay && hudCanvas.gameObject.activeInHierarchy)
        {
            UpdateHUDValues();
        }

        // Detectar tecla ESC para pausar
        if (Input.GetKeyDown(KeyCode.Escape) && isInGameplay && !gameOverCanvas.gameObject.activeInHierarchy)
        {
            TogglePause();
        }

        // Detectar Game Over (quando Luna morre)
        DetectGameOver();
    }

    private void CreateCompleteUI()
    {
        // ===== CRIAR TODOS OS 7 CANVASES =====
        mainMenuCanvas = CreateCanvas("MainMenuCanvas", true).GetComponent<Canvas>();
        pauseMenuCanvas = CreateCanvas("PauseMenuCanvas", false).GetComponent<Canvas>();
        settingsCanvas = CreateCanvas("SettingsCanvas", false).GetComponent<Canvas>();
        shopCanvas = CreateCanvas("ShopCanvas", false).GetComponent<Canvas>();
        gameOverCanvas = CreateCanvas("GameOverCanvas", false).GetComponent<Canvas>();
        hudCanvas = CreateCanvas("HUDCanvas", false).GetComponent<Canvas>();
        tutorialCanvas = CreateCanvas("TutorialCanvas", false).GetComponent<Canvas>();

        // ===== MAIN MENU =====
        CreateMainMenuUI(mainMenuCanvas.gameObject);

        // ===== PAUSE MENU =====
        CreatePauseMenuUI(pauseMenuCanvas.gameObject);

        // ===== SETTINGS =====
        CreateSettingsUI(settingsCanvas.gameObject);

        // ===== SHOP =====
        CreateShopUI(shopCanvas.gameObject);

        // ===== TUTORIAL =====
        CreateTutorialUI(tutorialCanvas.gameObject);

        // ===== GAME OVER =====
        CreateGameOverUI(gameOverCanvas.gameObject);

        // ===== HUD =====
        CreateHUDUI(hudCanvas.gameObject);
    }

    private GameObject CreateCanvas(string name, bool active)
    {
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        canvasObj.SetActive(active);
        canvasObj.transform.SetParent(transform, false);
        return canvasObj;
    }

    // ===== MAIN MENU =====
    private void CreateMainMenuUI(GameObject canvasObj)
    {
        // Fundo
        CreateBackground(canvasObj, new Color(0.1f, 0.1f, 0.15f, 1f));

        // Panel Principal
        GameObject panelObj = new GameObject("PanelMenu");
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(600, 800);

        VerticalLayoutGroup layoutGroup = panelObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 40;
        layoutGroup.padding = new RectOffset(60, 60, 60, 60);
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Título
        GameObject titleObj = CreateText(panelObj, "TxtTitulo", "GHOST BEAM", 80);
        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 150;

        // Botões - callbacks adicionados em SetupEventListeners()
        CreateMenuButton(panelObj, "BtnPlay", "PLAY", new Color(0.2f, 0.8f, 0.3f, 1f), null);
        CreateMenuButton(panelObj, "BtnShop", "LOJA", new Color(0.8f, 0.6f, 0.2f, 1f), null);
        CreateMenuButton(panelObj, "BtnTutorial", "📚 TUTORIAL", new Color(0.5f, 0.6f, 0.8f, 1f), null);
        CreateMenuButton(panelObj, "BtnSettings", "CONFIGURAÇÃO", new Color(0.3f, 0.5f, 0.8f, 1f), null);
        CreateMenuButton(panelObj, "BtnQuit", "SAIR", new Color(0.8f, 0.2f, 0.2f, 1f), null);
    }

    // ===== PAUSE MENU =====
    private void CreatePauseMenuUI(GameObject canvasObj)
    {
        CreateBackground(canvasObj, new Color(0, 0, 0, 0.7f));

        GameObject panelObj = new GameObject("PausePanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(600, 500);

        VerticalLayoutGroup layoutGroup = panelObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 30;
        layoutGroup.padding = new RectOffset(60, 60, 60, 60);
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Título
        GameObject titleObj = CreateText(panelObj, "TxtPauseTitle", "PAUSED", 60);
        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 100;

        // Botões - callbacks adicionados em SetupEventListeners()
        CreateMenuButton(panelObj, "BtnResume", "RESUME", new Color(0.2f, 0.8f, 0.3f, 1f), null);
        CreateMenuButton(panelObj, "BtnPauseSettings", "CONFIGURAÇÃO", new Color(0.3f, 0.5f, 0.8f, 1f), null);
        CreateMenuButton(panelObj, "BtnPauseQuitToMenu", "MENU", new Color(0.8f, 0.2f, 0.2f, 1f), null);
    }

    // ===== SETTINGS MENU =====
    private void CreateSettingsUI(GameObject canvasObj)
    {
        CreateBackground(canvasObj, new Color(0.1f, 0.1f, 0.15f, 1f));

        GameObject panelObj = new GameObject("SettingsPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(700, 700);

        VerticalLayoutGroup layoutGroup = panelObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 30;
        layoutGroup.padding = new RectOffset(60, 60, 60, 60);
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Título
        GameObject titleObj = CreateText(panelObj, "TxtSettingsTitle", "CONFIGURAÇÃO", 70);
        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 100;

        // ===== VOLUME SECTION =====
        CreateText(panelObj, "TxtVolumeLabel", "🔊 VOLUME", 35);

        // Volume Slider Container
        GameObject sliderContainerObj = new GameObject("VolumeSliderContainer");
        sliderContainerObj.transform.SetParent(panelObj.transform, false);
        HorizontalLayoutGroup sliderLayout = sliderContainerObj.AddComponent<HorizontalLayoutGroup>();
        sliderLayout.spacing = 10;
        sliderLayout.padding = new RectOffset(10, 10, 5, 5);
        sliderLayout.childForceExpandWidth = true;
        sliderLayout.childForceExpandHeight = false;

        // Min Volume Label
        CreateText(sliderContainerObj, "TxtMinVolume", "0%", 20);

        // Volume Slider
        GameObject sliderObj = new GameObject("SliderVolume");
        sliderObj.transform.SetParent(sliderContainerObj.transform, false);
        Slider volumeSlider = sliderObj.AddComponent<Slider>();
        volumeSlider.minValue = 0;
        volumeSlider.maxValue = 1;
        volumeSlider.value = AudioListener.volume;
        volumeSlider.direction = Slider.Direction.LeftToRight;
        
        // Slider background
        Image sliderBgImage = sliderObj.AddComponent<Image>();
        sliderBgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(400, 50);
        LayoutElement sliderLayoutElem = sliderObj.AddComponent<LayoutElement>();
        sliderLayoutElem.preferredWidth = 400;
        sliderLayoutElem.preferredHeight = 50;

        // Max Volume Label
        CreateText(sliderContainerObj, "TxtMaxVolume", "100%", 20);

        LayoutElement sliderContainerLayout = sliderContainerObj.AddComponent<LayoutElement>();
        sliderContainerLayout.preferredHeight = 70;

        // Volume Value Display
        GameObject volumeValueObj = CreateText(panelObj, "TxtVolumeValue", $"Volume: {(int)(AudioListener.volume * 100)}%", 25);
        volumeValueObj.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.9f, 1f, 1f);
        LayoutElement volumeValueLayout = volumeValueObj.AddComponent<LayoutElement>();
        volumeValueLayout.preferredHeight = 40;

        // Separador
        GameObject separatorObj1 = new GameObject("Separator1");
        separatorObj1.transform.SetParent(panelObj.transform, false);
        Image separatorImage1 = separatorObj1.AddComponent<Image>();
        separatorImage1.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        RectTransform separatorRect1 = separatorObj1.GetComponent<RectTransform>();
        separatorRect1.sizeDelta = new Vector2(500, 1);
        LayoutElement separatorLayout1 = separatorObj1.AddComponent<LayoutElement>();
        separatorLayout1.preferredHeight = 15;

        // ===== VIBRATION SECTION =====
        CreateText(panelObj, "TxtVibrationLabel", "📳 VIBRAÇÃO", 35);

        // Vibration Toggle Container
        GameObject toggleContainerObj = new GameObject("VibrationToggleContainer");
        toggleContainerObj.transform.SetParent(panelObj.transform, false);
        HorizontalLayoutGroup toggleLayout = toggleContainerObj.AddComponent<HorizontalLayoutGroup>();
        toggleLayout.spacing = 20;
        toggleLayout.padding = new RectOffset(10, 10, 5, 5);
        toggleLayout.childForceExpandWidth = false;
        toggleLayout.childForceExpandHeight = false;

        // Vibration Toggle
        GameObject toggleObj = new GameObject("ToggleVibration");
        toggleObj.transform.SetParent(toggleContainerObj.transform, false);
        Toggle vibrationToggle = toggleObj.AddComponent<Toggle>();
        vibrationToggle.isOn = true;
        Image toggleBgImage = toggleObj.AddComponent<Image>();
        toggleBgImage.color = vibrationToggle.isOn ? new Color(0.2f, 0.8f, 0.3f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f);
        RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
        toggleRect.sizeDelta = new Vector2(80, 60);
        LayoutElement toggleLayoutElem = toggleObj.AddComponent<LayoutElement>();
        toggleLayoutElem.preferredWidth = 80;
        toggleLayoutElem.preferredHeight = 60;

        // Vibration Status Text
        GameObject vibrationStatusObj = CreateText(toggleContainerObj, "TxtVibrationStatus", "ON", 28);
        vibrationStatusObj.GetComponent<TextMeshProUGUI>().color = new Color(0.2f, 0.8f, 0.3f, 1f);

        LayoutElement toggleContainerLayout = toggleContainerObj.AddComponent<LayoutElement>();
        toggleContainerLayout.preferredHeight = 80;

        // Separador
        GameObject separatorObj2 = new GameObject("Separator2");
        separatorObj2.transform.SetParent(panelObj.transform, false);
        Image separatorImage2 = separatorObj2.AddComponent<Image>();
        separatorImage2.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        RectTransform separatorRect2 = separatorObj2.GetComponent<RectTransform>();
        separatorRect2.sizeDelta = new Vector2(500, 1);
        LayoutElement separatorLayout2 = separatorObj2.AddComponent<LayoutElement>();
        separatorLayout2.preferredHeight = 15;

        // ===== ABOUT SECTION =====
        CreateText(panelObj, "TxtAbout", "ℹ️ SOBRE", 25);
        CreateText(panelObj, "TxtAboutDesc", "GHOSTBEAM v1.1\nPor Ghost Beam Dev Team", 18).GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f, 1f);

        // Botão Voltar - callback adicionado em SetupEventListeners()
        CreateMenuButton(panelObj, "BtnSettingsBack", "VOLTAR", new Color(0.5f, 0.5f, 0.5f, 1f), null);
    }

    // ===== SHOP MENU =====
    private void CreateShopUI(GameObject canvasObj)
    {
        CreateBackground(canvasObj, new Color(0.1f, 0.1f, 0.15f, 1f));

        GameObject panelObj = new GameObject("ShopPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(700, 800);

        VerticalLayoutGroup layoutGroup = panelObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 25;
        layoutGroup.padding = new RectOffset(60, 60, 60, 60);
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Título
        GameObject titleObj = CreateText(panelObj, "TxtShopTitle", "LOJA", 70);
        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 100;

        // Coins Display
        GameObject coinsDisplayObj = CreateText(panelObj, "TxtCoinsDisplay", "Moedas: 0", 35);
        LayoutElement coinsLayout = coinsDisplayObj.AddComponent<LayoutElement>();
        coinsLayout.preferredHeight = 50;

        // Separador
        GameObject separatorObj = new GameObject("Separator");
        separatorObj.transform.SetParent(panelObj.transform, false);
        Image separatorImage = separatorObj.AddComponent<Image>();
        separatorImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        RectTransform separatorRect = separatorObj.GetComponent<RectTransform>();
        separatorRect.sizeDelta = new Vector2(500, 2);
        LayoutElement separatorLayout = separatorObj.AddComponent<LayoutElement>();
        separatorLayout.preferredHeight = 10;

        // Upgrade 1: Lanterna Melhorada
        CreateShopItem(panelObj, "Upgrade1", "LANTERNA MELHORADA", "Dano +10%\nCusto: 50 moedas", "BuyUpgrade1", 0.2f, 0.6f, 0.8f);

        // Upgrade 2: Bateria Maior
        CreateShopItem(panelObj, "Upgrade2", "BATERIA MAIOR", "Duração +30%\nCusto: 75 moedas", "BuyUpgrade2", 0.8f, 0.6f, 0.2f);

        // Upgrade 3: Velocidade
        CreateShopItem(panelObj, "Upgrade3", "VELOCIDADE", "Movimento +20%\nCusto: 60 moedas", "BuyUpgrade3", 0.2f, 0.8f, 0.3f);

        // Botão Voltar
        CreateMenuButton(panelObj, "BtnShopBack", "VOLTAR", new Color(0.5f, 0.5f, 0.5f, 1f), null);
    }

    // Função auxiliar para criar items da loja
    private void CreateShopItem(GameObject parentObj, string itemName, string title, string description, string buttonName, float r, float g, float b)
    {
        GameObject itemObj = new GameObject(itemName);
        itemObj.transform.SetParent(parentObj.transform, false);
        RectTransform itemRect = itemObj.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(600, 120);

        HorizontalLayoutGroup hLayout = itemObj.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 15;
        hLayout.padding = new RectOffset(15, 15, 10, 10);
        hLayout.childForceExpandWidth = true;
        hLayout.childForceExpandHeight = true;

        // Descrição (esquerda)
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(itemObj.transform, false);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        
        VerticalLayoutGroup vLayout = descObj.AddComponent<VerticalLayoutGroup>();
        vLayout.spacing = 5;
        vLayout.childForceExpandWidth = true;
        vLayout.childForceExpandHeight = false;

        TextMeshProUGUI titleText = CreateText(descObj, "Title", title, 28).GetComponent<TextMeshProUGUI>();
        titleText.alignment = TextAlignmentOptions.Left;
        
        TextMeshProUGUI descText = CreateText(descObj, "Desc", description, 16).GetComponent<TextMeshProUGUI>();
        descText.alignment = TextAlignmentOptions.Left;
        descText.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        // Botão de compra (direita)
        GameObject btnObj = new GameObject(buttonName);
        btnObj.transform.SetParent(itemObj.transform, false);
        Button buyBtn = btnObj.AddComponent<Button>();
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(r, g, b, 1f);
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(150, 100);

        TextMeshProUGUI btnText = CreateText(btnObj, "TextBuy", "COMPRAR", 24).GetComponent<TextMeshProUGUI>();
        btnText.alignment = TextAlignmentOptions.Center;

        LayoutElement itemLayout = itemObj.AddComponent<LayoutElement>();
        itemLayout.preferredHeight = 130;
    }

    // ===== TUTORIAL =====
    private void CreateTutorialUI(GameObject canvasObj)
    {
        CreateBackground(canvasObj, new Color(0, 0, 0, 0.9f));

        GameObject panelObj = new GameObject("TutorialPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(1000, 900);

        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        // Título
        GameObject titleObj = CreateText(panelObj, "Title", "📚 GUIA DA LOJA 📚", 48);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 400);

        // Conteúdo do Tutorial
        ScrollRect scroll = panelObj.AddComponent<ScrollRect>();
        scroll.vertical = true;
        scroll.horizontal = false;

        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(panelObj.transform, false);
        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(950, 1200);
        contentRect.anchoredPosition = new Vector2(0, 50);

        VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 20;
        layoutGroup.childForceExpandHeight = false;

        // Seção 1: Como Ganhar Moedas
        CreateTutorialSection(contentObj, "🪙 COMO GANHAR MOEDAS", 
            new string[] {
                "• Derrote inimigos: Ganhe moedas",
                "• Sobreviva mais tempo: Mais moedas",
                "• Colete pickups de moeda: +50 moedas"
            });

        // Seção 2: Sobre Upgrades
        CreateTutorialSection(contentObj, "⚡ O QUE SÃO UPGRADES?", 
            new string[] {
                "Upgrades aprimoram suas habilidades permanentemente",
                "Cada upgrade pode ser comprado até 3 vezes",
                "Os bônus se acumulam (3x = efeito máximo)",
                "Seus upgrades são salvos entre sessões!"
            });

        // Seção 3: Upgrade 1
        CreateTutorialUpgradeInfo(contentObj, 1, "⚡ LANTERNA MELHORADA", "+10% de dano por compra");

        // Seção 4: Upgrade 2
        CreateTutorialUpgradeInfo(contentObj, 2, "🔋 BATERIA MAIOR", "+30% de duração da bateria");

        // Seção 5: Upgrade 3
        CreateTutorialUpgradeInfo(contentObj, 3, "💨 VELOCIDADE", "+20% de movimento");

        // Seção 6: Dicas
        CreateTutorialSection(contentObj, "💡 DICAS PROFISSIONAIS", 
            new string[] {
                "• Priorize a velocidade para sobreviver mais",
                "• Aumente o dano para derrotar inimigos rápido",
                "• Bateria maior = mais tempo atacando",
                "• Combine upgrades para máximo potencial!"
            });

        scroll.content = contentRect;

        // Botão Voltar
        CreateMenuButton(panelObj, "BackButton", "← VOLTAR", new Color(0.5f, 0.5f, 0.5f, 1f), null);
        
        // Encontrar o botão e adicionar o callback
        Button backBtn = panelObj.transform.Find("BackButton")?.GetComponent<Button>();
        if (backBtn != null) backBtn.onClick.AddListener(HideTutorial);
    }

    /// <summary>
    /// Cria uma seção de tutorial com múltiplas linhas
    /// </summary>
    private void CreateTutorialSection(GameObject parent, string title, string[] lines)
    {
        GameObject sectionObj = new GameObject("Section");
        sectionObj.transform.SetParent(parent.transform, false);
        LayoutElement sectionLayout = sectionObj.AddComponent<LayoutElement>();
        sectionLayout.preferredHeight = 50 + (lines.Length * 30);

        VerticalLayoutGroup sectionGroup = sectionObj.AddComponent<VerticalLayoutGroup>();
        sectionGroup.childForceExpandHeight = false;
        sectionGroup.spacing = 5;

        // Título da seção
        TextMeshProUGUI titleText = CreateText(sectionObj, "SectionTitle", title, 32)
            .GetComponent<TextMeshProUGUI>();
        titleText.color = new Color(1f, 0.8f, 0.3f);
        titleText.alignment = TextAlignmentOptions.BottomLeft;

        // Conteúdo
        foreach (string line in lines)
        {
            TextMeshProUGUI lineText = CreateText(sectionObj, "Line", line, 20)
                .GetComponent<TextMeshProUGUI>();
            lineText.color = new Color(0.9f, 0.9f, 0.9f);
            lineText.alignment = TextAlignmentOptions.BottomLeft;
            lineText.fontSize = 20;
        }
    }

    /// <summary>
    /// Cria informações sobre um upgrade específico no tutorial
    /// </summary>
    private void CreateTutorialUpgradeInfo(GameObject parent, int upgradeId, string title, string bonus)
    {
        GameObject upgradeObj = new GameObject("Upgrade" + upgradeId);
        upgradeObj.transform.SetParent(parent.transform, false);
        LayoutElement upgradeLayout = upgradeObj.AddComponent<LayoutElement>();
        upgradeLayout.preferredHeight = 120;

        Image upgradeBg = upgradeObj.AddComponent<Image>();
        upgradeBg.color = new Color(0.15f, 0.15f, 0.25f);

        VerticalLayoutGroup upgradeGroup = upgradeObj.AddComponent<VerticalLayoutGroup>();
        upgradeGroup.childForceExpandHeight = false;
        upgradeGroup.spacing = 5;
        upgradeGroup.padding = new RectOffset(15, 15, 10, 10);

        // Título
        TextMeshProUGUI titleText = CreateText(upgradeObj, "Title", title, 28)
            .GetComponent<TextMeshProUGUI>();
        titleText.color = new Color(1f, 0.8f, 0.3f);

        // Bonus
        TextMeshProUGUI bonusText = CreateText(upgradeObj, "Bonus", bonus, 22)
            .GetComponent<TextMeshProUGUI>();
        bonusText.color = new Color(0.7f, 1f, 0.7f);

        // Descrição completa
        string desc = UpgradeManager.Instance != null 
            ? UpgradeManager.Instance.GetShopUpgradeDescription(upgradeId)
            : "Upgrade desconhecido";
        
        TextMeshProUGUI descText = CreateText(upgradeObj, "Desc", desc, 18)
            .GetComponent<TextMeshProUGUI>();
        descText.color = new Color(0.8f, 0.8f, 0.8f);
    }

    /// <summary>
    /// Mostra o tutorial
    /// </summary>
    public void ShowTutorial()
    {
        tutorialCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// Esconde o tutorial
    /// </summary>
    public void HideTutorial()
    {
        tutorialCanvas.gameObject.SetActive(false);
    }

    // ===== GAME OVER MENU =====
    private void CreateGameOverUI(GameObject canvasObj)
    {
        CreateBackground(canvasObj, new Color(0, 0, 0, 0.8f));

        GameObject panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(600, 700);

        VerticalLayoutGroup layoutGroup = panelObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 30;
        layoutGroup.padding = new RectOffset(60, 60, 60, 60);
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Título
        GameObject titleObj = CreateText(panelObj, "TxtGameOverTitle", "GAME OVER", 80);
        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 120;

        // Score
        CreateText(panelObj, "TxtGameOverScore", "Score: 0", 40);

        // Highscore
        CreateText(panelObj, "TxtGameOverHighscore", "Highscore: 0", 40);

        // Botões - callbacks adicionados em SetupEventListeners()
        CreateMenuButton(panelObj, "BtnRestart", "RESTART", new Color(0.2f, 0.8f, 0.3f, 1f), null);
        CreateMenuButton(panelObj, "BtnGameOverMenu", "MENU", new Color(0.8f, 0.2f, 0.2f, 1f), null);
    }

    // ===== HUD =====
    private void CreateHUDUI(GameObject canvasObj)
    {
        // Layout Grid para HUD
        GameObject gridObj = new GameObject("HUDGrid");
        gridObj.transform.SetParent(canvasObj.transform, false);
        GridLayoutGroup gridLayout = gridObj.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(200, 60);
        gridLayout.spacing = new Vector2(20, 20);
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;

        RectTransform gridRect = gridObj.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0, 1);
        gridRect.anchorMax = new Vector2(1, 1);
        gridRect.pivot = new Vector2(0.5f, 1);
        gridRect.offsetMin = new Vector2(10, -300);
        gridRect.offsetMax = new Vector2(-10, -10);

        // Health
        CreateText(gridObj, "TxtHealth", "Health: 100/100", 24);
        CreateHealthBar(gridObj);

        // Score
        CreateText(gridObj, "TxtScore", "Score: 0", 24);

        // Highscore
        CreateText(gridObj, "TxtHighscore", "Highscore: 0", 24);

        // Survival Time
        CreateText(gridObj, "TxtSurvivalTime", "Time: 00:00", 24);

        // Wave
        CreateText(gridObj, "TxtWave", "Wave: 1", 24);

        // Coins
        CreateText(gridObj, "TxtCoins", "Coins: 0", 24);

        // Battery
        CreateText(gridObj, "TxtBattery", "Battery: 100%", 24);

        // Pause Button (Bottom Right) - callback adicionado em SetupEventListeners()
        GameObject btnPauseObj = new GameObject("BtnPauseInGame");
        btnPauseObj.transform.SetParent(canvasObj.transform, false);
        Button pauseBtn = btnPauseObj.AddComponent<Button>();
        Image pauseBtnImage = btnPauseObj.AddComponent<Image>();
        pauseBtnImage.color = new Color(0.3f, 0.5f, 0.8f, 1f);
        pauseBtn.targetGraphic = pauseBtnImage;

        RectTransform pauseRect = btnPauseObj.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(1, 1);
        pauseRect.anchorMax = new Vector2(1, 1);
        pauseRect.pivot = new Vector2(1, 1);
        pauseRect.offsetMin = new Vector2(-120, -120);
        pauseRect.offsetMax = new Vector2(-20, -20);

        GameObject pauseTextObj = CreateText(btnPauseObj, "Text - Pause", "PAUSE", 30);
    }

    private GameObject CreateMenuButton(GameObject parent, string name, string label, Color color, UnityEngine.Events.UnityAction callback)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);

        Button button = btnObj.AddComponent<Button>();
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = color;

        button.targetGraphic = btnImage;
        
        // Adicionar callback apenas se não for null
        if (callback != null)
            button.onClick.AddListener(callback);

        // Color Transition
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(Mathf.Min(color.r + 0.2f, 1), Mathf.Min(color.g + 0.2f, 1), Mathf.Min(color.b + 0.2f, 1), 1);
        colors.pressedColor = new Color(Mathf.Max(color.r - 0.2f, 0), Mathf.Max(color.g - 0.2f, 0), Mathf.Max(color.b - 0.2f, 0), 1);
        button.colors = colors;

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(500, 80);
        LayoutElement btnLayout = btnObj.AddComponent<LayoutElement>();
        btnLayout.preferredHeight = 80;

        // Texto
        GameObject textObj = CreateText(btnObj, "Text - TMP", label, 40);
        return btnObj;
    }

    private GameObject CreateText(GameObject parent, string name, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return textObj;
    }

    private void CreateBackground(GameObject canvasObj, Color color)
    {
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = color;
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        bgObj.transform.SetSiblingIndex(0);
    }

    private void CreateHealthBar(GameObject parent)
    {
        GameObject barObj = new GameObject("HealthBar");
        barObj.transform.SetParent(parent.transform, false);
        Image barImage = barObj.AddComponent<Image>();
        barImage.color = new Color(0.2f, 0.8f, 0.3f, 1f);
        RectTransform barRect = barObj.GetComponent<RectTransform>();
        barRect.sizeDelta = new Vector2(200, 30);
        
        healthBar = barImage;
    }

    // ===== SETUP EVENT LISTENERS =====
    private void SetupEventListeners()
    {
        // Main Menu
        playButton = transform.Find("MainMenuCanvas/PanelMenu/BtnPlay")?.GetComponent<Button>();
        settingsButton = transform.Find("MainMenuCanvas/PanelMenu/BtnSettings")?.GetComponent<Button>();
        shopButton = transform.Find("MainMenuCanvas/PanelMenu/BtnShop")?.GetComponent<Button>();
        quitButton = transform.Find("MainMenuCanvas/PanelMenu/BtnQuit")?.GetComponent<Button>();
        Button tutorialButton = transform.Find("MainMenuCanvas/PanelMenu/BtnTutorial")?.GetComponent<Button>();

        if (playButton != null) playButton.onClick.AddListener(OnPlayClick);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClick);
        if (shopButton != null) shopButton.onClick.AddListener(OnShopClick);
        if (tutorialButton != null) tutorialButton.onClick.AddListener(OnTutorialClick);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClick);

        // Pause Menu
        resumeButton = transform.Find("PauseMenuCanvas/PausePanel/BtnResume")?.GetComponent<Button>();
        pauseSettingsButton = transform.Find("PauseMenuCanvas/PausePanel/BtnPauseSettings")?.GetComponent<Button>();
        pauseQuitButton = transform.Find("PauseMenuCanvas/PausePanel/BtnPauseQuitToMenu")?.GetComponent<Button>();

        if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeClick);
        if (pauseSettingsButton != null) pauseSettingsButton.onClick.AddListener(OnSettingsClick);
        if (pauseQuitButton != null) pauseQuitButton.onClick.AddListener(OnQuitToMenuClick);

        // Settings
        volumeSlider = transform.Find("SettingsCanvas/SettingsPanel/SliderVolume")?.GetComponent<Slider>();
        vibrationToggle = transform.Find("SettingsCanvas/SettingsPanel/ToggleVibration")?.GetComponent<Toggle>();
        settingsBackButton = transform.Find("SettingsCanvas/SettingsPanel/BtnSettingsBack")?.GetComponent<Button>();

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        if (settingsBackButton != null) settingsBackButton.onClick.AddListener(OnSettingsBackClick);

        // Shop
        shopBackButton = transform.Find("ShopCanvas/ShopPanel/BtnShopBack")?.GetComponent<Button>();
        buyUpgrade1 = transform.Find("ShopCanvas/ShopPanel/Upgrade1/BuyUpgrade1")?.GetComponent<Button>();
        buyUpgrade2 = transform.Find("ShopCanvas/ShopPanel/Upgrade2/BuyUpgrade2")?.GetComponent<Button>();
        buyUpgrade3 = transform.Find("ShopCanvas/ShopPanel/Upgrade3/BuyUpgrade3")?.GetComponent<Button>();

        if (shopBackButton != null) shopBackButton.onClick.AddListener(OnShopBackClick);
        if (buyUpgrade1 != null) buyUpgrade1.onClick.AddListener(() => OnBuyUpgrade(1, 50));
        if (buyUpgrade2 != null) buyUpgrade2.onClick.AddListener(() => OnBuyUpgrade(2, 75));
        if (buyUpgrade3 != null) buyUpgrade3.onClick.AddListener(() => OnBuyUpgrade(3, 60));

        // Game Over
        gameOverScoreText = transform.Find("GameOverCanvas/GameOverPanel/TxtGameOverScore")?.GetComponent<TextMeshProUGUI>();
        gameOverHighscoreText = transform.Find("GameOverCanvas/GameOverPanel/TxtGameOverHighscore")?.GetComponent<TextMeshProUGUI>();
        restartButton = transform.Find("GameOverCanvas/GameOverPanel/BtnRestart")?.GetComponent<Button>();
        menuButton = transform.Find("GameOverCanvas/GameOverPanel/BtnGameOverMenu")?.GetComponent<Button>();

        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClick);
        if (menuButton != null) menuButton.onClick.AddListener(OnQuitToMenuClick);

        // HUD
        healthText = transform.Find("HUDCanvas/HUDGrid/TxtHealth")?.GetComponent<TextMeshProUGUI>();
        scoreText = transform.Find("HUDCanvas/HUDGrid/TxtScore")?.GetComponent<TextMeshProUGUI>();
        highscoreText = transform.Find("HUDCanvas/HUDGrid/TxtHighscore")?.GetComponent<TextMeshProUGUI>();
        survivalTimeText = transform.Find("HUDCanvas/HUDGrid/TxtSurvivalTime")?.GetComponent<TextMeshProUGUI>();
        waveText = transform.Find("HUDCanvas/HUDGrid/TxtWave")?.GetComponent<TextMeshProUGUI>();
        coinsText = transform.Find("HUDCanvas/HUDGrid/TxtCoins")?.GetComponent<TextMeshProUGUI>();
        batteryText = transform.Find("HUDCanvas/HUDGrid/TxtBattery")?.GetComponent<TextMeshProUGUI>();
        healthBar = transform.Find("HUDCanvas/HUDGrid/HealthBar")?.GetComponent<Image>();
        pauseInGameButton = transform.Find("HUDCanvas/BtnPauseInGame")?.GetComponent<Button>();

        if (pauseInGameButton != null) pauseInGameButton.onClick.AddListener(TogglePause);
    }

    // ===== MENU NAVIGATION =====
    public void ShowMainMenu()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        pauseMenuCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(false);
        shopCanvas.gameObject.SetActive(false);
        tutorialCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(false);
        hudCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
        isInGameplay = false;
        isPaused = false;
        survivalTime = 0f;
    }

    public void ShowPauseMenu()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        pauseMenuCanvas.gameObject.SetActive(true);
        settingsCanvas.gameObject.SetActive(false);
        shopCanvas.gameObject.SetActive(false);
        tutorialCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(false);
        hudCanvas.gameObject.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ShowGameplay()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        pauseMenuCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(false);
        shopCanvas.gameObject.SetActive(false);
        tutorialCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(false);
        hudCanvas.gameObject.SetActive(true);
        Time.timeScale = 1f;
        isInGameplay = true;
        isPaused = false;
        survivalTime = 0f;
    }

    public void ShowGameOver(int score, int highscore)
    {
        mainMenuCanvas.gameObject.SetActive(false);
        pauseMenuCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(false);
        shopCanvas.gameObject.SetActive(false);
        tutorialCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(true);
        hudCanvas.gameObject.SetActive(false);
        
        if (gameOverScoreText != null) gameOverScoreText.text = $"Score: {score}";
        if (gameOverHighscoreText != null) gameOverHighscoreText.text = $"Highscore: {highscore}";
        
        Time.timeScale = 0f;
        isInGameplay = false;
        isPaused = false;
    }

    public void ShowSettings()
    {
        settingsCanvas.gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        settingsCanvas.gameObject.SetActive(false);
    }

    public void ShowShop()
    {
        shopCanvas.gameObject.SetActive(true);
        UpdateShopUI();
    }

    public void HideShop()
    {
        shopCanvas.gameObject.SetActive(false);
    }

    // ===== BUTTON CALLBACKS =====
    private void OnPlayClick()
    {
        ShowGameplay();
        if (GameManager.Instance != null)
            GameManager.Instance.StartGameplay();
    }

    private void OnResumeClick()
    {
        HideSettings();
        pauseMenuCanvas.gameObject.SetActive(false);
        hudCanvas.gameObject.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    private void OnSettingsClick()
    {
        ShowSettings();
    }

    private void OnShopClick()
    {
        ShowShop();
    }

    private void OnTutorialClick()
    {
        ShowTutorial();
    }

    private void OnQuitClick()
    {
        Debug.Log("[MainMenuUIBuilder] Encerrando aplicação...");
        Time.timeScale = 1f; // Garantir que o tempo está normal
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnSettingsBackClick()
    {
        HideSettings();
        if (isInGameplay && isPaused)
        {
            pauseMenuCanvas.gameObject.SetActive(true);
        }
    }

    private void OnShopBackClick()
    {
        HideShop();
    }

    private void OnBuyUpgrade(int upgradeId, int cost)
    {
        if (UpgradeManager.Instance != null)
        {
            bool success = UpgradeManager.Instance.TryPurchaseShopUpgrade(upgradeId, cost);
            if (success)
            {
                Debug.Log($"[MainMenuUIBuilder] ✅ Upgrade {upgradeId} comprado!");
                // Atualizar descrição do upgrade na UI
                UpdateShopUI();
            }
        }
    }

    /// <summary>
    /// Atualiza descrições dos upgrades na Loja
    /// </summary>
    private void UpdateShopUI()
    {
        if (UpgradeManager.Instance == null) return;

        // Atualizar descrição de cada upgrade
        for (int i = 1; i <= 3; i++)
        {
            TextMeshProUGUI descText = transform.Find($"ShopCanvas/ShopPanel/Upgrade{i}/Description/Desc")?.GetComponent<TextMeshProUGUI>();
            if (descText != null)
            {
                descText.text = UpgradeManager.Instance.GetShopUpgradeDescription(i);
            }

            // Desabilitar botão se máximo já foi atingido
            Button buyBtn = transform.Find($"ShopCanvas/ShopPanel/Upgrade{i}/BuyUpgrade{i}")?.GetComponent<Button>();
            if (buyBtn != null)
            {
                buyBtn.interactable = UpgradeManager.Instance.CanPurchaseShopUpgrade(i);
            }
        }
    }

    private void OnRestartClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnQuitToMenuClick()
    {
        Time.timeScale = 1f;
        ShowMainMenu();
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMenu();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            OnResumeClick();
        }
        else if (isInGameplay && !gameOverCanvas.gameObject.activeInHierarchy)
        {
            ShowPauseMenu();
        }
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    // ===== HUD UPDATES =====
    private void UpdateHUDValues()
    {
        // Incrementar tempo de sobrevivência
        survivalTime += Time.deltaTime;

        // Atualizar Health
        HealthSystem healthSystem = FindAnyObjectByType<HealthSystem>();
        if (healthSystem != null && healthText != null)
        {
            int current = healthSystem.CurrentHealth;
            int max = healthSystem.maxHealth;
            healthText.text = $"Health: {current}/{max}";

            // Atualizar health bar com cores
            if (healthBar != null)
            {
                float fillAmount = (float)current / max;
                healthBar.fillAmount = fillAmount;

                if (fillAmount > 0.5f)
                    healthBar.color = new Color(0.2f, 0.8f, 0.3f, 1f); // Verde
                else if (fillAmount > 0.25f)
                    healthBar.color = new Color(1f, 1f, 0f, 1f); // Amarelo
                else
                    healthBar.color = new Color(1f, 0.2f, 0.2f, 1f); // Vermelho
            }
        }

        // Atualizar Score
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            if (scoreText != null) scoreText.text = $"Score: {scoreManager.Score}";
            if (highscoreText != null) highscoreText.text = $"Highscore: {scoreManager.Highscore}";
        }

        // Atualizar Tempo de Sobrevivência
        if (survivalTimeText != null)
        {
            int minutes = (int)(survivalTime / 60);
            int seconds = (int)(survivalTime % 60);
            survivalTimeText.text = $"Time: {minutes:D2}:{seconds:D2}";
        }

        // Atualizar Wave
        SpawnManager spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager != null && waveText != null)
        {
            waveText.text = $"Wave: {spawnManager.CurrentDifficultyStage + 1}";
        }

        // Atualizar Coins
        if (scoreManager != null && coinsText != null)
        {
            coinsText.text = $"Coins: {scoreManager.Coins}";
        }

        // Atualizar Battery
        BatterySystem batterySystem = FindAnyObjectByType<BatterySystem>();
        if (batterySystem != null && batteryText != null)
        {
            batteryText.text = $"Battery: {(int)(batterySystem.CurrentBattery * 100)}%";
        }
    }

    // ===== GAME OVER DETECTION =====
    private void DetectGameOver()
    {
        if (!isInGameplay || !hudCanvas.gameObject.activeInHierarchy)
            return;

        HealthSystem healthSystem = FindAnyObjectByType<HealthSystem>();
        if (healthSystem != null && healthSystem.CurrentHealth <= 0)
        {
            ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
            int score = scoreManager != null ? scoreManager.Score : 0;
            int highscore = scoreManager != null ? scoreManager.Highscore : 0;

            ShowGameOver(score, highscore);
            
            if (GameManager.Instance != null)
                GameManager.Instance.TriggerGameOver();
        }
    }
}
