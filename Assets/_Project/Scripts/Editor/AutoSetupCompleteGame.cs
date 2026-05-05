using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GhostBeam.Editor
{
    /// <summary>
    /// COMPLETE GAME SETUP - Creates Gameplay.unity and MainMenu.unity with ALL systems
    /// Includes: 5 enemy types, upgrades, shop, pause, performance overlay, pooling, mobile UI
    /// </summary>
    public class AutoSetupCompleteGame
    {
        private const string GAMEPLAY_SCENE_PATH = "Assets/_Project/Scenes/Gameplay.unity";
        private const string MAINMENU_SCENE_PATH = "Assets/_Project/Scenes/MainMenu.unity";
        
        // Sprites
        private const string PENADO_SPRITE_PATH = "Assets/_Project/Art/Sprites/Penado/penado_direita.png";
        private const string FUNDO_SPRITE_PATH = "Assets/_Project/Art/Sprites/fundo.png";
        private const string LUNA_SPRITE_PATH = "Assets/_Project/Art/Sprites/Lunatopdown.png";

        [MenuItem("GhostBeam/Setup/5. Complete Setup (Gameplay + Menu) - FULL")]
        public static void SetupCompleteGame()
        {
            try
            {
                Debug.Log("========== COMPLETE GAME SETUP (FULL) ==========");
                
                // Clean up any existing AudioListeners to avoid duplicates
                RemoveDuplicateAudioListeners();
                
                Debug.Log("Creating Gameplay.unity with ALL systems...");

                SetupGameplayScene();
                Debug.Log("SUCCESS: Gameplay scene created at " + GAMEPLAY_SCENE_PATH);

                // Reload Gameplay to ensure all saves are complete
                Debug.Log("Reloading Gameplay.unity to ensure all saves are persisted...");
                EditorSceneManager.OpenScene(GAMEPLAY_SCENE_PATH, OpenSceneMode.Single);
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), GAMEPLAY_SCENE_PATH);
                Debug.Log("Gameplay.unity fully persisted");

                Debug.Log("\nCreating MainMenu.unity with shop and settings...");
                SetupMainMenuScene();
                Debug.Log("SUCCESS: MainMenu scene created at " + MAINMENU_SCENE_PATH);

                Debug.Log("\n========== COMPLETE SETUP DONE ==========");
                Debug.Log("Game is ready! Next steps:");
                Debug.Log("1. Click Play to test Gameplay.unity");
                Debug.Log("2. Open MainMenu.unity to test menu");
                Debug.Log("\nFeatures:");
                Debug.Log("  * Luna player with movement (WSAD) and aiming (mouse)");
                Debug.Log("  * Flashlight that rotates toward aim");
                Debug.Log("  * 5 managers: GameManager, AudioManager, SettingsManager, ScoreManager, SpawnManager");
                Debug.Log("  * 3 prefabs: Enemy, BatteryPickup, CoinPickup");
                Debug.Log("  * HUD displaying Score, Coins, Health, Battery, Best, Time");
                Debug.Log("  * Game Over panel");
                Debug.Log("  * Pause system");
                Debug.Log("  * Mobile virtual joystick support");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("ERROR in SetupCompleteGame: " + ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        [MenuItem("GhostBeam/Setup/6. Update MainMenu UI Only (No Duplicates)")]
        public static void UpdateMainMenuUIOnly()
        {
            try
            {
                Debug.Log("========== UPDATING MAINMENU UI ONLY ==========");
                
                // Load MainMenu scene
                var scene = EditorSceneManager.OpenScene(MAINMENU_SCENE_PATH, OpenSceneMode.Single);
                Debug.Log("Opened MainMenu.unity");

                // Remove old MenuCanvas if it exists
                GameObject oldCanvas = GameObject.Find("MenuCanvas");
                if (oldCanvas != null)
                {
                    Object.DestroyImmediate(oldCanvas);
                    Debug.Log("Removed old MenuCanvas");
                }

                // Create new UI
                SetupMainMenuUI();
                Debug.Log("Created new MainMenu UI (premium version)");

                // Save scene
                EditorSceneManager.SaveScene(scene, MAINMENU_SCENE_PATH);
                Debug.Log("✅ MainMenu scene updated and saved!");
                Debug.Log("========== UI UPDATE COMPLETE ==========");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("ERROR in UpdateMainMenuUIOnly: " + ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        private static void RemoveDuplicateAudioListeners()
        {
            try
            {
                var allListeners = GameObject.FindObjectsByType<AudioListener>();
                if (allListeners.Length > 1)
                {
                    Debug.Log($"Found {allListeners.Length} AudioListeners, removing duplicates...");
                    for (int i = 1; i < allListeners.Length; i++)
                    {
                        Object.DestroyImmediate(allListeners[i]);
                        Debug.Log("Removed duplicate AudioListener");
                    }
                }
            }
            catch { }
        }

        private static Scene ClearOrCreateScene(string scenePath)
        {
            // Check if scene file already exists
            if (System.IO.File.Exists(scenePath))
            {
                // Load existing scene and clear all GameObjects
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var rootGameObjects = scene.GetRootGameObjects();
                foreach (var go in rootGameObjects)
                {
                    Object.DestroyImmediate(go);
                }
                Debug.Log($"Cleared existing scene: {scenePath}");
                return scene;
            }
            else
            {
                // Create new scene if it doesn't exist
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                Debug.Log($"Created new scene: {scenePath}");
                return newScene;
            }
        }

        #region GAMEPLAY SCENE

        private static void SetupGameplayScene()
        {
            try
            {
                var scene = ClearOrCreateScene(GAMEPLAY_SCENE_PATH);
                Debug.Log("  [1/7] Scene cleared/created");

                SetupGameplayCamera();
                Debug.Log("  [2/7] Camera setup");

                SetupGameplayLighting();
                Debug.Log("  [3/7] Lighting setup");

                SetupGameplayBackground();
                Debug.Log("  [4/7] Background setup");

                SetupGameplayLuna();
                Debug.Log("  [5/7] Luna player setup");

                SetupGameplayManagers();
                Debug.Log("  [6/7] Managers setup");

                SetupGameplayUI();
                Debug.Log("  [7/7] UI setup");

                // Save ONLY at the end
                EditorSceneManager.SaveScene(scene, GAMEPLAY_SCENE_PATH);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("ERROR in SetupGameplayScene: " + ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        private static void SetupGameplayCamera()
        {
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            cameraObj.transform.position = new Vector3(0, 0, -10);

            Camera camera = cameraObj.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7.5f;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 1000f;
            camera.backgroundColor = Color.black;

            cameraObj.AddComponent<AudioListener>();
        }

        private static void SetupGameplayLighting()
        {
            GameObject lightObj = new GameObject("Global Light 2D");

            var light = lightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            light.lightType = (UnityEngine.Rendering.Universal.Light2D.LightType)0; // Global
            // Keep a tiny ambient fallback so scene never turns completely black if cone light fails.
            light.intensity = 0.03f;
            light.blendStyleIndex = 0;

            // If URP is not active, Light2D will not render as expected.
            var rp = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            if (rp == null || !rp.GetType().Name.Contains("UniversalRenderPipelineAsset"))
            {
                Debug.LogWarning("[AutoSetupCompleteGame] URP nao esta ativo. Light2D/lanterna pode nao funcionar. Verifique Project Settings > Graphics > Render Pipeline Asset (URP 2D).");
            }
        }

        private static void SetupGameplayBackground()
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.position = Vector3.zero;
            bgObj.transform.localScale = new Vector3(100, 100, 1);

            var spriteRenderer = bgObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = -10;

            Sprite fundoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(FUNDO_SPRITE_PATH);
            if (fundoSprite != null)
            {
                spriteRenderer.sprite = fundoSprite;
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = new Color(30f / 255f, 30f / 255f, 50f / 255f, 1f);
            }
        }

        private static void SetupGameplayLuna()
        {
            GameObject lunaObj = new GameObject("Luna");
            lunaObj.tag = "Player";
            lunaObj.transform.position = Vector3.zero;
            lunaObj.transform.localScale = Vector3.one;

            // Sprite
            var spriteRenderer = lunaObj.AddComponent<SpriteRenderer>();
            Sprite lunaSprite = AssetDatabase.LoadAssetAtPath<Sprite>(LUNA_SPRITE_PATH);
            if (lunaSprite != null)
            {
                spriteRenderer.sprite = lunaSprite;
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = new Color(200f / 255f, 100f / 255f, 1f, 1f);
            }

            // Physics
            var rb = lunaObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 1;
            rb.gravityScale = 0;
            rb.linearDamping = 0;
            rb.angularDamping = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = lunaObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;

            // Scripts
            lunaObj.AddComponent<Player.LunaController>();
            lunaObj.AddComponent<Gameplay.HealthSystem>();
            lunaObj.AddComponent<Gameplay.BatterySystem>();
            lunaObj.AddComponent<Player.FlashlightController>();

            // Flashlight
            GameObject flashlightObj = new GameObject("Flashlight");
            flashlightObj.transform.parent = lunaObj.transform;
            flashlightObj.transform.localPosition = Vector3.zero;

            var light2d = flashlightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            light2d.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Point;
            light2d.intensity = 1.9f;
            light2d.pointLightInnerRadius = 1.2f;
            light2d.pointLightOuterRadius = 14f;
            light2d.pointLightInnerAngle = 22f;
            light2d.pointLightOuterAngle = 60f;
            light2d.blendStyleIndex = 0;
            light2d.color = Color.white;

            var flashlightController = lunaObj.GetComponent<Player.FlashlightController>();
            if (flashlightController != null)
            {
                var serialized = new SerializedObject(flashlightController);
                serialized.FindProperty("flashlight").objectReferenceValue = light2d;
                serialized.ApplyModifiedProperties();
            }
        }

        private static void SetupGameplayManagers()
        {
            // Global Singletons (persist across scenes)
            CreateManagerIfMissing("GameManager", typeof(Managers.GameManager));
            CreateManagerIfMissing("AudioManager", typeof(Managers.AudioManager));
            CreateManagerIfMissing("SettingsManager", typeof(Managers.SettingsManager));

            // Scene Managers
            CreateManagerIfMissing("ScoreManager", typeof(Managers.ScoreManager));
            CreateManagerIfMissing("SpawnManager", typeof(Managers.SpawnManager));

            // Item Spawners
            CreateManagerIfMissing("BatteryPickupSpawner", typeof(Items.BatteryPickupSpawner));
            CreateManagerIfMissing("CoinPickupSpawner", typeof(Items.CoinPickupSpawner));

            // Create ALL prefabs
            CreateAllPrefabs();

            // Link prefabs to spawners
            LinkPrefabsToSpawners();

            // UI Systems
            CreateUIManagersInScene();
        }

        private static void CreateManagerIfMissing(string name, System.Type scriptType)
        {
            GameObject existing = GameObject.Find(name);
            if (existing != null)
                return;

            GameObject obj = new GameObject(name);
            obj.transform.position = Vector3.zero;

            if (obj.GetComponent(scriptType) == null)
                obj.AddComponent(scriptType);
        }

        private static void CreateAllPrefabs()
        {
            // Enemy prefab (will use Penado sprite by default)
            CreateEnemyPrefab();

            // Pickup prefabs
            CreateBatteryPickupPrefab();
            CreateCoinPickupPrefab();
        }

        private static void CreateEnemyPrefab()
        {
            string prefabPath = "Assets/_Project/Prefabs/Enemy.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
                return; // Already exists

            GameObject enemyObj = new GameObject("Enemy");
            enemyObj.transform.position = new Vector3(5, 5, 0);
            enemyObj.transform.localScale = new Vector3(0.8f, 0.8f, 1);

            var spriteRenderer = enemyObj.AddComponent<SpriteRenderer>();
            Sprite penadoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PENADO_SPRITE_PATH);
            if (penadoSprite != null)
            {
                spriteRenderer.sprite = penadoSprite;
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = new Color(100f / 255f, 255f / 255f, 100f / 255f, 1f);
            }

            var rb = enemyObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 1;
            rb.gravityScale = 0;
            rb.linearDamping = 0;
            rb.angularDamping = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            var collider = enemyObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;

            enemyObj.AddComponent<Enemy.EnemyController>();

            PrefabUtility.SaveAsPrefabAsset(enemyObj, prefabPath);
            Object.DestroyImmediate(enemyObj);
        }

        private static void CreateBatteryPickupPrefab()
        {
            string prefabPath = "Assets/_Project/Prefabs/BatteryPickup.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
                return;

            GameObject pickupObj = new GameObject("BatteryPickup");
            pickupObj.transform.localScale = new Vector3(0.4f, 0.4f, 1);

            var spriteRenderer = pickupObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(255f / 255f, 255f / 255f, 0f / 255f, 1f); // Yellow

            var collider = pickupObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;

            var rb = pickupObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;

            pickupObj.AddComponent<Items.BatteryPickup>();
            pickupObj.AddComponent<Utilities.PooledObject>();

            PrefabUtility.SaveAsPrefabAsset(pickupObj, prefabPath);
            Object.DestroyImmediate(pickupObj);
        }

        private static void CreateCoinPickupPrefab()
        {
            string prefabPath = "Assets/_Project/Prefabs/CoinPickup.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
                return;

            GameObject pickupObj = new GameObject("CoinPickup");
            pickupObj.transform.localScale = new Vector3(0.25f, 0.25f, 1);

            var spriteRenderer = pickupObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(255f / 255f, 200f / 255f, 0f / 255f, 1f); // Orange

            var collider = pickupObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;

            var rb = pickupObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;

            pickupObj.AddComponent<Items.CoinPickup>();
            pickupObj.AddComponent<Utilities.PooledObject>();

            PrefabUtility.SaveAsPrefabAsset(pickupObj, prefabPath);
            Object.DestroyImmediate(pickupObj);
        }

        private static void LinkPrefabsToSpawners()
        {
            try
            {
                // Link Enemy prefab to SpawnManager
                GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Enemy.prefab");
                GameObject spawnManagerObj = GameObject.Find("SpawnManager");
                
                if (enemyPrefab != null && spawnManagerObj != null)
                {
                    var spawnManager = spawnManagerObj.GetComponent<Managers.SpawnManager>();
                    if (spawnManager != null)
                    {
                        var serialized = new SerializedObject(spawnManager);
                        serialized.FindProperty("enemyPrefab").objectReferenceValue = enemyPrefab;
                        serialized.ApplyModifiedProperties();
                    }
                }

                // Link BatteryPickup prefab to BatteryPickupSpawner
                GameObject batteryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/BatteryPickup.prefab");
                GameObject batterySpawnerObj = GameObject.Find("BatteryPickupSpawner");
                
                if (batteryPrefab != null && batterySpawnerObj != null)
                {
                    var batterySpawner = batterySpawnerObj.GetComponent<Items.BatteryPickupSpawner>();
                    if (batterySpawner != null)
                    {
                        var serialized = new SerializedObject(batterySpawner);
                        serialized.FindProperty("batteryPickupPrefab").objectReferenceValue = batteryPrefab;
                        serialized.ApplyModifiedProperties();
                    }
                }

                // Link CoinPickup prefab to CoinPickupSpawner
                GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/CoinPickup.prefab");
                GameObject coinSpawnerObj = GameObject.Find("CoinPickupSpawner");
                
                if (coinPrefab != null && coinSpawnerObj != null)
                {
                    var coinSpawner = coinSpawnerObj.GetComponent<Items.CoinPickupSpawner>();
                    if (coinSpawner != null)
                    {
                        var serialized = new SerializedObject(coinSpawner);
                        serialized.FindProperty("coinPickupPrefab").objectReferenceValue = coinPrefab;
                        serialized.ApplyModifiedProperties();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("WARNING in LinkPrefabsToSpawners: " + ex.Message);
            }
        }

        private static void CreateUIManagersInScene()
        {
            // PauseSystem on a new object
            GameObject pauseObj = new GameObject("PauseSystem");
            pauseObj.AddComponent<Utilities.PauseSystem>();
        }

        private static void SetupGameplayUI()
        {
            try
            {
                GameObject canvasObj = new GameObject("CanvasHUD");

                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);

                canvasObj.AddComponent<GraphicRaycaster>();

                // HUD Controller
                var hudController = canvasObj.AddComponent<UI.HUDController>();
                canvasObj.AddComponent<UI.GameOverPanelController>();
                canvasObj.AddComponent<UI.UIBootstrapper>();

                // Create UI text elements - REORGANIZED FOR MOBILE WITH ANCHORS
                // TOP LEFT
                CreateUIText(canvasObj, "TxtHealth", new Vector2(20, -20), "Health: 100", TextAnchor.UpperLeft);
                CreateUIText(canvasObj, "TxtBattery", new Vector2(20, -70), "Battery: 100%", TextAnchor.UpperLeft);
                
                // TOP RIGHT
                CreateUIText(canvasObj, "TxtScore", new Vector2(-20, -20), "Score: 0", TextAnchor.UpperRight);
                CreateUIText(canvasObj, "TxtCoins", new Vector2(-20, -70), "Coins: 0", TextAnchor.UpperRight);
                CreateUIText(canvasObj, "TxtHighScore", new Vector2(-20, -120), "Best: 0", TextAnchor.UpperRight);
                
                // BOTTOM CENTER
                CreateUIText(canvasObj, "TxtTime", new Vector2(0, 20), "00:00", TextAnchor.LowerCenter);

                // Link HUD references - SAFE VERSION
                if (hudController != null)
                {
                    var serialized = new SerializedObject(hudController);
                    AssignUIFieldSafe(serialized, canvasObj, "TxtHealth", "txtHealth");
                    AssignUIFieldSafe(serialized, canvasObj, "TxtBattery", "txtBattery");
                    AssignUIFieldSafe(serialized, canvasObj, "TxtScore", "txtScore");
                    AssignUIFieldSafe(serialized, canvasObj, "TxtCoins", "txtCoins");
                    AssignUIFieldSafe(serialized, canvasObj, "TxtHighScore", "txtHighScore");
                    AssignUIFieldSafe(serialized, canvasObj, "TxtTime", "txtTime");
                    serialized.ApplyModifiedProperties();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("ERROR in SetupGameplayUI: " + ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        private static void CreateUIText(GameObject parent, string name, Vector2 position, string defaultText = "", TextAnchor anchor = TextAnchor.UpperCenter)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.parent = parent.transform;

            var rect = textObj.AddComponent<RectTransform>();
            
            // Set anchors based on alignment
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = new Vector2(0, 1);
                    rect.pivot = new Vector2(0, 1);
                    break;
                case TextAnchor.UpperRight:
                    rect.anchorMin = new Vector2(1, 1);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(1, 1);
                    break;
                case TextAnchor.LowerCenter:
                    rect.anchorMin = new Vector2(0.5f, 0);
                    rect.anchorMax = new Vector2(0.5f, 0);
                    rect.pivot = new Vector2(0.5f, 0);
                    break;
                default:
                    rect.anchorMin = new Vector2(0.5f, 1);
                    rect.anchorMax = new Vector2(0.5f, 1);
                    rect.pivot = new Vector2(0.5f, 1);
                    break;
            }
            
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(300, 100);

            var textComponent = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            textComponent.text = string.IsNullOrEmpty(defaultText) ? name : defaultText;
            textComponent.alignment = TMPro.TextAlignmentOptions.Center;
            textComponent.fontSize = 36;
            textComponent.color = Color.white;
        }

        private static void AssignUIFieldSafe(SerializedObject serialized, GameObject parent, string childName, string fieldName)
        {
            try
            {
                Transform child = parent.transform.Find(childName);
                if (child != null)
                {
                    var text = child.GetComponent<TMPro.TextMeshProUGUI>();
                    if (text != null)
                    {
                        var prop = serialized.FindProperty(fieldName);
                        if (prop != null)
                        {
                            prop.objectReferenceValue = text;
                        }
                    }
                }
            }
            catch { }
        }

        private static void SetupMobileJoystick()
        {
            // Create a hidden mobile UI container (for virtual joystick)
            // This is just a placeholder - actual mobile input is handled in LunaController
            GameObject mobileUIObj = new GameObject("MobileUI");
            var canvas = mobileUIObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = mobileUIObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            // Joystick visualization (left side)
            GameObject joystickLeft = new GameObject("JoystickLeft");
            joystickLeft.transform.parent = mobileUIObj.transform;
            var jlRect = joystickLeft.AddComponent<RectTransform>();
            jlRect.anchorMin = new Vector2(0, 0);
            jlRect.anchorMax = new Vector2(0, 0);
            jlRect.anchoredPosition = new Vector2(150, 150);
            jlRect.sizeDelta = new Vector2(300, 300);

            var jlImg = joystickLeft.AddComponent<Image>();
            jlImg.color = new Color(1, 1, 1, 0.2f);

            // Joystick visualization (right side)
            GameObject joystickRight = new GameObject("JoystickRight");
            joystickRight.transform.parent = mobileUIObj.transform;
            var jrRect = joystickRight.AddComponent<RectTransform>();
            jrRect.anchorMin = new Vector2(1, 0);
            jrRect.anchorMax = new Vector2(1, 0);
            jrRect.anchoredPosition = new Vector2(-150, 150);
            jrRect.sizeDelta = new Vector2(300, 300);

            var jrImg = joystickRight.AddComponent<Image>();
            jrImg.color = new Color(1, 1, 1, 0.2f);
        }

        #endregion

        #region MAINMENU SCENE

        private static void SetupMainMenuScene()
        {
            try
            {
                var scene = ClearOrCreateScene(MAINMENU_SCENE_PATH);
                Debug.Log("  [1/4] Scene cleared/created");

                SetupMenuCamera();
                Debug.Log("  [2/4] Camera setup");

                SetupMenuBackground();
                Debug.Log("  [3/4] Background setup");

                SetupMainMenuUI();
                Debug.Log("  [4/4] Menu UI setup");

                // Save ONLY at the end
                EditorSceneManager.SaveScene(scene, MAINMENU_SCENE_PATH);
                Debug.Log("  MainMenu saved successfully");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("ERROR in SetupMainMenuScene: " + ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        private static void SetupMenuCamera()
        {
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            cameraObj.transform.position = new Vector3(0, 0, -10);

            Camera camera = cameraObj.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 7.5f;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 1000f;
            camera.backgroundColor = Color.black;

            // AudioListener should only be in Gameplay, not MainMenu
        }

        private static void SetupMenuBackground()
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.position = Vector3.zero;
            bgObj.transform.localScale = new Vector3(100, 100, 1);

            var spriteRenderer = bgObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(30f / 255f, 30f / 255f, 50f / 255f, 1f);
            spriteRenderer.sortingOrder = -10;

            Sprite fundoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(FUNDO_SPRITE_PATH);
            if (fundoSprite != null)
            {
                spriteRenderer.sprite = fundoSprite;
                spriteRenderer.color = Color.white;
            }
        }

        private static void SetupMainMenuUI()
        {
            GameObject canvasObj = new GameObject("MenuCanvas");

            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();

            // Background gradient effect
            GameObject bgObj = new GameObject("MenuBackground");
            bgObj.transform.parent = canvasObj.transform;
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(15f / 255f, 20f / 255f, 40f / 255f, 1f); // Dark blue
            bgImage.raycastTarget = true;  // BLOCK clicks to prevent gameplay from receiving input

            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.parent = canvasObj.transform;
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -150);
            titleRect.sizeDelta = new Vector2(800, 200);
            
            var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = "GHOST BEAM";
            titleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            titleTxt.fontSize = 100;
            titleTxt.color = new Color(0.9f, 0.7f, 1f, 1f); // Purple-ish
            titleTxt.fontStyle = TMPro.FontStyles.Bold;

            // Subtitle
            GameObject subtitleObj = new GameObject("Subtitle");
            subtitleObj.transform.parent = canvasObj.transform;
            var subtitleRect = subtitleObj.AddComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0.5f, 1f);
            subtitleRect.anchorMax = new Vector2(0.5f, 1f);
            subtitleRect.anchoredPosition = new Vector2(0, -270);
            subtitleRect.sizeDelta = new Vector2(600, 80);
            
            var subtitleTxt = subtitleObj.AddComponent<TMPro.TextMeshProUGUI>();
            subtitleTxt.text = "Sobreviva no Escuro";
            subtitleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            subtitleTxt.fontSize = 36;
            subtitleTxt.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);

            // Main menu container - Optimized for mobile landscape
            GameObject mainMenuContainer = new GameObject("MainMenuContainer");
            mainMenuContainer.transform.parent = canvasObj.transform;
            var containerRect = mainMenuContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(600, 420);  // Reduced from 800x500 for mobile fit
            containerRect.anchoredPosition = new Vector2(0, 20);  // Shifted down slightly

            // Container layout with responsive spacing
            var vlg = mainMenuContainer.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.spacing = 18f;  // Reduced from 25
            vlg.padding = new RectOffset(20, 20, 20, 20);  // Added padding

            // Main menu buttons
            CreatePremiumMenuButton(mainMenuContainer, "BtnPlay", "JOGAR", new Color(0.2f, 0.8f, 0.3f, 1f));
            CreatePremiumMenuButton(mainMenuContainer, "BtnShop", "LOJA", new Color(1f, 0.8f, 0.2f, 1f));
            CreatePremiumMenuButton(mainMenuContainer, "BtnSettings", "CONFIGURAÇÕES", new Color(0.3f, 0.6f, 1f, 1f));
            CreatePremiumMenuButton(mainMenuContainer, "BtnQuit", "SAIR", new Color(1f, 0.3f, 0.3f, 1f));

            // Add MenuController script for button handling
            canvasObj.AddComponent<UI.MenuController>();

            // Shop panel (hidden initially)
            CreateShopPanel(canvasObj);

            // Settings panel (hidden initially)
            CreateSettingsPanel(canvasObj);
        }

        private static void CreatePremiumMenuButton(GameObject parent, string name, string label, Color buttonColor)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.parent = parent.transform;

            var rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600, 75);  // Reduced from 800x90 - more proportional for mobile

            var layoutElement = btnObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 75f;  // Match height
            layoutElement.preferredWidth = -1f;   // Use layout

            // Button background with gradient effect
            var image = btnObj.AddComponent<Image>();
            image.color = buttonColor;

            // Button shadow
            var shadow = btnObj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(5, -5);

            var button = btnObj.AddComponent<Button>();
            button.interactable = true;

            // Color transition on hover
            var colors = button.colors;
            colors.normalColor = buttonColor;
            colors.highlightedColor = buttonColor * 1.2f;
            colors.pressedColor = buttonColor * 0.8f;
            colors.selectedColor = buttonColor;
            button.colors = colors;

            // NOTE: Button listeners are added by MenuController at runtime

            // Text label - Properly centered with padding
            GameObject textObj = new GameObject("Label");
            textObj.transform.parent = btnObj.transform;
            textObj.transform.localPosition = Vector3.zero;

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);  // Padding inside button
            textRect.offsetMax = new Vector2(-10, -5);

            var textComponent = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            textComponent.text = label;
            textComponent.alignment = TMPro.TextAlignmentOptions.Center;
            textComponent.fontSize = 32;  // Reduced from 48 - better proportion
            textComponent.color = Color.white;
            textComponent.fontStyle = TMPro.FontStyles.Bold;
            textComponent.raycastTarget = false;
            textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
        }

        private static void CreateShopPanel(GameObject parent)
        {
            GameObject shopPanel = new GameObject("ShopPanel");
            shopPanel.transform.SetParent(parent.transform, false);
            shopPanel.SetActive(false); // Hidden initially

            var rect = shopPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = shopPanel.AddComponent<Image>();
            image.color = new Color(10f / 255f, 15f / 255f, 30f / 255f, 0.98f);

            // Scroll content - optimized for mobile
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(shopPanel.transform, false);
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(700, 500);  // Reduced from 900x600
            contentRect.anchoredPosition = Vector2.zero;

            var cvlg = contentObj.AddComponent<VerticalLayoutGroup>();
            cvlg.childForceExpandHeight = false;
            cvlg.childForceExpandWidth = false;
            cvlg.childAlignment = TextAnchor.UpperCenter;
            cvlg.spacing = 14f;
            cvlg.padding = new RectOffset(40, 40, 30, 30);

            // Shop title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(contentObj.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(620, 80);
            var titleLE = titleObj.AddComponent<LayoutElement>();
            titleLE.preferredHeight = 80f;
            titleLE.preferredWidth = 620f;
            var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = "LOJA DE UPGRADES";
            titleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            titleTxt.fontSize = 60;
            titleTxt.color = new Color(1f, 0.8f, 0.2f, 1f);
            titleTxt.fontStyle = TMPro.FontStyles.Bold;

            // Coins display
            GameObject coinsObj = new GameObject("ShopCoinsText");
            coinsObj.transform.SetParent(contentObj.transform, false);
            var coinsRect = coinsObj.AddComponent<RectTransform>();
            coinsRect.sizeDelta = new Vector2(620, 36);
            var coinsLE = coinsObj.AddComponent<LayoutElement>();
            coinsLE.preferredHeight = 36f;
            coinsLE.preferredWidth = 620f;
            var coinsTxt = coinsObj.AddComponent<TMPro.TextMeshProUGUI>();
            coinsTxt.text = "Moedas: 0";
            coinsTxt.alignment = TMPro.TextAlignmentOptions.Right;
            coinsTxt.fontSize = 24;
            coinsTxt.color = new Color(0.25f, 0.95f, 0.45f, 1f);
            coinsTxt.fontStyle = TMPro.FontStyles.Bold;

            // Upgrade items
            CreateShopItem(contentObj, "Aumento de Feixe", "Amplie o alcance de detecção", 500);
            CreateShopItem(contentObj, "Poder da Lanterna", "Aumente a intensidade da luz", 750);
            CreateShopItem(contentObj, "Bateria Melhorada", "Aumente a capacidade de bateria", 1000);
            CreateShopItem(contentObj, "Vida Extra", "Aumente a vida máxima", 150);

            // Feedback text
            GameObject feedbackObj = new GameObject("ShopFeedbackText");
            feedbackObj.transform.SetParent(contentObj.transform, false);
            var feedbackRect = feedbackObj.AddComponent<RectTransform>();
            feedbackRect.sizeDelta = new Vector2(620, 40);
            var feedbackLE = feedbackObj.AddComponent<LayoutElement>();
            feedbackLE.preferredHeight = 40f;
            var feedbackTxt = feedbackObj.AddComponent<TMPro.TextMeshProUGUI>();
            feedbackTxt.text = "";
            feedbackTxt.alignment = TMPro.TextAlignmentOptions.Center;
            feedbackTxt.fontSize = 22;
            feedbackTxt.color = new Color(0.85f, 0.9f, 1f, 0.95f);
            feedbackTxt.fontStyle = TMPro.FontStyles.Bold;

            // Back button at bottom
            CreatePremiumMenuButton(contentObj, "BtnBack", "VOLTAR", new Color(0.5f, 0.5f, 0.5f, 1f));
        }

        private static void CreateShopItem(GameObject parent, string title, string description, int price)
        {
            GameObject itemObj = new GameObject("Item_" + title);
            itemObj.transform.SetParent(parent.transform, false);

            var rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(620, 110);
            var le = itemObj.AddComponent<LayoutElement>();
            le.preferredHeight = 110f;
            le.preferredWidth = 620f;

            var bgImage = itemObj.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.2f, 0.35f, 1f);

            // Border effect
            var outline = itemObj.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 0.8f, 0.2f, 0.5f);
            outline.effectDistance = new Vector2(3, 3);

            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(itemObj.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.5f);
            titleRect.anchorMax = new Vector2(0, 0.5f);
            titleRect.pivot = new Vector2(0, 0.5f);
            titleRect.anchoredPosition = new Vector2(18, 20);
            titleRect.sizeDelta = new Vector2(340, 40);
            var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = title;
            titleTxt.alignment = TMPro.TextAlignmentOptions.Left;
            titleTxt.fontSize = 28;  // Reduced from 40
            titleTxt.color = Color.white;
            titleTxt.fontStyle = TMPro.FontStyles.Bold;

            // Description
            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(itemObj.transform, false);
            var descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.5f);
            descRect.anchorMax = new Vector2(0, 0.5f);
            descRect.pivot = new Vector2(0, 0.5f);
            descRect.anchoredPosition = new Vector2(18, -20);
            descRect.sizeDelta = new Vector2(350, 25);
            var descTxt = descObj.AddComponent<TMPro.TextMeshProUGUI>();
            descTxt.text = description;
            descTxt.alignment = TMPro.TextAlignmentOptions.Left;
            descTxt.fontSize = 18;  // Reduced from 24
            descTxt.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);

            // Price
            GameObject priceObj = new GameObject("Price");
            priceObj.transform.SetParent(itemObj.transform, false);
            var priceRect = priceObj.AddComponent<RectTransform>();
            priceRect.anchorMin = new Vector2(1f, 0.5f);
            priceRect.anchorMax = new Vector2(1f, 0.5f);
            priceRect.pivot = new Vector2(1f, 0.5f);
            priceRect.anchoredPosition = new Vector2(-148, 0);
            priceRect.sizeDelta = new Vector2(130, 50);
            var priceTxt = priceObj.AddComponent<TMPro.TextMeshProUGUI>();
            priceTxt.text = price + " C";
            priceTxt.alignment = TMPro.TextAlignmentOptions.Right;
            priceTxt.fontSize = 28;  // Reduced from 36
            priceTxt.color = new Color(0.2f, 0.8f, 0.3f, 1f);
            priceTxt.fontStyle = TMPro.FontStyles.Bold;

            // Buy button
            string itemKey = title.Replace(" ", string.Empty);
            GameObject buyBtnObj = new GameObject("Buy_" + itemKey + "_" + price);
            buyBtnObj.transform.SetParent(itemObj.transform, false);
            var buyRect = buyBtnObj.AddComponent<RectTransform>();
            buyRect.anchorMin = new Vector2(1f, 0.5f);
            buyRect.anchorMax = new Vector2(1f, 0.5f);
            buyRect.pivot = new Vector2(1f, 0.5f);
            buyRect.anchoredPosition = new Vector2(-18, 0);
            buyRect.sizeDelta = new Vector2(118, 46);

            var buyImage = buyBtnObj.AddComponent<Image>();
            buyImage.color = new Color(0.3f, 0.65f, 1f, 1f);
            var buyButton = buyBtnObj.AddComponent<Button>();
            var buyColors = buyButton.colors;
            buyColors.normalColor = buyImage.color;
            buyColors.highlightedColor = new Color(0.4f, 0.75f, 1f, 1f);
            buyColors.pressedColor = new Color(0.2f, 0.55f, 0.9f, 1f);
            buyButton.colors = buyColors;

            GameObject buyLabelObj = new GameObject("Label");
            buyLabelObj.transform.SetParent(buyBtnObj.transform, false);
            var buyLabelRect = buyLabelObj.AddComponent<RectTransform>();
            buyLabelRect.anchorMin = Vector2.zero;
            buyLabelRect.anchorMax = Vector2.one;
            buyLabelRect.offsetMin = Vector2.zero;
            buyLabelRect.offsetMax = Vector2.zero;
            var buyLabel = buyLabelObj.AddComponent<TMPro.TextMeshProUGUI>();
            buyLabel.text = "COMPRAR";
            buyLabel.fontSize = 11;
            buyLabel.alignment = TMPro.TextAlignmentOptions.Center;
            buyLabel.color = Color.white;
            buyLabel.fontStyle = TMPro.FontStyles.Bold;
            buyLabel.raycastTarget = false;

            GameObject tierObj = new GameObject("Tier");
            tierObj.transform.SetParent(itemObj.transform, false);
            var tierRect = tierObj.AddComponent<RectTransform>();
            tierRect.anchorMin = new Vector2(1f, 1f);
            tierRect.anchorMax = new Vector2(1f, 1f);
            tierRect.pivot = new Vector2(1f, 1f);
            tierRect.anchoredPosition = new Vector2(-12f, -8f);
            tierRect.sizeDelta = new Vector2(180f, 22f);
            var tierTxt = tierObj.AddComponent<TMPro.TextMeshProUGUI>();
            tierTxt.text = "Tier 0/3";
            tierTxt.fontSize = 15;
            tierTxt.alignment = TMPro.TextAlignmentOptions.Right;
            tierTxt.color = new Color(0.95f, 0.9f, 0.35f, 1f);
            tierTxt.fontStyle = TMPro.FontStyles.Bold;
        }

        private static void CreateSettingsPanel(GameObject parent)
        {
            GameObject settingsPanel = new GameObject("SettingsPanel");
            settingsPanel.transform.SetParent(parent.transform, false);
            settingsPanel.SetActive(false); // Hidden initially

            var rect = settingsPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = settingsPanel.AddComponent<Image>();
            image.color = new Color(10f / 255f, 15f / 255f, 30f / 255f, 0.98f);

            // Settings content - optimized for mobile
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(settingsPanel.transform, false);
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(700, 500);  // Reduced from 900x600
            contentRect.anchoredPosition = Vector2.zero;

            var vlg = contentObj.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.spacing = 16f;
            vlg.padding = new RectOffset(40, 40, 35, 35);

            // Settings title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(contentObj.transform, false);
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(700, 80);
            var titleLE = titleObj.AddComponent<LayoutElement>();
            titleLE.preferredHeight = 80f;
            var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = "CONFIGURAÇÕES";
            titleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            titleTxt.fontSize = 60;
            titleTxt.color = new Color(0.3f, 0.6f, 1f, 1f);
            titleTxt.fontStyle = TMPro.FontStyles.Bold;

            // Volume control
            CreateSettingItem(contentObj, "MasterVolume", "Volume", "0%", "100%");

            // Vibration toggle
            CreateSettingToggle(contentObj, "Vibracao", "Vibração", "ON");

            // Performance overlay
            CreateSettingToggle(contentObj, "FPSDisplay", "FPS Display", "ON");

            // Back button
            CreatePremiumMenuButton(contentObj, "BtnBack", "VOLTAR", new Color(0.5f, 0.5f, 0.5f, 1f));
        }

        private static void CreateSettingItem(GameObject parent, string key, string label, string minValue, string maxValue)
        {
            GameObject itemObj = new GameObject("Setting_" + key);
            itemObj.transform.SetParent(parent.transform, false);

            var rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(620, 76);
            var le = itemObj.AddComponent<LayoutElement>();
            le.preferredHeight = 76f;
            le.preferredWidth = 620f;

            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(itemObj.transform, false);
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0, 0.5f);
            labelRect.anchoredPosition = new Vector2(20, 0);
            labelRect.sizeDelta = new Vector2(170, 50);
            var labelTxt = labelObj.AddComponent<TMPro.TextMeshProUGUI>();
            labelTxt.text = label;
            labelTxt.fontSize = 28;  // Reduced from 40
            labelTxt.color = Color.white;
            labelTxt.fontStyle = TMPro.FontStyles.Bold;

            // Slider background - reduced size
            GameObject sliderBgObj = new GameObject(key + "Slider");
            sliderBgObj.transform.SetParent(itemObj.transform, false);
            var sliderBgRect = sliderBgObj.AddComponent<RectTransform>();
            sliderBgRect.anchorMin = new Vector2(0.5f, 0.5f);
            sliderBgRect.anchorMax = new Vector2(0.5f, 0.5f);
            sliderBgRect.sizeDelta = new Vector2(240, 30);
            sliderBgRect.anchoredPosition = Vector2.zero;
            var sliderBgImg = sliderBgObj.AddComponent<Image>();
            sliderBgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            // Slider
            var slider = sliderBgObj.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 50;

            GameObject fillAreaObj = new GameObject("FillArea");
            fillAreaObj.transform.SetParent(sliderBgObj.transform, false);
            var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0);
            fillAreaRect.anchorMax = new Vector2(1, 1);
            fillAreaRect.offsetMin = new Vector2(5, 5);
            fillAreaRect.offsetMax = new Vector2(-5, -5);

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.25f, 0.75f, 1f, 1f);

            GameObject handleAreaObj = new GameObject("HandleSlideArea");
            handleAreaObj.transform.SetParent(sliderBgObj.transform, false);
            var handleAreaRect = handleAreaObj.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);
            handleAreaRect.offsetMin = Vector2.zero;
            handleAreaRect.offsetMax = Vector2.zero;

            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleAreaObj.transform, false);
            var handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(22, 34);
            var handleImage = handleObj.AddComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0.95f);

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;

            // Min/Max labels
            GameObject minObj = new GameObject("Min");
            minObj.transform.SetParent(itemObj.transform, false);
            var minRect = minObj.AddComponent<RectTransform>();
            minRect.anchorMin = new Vector2(0.5f, 0.5f);
            minRect.anchorMax = new Vector2(0.5f, 0.5f);
            minRect.anchoredPosition = new Vector2(-150, 0);
            minRect.sizeDelta = new Vector2(50, 40);
            var minTxt = minObj.AddComponent<TMPro.TextMeshProUGUI>();
            minTxt.text = minValue;
            minTxt.fontSize = 24;
            minTxt.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            GameObject maxObj = new GameObject("Max");
            maxObj.transform.SetParent(itemObj.transform, false);
            var maxRect = maxObj.AddComponent<RectTransform>();
            maxRect.anchorMin = new Vector2(0.5f, 0.5f);
            maxRect.anchorMax = new Vector2(0.5f, 0.5f);
            maxRect.anchoredPosition = new Vector2(150, 0);
            maxRect.sizeDelta = new Vector2(60, 40);
            var maxTxt = maxObj.AddComponent<TMPro.TextMeshProUGUI>();
            maxTxt.text = maxValue;
            maxTxt.fontSize = 24;
            maxTxt.alignment = TMPro.TextAlignmentOptions.Left;
            maxTxt.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            GameObject valueObj = new GameObject(key + "Value");
            valueObj.transform.SetParent(itemObj.transform, false);
            var valueRect = valueObj.AddComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(1f, 0.5f);
            valueRect.anchorMax = new Vector2(1f, 0.5f);
            valueRect.anchoredPosition = new Vector2(-20, 0);
            valueRect.sizeDelta = new Vector2(90, 40);
            var valueTxt = valueObj.AddComponent<TMPro.TextMeshProUGUI>();
            valueTxt.text = "50%";
            valueTxt.fontSize = 24;
            valueTxt.alignment = TMPro.TextAlignmentOptions.Right;
            valueTxt.color = Color.white;
        }

        private static void CreateSettingToggle(GameObject parent, string key, string label, string state)
        {
            GameObject toggleObj = new GameObject("Toggle_" + key);
            toggleObj.transform.SetParent(parent.transform, false);

            var rect = toggleObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(620, 72);
            var le = toggleObj.AddComponent<LayoutElement>();
            le.preferredHeight = 72f;
            le.preferredWidth = 620f;

            var bgImage = toggleObj.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.2f, 0.35f, 1f);

            var button = toggleObj.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = bgImage.color;
            colors.highlightedColor = new Color(0.2f, 0.25f, 0.45f, 1f);
            colors.pressedColor = new Color(0.1f, 0.15f, 0.3f, 1f);
            button.colors = colors;

            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(toggleObj.transform, false);
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0, 0.5f);
            labelRect.anchoredPosition = new Vector2(20, 0);
            labelRect.sizeDelta = new Vector2(340, 50);
            var labelTxt = labelObj.AddComponent<TMPro.TextMeshProUGUI>();
            labelTxt.text = label;
            labelTxt.fontSize = 28;  // Reduced from 40
            labelTxt.color = Color.white;
            labelTxt.fontStyle = TMPro.FontStyles.Bold;

            // State indicator
            GameObject stateObj = new GameObject(key + "State");
            stateObj.transform.SetParent(toggleObj.transform, false);
            var stateRect = stateObj.AddComponent<RectTransform>();
            stateRect.anchorMin = new Vector2(1f, 0.5f);
            stateRect.anchorMax = new Vector2(1f, 0.5f);
            stateRect.anchoredPosition = new Vector2(-40, 0);
            stateRect.sizeDelta = new Vector2(80, 50);  // Reduced
            var stateTxt = stateObj.AddComponent<TMPro.TextMeshProUGUI>();
            stateTxt.text = state == "ON" ? "ON" : "OFF";  // Changed from symbols to text (font compatibility)
            stateTxt.alignment = TMPro.TextAlignmentOptions.Right;
            stateTxt.fontSize = 24;  // Reduced from 50
            stateTxt.color = state == "ON" ? new Color(0.2f, 0.8f, 0.3f, 1f) : new Color(1f, 0.3f, 0.3f, 1f);
            stateTxt.fontStyle = TMPro.FontStyles.Bold;
        }

        #endregion
    }
}
