using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIBootstrapper
{
    private void ApplyHudLayout(Transform root)
    {
        EnsureText(root, survivalTimeName);
        EnsureText(root, stageName);
        EnsureText(root, performanceTextName);

        TMP_Text health = FindTMP(root, healthName);
        TMP_Text battery = FindTMP(root, batteryName);
        TMP_Text score = FindTMP(root, scoreName);
        TMP_Text highscore = FindTMP(root, highscoreName);
        TMP_Text coins = FindTMP(root, coinsName);
        TMP_Text survivalTime = FindTMP(root, survivalTimeName);
        TMP_Text stage = FindTMP(root, stageName);
        TMP_Text perf = FindTMP(root, performanceTextName);

        ConfigureHudText(health, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), TextAlignmentOptions.TopLeft);
        ConfigureHudText(battery, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -74f), TextAlignmentOptions.TopLeft);
        ConfigureHudText(score, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -24f), TextAlignmentOptions.TopRight);
        ConfigureHudText(highscore, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -74f), TextAlignmentOptions.TopRight);
        ConfigureHudText(coins, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -124f), TextAlignmentOptions.TopRight);
        ConfigureHudText(survivalTime, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -106f), TextAlignmentOptions.Top);
        ConfigureHudText(stage, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -156f), TextAlignmentOptions.Top);
        ConfigureHudText(perf, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(20f, 20f), TextAlignmentOptions.BottomLeft);

        ApplyHudTextTheme(health, healthTextColor, true);
        ApplyHudTextTheme(battery, batteryTextColor, true);
        ApplyHudTextTheme(score, scoreTextColor, true);
        ApplyHudTextTheme(highscore, highscoreTextColor, false);
        ApplyHudTextTheme(coins, coinsTextColor, true);
        ApplyHudTextTheme(survivalTime, new Color(0.92f, 0.92f, 0.96f, 1f), true);
        ApplyHudTextTheme(stage, new Color(0.86f, 0.96f, 1f, 1f), true);
        ApplyHudTextTheme(perf, new Color(0.8f, 0.96f, 0.82f, 1f), false);

        SetDefaultTextIfPlaceholder(health, "Vida: -");
        SetDefaultTextIfPlaceholder(battery, "Bateria: -");
        SetDefaultTextIfPlaceholder(score, "Score: 0");
        SetDefaultTextIfPlaceholder(highscore, "Recorde: 0");
        SetDefaultTextIfPlaceholder(coins, "Moedas: 0");
        SetDefaultTextIfPlaceholder(survivalTime, "Tempo: 00:00");
        SetDefaultTextIfPlaceholder(stage, "Fase: Apresentacao");
        SetDefaultTextIfPlaceholder(perf, "FPS --");

        Transform pauseTransform = FindChildByName(root, pauseButtonName);
        if (pauseTransform != null)
        {
            ConfigurePauseButton(pauseTransform);
        }
    }

    private void SetDefaultTextIfPlaceholder(TMP_Text text, string defaultValue)
    {
        if (text == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(text.text) || text.text.Contains("New Text"))
        {
            text.text = defaultValue;
        }
    }

    private void ConfigureHudText(TMP_Text text, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, TextAlignmentOptions alignment)
    {
        if (text == null)
        {
            return;
        }

        RectTransform rect = text.rectTransform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = anchorMax;
        rect.sizeDelta = new Vector2(520f, 52f);
        rect.anchoredPosition = anchoredPosition;

        text.alignment = alignment;
        text.enableAutoSizing = true;
        text.fontSizeMin = 24;
        text.fontSizeMax = hudFontSize;
        text.overflowMode = TextOverflowModes.Ellipsis;
    }

    private void ApplyHudTextTheme(TMP_Text text, Color color, bool bold)
    {
        if (text == null)
        {
            return;
        }

        text.color = color;
        text.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
    }

    private void ApplyGameOverLayout(Transform root)
    {
        Transform panelTransform = FindChildByName(root, panelName);
        if (panelTransform == null)
        {
            return;
        }

        RectTransform panelRect = panelTransform as RectTransform;
        if (panelRect != null)
        {
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        Image panelImage = panelTransform.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = gameOverPanelColor;
        }

        panelTransform.gameObject.SetActive(false);

        TMP_Text finalScore = FindTMP(root, finalScoreName);
        TMP_Text finalHighscore = FindTMP(root, finalHighscoreName);

        ConfigureGameOverText(finalScore, new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(900f, 90f), gameOverTitleFontSize);
        ConfigureGameOverText(finalHighscore, new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(900f, 90f), gameOverInfoFontSize);

        ApplyGameOverTextTheme(finalScore, gameOverPrimaryTextColor, true);
        ApplyGameOverTextTheme(finalHighscore, gameOverSecondaryTextColor, true);

        if (finalScore != null && (string.IsNullOrWhiteSpace(finalScore.text) || finalScore.text.Contains("New Text")))
        {
            finalScore.text = "Score Final: 0";
        }

        if (finalHighscore != null && (string.IsNullOrWhiteSpace(finalHighscore.text) || finalHighscore.text.Contains("New Text")))
        {
            finalHighscore.text = "Recorde: 0";
        }

        Transform buttonTransform = FindChildByName(root, restartButtonName);
        if (buttonTransform != null)
        {
            ConfigureRestartButton(buttonTransform);
        }
    }

    private void ConfigureGameOverText(TMP_Text text, Vector2 anchor, Vector2 position, Vector2 size, int maxSize)
    {
        if (text == null)
        {
            return;
        }

        RectTransform rect = text.rectTransform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        text.alignment = TextAlignmentOptions.Center;
        text.enableAutoSizing = true;
        text.fontSizeMin = 24;
        text.fontSizeMax = maxSize;
        text.overflowMode = TextOverflowModes.Overflow;
    }

    private void ApplyGameOverTextTheme(TMP_Text text, Color color, bool bold)
    {
        if (text == null)
        {
            return;
        }

        text.color = color;
        text.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
    }

    private void ConfigureRestartButton(Transform buttonTransform)
    {
        RectTransform rect = buttonTransform as RectTransform;
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(420f, 110f);
            rect.anchoredPosition = new Vector2(0f, -120f);
        }

        Image buttonImage = buttonTransform.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = restartButtonBaseColor;
        }

        Button button = buttonTransform.GetComponent<Button>();
        if (button != null)
        {
            ApplyButtonColorTint(button, restartButtonBaseColor);
        }

        TMP_Text buttonLabel = buttonTransform.GetComponentInChildren<TMP_Text>(true);
        if (buttonLabel != null)
        {
            buttonLabel.alignment = TextAlignmentOptions.Center;
            buttonLabel.enableAutoSizing = true;
            buttonLabel.fontSizeMin = 22;
            buttonLabel.fontSizeMax = gameOverButtonFontSize;
            buttonLabel.color = Color.white;
            buttonLabel.fontStyle = FontStyles.Bold;
            if (string.IsNullOrWhiteSpace(buttonLabel.text) || buttonLabel.text.Contains("Button"))
            {
                buttonLabel.text = "REINICIAR";
            }
        }
    }

    private void ConfigurePauseButton(Transform buttonTransform)
    {
        RectTransform rect = buttonTransform as RectTransform;
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(220f, 64f);
            rect.anchoredPosition = new Vector2(0f, -34f);
        }

        Image buttonImage = buttonTransform.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = pauseButtonBaseColor;
        }

        Button button = buttonTransform.GetComponent<Button>();
        if (button != null)
        {
            ApplyButtonColorTint(button, pauseButtonBaseColor);
        }

        TMP_Text buttonLabel = buttonTransform.GetComponentInChildren<TMP_Text>(true);
        if (buttonLabel != null)
        {
            buttonLabel.alignment = TextAlignmentOptions.Center;
            buttonLabel.enableAutoSizing = true;
            buttonLabel.fontSizeMin = 18;
            buttonLabel.fontSizeMax = 34;
            buttonLabel.color = hudDefaultTextColor;
            buttonLabel.fontStyle = FontStyles.Bold;

            if (string.IsNullOrWhiteSpace(buttonLabel.text) || buttonLabel.text.Contains("Button"))
            {
                buttonLabel.text = "PAUSAR";
            }
        }
    }

    private void ApplyButtonColorTint(Button button, Color baseColor)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = baseColor;
        cb.highlightedColor = Color.Lerp(baseColor, Color.white, 0.18f);
        cb.pressedColor = Color.Lerp(baseColor, Color.black, 0.25f);
        cb.selectedColor = Color.Lerp(baseColor, Color.white, 0.12f);
        cb.disabledColor = new Color(baseColor.r * 0.45f, baseColor.g * 0.45f, baseColor.b * 0.45f, 0.7f);
        cb.colorMultiplier = 1f;
        cb.fadeDuration = 0.08f;
        button.colors = cb;
    }

    private void ApplyMainMenuLayout(Transform root)
    {
        Transform panelTransform = FindChildByName(root, menuPanelName);
        if (panelTransform == null)
        {
            return;
        }

        RectTransform panelRect = panelTransform as RectTransform;
        if (panelRect != null)
        {
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        Image panelImage = panelTransform.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = menuPanelColor;
        }

        TMP_Text title = FindTMP(root, menuTitleName);
        if (title != null)
        {
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(980f, 130f);
            titleRect.anchoredPosition = new Vector2(0f, 170f);

            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 30;
            title.fontSizeMax = 84;
            title.fontStyle = FontStyles.Bold;
            title.color = new Color(0.96f, 0.98f, 1f, 1f);

            if (string.IsNullOrWhiteSpace(title.text) || title.text.Contains("New Text"))
            {
                title.text = "GHOST BEAM";
            }
        }

        EnsureMenuButton(panelTransform, startButtonName);
        EnsureMenuButton(panelTransform, openShopButtonName);
        EnsureMenuButton(panelTransform, openSettingsButtonName);
        EnsureMenuButton(panelTransform, quitButtonName);

        Transform shopPanelTransform = EnsurePanel(root, shopPanelName);
        ApplyShopLayout(root, shopPanelTransform);
        Transform settingsPanelTransform = EnsurePanel(root, settingsPanelName);
        ApplySettingsLayout(settingsPanelTransform);

        Transform startTransform = FindChildByName(panelTransform, startButtonName);
        if (startTransform != null)
        {
            ConfigureMenuButton(startTransform, new Vector2(0f, 10f), "JOGAR", startButtonBaseColor);
        }

        Transform shopTransform = FindChildByName(panelTransform, openShopButtonName);
        if (shopTransform != null)
        {
            ConfigureMenuButton(shopTransform, new Vector2(0f, -90f), "LOJA", shopButtonBaseColor);
        }

        Transform settingsTransform = FindChildByName(panelTransform, openSettingsButtonName);
        if (settingsTransform != null)
        {
            ConfigureMenuButton(settingsTransform, new Vector2(0f, -190f), "CONFIGURACOES", settingsButtonBaseColor);
        }

        Transform quitTransform = FindChildByName(panelTransform, quitButtonName);
        if (quitTransform != null)
        {
            ConfigureMenuButton(quitTransform, new Vector2(0f, -290f), "SAIR", quitButtonBaseColor);
        }
    }

    private void ApplySettingsLayout(Transform settingsPanelTransform)
    {
        if (settingsPanelTransform == null)
        {
            return;
        }

        RectTransform panelRect = settingsPanelTransform as RectTransform;
        if (panelRect != null)
        {
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        Image panelImage = settingsPanelTransform.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = settingsPanelColor;
        }

        EnsureText(settingsPanelTransform, settingsTitleName);
        EnsureText(settingsPanelTransform, masterVolumeLabelName);
        EnsureSlider(settingsPanelTransform, masterVolumeSliderName);
        EnsureMenuButton(settingsPanelTransform, vibrationButtonName);
        EnsureMenuButton(settingsPanelTransform, timerButtonName);
        EnsureMenuButton(settingsPanelTransform, perfOverlayButtonName);
        EnsureMenuButton(settingsPanelTransform, closeSettingsButtonName);

        TMP_Text title = FindTMP(settingsPanelTransform, settingsTitleName);
        if (title != null)
        {
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(980f, 110f);
            titleRect.anchoredPosition = new Vector2(0f, 220f);

            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 28;
            title.fontSizeMax = 70;
            title.fontStyle = FontStyles.Bold;
            title.color = new Color(0.87f, 0.9f, 1f, 1f);
            title.text = "CONFIGURACOES";
        }

        TMP_Text volumeLabel = FindTMP(settingsPanelTransform, masterVolumeLabelName);
        if (volumeLabel != null)
        {
            RectTransform labelRect = volumeLabel.rectTransform;
            labelRect.anchorMin = new Vector2(0.5f, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.sizeDelta = new Vector2(760f, 70f);
            labelRect.anchoredPosition = new Vector2(0f, 120f);

            volumeLabel.alignment = TextAlignmentOptions.Center;
            volumeLabel.enableAutoSizing = true;
            volumeLabel.fontSizeMin = 18;
            volumeLabel.fontSizeMax = 42;
            volumeLabel.fontStyle = FontStyles.Bold;
            volumeLabel.color = new Color(0.96f, 0.9f, 0.74f, 1f);
            volumeLabel.text = "VOLUME MASTER: 100%";
        }

        Transform sliderTransform = FindChildByName(settingsPanelTransform, masterVolumeSliderName);
        if (sliderTransform != null)
        {
            RectTransform sliderRect = sliderTransform as RectTransform;
            if (sliderRect != null)
            {
                sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
                sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
                sliderRect.pivot = new Vector2(0.5f, 0.5f);
                sliderRect.sizeDelta = new Vector2(520f, 28f);
                sliderRect.anchoredPosition = new Vector2(0f, 70f);
            }
        }

        Transform vibrationTransform = FindChildByName(settingsPanelTransform, vibrationButtonName);
        if (vibrationTransform != null)
        {
            ConfigureMenuButton(vibrationTransform, new Vector2(0f, -14f), "VIBRACAO: ON", new Color(0.24f, 0.32f, 0.18f, 1f));
        }

        Transform timerTransform = FindChildByName(settingsPanelTransform, timerButtonName);
        if (timerTransform != null)
        {
            ConfigureMenuButton(timerTransform, new Vector2(0f, -112f), "TIMER: ON", new Color(0.18f, 0.29f, 0.34f, 1f));
        }

        Transform perfTransform = FindChildByName(settingsPanelTransform, perfOverlayButtonName);
        if (perfTransform != null)
        {
            ConfigureMenuButton(perfTransform, new Vector2(0f, -210f), "PERFORMANCE: OFF", new Color(0.2f, 0.2f, 0.3f, 1f));
        }

        Transform closeTransform = FindChildByName(settingsPanelTransform, closeSettingsButtonName);
        if (closeTransform != null)
        {
            ConfigureMenuButton(closeTransform, new Vector2(0f, -310f), "VOLTAR MENU", backShopButtonBaseColor);
        }

        if (!Application.isPlaying)
        {
            settingsPanelTransform.gameObject.SetActive(false);
        }
    }

    private void ApplyShopLayout(Transform root, Transform shopPanelTransform)
    {
        if (shopPanelTransform == null)
        {
            return;
        }

        RectTransform panelRect = shopPanelTransform as RectTransform;
        if (panelRect != null)
        {
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        Image panelImage = shopPanelTransform.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = shopPanelColor;
        }

        EnsureText(shopPanelTransform, shopTitleName);
        EnsureText(shopPanelTransform, shopCoinsName);
        EnsureText(shopPanelTransform, beamStatusName);
        EnsureText(shopPanelTransform, powerStatusName);
        EnsureText(shopPanelTransform, batteryStatusName);
        MoveExistingByName(root, shopPanelTransform, beamUpgradeButtonName);
        MoveExistingByName(root, shopPanelTransform, powerUpgradeButtonName);
        MoveExistingByName(root, shopPanelTransform, batteryUpgradeButtonName);
        MoveExistingByName(root, shopPanelTransform, lunaSkinButtonName);
        MoveExistingByName(root, shopPanelTransform, flashSkinButtonName);
        EnsureMenuButton(shopPanelTransform, beamUpgradeButtonName);
        EnsureMenuButton(shopPanelTransform, powerUpgradeButtonName);
        EnsureMenuButton(shopPanelTransform, batteryUpgradeButtonName);
        EnsureMenuButton(shopPanelTransform, lunaSkinButtonName);
        EnsureMenuButton(shopPanelTransform, flashSkinButtonName);
        EnsureMenuButton(shopPanelTransform, closeShopButtonName);
        RemoveDuplicateButtonsByName(shopPanelTransform, beamUpgradeButtonName);
        RemoveDuplicateButtonsByName(shopPanelTransform, powerUpgradeButtonName);
        RemoveDuplicateButtonsByName(shopPanelTransform, batteryUpgradeButtonName);
        RemoveDuplicateButtonsByName(shopPanelTransform, lunaSkinButtonName);
        RemoveDuplicateButtonsByName(shopPanelTransform, flashSkinButtonName);
        RemoveDuplicateButtonsByName(shopPanelTransform, closeShopButtonName);

        TMP_Text title = FindTMP(shopPanelTransform, shopTitleName);
        if (title != null)
        {
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(980f, 110f);
            titleRect.anchoredPosition = new Vector2(0f, 240f);

            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = true;
            title.fontSizeMin = 28;
            title.fontSizeMax = 72;
            title.fontStyle = FontStyles.Bold;
            title.color = shopPrimaryTextColor;
            title.text = "ARSENAL DA SOMBRA";
            title.characterSpacing = 6f;
        }

        TMP_Text coins = FindTMP(shopPanelTransform, shopCoinsName);
        if (coins != null)
        {
            RectTransform coinsRect = coins.rectTransform;
            coinsRect.anchorMin = new Vector2(0.5f, 0.5f);
            coinsRect.anchorMax = new Vector2(0.5f, 0.5f);
            coinsRect.pivot = new Vector2(0.5f, 0.5f);
            coinsRect.sizeDelta = new Vector2(760f, 84f);
            coinsRect.anchoredPosition = new Vector2(0f, 158f);

            coins.alignment = TextAlignmentOptions.Center;
            coins.enableAutoSizing = true;
            coins.fontSizeMin = 22;
            coins.fontSizeMax = 52;
            coins.fontStyle = FontStyles.Bold;
            coins.color = shopAccentTextColor;
            coins.text = "MOEDAS: 0";
            coins.characterSpacing = 4f;
        }

        Transform beamUpgradeTransform = FindChildByName(shopPanelTransform, beamUpgradeButtonName);
        if (beamUpgradeTransform != null)
        {
            ConfigureMenuButton(beamUpgradeTransform, new Vector2(0f, 92f), "UP FEIXE L1", beamUpgradeButtonColor);
        }

        TMP_Text beamStatus = FindTMP(shopPanelTransform, beamStatusName);
        if (beamStatus != null)
        {
            beamStatus.gameObject.SetActive(false);
        }

        Transform powerUpgradeTransform = FindChildByName(shopPanelTransform, powerUpgradeButtonName);
        if (powerUpgradeTransform != null)
        {
            ConfigureMenuButton(powerUpgradeTransform, new Vector2(0f, 2f), "UP PODER L1", powerUpgradeButtonColor);
        }

        Transform batteryUpgradeTransform = FindChildByName(shopPanelTransform, batteryUpgradeButtonName);
        if (batteryUpgradeTransform != null)
        {
            ConfigureMenuButton(batteryUpgradeTransform, new Vector2(0f, -88f), "UP BATERIA L1", new Color(0.18f, 0.3f, 0.18f, 1f));
        }

        Transform lunaSkinTransform = FindChildByName(shopPanelTransform, lunaSkinButtonName);
        if (lunaSkinTransform != null)
        {
            ConfigureMenuButton(lunaSkinTransform, new Vector2(0f, -178f), "SKIN LUNA", new Color(0.16f, 0.34f, 0.42f, 1f));
        }

        Transform flashSkinTransform = FindChildByName(shopPanelTransform, flashSkinButtonName);
        if (flashSkinTransform != null)
        {
            ConfigureMenuButton(flashSkinTransform, new Vector2(0f, -268f), "SKIN LANTERNA", new Color(0.38f, 0.2f, 0.1f, 1f));
        }

        TMP_Text powerStatus = FindTMP(shopPanelTransform, powerStatusName);
        if (powerStatus != null)
        {
            powerStatus.gameObject.SetActive(false);
        }

        TMP_Text batteryStatus = FindTMP(shopPanelTransform, batteryStatusName);
        if (batteryStatus != null)
        {
            batteryStatus.gameObject.SetActive(false);
        }

        Transform closeShopTransform = FindChildByName(shopPanelTransform, closeShopButtonName);
        if (closeShopTransform != null)
        {
            ConfigureMenuButton(closeShopTransform, new Vector2(0f, -372f), "VOLTAR MENU", backShopButtonBaseColor);
        }

        if (!Application.isPlaying)
        {
            shopPanelTransform.gameObject.SetActive(false);
        }
    }

    private void MoveExistingByName(Transform root, Transform targetParent, string objectName)
    {
        if (root == null || targetParent == null || string.IsNullOrEmpty(objectName))
        {
            return;
        }

        Transform existing = FindChildByName(root, objectName);
        if (existing == null || existing.parent == targetParent)
        {
            return;
        }

        existing.SetParent(targetParent, false);
    }

    private Transform EnsurePanel(Transform root, string panelNameToEnsure)
    {
        Transform existing = FindChildByName(root, panelNameToEnsure);
        if (existing != null)
        {
            return existing;
        }

        GameObject panelObject = new GameObject(panelNameToEnsure, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(root, false);
        return panelObject.transform;
    }

    private void EnsureText(Transform parent, string textName)
    {
        if (parent == null || string.IsNullOrEmpty(textName))
        {
            return;
        }

        Transform existing = FindChildByName(parent, textName);
        if (existing != null)
        {
            return;
        }

        GameObject textObject = new GameObject(textName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
    }

    private void EnsureSlider(Transform parent, string sliderName)
    {
        if (parent == null || string.IsNullOrEmpty(sliderName))
        {
            return;
        }

        Transform existing = FindChildByName(parent, sliderName);
        if (existing != null)
        {
            return;
        }

        GameObject sliderObject = new GameObject(sliderName, typeof(RectTransform), typeof(Slider), typeof(Image));
        sliderObject.transform.SetParent(parent, false);

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0f);
        fillAreaRect.anchorMax = new Vector2(1f, 1f);
        fillAreaRect.offsetMin = new Vector2(8f, 8f);
        fillAreaRect.offsetMax = new Vector2(-8f, -8f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.GetComponent<Image>();
        fillImage.color = new Color(0.88f, 0.71f, 0.28f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderObject.transform, false);
        RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = new Vector2(0f, 0f);
        handleAreaRect.anchorMax = new Vector2(1f, 1f);
        handleAreaRect.offsetMin = new Vector2(8f, 8f);
        handleAreaRect.offsetMax = new Vector2(-8f, -8f);

        GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.GetComponent<Image>();
        handleImage.color = new Color(0.93f, 0.93f, 0.96f, 1f);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(18f, 34f);

        Slider slider = sliderObject.GetComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        Image background = sliderObject.GetComponent<Image>();
        background.color = new Color(0.08f, 0.08f, 0.12f, 0.92f);
    }

    private void EnsureMenuButton(Transform parent, string buttonName)
    {
        if (parent == null || string.IsNullOrEmpty(buttonName))
        {
            return;
        }

        Transform existing = FindChildByName(parent, buttonName);
        if (existing != null)
        {
            NormalizeButtonLabel(existing);
            return;
        }

        GameObject buttonObject = new GameObject(buttonName, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        NormalizeButtonLabel(buttonObject.transform);
    }

    private void ConfigureMenuButton(Transform buttonTransform, Vector2 position, string defaultLabel, Color baseColor)
    {
        NormalizeButtonLabel(buttonTransform);

        RectTransform rect = buttonTransform as RectTransform;
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(460f, 90f);
            rect.anchoredPosition = position;
        }

        Image buttonImage = buttonTransform.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = baseColor;
        }

        Button button = buttonTransform.GetComponent<Button>();
        if (button != null)
        {
            ApplyButtonColorTint(button, baseColor);
        }

        TMP_Text label = buttonTransform.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 18;
            label.fontSizeMax = 42;
            label.fontStyle = FontStyles.Bold;
            label.color = Color.white;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Ellipsis;

            if (string.IsNullOrWhiteSpace(label.text) || label.text.Contains("Button"))
            {
                label.text = defaultLabel;
            }
        }
    }

    private void ConfigureShopStatusText(TMP_Text text, Vector2 position)
    {
        if (text == null)
        {
            return;
        }

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(620f, 48f);
        rect.anchoredPosition = position;

        text.alignment = TextAlignmentOptions.Center;
        text.enableAutoSizing = true;
        text.fontSizeMin = 14;
        text.fontSizeMax = 26;
        text.fontStyle = FontStyles.Bold;
        text.color = new Color(0.96f, 0.77f, 0.77f, 1f);
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Ellipsis;
    }

    private void NormalizeButtonLabel(Transform buttonTransform)
    {
        if (buttonTransform == null)
        {
            return;
        }

        TMP_Text[] labels = buttonTransform.GetComponentsInChildren<TMP_Text>(true);
        Text[] legacyLabels = buttonTransform.GetComponentsInChildren<Text>(true);
        TMP_Text keep = null;

        for (int i = 0; i < labels.Length; i++)
        {
            TMP_Text current = labels[i];
            if (current == null)
            {
                continue;
            }

            if (keep == null || current.name == "Label")
            {
                keep = current;
                if (current.name == "Label")
                {
                    break;
                }
            }
        }

        if (keep == null)
        {
            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(buttonTransform, false);
            keep = labelObject.GetComponent<TMP_Text>();
        }

        keep.name = "Label";
        for (int i = 0; i < labels.Length; i++)
        {
            TMP_Text current = labels[i];
            if (current == null || current == keep)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(current.gameObject);
            }
            else
            {
                DestroyImmediate(current.gameObject);
            }
        }

        RectTransform keepRect = keep.rectTransform;
        keepRect.SetParent(buttonTransform, false);
        keepRect.anchorMin = Vector2.zero;
        keepRect.anchorMax = Vector2.one;
        keepRect.offsetMin = Vector2.zero;
        keepRect.offsetMax = Vector2.zero;

        for (int i = 0; i < legacyLabels.Length; i++)
        {
            Text legacy = legacyLabels[i];
            if (legacy == null)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(legacy.gameObject);
            }
            else
            {
                DestroyImmediate(legacy.gameObject);
            }
        }
    }

    private void RemoveDuplicateButtonsByName(Transform parent, string buttonName)
    {
        if (parent == null || string.IsNullOrEmpty(buttonName))
        {
            return;
        }

        Transform[] all = parent.GetComponentsInChildren<Transform>(true);
        Transform keep = null;

        for (int i = 0; i < all.Length; i++)
        {
            Transform current = all[i];
            if (current == null || current.name != buttonName)
            {
                continue;
            }

            if (keep == null)
            {
                keep = current;
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(current.gameObject);
            }
            else
            {
                DestroyImmediate(current.gameObject);
            }
        }

        if (keep != null)
        {
            NormalizeButtonLabel(keep);
        }
    }
}
