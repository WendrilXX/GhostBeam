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
        [SerializeField] private float drainRate = 2f;

        private float currentBattery;
        private bool isLighting = false;

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

            if (isLighting)
            {
                currentBattery -= drainRate * Time.deltaTime;
                if (currentBattery <= 0)
                {
                    currentBattery = 0;
                    OnBatteryDepleted();
                }
            }
            else
            {
                // Recarrega lentamente quando não ilumina
                currentBattery = Mathf.Min(currentBattery + (drainRate * 0.3f) * Time.deltaTime, maxBattery);
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
            onBatteryChanged?.Invoke(currentBattery);
        }
    }
}
