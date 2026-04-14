using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager
{
    public void StartGameplayFromMenu()
    {
        if (!IsInMainMenu)
        {
            return;
        }

        // Fade out antes de iniciar gameplay
        if (SceneTransitioner.Instance != null)
        {
            SceneTransitioner.Instance.FadeOut(0.4f);
        }

        StartGameplayState();
    }

    public void ReturnToMainMenu()
    {
        startInMenuOnNextLoad = true;
        Time.timeScale = 1f;
        
        // Fade out antes de retornar ao menu
        if (SceneTransitioner.Instance != null)
        {
            SceneTransitioner.Instance.FadeOut(0.3f);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartCurrentScene()
    {
        startInMenuOnNextLoad = false;
        IsPaused = false;
        Time.timeScale = 1f;
        
        // Fade out antes de reiniciar
        if (SceneTransitioner.Instance != null)
        {
            SceneTransitioner.Instance.FadeOut(0.3f);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
