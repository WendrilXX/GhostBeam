using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public static System.Action<int, int> onHealthChanged;

    public int maxHealth = 3;
    public float damageCooldown = 0.5f;
    public float contactDamageDistance = 0.8f;

    public int CurrentHealth { get; private set; }

    private float nextDamageTime;
    private BatterySystem batterySystem;

    private void Awake()
    {
        batterySystem = FindAnyObjectByType<BatterySystem>();
        CurrentHealth = maxHealth;
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Update()
    {
        TryApplyProximityDamage();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryApplyContactDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryApplyContactDamage(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryApplyContactDamage(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryApplyContactDamage(collision.collider);
    }

    private void TryApplyContactDamage(Collider2D other)
    {
        if (other == null)
        {
            return;
        }

        if (Time.time < nextDamageTime)
        {
            return;
        }

        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused || GameManager.Instance.IsInMainMenu))
        {
            return;
        }

        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy == null)
        {
            enemy = other.GetComponentInParent<EnemyController>();
        }

        if (enemy == null)
        {
            return;
        }

        if (ShouldTriggerImmediateGameOver())
        {
            GameManager.Instance?.TriggerGameOver();
            nextDamageTime = Time.time + damageCooldown;
            return;
        }

        TakeDamage(1);
        nextDamageTime = Time.time + damageCooldown;
    }

    private void TryApplyProximityDamage()
    {
        if (Time.time < nextDamageTime)
        {
            return;
        }

        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused || GameManager.Instance.IsInMainMenu))
        {
            return;
        }

        System.Collections.Generic.IReadOnlyList<EnemyController> enemies = EnemyController.ActiveEnemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, enemies[i].transform.position);
            if (distance > contactDamageDistance)
            {
                continue;
            }

            if (ShouldTriggerImmediateGameOver())
            {
                GameManager.Instance?.TriggerGameOver();
                nextDamageTime = Time.time + damageCooldown;
                return;
            }

            TakeDamage(1);
            nextDamageTime = Time.time + damageCooldown;
            return;
        }
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
        if (amount > 0)
        {
            AudioManager.Instance?.PlayPlayerDamage();
        }

        if (CurrentHealth <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }

    private bool ShouldTriggerImmediateGameOver()
    {
        if (batterySystem == null)
        {
            batterySystem = FindAnyObjectByType<BatterySystem>();
        }

        return batterySystem != null && batterySystem.IsDisabled;
    }
}
