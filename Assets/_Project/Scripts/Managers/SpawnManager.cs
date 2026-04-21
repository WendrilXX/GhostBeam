using UnityEngine;

namespace GhostBeam.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int poolSize = 30;
        [SerializeField] private float initialSpawnRate = 2.8f;
        [SerializeField] private int maxSimultaneous = 6;
        [SerializeField] private float spawnRadius = 15f;

        private Utilities.ObjectPool<GameObject> enemyPool;
        private float spawnTimer = 0f;
        private float currentSpawnRate;
        private int currentEnemyCount = 0;
        private Transform playerTransform;

        private void Start()
        {
            playerTransform = FindAnyObjectByType<Player.LunaController>()?.transform;
            InitializePool();
            currentSpawnRate = initialSpawnRate;
        }

        private void InitializePool()
        {
            // Load prefab dynamically if not assigned
            if (enemyPrefab == null)
            {
                enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
            }

            if (enemyPrefab == null)
            {
                Debug.LogError("SpawnManager: Enemy prefab not found! Please assign it in the Inspector or ensure it's in Resources/Prefabs/ folder.");
                return;
            }

            enemyPool = new Utilities.ObjectPool<GameObject>(
                create: () => Instantiate(enemyPrefab, transform),
                onGet: (obj) => 
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                        currentEnemyCount++;
                    }
                },
                onRelease: (obj) => 
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                        currentEnemyCount--;
                    }
                },
                initialSize: poolSize
            );
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.IsPaused)
                return;

            AdjustForStage();
            UpdateSpawning();
        }

        private void AdjustForStage()
        {
            float gameTime = Time.timeSinceLevelLoad;

            if (gameTime < 35f)
            {
                // Stage 1: Crescimento rápido
                currentSpawnRate = 2.8f - (gameTime * 0.018f);
                maxSimultaneous = 4;
            }
            else if (gameTime < 125f)
            {
                // Stage 2: Crescimento moderado
                currentSpawnRate = 1.4f - ((gameTime - 35) * 0.020f);
                maxSimultaneous = 6;
            }
            else
            {
                // Stage 3: Modo climático
                currentSpawnRate = 0.95f - ((gameTime - 125) * 0.025f);
                maxSimultaneous = 8;
            }

            currentSpawnRate = Mathf.Max(currentSpawnRate, 0.3f);
        }

        private void UpdateSpawning()
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= currentSpawnRate && currentEnemyCount < maxSimultaneous)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }

        private void SpawnEnemy()
        {
            if (playerTransform == null || enemyPool == null)
                return;

            // Spawn na borda oposta da Luna
            Vector2 spawnPos = GetSpawnPosition();
            
            GameObject enemy = enemyPool.Get();
            if (enemy != null)
            {
                enemy.transform.position = spawnPos;
                // Ensure enemy is properly initialized
                var controller = enemy.GetComponent<Enemy.EnemyController>();
                if (controller != null)
                    controller.Reset();
            }
        }

        private Vector2 GetSpawnPosition()
        {
            // Spawn em posição aleatória nas bordas, afastado da Luna
            Vector2 playerPos = playerTransform.position;
            Vector2 spawnPos = playerPos + Random.insideUnitCircle.normalized * spawnRadius;

            // Clamp aos limites da tela
            float screenHalfHeight = Camera.main.orthographicSize;
            float screenHalfWidth = screenHalfHeight * Camera.main.aspect;

            spawnPos.x = Mathf.Clamp(spawnPos.x, -screenHalfWidth, screenHalfWidth);
            spawnPos.y = Mathf.Clamp(spawnPos.y, -screenHalfHeight, screenHalfHeight);

            return spawnPos;
        }
    }
}
