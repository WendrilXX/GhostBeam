using UnityEngine;

public class SkinManager : MonoBehaviour
{
    private const string LunaUnlockedKey = "Skin_LunaUnlocked";
    private const string FlashUnlockedKey = "Skin_FlashUnlocked";
    private const string LunaEquippedKey = "Skin_LunaEquipped";
    private const string FlashEquippedKey = "Skin_FlashEquipped";

    public static SkinManager Instance { get; private set; }
    public static System.Action onSkinsChanged;

    [Header("Costs")]
    public int lunaSkinCost = 40;
    public int flashSkinCost = 45;

    [Header("Skin Colors")]
    public Color lunaSkinColor = new Color(0.72f, 0.9f, 1f, 1f);
    public Color flashSkinColor = new Color(1f, 0.55f, 0.25f, 1f);

    private bool lunaSkinUnlocked;
    private bool flashSkinUnlocked;
    private bool lunaSkinEquipped;
    private bool flashSkinEquipped;

    private SpriteRenderer lunaRenderer;
    private SpriteRenderer flashlightRenderer;
    private Color defaultLunaColor = Color.white;
    private Color defaultFlashColor = Color.white;
    private bool cachedDefaults;

    public bool LunaSkinUnlocked => lunaSkinUnlocked;
    public bool FlashSkinUnlocked => flashSkinUnlocked;
    public bool LunaSkinEquipped => lunaSkinEquipped;
    public bool FlashSkinEquipped => flashSkinEquipped;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadState();
        CacheRenderers();
        ApplyVisuals();
    }

    public bool TryUnlockOrEquipLunaSkin()
    {
        if (!lunaSkinUnlocked)
        {
            if (ScoreManager.Instance == null || !ScoreManager.Instance.TrySpendCoins(lunaSkinCost))
            {
                return false;
            }

            lunaSkinUnlocked = true;
        }

        lunaSkinEquipped = true;
        SaveState();
        ApplyVisuals();
        onSkinsChanged?.Invoke();
        return true;
    }

    public bool TryUnlockOrEquipFlashSkin()
    {
        if (!flashSkinUnlocked)
        {
            if (ScoreManager.Instance == null || !ScoreManager.Instance.TrySpendCoins(flashSkinCost))
            {
                return false;
            }

            flashSkinUnlocked = true;
        }

        flashSkinEquipped = true;
        SaveState();
        ApplyVisuals();
        onSkinsChanged?.Invoke();
        return true;
    }

    public int GetLunaSkinCostOrZero()
    {
        return lunaSkinUnlocked ? 0 : lunaSkinCost;
    }

    public int GetFlashSkinCostOrZero()
    {
        return flashSkinUnlocked ? 0 : flashSkinCost;
    }

    private void CacheRenderers()
    {
        if (lunaRenderer == null)
        {
            LunaController luna = FindAnyObjectByType<LunaController>();
            if (luna != null)
            {
                lunaRenderer = luna.GetComponentInChildren<SpriteRenderer>(true);
            }
        }

        if (flashlightRenderer == null)
        {
            FlashlightController flash = FindAnyObjectByType<FlashlightController>();
            if (flash != null)
            {
                flashlightRenderer = flash.GetComponentInChildren<SpriteRenderer>(true);
            }
        }

        if (!cachedDefaults)
        {
            if (lunaRenderer != null)
            {
                defaultLunaColor = lunaRenderer.color;
            }

            if (flashlightRenderer != null)
            {
                defaultFlashColor = flashlightRenderer.color;
            }

            cachedDefaults = true;
        }
    }

    private void ApplyVisuals()
    {
        CacheRenderers();

        if (lunaRenderer != null)
        {
            lunaRenderer.color = lunaSkinEquipped ? lunaSkinColor : defaultLunaColor;
        }

        if (flashlightRenderer != null)
        {
            flashlightRenderer.color = flashSkinEquipped ? flashSkinColor : defaultFlashColor;
        }
    }

    private void LoadState()
    {
        lunaSkinUnlocked = PlayerPrefs.GetInt(LunaUnlockedKey, 0) == 1;
        flashSkinUnlocked = PlayerPrefs.GetInt(FlashUnlockedKey, 0) == 1;
        lunaSkinEquipped = PlayerPrefs.GetInt(LunaEquippedKey, 0) == 1;
        flashSkinEquipped = PlayerPrefs.GetInt(FlashEquippedKey, 0) == 1;

        if (lunaSkinEquipped && !lunaSkinUnlocked)
        {
            lunaSkinEquipped = false;
        }

        if (flashSkinEquipped && !flashSkinUnlocked)
        {
            flashSkinEquipped = false;
        }
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt(LunaUnlockedKey, lunaSkinUnlocked ? 1 : 0);
        PlayerPrefs.SetInt(FlashUnlockedKey, flashSkinUnlocked ? 1 : 0);
        PlayerPrefs.SetInt(LunaEquippedKey, lunaSkinEquipped ? 1 : 0);
        PlayerPrefs.SetInt(FlashEquippedKey, flashSkinEquipped ? 1 : 0);
        PlayerPrefs.Save();
    }
}
