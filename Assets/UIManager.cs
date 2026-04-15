using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Gerenciador central de UI do jogo.
/// Controla Menu Principal, Pause, Settings e Game Over.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas mainMenuCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas settingsCanvas;
    public Canvas gameOverCanvas;
    public Canvas hudCanvas;

    [Header("Main Menu")]
    public Button playButton;
    public Button settingsButton;
    public Button shopButton;
    public Button quitButton;
    public TMP_Text titleText;

    [Header("Pause Menu")]
    public Button resumeButton;
    public Button pauseSettingsButton;
    public Button pauseQuitButton;
    public TMP_Text pauseTitleText;

    [Header("Settings")]
    public Slider volumeSlider;
    public Toggle vibrationToggle;
    public Button settingsBackButton;
    public TMP_Text volumeLabel;

    [Header("Game Over")]
    public TMP_Text gameOverScoreText;
    public TMP_Text gameOverHighscoreText;
    public Button restartButton;
    public Button menuButton;

    private static UIManager instance;
    public static UIManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Encontrar canvases se não atribuídos
        if (mainMenuCanvas == null) mainMenuCanvas = FindCanvasByTag("MainMenu");
        if (pauseMenuCanvas == null) pauseMenuCanvas = FindCanvasByTag("PauseMenu");
        if (settingsCanvas == null) settingsCanvas = FindCanvasByTag("Settings");
        if (gameOverCanvas == null) gameOverCanvas = FindCanvasByTag("GameOver");
        if (hudCanvas == null) hudCanvas = FindCanvasByTag("HUD");
    }

    private void Start()
    {
        // Conectar botões
        if (playButton != null) playButton.onClick.AddListener(PlayGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (shopButton != null) shopButton.onClick.AddListener(OpenShop);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);

        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (pauseSettingsButton != null) pauseSettingsButton.onClick.AddListener(OpenSettings);
        if (pauseQuitButton != null) pauseQuitButton.onClick.AddListener(QuitToMenu);

        if (settingsBackButton != null) settingsBackButton.onClick.AddListener(CloseSettings);
        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(SetVolume);

        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (menuButton != null) menuButton.onClick.AddListener(QuitToMenu);

        // Estado inicial
        ShowMainMenu();
    }

    #region Menu Navigation

    public void ShowMainMenu()
    {
        SetCanvasActive(mainMenuCanvas, true);
        SetCanvasActive(pauseMenuCanvas, false);
        SetCanvasActive(settingsCanvas, false);
        SetCanvasActive(gameOverCanvas, false);
        SetCanvasActive(hudCanvas, false);
        Time.timeScale = 1f;
    }

    public void ShowPauseMenu()
    {
        SetCanvasActive(pauseMenuCanvas, true);
        SetCanvasActive(mainMenuCanvas, false);
        SetCanvasActive(hudCanvas, false);
        Time.timeScale = 0f;
    }

    public void ShowSettings()
    {
        SetCanvasActive(settingsCanvas, true);
    }

    public void HideSettings()
    {
        SetCanvasActive(settingsCanvas, false);
    }

    public void ShowGameOver(int score, int highscore)
    {
        SetCanvasActive(gameOverCanvas, true);
        SetCanvasActive(hudCanvas, false);
        if (gameOverScoreText != null) gameOverScoreText.text = $"Score: {score}";
        if (gameOverHighscoreText != null) gameOverHighscoreText.text = $"Recorde: {highscore}";
        Time.timeScale = 0f;
    }

    public void ShowGameplay()
    {
        SetCanvasActive(mainMenuCanvas, false);
        SetCanvasActive(pauseMenuCanvas, false);
        SetCanvasActive(settingsCanvas, false);
        SetCanvasActive(gameOverCanvas, false);
        SetCanvasActive(hudCanvas, true);
        Time.timeScale = 1f;
    }

    #endregion

    #region Button Callbacks

    private void PlayGame()
    {
        ShowGameplay();
        GameManager.Instance?.StartGameplay();
    }

    private void ResumeGame()
    {
        HideSettings();
        SetCanvasActive(pauseMenuCanvas, false);
        SetCanvasActive(hudCanvas, true);
        Time.timeScale = 1f;
    }

    private void OpenSettings()
    {
        ShowSettings();
    }

    private void CloseSettings()
    {
        HideSettings();
    }

    private void OpenShop()
    {
        Debug.Log("Abrir loja (não implementado)");
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    private void QuitToMenu()
    {
        Time.timeScale = 1f;
        ShowMainMenu();
        GameManager.Instance?.ReturnToMenu();
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;
        if (volumeLabel != null) volumeLabel.text = $"Volume: {(int)(value * 100)}%";
    }

    #endregion

    #region Helpers

    private Canvas FindCanvasByTag(string tag)
    {
        Canvas[] canvases = FindObjectsByType<Canvas>();
        foreach (Canvas c in canvases)
        {
            if (c.gameObject.name.Contains(tag))
                return c;
        }
        return null;
    }

    private void SetCanvasActive(Canvas canvas, bool active)
    {
        if (canvas != null)
            canvas.gameObject.SetActive(active);
    }

    #endregion
}
