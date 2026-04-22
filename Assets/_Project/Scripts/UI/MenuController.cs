using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GhostBeam.Managers;
using GhostBeam.Player;
using GhostBeam.Enemy;
using GhostBeam.Items;

namespace GhostBeam.UI
{
    /// <summary>
    /// Runtime menu controller - handles button clicks and panel visibility
    /// </summary>
    public class MenuController : MonoBehaviour
    {
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
            Button backBtnShop = FindChildByName(transform, "ShopPanel")?.GetComponentInChildren<Button>(true);
            if (backBtnShop != null && backBtnShop.gameObject.name != "BtnBack")
            {
                var allButtons = FindChildByName(transform, "ShopPanel")?.GetComponentsInChildren<Button>(true);
                if (allButtons != null)
                {
                    foreach (var button in allButtons)
                    {
                        if (button != null && button.gameObject.name == "BtnBack")
                        {
                            backBtnShop = button;
                            break;
                        }
                    }
                }
            }
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
            
            Debug.Log("[MenuController] Setup complete - buttons ready for clicks");
        }
        
        private void Update()
        {
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
        }

        private void OnPlayClick()
        {
            Debug.Log("[MenuController] *** PLAY BUTTON CLICKED ***");
            isMenuActive = false;
            ResumeGameplay();  // Re-enable all systems before loading
            Debug.Log("[MenuController] Loading Gameplay scene...");
            SceneManager.LoadScene("Assets/_Project/Scenes/Gameplay.unity", LoadSceneMode.Single);
        }

        private void OnShopClick()
        {
            Debug.Log("[MenuController] *** SHOP BUTTON CLICKED ***");
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                if (settingsPanel != null) settingsPanel.SetActive(false);
                if (mainMenuContainer != null) mainMenuContainer.SetActive(false);
                UpdateShopCoinsText(ScoreManager.Instance != null ? ScoreManager.Instance.Coins : 0);
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

                int price = ParsePriceFromButtonName(button.gameObject.name);
                string itemName = button.gameObject.name.Replace("Buy_", string.Empty);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnBuyItemClick(itemName, price));
                hooked++;
            }

            Debug.Log($"[MenuController] Shop item listeners registered: {hooked}");
        }

        private void OnBuyItemClick(string itemName, int price)
        {
            if (ScoreManager.Instance == null)
            {
                Debug.LogWarning("[MenuController] ScoreManager not found. Cannot purchase.");
                ShowShopFeedback("Sistema de moedas indisponivel", new Color(1f, 0.45f, 0.45f, 1f));
                return;
            }

            if (ScoreManager.Instance.TrySpendCoins(price))
            {
                Debug.Log($"[MenuController] Purchase successful: {itemName} ({price} coins)");
                ShowShopFeedback($"Compra realizada: {itemName.Replace("_", " ")}", new Color(0.25f, 0.95f, 0.45f, 1f));
            }
            else
            {
                Debug.Log($"[MenuController] Not enough coins for {itemName}. Need {price}, have {ScoreManager.Instance.Coins}");
                ShowShopFeedback("Moedas insuficientes", new Color(1f, 0.45f, 0.45f, 1f));
            }

            UpdateShopCoinsText(ScoreManager.Instance.Coins);
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
