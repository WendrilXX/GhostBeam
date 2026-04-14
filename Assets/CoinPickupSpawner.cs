using UnityEngine;

public class CoinPickupSpawner : MonoBehaviour
{
    public static CoinPickupSpawner Instance { get; private set; }

    public GameObject coinPickupPrefab;
    public int coinsPerDrop = 1;
    [Range(0f, 1f)] public float enemyCoinDropChance = 1f;
    [Range(0f, 2f)] public float ictericiaDropMultiplier = 1.1f;
    [Range(0f, 2f)] public float ectogangueDropMultiplier = 1.0f;
    [Range(0f, 2f)] public float titaDropMultiplier = 1.5f;
    [Range(0f, 2f)] public float espectroDropMultiplier = 1.2f;
    public int maxCoinPickups = 40;
    public int pickupPoolPrewarm = 10;

    private ObjectPool pickupPool;
    private Sprite fallbackSprite;

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

    public bool TrySpawnDropFromEnemy(Vector2 worldPosition, EnemyArchetype archetype)
    {
        float chance = Mathf.Clamp01(enemyCoinDropChance * GetChanceMultiplier(archetype));
        if (Random.value > chance)
        {
            return false;
        }

        EnsurePool();

        GameObject pickup = pickupPool != null
            ? pickupPool.Spawn(worldPosition, Quaternion.identity)
            : CreateFallbackCoinPickup(worldPosition);

        if (pickup == null)
        {
            return false;
        }

        CoinPickup coin = pickup.GetComponent<CoinPickup>();
        if (coin != null)
        {
            // Drop aleatório entre 1-20 moedas
            coin.coinValue = Random.Range(1, 21);
        }

        return true;
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

    private float GetValueMultiplier(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Tita:
                return 2f;
            case EnemyArchetype.Espectro:
                return 1.5f;
            case EnemyArchetype.Ictericia:
                return 1.25f;
            default:
                return 1f;
        }
    }

    private void EnsurePool()
    {
        if (pickupPool != null)
        {
            return;
        }

        GameObject source = coinPickupPrefab;
        if (source == null)
        {
            source = CreateFallbackCoinPickupTemplate();
        }

        if (source == null)
        {
            return;
        }

        GameObject poolObject = new GameObject("CoinPickupPool");
        pickupPool = poolObject.AddComponent<ObjectPool>();
        pickupPool.Initialize(source, pickupPoolPrewarm, Mathf.Max(maxCoinPickups, pickupPoolPrewarm));

        if (coinPickupPrefab == null)
        {
            Destroy(source);
        }
    }

    private GameObject CreateFallbackCoinPickup(Vector2 position)
    {
        GameObject coinObj = CreateFallbackCoinPickupTemplate();
        if (coinObj == null)
        {
            return null;
        }

        coinObj.transform.position = position;
        return coinObj;
    }

    private GameObject CreateFallbackCoinPickupTemplate()
    {
        GameObject coinObj = new GameObject("CoinPickup");
        coinObj.transform.localScale = new Vector3(0.45f, 0.45f, 1f);

        SpriteRenderer sr = coinObj.AddComponent<SpriteRenderer>();
        sr.sprite = GetFallbackSprite();
        sr.color = new Color(1f, 0.85f, 0.2f, 1f);
        sr.sortingOrder = 8;

        CircleCollider2D col = coinObj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.45f;

        Rigidbody2D rb = coinObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.gravityScale = 0f;

        coinObj.AddComponent<CoinPickup>();
        coinObj.AddComponent<PooledObject>();
        return coinObj;
    }

    private Sprite GetFallbackSprite()
    {
        if (fallbackSprite != null)
        {
            return fallbackSprite;
        }

        const int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Vector2 c = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.44f;
        float edge = size * 0.07f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c);
                float a = Mathf.Clamp01((radius - d) / edge);
                Color baseColor = new Color(1f, 0.86f, 0.24f, a);
                tex.SetPixel(x, y, baseColor);
            }
        }

        tex.Apply();
        fallbackSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
        return fallbackSprite;
    }
}
