using UnityEngine;
using GhostBeam.Enemy;

namespace GhostBeam.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int poolSize = 30;
        [SerializeField] private float initialSpawnRate = 1.5f;  // Reduzido de 2.8f para spawn mais rápido
        [SerializeField] private int maxSimultaneous = 6;
        [SerializeField] private float spawnRadius = 15f;

        [Header("Espectro (fase 160s+) — ondas")]
        [SerializeField] [Tooltip("Segundos em que Espectros podem sair no sorteio.")]
        private float spectreWindowOnSeconds = 32f;
        [SerializeField] [Tooltip("Segundos sem Espectro no sorteio (só outros tipos).")]
        private float spectreWindowOffSeconds = 48f;
        [SerializeField] [Tooltip("Tempo de jogo em que a oscilação começa (após intro).")]
        private float spectreWavePhaseStartTime = 160f;

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
                create: () =>
                {
                    var go = Instantiate(enemyPrefab, transform);
                    go.SetActive(false);
                    return go;
                },
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

            // Ensure gameplay has started (fallback if no intro fade)
            GameplayIntroState.EnsureGameplayStarted();

            // Auto-end intro if it's stuck (fallback for scenes without GameplayIntroFade)
            GameplayIntroState.AutoEndIntroIfStuck(2f);

            if (!GameplayIntroState.AllowGameplay)
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
            float t = GameplayIntroState.StageElapsedSeconds;

            if (t < 40f)
            {
                maxSimultaneous = 4;
                currentSpawnRate = Mathf.Lerp(1.8f, 1.5f, t / 40f);
            }
            else if (t < 80f)
            {
                maxSimultaneous = 5;
                float u = (t - 40f) / 40f;
                currentSpawnRate = Mathf.Lerp(1.5f, 1.2f, u);
            }
            else if (t < 120f)
            {
                maxSimultaneous = 6;
                float u = (t - 80f) / 40f;
                currentSpawnRate = Mathf.Lerp(1.2f, 0.9f, u);
            }
            else if (t < 160f)
            {
                maxSimultaneous = 7;
                float u = (t - 120f) / 40f;
                currentSpawnRate = Mathf.Lerp(0.9f, 0.6f, u);
            }
            else
            {
                maxSimultaneous = 8;
                currentSpawnRate = Mathf.Max(0.25f, 0.6f - (t - 160f) * 0.002f);
            }

            currentSpawnRate = Mathf.Max(currentSpawnRate, 0.25f);
        }

        private void UpdateSpawning()
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= currentSpawnRate && currentEnemyCount < maxSimultaneous)
            {
                float debugTime = GameplayIntroState.StageElapsedSeconds;
                bool allowGameplay = GameplayIntroState.AllowGameplay;
                Debug.Log($"[SpawnManager DEBUG] Time={debugTime:F1}s, AllowGameplay={allowGameplay}, Rate={currentSpawnRate:F2}s");
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }

        private void SpawnEnemy()
        {
            if (playerTransform == null || enemyPool == null)
                return;

            int room = maxSimultaneous - currentEnemyCount;
            if (room <= 0)
                return;

            EnemyController.EnemyArchetype archetype =
                ChooseEnemyArchetype(GameplayIntroState.StageElapsedSeconds);

            Debug.Log($"[SpawnManager] Spawning: {archetype} | Time: {GameplayIntroState.StageElapsedSeconds:F1}s | Current: {currentEnemyCount}/{maxSimultaneous}");

            if (archetype == EnemyController.EnemyArchetype.Ectogangue && room >= 2)
            {
                int packSize = Mathf.Min(room, Random.Range(2, 4));
                Vector2 anchor = GetSpawnPosition();
                const float packSpread = 1.05f;
                for (int i = 0; i < packSize; i++)
                {
                    if (currentEnemyCount >= maxSimultaneous)
                        break;
                    Vector2 pos = ClampToPlayfield(anchor + (Vector2)(Random.insideUnitCircle * packSpread));
                    if (!TrySpawnAt(pos, archetype))
                        break;
                }
            }
            else
            {
                TrySpawnAt(GetSpawnPosition(), archetype);
            }
        }

        private bool TrySpawnAt(Vector2 spawnPos, EnemyController.EnemyArchetype archetype)
        {
            if (enemyPool == null || currentEnemyCount >= maxSimultaneous)
                return false;

            GameObject enemy = enemyPool.Get();
            if (enemy == null)
                return false;

            var pooled = enemy.GetComponent<Utilities.PooledObject>();
            if (pooled == null)
                pooled = enemy.AddComponent<Utilities.PooledObject>();

            pooled.Initialize(p => enemyPool.Release(p.gameObject));

            enemy.transform.position = spawnPos;
            var controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.InitializeArchetype(archetype);
                controller.Reset();
            }

            return true;
        }

        private Vector2 ClampToPlayfield(Vector2 spawnPos)
        {
            if (Camera.main == null)
                return spawnPos;

            float screenHalfHeight = Camera.main.orthographicSize;
            float screenHalfWidth = screenHalfHeight * Camera.main.aspect;
            spawnPos.x = Mathf.Clamp(spawnPos.x, -screenHalfWidth, screenHalfWidth);
            spawnPos.y = Mathf.Clamp(spawnPos.y, -screenHalfHeight, screenHalfHeight);
            return spawnPos;
        }

        /// <summary>GDD: 0–40 só Penado; 40–80 Icterícia; 80–120 Ectogangue; 120–160 Titã; 160+ Espectro + mistura.</summary>
        private EnemyController.EnemyArchetype ChooseEnemyArchetype(float gameTime)
        {
            if (gameTime < 40f)
            {
                Debug.Log($"[ChooseEnemyArchetype] Time {gameTime:F1}s < 40s → Penado only");
                return EnemyController.EnemyArchetype.Penado;
            }

            if (gameTime < 80f)
            {
                Debug.Log($"[ChooseEnemyArchetype] Time {gameTime:F1}s: 40-80s phase → Penado (38%) or Ictericia (62%)");
                return PickWeighted(
                    EnemyController.EnemyArchetype.Penado, 0.38f,
                    EnemyController.EnemyArchetype.Ictericia, 0.62f);
            }

            if (gameTime < 120f)
            {
                Debug.Log($"[ChooseEnemyArchetype] Time {gameTime:F1}s: 80-120s phase → Mix of 3 types");
                return PickWeighted3(
                    EnemyController.EnemyArchetype.Penado, 0.22f,
                    EnemyController.EnemyArchetype.Ictericia, 0.28f,
                    EnemyController.EnemyArchetype.Ectogangue, 0.50f);
            }

            if (gameTime < 160f)
            {
                Debug.Log($"[ChooseEnemyArchetype] Time {gameTime:F1}s: 120-160s phase → Mix of 4 types with Titã");
                return PickWeighted4(
                    EnemyController.EnemyArchetype.Penado, 0.12f,
                    EnemyController.EnemyArchetype.Ictericia, 0.18f,
                    EnemyController.EnemyArchetype.Ectogangue, 0.38f,
                    EnemyController.EnemyArchetype.Tita, 0.32f);
            }

            Debug.Log($"[ChooseEnemyArchetype] Time {gameTime:F1}s: 160s+ phase → Spectro waves");
            return ChooseArchetypeSpectrePhase(gameTime);
        }

        /// <summary>
        /// Após 160s: alterna janelas com Espectro no pool e janelas só com os outros tipos (respiração de dificuldade).
        /// </summary>
        private EnemyController.EnemyArchetype ChooseArchetypeSpectrePhase(float gameTime)
        {
            if (IsSpectreSpawnWindowActive(gameTime))
            {
                return PickWeighted(
                    EnemyController.EnemyArchetype.Penado, 0.11f,
                    EnemyController.EnemyArchetype.Ictericia, 0.13f,
                    EnemyController.EnemyArchetype.Ectogangue, 0.24f,
                    EnemyController.EnemyArchetype.Tita, 0.19f,
                    EnemyController.EnemyArchetype.Espectro, 0.33f);
            }

            return PickWeighted4(
                EnemyController.EnemyArchetype.Penado, 0.18f,
                EnemyController.EnemyArchetype.Ictericia, 0.22f,
                EnemyController.EnemyArchetype.Ectogangue, 0.32f,
                EnemyController.EnemyArchetype.Tita, 0.28f);
        }

        private bool IsSpectreSpawnWindowActive(float gameTime)
        {
            if (gameTime < spectreWavePhaseStartTime)
                return false;

            float since = gameTime - spectreWavePhaseStartTime;
            float cycle = Mathf.Max(1f, spectreWindowOnSeconds + spectreWindowOffSeconds);
            float phase = since % cycle;
            return phase < spectreWindowOnSeconds;
        }

        private EnemyController.EnemyArchetype PickWeighted3(
            EnemyController.EnemyArchetype a, float wa,
            EnemyController.EnemyArchetype b, float wb,
            EnemyController.EnemyArchetype c, float wc)
        {
            float r = Random.value * (wa + wb + wc);
            if (r < wa)
                return a;
            r -= wa;
            if (r < wb)
                return b;
            return c;
        }

        private EnemyController.EnemyArchetype PickWeighted4(
            EnemyController.EnemyArchetype a, float wa,
            EnemyController.EnemyArchetype b, float wb,
            EnemyController.EnemyArchetype c, float wc,
            EnemyController.EnemyArchetype d, float wd)
        {
            float r = Random.value * (wa + wb + wc + wd);
            if (r < wa)
                return a;
            r -= wa;
            if (r < wb)
                return b;
            r -= wb;
            if (r < wc)
                return c;
            return d;
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
            Vector2 playerPos = playerTransform.position;
            Vector2 spawnPos = playerPos + Random.insideUnitCircle.normalized * spawnRadius;
            return ClampToPlayfield(spawnPos);
        }

        private void OnDestroy()
        {
            EnemyController.onEnemyRemoved -= OnEnemyRemoved;
        }
    }
}
