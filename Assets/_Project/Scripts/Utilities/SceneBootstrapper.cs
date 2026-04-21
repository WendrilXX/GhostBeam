using UnityEngine;
using UnityEngine.SceneManagement;

namespace GhostBeam.Utilities
{
    public class SceneBootstrapper : MonoBehaviour
    {
        [SerializeField] private bool loadGameplay = false;

        private void Awake()
        {
            // Assegurar que os managers singleton estão inicializados
            if (Managers.GameManager.Instance == null)
            {
                var gmObj = new GameObject("GameManager");
                gmObj.AddComponent<Managers.GameManager>();
            }

            if (Managers.ScoreManager.Instance == null)
            {
                var smObj = new GameObject("ScoreManager");
                smObj.AddComponent<Managers.ScoreManager>();
            }

            if (Managers.AudioManager.Instance == null)
            {
                var amObj = new GameObject("AudioManager");
                amObj.AddComponent<Managers.AudioManager>();
            }

            if (Managers.SettingsManager.Instance == null)
            {
                var setObj = new GameObject("SettingsManager");
                setObj.AddComponent<Managers.SettingsManager>();
            }
        }

        private void Start()
        {
            if (loadGameplay && SceneManager.GetActiveScene().name != "Gameplay")
            {
                SceneManager.LoadScene("Gameplay");
            }
        }
    }
}
