using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip backgroundMusicClip;
    public AudioClip flashlightLoopClip;
    public AudioClip playerDamageClip;
    public AudioClip enemyKillClip;
    public AudioClip coinPickupClip;
    public AudioClip batteryPickupClip;

    [Header("Mixer Levels")]
    [Range(0f, 1f)] public float musicVolume = 0.3f;
    [Range(0f, 1f)] public float flashlightVolume = 0.22f;
    [Range(0f, 1f)] public float sfxVolume = 0.85f;

    private AudioSource musicSource;
    private AudioSource flashlightSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureSources();
        StartBackgroundMusic();
    }

    private void EnsureSources()
    {
        musicSource = GetOrCreateSource("MusicSource", true, false, musicVolume);
        flashlightSource = GetOrCreateSource("FlashlightSource", true, false, flashlightVolume);
        sfxSource = GetOrCreateSource("SfxSource", false, false, sfxVolume);
    }

    private AudioSource GetOrCreateSource(string nameId, bool loop, bool playOnAwake, float volume)
    {
        Transform existing = transform.Find(nameId);
        AudioSource source;
        if (existing == null)
        {
            GameObject sourceObject = new GameObject(nameId, typeof(AudioSource));
            sourceObject.transform.SetParent(transform, false);
            source = sourceObject.GetComponent<AudioSource>();
        }
        else
        {
            source = existing.GetComponent<AudioSource>();
            if (source == null)
            {
                source = existing.gameObject.AddComponent<AudioSource>();
            }
        }

        source.loop = loop;
        source.playOnAwake = playOnAwake;
        source.volume = volume;
        return source;
    }

    public void StartBackgroundMusic()
    {
        if (musicSource == null)
        {
            EnsureSources();
        }

        if (backgroundMusicClip == null)
        {
            return;
        }

        if (musicSource.isPlaying && musicSource.clip == backgroundMusicClip)
        {
            return;
        }

        musicSource.clip = backgroundMusicClip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void SetFlashlightActive(bool active)
    {
        if (flashlightSource == null)
        {
            EnsureSources();
        }

        if (flashlightLoopClip == null)
        {
            return;
        }

        if (active)
        {
            if (flashlightSource.clip != flashlightLoopClip)
            {
                flashlightSource.clip = flashlightLoopClip;
            }

            flashlightSource.volume = flashlightVolume;
            if (!flashlightSource.isPlaying)
            {
                flashlightSource.Play();
            }

            return;
        }

        if (flashlightSource.isPlaying)
        {
            flashlightSource.Stop();
        }
    }

    public void PlayPlayerDamage()
    {
        PlayOneShot(playerDamageClip, sfxVolume);
    }

    public void PlayEnemyKill()
    {
        PlayOneShot(enemyKillClip, sfxVolume);
    }

    public void PlayCoinPickup()
    {
        PlayOneShot(coinPickupClip, sfxVolume);
    }

    public void PlayBatteryPickup()
    {
        PlayOneShot(batteryPickupClip, sfxVolume);
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            return;
        }

        if (sfxSource == null)
        {
            EnsureSources();
        }

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
