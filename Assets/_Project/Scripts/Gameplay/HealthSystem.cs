using UnityEngine;
using GhostBeam.Managers;
using System;
using System.Collections;

namespace GhostBeam.Gameplay
{
    public class HealthSystem : MonoBehaviour
    {
        private const string HealthUpgradeTierKey = "Upgrade_Health_Tier";

        [SerializeField] private int maxHealth = 3;
        [SerializeField] private int maxHealthCap = 6;
        [SerializeField] private float damageFlashDuration = 0.3f;  // Duration of red flash

        private int currentHealth;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine damageFlashCoroutine;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        public static event Action<int> onHealthChanged;
        public static event Action onHealthDepleted;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;

            int healthTier = Mathf.Clamp(PlayerPrefs.GetInt(HealthUpgradeTierKey, 0), 0, 3);
            int baseMaxHealth = maxHealth;
            maxHealth = Mathf.Min(maxHealthCap, baseMaxHealth + healthTier);
            currentHealth = maxHealth;
        }

        public void TakeDamage(int amount = 1)
        {
            currentHealth -= amount;
            if (currentHealth < 0)
                currentHealth = 0;

            // Visual feedback: flash red
            if (spriteRenderer != null)
            {
                if (damageFlashCoroutine != null)
                    StopCoroutine(damageFlashCoroutine);
                damageFlashCoroutine = StartCoroutine(DamageFlash());
            }

            onHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                onHealthDepleted?.Invoke();
                Debug.Log("[HealthSystem] Health depleted - triggering game over");
                Managers.GameManager.TriggerGameOver();
            }
        }

        private IEnumerator DamageFlash()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(damageFlashDuration);
            spriteRenderer.color = originalColor;
        }

        public void Heal(int amount = 1)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            onHealthChanged?.Invoke(currentHealth);
        }

        public void SetMaxHealth(int value)
        {
            maxHealth = value;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            onHealthChanged?.Invoke(currentHealth);
        }

        public void Reset()
        {
            currentHealth = maxHealth;
            onHealthChanged?.Invoke(currentHealth);
        }
    }
}
