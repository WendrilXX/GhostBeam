using UnityEngine;

namespace GhostBeam.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private AudioSource musicSource;
        private float masterVolume = 1f;

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
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource == null || clip == null)
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

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.Save();
        }
    }
}
