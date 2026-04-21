using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace GhostBeam.Editor
{
    public class PrefabBuilder
    {
        [MenuItem("GhostBeam/Setup/3. Create All Prefabs")]
        public static void CreateAllPrefabs()
        {
            Debug.Log("========== CREATING PREFABS ==========");

            // Create Prefabs folder if needed
            if (!System.IO.Directory.Exists("Assets/_Project/Prefabs"))
                System.IO.Directory.CreateDirectory("Assets/_Project/Prefabs");

            CreateEnemyPrefab();
            CreateBatteryPickupPrefab();
            CreateCoinPickupPrefab();

            AssetDatabase.Refresh();
            Debug.Log("========== ALL PREFABS CREATED ==========");
        }

        private static void CreateEnemyPrefab()
        {
            GameObject enemyObj = new GameObject("Enemy");
            
            // Sprite Renderer - Red
            SpriteRenderer spriteRenderer = enemyObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1f, 0f, 0f, 1f); // Red

            // Circle Collider
            CircleCollider2D collider = enemyObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;

            // Enemy Controller Script
            enemyObj.AddComponent<Enemy.EnemyController>();

            // Save as prefab
            string prefabPath = "Assets/_Project/Prefabs/Enemy.prefab";
            PrefabUtility.SaveAsPrefabAsset(enemyObj, prefabPath);
            Object.DestroyImmediate(enemyObj);
            Debug.Log("✅ Enemy.prefab created!");
        }

        private static void CreateBatteryPickupPrefab()
        {
            GameObject batteryObj = new GameObject("BatteryPickup");
            
            // Sprite Renderer - Yellow
            SpriteRenderer spriteRenderer = batteryObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1f, 1f, 0f, 1f); // Yellow

            // Circle Collider (Trigger)
            CircleCollider2D collider = batteryObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            collider.isTrigger = true;

            // Battery Pickup Script
            batteryObj.AddComponent<Items.BatteryPickup>();

            // Save as prefab
            string prefabPath = "Assets/_Project/Prefabs/BatteryPickup.prefab";
            PrefabUtility.SaveAsPrefabAsset(batteryObj, prefabPath);
            Object.DestroyImmediate(batteryObj);
            Debug.Log("✅ BatteryPickup.prefab created!");
        }

        private static void CreateCoinPickupPrefab()
        {
            GameObject coinObj = new GameObject("CoinPickup");
            
            // Sprite Renderer - Orange
            SpriteRenderer spriteRenderer = coinObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1f, 0.65f, 0f, 1f); // Orange

            // Circle Collider (Trigger)
            CircleCollider2D collider = coinObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;
            collider.isTrigger = true;

            // Coin Pickup Script
            coinObj.AddComponent<Items.CoinPickup>();

            // Save as prefab
            string prefabPath = "Assets/_Project/Prefabs/CoinPickup.prefab";
            PrefabUtility.SaveAsPrefabAsset(coinObj, prefabPath);
            Object.DestroyImmediate(coinObj);
            Debug.Log("✅ CoinPickup.prefab created!");
        }
    }
}
