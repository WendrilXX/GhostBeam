using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

namespace GhostBeam.Editor
{
    /// <summary>
    /// Script para montar automaticamente a hierarquia do projeto Gameplay Scene
    /// Use: Create > Empty na scene, adicione este script, execute SetupGameplayScene()
    /// </summary>
    public class HierarchyBuilder : MonoBehaviour
    {
        public static void SetupGameplayScene()
        {
            Debug.Log("========== BUILDING GAMEPLAY HIERARCHY ==========");

            // 1. Setup Camera
            SetupCamera();

            // 2. Setup Lighting
            SetupLighting();

            // 3. Setup Background
            SetupBackground();

            // 4. Setup Player (Luna)
            SetupPlayer();

            // 5. Setup Systems
            SetupSystems();

            // 6. Setup UI
            SetupUI();

            Debug.Log("========== HIERARCHY BUILD COMPLETE ==========");
        }

        private static void SetupCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                camera = cameraObj.AddComponent<Camera>();
            }

            camera.transform.position = new Vector3(0, 0, -10);
            camera.orthographic = true;
            camera.orthographicSize = 7.5f;
            camera.backgroundColor = Color.black;

            Debug.Log("✅ Camera configured");
        }

        private static void SetupLighting()
        {
            // Find or create Global Light 2D
            Light2D globalLight = FindAnyObjectByType<Light2D>();
            if (globalLight == null)
            {
                GameObject lightObj = new GameObject("Global Light 2D");
                globalLight = lightObj.AddComponent<Light2D>();
                globalLight.lightType = Light2D.LightType.Global;
            }

            globalLight.intensity = 0;
            globalLight.blendStyleIndex = 0;

            Debug.Log("✅ Global Light 2D configured");
        }

        private static void SetupBackground()
        {
            GameObject bgObj = new GameObject("Background");
            SpriteRenderer spriteRenderer = bgObj.AddComponent<SpriteRenderer>();

            Sprite square = Resources.Load<Sprite>("unity_builtin_extra");
            spriteRenderer.sprite = square;
            spriteRenderer.color = new Color(0.12f, 0.12f, 0.2f, 1f);
            spriteRenderer.sortingOrder = -10;

            bgObj.transform.localScale = new Vector3(100, 100, 1);

            Debug.Log("✅ Background created");
        }

        private static void SetupPlayer()
        {
            // Create Luna
            GameObject lunaObj = new GameObject("Luna");
            lunaObj.transform.position = Vector3.zero;

            // Sprite
            SpriteRenderer spriteRenderer = lunaObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0.78f, 0.39f, 1f, 1f);

            // Rigidbody 2D
            Rigidbody2D rb = lunaObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 1;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Circle Collider
            CircleCollider2D collider = lunaObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = false;

            // Tag
            lunaObj.tag = "Player";

            // Scripts
            lunaObj.AddComponent<Gameplay.HealthSystem>();
            lunaObj.AddComponent<Player.LunaController>();

            // Flashlight
            GameObject flashlightObj = new GameObject("Flashlight");
            flashlightObj.transform.parent = lunaObj.transform;
            flashlightObj.transform.localPosition = Vector3.zero;

            Light2D flashlight = flashlightObj.AddComponent<Light2D>();
            flashlight.lightType = (Light2D.LightType)1; // Spot type
            flashlight.intensity = 1f;
            flashlight.pointLightOuterRadius = 15f;
            flashlight.pointLightOuterAngle = 70f;
            flashlight.blendStyleIndex = 0;

            Player.FlashlightController flashlightController = lunaObj.AddComponent<Player.FlashlightController>();
            flashlightController.GetType().GetField("flashlight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(flashlightController, flashlight);

            Debug.Log("✅ Luna with Flashlight created");
        }

        private static void SetupSystems()
        {
            GameObject systemsObj = new GameObject("Systems");

            // Create GameObject para cada manager
            string[] managers = { "GameManager", "ScoreManager", "AudioManager", "SettingsManager", "SpawnManager", "BatteryPickupSpawner", "CoinPickupSpawner" };

            foreach (string manager in managers)
            {
                GameObject managerObj = new GameObject(manager);
                managerObj.transform.parent = systemsObj.transform;

                System.Type managerType = System.Type.GetType($"GhostBeam.Managers.{manager}");
                if (managerType != null)
                {
                    managerObj.AddComponent(managerType);
                }
                else
                {
                    System.Type otherType = System.Type.GetType($"GhostBeam.Items.{manager}");
                    if (otherType != null)
                        managerObj.AddComponent(otherType);
                }
            }

            // Battery System
            GameObject batteryObj = new GameObject("BatterySystem");
            batteryObj.transform.parent = systemsObj.transform;
            batteryObj.AddComponent<Gameplay.BatterySystem>();

            Debug.Log("✅ Systems created (7 managers + battery)");
        }

        private static void SetupUI()
        {
            // Canvas
            GameObject canvasObj = new GameObject("CanvasHUD");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Add UI Controller
            canvasObj.AddComponent<UI.HUDController>();
            canvasObj.AddComponent<UI.GameOverPanelController>();
            canvasObj.AddComponent<UI.UIBootstrapper>();

            // Create HUD texts
            CreateUIText(canvasObj, "TxtHealth", "❤️ 3", new Vector2(-900, 500));
            CreateUIText(canvasObj, "TxtBattery", "🔋 100%", new Vector2(-900, -500));
            CreateUIText(canvasObj, "TxtScore", "Score: 0", new Vector2(0, 500));
            CreateUIText(canvasObj, "TxtCoins", "💰 0", new Vector2(900, 500));
            CreateUIText(canvasObj, "TxtHighScore", "Best: 0", new Vector2(0, 0));
            CreateUIText(canvasObj, "TxtTime", "00:00", new Vector2(0, -500));

            // Create Button Pause
            GameObject btnPauseObj = new GameObject("BtnPause");
            RectTransform btnPauseRect = btnPauseObj.AddComponent<RectTransform>();
            btnPauseRect.anchorMin = new Vector2(1, 1);
            btnPauseRect.anchorMax = new Vector2(1, 1);
            btnPauseRect.anchoredPosition = new Vector2(-50, -50);
            btnPauseRect.sizeDelta = new Vector2(80, 80);

            Image btnPauseImg = btnPauseObj.AddComponent<Image>();
            btnPauseImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            Button btnPause = btnPauseObj.AddComponent<Button>();

            GameObject pauseTextObj = new GameObject("Text");
            RectTransform pauseTextRect = pauseTextObj.AddComponent<RectTransform>();
            pauseTextRect.anchorMin = Vector2.zero;
            pauseTextRect.anchorMax = Vector2.one;
            pauseTextRect.offsetMin = Vector2.zero;
            pauseTextRect.offsetMax = Vector2.zero;

            TextMeshProUGUI pauseText = pauseTextObj.AddComponent<TextMeshProUGUI>();
            pauseText.text = "⏸";
            pauseText.alignment = TextAlignmentOptions.Center;
            pauseText.fontSize = 50;

            pauseTextObj.transform.parent = btnPauseObj.transform;
            btnPauseObj.transform.parent = canvasObj.transform;

            Debug.Log("✅ UI Canvas with HUD texts created");
        }

        private static void CreateUIText(GameObject parent, string name, string content, Vector2 position)
        {
            GameObject textObj = new GameObject(name);
            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(300, 100);

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = 36;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            textObj.transform.parent = parent.transform;
        }
    }
}
