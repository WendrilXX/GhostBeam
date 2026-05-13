using UnityEngine;
using GhostBeam.Managers;
using System;

namespace GhostBeam.Gameplay
{
    public class BatterySystem : MonoBehaviour
    {
        private const string BatteryUpgradeTierKey = "Upgrade_Battery_Tier";
        private const int MaxBatteryTier = 3;

        [SerializeField] private float maxBattery = 150f;
        [SerializeField] private float minDrainPercentPerSecond = 2f;  // Mínimo 2% por segundo
        [SerializeField] private float maxDrainPercentPerSecond = 7f;  // Máximo 7% por segundo

        private float currentBattery;
        private bool isLighting = false;
        private bool hasDepletedOnce = false;  // Garantir que só dispara uma vez

        public float CurrentBattery => currentBattery;
        public float MaxBattery => maxBattery;
        public float BatteryPercent => currentBattery / maxBattery;

        public static event Action<float> onBatteryChanged;
        public static event Action onBatteryDepleted;

        private void Awake()
        {
            int batteryTier = Mathf.Clamp(PlayerPrefs.GetInt(BatteryUpgradeTierKey, 0), 0, 3);
            float baseMaxBattery = maxBattery;
            float tierFactor = batteryTier / (float)MaxBatteryTier;
            maxBattery = baseMaxBattery * (1f + tierFactor);

            currentBattery = maxBattery;
        }

        private void Update()
        {
            if (Managers.GameManager.Instance == null || Managers.GameManager.Instance.IsPaused)
                return;

            if (!Managers.GameplayIntroState.AllowGameplay)
                return;

            if (isLighting)
            {
                // Drain between 2-7% per second randomly
                float drainPercentPerSecond = UnityEngine.Random.Range(minDrainPercentPerSecond, maxDrainPercentPerSecond);
                float drainAmount = (maxBattery * drainPercentPerSecond / 100f) * Time.deltaTime;
                currentBattery -= drainAmount;
            }
            // BATERIA SÓ RECARREGA AO MATAR INIMIGOS, não automaticamente

            // SEMPRE verificar se bateria chegou a 0 (não só enquanto iluminando!)
            if (currentBattery <= 0 && !hasDepletedOnce)
            {
                currentBattery = 0;
                hasDepletedOnce = true;
                OnBatteryDepleted();
            }

            onBatteryChanged?.Invoke(currentBattery);
        }

        public void SetLighting(bool active)
        {
            isLighting = active;
        }

        public void Recharge(float amount)
        {
            currentBattery = Mathf.Min(currentBattery + amount, maxBattery);
            onBatteryChanged?.Invoke(currentBattery);
        }

        private void OnBatteryDepleted()
        {
            isLighting = false;
            onBatteryDepleted?.Invoke();
            Debug.Log("[BatterySystem] Battery depleted - triggering game over");
            Managers.GameManager.TriggerGameOver();
        }

        public void UpgradeMaxBattery(float additionalCapacity)
        {
            maxBattery += additionalCapacity;
            currentBattery = Mathf.Min(currentBattery, maxBattery);
            onBatteryChanged?.Invoke(currentBattery);
        }

        public void Reset()
        {
            currentBattery = maxBattery;
            isLighting = false;
            hasDepletedOnce = false;  // Resetar flag para novo jogo
            onBatteryChanged?.Invoke(currentBattery);
        }
    }
}
