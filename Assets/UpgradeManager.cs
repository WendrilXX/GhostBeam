using UnityEngine;

/// <summary>
/// Gerencia todos os upgrades do jogo com persistência e efeitos automáticos.
/// Suporta múltiplas compras do mesmo upgrade com bonuses cumulativos.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    private const string BeamLevelKey = "Upgrade_BeamLevel";
    private const string PowerLevelKey = "Upgrade_PowerLevel";
    private const string BatteryLevelKey = "Upgrade_BatteryLevel";
    private const string FlashlightDamageKey = "Upgrade_FlashlightDamage";
    private const string BatteryCapacityKey = "Upgrade_BatteryCapacity";
    private const string MovementSpeedKey = "Upgrade_MovementSpeed";

    public static UpgradeManager Instance { get; private set; }
    public static System.Action onUpgradesChanged;
    public static System.Action<int, bool> onUpgradePurchased; // (upgradeId, success)

    [Header("Upgrade Limits")]
    public int maxBeamLevel = 5;
    public int maxPowerLevel = 5;
    public int maxBatteryLevel = 5;
    public int maxShopUpgradePurchases = 3;

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

    [Header("Shop Upgrades")]
    public float flashlightDamagePerUpgrade = 0.1f;   // +10%
    public float batteryCapacityPerUpgrade = 0.3f;    // +30%
    public float movementSpeedPerUpgrade = 0.2f;      // +20%

    public int BeamLevel { get; private set; }
    public int PowerLevel { get; private set; }
    public int BatteryLevel { get; private set; }

    // Shop upgrade levels (quantas vezes foram comprados)
    public int FlashlightDamageLevel { get; private set; }
    public int BatteryCapacityLevel { get; private set; }
    public int MovementSpeedLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    // ===== SHOP UPGRADES =====
    public bool TryPurchaseShopUpgrade(int upgradeId, int cost)
    {
        if (upgradeId < 1 || upgradeId > 3)
        {
            Debug.LogError($"[UpgradeManager] ID de upgrade inválido: {upgradeId}");
            onUpgradePurchased?.Invoke(upgradeId, false);
            return false;
        }

        // Gastar moedas
        if (ScoreManager.Instance == null || !ScoreManager.Instance.TrySpendCoins(cost))
        {
            Debug.Log($"[UpgradeManager] Moedas insuficientes para upgrade {upgradeId}");
            onUpgradePurchased?.Invoke(upgradeId, false);
            return false;
        }

        // Aplicar upgrade baseado no ID
        bool success = false;
        switch (upgradeId)
        {
            case 1: // Lanterna Melhorada
                if (FlashlightDamageLevel < maxShopUpgradePurchases)
                {
                    FlashlightDamageLevel++;
                    success = true;
                    Debug.Log($"[UpgradeManager] Flashlight Damage: +10% (Level {FlashlightDamageLevel}/{maxShopUpgradePurchases})");
                }
                break;

            case 2: // Bateria Maior
                if (BatteryCapacityLevel < maxShopUpgradePurchases)
                {
                    BatteryCapacityLevel++;
                    success = true;
                    Debug.Log($"[UpgradeManager] Battery Capacity: +30% (Level {BatteryCapacityLevel}/{maxShopUpgradePurchases})");
                }
                break;

            case 3: // Velocidade
                if (MovementSpeedLevel < maxShopUpgradePurchases)
                {
                    MovementSpeedLevel++;
                    success = true;
                    Debug.Log($"[UpgradeManager] Movement Speed: +20% (Level {MovementSpeedLevel}/{maxShopUpgradePurchases})");
                }
                break;
        }

        if (success)
        {
            SaveState();
            onUpgradesChanged?.Invoke();
        }

        onUpgradePurchased?.Invoke(upgradeId, success);
        return success;
    }

    /// <summary>
    /// Retorna o multiplicador de bônus para um upgrade da loja
    /// </summary>
    public float GetShopUpgradeBonus(int upgradeId)
    {
        switch (upgradeId)
        {
            case 1: return 1.0f + (FlashlightDamageLevel * flashlightDamagePerUpgrade);
            case 2: return 1.0f + (BatteryCapacityLevel * batteryCapacityPerUpgrade);
            case 3: return 1.0f + (MovementSpeedLevel * movementSpeedPerUpgrade);
            default: return 1.0f;
        }
    }

    /// <summary>
    /// Retorna descrição atualizada de um upgrade da loja
    /// </summary>
    public string GetShopUpgradeDescription(int upgradeId)
    {
        switch (upgradeId)
        {
            case 1:
            {
                float bonus = (FlashlightDamageLevel * flashlightDamagePerUpgrade * 100);
                return $"Dano +10%\nNível: {FlashlightDamageLevel}/{maxShopUpgradePurchases}\nBonus Total: {bonus:F0}%\nCusto: 50 moedas";
            }
            case 2:
            {
                float bonus = (BatteryCapacityLevel * batteryCapacityPerUpgrade * 100);
                return $"Duração +30%\nNível: {BatteryCapacityLevel}/{maxShopUpgradePurchases}\nBonus Total: {bonus:F0}%\nCusto: 75 moedas";
            }
            case 3:
            {
                float bonus = (MovementSpeedLevel * movementSpeedPerUpgrade * 100);
                return $"Movimento +20%\nNível: {MovementSpeedLevel}/{maxShopUpgradePurchases}\nBonus Total: {bonus:F0}%\nCusto: 60 moedas";
            }
            default:
                return "Upgrade desconhecido";
        }
    }

    /// <summary>
    /// Verifica se um upgrade pode ser comprado (ainda tem slots disponíveis)
    /// </summary>
    public bool CanPurchaseShopUpgrade(int upgradeId)
    {
        switch (upgradeId)
        {
            case 1: return FlashlightDamageLevel < maxShopUpgradePurchases;
            case 2: return BatteryCapacityLevel < maxShopUpgradePurchases;
            case 3: return MovementSpeedLevel < maxShopUpgradePurchases;
            default: return false;
        }
    }

    private void LoadState()
    {
        BeamLevel = Mathf.Clamp(PlayerPrefs.GetInt(BeamLevelKey, 0), 0, maxBeamLevel);
        PowerLevel = Mathf.Clamp(PlayerPrefs.GetInt(PowerLevelKey, 0), 0, maxPowerLevel);
        BatteryLevel = Mathf.Clamp(PlayerPrefs.GetInt(BatteryLevelKey, 0), 0, maxBatteryLevel);
        
        // Carregar upgrades da loja
        FlashlightDamageLevel = Mathf.Clamp(PlayerPrefs.GetInt(FlashlightDamageKey, 0), 0, maxShopUpgradePurchases);
        BatteryCapacityLevel = Mathf.Clamp(PlayerPrefs.GetInt(BatteryCapacityKey, 0), 0, maxShopUpgradePurchases);
        MovementSpeedLevel = Mathf.Clamp(PlayerPrefs.GetInt(MovementSpeedKey, 0), 0, maxShopUpgradePurchases);
        
        Debug.Log($"[UpgradeManager] Upgrades carregados: Beam={BeamLevel}, Power={PowerLevel}, Battery={BatteryLevel}, Flash={FlashlightDamageLevel}, Speed={MovementSpeedLevel}");
        onUpgradesChanged?.Invoke();
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt(BeamLevelKey, BeamLevel);
        PlayerPrefs.SetInt(PowerLevelKey, PowerLevel);
        PlayerPrefs.SetInt(BatteryLevelKey, BatteryLevel);
        
        // Salvar upgrades da loja
        PlayerPrefs.SetInt(FlashlightDamageKey, FlashlightDamageLevel);
        PlayerPrefs.SetInt(BatteryCapacityKey, BatteryCapacityLevel);
        PlayerPrefs.SetInt(MovementSpeedKey, MovementSpeedLevel);
        
        PlayerPrefs.Save();
        Debug.Log("[UpgradeManager] Upgrades saved successfully");
    }
}
