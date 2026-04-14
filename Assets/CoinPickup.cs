using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public static System.Action<int, Vector3> onCoinCollected;

    private static readonly HashSet<CoinPickup> activeCoins = new HashSet<CoinPickup>();

    public static int ActiveCount => activeCoins.Count;

    public int coinValue = 1;
    public float autoDespawnSeconds = 14f;
    public float magnetRange = 2.4f;
    public float magnetSpeed = 8f;

    private float lifeTimer;
    private Transform luna;

    private void OnEnable()
    {
        activeCoins.Add(this);
        lifeTimer = 0f;
    }

    private void OnDisable()
    {
        activeCoins.Remove(this);
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

        if (luna != null)
        {
            Vector2 toLuna = luna.position - transform.position;
            float distance = toLuna.magnitude;
            if (distance <= magnetRange)
            {
                Vector3 step = (Vector3)(toLuna.normalized * magnetSpeed * Time.deltaTime);
                if (step.sqrMagnitude > toLuna.sqrMagnitude)
                {
                    step = toLuna;
                }

                transform.position += step;
            }
        }

        lifeTimer += Time.deltaTime;
        if (lifeTimer < autoDespawnSeconds)
        {
            return;
        }

        ReturnOrDestroy();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isLuna = other.GetComponent<LunaController>() != null || other.GetComponentInParent<LunaController>() != null;
        if (!isLuna)
        {
            return;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CollectCoins(coinValue);
        }

        onCoinCollected?.Invoke(coinValue, transform.position);
        AudioManager.Instance?.PlayCoinPickup();

        ReturnOrDestroy();
    }

    private void ReturnOrDestroy()
    {
        PooledObject pooled = GetComponent<PooledObject>();
        if (pooled != null)
        {
            pooled.ReturnToPool();
            return;
        }

        Destroy(gameObject);
    }
}
