using UnityEngine;

public partial class GameManager
{
    public void TriggerGameOver()
    {
        if (IsGameOver || IsInMainMenu)
        {
            return;
        }

        IsGameOver = true;
        IsPaused = false;
        IsInMainMenu = false;
        Time.timeScale = 0f;
        onGameOver?.Invoke();
        onPauseChanged?.Invoke(false);
        onMainMenuChanged?.Invoke(false);
    }

    public void TogglePause()
    {
        SetPause(!IsPaused);
    }

    public void SetPause(bool value)
    {
        if (IsGameOver || IsInMainMenu)
        {
            return;
        }

        if (IsPaused == value)
        {
            return;
        }

        IsPaused = value;
        Time.timeScale = IsPaused ? 0f : 1f;
        onPauseChanged?.Invoke(IsPaused);
    }

    private void EnterMainMenuState()
    {
        Debug.Log("[GameManager] 🎮 EnterMainMenuState() - Menu principal ativo");
        
        IsGameOver = false;
        IsInMainMenu = true;
        IsPaused = true;
        Time.timeScale = 0f;

        onPauseChanged?.Invoke(true);
        Debug.Log("[GameManager] Disparando onMainMenuChanged(true)");
        onMainMenuChanged?.Invoke(true);
    }

    private void StartGameplayState()
    {
        IsGameOver = false;
        IsInMainMenu = false;
        IsPaused = false;
        Time.timeScale = 1f;

        onPauseChanged?.Invoke(false);
        onMainMenuChanged?.Invoke(false);
    }
}
