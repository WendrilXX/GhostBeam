using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
            
            // Find all UI elements
            shopPanel = GameObject.Find("ShopPanel");
            settingsPanel = GameObject.Find("SettingsPanel");
            
            btnPlay = GameObject.Find("BtnPlay")?.GetComponent<Button>();
            btnShop = GameObject.Find("BtnShop")?.GetComponent<Button>();
            btnSettings = GameObject.Find("BtnSettings")?.GetComponent<Button>();
            btnQuit = GameObject.Find("BtnQuit")?.GetComponent<Button>();

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
            if (shopPanel != null) shopPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            
            // Add back button listeners from panels
            Button backBtnShop = GameObject.Find("ShopPanel/Content/BtnBack")?.GetComponent<Button>();
            if (backBtnShop != null)
            {
                backBtnShop.onClick.RemoveAllListeners();
                backBtnShop.onClick.AddListener(OnBackClick);
                Debug.Log("[MenuController] Shop back button listener registered");
            }
            
            Button backBtnSettings = GameObject.Find("SettingsPanel/Content/BtnBack")?.GetComponent<Button>();
            if (backBtnSettings != null)
            {
                backBtnSettings.onClick.RemoveAllListeners();
                backBtnSettings.onClick.AddListener(OnBackClick);
                Debug.Log("[MenuController] Settings back button listener registered");
            }
            
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
                Debug.Log("[MenuController] Shop panel shown");
            }
        }

        private void OnSettingsClick()
        {
            Debug.Log("[MenuController] *** SETTINGS BUTTON CLICKED ***");
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
                if (shopPanel != null) shopPanel.SetActive(false);
                Debug.Log("[MenuController] Settings panel shown");
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
            Debug.Log("[MenuController] Returned to main menu");
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
            var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
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
            
            var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
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
    }
}
