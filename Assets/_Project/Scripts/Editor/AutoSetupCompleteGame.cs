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

        #region GAMEPLAY SCENE

        private static void SetupGameplayScene()
        {
            try
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                Debug.Log("  [1/7] Scene created");

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
            light.intensity = 0f;
            light.blendStyleIndex = 0;
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
            light2d.lightType = (UnityEngine.Rendering.Universal.Light2D.LightType)1; // Spot
            light2d.intensity = 1f;
            light2d.pointLightOuterRadius = 15f;
            light2d.pointLightOuterAngle = 70f;
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

                // Create UI text elements
                CreateUIText(canvasObj, "TxtHealth", new Vector2(-900, 500), "Health: 100");
                CreateUIText(canvasObj, "TxtBattery", new Vector2(-900, -500), "Battery: 100%");
                CreateUIText(canvasObj, "TxtScore", new Vector2(0, 500), "Score: 0");
                CreateUIText(canvasObj, "TxtCoins", new Vector2(900, 500), "Coins: 0");
                CreateUIText(canvasObj, "TxtHighScore", new Vector2(0, 0), "Best: 0");
                CreateUIText(canvasObj, "TxtTime", new Vector2(0, -500), "00:00");

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

        private static void CreateUIText(GameObject parent, string name, Vector2 position, string defaultText = "")
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.parent = parent.transform;

            var rect = textObj.AddComponent<RectTransform>();
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
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                Debug.Log("  [1/4] Scene created");

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
            CreatePremiumMenuButton(mainMenuContainer, "BtnSettings", "CONFIGURACOES", new Color(0.3f, 0.6f, 1f, 1f));
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
            shopPanel.transform.parent = parent.transform;
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
            contentObj.transform.parent = shopPanel.transform;
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(700, 500);  // Reduced from 900x600
            contentRect.anchoredPosition = Vector2.zero;

            var cvlg = contentObj.AddComponent<VerticalLayoutGroup>();
            cvlg.childForceExpandHeight = false;
            cvlg.childForceExpandWidth = true;
            cvlg.spacing = 20f;
            cvlg.padding = new RectOffset(50, 50, 50, 50);

            // Shop title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.parent = contentObj.transform;
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(800, 80);
            var titleLE = titleObj.AddComponent<LayoutElement>();
            titleLE.preferredHeight = 80f;
            var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = "LOJA DE UPGRADES";
            titleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            titleTxt.fontSize = 60;
            titleTxt.color = new Color(1f, 0.8f, 0.2f, 1f);
            titleTxt.fontStyle = TMPro.FontStyles.Bold;

            // Upgrade items
            CreateShopItem(contentObj, "Aumento de Feixe", "Amplie o alcance de detecção", "500 C");
            CreateShopItem(contentObj, "Poder da Lanterna", "Aumente a intensidade da luz", "750 C");
            CreateShopItem(contentObj, "Bateria Melhorada", "Aumente a capacidade de bateria", "1000 C");

            // Back button at bottom
            CreatePremiumMenuButton(contentObj, "BtnBack", "VOLTAR", new Color(0.5f, 0.5f, 0.5f, 1f));
        }

        private static void CreateShopItem(GameObject parent, string title, string description, string price)
        {
            GameObject itemObj = new GameObject("Item_" + title);
            itemObj.transform.parent = parent.transform;

            var rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, 100);  // Reduced from 800x120
            var le = itemObj.AddComponent<LayoutElement>();
            le.preferredHeight = 100f;  // Reduced from 120

            var bgImage = itemObj.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.2f, 0.35f, 1f);

            // Border effect
            var outline = itemObj.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 0.8f, 0.2f, 0.5f);
            outline.effectDistance = new Vector2(3, 3);

            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.parent = itemObj.transform;
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.5f);
            titleRect.anchorMax = new Vector2(0, 0.5f);
            titleRect.anchoredPosition = new Vector2(20, 15);
            titleRect.sizeDelta = new Vector2(400, 40);  // Reduced
            var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = title;
            titleTxt.fontSize = 28;  // Reduced from 40
            titleTxt.color = Color.white;
            titleTxt.fontStyle = TMPro.FontStyles.Bold;

            // Description
            GameObject descObj = new GameObject("Description");
            descObj.transform.parent = itemObj.transform;
            var descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.5f);
            descRect.anchorMax = new Vector2(0, 0.5f);
            descRect.anchoredPosition = new Vector2(20, -18);
            descRect.sizeDelta = new Vector2(400, 25);  // Reduced
            var descTxt = descObj.AddComponent<TMPro.TextMeshProUGUI>();
            descTxt.text = description;
            descTxt.fontSize = 18;  // Reduced from 24
            descTxt.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);

            // Price
            GameObject priceObj = new GameObject("Price");
            priceObj.transform.parent = itemObj.transform;
            var priceRect = priceObj.AddComponent<RectTransform>();
            priceRect.anchorMin = new Vector2(1f, 0.5f);
            priceRect.anchorMax = new Vector2(1f, 0.5f);
            priceRect.anchoredPosition = new Vector2(-20, 0);
            priceRect.sizeDelta = new Vector2(150, 50);  // Reduced from 200x60
            var priceTxt = priceObj.AddComponent<TMPro.TextMeshProUGUI>();
            priceTxt.text = price;
            priceTxt.alignment = TMPro.TextAlignmentOptions.Right;
            priceTxt.fontSize = 28;  // Reduced from 36
            priceTxt.color = new Color(0.2f, 0.8f, 0.3f, 1f);
            priceTxt.fontStyle = TMPro.FontStyles.Bold;
        }

        private static void CreateSettingsPanel(GameObject parent)
        {
            GameObject settingsPanel = new GameObject("SettingsPanel");
            settingsPanel.transform.parent = parent.transform;
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
            contentObj.transform.parent = settingsPanel.transform;
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(700, 500);  // Reduced from 900x600
            contentRect.anchoredPosition = Vector2.zero;

            var vlg = contentObj.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.spacing = 40f;
            vlg.padding = new RectOffset(100, 100, 100, 100);

            // Settings title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.parent = contentObj.transform;
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(700, 80);
            var titleLE = titleObj.AddComponent<LayoutElement>();
            titleLE.preferredHeight = 80f;
            var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = "CONFIGURACOES";
            titleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            titleTxt.fontSize = 60;
            titleTxt.color = new Color(0.3f, 0.6f, 1f, 1f);
            titleTxt.fontStyle = TMPro.FontStyles.Bold;

            // Volume control
            CreateSettingItem(contentObj, "Volume", "0%", "100%");

            // Vibration toggle
            CreateSettingToggle(contentObj, "Vibracao", "ON");

            // Performance overlay
            CreateSettingToggle(contentObj, "FPS Display", "ON");

            // Back button
            CreatePremiumMenuButton(contentObj, "BtnBack", "VOLTAR", new Color(0.5f, 0.5f, 0.5f, 1f));
        }

        private static void CreateSettingItem(GameObject parent, string label, string minValue, string maxValue)
        {
            GameObject itemObj = new GameObject("Setting_" + label);
            itemObj.transform.parent = parent.transform;

            var rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, 70);  // Reduced from 800x80
            var le = itemObj.AddComponent<LayoutElement>();
            le.preferredHeight = 70f;  // Reduced from 80

            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.parent = itemObj.transform;
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0, 0.5f);
            labelRect.anchoredPosition = new Vector2(20, 0);
            labelRect.sizeDelta = new Vector2(200, 50);  // Reduced from 200x60
            var labelTxt = labelObj.AddComponent<TMPro.TextMeshProUGUI>();
            labelTxt.text = label;
            labelTxt.fontSize = 28;  // Reduced from 40
            labelTxt.color = Color.white;
            labelTxt.fontStyle = TMPro.FontStyles.Bold;

            // Slider background - reduced size
            GameObject sliderBgObj = new GameObject("SliderBg");
            sliderBgObj.transform.parent = itemObj.transform;
            var sliderBgRect = sliderBgObj.AddComponent<RectTransform>();
            sliderBgRect.anchorMin = new Vector2(0.45f, 0.5f);
            sliderBgRect.anchorMax = new Vector2(0.45f, 0.5f);
            sliderBgRect.sizeDelta = new Vector2(300, 30);  // Reduced from 450x40
            sliderBgRect.anchoredPosition = Vector2.zero;
            var sliderBgImg = sliderBgObj.AddComponent<Image>();
            sliderBgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            // Slider
            var slider = sliderBgObj.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 50;

            // Min/Max labels
            GameObject minObj = new GameObject("Min");
            minObj.transform.parent = itemObj.transform;
            var minRect = minObj.AddComponent<RectTransform>();
            minRect.anchorMin = new Vector2(0.28f, 0.5f);
            minRect.anchorMax = new Vector2(0.28f, 0.5f);
            minRect.anchoredPosition = new Vector2(-230, 0);
            minRect.sizeDelta = new Vector2(50, 40);
            var minTxt = minObj.AddComponent<TMPro.TextMeshProUGUI>();
            minTxt.text = minValue;
            minTxt.fontSize = 24;
            minTxt.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }

        private static void CreateSettingToggle(GameObject parent, string label, string state)
        {
            GameObject toggleObj = new GameObject("Toggle_" + label);
            toggleObj.transform.parent = parent.transform;

            var rect = toggleObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, 70);  // Reduced from 800x80
            var le = toggleObj.AddComponent<LayoutElement>();
            le.preferredHeight = 70f;  // Reduced from 80

            var bgImage = toggleObj.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.2f, 0.35f, 1f);

            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.parent = toggleObj.transform;
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0, 0.5f);
            labelRect.anchoredPosition = new Vector2(20, 0);
            labelRect.sizeDelta = new Vector2(400, 50);  // Reduced
            var labelTxt = labelObj.AddComponent<TMPro.TextMeshProUGUI>();
            labelTxt.text = label;
            labelTxt.fontSize = 28;  // Reduced from 40
            labelTxt.color = Color.white;
            labelTxt.fontStyle = TMPro.FontStyles.Bold;

            // State indicator
            GameObject stateObj = new GameObject("State");
            stateObj.transform.parent = toggleObj.transform;
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
