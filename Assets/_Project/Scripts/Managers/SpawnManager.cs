using UnityEngine;
using GhostBeam.Enemy;

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
            EnemyController.onEnemyRemoved -= OnEnemyRemoved;
            EnemyController.onEnemyRemoved += OnEnemyRemoved;
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

        private void OnEnemyRemoved()
        {
            currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
        }

        private void AdjustForStage()
        {
            float gameTime = Time.timeSinceLevelLoad;

            if (gameTime < 35f)
            {
                // Stage 1: Crescimento rápido
                currentSpawnRate = 2.8f - (gameTime * 0.034f);
                maxSimultaneous = 6;
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
                var pooled = enemy.GetComponent<Utilities.PooledObject>();
                if (pooled == null)
                    pooled = enemy.AddComponent<Utilities.PooledObject>();

                pooled.Initialize(p => enemyPool.Release(p.gameObject));

                enemy.transform.position = spawnPos;
                // Ensure enemy is properly initialized
                var controller = enemy.GetComponent<Enemy.EnemyController>();
                if (controller != null)
                {
                    EnemyController.EnemyArchetype archetype = ChooseEnemyArchetype(Time.timeSinceLevelLoad);
                    controller.InitializeArchetype(archetype);
                    controller.Reset();
                }
            }
        }

        private EnemyController.EnemyArchetype ChooseEnemyArchetype(float gameTime)
        {
            // Stage 1 (0-35s): Penado 70%, Ictericia 30%
            if (gameTime < 35f)
            {
                return PickWeighted(
                    EnemyController.EnemyArchetype.Penado, 0.70f,
                    EnemyController.EnemyArchetype.Ictericia, 0.30f
                );
            }

            // Stage 2 (35-125s): Penado 40%, Ictericia 30%, Ectogangue 20%, Espectro 5%, Tita 5%
            if (gameTime < 125f)
            {
                return PickWeighted(
                    EnemyController.EnemyArchetype.Penado, 0.40f,
                    EnemyController.EnemyArchetype.Ictericia, 0.30f,
                    EnemyController.EnemyArchetype.Ectogangue, 0.20f,
                    EnemyController.EnemyArchetype.Espectro, 0.05f,
                    EnemyController.EnemyArchetype.Tita, 0.05f
                );
            }

            // Stage 3 (125s+): Penado 30%, Ictericia 25%, Ectogangue 20%, Espectro 15%, Tita 10%
            return PickWeighted(
                EnemyController.EnemyArchetype.Penado, 0.30f,
                EnemyController.EnemyArchetype.Ictericia, 0.25f,
                EnemyController.EnemyArchetype.Ectogangue, 0.20f,
                EnemyController.EnemyArchetype.Espectro, 0.15f,
                EnemyController.EnemyArchetype.Tita, 0.10f
            );
        }

        private EnemyController.EnemyArchetype PickWeighted(
            EnemyController.EnemyArchetype a, float wa,
            EnemyController.EnemyArchetype b, float wb)
        {
            float roll = Random.value * (wa + wb);
            return roll < wa ? a : b;
        }

        private EnemyController.EnemyArchetype PickWeighted(
            EnemyController.EnemyArchetype a, float wa,
            EnemyController.EnemyArchetype b, float wb,
            EnemyController.EnemyArchetype c, float wc,
            EnemyController.EnemyArchetype d, float wd,
            EnemyController.EnemyArchetype e, float we)
        {
            float total = wa + wb + wc + wd + we;
            float roll = Random.value * total;

            if (roll < wa)
                return a;
            roll -= wa;
            if (roll < wb)
                return b;
            roll -= wb;
            if (roll < wc)
                return c;
            roll -= wc;
            if (roll < wd)
                return d;
            return e;
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

        private void OnDestroy()
        {
            EnemyController.onEnemyRemoved -= OnEnemyRemoved;
        }
    }
}
