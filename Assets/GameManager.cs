using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static System.Action onGameOver;
    public static System.Action<bool> onPauseChanged;
    public static System.Action<bool> onMainMenuChanged;

    private static bool startInMenuOnNextLoad = true;

    [SerializeField] private bool startInMainMenu = true;

    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsInMainMenu { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ativar brilho do jogo
        BoostBrightness();

        if (Application.isMobilePlatform)
        {
            // Mobile runtime defaults: keep vSync off and cap to a stable frame target.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        bool shouldStartInMenu = startInMenuOnNextLoad && startInMainMenu;
        startInMenuOnNextLoad = true;

        if (shouldStartInMenu)
        {
            EnterMainMenuState();
        }
        else
        {
            StartGameplayState();
        }
    }

    private void BoostBrightness()
    {
        // Aumentar luz ambiente
        RenderSettings.ambientLight = new Color(0.6f, 0.6f, 0.6f, 1f) * 1.5f;
        
        // Definir cor de fundo da câmera como cinza claro
        if (Camera.main != null)
        {
            Camera.main.backgroundColor = new Color(0.15f, 0.15f, 0.2f, 1f);
        }
    }
}
