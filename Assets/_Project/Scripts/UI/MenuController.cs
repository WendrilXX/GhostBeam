using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GhostBeam.Managers;
using GhostBeam.Player;
using GhostBeam.Enemy;
using GhostBeam.Items;
using System.Collections.Generic;

namespace GhostBeam.UI
{
    /// <summary>
    /// Runtime menu controller - handles button clicks and panel visibility
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        private const string BeamUpgradeTierKey = "Upgrade_Beam_Tier";
        private const string PowerUpgradeTierKey = "Upgrade_Power_Tier";
        private const string BatteryUpgradeTierKey = "Upgrade_Battery_Tier";
        private const string HealthUpgradeTierKey = "Upgrade_Health_Tier";
        private const int MaxUpgradeTier = 3;

        private static readonly int[] BeamTierPrices = { 500, 900, 1400 };
        private static readonly int[] PowerTierPrices = { 750, 1250, 1850 };
        private static readonly int[] BatteryTierPrices = { 1000, 1600, 2300 };
        private static readonly int[] HealthTierPrices = { 150, 150, 150 };

        private GameObject shopPanel;
        private GameObject settingsPanel;
        private Button btnPlay;
        private Button btnShop;
        private Button btnSettings;
        private Button btnQuit;
        private GameObject mainMenuContainer;
        private Slider volumeSlider;
        private TextMeshProUGUI volumeValueText;
        private Button vibrationToggleButton;
        private TextMeshProUGUI vibrationStateText;
        private Button fpsToggleButton;
        private TextMeshProUGUI fpsStateText;
        private TextMeshProUGUI shopFeedbackText;
        private TextMeshProUGUI shopCoinsText;
        private static bool isMenuActive = false;

        private void Start()
        {
            Debug.Log("[MenuController] Menu started...");
            
            // Ensure EventSystem exists
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                Debug.Log("[MenuController] Creating EventSystem...");
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            // Find all UI elements from this menu canvas, including inactive objects
            shopPanel = FindChildByName(transform, "ShopPanel")?.gameObject;
            settingsPanel = FindChildByName(transform, "SettingsPanel")?.gameObject;
            mainMenuContainer = FindChildByName(transform, "MainMenuContainer")?.gameObject;

            btnPlay = FindChildByName(transform, "BtnPlay")?.GetComponent<Button>();
            btnShop = FindChildByName(transform, "BtnShop")?.GetComponent<Button>();
            btnSettings = FindChildByName(transform, "BtnSettings")?.GetComponent<Button>();
            btnQuit = FindChildByName(transform, "BtnQuit")?.GetComponent<Button>();

            Debug.Log($"[MenuController] Found buttons - Play:{btnPlay != null}, Shop:{btnShop != null}, Settings:{btnSettings != null}, Quit:{btnQuit != null}");

            // Register button listeners
            if (btnPlay != null)
            {
                btnPlay.onClick.RemoveAllListeners();
                btnPlay.onClick.AddListener(OnPlayClick);
                Debug.Log("[MenuController] Play button listener registered");
            }
            
            if (btnShop != null)
            {
                btnShop.onClick.RemoveAllListeners();
                btnShop.onClick.AddListener(OnShopClick);
                Debug.Log("[MenuController] Shop button listener registered");
            }
            
            if (btnSettings != null)
            {
                btnSettings.onClick.RemoveAllListeners();
                btnSettings.onClick.AddListener(OnSettingsClick);
                Debug.Log("[MenuController] Settings button listener registered");
            }
            
            if (btnQuit != null)
            {
                btnQuit.onClick.RemoveAllListeners();
                btnQuit.onClick.AddListener(OnQuitClick);
                Debug.Log("[MenuController] Quit button listener registered");
            }

            // Ensure main menu is visible, panels hidden
            if (mainMenuContainer != null) mainMenuContainer.SetActive(true);
            if (shopPanel != null) shopPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            
            // Add back button listeners from panels
            Button backBtnShop = EnsureShopBackButton();
            if (backBtnShop != null)
            {
                backBtnShop.onClick.RemoveAllListeners();
                backBtnShop.onClick.AddListener(OnBackClick);
                Debug.Log("[MenuController] Shop back button listener registered");
            }
            
            Button backBtnSettings = FindBackButtonInPanel("SettingsPanel");
            if (backBtnSettings != null)
            {
                backBtnSettings.onClick.RemoveAllListeners();
                backBtnSettings.onClick.AddListener(OnBackClick);
                Debug.Log("[MenuController] Settings back button listener registered");
            }

            SetupShopInteractions();
            SetupSettingsInteractions();

            ScoreManager.onCoinsChanged -= OnCoinsChanged;
            ScoreManager.onCoinsChanged += OnCoinsChanged;
            UpdateShopCoinsText(ScoreManager.Instance != null ? ScoreManager.Instance.Coins : 0);
            
            // PAUSE ALL GAMEPLAY MANAGERS
            PauseGameplay();
            
            // Hide Gameplay HUD canvas
            GameObject gameplayCanvas = GameObject.Find("CanvasHUD");
            if (gameplayCanvas != null)
            {
                gameplayCanvas.SetActive(false);
                Debug.Log("[MenuController] Gameplay HUD hidden");
            }
            
            // IMPORTANT: Do NOT use Time.timeScale = 0 as it breaks UI events
            // Instead, let the game run but disable spawning/updates via GameManager
            // For now, we'll leave Time running so UI events work properly
            isMenuActive = true;
            Debug.Log("[MenuController] Menu activated - Gameplay paused");

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuMusic();
            }
            
