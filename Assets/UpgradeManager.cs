using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private const string BeamLevelKey = "Upgrade_BeamLevel";
    private const string PowerLevelKey = "Upgrade_PowerLevel";
    private const string BatteryLevelKey = "Upgrade_BatteryLevel";

    public static UpgradeManager Instance { get; private set; }
    public static System.Action onUpgradesChanged;

    [Header("Upgrade Limits")]
    public int maxBeamLevel = 5;
    public int maxPowerLevel = 5;
    public int maxBatteryLevel = 5;

    [Header("Beam Upgrade")]
    public int beamBaseCost = 25;
    public int beamCostStep = 15;
    public float beamAngleBonusPerLevel = 4f;
    public float beamRangeBonusPerLevel = 0.75f;

    [Header("Power Upgrade")]
    public int powerBaseCost = 30;
    public int powerCostStep = 20;
    [Range(0.01f, 0.5f)] public float killTimeReductionPerLevel = 0.1f;

    [Header("Battery Upgrade")]
    public int batteryBaseCost = 40;
    public int batteryCostStep = 25;
    [Range(0.05f, 0.5f)] public float batteryCapacityBonusPerLevel = 0.25f;

    public int BeamLevel { get; private set; }
    public int PowerLevel { get; private set; }
    public int BatteryLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadState();
    }

    public float GetBeamAngleBonus()
    {
        return BeamLevel * beamAngleBonusPerLevel;
    }

    public float GetBeamRangeBonus()
    {
        return BeamLevel * beamRangeBonusPerLevel;
    }

    public float GetKillTimeMultiplier()
    {
        float multiplier = 1f - (PowerLevel * killTimeReductionPerLevel);
        return Mathf.Clamp(multiplier, 0.35f, 1f);
    }

    public float GetBatteryCapacityMultiplier()
    {
        return 1f + (BatteryLevel * batteryCapacityBonusPerLevel);
    }

    public int GetBeamUpgradeCost()
    {
        if (BeamLevel >= maxBeamLevel)
        {
            return -1;
        }

        return beamBaseCost + (BeamLevel * beamCostStep);
    }

    public int GetPowerUpgradeCost()
    {
        if (PowerLevel >= maxPowerLevel)
        {
            return -1;
        }

        return powerBaseCost + (PowerLevel * powerCostStep);
    }

    public int GetBatteryUpgradeCost()
    {
        if (BatteryLevel >= maxBatteryLevel)
        {
            return -1;
        }

        return batteryBaseCost + (BatteryLevel * batteryCostStep);
    }

    public bool TryPurchaseBeamUpgrade()
    {
        int cost = GetBeamUpgradeCost();
        if (cost < 0)
        {
            return false;
        }

        if (ScoreManager.Instance == null || !ScoreManager.Instance.TrySpendCoins(cost))
        {
            return false;
        }

        BeamLevel++;
        SaveState();
        onUpgradesChanged?.Invoke();
        return true;
    }

    public bool TryPurchasePowerUpgrade()
    {
        int cost = GetPowerUpgradeCost();
        if (cost < 0)
        {
            return false;
        }

        if (ScoreManager.Instance == null || !ScoreManager.Instance.TrySpendCoins(cost))
        {
            return false;
        }

        PowerLevel++;
        SaveState();
        onUpgradesChanged?.Invoke();
        return true;
    }

    public bool TryPurchaseBatteryUpgrade()
    {
        int cost = GetBatteryUpgradeCost();
        if (cost < 0)
        {
            return false;
        }

        if (ScoreManager.Instance == null || !ScoreManager.Instance.TrySpendCoins(cost))
        {
            return false;
        }

        BatteryLevel++;
        SaveState();
        onUpgradesChanged?.Invoke();
        return true;
    }

    private void LoadState()
    {
        BeamLevel = Mathf.Clamp(PlayerPrefs.GetInt(BeamLevelKey, 0), 0, maxBeamLevel);
        PowerLevel = Mathf.Clamp(PlayerPrefs.GetInt(PowerLevelKey, 0), 0, maxPowerLevel);
        BatteryLevel = Mathf.Clamp(PlayerPrefs.GetInt(BatteryLevelKey, 0), 0, maxBatteryLevel);
        onUpgradesChanged?.Invoke();
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt(BeamLevelKey, BeamLevel);
        PlayerPrefs.SetInt(PowerLevelKey, PowerLevel);
        PlayerPrefs.SetInt(BatteryLevelKey, BatteryLevel);
        PlayerPrefs.Save();
    }
}
