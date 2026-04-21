using UnityEngine;

namespace GhostBeam.Items
{
    public class BatteryPickupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject batteryPickupPrefab;
        [SerializeField] private int poolSize = 20;
        [SerializeField] private float spawnRadius = 2f;

        private Utilities.ObjectPool<GameObject> batteryPool;
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
            if (batteryPickupPrefab == null)
            {
                batteryPickupPrefab = Resources.Load<GameObject>("Prefabs/BatteryPickup");
            }

            if (batteryPickupPrefab == null)
            {
                Debug.LogError("BatteryPickupSpawner: Prefab not found! Please assign it in the Inspector or ensure it's in Resources/Prefabs/ folder.");
                return;
            }

            batteryPool = new Utilities.ObjectPool<GameObject>(
                create: () => Instantiate(batteryPickupPrefab, transform),
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
            // Chance de dropar bateria (50%)
            if (Random.value > 0.5f)
                return;

            SpawnBattery(deathPosition);
        }

        private void SpawnBattery(Vector3 position)
        {
            Vector3 spawnPos = position + (Vector3)Random.insideUnitCircle * spawnRadius;
            GameObject battery = batteryPool.Get();
            
            if (battery != null)
            {
                battery.transform.position = spawnPos;
                var batteryPickup = battery.GetComponent<BatteryPickup>();
                if (batteryPickup != null)
                {
                    batteryPickup.Collect();
                    Gameplay.BatterySystem batterySystem = FindAnyObjectByType<Gameplay.BatterySystem>();
                    if (batterySystem != null)
                    {
                        batterySystem.Recharge(batteryPickup.RechargeAmount);
                    }
                    batteryPool.Release(battery);
                }
            }
        }

        private void OnDestroy()
        {
            Enemy.EnemyController.onEnemyKilled -= OnEnemyKilled;
        }
    }
}
