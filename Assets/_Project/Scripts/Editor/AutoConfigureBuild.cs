using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GhostBeam.Editor
{
    public class AutoConfigureBuild
    {
        [MenuItem("GhostBeam/Advanced/4. Auto-Configure Build Settings")]
        public static void ConfigureBuildSettings()
        {
            Debug.Log("========== CONFIGURING BUILD SETTINGS ==========");

            // 1. Add scenes to build
            SetupScenes();

            // 2. Configure Player Settings
            ConfigurePlayerSettings();

            // 3. Configure Quality Settings
            ConfigureQualitySettings();

            Debug.Log("========== BUILD SETTINGS CONFIGURED ==========");
        }

        private static void SetupScenes()
        {
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[2];
            
            // Scene 0: MainMenu
            scenes[0] = new EditorBuildSettingsScene("Assets/_Project/Scenes/MainMenu.unity", true);
            
            // Scene 1: Gameplay
            scenes[1] = new EditorBuildSettingsScene("Assets/_Project/Scenes/Gameplay.unity", true);

            EditorBuildSettings.scenes = scenes;
            Debug.Log("✅ Scenes added to Build Settings (MainMenu, Gameplay)");
        }

        private static void ConfigurePlayerSettings()
        {
            // Company Name
            PlayerSettings.companyName = "GhostBeam";
            
            // Product Name
            PlayerSettings.productName = "GhostBeam";

            // Resolution
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;

            // Orientation - Landscape only
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            // Target Frame Rate
            QualitySettings.vSyncCount = 1;

            Debug.Log("✅ Player Settings configured (1920x1080, Landscape, 60 FPS)");
        }

        private static void ConfigureQualitySettings()
        {
            // Set all quality levels to 60 FPS target
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.vSyncCount = 1; // Enable V-Sync
            }

            Debug.Log("✅ Quality Settings configured (V-Sync enabled)");
        }

        [MenuItem("GhostBeam/Advanced/Quick Build Check")]
        public static void QuickBuildCheck()
        {
            Debug.Log("========== BUILD CHECK ==========");
            
            var scenes = EditorBuildSettings.scenes;
            Debug.Log($"Scenes in Build: {scenes.Length}");
            foreach (var scene in scenes)
            {
                Debug.Log($"  - {scene.path}");
            }

            Debug.Log($"Resolution: {PlayerSettings.defaultScreenWidth}x{PlayerSettings.defaultScreenHeight}");
            Debug.Log($"Company: {PlayerSettings.companyName}");
            Debug.Log($"Product: {PlayerSettings.productName}");
            Debug.Log($"Target FPS: {QualitySettings.vSyncCount} (V-Sync)");
        }
    }
}
