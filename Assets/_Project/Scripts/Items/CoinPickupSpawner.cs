using UnityEngine;
using GhostBeam.Utilities;

namespace GhostBeam.Items
{
    public class CoinPickupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject coinPickupPrefab;
        [SerializeField] private int poolSize = 30;
        [SerializeField] private float spawnRadius = 2f;

        private ObjectPool<GameObject> coinPool;

        private void Start()
        {
            InitializePool();
            Enemy.EnemyController.onEnemyKilled += OnEnemyKilled;
        }

        private void InitializePool()
        {
            if (coinPickupPrefab == null)
                coinPickupPrefab = Resources.Load<GameObject>("Prefabs/CoinPickup");

            if (coinPickupPrefab == null)
            {
                Debug.LogError("CoinPickupSpawner: Prefab not found! Assign in Inspector or Resources/Prefabs/CoinPickup.");
                return;
            }

            coinPool = new ObjectPool<GameObject>(
                create: () =>
                {
                    var go = Instantiate(coinPickupPrefab, transform);
                    go.SetActive(false);
                    return go;
                },
                onGet: obj =>
                {
                    if (obj != null)
                        obj.SetActive(true);
                },
                onRelease: obj =>
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
            int coinAmount = Mathf.Max(1, enemyType);
            SpawnCoins(deathPosition, coinAmount);
        }

        private void SpawnCoins(Vector3 position, int count)
        {
            if (coinPool == null)
                return;

            for (int i = 0; i < count; i++)
            {
                GameObject coin = coinPool.Get();
                if (coin == null)
                    continue;

                Vector3 spawnPos = position + (Vector3)Random.insideUnitCircle * spawnRadius;
                coin.transform.position = spawnPos;

                var pooled = coin.GetComponent<PooledObject>();
                if (pooled == null)
                    pooled = coin.AddComponent<PooledObject>();
                pooled.Initialize(p => coinPool.Release(p.gameObject));

                var coinPickup = coin.GetComponent<CoinPickup>();
                if (coinPickup != null)
                {
                    coinPickup.SetCoinAmount(1);
                    coinPickup.Reset();
                }
            }
        }

        private void OnDestroy()
        {
            Enemy.EnemyController.onEnemyKilled -= OnEnemyKilled;
        }
    }
}
