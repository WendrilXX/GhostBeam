using UnityEngine;

/// <summary>
/// Sistema de pausa que integra com UIManager.
/// Mostra Pause Menu quando pausa o jogo.
/// </summary>
public class PauseSystem : MonoBehaviour
{
    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        GameManager.onPauseChanged += HandlePauseChanged;
    }

    private void OnDisable()
    {
        GameManager.onPauseChanged -= HandlePauseChanged;
    }

    private void Update()
    {
        // Atalho: ESC para pausar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsInMainMenu && !GameManager.Instance.IsGameOver)
            {
                GameManager.Instance.TogglePause();
            }
        }
    }

    private void HandlePauseChanged(bool isPaused)
    {
        if (uiManager == null)
            return;

        if (isPaused && !GameManager.Instance.IsInMainMenu)
        {
            uiManager.ShowPauseMenu();
        }
        else if (!isPaused)
        {
            uiManager.ShowGameplay();
        }
    }
}
