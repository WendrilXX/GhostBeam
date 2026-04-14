using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// ShopScreenController - Gerencia a tela da Loja integrada no Menu Principal
/// </summary>
public class ShopScreenController : MonoBehaviour
{
    public GameObject shopPanelRoot;
    public TMP_Text shopCoinsText;
    public Button[] buyButtons;
    public Button closeButton;

    private ScoreManager scoreManager;
    private const string ShopPanelObjectName = "PanelLoja";

    private void Awake()
    {
        AutoAssignIfMissing();
    }

    private void OnEnable()
    {
        ScoreManager.onCoinsChanged += UpdateCoinDisplay;
        GameManager.onMainMenuChanged += HandleMainMenuChanged;
    }

    private void OnDisable()
    {
        ScoreManager.onCoinsChanged -= UpdateCoinDisplay;
        GameManager.onMainMenuChanged -= HandleMainMenuChanged;
    }

    public void SetVisible(bool visible)
    {
        if (shopPanelRoot != null)
        {
            shopPanelRoot.SetActive(visible);

            if (visible)
            {
                scoreManager = FindAnyObjectByType<ScoreManager>();
                UpdateCoinDisplay(scoreManager != null ? scoreManager.Coins : 0);
                SetupButtons();
            }
        }
    }

    private void SetupButtons()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => SetVisible(false));
        }

        if (buyButtons != null && buyButtons.Length > 0)
        {
            for (int i = 0; i < buyButtons.Length; i++)
            {
                int index = i;
                buyButtons[i].onClick.AddListener(() => HandleBuy(index));
            }
        }
    }

    private void HandleBuy(int itemIndex)
    {
        Debug.Log($"[Shop] Tentativa de compra do item {itemIndex}");
        // TODO: Integrar com UpgradeManager/SkinManager
    }

    private void UpdateCoinDisplay(int coins)
    {
        if (shopCoinsText != null)
        {
            shopCoinsText.text = $"MOEDAS: {coins}";
        }
    }

    private void HandleMainMenuChanged(bool isVisible)
    {
        if (!isVisible && shopPanelRoot != null)
        {
            shopPanelRoot.SetActive(false);
        }
    }

    private void AutoAssignIfMissing()
    {
        if (shopPanelRoot == null)
        {
            Transform found = transform.Find(ShopPanelObjectName);
            if (found != null)
                shopPanelRoot = found.gameObject;
        }
    }
}
