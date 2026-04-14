using UnityEngine;
using System.Collections.Generic;

public enum EnemyArchetype
{
    Base = 0,
    Ictericia = 1,
    Ectogangue = 2,
    Tita = 3,
    Espectro = 4,
}

public class EnemyController : MonoBehaviour
{
    public static System.Action onEnemyKilled;
    private static readonly List<EnemyController> activeEnemies = new List<EnemyController>();

    public static int ActiveCount => activeEnemies.Count;
    public static IReadOnlyList<EnemyController> ActiveEnemies => activeEnemies;

    public Transform target;
    public Transform flashlight;
    public float speed = 1.5f;
    public float timeToKill = 2f;
    public float baseLightRange = 15f;
    public float baseConeAngle = 35f;
    public EnemyArchetype archetype = EnemyArchetype.Base;
    [Range(0f, 1f)] public float litFeedbackStrength = 0.7f;
    public float deathFlashDuration = 0.12f;

    private SpriteRenderer spriteRenderer;
    private bool archetypeApplied;
    private Color archetypeBaseColor = Color.white;
    private bool isDying;

    private float lightTimer = 0f;

    private void OnEnable()
    {
        if (!activeEnemies.Contains(this))
        {
            activeEnemies.Add(this);
        }

        isDying = false;
        lightTimer = 0f;
        transform.localScale = Vector3.one;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = true;
            }
        }
    }

    private void OnDisable()
    {
        activeEnemies.Remove(this);
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ApplyArchetypePreset();
    }

    public void SetArchetype(EnemyArchetype value)
    {
        archetype = value;
        archetypeApplied = false;
        ApplyArchetypePreset();
    }

    private void ApplyArchetypePreset()
    {
        if (archetypeApplied)
        {
            return;
        }

        archetypeApplied = true;

        switch (archetype)
        {
            case EnemyArchetype.Ictericia:
                speed = 1.9f;
                timeToKill = 2f;
                Tint(new Color(0.9f, 0.85f, 0.35f, 1f));
                break;
            case EnemyArchetype.Ectogangue:
                speed = 1.9f;
                timeToKill = 2.8f;
                Tint(new Color(0.58f, 0.9f, 1f, 1f));
                break;
            case EnemyArchetype.Tita:
                speed = 0.95f;
                timeToKill = 4.6f;
                baseConeAngle = 32f;
                Tint(new Color(0.66f, 0.42f, 0.82f, 1f));
                break;
            case EnemyArchetype.Espectro:
                speed = 3.15f;
                timeToKill = 0.95f;
                baseLightRange = 14f;
                Tint(new Color(0.85f, 1f, 0.95f, 0.85f));
                break;
            default:
                speed = 1.5f;
                timeToKill = 2f;
                Tint(new Color(1f, 1f, 1f, 1f));
                break;
        }
    }

    private void Tint(Color color)
    {
        archetypeBaseColor = color;

        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = color;
    }

    void Update()
    {
        if (isDying)
        {
            return;
        }

        if (target == null || flashlight == null)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        Vector2 toEnemy = (Vector2)(transform.position - flashlight.position);
        float distance = toEnemy.magnitude;
        float angle = Vector2.Angle(flashlight.up, toEnemy);

        float range = baseLightRange;
        float coneAngle = baseConeAngle;
        float killTimeMultiplier = 1f;

        if (UpgradeManager.Instance != null)
        {
            range += UpgradeManager.Instance.GetBeamRangeBonus();
            coneAngle += UpgradeManager.Instance.GetBeamAngleBonus();
            killTimeMultiplier = UpgradeManager.Instance.GetKillTimeMultiplier();
        }

        float effectiveKillTime = Mathf.Max(0.1f, timeToKill * killTimeMultiplier);

        bool isIlluminated = distance < range && angle < coneAngle;
        if (isIlluminated)
        {
            lightTimer += Time.deltaTime;
            ApplyLitFeedback(true);

            if (lightTimer >= effectiveKillTime)
            {
                StartCoroutine(PlayDeathAndDestroy());
            }
        }
        else
        {
            lightTimer = Mathf.Max(0, lightTimer - Time.deltaTime);
            ApplyLitFeedback(false);
        }
    }

    private void ApplyLitFeedback(bool illuminated)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Color litColor = Color.Lerp(archetypeBaseColor, Color.white, litFeedbackStrength);
        Color targetColor = illuminated ? litColor : archetypeBaseColor;
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * 14f);
    }

    private System.Collections.IEnumerator PlayDeathAndDestroy()
    {
        if (isDying)
        {
            yield break;
        }

        isDying = true;
        onEnemyKilled?.Invoke();
        AudioManager.Instance?.PlayEnemyKill();

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = false;
            }
        }

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.25f;
        while (elapsed < deathFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / deathFlashDuration);

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), t);
            }

            yield return null;
        }

        Vector2 dropPosition = transform.position;
        CoinPickupSpawner.Instance?.TrySpawnDropFromEnemy(dropPosition, archetype);
        BatteryPickupSpawner.Instance?.TrySpawnDropFromEnemy(dropPosition, archetype);

        PooledObject pooled = GetComponent<PooledObject>();
        if (pooled != null)
        {
            pooled.ReturnToPool();
            yield break;
        }

        Destroy(gameObject);
    }
}