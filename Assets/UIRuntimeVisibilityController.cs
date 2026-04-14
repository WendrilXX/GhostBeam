using UnityEngine;

public class UIRuntimeVisibilityController : MonoBehaviour
{
    public GameObject hudCanvasRoot;

    private void OnEnable()
    {
        GameManager.onMainMenuChanged += HandleMainMenuChanged;
        SyncWithGameState();
    }

    private void OnDisable()
    {
        GameManager.onMainMenuChanged -= HandleMainMenuChanged;
    }

    private void HandleMainMenuChanged(bool isInMainMenu)
    {
        if (hudCanvasRoot != null)
        {
            hudCanvasRoot.SetActive(!isInMainMenu);
        }
    }

    private void SyncWithGameState()
    {
        bool isInMainMenu = GameManager.Instance != null && GameManager.Instance.IsInMainMenu;
        HandleMainMenuChanged(isInMainMenu);
    }
}
