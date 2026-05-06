using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GhostBeam.Editor
{
    public class AutoCreatePrefabsAdvanced
    {
        [MenuItem("GhostBeam/Advanced/2. Auto-Create All Prefabs (Exact Specs)")]
        public static void CreateAllPrefabs()
        {
            Debug.Log("========== CREATING PREFABS WITH EXACT SPECIFICATIONS ==========");

            // Create Prefabs folder if needed
            if (!System.IO.Directory.Exists("Assets/_Project/Prefabs"))
                System.IO.Directory.CreateDirectory("Assets/_Project/Prefabs");

            CreateEnemyPrefab();
            CreateBatteryPickupPrefab();
            CreateCoinPickupPrefab();

            AssetDatabase.Refresh();
            Debug.Log("========== ALL PREFABS CREATED WITH EXACT SPECS ==========");
        }

        private static void CreateEnemyPrefab()
        {
            GameObject enemyObj = new GameObject("Enemy");
            enemyObj.transform.position = new Vector3(5, 5, 0);
            enemyObj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            // Sprite Renderer - White (no tint - sprites have correct colors)
            SpriteRenderer spriteRenderer = enemyObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = Color.white; // No tint - use sprite colors directly
            spriteRenderer.sortingOrder = 0;

            // Rigidbody 2D
            Rigidbody2D rb = enemyObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 1;
            rb.gravityScale = 0;
            rb.linearDamping = 0;
            rb.angularDamping = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Circle Collider - Trigger
            CircleCollider2D collider = enemyObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            collider.isTrigger = true;

            // Enemy Controller Script
            var enemyController = enemyObj.AddComponent<Enemy.EnemyController>();

            // Save as prefab
            string prefabPath = "Assets/_Project/Prefabs/Enemy.prefab";
            PrefabUtility.SaveAsPrefabAsset(enemyObj, prefabPath);
            Object.DestroyImmediate(enemyObj);
            Debug.Log("✅ Enemy.prefab created! (Green 100,255,100 | Scale 0.8x0.8 | Collider: 0.4)");
        }

        private static void CreateBatteryPickupPrefab()
        {
            GameObject batteryObj = new GameObject("BatteryPickup");
            batteryObj.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

            // Sprite Renderer - Yellow (255, 255, 0)
            SpriteRenderer spriteRenderer = batteryObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1f, 1f, 0f, 1f); // Amarelo
            spriteRenderer.sortingOrder = 0;

            // Circle Collider - Trigger
            CircleCollider2D collider = batteryObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.2f;
            collider.isTrigger = true;

            // Battery Pickup Script
            var batteryPickup = batteryObj.AddComponent<Items.BatteryPickup>();

            // Save as prefab
            string prefabPath = "Assets/_Project/Prefabs/BatteryPickup.prefab";
            PrefabUtility.SaveAsPrefabAsset(batteryObj, prefabPath);
            Object.DestroyImmediate(batteryObj);
            Debug.Log("✅ BatteryPickup.prefab created! (Yellow 255,255,0 | Scale 0.4x0.4 | Collider: 0.2)");
        }

        private static void CreateCoinPickupPrefab()
        {
            GameObject coinObj = new GameObject("CoinPickup");
            coinObj.transform.localScale = new Vector3(0.25f, 0.25f, 1f);

            // Sprite Renderer - Orange (255, 200, 0)
            SpriteRenderer spriteRenderer = coinObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1f, 200f / 255f, 0f, 1f); // Laranja
            spriteRenderer.sortingOrder = 0;

            // Circle Collider - Trigger
            CircleCollider2D collider = coinObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.15f;
            collider.isTrigger = true;

            // Coin Pickup Script
            var coinPickup = coinObj.AddComponent<Items.CoinPickup>();

            // Save as prefab
            string prefabPath = "Assets/_Project/Prefabs/CoinPickup.prefab";
            PrefabUtility.SaveAsPrefabAsset(coinObj, prefabPath);
            Object.DestroyImmediate(coinObj);
            Debug.Log("✅ CoinPickup.prefab created! (Orange 255,200,0 | Scale 0.25x0.25 | Collider: 0.15)");
        }
    }
}
