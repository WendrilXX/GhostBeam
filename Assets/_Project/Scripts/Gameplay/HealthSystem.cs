using UnityEngine;
using GhostBeam.Managers;
using System;

namespace GhostBeam.Gameplay
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;

        private int currentHealth;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        public static event Action<int> onHealthChanged;
        public static event Action onHealthDepleted;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int amount = 1)
        {
            currentHealth -= amount;
            onHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
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
