using System.Collections.Generic;
using UnityEngine;

public enum DifficultyStage
{
    Presentation = 0,
    Test = 1,
    Climax = 2,
}

public class SpawnManager : MonoBehaviour
{
    public static System.Action<DifficultyStage> onDifficultyStageChanged;

    [Header("Legacy (fallback)")]
    public GameObject enemyPrefab;

    [Header("Enemy Prefabs By Archetype")]
    public GameObject baseEnemyPrefab;
    public GameObject ictericiaPrefab;
    public GameObject ectoganguePrefab;
    public GameObject titaPrefab;
    public GameObject espectroPrefab;

    public Transform target;
    public Transform flashlight;

    public float initialSpawnInterval = 2.8f;
    public float minimumSpawnInterval = 0.75f;
    public float spawnAcceleration = 0.025f;
    public int maxEnemies = 20;
    public float spawnMargin = 8f;  // Aumentado de 2f para mais distância (fantasmas não nascem perto de Luna)
    public int enemyPoolPrewarm = 24;

    [Header("Difficulty Stages")]
    public float presentationDuration = 35f;
    public float testDuration = 90f;
    public float presentationMinSpawnInterval = 1.6f;
    public float testMinSpawnInterval = 1.0f;
    public float climaxMinSpawnInterval = 0.75f;
    public float presentationSpawnAcceleration = 0.018f;
    public float testSpawnAcceleration = 0.020f;
    public float climaxSpawnAcceleration = 0.025f;
    public int presentationMaxEnemies = 6;
    public int testMaxEnemies = 12;
    public int climaxMaxEnemies = 18;

    [Header("Adaptive Performance")]
    public bool useAdaptivePerformance = true;
    public float adaptiveWindowSeconds = 2f;
    public float lowFpsThreshold = 35f;
    public float highFpsThreshold = 55f;
    [Range(0f, 0.5f)] public float maxSpawnSlowdown = 0.25f;
    [Range(0f, 0.5f)] public float maxEnemyCapReduction = 0.3f;

    public float CurrentAverageFps { get; private set; } = 60f;
    public float CurrentPerformancePressure { get; private set; }

    private float currentSpawnInterval;
    private float spawnTimer;
    private float elapsedRunTime;
    private float performanceTimer;
    private int performanceFrames;
    private readonly Dictionary<EnemyArchetype, ObjectPool> enemyPools = new Dictionary<EnemyArchetype, ObjectPool>();
    private DifficultyStage currentStage;

    public DifficultyStage CurrentDifficultyStage => currentStage;

    private void OnValidate()
    {
        // Keeps old scenes working after moving to per-archetype prefabs.
        if (baseEnemyPrefab == null && enemyPrefab != null)
        {
            baseEnemyPrefab = enemyPrefab;
        }

        if (enemyPrefab == null && baseEnemyPrefab != null)
        {
            enemyPrefab = baseEnemyPrefab;
        }
    }

    [ContextMenu("Enemy Setup/Populate Empty Archetypes With Base")]
    private void PopulateEmptyArchetypesWithBase()
    {
        GameObject fallback = GetBasePrefab();
        if (fallback == null)
        {
            Debug.LogWarning("SpawnManager: Defina Base Enemy Prefab (ou enemyPrefab legado) antes de preencher os arquétipos.", this);
            return;
        }

        if (ictericiaPrefab == null) ictericiaPrefab = fallback;
        if (ectoganguePrefab == null) ectoganguePrefab = fallback;
        if (titaPrefab == null) titaPrefab = fallback;
        if (espectroPrefab == null) espectroPrefab = fallback;
    }

    [ContextMenu("Enemy Setup/Validate Archetype Prefabs")]
    private void ValidateArchetypePrefabs()
    {
        if (GetBasePrefab() == null)
        {
            Debug.LogWarning("SpawnManager: Base Enemy Prefab nao definido.", this);
        }

        if (ictericiaPrefab == null)
        {
            Debug.LogWarning("SpawnManager: prefab de Ictericia nao definido (vai usar base).", this);
        }

        if (ectoganguePrefab == null)
        {
            Debug.LogWarning("SpawnManager: prefab de Ectogangue nao definido (vai usar base).", this);
        }

        if (titaPrefab == null)
        {
            Debug.LogWarning("SpawnManager: prefab de Tita nao definido (vai usar base).", this);
        }

        if (espectroPrefab == null)
        {
            Debug.LogWarning("SpawnManager: prefab de Espectro nao definido (vai usar base).", this);
        }
    }

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        currentStage = EvaluateStage(elapsedRunTime);
        onDifficultyStageChanged?.Invoke(currentStage);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (target == null || flashlight == null || Camera.main == null)
        {
            return;
        }

