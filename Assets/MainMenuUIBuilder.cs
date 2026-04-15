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
    private Canvas gameOverCanvas;
    private Canvas hudCanvas;

    [Header("Button References")]
    private Button playButton, settingsButton, shopButton, quitButton;
    private Button resumeButton, pauseSettingsButton, pauseQuitButton;
    private Button settingsBackButton, restartButton, menuButton;

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
        // ===== CRIAR TODOS OS 5 CANVASES =====
        mainMenuCanvas = CreateCanvas("MainMenuCanvas", true).GetComponent<Canvas>();
        pauseMenuCanvas = CreateCanvas("PauseMenuCanvas", false).GetComponent<Canvas>();
        settingsCanvas = CreateCanvas("SettingsCanvas", false).GetComponent<Canvas>();
        gameOverCanvas = CreateCanvas("GameOverCanvas", false).GetComponent<Canvas>();
        hudCanvas = CreateCanvas("HUDCanvas", false).GetComponent<Canvas>();

        // ===== MAIN MENU =====
        CreateMainMenuUI(mainMenuCanvas.gameObject);

        // ===== PAUSE MENU =====
        CreatePauseMenuUI(pauseMenuCanvas.gameObject);

        // ===== SETTINGS =====
        CreateSettingsUI(settingsCanvas.gameObject);

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
        CreateMenuButton(panelObj, "BtnSettings", "CONFIGURAÇÃO", new Color(0.3f, 0.5f, 0.8f, 1f), null);
        CreateMenuButton(panelObj, "BtnQuit", "QUIT", new Color(0.8f, 0.2f, 0.2f, 1f), null);
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
        panelRect.sizeDelta = new Vector2(600, 600);

        VerticalLayoutGroup layoutGroup = panelObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 30;
        layoutGroup.padding = new RectOffset(60, 60, 60, 60);
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Título
        GameObject titleObj = CreateText(panelObj, "TxtSettingsTitle", "CONFIGURAÇÃO", 60);
        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 100;

        // Volume Label
        CreateText(panelObj, "TxtVolumeLabel", "VOLUME", 30);

        // Volume Slider
        GameObject sliderObj = new GameObject("SliderVolume");
        sliderObj.transform.SetParent(panelObj.transform, false);
        Slider volumeSlider = sliderObj.AddComponent<Slider>();
        volumeSlider.minValue = 0;
        volumeSlider.maxValue = 1;
        volumeSlider.value = AudioListener.volume;
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(500, 50);
        LayoutElement sliderLayout = sliderObj.AddComponent<LayoutElement>();
        sliderLayout.preferredHeight = 50;

        // Vibration Label
        CreateText(panelObj, "TxtVibrationLabel", "VIBRAÇÃO", 30);

        // Vibration Toggle
        GameObject toggleObj = new GameObject("ToggleVibration");
        toggleObj.transform.SetParent(panelObj.transform, false);
        Toggle vibrationToggle = toggleObj.AddComponent<Toggle>();
        vibrationToggle.isOn = true;
        RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
        toggleRect.sizeDelta = new Vector2(500, 60);
        LayoutElement toggleLayout = toggleObj.AddComponent<LayoutElement>();
        toggleLayout.preferredHeight = 60;

        // Botão Voltar - callback adicionado em SetupEventListeners()
        CreateMenuButton(panelObj, "BtnSettingsBack", "BACK", new Color(0.5f, 0.5f, 0.5f, 1f), null);
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

        if (playButton != null) playButton.onClick.AddListener(OnPlayClick);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClick);
        if (shopButton != null) shopButton.onClick.AddListener(() => Debug.Log("Shop não implementado"));
        if (quitButton != null) quitButton.onClick.AddListener(() => Application.Quit());

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

    private void OnSettingsBackClick()
    {
        HideSettings();
        if (isInGameplay && isPaused)
        {
            pauseMenuCanvas.gameObject.SetActive(true);
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
