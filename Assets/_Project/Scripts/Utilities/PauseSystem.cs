using UnityEngine;
using UnityEngine.UI;

namespace GhostBeam.Utilities
{
    public class PauseSystem : MonoBehaviour
    {
        [SerializeField] private Canvas pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (pausePanel != null)
                pausePanel.enabled = false;

            if (resumeButton != null)
                resumeButton.onClick.AddListener(() => Managers.GameManager.TogglePause());

            if (quitButton != null)
                quitButton.onClick.AddListener(() => Managers.GameManager.ReturnToMainMenu());

            Managers.GameManager.onPauseChanged += OnPauseChanged;
        }

        private void OnPauseChanged(bool isPaused)
        {
            if (pausePanel != null)
                pausePanel.enabled = isPaused;
        }

        private void OnDestroy()
        {
            if (Managers.GameManager.Instance != null)
                Managers.GameManager.onPauseChanged -= OnPauseChanged;
        }
    }
}
