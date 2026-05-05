using UnityEngine;
using GhostBeam.Managers;
using System;

namespace GhostBeam.Gameplay
{
    public class HealthSystem : MonoBehaviour
    {
        private const string HealthUpgradeTierKey = "Upgrade_Health_Tier";

        [SerializeField] private int maxHealth = 3;
        [SerializeField] private int maxHealthCap = 6;

        private int currentHealth;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        public static event Action<int> onHealthChanged;
        public static event Action onHealthDepleted;

        private void Awake()
        {
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

            onHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                onHealthDepleted?.Invoke();
                Managers.GameManager.TriggerGameOver();
            }
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
