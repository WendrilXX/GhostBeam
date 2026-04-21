using UnityEngine;

namespace GhostBeam.Items
{
    public class CoinPickupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject coinPickupPrefab;
        [SerializeField] private int poolSize = 30;
        [SerializeField] private float spawnRadius = 2f;

        private Utilities.ObjectPool<GameObject> coinPool;
        private Transform playerTransform;

        private void Start()
        {
            playerTransform = FindAnyObjectByType<Player.LunaController>()?.transform;
            
            InitializePool();
            Enemy.EnemyController.onEnemyKilled += OnEnemyKilled;
        }

        private void InitializePool()
        {
            // Load prefab dynamically if not assigned
            if (coinPickupPrefab == null)
            {
                coinPickupPrefab = Resources.Load<GameObject>("Prefabs/CoinPickup");
            }

            if (coinPickupPrefab == null)
            {
                Debug.LogError("CoinPickupSpawner: Prefab not found! Please assign it in the Inspector or ensure it's in Resources/Prefabs/ folder.");
                return;
            }

            coinPool = new Utilities.ObjectPool<GameObject>(
                create: () => Instantiate(coinPickupPrefab, transform),
                onGet: (obj) => 
                {
                    if (obj != null)
                        obj.SetActive(true);
                },
                onRelease: (obj) => 
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                        var coinPickup = obj.GetComponent<CoinPickup>();
                        if (coinPickup != null)
                            coinPickup.Reset();
                    }
                },
                initialSize: poolSize
            );
        }

        private void OnEnemyKilled(Vector3 deathPosition, int enemyType)
        {
            // 100% de drop de moeda
            int coinAmount = Mathf.Max(1, enemyType);
            SpawnCoins(deathPosition, coinAmount);
        }

        private void SpawnCoins(Vector3 position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = position + (Vector3)Random.insideUnitCircle * spawnRadius;
                GameObject coin = coinPool.Get();
                
                if (coin != null)
                {
                    coin.transform.position = spawnPos;
                    var coinPickup = coin.GetComponent<CoinPickup>();
                    if (coinPickup != null)
                    {
                        Managers.ScoreManager.Instance?.AddCoins(coinPickup.CoinAmount);
                        coinPickup.Collect();
                        coinPool.Release(coin);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            Enemy.EnemyController.onEnemyKilled -= OnEnemyKilled;
        }
    }
}
