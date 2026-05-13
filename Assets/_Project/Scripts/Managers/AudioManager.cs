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
    private const string DefaultMenuMusicPath = "Assets/_Project/Audio/Music/korku.wav";
    private const string DefaultGameplayMusicPath = "Assets/_Project/Audio/Music/565693__zhr__horror-background-2.mp3";
#endif

        [Header("Music")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;

        [Header("SFX")]
        [SerializeField] private AudioClip menuClickSfx;

        private AudioSource musicSource;
        private float masterVolume = 1f;
        private float musicVolume = 1f;
        private float sfxVolume = 1f;
        private string lastMusicSceneName;

        public float MasterVolume 
        { 
            get => masterVolume;
            set 
            { 
                masterVolume = Mathf.Clamp01(value);
                if (musicSource != null)
                    musicSource.volume = masterVolume * musicVolume;
            }
        }

        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = Mathf.Clamp01(value);
                if (musicSource != null)
                    musicSource.volume = masterVolume * musicVolume;
            }
        }

        public float SfxVolume
        {
            get => sfxVolume;
            set => sfxVolume = Mathf.Clamp01(value);
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
            EnsureAudioListener();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (menuClickSfx == null)
            {
                menuClickSfx = AssetDatabase.LoadAssetAtPath<AudioClip>(DefaultMenuClickSfxPath);
            }

            if (menuMusic == null)
            {
                menuMusic = AssetDatabase.LoadAssetAtPath<AudioClip>(DefaultMenuMusicPath);
            }

            if (gameplayMusic == null)
            {
                gameplayMusic = AssetDatabase.LoadAssetAtPath<AudioClip>(DefaultGameplayMusicPath);
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
            musicSource.volume = masterVolume * musicVolume;
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

            AudioSource.PlayClipAtPoint(clip, Vector3.zero, volume * masterVolume * sfxVolume);
        }

        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null)
                return;

            AudioSource.PlayClipAtPoint(clip, position, volume * masterVolume * sfxVolume);
        }

        private void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
            if (musicSource != null)
                musicSource.volume = masterVolume * musicVolume;
        }

        public void PlayMenuMusic()
        {
            if (menuMusic != null)
            {
                PlayMusic(menuMusic, true);
                return;
            }

            Debug.LogWarning("[AudioManager] Menu music clip is not assigned.");
        }

        public void PlayGameplayMusic()
        {
            if (gameplayMusic != null)
            {
                // Ensure volume is not zero (common issue on mobile)
                if (musicSource.volume <= 0)
                {
                    musicVolume = 1f;
                    masterVolume = 1f;
                }
                
                PlayMusic(gameplayMusic, true);
                Debug.Log($"[AudioManager] Playing gameplay music. Volume: {musicSource.volume}");
                return;
            }

            Debug.LogError("[AudioManager] CRITICAL: Gameplay music clip is NOT assigned! Please assign it in the Inspector.");
        }

        public void PlayMenuClick()
        {
            if (menuClickSfx != null)
                PlaySFX(menuClickSfx, 1f);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnsureAudioListener();

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

        private void EnsureAudioListener()
        {
            var listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            if (listeners.Length == 0)
            {
                gameObject.AddComponent<AudioListener>();
                return;
            }

            if (listeners.Length <= 1)
                return;

            AudioListener preferred = null;
            foreach (var listener in listeners)
            {
                if (listener != null && listener.GetComponent<Camera>() != null)
                {
                    preferred = listener;
                    break;
                }
            }

            if (preferred == null)
                preferred = listeners[0];

            foreach (var listener in listeners)
            {
                if (listener != null && listener != preferred)
                    listener.enabled = false;
            }
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
            PlayerPrefs.Save();
        }
    }
}
