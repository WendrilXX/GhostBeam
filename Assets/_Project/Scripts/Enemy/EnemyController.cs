using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using System.Collections;

namespace GhostBeam.Enemy
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float timeToKillWhileIlluminated = 3f;
        [SerializeField] private int damageOnContact = 1;

        private Transform playerTransform;
        private Light2D flashlight;
        private float illuminationTimer = 0f;
        private bool isDead = false;

        private SpriteRenderer spriteRenderer;
        private Color originalColor;

        public static event Action<Vector3, int> onEnemyKilled;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        private void Start()
        {
            var lunaController = FindAnyObjectByType<Player.LunaController>();
            if (lunaController != null)
            {
                playerTransform = lunaController.transform;
                flashlight = lunaController.GetComponentInChildren<Light2D>();
            }
        }

        private void Update()
        {
            if (isDead || playerTransform == null)
                return;

            MoveTowardPlayer();
            UpdateIllumination();
        }

        private void MoveTowardPlayer()
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                speed * Time.deltaTime
            );
        }

        private void UpdateIllumination()
        {
            bool isIlluminated = IsIlluminated();

            if (isIlluminated)
            {
                illuminationTimer += Time.deltaTime;
                
                // Feedback visual: lerp para branco
                float lerpValue = Mathf.Clamp01(illuminationTimer / timeToKillWhileIlluminated);
                spriteRenderer.color = Color.Lerp(originalColor, Color.white, lerpValue * 0.5f);

                if (illuminationTimer >= timeToKillWhileIlluminated)
                {
                    Die();
                }
            }
            else
            {
                illuminationTimer = 0f;
                spriteRenderer.color = originalColor;
            }
        }

        private bool IsIlluminated()
        {
            if (flashlight == null || !flashlight.enabled)
                return false;

            Vector2 toEnemy = (Vector2)(transform.position - flashlight.transform.position);
            float distance = toEnemy.magnitude;
            float angle = Vector2.Angle(flashlight.transform.up, toEnemy);

            return distance < 15f && angle < 35f;
        }

        private void Die()
        {
            if (isDead)
                return;

            isDead = true;

            // Feedback visual de morte
            spriteRenderer.color = Color.red;
            
            // Score
            if (Managers.ScoreManager.Instance != null)
            {
                Managers.ScoreManager.Instance.AddScore(15); // +15 pontos por kill
            }

            // Evento para spawners
            onEnemyKilled?.Invoke(transform.position, 1);

            // Vibração
            if (Managers.SettingsManager.Instance != null)
            {
                Managers.SettingsManager.Instance.Vibrate();
            }

            // Destruir
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                var healthSystem = collision.GetComponent<Gameplay.HealthSystem>();
                if (healthSystem != null && !isDead)
                {
                    healthSystem.TakeDamage(damageOnContact);
                    
                    if (Managers.SettingsManager.Instance != null)
                    {
                        Managers.SettingsManager.Instance.Vibrate();
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 15f);
            #endif
        }

        public void Reset()
        {
            isDead = false;
            illuminationTimer = 0f;
            spriteRenderer.color = originalColor;
        }
    }
}