        elapsedRunTime += Time.deltaTime;
        UpdatePerformancePressure();
        UpdateStage();

        float stageMinInterval = GetStageMinSpawnInterval();
        float stageAcceleration = GetStageSpawnAcceleration();
        int stageMaxEnemies = GetStageMaxEnemies();

        // GDD sync: every 30s increase pressure by reducing interval and raising cap.
        int progressionStep = Mathf.FloorToInt(elapsedRunTime / 30f);
        stageMinInterval = Mathf.Max(0.35f, stageMinInterval - (progressionStep * 0.02f));
        stageAcceleration += progressionStep * 0.001f;
        stageMaxEnemies += progressionStep / 2;

        if (useAdaptivePerformance)
        {
            stageMinInterval *= 1f + (maxSpawnSlowdown * CurrentPerformancePressure);
            stageAcceleration *= 1f - (0.6f * CurrentPerformancePressure);
            stageMaxEnemies = Mathf.Max(1, Mathf.RoundToInt(stageMaxEnemies * (1f - (maxEnemyCapReduction * CurrentPerformancePressure))));
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer < currentSpawnInterval)
        {
            return;
        }

        spawnTimer = 0f;

        if (CountEnemies() >= stageMaxEnemies)
        {
            return;
        }

        SpawnEnemyGroup();
        currentSpawnInterval = Mathf.Max(stageMinInterval, currentSpawnInterval - stageAcceleration);
    }

    private void UpdatePerformancePressure()
    {
        if (!useAdaptivePerformance)
        {
            CurrentPerformancePressure = 0f;
            return;
        }

        performanceFrames++;
        performanceTimer += Time.unscaledDeltaTime;
        if (performanceTimer < Mathf.Max(0.2f, adaptiveWindowSeconds))
        {
            return;
        }

        CurrentAverageFps = performanceFrames / Mathf.Max(0.0001f, performanceTimer);

        float targetPressure = 0f;
        if (CurrentAverageFps < lowFpsThreshold)
        {
            targetPressure = Mathf.Clamp01((lowFpsThreshold - CurrentAverageFps) / Mathf.Max(1f, lowFpsThreshold));
        }
        else if (CurrentAverageFps > highFpsThreshold)
        {
            targetPressure = 0f;
        }
        else if (CurrentPerformancePressure > 0f)
        {
            targetPressure = CurrentPerformancePressure * 0.75f;
        }

        CurrentPerformancePressure = Mathf.Lerp(CurrentPerformancePressure, targetPressure, 0.75f);

        performanceFrames = 0;
        performanceTimer = 0f;
    }

    private void SpawnEnemyGroup()
    {
        EnemyArchetype archetype = ChooseArchetype();
        GameObject archetypePrefab = GetPrefabForArchetype(archetype);
        if (archetypePrefab == null)
        {
            return;
        }

        ObjectPool pool = EnsurePool(archetype, archetypePrefab);
        int intendedCount = archetype == EnemyArchetype.Ectogangue ? Random.Range(2, 5) : 1;
        if (elapsedRunTime >= 240f)
        {
            if (archetype == EnemyArchetype.Ectogangue)
            {
                intendedCount = Random.Range(3, 6);
            }
            else if (archetype != EnemyArchetype.Tita)
            {
                intendedCount = Random.Range(1, 3);
            }
        }
        int currentEnemies = CountEnemies();
        int allowed = Mathf.Max(0, maxEnemies - currentEnemies);
        int spawnCount = Mathf.Min(intendedCount, allowed);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 center = GetSpawnPositionOutsideCamera();
            Vector2 offset = spawnCount > 1 ? Random.insideUnitCircle * 1.25f : Vector2.zero;
            Vector2 spawnPosition = center + offset;

            GameObject enemy = pool != null
                ? pool.Spawn(spawnPosition, Quaternion.identity)
                : Instantiate(archetypePrefab, spawnPosition, Quaternion.identity);

            if (enemy == null)
            {
                continue;
            }

            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.target = target;
                enemyController.flashlight = flashlight;
                enemyController.SetArchetype(archetype);
            }
        }
    }

    private EnemyArchetype ChooseArchetype()
    {
        float t = elapsedRunTime;
        float roll = Random.value;

        // 0-60: only Penado (Base)
        if (t < 60f)
        {
            return EnemyArchetype.Base;
        }

        // 60-120: Penado + Ictericia
        if (t < 120f)
        {
            return roll < 0.62f ? EnemyArchetype.Base : EnemyArchetype.Ictericia;
        }

        // 120-180: add Ectogangue
        if (t < 180f)
        {
            if (roll < 0.34f) return EnemyArchetype.Base;
            if (roll < 0.62f) return EnemyArchetype.Ictericia;
            return EnemyArchetype.Ectogangue;
        }

        // 180-240: add Tita
        if (t < 240f)
        {
            if (roll < 0.2f) return EnemyArchetype.Base;
            if (roll < 0.42f) return EnemyArchetype.Ictericia;
            if (roll < 0.68f) return EnemyArchetype.Ectogangue;
            return EnemyArchetype.Tita;
        }

        // 240+: add Espectro and keep mixed climax.
        if (roll < 0.12f) return EnemyArchetype.Base;
        if (roll < 0.29f) return EnemyArchetype.Ictericia;
        if (roll < 0.53f) return EnemyArchetype.Ectogangue;
        if (roll < 0.74f) return EnemyArchetype.Tita;
        return EnemyArchetype.Espectro;
    }

    private void UpdateStage()
    {
        DifficultyStage newStage = EvaluateStage(elapsedRunTime);
        if (newStage == currentStage)
        {
            return;
        }

        currentStage = newStage;
        onDifficultyStageChanged?.Invoke(currentStage);
    }

    private DifficultyStage EvaluateStage(float elapsed)
    {
        if (elapsed < presentationDuration)
        {
            return DifficultyStage.Presentation;
        }

        if (elapsed < presentationDuration + testDuration)
        {
            return DifficultyStage.Test;
        }

        return DifficultyStage.Climax;
    }

    private float GetStageMinSpawnInterval()
    {
        switch (currentStage)
        {
            case DifficultyStage.Presentation:
                return presentationMinSpawnInterval;
            case DifficultyStage.Test:
                return testMinSpawnInterval;
            default:
                return climaxMinSpawnInterval;
        }
    }

    private float GetStageSpawnAcceleration()
    {
        switch (currentStage)
        {
            case DifficultyStage.Presentation:
                return presentationSpawnAcceleration;
            case DifficultyStage.Test:
                return testSpawnAcceleration;
            default:
                return climaxSpawnAcceleration;
        }
    }

    private int GetStageMaxEnemies()
    {
        switch (currentStage)
        {
            case DifficultyStage.Presentation:
                return presentationMaxEnemies;
            case DifficultyStage.Test:
                return testMaxEnemies;
            default:
                return climaxMaxEnemies;
        }
    }

    private Vector2 GetSpawnPositionOutsideCamera()
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        Vector2 center = cam.transform.position;
        float halfW = camWidth * 0.5f + spawnMargin;
        float halfH = camHeight * 0.5f + spawnMargin;

        Vector2 topLeft = new Vector2(center.x - halfW, center.y + halfH);
        Vector2 topRight = new Vector2(center.x + halfW, center.y + halfH);
        Vector2 bottomLeft = new Vector2(center.x - halfW, center.y - halfH);
        Vector2 bottomRight = new Vector2(center.x + halfW, center.y - halfH);

        Vector2[] corners = { topLeft, topRight, bottomLeft, bottomRight };
        Vector2 corner = corners[Random.Range(0, corners.Length)];

        float jitter = Mathf.Max(0.6f, spawnMargin * 0.6f);
        return corner + new Vector2(Random.Range(-jitter, jitter), Random.Range(-jitter, jitter));
    }

    private int CountEnemies()
    {
        if (enemyPools.Count > 0)
        {
            int total = 0;
            foreach (ObjectPool pool in enemyPools.Values)
            {
                if (pool != null)
                {
                    total += pool.ActiveCount;
                }
            }

            return total;
        }

        return FindObjectsByType<EnemyController>().Length;
    }

    private ObjectPool EnsurePool(EnemyArchetype archetype, GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        if (enemyPools.TryGetValue(archetype, out ObjectPool existingPool) && existingPool != null)
        {
            return existingPool;
        }

        GameObject poolObject = new GameObject("EnemyPool_" + archetype);
        ObjectPool pool = poolObject.AddComponent<ObjectPool>();
        pool.Initialize(prefab, enemyPoolPrewarm, Mathf.Max(maxEnemies, enemyPoolPrewarm));
        enemyPools[archetype] = pool;
        return pool;
    }

    private GameObject GetPrefabForArchetype(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Ictericia:
                return ictericiaPrefab != null ? ictericiaPrefab : GetBasePrefab();
            case EnemyArchetype.Ectogangue:
                return ectoganguePrefab != null ? ectoganguePrefab : GetBasePrefab();
            case EnemyArchetype.Tita:
                return titaPrefab != null ? titaPrefab : GetBasePrefab();
            case EnemyArchetype.Espectro:
                return espectroPrefab != null ? espectroPrefab : GetBasePrefab();
            default:
                return GetBasePrefab();
        }
    }

    private GameObject GetBasePrefab()
    {
        if (baseEnemyPrefab != null)
        {
            return baseEnemyPrefab;
        }

        return enemyPrefab;
    }
}
