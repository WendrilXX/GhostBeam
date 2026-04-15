using UnityEngine;

/// <summary>
/// Integra Game Over com o sistema de saúde.
/// Mostra a tela de Game Over quando Luna morre.
/// </summary>
public class GameOverSystem : MonoBehaviour
{
    private UIManager uiManager;
    private ScoreManager scoreManager;
    private HealthSystem healthSystem;

    private void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        scoreManager = FindAnyObjectByType<ScoreManager>();
        healthSystem = FindAnyObjectByType<HealthSystem>();

        if (healthSystem != null)
            HealthSystem.onHealthChanged += CheckGameOver;
    }

    private void OnDisable()
    {
        if (healthSystem != null)
            HealthSystem.onHealthChanged -= CheckGameOver;
    }

    private void CheckGameOver(int current, int max)
    {
        if (current <= 0 && uiManager != null && scoreManager != null)
        {
            uiManager.ShowGameOver(scoreManager.Score, scoreManager.Highscore);
            GameManager.Instance?.TriggerGameOver();
        }
    }
}
