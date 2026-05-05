using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GhostBeam.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

#if UNITY_EDITOR
        private const string DefaultMenuClickSfxPath = "Assets/_Project/Audio/SFX/Edited/snd_base_boss_prehit.ogg";
#endif

        [Header("Music")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;

        [Header("SFX")]
        [SerializeField] private AudioClip menuClickSfx;

        private AudioSource musicSource;
        private float masterVolume = 1f;
        private string lastMusicSceneName;

        public float MasterVolume 
        { 
            get => masterVolume;
            set 
            { 
                masterVolume = Mathf.Clamp01(value);
                if (musicSource != null)
                    musicSource.volume = masterVolume;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            musicSource = GetComponent<AudioSource>();
            LoadSettings();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (menuClickSfx == null)
            {
                menuClickSfx = AssetDatabase.LoadAssetAtPath<AudioClip>(DefaultMenuClickSfxPath);
            }
        }
#endif

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource == null || clip == null)
                return;

            if (musicSource.isPlaying && musicSource.clip == clip)
                return;

            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = masterVolume;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
                return;

            AudioSource.PlayClipAtPoint(clip, Vector3.zero, volume * masterVolume);
        }

        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null)
                return;

            AudioSource.PlayClipAtPoint(clip, position, volume * masterVolume);
        }

        private void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            if (musicSource != null)
                musicSource.volume = masterVolume;
        }

        public void PlayMenuMusic()
        {
            if (menuMusic != null)
                PlayMusic(menuMusic, true);
        }

        public void PlayGameplayMusic()
        {
            if (gameplayMusic != null)
                PlayMusic(gameplayMusic, true);
        }

        public void PlayMenuClick()
        {
            if (menuClickSfx != null)
                PlaySFX(menuClickSfx, 1f);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == lastMusicSceneName)
                return;

            if (scene.name == "MainMenu")
            {
                PlayMenuMusic();
                lastMusicSceneName = scene.name;
                return;
            }

            if (scene.name == "Gameplay")
            {
                PlayGameplayMusic();
                lastMusicSceneName = scene.name;
            }
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.Save();
        }
    }
}