            Debug.Log("[MenuController] Setup complete - buttons ready for clicks");
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            // Test mouse input in realtime
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Input.mousePosition;
                Debug.Log($"[MenuController] Mouse clicked at {mousePos}");
                
                // Check EventSystem status
                var eventSystem = UnityEngine.EventSystems.EventSystem.current;
                if (eventSystem != null)
                {
                    Debug.Log($"[MenuController] EventSystem found: {eventSystem.name}");
                    var selectedObject = eventSystem.currentSelectedGameObject;
                    Debug.Log($"[MenuController] Currently selected object: {selectedObject?.name ?? "NONE"}");
                }
                else
                {
                    Debug.LogError("[MenuController] EventSystem NOT found!");
                }
                
                // Check button states
                if (btnPlay != null)
                {
                    Debug.Log($"[MenuController] Play button - interactable:{btnPlay.interactable}, active:{btnPlay.gameObject.activeSelf}");
                }
            }
#endif
        }

        private void OnPlayClick()
        {
            Debug.Log("[MenuController] *** PLAY BUTTON CLICKED ***");
            PlayMenuClickSfx();
            isMenuActive = false;
            ResumeGameplay();  // Re-enable all systems before loading
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameplayMusic();
            }
            Debug.Log("[MenuController] Loading Gameplay scene...");
            SceneManager.LoadScene("Assets/_Project/Scenes/Gameplay.unity", LoadSceneMode.Single);
        }

        private void OnShopClick()
        {
            Debug.Log("[MenuController] *** SHOP BUTTON CLICKED ***");
            PlayMenuClickSfx();
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                if (settingsPanel != null) settingsPanel.SetActive(false);
                if (mainMenuContainer != null) mainMenuContainer.SetActive(false);
                UpdateShopCoinsText(ScoreManager.Instance != null ? ScoreManager.Instance.Coins : 0);
                RefreshShopItemsUI();
                ShowShopFeedback("Selecione um upgrade", new Color(0.85f, 0.9f, 1f, 0.95f));
                Debug.Log("[MenuController] Shop panel shown");
            }
            else
            {
                Debug.LogWarning("[MenuController] ShopPanel not found");
            }
        }

        private void OnSettingsClick()
        {
            Debug.Log("[MenuController] *** SETTINGS BUTTON CLICKED ***");
            PlayMenuClickSfx();
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
                if (shopPanel != null) shopPanel.SetActive(false);
                if (mainMenuContainer != null) mainMenuContainer.SetActive(false);
                Debug.Log("[MenuController] Settings panel shown");
            }
            else
            {
                Debug.LogWarning("[MenuController] SettingsPanel not found");
            }
        }

        private void OnQuitClick()
        {
            Debug.Log("[MenuController] *** QUIT BUTTON CLICKED ***");
            PlayMenuClickSfx();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        public static bool IsMenuActive => isMenuActive;

        private void OnBackClick()
        {
            Debug.Log("[MenuController] *** BACK BUTTON CLICKED ***");
            PlayMenuClickSfx();
            if (shopPanel != null) shopPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (mainMenuContainer != null) mainMenuContainer.SetActive(true);
            Debug.Log("[MenuController] Returned to main menu");
        }

        private void SetupShopInteractions()
        {
            var panel = FindChildByName(transform, "ShopPanel");
            if (panel == null)
            {
                Debug.LogWarning("[MenuController] ShopPanel not found for interactions");
                return;
            }

            shopFeedbackText = FindChildByName(panel, "ShopFeedbackText")?.GetComponent<TextMeshProUGUI>();
            shopCoinsText = FindChildByName(panel, "ShopCoinsText")?.GetComponent<TextMeshProUGUI>();
            UpdateShopCoinsText(ScoreManager.Instance != null ? ScoreManager.Instance.Coins : 0);

            var buttons = panel.GetComponentsInChildren<Button>(true);
            int hooked = 0;
            foreach (var button in buttons)
            {
                if (button == null || button.gameObject.name == "BtnBack")
                    continue;

                if (!button.gameObject.name.StartsWith("Buy_"))
                    continue;

                string itemName = ParseItemTokenFromButtonName(button.gameObject.name);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnBuyItemClick(itemName));
                hooked++;
            }

            Debug.Log($"[MenuController] Shop item listeners registered: {hooked}");
            RefreshShopItemsUI();
        }

        private void OnBuyItemClick(string itemName)
        {
            if (ScoreManager.Instance == null)
            {
                Debug.LogWarning("[MenuController] ScoreManager not found. Cannot purchase.");
                ShowShopFeedback("Sistema de moedas indisponivel", new Color(1f, 0.45f, 0.45f, 1f));
                return;
            }

            string key = GetUpgradeTierKey(itemName);
            if (string.IsNullOrEmpty(key))
            {
                ShowShopFeedback("Upgrade invalido", new Color(1f, 0.45f, 0.45f, 1f));
                return;
            }

            int currentTier = Mathf.Clamp(PlayerPrefs.GetInt(key, 0), 0, MaxUpgradeTier);
            if (currentTier >= MaxUpgradeTier)
            {
                ShowShopFeedback("Esse upgrade ja esta no nivel MAX", new Color(0.95f, 0.85f, 0.3f, 1f));
                RefreshShopItemsUI();
                return;
            }

            int price = GetPriceForNextTier(itemName, currentTier);
            if (price <= 0)
            {
                ShowShopFeedback("Nao foi possivel calcular o preco", new Color(1f, 0.45f, 0.45f, 1f));
                return;
            }

            if (ScoreManager.Instance.TrySpendCoins(price))
            {
                PlayMenuClickSfx();
                int newTier = currentTier + 1;
                PlayerPrefs.SetInt(key, newTier);
                PlayerPrefs.Save();

                Debug.Log($"[MenuController] Purchase successful: {itemName} T{newTier} ({price} coins)");
                ShowShopFeedback($"Upgrade aplicado: {GetDisplayName(itemName)} T{newTier}/{MaxUpgradeTier}", new Color(0.25f, 0.95f, 0.45f, 1f));
            }
            else
            {
                Debug.Log($"[MenuController] Not enough coins for {itemName}. Need {price}, have {ScoreManager.Instance.Coins}");
                ShowShopFeedback("Moedas insuficientes", new Color(1f, 0.45f, 0.45f, 1f));
            }

            UpdateShopCoinsText(ScoreManager.Instance.Coins);
            RefreshShopItemsUI();
        }

        private void PlayMenuClickSfx()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuClick();
            }
        }

        private void ShowShopFeedback(string message, Color color)
        {
            if (shopFeedbackText == null)
                return;

            shopFeedbackText.text = message;
            shopFeedbackText.color = color;
        }

        private void OnCoinsChanged(int coins)
        {
            UpdateShopCoinsText(coins);
        }

        private void UpdateShopCoinsText(int coins)
        {
            if (shopCoinsText == null)
                return;

            shopCoinsText.text = $"Moedas: {coins}";
        }

        private void SetupSettingsInteractions()
        {
            var panel = FindChildByName(transform, "SettingsPanel");
            if (panel == null)
            {
                Debug.LogWarning("[MenuController] SettingsPanel not found for interactions");
                return;
            }

            volumeSlider = FindChildByName(panel, "MasterVolumeSlider")?.GetComponent<Slider>();
            volumeValueText = FindChildByName(panel, "MasterVolumeValue")?.GetComponent<TextMeshProUGUI>();

            vibrationToggleButton = FindChildByName(panel, "Toggle_Vibracao")?.GetComponent<Button>();
            vibrationStateText = FindChildByName(panel, "VibracaoState")?.GetComponent<TextMeshProUGUI>();

            fpsToggleButton = FindChildByName(panel, "Toggle_FPSDisplay")?.GetComponent<Button>();
            fpsStateText = FindChildByName(panel, "FPSDisplayState")?.GetComponent<TextMeshProUGUI>();

            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveAllListeners();
                float currentVolume = SettingsManager.Instance != null ? SettingsManager.Instance.MasterVolume : 1f;
                volumeSlider.SetValueWithoutNotify(currentVolume * 100f);
                UpdateVolumeText(volumeSlider.value);
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }

            if (vibrationToggleButton != null)
            {
                vibrationToggleButton.onClick.RemoveAllListeners();
                vibrationToggleButton.onClick.AddListener(ToggleVibration);
            }

            if (fpsToggleButton != null)
            {
                fpsToggleButton.onClick.RemoveAllListeners();
                fpsToggleButton.onClick.AddListener(ToggleFPSDisplay);
            }

            RefreshSettingsUI();
        }

        private void OnVolumeChanged(float value)
        {
            float normalized = Mathf.Clamp01(value / 100f);
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.MasterVolume = normalized;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.MasterVolume = normalized;
                AudioManager.Instance.SaveSettings();
            }

            UpdateVolumeText(value);
            Debug.Log($"[MenuController] Master volume set to {(int)value}%");
        }

        private void ToggleVibration()
        {
            if (SettingsManager.Instance == null)
                return;

            SettingsManager.Instance.VibrationEnabled = !SettingsManager.Instance.VibrationEnabled;
            RefreshSettingsUI();
            Debug.Log($"[MenuController] Vibration: {(SettingsManager.Instance.VibrationEnabled ? "ON" : "OFF")}");
        }

        private void ToggleFPSDisplay()
        {
            if (SettingsManager.Instance == null)
                return;

            SettingsManager.Instance.PerformanceOverlayEnabled = !SettingsManager.Instance.PerformanceOverlayEnabled;
            RefreshSettingsUI();
            Debug.Log($"[MenuController] FPS Display: {(SettingsManager.Instance.PerformanceOverlayEnabled ? "ON" : "OFF")}");
        }

        private void RefreshSettingsUI()
        {
            if (SettingsManager.Instance == null)
                return;

            if (vibrationStateText != null)
            {
                vibrationStateText.text = SettingsManager.Instance.VibrationEnabled ? "ON" : "OFF";
                vibrationStateText.color = SettingsManager.Instance.VibrationEnabled ? new Color(0.2f, 0.8f, 0.3f, 1f) : new Color(1f, 0.3f, 0.3f, 1f);
            }

            if (fpsStateText != null)
            {
                fpsStateText.text = SettingsManager.Instance.PerformanceOverlayEnabled ? "ON" : "OFF";
                fpsStateText.color = SettingsManager.Instance.PerformanceOverlayEnabled ? new Color(0.2f, 0.8f, 0.3f, 1f) : new Color(1f, 0.3f, 0.3f, 1f);
            }
        }

        private void UpdateVolumeText(float sliderValue)
        {
            if (volumeValueText != null)
            {
                volumeValueText.text = $"{Mathf.RoundToInt(sliderValue)}%";
            }
        }

        private Button EnsureShopBackButton()
        {
            var panel = FindChildByName(transform, "ShopPanel");
            if (panel == null)
                return null;

            var existing = FindChildByName(panel, "BtnBack")?.GetComponent<Button>();
            if (existing != null)
            {
                ConfigureShopBackButton(existing, panel);
                return existing;
            }

            GameObject backObj = new GameObject("BtnBack");
            backObj.transform.SetParent(panel, false);

            var rect = backObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 22f);
            rect.sizeDelta = new Vector2(620f, 75f);

            var image = backObj.AddComponent<Image>();
            image.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            var button = backObj.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            colors.pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            button.colors = colors;

            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(backObj.transform, false);

            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = "VOLTAR";
            label.alignment = TMPro.TextAlignmentOptions.Center;
            label.fontSize = 32;
            label.color = Color.white;
            label.fontStyle = TMPro.FontStyles.Bold;
            label.raycastTarget = false;

            ConfigureShopBackButton(button, panel);
            AdjustShopContentForBackButton(panel);
            return button;
        }

        private void ConfigureShopBackButton(Button button, Transform panel)
        {
            if (button == null || panel == null)
                return;

            if (button.transform.parent != panel)
            {
                button.transform.SetParent(panel, false);
            }

            var rect = button.GetComponent<RectTransform>();
            if (rect == null)
                return;

            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 22f);
            rect.sizeDelta = new Vector2(620f, 75f);
            button.transform.SetAsLastSibling();
        }

        private void AdjustShopContentForBackButton(Transform panel)
        {
            if (panel == null)
                return;

            var content = FindChildByName(panel, "Content");
            if (content == null)
                return;
            if (content.parent != panel)
            {
                content.SetParent(panel, false);
            }

            var rect = content.GetComponent<RectTransform>();
            if (rect == null)
                return;

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(700f, 360f);
            rect.anchoredPosition = new Vector2(0f, 90f);
        }

        private static int ParsePriceFromButtonName(string buttonName)
        {
            // Expected format: Buy_ItemName_500
            var parts = buttonName.Split('_');
            if (parts.Length > 0 && int.TryParse(parts[parts.Length - 1], out int parsedPrice))
            {
                return parsedPrice;
            }

            return 0;
        }

        private string ParseItemTokenFromButtonName(string buttonName)
        {
            string cleaned = buttonName.Replace("Buy_", string.Empty);
            int lastUnderscore = cleaned.LastIndexOf('_');
            if (lastUnderscore > 0 && int.TryParse(cleaned.Substring(lastUnderscore + 1), out _))
            {
                return cleaned.Substring(0, lastUnderscore);
            }

            return cleaned;
        }

        private string GetUpgradeTierKey(string itemToken)
        {
            if (IsBeamToken(itemToken))
                return BeamUpgradeTierKey;
            if (IsPowerToken(itemToken))
                return PowerUpgradeTierKey;
            if (IsBatteryToken(itemToken))
                return BatteryUpgradeTierKey;
            if (IsHealthToken(itemToken))
                return HealthUpgradeTierKey;
            return null;
        }

        private int GetPriceForNextTier(string itemToken, int currentTier)
        {
            int[] prices = GetPriceTable(itemToken);
            if (prices == null || currentTier < 0 || currentTier >= prices.Length)
                return 0;

            return prices[currentTier];
        }

        private int[] GetPriceTable(string itemToken)
        {
            if (IsBeamToken(itemToken))
                return BeamTierPrices;
            if (IsPowerToken(itemToken))
                return PowerTierPrices;
            if (IsBatteryToken(itemToken))
                return BatteryTierPrices;
            if (IsHealthToken(itemToken))
                return HealthTierPrices;
            return null;
        }

        private string GetDisplayName(string itemToken)
        {
            if (IsBeamToken(itemToken))
                return "Aumento de Feixe";
            if (IsPowerToken(itemToken))
                return "Poder da Lanterna";
            if (IsBatteryToken(itemToken))
                return "Bateria Melhorada";
            if (IsHealthToken(itemToken))
                return "Vida Extra";
            return itemToken;
        }

        private bool IsBeamToken(string token)
        {
            return token.Contains("AumentodeFeixe");
        }

        private bool IsPowerToken(string token)
        {
            return token.Contains("PoderdaLanterna");
        }

        private bool IsBatteryToken(string token)
        {
            return token.Contains("BateriaMelhorada");
        }

        private bool IsHealthToken(string token)
        {
            return token.Contains("VidaExtra");
        }

        private void RefreshShopItemsUI()
        {
            if (shopPanel == null)
                return;

            var itemRoots = shopPanel.GetComponentsInChildren<Transform>(true);
            foreach (var itemRoot in itemRoots)
            {
                if (itemRoot == null || !itemRoot.name.StartsWith("Item_"))
                    continue;

                string token = itemRoot.name.Replace("Item_", string.Empty).Replace(" ", string.Empty);
                string key = GetUpgradeTierKey(token);
                if (string.IsNullOrEmpty(key))
                    continue;

                int tier = Mathf.Clamp(PlayerPrefs.GetInt(key, 0), 0, MaxUpgradeTier);
                bool isMax = tier >= MaxUpgradeTier;
                int nextPrice = isMax ? 0 : GetPriceForNextTier(token, tier);

                var itemRect = itemRoot.GetComponent<RectTransform>();
                if (itemRect != null)
                    itemRect.sizeDelta = new Vector2(620f, 90f);

                var title = itemRoot.Find("Title")?.GetComponent<TextMeshProUGUI>();
                if (title != null)
                {
                    title.fontSize = 24;
                    title.enableWordWrapping = false;
                    title.overflowMode = TextOverflowModes.Ellipsis;
                    title.text = GetDisplayName(token);

                    var titleRect = title.GetComponent<RectTransform>();
                    if (titleRect != null)
                    {
                        titleRect.anchorMin = new Vector2(0f, 0.5f);
                        titleRect.anchorMax = new Vector2(0f, 0.5f);
                        titleRect.pivot = new Vector2(0f, 0.5f);
                        titleRect.anchoredPosition = new Vector2(18f, 16f);
                        titleRect.sizeDelta = new Vector2(340f, 34f);
                    }
                }

                var desc = itemRoot.Find("Description")?.GetComponent<TextMeshProUGUI>();
                if (desc != null)
                {
                    desc.fontSize = 14;
                    desc.enableWordWrapping = false;
                    desc.overflowMode = TextOverflowModes.Ellipsis;

                    var descRect = desc.GetComponent<RectTransform>();
                    if (descRect != null)
                    {
                        descRect.anchorMin = new Vector2(0f, 0.5f);
                        descRect.anchorMax = new Vector2(0f, 0.5f);
                        descRect.pivot = new Vector2(0f, 0.5f);
                        descRect.anchoredPosition = new Vector2(18f, -16f);
                        descRect.sizeDelta = new Vector2(350f, 22f);
                    }
                }

                var price = itemRoot.Find("Price")?.GetComponent<TextMeshProUGUI>();
                if (price != null)
                {
                    price.fontSize = 22;
                    price.enableWordWrapping = false;
                    price.overflowMode = TextOverflowModes.Overflow;
                    price.text = isMax ? "MAX" : $"{nextPrice} C";

                    var priceRect = price.GetComponent<RectTransform>();
                    if (priceRect != null)
                    {
                        priceRect.anchorMin = new Vector2(1f, 0.5f);
                        priceRect.anchorMax = new Vector2(1f, 0.5f);
                        priceRect.pivot = new Vector2(1f, 0.5f);
                        priceRect.anchoredPosition = new Vector2(-148f, 2f);
                        priceRect.sizeDelta = new Vector2(130f, 50f);
                    }
                }

                var buyButton = FindBuyButtonInItem(itemRoot);
                if (buyButton != null)
                {
                    buyButton.interactable = !isMax;
                    var buyRect = buyButton.GetComponent<RectTransform>();
                    if (buyRect != null)
                    {
                        buyRect.sizeDelta = new Vector2(112f, 40f);
                        buyRect.anchoredPosition = new Vector2(-18f, 2f);
                    }

                    var label = buyButton.GetComponentInChildren<TextMeshProUGUI>(true);
                    if (label != null)
                    {
                        label.fontSize = 10;
                        label.text = isMax ? "MAX" : "COMPRAR";
                    }
                }

                EnsureTierLabel(itemRoot, tier);
            }
        }

        private Button FindBuyButtonInItem(Transform itemRoot)
        {
            var buttons = itemRoot.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                if (btn != null && btn.gameObject.name.StartsWith("Buy_"))
                    return btn;
            }

            return null;
        }

        private void EnsureTierLabel(Transform itemRoot, int tier)
        {
            Transform tierTransform = itemRoot.Find("Tier");
            TextMeshProUGUI tierText;

            if (tierTransform == null)
            {
                var tierObj = new GameObject("Tier");
                tierObj.transform.SetParent(itemRoot, false);
                var rect = tierObj.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 1f);
                rect.anchoredPosition = new Vector2(-12f, -8f);
                rect.sizeDelta = new Vector2(180f, 22f);

                tierText = tierObj.AddComponent<TextMeshProUGUI>();
                tierText.alignment = TextAlignmentOptions.Right;
                tierText.fontSize = 15;
                tierText.fontStyle = FontStyles.Bold;
                tierText.color = new Color(0.95f, 0.9f, 0.35f, 1f);
            }
            else
            {
                tierText = tierTransform.GetComponent<TextMeshProUGUI>();
            }

            if (tierText != null)
            {
                tierText.text = $"Tier {tier}/{MaxUpgradeTier}";
            }
        }

        private Button FindBackButtonInPanel(string panelName)
        {
            var panel = FindChildByName(transform, panelName);
            if (panel == null)
                return null;

            var allButtons = panel.GetComponentsInChildren<Button>(true);
            foreach (var button in allButtons)
            {
                if (button != null && button.gameObject.name == "BtnBack")
                {
                    return button;
                }
            }

            return null;
        }

        private static Transform FindChildByName(Transform root, string name)
        {
            if (root == null)
                return null;

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == name)
                    return child;
            }

            return null;
        }

        private static Transform FindChildByName(Component root, string name)
        {
            return root == null ? null : FindChildByName(root.transform, name);
        }
        
        private void PauseGameplay()
        {
            // Disable all spawners
            var spawnManager = FindAnyObjectByType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.enabled = false;
                Debug.Log("[MenuController] SpawnManager disabled");
            }
            
            // Disable all enemy controllers
            var enemies = FindObjectsByType<EnemyController>();
            foreach (var enemy in enemies)
            {
                enemy.enabled = false;
            }
            Debug.Log($"[MenuController] Disabled {enemies.Length} enemies");
            
            var batterySpawner = FindAnyObjectByType<BatteryPickupSpawner>();
            if (batterySpawner != null)
            {
                batterySpawner.enabled = false;
                Debug.Log("[MenuController] BatteryPickupSpawner disabled");
            }
            
            var coinSpawner = FindAnyObjectByType<CoinPickupSpawner>();
            if (coinSpawner != null)
            {
                coinSpawner.enabled = false;
                Debug.Log("[MenuController] CoinPickupSpawner disabled");
            }
            
            // Stop Luna movement
            var luna = FindAnyObjectByType<LunaController>();
            if (luna != null)
            {
                luna.enabled = false;
                Debug.Log("[MenuController] LunaController disabled");
            }
        }
        
        private void ResumeGameplay()
        {
            // Re-enable all systems
            var spawnManager = FindAnyObjectByType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.enabled = true;
                Debug.Log("[MenuController] SpawnManager enabled");
            }
            
            var enemies = FindObjectsByType<EnemyController>();
            foreach (var enemy in enemies)
            {
                enemy.enabled = true;
            }
            
            var batterySpawner = FindAnyObjectByType<BatteryPickupSpawner>();
            if (batterySpawner != null)
            {
                batterySpawner.enabled = true;
                Debug.Log("[MenuController] BatteryPickupSpawner enabled");
            }
            
            var coinSpawner = FindAnyObjectByType<CoinPickupSpawner>();
            if (coinSpawner != null)
            {
                coinSpawner.enabled = true;
                Debug.Log("[MenuController] CoinPickupSpawner enabled");
            }
            
            var luna = FindAnyObjectByType<LunaController>();
            if (luna != null)
            {
                luna.enabled = true;
                Debug.Log("[MenuController] LunaController enabled");
            }
            
            // Show Gameplay HUD
            GameObject gameplayCanvas = GameObject.Find("CanvasHUD");
            if (gameplayCanvas != null)
            {
                gameplayCanvas.SetActive(true);
                Debug.Log("[MenuController] Gameplay HUD shown");
            }
        }

        private void OnDestroy()
        {
            ScoreManager.onCoinsChanged -= OnCoinsChanged;
        }
    }
}
