using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GhostBeam.Editor
{
    public class AutoLinkReferences
    {
        [MenuItem("GhostBeam/Advanced/3. Auto-Link All References")]
        public static void LinkAllReferences()
        {
            Debug.Log("========== LINKING ALL REFERENCES ==========");

            // Link Spawner Prefabs
            LinkSpawnerPrefabs();

            // Link HUD References
            LinkHUDReferences();

            // Link Flashlight
            LinkFlashlightReference();

            Debug.Log("========== ALL REFERENCES LINKED ==========");
        }

        private static void LinkSpawnerPrefabs()
        {
            // Find SpawnManager
            var spawnManager = Object.FindAnyObjectByType<Managers.SpawnManager>();
            if (spawnManager != null)
            {
                GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Enemy.prefab");
                if (enemyPrefab != null)
                {
                    var serialized = new SerializedObject(spawnManager);
                    serialized.FindProperty("enemyPrefab").objectReferenceValue = enemyPrefab;
                    serialized.ApplyModifiedProperties();
                    Debug.Log("✅ SpawnManager: Enemy prefab linked");
                }
            }

            // Find BatteryPickupSpawner
            var batterySpawner = Object.FindAnyObjectByType<Items.BatteryPickupSpawner>();
            if (batterySpawner != null)
            {
                GameObject batteryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/BatteryPickup.prefab");
                if (batteryPrefab != null)
                {
                    var serialized = new SerializedObject(batterySpawner);
                    serialized.FindProperty("batteryPickupPrefab").objectReferenceValue = batteryPrefab;
                    serialized.ApplyModifiedProperties();
                    Debug.Log("✅ BatteryPickupSpawner: Battery prefab linked");
                }
            }

            // Find CoinPickupSpawner
            var coinSpawner = Object.FindAnyObjectByType<Items.CoinPickupSpawner>();
            if (coinSpawner != null)
            {
                GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/CoinPickup.prefab");
                if (coinPrefab != null)
                {
                    var serialized = new SerializedObject(coinSpawner);
                    serialized.FindProperty("coinPickupPrefab").objectReferenceValue = coinPrefab;
                    serialized.ApplyModifiedProperties();
                    Debug.Log("✅ CoinPickupSpawner: Coin prefab linked");
                }
            }
        }

        private static void LinkHUDReferences()
        {
            var hudController = Object.FindAnyObjectByType<UI.HUDController>();
            if (hudController == null)
            {
                Debug.LogWarning("❌ HUDController not found!");
                return;
            }

            var canvasObj = hudController.gameObject;
            var serialized = new SerializedObject(hudController);

            LinkUITextField(serialized, canvasObj, "TxtHealth", "txtHealth");
            LinkUITextField(serialized, canvasObj, "TxtBattery", "txtBattery");
            LinkUITextField(serialized, canvasObj, "TxtScore", "txtScore");
            LinkUITextField(serialized, canvasObj, "TxtCoins", "txtCoins");
            LinkUITextField(serialized, canvasObj, "TxtHighScore", "txtHighScore");
            LinkUITextField(serialized, canvasObj, "TxtTime", "txtTime");

            serialized.ApplyModifiedProperties();
            Debug.Log("✅ HUDController: All text fields linked");
        }

        private static void LinkUITextField(SerializedObject serialized, GameObject canvas, string childName, string fieldName)
        {
            Transform child = canvas.transform.Find(childName);
            if (child != null)
            {
                var text = child.GetComponent<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    serialized.FindProperty(fieldName).objectReferenceValue = text;
                }
            }
        }

        private static void LinkFlashlightReference()
        {
            var lunaController = Object.FindAnyObjectByType<Player.LunaController>();
            if (lunaController != null)
            {
                var flashlightObj = lunaController.transform.Find("Flashlight");
                if (flashlightObj != null)
                {
                    var light2d = flashlightObj.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
                    if (light2d != null)
                    {
                        var flashlightController = lunaController.GetComponent<Player.FlashlightController>();
                        if (flashlightController != null)
                        {
                            var serialized = new SerializedObject(flashlightController);
                            serialized.FindProperty("flashlight").objectReferenceValue = light2d;
                            serialized.ApplyModifiedProperties();
                            Debug.Log("✅ FlashlightController: Light2D reference linked");
                        }
                    }
                }
            }
        }
    }
}
