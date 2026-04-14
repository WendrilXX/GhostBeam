using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIBootstrapper
{
    private void BindHud(GameObject hudCanvasObj)
    {
        HUDController hudController = hudCanvasObj.GetComponent<HUDController>();
        if (hudController == null)
        {
            return;
        }

        hudController.healthText = FindTMP(hudCanvasObj.transform, healthName);
        hudController.scoreText = FindTMP(hudCanvasObj.transform, scoreName);
        hudController.highscoreText = FindTMP(hudCanvasObj.transform, highscoreName);
        hudController.batteryText = FindTMP(hudCanvasObj.transform, batteryName);
        hudController.coinsText = FindTMP(hudCanvasObj.transform, coinsName);
        hudController.survivalTimeText = FindTMP(hudCanvasObj.transform, survivalTimeName);
        hudController.stageText = FindTMP(hudCanvasObj.transform, stageName);
        hudController.performanceText = FindTMP(hudCanvasObj.transform, performanceTextName);

        PerformanceOverlay perfOverlay = hudCanvasObj.GetComponent<PerformanceOverlay>();
        if (perfOverlay == null)
        {
            perfOverlay = hudCanvasObj.AddComponent<PerformanceOverlay>();
        }

        perfOverlay.overlayText = hudController.performanceText;

        Transform pauseTransform = FindChildByName(hudCanvasObj.transform, pauseButtonName);
        if (pauseTransform == null)
        {
            return;
        }

        Button pauseButton = pauseTransform.GetComponent<Button>();
        if (pauseButton == null)
        {
            return;
        }

        PauseController pauseController = FindAnyObjectByType<PauseController>();
        if (pauseController == null)
        {
            pauseController = hudCanvasObj.AddComponent<PauseController>();
        }

        pauseController.pauseButtonLabel = pauseTransform.GetComponentInChildren<TMP_Text>(true);
        pauseButton.onClick.RemoveListener(pauseController.TogglePause);
        pauseButton.onClick.AddListener(pauseController.TogglePause);

        UIRuntimeVisibilityController visibilityController = FindAnyObjectByType<UIRuntimeVisibilityController>();
        if (visibilityController == null)
        {
            visibilityController = gameObject.AddComponent<UIRuntimeVisibilityController>();
        }

        visibilityController.hudCanvasRoot = hudCanvasObj;
    }

    private void BindGameOver(GameObject gameOverCanvasObj)
    {
        GameOverPanelController controller = FindAnyObjectByType<GameOverPanelController>();
        if (controller == null)
        {
            controller = gameOverCanvasObj.AddComponent<GameOverPanelController>();
        }

        Transform panel = FindChildByName(gameOverCanvasObj.transform, panelName);
        controller.panelRoot = panel != null ? panel.gameObject : null;
        controller.finalScoreText = FindTMP(gameOverCanvasObj.transform, finalScoreName);
        controller.highscoreText = FindTMP(gameOverCanvasObj.transform, finalHighscoreName);

        Transform buttonTransform = FindChildByName(gameOverCanvasObj.transform, restartButtonName);
        if (buttonTransform == null)
        {
            return;
        }

        Button restartButton = buttonTransform.GetComponent<Button>();
        if (restartButton == null)
        {
            return;
        }

        restartButton.onClick.RemoveListener(controller.RestartGame);
        restartButton.onClick.AddListener(controller.RestartGame);
    }

    private void BindMainMenu(GameObject menuCanvasObj)
    {
        MainMenuController controller = FindAnyObjectByType<MainMenuController>();
        if (controller == null)
        {
            controller = menuCanvasObj.AddComponent<MainMenuController>();
        }

        Transform panel = FindChildByName(menuCanvasObj.transform, menuPanelName);
        controller.panelRoot = panel != null ? panel.gameObject : null;
        controller.titleText = FindTMP(menuCanvasObj.transform, menuTitleName);
        Transform shopPanel = FindChildByName(menuCanvasObj.transform, shopPanelName);
        controller.shopPanelRoot = shopPanel != null ? shopPanel.gameObject : null;
        Transform settingsPanel = FindChildByName(menuCanvasObj.transform, settingsPanelName);
        controller.settingsPanelRoot = settingsPanel != null ? settingsPanel.gameObject : null;
        controller.shopCoinsText = FindTMP(menuCanvasObj.transform, shopCoinsName);
        controller.beamUpgradeStatusText = FindTMP(menuCanvasObj.transform, beamStatusName);
        controller.powerUpgradeStatusText = FindTMP(menuCanvasObj.transform, powerStatusName);
        controller.masterVolumeLabel = FindTMP(menuCanvasObj.transform, masterVolumeLabelName);

        Transform startTransform = panel != null ? FindChildByName(panel, startButtonName) : FindChildByName(menuCanvasObj.transform, startButtonName);
        if (startTransform != null)
        {
            Button startButton = startTransform.GetComponent<Button>();
            if (startButton != null)
            {
                controller.startButtonLabel = startTransform.GetComponentInChildren<TMP_Text>(true);
                startButton.onClick.RemoveListener(controller.StartGame);
                startButton.onClick.AddListener(controller.StartGame);
            }
        }

        Transform openShopTransform = panel != null ? FindChildByName(panel, openShopButtonName) : FindChildByName(menuCanvasObj.transform, openShopButtonName);
        if (openShopTransform != null)
        {
            Button openShopButton = openShopTransform.GetComponent<Button>();
            if (openShopButton != null)
            {
                controller.openShopButtonLabel = openShopTransform.GetComponentInChildren<TMP_Text>(true);
                openShopButton.onClick.RemoveListener(controller.OpenShop);
                openShopButton.onClick.AddListener(controller.OpenShop);
            }
        }

        Transform openSettingsTransform = panel != null ? FindChildByName(panel, openSettingsButtonName) : FindChildByName(menuCanvasObj.transform, openSettingsButtonName);
        if (openSettingsTransform != null)
        {
            Button openSettingsButton = openSettingsTransform.GetComponent<Button>();
            if (openSettingsButton != null)
            {
                controller.openSettingsButtonLabel = openSettingsTransform.GetComponentInChildren<TMP_Text>(true);
                openSettingsButton.onClick.RemoveListener(controller.OpenSettings);
                openSettingsButton.onClick.AddListener(controller.OpenSettings);
            }
        }

        Transform beamUpgradeTransform = shopPanel != null ? FindChildByName(shopPanel, beamUpgradeButtonName) : FindChildByName(menuCanvasObj.transform, beamUpgradeButtonName);
        if (beamUpgradeTransform != null)
        {
            Button beamUpgradeButton = beamUpgradeTransform.GetComponent<Button>();
            if (beamUpgradeButton != null)
            {
                controller.beamUpgradeButtonLabel = beamUpgradeTransform.GetComponentInChildren<TMP_Text>(true);
                controller.beamUpgradeButton = beamUpgradeButton;
                beamUpgradeButton.onClick.RemoveListener(controller.BuyBeamUpgrade);
                beamUpgradeButton.onClick.AddListener(controller.BuyBeamUpgrade);
            }
        }

        Transform powerUpgradeTransform = shopPanel != null ? FindChildByName(shopPanel, powerUpgradeButtonName) : FindChildByName(menuCanvasObj.transform, powerUpgradeButtonName);
        if (powerUpgradeTransform != null)
        {
            Button powerUpgradeButton = powerUpgradeTransform.GetComponent<Button>();
            if (powerUpgradeButton != null)
            {
                controller.powerUpgradeButtonLabel = powerUpgradeTransform.GetComponentInChildren<TMP_Text>(true);
                controller.powerUpgradeButton = powerUpgradeButton;
                powerUpgradeButton.onClick.RemoveListener(controller.BuyPowerUpgrade);
                powerUpgradeButton.onClick.AddListener(controller.BuyPowerUpgrade);
            }
        }

        Transform batteryUpgradeTransform = shopPanel != null ? FindChildByName(shopPanel, batteryUpgradeButtonName) : FindChildByName(menuCanvasObj.transform, batteryUpgradeButtonName);
        if (batteryUpgradeTransform != null)
        {
            Button batteryUpgradeButton = batteryUpgradeTransform.GetComponent<Button>();
            if (batteryUpgradeButton != null)
            {
                controller.batteryUpgradeButtonLabel = batteryUpgradeTransform.GetComponentInChildren<TMP_Text>(true);
                controller.batteryUpgradeButton = batteryUpgradeButton;
                batteryUpgradeButton.onClick.RemoveListener(controller.BuyBatteryUpgrade);
                batteryUpgradeButton.onClick.AddListener(controller.BuyBatteryUpgrade);
            }
        }

        Transform lunaSkinTransform = shopPanel != null ? FindChildByName(shopPanel, lunaSkinButtonName) : FindChildByName(menuCanvasObj.transform, lunaSkinButtonName);
        if (lunaSkinTransform != null)
        {
            Button lunaSkinButton = lunaSkinTransform.GetComponent<Button>();
            if (lunaSkinButton != null)
            {
                controller.lunaSkinButtonLabel = lunaSkinTransform.GetComponentInChildren<TMP_Text>(true);
                controller.lunaSkinButton = lunaSkinButton;
                lunaSkinButton.onClick.RemoveListener(controller.BuyOrEquipLunaSkin);
                lunaSkinButton.onClick.AddListener(controller.BuyOrEquipLunaSkin);
            }
        }

        Transform flashSkinTransform = shopPanel != null ? FindChildByName(shopPanel, flashSkinButtonName) : FindChildByName(menuCanvasObj.transform, flashSkinButtonName);
        if (flashSkinTransform != null)
        {
            Button flashSkinButton = flashSkinTransform.GetComponent<Button>();
            if (flashSkinButton != null)
            {
                controller.flashSkinButtonLabel = flashSkinTransform.GetComponentInChildren<TMP_Text>(true);
                controller.flashSkinButton = flashSkinButton;
                flashSkinButton.onClick.RemoveListener(controller.BuyOrEquipFlashSkin);
                flashSkinButton.onClick.AddListener(controller.BuyOrEquipFlashSkin);
            }
        }

        Transform closeShopTransform = shopPanel != null ? FindChildByName(shopPanel, closeShopButtonName) : FindChildByName(menuCanvasObj.transform, closeShopButtonName);
        if (closeShopTransform != null)
        {
            Button closeShopButton = closeShopTransform.GetComponent<Button>();
            if (closeShopButton != null)
            {
                controller.closeShopButtonLabel = closeShopTransform.GetComponentInChildren<TMP_Text>(true);
                closeShopButton.onClick.RemoveListener(controller.CloseShop);
                closeShopButton.onClick.AddListener(controller.CloseShop);
            }
        }

        Transform volumeSliderTransform = settingsPanel != null ? FindChildByName(settingsPanel, masterVolumeSliderName) : FindChildByName(menuCanvasObj.transform, masterVolumeSliderName);
        if (volumeSliderTransform != null)
        {
            Slider volumeSlider = volumeSliderTransform.GetComponent<Slider>();
            if (volumeSlider != null)
            {
                controller.masterVolumeSlider = volumeSlider;
                volumeSlider.onValueChanged.RemoveListener(controller.SetMasterVolume);
                volumeSlider.onValueChanged.AddListener(controller.SetMasterVolume);
            }
        }

        Transform vibrationTransform = settingsPanel != null ? FindChildByName(settingsPanel, vibrationButtonName) : FindChildByName(menuCanvasObj.transform, vibrationButtonName);
        if (vibrationTransform != null)
        {
            Button vibrationButton = vibrationTransform.GetComponent<Button>();
            if (vibrationButton != null)
            {
                controller.vibrationButtonLabel = vibrationTransform.GetComponentInChildren<TMP_Text>(true);
                vibrationButton.onClick.RemoveListener(controller.ToggleVibration);
                vibrationButton.onClick.AddListener(controller.ToggleVibration);
            }
        }

        Transform timerTransform = settingsPanel != null ? FindChildByName(settingsPanel, timerButtonName) : FindChildByName(menuCanvasObj.transform, timerButtonName);
        if (timerTransform != null)
        {
            Button timerButton = timerTransform.GetComponent<Button>();
            if (timerButton != null)
            {
                controller.timerButtonLabel = timerTransform.GetComponentInChildren<TMP_Text>(true);
                timerButton.onClick.RemoveListener(controller.ToggleHudTimer);
                timerButton.onClick.AddListener(controller.ToggleHudTimer);
            }
        }

        Transform perfTransform = settingsPanel != null ? FindChildByName(settingsPanel, perfOverlayButtonName) : FindChildByName(menuCanvasObj.transform, perfOverlayButtonName);
        if (perfTransform != null)
        {
            Button perfButton = perfTransform.GetComponent<Button>();
            if (perfButton != null)
            {
                controller.perfOverlayButtonLabel = perfTransform.GetComponentInChildren<TMP_Text>(true);
                perfButton.onClick.RemoveListener(controller.TogglePerfOverlay);
                perfButton.onClick.AddListener(controller.TogglePerfOverlay);
            }
        }

        Transform closeSettingsTransform = settingsPanel != null ? FindChildByName(settingsPanel, closeSettingsButtonName) : FindChildByName(menuCanvasObj.transform, closeSettingsButtonName);
        if (closeSettingsTransform != null)
        {
            Button closeSettingsButton = closeSettingsTransform.GetComponent<Button>();
            if (closeSettingsButton != null)
            {
                controller.closeSettingsButtonLabel = closeSettingsTransform.GetComponentInChildren<TMP_Text>(true);
                closeSettingsButton.onClick.RemoveListener(controller.CloseSettings);
                closeSettingsButton.onClick.AddListener(controller.CloseSettings);
            }
        }

        Transform quitTransform = panel != null ? FindChildByName(panel, quitButtonName) : FindChildByName(menuCanvasObj.transform, quitButtonName);
        if (quitTransform != null)
        {
            Button quitButton = quitTransform.GetComponent<Button>();
            if (quitButton != null)
            {
                controller.quitButtonLabel = quitTransform.GetComponentInChildren<TMP_Text>(true);
                quitButton.onClick.RemoveListener(controller.QuitGame);
                quitButton.onClick.AddListener(controller.QuitGame);
            }
        }

        if (FindAnyObjectByType<UpgradeManager>() == null)
        {
            gameObject.AddComponent<UpgradeManager>();
        }

        if (FindAnyObjectByType<SkinManager>() == null)
        {
            gameObject.AddComponent<SkinManager>();
        }

        if (FindAnyObjectByType<SettingsManager>() == null)
        {
            gameObject.AddComponent<SettingsManager>();
        }

        if (FindAnyObjectByType<CoinPickupSpawner>() == null)
        {
            gameObject.AddComponent<CoinPickupSpawner>();
        }

        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager == null)
        {
            audioManager = gameObject.AddComponent<AudioManager>();
        }

        audioManager.StartBackgroundMusic();
    }
}
