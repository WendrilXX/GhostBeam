using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace GhostBeam.Utilities
{
    public class ProjectValidator : MonoBehaviour
    {
        public static void ValidateProject()
        {
            Debug.Log("========== GHOST BEAM PROJECT VALIDATOR ==========");
            
            var checks = new List<(string name, bool result)>();

            // 1. Managers
            checks.Add(("GameManager Singleton", Managers.GameManager.Instance != null));
            checks.Add(("ScoreManager Singleton", Managers.ScoreManager.Instance != null));
            checks.Add(("AudioManager Singleton", Managers.AudioManager.Instance != null));
            checks.Add(("SettingsManager Singleton", Managers.SettingsManager.Instance != null));

            // 2. Player
            var luna = FindAnyObjectByType<Player.LunaController>();
            checks.Add(("Luna Present", luna != null));
            
            var flashlight = FindAnyObjectByType<Player.FlashlightController>();
            checks.Add(("Flashlight Present", flashlight != null));

            // 3. Health System
            var health = FindAnyObjectByType<Gameplay.HealthSystem>();
            checks.Add(("HealthSystem Present", health != null));

            // 4. Battery System
            var battery = FindAnyObjectByType<Gameplay.BatterySystem>();
            checks.Add(("BatterySystem Present", battery != null));

            // 5. Spawn Manager
            var spawn = FindAnyObjectByType<Managers.SpawnManager>();
            checks.Add(("SpawnManager Present", spawn != null));

            // 6. UI
            var hud = FindAnyObjectByType<UI.HUDController>();
            checks.Add(("HUDController Present", hud != null));

            var gameOver = FindAnyObjectByType<UI.GameOverPanelController>();
            checks.Add(("GameOverPanelController Present", gameOver != null));

            // 7. Camera
            var camera = Camera.main;
            checks.Add(("Main Camera", camera != null));
            if (camera != null)
                checks.Add(("Camera Orthographic", camera.orthographic));

            // 8. Lighting
            var globalLight = FindAnyObjectByType<Light2D>(FindObjectsInactive.Exclude);
            checks.Add(("Global Light 2D", globalLight != null));

            // 9. Tags
            checks.Add(("Player Tag Exists", UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0].CompareTag("Player") || HasTag("Player")));

            // Print results
            int passed = 0;
            int failed = 0;

            foreach (var (name, result) in checks)
            {
                string icon = result ? "✅" : "❌";
                Debug.Log($"{icon} {name}: {(result ? "OK" : "MISSING")}");
                if (result) passed++;
                else failed++;
            }

            Debug.Log($"\n========== RESULTS: {passed} Passed, {failed} Failed ==========");
            Debug.Log(failed == 0 ? "✅ PROJECT READY FOR PRODUCTION!" : "⚠️ SOME ISSUES DETECTED - CHECK ABOVE");
        }

        private static bool HasTag(string tag)
        {
            try
            {
                var rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (var obj in rootObjs)
                {
                    if (obj.CompareTag(tag))
                        return true;
                }
            }
            catch { }
            return false;
        }
    }
}
