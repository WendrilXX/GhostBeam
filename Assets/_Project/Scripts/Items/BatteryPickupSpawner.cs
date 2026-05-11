using UnityEngine;
using GhostBeam.Utilities;

namespace GhostBeam.Items
{
    public class BatteryPickupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject batteryPickupPrefab;
        [SerializeField] private int poolSize = 20;
        [SerializeField] private float spawnRadius = 2f;

        private ObjectPool<GameObject> batteryPool;

        private void Start()
        {
            InitializePool();
            Enemy.EnemyController.onEnemyKilled += OnEnemyKilled;
        }

        private void InitializePool()
        {
            if (batteryPickupPrefab == null)
                batteryPickupPrefab = Resources.Load<GameObject>("Prefabs/BatteryPickup");

            if (batteryPickupPrefab == null)
            {
                Debug.LogError("BatteryPickupSpawner: Prefab not found! Assign in Inspector or Resources/Prefabs/BatteryPickup.");
                return;
            }

            batteryPool = new ObjectPool<GameObject>(
                create: () =>
                {
                    var go = Instantiate(batteryPickupPrefab, transform);
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
                        var batteryPickup = obj.GetComponent<BatteryPickup>();
                        if (batteryPickup != null)
                            batteryPickup.Reset();
                    }
                },
                initialSize: poolSize
            );
        }

        private void OnEnemyKilled(Vector3 deathPosition, int enemyType)
        {
            if (Random.value > 0.5f)
                return;

            SpawnBattery(deathPosition);
        }

        private void SpawnBattery(Vector3 position)
        {
            if (batteryPool == null)
                return;

            GameObject battery = batteryPool.Get();
            if (battery == null)
                return;

            Vector3 spawnPos = position + (Vector3)Random.insideUnitCircle * spawnRadius;
            battery.transform.position = spawnPos;

            var pooled = battery.GetComponent<PooledObject>();
            if (pooled == null)
                pooled = battery.AddComponent<PooledObject>();
            pooled.Initialize(p => batteryPool.Release(p.gameObject));

            var batteryPickup = battery.GetComponent<BatteryPickup>();
            batteryPickup?.Reset();
        }

        private void OnDestroy()
        {
            Enemy.EnemyController.onEnemyKilled -= OnEnemyKilled;
        }
    }
}
