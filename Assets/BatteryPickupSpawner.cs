using UnityEngine;

public class BatteryPickupSpawner : MonoBehaviour
{
    public static BatteryPickupSpawner Instance { get; private set; }

    public GameObject batteryPickupPrefab;
    public float spawnInterval = 12f;
    public int maxPickups = 2;
    public float screenMargin = 1f;
    public int pickupPoolPrewarm = 4;
    public bool enableEnemyDrop = true;
    [Range(0f, 1f)] public float enemyDropChance = 1f;
    [Range(0f, 2f)] public float ictericiaDropMultiplier = 1.5f;
    [Range(0f, 2f)] public float ectogangueDropMultiplier = 2f;
    [Range(0f, 2f)] public float titaDropMultiplier = 2.5f;
    [Range(0f, 2f)] public float espectroDropMultiplier = 2f;
    public int extraEnemyDropCapacity = 6;

    private float spawnTimer;
    private ObjectPool pickupPool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (batteryPickupPrefab == null || Camera.main == null)
        {
            return;
        }

        EnsurePool();

        spawnTimer += Time.deltaTime;
        if (spawnTimer < spawnInterval)
        {
            return;
        }

        spawnTimer = 0f;

        if (CountPickups() >= maxPickups)
        {
            return;
        }

        SpawnPickup();
    }

    private void SpawnPickup()
    {
        Vector2 spawnPosition = GetRandomPositionInsideCamera();
        SpawnPickupAt(spawnPosition);
    }

    public bool TrySpawnDropFromEnemy(Vector2 worldPosition, EnemyArchetype archetype)
    {
        if (!enableEnemyDrop || batteryPickupPrefab == null)
        {
            return false;
        }

        float chance = Mathf.Clamp01(enemyDropChance * GetChanceMultiplier(archetype));
        if (Random.value > chance)
        {
            return false;
        }

        EnsurePool();
        GameObject pickup = pickupPool != null
            ? pickupPool.Spawn(worldPosition, Quaternion.identity)
            : InstantiatePickup(worldPosition);

        if (pickup != null)
        {
            BatteryPickup batteryPickup = pickup.GetComponent<BatteryPickup>();
            if (batteryPickup != null)
            {
                // Drop aleatório entre 5-25% da bateria máxima
                BatterySystem batterySystem = FindAnyObjectByType<BatterySystem>();
                float maxBatteryCapacity = batterySystem != null ? batterySystem.maxBattery : 150f;
                batteryPickup.restoreAmount = Random.Range(0.05f, 0.25f) * maxBatteryCapacity;
            }
            return true;
        }

        return false;
    }

    private GameObject InstantiatePickup(Vector2 position)
    {
        if (batteryPickupPrefab == null)
        {
            return null;
        }

        return Instantiate(batteryPickupPrefab, position, Quaternion.identity);
    }

    private float GetChanceMultiplier(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Ictericia:
                return ictericiaDropMultiplier;
            case EnemyArchetype.Ectogangue:
                return ectogangueDropMultiplier;
            case EnemyArchetype.Tita:
                return titaDropMultiplier;
            case EnemyArchetype.Espectro:
                return espectroDropMultiplier;
            default:
                return 1f;
        }
    }

    private void SpawnPickupAt(Vector2 spawnPosition)
    {
        if (pickupPool != null)
        {
            pickupPool.Spawn(spawnPosition, Quaternion.identity);
            return;
        }

        Instantiate(batteryPickupPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector2 GetRandomPositionInsideCamera()
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        Vector2 center = cam.transform.position;
        float halfW = Mathf.Max(0f, camWidth * 0.5f - screenMargin);
        float halfH = Mathf.Max(0f, camHeight * 0.5f - screenMargin);

        float x = Random.Range(center.x - halfW, center.x + halfW);
        float y = Random.Range(center.y - halfH, center.y + halfH);

        return new Vector2(x, y);
    }

    private int CountPickups()
    {
        if (pickupPool != null)
        {
            return pickupPool.ActiveCount;
        }

        return FindObjectsByType<BatteryPickup>().Length;
    }

    private void EnsurePool()
    {
        if (pickupPool != null || batteryPickupPrefab == null)
        {
            return;
        }

        GameObject poolObject = new GameObject("BatteryPickupPool");
        pickupPool = poolObject.AddComponent<ObjectPool>();
        int poolLimit = Mathf.Max(maxPickups + extraEnemyDropCapacity, pickupPoolPrewarm);
        pickupPool.Initialize(batteryPickupPrefab, pickupPoolPrewarm, poolLimit);
    }
}
