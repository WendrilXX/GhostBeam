using UnityEngine;

namespace GhostBeam.UI
{
    public class UIBootstrapper : MonoBehaviour
    {
        [SerializeField] private Canvas hudCanvas;
        [SerializeField] private Canvas gameOverCanvas;
        [SerializeField] private Canvas pauseCanvas;

        private void Awake()
        {
            // Initialize canvas if not assigned
            if (hudCanvas == null)
                hudCanvas = FindAnyObjectByType<Canvas>();
            if (gameOverCanvas == null)
                gameOverCanvas = FindAnyObjectByType<Canvas>();

            ConfigureCanvases();
        }

        private void ConfigureCanvases()
        {
            // Make sure canvases are on proper layers
            if (hudCanvas != null)
            {
                hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            if (gameOverCanvas != null)
            {
                gameOverCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }

        private void Start()
        {
            // Subscribe to pause events
            Managers.GameManager.onPauseChanged += OnPauseChanged;
        }

        private void OnPauseChanged(bool isPaused)
        {
            if (pauseCanvas != null)
                pauseCanvas.enabled = isPaused;
        }

        private void OnDestroy()
        {
            if (Managers.GameManager.Instance != null)
                Managers.GameManager.onPauseChanged -= OnPauseChanged;
        }
    }
}
