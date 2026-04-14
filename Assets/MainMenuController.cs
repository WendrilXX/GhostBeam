using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject panelRoot;
    public GameObject shopPanelRoot;
    public GameObject settingsPanelRoot;
    public TMP_Text titleText;
    public TMP_Text startButtonLabel;
    public TMP_Text openShopButtonLabel;
    public TMP_Text openSettingsButtonLabel;
    public TMP_Text shopCoinsText;
    public TMP_Text masterVolumeLabel;
    public TMP_Text vibrationButtonLabel;
    public TMP_Text timerButtonLabel;
    public TMP_Text perfOverlayButtonLabel;
    public TMP_Text closeSettingsButtonLabel;
    public TMP_Text beamUpgradeStatusText;
    public TMP_Text powerUpgradeStatusText;
    public TMP_Text batteryUpgradeStatusText;
    public TMP_Text beamUpgradeButtonLabel;
    public TMP_Text powerUpgradeButtonLabel;
    public TMP_Text batteryUpgradeButtonLabel;
    public TMP_Text lunaSkinButtonLabel;
    public TMP_Text flashSkinButtonLabel;
    public TMP_Text closeShopButtonLabel;
    public TMP_Text quitButtonLabel;
    public Button beamUpgradeButton;
    public Button powerUpgradeButton;
    public Button batteryUpgradeButton;
    public Button lunaSkinButton;
    public Button flashSkinButton;
    public Slider masterVolumeSlider;
    
    // Novos Controllers
    public ShopScreenController shopController;
    public LeaderboardScreenController leaderboardController;
    public DailyQuestsScreenController dailyQuestsController;

    private const string PanelObjectName = "PanelMenu";
    private const string ShopPanelObjectName = "PanelLoja";
    private const string SettingsPanelObjectName = "PanelConfiguracoes";
    private const string TitleObjectName = "TxtTitulo";
    private const string StartButtonObjectName = "BtnJogar";
    private const string OpenShopButtonObjectName = "BtnLoja";
    private const string OpenSettingsButtonObjectName = "BtnConfiguracoes";
    private const string ShopCoinsObjectName = "TxtMoedasLoja";
    private const string MasterVolumeLabelObjectName = "TxtVolumeMaster";
    private const string MasterVolumeSliderObjectName = "SliderVolumeMaster";
    private const string VibrationButtonObjectName = "BtnVibracao";
    private const string TimerButtonObjectName = "BtnTimerHUD";
    private const string PerfOverlayButtonObjectName = "BtnPerfHUD";
    private const string CloseSettingsButtonObjectName = "BtnVoltarConfig";
    private const string BeamStatusObjectName = "TxtStatusFeixe";
    private const string PowerStatusObjectName = "TxtStatusPoder";
    private const string BatteryStatusObjectName = "TxtStatusBateria";
    private const string BeamUpgradeButtonObjectName = "BtnUpgradeFeixe";
    private const string PowerUpgradeButtonObjectName = "BtnUpgradePoder";
    private const string BatteryUpgradeButtonObjectName = "BtnUpgradeBateria";
    private const string LunaSkinButtonObjectName = "BtnSkinLuna";
    private const string FlashSkinButtonObjectName = "BtnSkinLanterna";
    private const string CloseShopButtonObjectName = "BtnVoltarLoja";
    private const string QuitButtonObjectName = "BtnSair";

    private bool isMainMenuVisible;
    private bool isShopOpen;
    private bool isSettingsOpen;

    private void Awake()
    {
        AutoAssignIfMissing();
        
        // Auto-atribuir Controllers se não forem atribuídos
        if (shopController == null)
        {
            shopController = GetComponentInChildren<ShopScreenController>(true);
        }
        if (leaderboardController == null)
        {
            leaderboardController = GetComponentInChildren<LeaderboardScreenController>(true);
        }
        if (dailyQuestsController == null)
        {
            dailyQuestsController = GetComponentInChildren<DailyQuestsScreenController>(true);
        }
    }

    private void OnEnable()
    {
        GameManager.onMainMenuChanged += SetVisible;
        ScoreManager.onCoinsChanged += HandleCoinsChanged;
        UpgradeManager.onUpgradesChanged += RefreshUpgradeLabels;
        SkinManager.onSkinsChanged += RefreshUpgradeLabels;
        SettingsManager.onSettingsChanged += RefreshSettingsUI;
        SetVisible(GameManager.Instance != null && GameManager.Instance.IsInMainMenu);
        RefreshUpgradeLabels();
        RefreshSettingsUI();
    }

    private void OnDisable()
    {
        GameManager.onMainMenuChanged -= SetVisible;
        ScoreManager.onCoinsChanged -= HandleCoinsChanged;
        UpgradeManager.onUpgradesChanged -= RefreshUpgradeLabels;
        SkinManager.onSkinsChanged -= RefreshUpgradeLabels;
        SettingsManager.onSettingsChanged -= RefreshSettingsUI;
    }

    public void StartGame()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        isShopOpen = false;
        isSettingsOpen = false;
        UpdatePanels();
        GameManager.Instance.StartGameplayFromMenu();
    }

    public void OpenShop()
    {
        Debug.Log($"[MainMenu] OpenShop() called - isMainMenuVisible={isMainMenuVisible}, shopPanelRoot={shopPanelRoot}");
        
        if (!isMainMenuVisible)
        {
            Debug.LogWarning("[MainMenu] OpenShop() failed - not in main menu visible state");
            return;
        }

        isShopOpen = true;
        isSettingsOpen = false;
        UpdatePanels();
        RefreshUpgradeLabels();
        Debug.Log("[MainMenu] Shop opened successfully");
    }

    public void CloseShop()
    {
        isShopOpen = false;
        UpdatePanels();
    }

    public void OpenSettings()
    {
        if (!isMainMenuVisible)
        {
            return;
        }

        isSettingsOpen = true;
        isShopOpen = false;
        UpdatePanels();
        RefreshSettingsUI();
    }

    public void CloseSettings()
    {
        isSettingsOpen = false;
        UpdatePanels();
    }

    public void OpenLeaderboard()
    {
        if (!isMainMenuVisible)
        {
            return;
        }

        isSettingsOpen = false;
        isShopOpen = false;
        UpdatePanels();

        if (leaderboardController != null)
        {
            leaderboardController.SetVisible(true);
        }
    }

    public void CloseLeaderboard()
    {
        UpdatePanels();
        
        if (leaderboardController != null)
        {
            leaderboardController.SetVisible(false);
        }
    }

    public void OpenDailyQuests()
    {
        if (!isMainMenuVisible)
        {
            return;
        }

        isSettingsOpen = false;
        isShopOpen = false;
        UpdatePanels();

        if (dailyQuestsController != null)
        {
            dailyQuestsController.SetVisible(true);
        }
    }

    public void CloseDailyQuests()
    {
        UpdatePanels();
        
        if (dailyQuestsController != null)
        {
            dailyQuestsController.SetVisible(false);
        }
    }

    public void SetMasterVolume(float value)
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

        SettingsManager.Instance.SetMasterVolume(value);
        RefreshSettingsUI();
    }

    public void ToggleVibration()
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

        SettingsManager.Instance.ToggleVibration();
        RefreshSettingsUI();
    }

    public void ToggleHudTimer()
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

        SettingsManager.Instance.ToggleHudTimer();
        RefreshSettingsUI();
    }

    public void TogglePerfOverlay()
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

        SettingsManager.Instance.TogglePerfOverlay();
        RefreshSettingsUI();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void BuyBeamUpgrade()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        UpgradeManager.Instance.TryPurchaseBeamUpgrade();
        RefreshUpgradeLabels();
    }

    public void BuyPowerUpgrade()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        UpgradeManager.Instance.TryPurchasePowerUpgrade();
        RefreshUpgradeLabels();
    }

    public void BuyBatteryUpgrade()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        UpgradeManager.Instance.TryPurchaseBatteryUpgrade();
        RefreshUpgradeLabels();
    }

    public void BuyOrEquipLunaSkin()
    {
        // Funçao de compra de skin removida
        return;
    }

    public void BuyOrEquipFlashSkin()
    {
        // Funçao de compra de skin removida
        return;
    }

    private void SetVisible(bool visible)
    {
        Debug.Log($"[MainMenu] SetVisible({visible}) - panelRoot={panelRoot}, shopPanelRoot={shopPanelRoot}");
        isMainMenuVisible = visible;
        if (!visible)
        {
            isShopOpen = false;
            isSettingsOpen = false;
        }

        UpdatePanels();
    }

    private void AutoAssignIfMissing()
    {
        if (panelRoot == null)
        {
            Transform panelTransform = FindChildByName(transform, PanelObjectName);
            if (panelTransform != null)
            {
                panelRoot = panelTransform.gameObject;
            }
        }

        if (shopPanelRoot == null)
        {
            Transform shopPanelTransform = FindChildByName(transform, ShopPanelObjectName);
            if (shopPanelTransform != null)
            {
                shopPanelRoot = shopPanelTransform.gameObject;
            }
        }

        if (settingsPanelRoot == null)
        {
            Transform settingsPanelTransform = FindChildByName(transform, SettingsPanelObjectName);
            if (settingsPanelTransform != null)
            {
                settingsPanelRoot = settingsPanelTransform.gameObject;
            }
        }

        if (titleText == null)
        {
            titleText = FindTMPByName(TitleObjectName);
        }

        if (startButtonLabel == null)
        {
            Transform startButton = FindChildByName(transform, StartButtonObjectName);
            if (startButton != null)
            {
                startButtonLabel = startButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (openShopButtonLabel == null)
        {
            Transform openShopButton = FindChildByName(transform, OpenShopButtonObjectName);
            if (openShopButton != null)
            {
                openShopButtonLabel = openShopButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (openSettingsButtonLabel == null)
        {
            Transform openSettingsButton = FindChildByName(transform, OpenSettingsButtonObjectName);
            if (openSettingsButton != null)
            {
                openSettingsButtonLabel = openSettingsButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (shopCoinsText == null)
        {
            shopCoinsText = FindTMPByName(ShopCoinsObjectName);
        }

        if (masterVolumeLabel == null)
        {
            masterVolumeLabel = FindTMPByName(MasterVolumeLabelObjectName);
        }

        if (masterVolumeSlider == null)
        {
            Transform volumeSliderTransform = FindChildByName(transform, MasterVolumeSliderObjectName);
            if (volumeSliderTransform != null)
            {
                masterVolumeSlider = volumeSliderTransform.GetComponent<Slider>();
            }
        }

        if (vibrationButtonLabel == null)
        {
            Transform vibrationButton = FindChildByName(transform, VibrationButtonObjectName);
            if (vibrationButton != null)
            {
                vibrationButtonLabel = vibrationButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (timerButtonLabel == null)
        {
            Transform timerButton = FindChildByName(transform, TimerButtonObjectName);
            if (timerButton != null)
            {
                timerButtonLabel = timerButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (perfOverlayButtonLabel == null)
        {
            Transform perfOverlayButton = FindChildByName(transform, PerfOverlayButtonObjectName);
            if (perfOverlayButton != null)
            {
                perfOverlayButtonLabel = perfOverlayButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (closeSettingsButtonLabel == null)
        {
            Transform closeSettingsButton = FindChildByName(transform, CloseSettingsButtonObjectName);
            if (closeSettingsButton != null)
            {
                closeSettingsButtonLabel = closeSettingsButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (beamUpgradeStatusText == null)
        {
            beamUpgradeStatusText = FindTMPByName(BeamStatusObjectName);
        }

        if (powerUpgradeStatusText == null)
        {
            powerUpgradeStatusText = FindTMPByName(PowerStatusObjectName);
        }

        if (batteryUpgradeStatusText == null)
        {
            batteryUpgradeStatusText = FindTMPByName(BatteryStatusObjectName);
        }

        if (beamUpgradeButtonLabel == null)
        {
            Transform beamUpgradeButton = FindChildByName(transform, BeamUpgradeButtonObjectName);
            if (beamUpgradeButton != null)
            {
                beamUpgradeButtonLabel = beamUpgradeButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (powerUpgradeButtonLabel == null)
        {
            Transform powerUpgradeButton = FindChildByName(transform, PowerUpgradeButtonObjectName);
            if (powerUpgradeButton != null)
            {
                powerUpgradeButtonLabel = powerUpgradeButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (batteryUpgradeButtonLabel == null)
        {
            Transform batteryUpgradeButtonTransform = FindChildByName(transform, BatteryUpgradeButtonObjectName);
            if (batteryUpgradeButtonTransform != null)
            {
                batteryUpgradeButtonLabel = batteryUpgradeButtonTransform.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (lunaSkinButtonLabel == null)
        {
            Transform lunaSkinButtonTransform = FindChildByName(transform, LunaSkinButtonObjectName);
            if (lunaSkinButtonTransform != null)
            {
                lunaSkinButtonLabel = lunaSkinButtonTransform.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (flashSkinButtonLabel == null)
        {
            Transform flashSkinButtonTransform = FindChildByName(transform, FlashSkinButtonObjectName);
            if (flashSkinButtonTransform != null)
            {
                flashSkinButtonLabel = flashSkinButtonTransform.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (closeShopButtonLabel == null)
        {
            Transform closeShopButton = FindChildByName(transform, CloseShopButtonObjectName);
            if (closeShopButton != null)
            {
                closeShopButtonLabel = closeShopButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        if (quitButtonLabel == null)
        {
            Transform quitButton = FindChildByName(transform, QuitButtonObjectName);
            if (quitButton != null)
            {
                quitButtonLabel = quitButton.GetComponentInChildren<TMP_Text>(true);
            }
        }
    }

    private TMP_Text FindTMPByName(string objectName)
    {
        Transform target = FindChildByName(transform, objectName);
        if (target == null)
        {
            return null;
        }

        TMP_Text tmp = target.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            return tmp;
        }

        return target.GetComponentInChildren<TMP_Text>(true);
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

    private void HandleCoinsChanged(int _)
    {
        RefreshUpgradeLabels();
    }

    private void RefreshUpgradeLabels()
    {
        int coins = ScoreManager.Instance != null ? ScoreManager.Instance.Coins : 0;
        if (shopCoinsText != null)
        {
            shopCoinsText.text = $"<color=#FFD700>MOEDAS: {coins}</color>";
            shopCoinsText.fontSize = 32;
        }

        if (UpgradeManager.Instance != null)
        {
            int beamCost = UpgradeManager.Instance.GetBeamUpgradeCost();
            int powerCost = UpgradeManager.Instance.GetPowerUpgradeCost();
            int batteryCost = UpgradeManager.Instance.GetBatteryUpgradeCost();
            bool isBeamMax = beamCost < 0;
            bool isPowerMax = powerCost < 0;
            bool isBatteryMax = batteryCost < 0;
            bool canBuyBeam = beamCost >= 0 && coins >= beamCost;
            bool canBuyPower = powerCost >= 0 && coins >= powerCost;
            bool canBuyBattery = batteryCost >= 0 && coins >= batteryCost;

            if (beamUpgradeButtonLabel != null)
            {
                beamUpgradeButtonLabel.text = isBeamMax
                    ? "UPGRADE FEIXE\n(MAXIMO)"
                    : "UPGRADE FEIXE\nNIVEL " + (UpgradeManager.Instance.BeamLevel + 1) + "\nCUSTO: " + beamCost;
                beamUpgradeButtonLabel.color = canBuyBeam || isBeamMax ? Color.white : new Color(1, 0.5f, 0.5f);
                beamUpgradeButtonLabel.fontSize = 20;
            }

            if (powerUpgradeButtonLabel != null)
            {
                powerUpgradeButtonLabel.text = isPowerMax
                    ? "UPGRADE PODER\n(MAXIMO)"
                    : "UPGRADE PODER\nNIVEL " + (UpgradeManager.Instance.PowerLevel + 1) + "\nCUSTO: " + powerCost;
                powerUpgradeButtonLabel.color = canBuyPower || isPowerMax ? Color.white : new Color(1, 0.5f, 0.5f);
                powerUpgradeButtonLabel.fontSize = 20;
            }

            if (batteryUpgradeButtonLabel != null)
            {
                batteryUpgradeButtonLabel.text = isBatteryMax
                    ? "UPGRADE BATERIA\n(MAXIMO)"
                    : "UPGRADE BATERIA\nNIVEL " + (UpgradeManager.Instance.BatteryLevel + 1) + "\nCUSTO: " + batteryCost;
                batteryUpgradeButtonLabel.color = canBuyBattery || isBatteryMax ? Color.white : new Color(1, 0.5f, 0.5f);
                batteryUpgradeButtonLabel.fontSize = 20;
            }

            if (beamUpgradeStatusText != null)
            {
                beamUpgradeStatusText.text = string.Empty;
                beamUpgradeStatusText.gameObject.SetActive(false);
            }

            if (powerUpgradeStatusText != null)
            {
                powerUpgradeStatusText.text = string.Empty;
                powerUpgradeStatusText.gameObject.SetActive(false);
            }

            if (batteryUpgradeStatusText != null)
            {
                batteryUpgradeStatusText.text = string.Empty;
                batteryUpgradeStatusText.gameObject.SetActive(false);
            }

            if (closeShopButtonLabel != null)
            {
                closeShopButtonLabel.text = "VOLTAR MENU";
            }

            if (beamUpgradeButton != null)
            {
                beamUpgradeButton.interactable = !isBeamMax && canBuyBeam;
            }

            if (powerUpgradeButton != null)
            {
                powerUpgradeButton.interactable = !isPowerMax && canBuyPower;
            }

            if (batteryUpgradeButton != null)
            {
                batteryUpgradeButton.interactable = !isBatteryMax && canBuyBattery;
            }

            // Skins removidas da loja - botões desabilitados
            if (lunaSkinButton != null)
            {
                lunaSkinButton.interactable = false;
                lunaSkinButton.gameObject.SetActive(false);
            }

            if (flashSkinButton != null)
            {
                flashSkinButton.interactable = false;
                flashSkinButton.gameObject.SetActive(false);
            }

            return;
        }

        if (beamUpgradeButtonLabel != null)
        {
            beamUpgradeButtonLabel.text = "UP FEIXE L1";
        }

        if (beamUpgradeStatusText != null)
        {
            beamUpgradeStatusText.text = string.Empty;
            beamUpgradeStatusText.gameObject.SetActive(false);
        }

        if (powerUpgradeButtonLabel != null)
        {
            powerUpgradeButtonLabel.text = "UP PODER L1";
        }

        if (lunaSkinButtonLabel != null)
        {
            lunaSkinButtonLabel.text = "SKIN LUNA";
        }

        if (batteryUpgradeButtonLabel != null)
        {
            batteryUpgradeButtonLabel.text = "UP BATERIA L1";
        }

        if (flashSkinButtonLabel != null)
        {
            flashSkinButtonLabel.text = "SKIN LANTERNA";
        }

        if (powerUpgradeStatusText != null)
        {
            powerUpgradeStatusText.text = string.Empty;
            powerUpgradeStatusText.gameObject.SetActive(false);
        }

        if (batteryUpgradeStatusText != null)
        {
            batteryUpgradeStatusText.text = string.Empty;
            batteryUpgradeStatusText.gameObject.SetActive(false);
        }

        if (closeShopButtonLabel != null)
        {
            closeShopButtonLabel.text = "VOLTAR MENU";
        }
    }

    private void UpdatePanels()
    {
        Debug.Log($"[MainMenu] UpdatePanels() - isMainMenuVisible={isMainMenuVisible}, isShopOpen={isShopOpen}, isSettingsOpen={isSettingsOpen}");
        
        if (panelRoot != null)
        {
            bool mainActive = isMainMenuVisible && !isShopOpen && !isSettingsOpen;
            panelRoot.SetActive(mainActive);
            Debug.Log($"  panelRoot -> {mainActive}");
        }
        else
        {
            Debug.LogWarning("  panelRoot is NULL!");
        }

        if (shopPanelRoot != null)
        {
            bool shopActive = isMainMenuVisible && isShopOpen;
            shopPanelRoot.SetActive(shopActive);
            Debug.Log($"  shopPanelRoot -> {shopActive}");
        }
        else
        {
            Debug.LogWarning("  shopPanelRoot is NULL!");
        }

        if (settingsPanelRoot != null)
        {
            bool settingsActive = isMainMenuVisible && isSettingsOpen;
            settingsPanelRoot.SetActive(settingsActive);
            Debug.Log($"  settingsPanelRoot -> {settingsActive}");
        }
        else
        {
            Debug.LogWarning("  settingsPanelRoot is NULL!");
        }
    }

    private void RefreshSettingsUI()
    {
        if (openSettingsButtonLabel != null)
        {
            openSettingsButtonLabel.text = "CONFIGURAÇÕES";
            openSettingsButtonLabel.fontSize = 28;
        }

        if (closeSettingsButtonLabel != null)
        {
            closeSettingsButtonLabel.text = "VOLTAR";
            closeSettingsButtonLabel.fontSize = 26;
        }

        if (SettingsManager.Instance == null)
        {
            return;
        }

        if (masterVolumeLabel != null)
        {
            int percent = Mathf.RoundToInt(SettingsManager.Instance.MasterVolume * 100f);
            masterVolumeLabel.text = "🔊 VOLUME MASTER: " + percent + "%";
            masterVolumeLabel.fontSize = 28;
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(SettingsManager.Instance.MasterVolume);
        }

        if (vibrationButtonLabel != null)
        {
            string vibState = SettingsManager.Instance.VibrationEnabled ? "✓ ON" : "✗ OFF";
            vibrationButtonLabel.text = $"📳 VIBRAÇÃO: {vibState}";
            vibrationButtonLabel.fontSize = 26;
        }

        if (timerButtonLabel != null)
        {
            string timerState = SettingsManager.Instance.ShowHudTimer ? "✓ ON" : "✗ OFF";
            timerButtonLabel.text = $"⏱️ TIMER: {timerState}";
            timerButtonLabel.fontSize = 26;
        }

        if (perfOverlayButtonLabel != null)
        {
            string perfState = SettingsManager.Instance.ShowPerfOverlay ? "✓ ON" : "✗ OFF";
            perfOverlayButtonLabel.text = $"📊 PERFORMANCE: {perfState}";
            perfOverlayButtonLabel.fontSize = 26;
        }
    }
}
