using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GhostBeam.UI
{
    public class GameOverPanelController : MonoBehaviour
    {
        [SerializeField] private Canvas gameOverCanvas;
        [SerializeField] private TextMeshProUGUI txtScoreFinal;
        [SerializeField] private TextMeshProUGUI txtHighScoreFinal;
        [SerializeField] private TextMeshProUGUI txtCoinsFinal;
        [SerializeField] private Button btnRestart;
        [SerializeField] private Button btnMenu;

        private void Awake()
        {
            if (gameOverCanvas != null)
                gameOverCanvas.enabled = false;
        }

        private void Start()
        {
            Managers.GameManager.onGameOver += ShowGameOver;
            
            if (btnRestart != null)
                btnRestart.onClick.AddListener(() => Managers.GameManager.RestartScene());
            if (btnMenu != null)
                btnMenu.onClick.AddListener(() => Managers.GameManager.ReturnToMainMenu());
        }

        private void ShowGameOver()
        {
            if (gameOverCanvas != null)
                gameOverCanvas.enabled = true;

            var scoreManager = Managers.ScoreManager.Instance;
            if (scoreManager != null)
            {
                if (txtScoreFinal != null)
                    txtScoreFinal.text = $"Score: {scoreManager.CurrentScore}";
                if (txtHighScoreFinal != null)
                    txtHighScoreFinal.text = $"Best: {scoreManager.HighScore}";
                if (txtCoinsFinal != null)
                    txtCoinsFinal.text = $"Moedas: {scoreManager.Coins}";
            }
        }

        private void OnDestroy()
        {
            if (Managers.GameManager.Instance != null)
                Managers.GameManager.onGameOver -= ShowGameOver;
        }
    }
}
