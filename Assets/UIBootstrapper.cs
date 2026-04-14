using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public partial class UIBootstrapper : MonoBehaviour
{
    [Header("Canvas Names")]
    public string hudCanvasName = "CanvasHUD";
    public string gameOverCanvasName = "CanvasGameOver";
    public string menuCanvasName = "CanvasMenu";

    [Header("HUD Object Names")]
    public string healthName = "TxtVida";
    public string scoreName = "TxtScore";
    public string highscoreName = "TxtRecorde";
    public string batteryName = "TxtBateria";
    public string coinsName = "TxtMoedas";
    public string survivalTimeName = "TxtTempo";
    public string stageName = "TxtFase";
    public string performanceTextName = "TxtPerf";
    public string pauseButtonName = "BtnPausa";

    [Header("Game Over Object Names")]
    public string panelName = "PanelGameOver";
    public string finalScoreName = "TxtScoreFinal";
    public string finalHighscoreName = "TxtRecordeFinal";
    public string restartButtonName = "BtnReiniciar";

    [Header("Main Menu Object Names")]
    public string menuPanelName = "PanelMenu";
    public string menuTitleName = "TxtTitulo";
    public string startButtonName = "BtnJogar";
    public string openShopButtonName = "BtnLoja";
    public string openSettingsButtonName = "BtnConfiguracoes";
    public string quitButtonName = "BtnSair";

    [Header("Shop Object Names")]
    public string shopPanelName = "PanelLoja";
    public string shopTitleName = "TxtTituloLoja";
    public string shopCoinsName = "TxtMoedasLoja";
    public string beamStatusName = "TxtStatusFeixe";
    public string powerStatusName = "TxtStatusPoder";
    public string batteryStatusName = "TxtStatusBateria";
    public string beamUpgradeButtonName = "BtnUpgradeFeixe";
    public string powerUpgradeButtonName = "BtnUpgradePoder";
    public string batteryUpgradeButtonName = "BtnUpgradeBateria";
    public string lunaSkinButtonName = "BtnSkinLuna";
    public string flashSkinButtonName = "BtnSkinLanterna";
    public string closeShopButtonName = "BtnVoltarLoja";

    [Header("Settings Object Names")]
    public string settingsPanelName = "PanelConfiguracoes";
    public string settingsTitleName = "TxtTituloConfig";
    public string masterVolumeLabelName = "TxtVolumeMaster";
    public string masterVolumeSliderName = "SliderVolumeMaster";
    public string vibrationButtonName = "BtnVibracao";
    public string timerButtonName = "BtnTimerHUD";
    public string perfOverlayButtonName = "BtnPerfHUD";
    public string closeSettingsButtonName = "BtnVoltarConfig";

    [Header("Canvas Scaler")]
    public Vector2 referenceResolution = new Vector2(1920f, 1080f);
    [Range(0f, 1f)] public float matchWidthOrHeight = 0.5f;

    [Header("Visual Preset")]
    public bool applyVisualPreset = true;
    public bool applyInEditMode = true;
    public int hudFontSize = 40;
    public int gameOverTitleFontSize = 64;
    public int gameOverInfoFontSize = 46;
    public int gameOverButtonFontSize = 44;

    [Header("Theme Colors")]
    public Color hudDefaultTextColor = new Color(0.85f, 0.92f, 1f, 1f);
    public Color healthTextColor = new Color(1f, 0.48f, 0.48f, 1f);
    public Color batteryTextColor = new Color(0.45f, 0.9f, 1f, 1f);
    public Color scoreTextColor = new Color(0.92f, 0.96f, 1f, 1f);
    public Color highscoreTextColor = new Color(0.77f, 0.88f, 1f, 1f);
    public Color coinsTextColor = new Color(1f, 0.84f, 0.35f, 1f);
    public Color gameOverPanelColor = new Color(0.02f, 0.02f, 0.05f, 0.82f);
    public Color gameOverPrimaryTextColor = new Color(1f, 0.9f, 0.75f, 1f);
    public Color gameOverSecondaryTextColor = new Color(0.85f, 0.9f, 1f, 1f);
    public Color pauseButtonBaseColor = new Color(0.12f, 0.14f, 0.18f, 0.92f);
    public Color restartButtonBaseColor = new Color(0.93f, 0.24f, 0.24f, 1f);
    public Color menuPanelColor = new Color(0.02f, 0.01f, 0.02f, 0.9f);
    public Color startButtonBaseColor = new Color(0.08f, 0.35f, 0.2f, 1f);
    public Color shopButtonBaseColor = new Color(0.2f, 0.14f, 0.42f, 1f);
    public Color beamUpgradeButtonColor = new Color(0.14f, 0.16f, 0.48f, 1f);
    public Color powerUpgradeButtonColor = new Color(0.44f, 0.16f, 0.08f, 1f);
    public Color shopPanelColor = new Color(0.05f, 0.01f, 0.01f, 0.95f);
    public Color shopPrimaryTextColor = new Color(0.94f, 0.84f, 0.84f, 1f);
    public Color shopAccentTextColor = new Color(0.95f, 0.72f, 0.35f, 1f);
    public Color backShopButtonBaseColor = new Color(0.18f, 0.18f, 0.2f, 1f);
    public Color settingsPanelColor = new Color(0.02f, 0.02f, 0.06f, 0.95f);
    public Color settingsButtonBaseColor = new Color(0.22f, 0.22f, 0.42f, 1f);
    public Color quitButtonBaseColor = new Color(0.55f, 0.18f, 0.18f, 1f);

    private void OnEnable()
    {
        if (!Application.isPlaying && !applyInEditMode)
        {
            return;
        }

        ApplySetup();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && !applyInEditMode)
        {
            return;
        }

        ApplySetup();
    }

    [ContextMenu("Apply UI Setup")]
    public void ApplySetup()
    {
        NormalizeObjectNames();

        GameObject hudCanvasObj = GameObject.Find(hudCanvasName);
        GameObject gameOverCanvasObj = GameObject.Find(gameOverCanvasName);
        GameObject menuCanvasObj = GameObject.Find(menuCanvasName);

        SetupCanvas(hudCanvasObj, 0);
        SetupCanvas(gameOverCanvasObj, 10);
        SetupCanvas(menuCanvasObj, 20);

        if (hudCanvasObj != null)
        {
            BindHud(hudCanvasObj);
            if (applyVisualPreset)
            {
                ApplyHudLayout(hudCanvasObj.transform);
            }
        }

        if (gameOverCanvasObj != null)
        {
            BindGameOver(gameOverCanvasObj);
            if (applyVisualPreset)
            {
                ApplyGameOverLayout(gameOverCanvasObj.transform);
            }
        }

        if (menuCanvasObj != null)
        {
            BindMainMenu(menuCanvasObj);
            if (applyVisualPreset)
            {
                ApplyMainMenuLayout(menuCanvasObj.transform);
            }
        }
    }

    private void SetupCanvas(GameObject canvasObj, int sortOrder)
    {
        if (canvasObj == null)
        {
            return;
        }

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        if (canvas == null)
        {
            return;
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;

        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = canvasObj.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = matchWidthOrHeight;

        if (canvasObj.GetComponent<SafeAreaFitter>() == null)
        {
            canvasObj.AddComponent<SafeAreaFitter>();
        }
    }

    private TMP_Text FindTMP(Transform root, string objectName)
    {
        Transform t = FindChildByName(root, objectName);
        if (t == null)
        {
            return null;
        }

        TMP_Text text = t.GetComponent<TMP_Text>();
        if (text != null)
        {
            return text;
        }

        return t.GetComponentInChildren<TMP_Text>(true);
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

    private void NormalizeObjectNames()
    {
        if (string.IsNullOrWhiteSpace(menuPanelName)) menuPanelName = "PanelMenu";
        if (string.IsNullOrWhiteSpace(menuTitleName)) menuTitleName = "TxtTitulo";
        if (string.IsNullOrWhiteSpace(startButtonName)) startButtonName = "BtnJogar";
        if (string.IsNullOrWhiteSpace(openShopButtonName)) openShopButtonName = "BtnLoja";
        if (string.IsNullOrWhiteSpace(openSettingsButtonName)) openSettingsButtonName = "BtnConfiguracoes";
        if (string.IsNullOrWhiteSpace(quitButtonName)) quitButtonName = "BtnSair";

        if (string.IsNullOrWhiteSpace(shopPanelName)) shopPanelName = "PanelLoja";
        if (string.IsNullOrWhiteSpace(shopTitleName)) shopTitleName = "TxtTituloLoja";
        if (string.IsNullOrWhiteSpace(shopCoinsName)) shopCoinsName = "TxtMoedasLoja";
        if (string.IsNullOrWhiteSpace(beamStatusName)) beamStatusName = "TxtStatusFeixe";
        if (string.IsNullOrWhiteSpace(powerStatusName)) powerStatusName = "TxtStatusPoder";
        if (string.IsNullOrWhiteSpace(batteryStatusName)) batteryStatusName = "TxtStatusBateria";
        if (string.IsNullOrWhiteSpace(beamUpgradeButtonName)) beamUpgradeButtonName = "BtnUpgradeFeixe";
        if (string.IsNullOrWhiteSpace(powerUpgradeButtonName)) powerUpgradeButtonName = "BtnUpgradePoder";
        if (string.IsNullOrWhiteSpace(batteryUpgradeButtonName)) batteryUpgradeButtonName = "BtnUpgradeBateria";
        if (string.IsNullOrWhiteSpace(lunaSkinButtonName)) lunaSkinButtonName = "BtnSkinLuna";
        if (string.IsNullOrWhiteSpace(flashSkinButtonName)) flashSkinButtonName = "BtnSkinLanterna";
        if (string.IsNullOrWhiteSpace(closeShopButtonName)) closeShopButtonName = "BtnVoltarLoja";

        if (string.IsNullOrWhiteSpace(settingsPanelName)) settingsPanelName = "PanelConfiguracoes";
        if (string.IsNullOrWhiteSpace(settingsTitleName)) settingsTitleName = "TxtTituloConfig";
        if (string.IsNullOrWhiteSpace(masterVolumeLabelName)) masterVolumeLabelName = "TxtVolumeMaster";
        if (string.IsNullOrWhiteSpace(masterVolumeSliderName)) masterVolumeSliderName = "SliderVolumeMaster";
        if (string.IsNullOrWhiteSpace(vibrationButtonName)) vibrationButtonName = "BtnVibracao";
        if (string.IsNullOrWhiteSpace(timerButtonName)) timerButtonName = "BtnTimerHUD";
        if (string.IsNullOrWhiteSpace(perfOverlayButtonName)) perfOverlayButtonName = "BtnPerfHUD";
        if (string.IsNullOrWhiteSpace(closeSettingsButtonName)) closeSettingsButtonName = "BtnVoltarConfig";

        // Handles legacy inspector values when field order changed.
        if (openShopButtonName == "BtnUpgradeFeixe" || openShopButtonName == "BtnUpgradePoder") openShopButtonName = "BtnLoja";
        if (quitButtonName == "BtnUpgradeFeixe" || quitButtonName == "BtnUpgradePoder") quitButtonName = "BtnSair";
        if (beamUpgradeButtonName == "BtnLoja" || beamUpgradeButtonName == "BtnSair") beamUpgradeButtonName = "BtnUpgradeFeixe";
        if (powerUpgradeButtonName == "BtnLoja" || powerUpgradeButtonName == "BtnSair") powerUpgradeButtonName = "BtnUpgradePoder";
        if (closeShopButtonName == "BtnUpgradeFeixe" || closeShopButtonName == "BtnUpgradePoder" || closeShopButtonName == "BtnLoja") closeShopButtonName = "BtnVoltarLoja";

        if (openShopButtonName == quitButtonName) openShopButtonName = "BtnLoja";
        if (openSettingsButtonName == openShopButtonName || openSettingsButtonName == quitButtonName) openSettingsButtonName = "BtnConfiguracoes";
        if (beamUpgradeButtonName == powerUpgradeButtonName) powerUpgradeButtonName = "BtnUpgradePoder";
        if (batteryUpgradeButtonName == beamUpgradeButtonName || batteryUpgradeButtonName == powerUpgradeButtonName) batteryUpgradeButtonName = "BtnUpgradeBateria";
        if (lunaSkinButtonName == beamUpgradeButtonName || lunaSkinButtonName == powerUpgradeButtonName || lunaSkinButtonName == batteryUpgradeButtonName) lunaSkinButtonName = "BtnSkinLuna";
        if (flashSkinButtonName == beamUpgradeButtonName || flashSkinButtonName == powerUpgradeButtonName || flashSkinButtonName == batteryUpgradeButtonName || flashSkinButtonName == lunaSkinButtonName) flashSkinButtonName = "BtnSkinLanterna";
        if (closeShopButtonName == beamUpgradeButtonName || closeShopButtonName == powerUpgradeButtonName || closeShopButtonName == batteryUpgradeButtonName) closeShopButtonName = "BtnVoltarLoja";
        if (closeSettingsButtonName == closeShopButtonName || closeSettingsButtonName == vibrationButtonName) closeSettingsButtonName = "BtnVoltarConfig";
        if (timerButtonName == vibrationButtonName || timerButtonName == closeSettingsButtonName) timerButtonName = "BtnTimerHUD";
        if (perfOverlayButtonName == timerButtonName || perfOverlayButtonName == vibrationButtonName || perfOverlayButtonName == closeSettingsButtonName) perfOverlayButtonName = "BtnPerfHUD";

        if (string.IsNullOrWhiteSpace(survivalTimeName)) survivalTimeName = "TxtTempo";
        if (string.IsNullOrWhiteSpace(stageName)) stageName = "TxtFase";
        if (string.IsNullOrWhiteSpace(performanceTextName)) performanceTextName = "TxtPerf";
    }
}