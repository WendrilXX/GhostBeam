using UnityEngine;
using System.Collections.Generic;

public class BatteryPickup : MonoBehaviour
{
    private static readonly HashSet<BatteryPickup> activePickups = new HashSet<BatteryPickup>();

    public static int ActiveCount => activePickups.Count;
    public static System.Action<float, Vector3> onBatteryCollected;

    public float restoreAmount = 25f;
    public float magnetRange = 2.2f;
    public float magnetSpeed = 7f;

    private BatterySystem batterySystem;
    private Transform luna;

    private void Awake()
    {
        // Garantir que tem Rigidbody2D para colisões funcionarem
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Garantir que tem CircleCollider2D como trigger
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
        }
        collider.isTrigger = true;
    }

    private void Start()
    {
        batterySystem = FindAnyObjectByType<BatterySystem>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (luna == null)
        {
            LunaController lunaController = FindAnyObjectByType<LunaController>();
            if (lunaController != null)
            {
                luna = lunaController.transform;
            }
        }

        if (luna == null)
        {
            return;
        }

        Vector2 toLuna = luna.position - transform.position;
        float distance = toLuna.magnitude;
        if (distance > magnetRange)
        {
            return;
        }

        Vector3 step = (Vector3)(toLuna.normalized * magnetSpeed * Time.deltaTime);
        if (step.sqrMagnitude > toLuna.sqrMagnitude)
        {
            step = toLuna;
        }

        transform.position += step;
    }

    private void OnEnable()
    {
        activePickups.Add(this);
    }

    private void OnDisable()
    {
        activePickups.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (batterySystem == null)
        {
            batterySystem = FindAnyObjectByType<BatterySystem>();
        }

        if (batterySystem == null)
        {
            return;
        }

        bool isLuna = other.GetComponent<LunaController>() != null || other.GetComponentInParent<LunaController>() != null;
        if (!isLuna)
        {
            return;
        }

        batterySystem.RestoreBattery(restoreAmount);
        onBatteryCollected?.Invoke(restoreAmount, transform.position);
        AudioManager.Instance?.PlayBatteryPickup();
        PooledObject pooled = GetComponent<PooledObject>();
        if (pooled != null)
        {
            pooled.ReturnToPool();
            return;
        }

        Destroy(gameObject);
    }
}
