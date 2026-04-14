using UnityEngine;
using UnityEditor;

public class BatteryPickupPrefabCreator : MonoBehaviour
{
    [MenuItem("Assets/Create Battery Pickup Prefab")]
    public static void CreateBatteryPickupPrefab()
    {
        // Criar pasta se não existir
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Pickups"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Pickups");
        }

        // Criar GameObject base
        GameObject prefab = new GameObject("BatteryPickup");

        // Adicionar SpriteRenderer
        SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(0f, 1f, 1f, 0.7f);
        spriteRenderer.sortingOrder = 1;

        // Adicionar CircleCollider2D (trigger)
        CircleCollider2D collider = prefab.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        // Adicionar BatteryPickup script
        BatteryPickup batteryPickup = prefab.AddComponent<BatteryPickup>();
        batteryPickup.restoreAmount = 25f;
        batteryPickup.magnetRange = 2.2f;
        batteryPickup.magnetSpeed = 7f;

        // Adicionar PooledObject script
        PooledObject pooledObject = prefab.AddComponent<PooledObject>();

        // Salvar como prefab
        string prefabPath = "Assets/Prefabs/Pickups/BatteryPickup.prefab";
        Object prefabAsset = PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
        
        // Destruir GameObject temporário
        DestroyImmediate(prefab);

        Debug.Log($"✅ Battery Pickup prefab criado em: {prefabPath}");
        EditorUtility.DisplayDialog("Sucesso!", $"Prefab criado!\n{prefabPath}", "OK");
        
        // Selecionar o prefab criado para fácil acesso
        EditorGUIUtility.PingObject(prefabAsset);
    }
}
