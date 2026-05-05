using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace GhostBeam.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private bool isPaused = false;
        private bool isGameOver = false;
        public bool IsPaused => isPaused;
        public bool IsGameOver => isGameOver;

        public static event Action onGameOver;
        public static event Action<bool> onPauseChanged;
        public static event Action<bool> onMainMenuChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (isGameOver)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public static void TriggerGameOver()
        {
            if (Instance != null)
            {
                if (Instance.isGameOver)
                    return;

                Instance.isGameOver = true;
                Instance.isPaused = false;
                Time.timeScale = 0f;
                onGameOver?.Invoke();
            }
        }

        public static void TogglePause()
        {
            if (Instance != null)
            {
                if (Instance.isGameOver)
                    return;

                SetPause(!Instance.isPaused);
            }
        }

        public static void SetPause(bool value)
        {
            if (Instance != null && Instance.isPaused != value)
            {
                if (Instance.isGameOver)
                    return;

                Instance.isPaused = value;
                Time.timeScale = value ? 0f : 1f;
                onPauseChanged?.Invoke(value);
            }
        }

        public static void ReturnToMainMenu()
        {
            if (Instance != null)
            {
                Instance.isPaused = false;
                Instance.isGameOver = false;
            }

            Time.timeScale = 1f;
            onMainMenuChanged?.Invoke(true);
            SceneManager.LoadScene("MainMenu");
        }

        public static void RestartScene()
        {
            if (Instance != null)
            {
                Instance.isPaused = false;
                Instance.isGameOver = false;
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
