using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private const string MasterVolumeKey = "Settings_MasterVolume";
    private const string VibrationKey = "Settings_Vibration";
    private const string ShowTimerKey = "Settings_ShowTimer";
    private const string ShowPerfOverlayKey = "Settings_ShowPerfOverlay";

    public static SettingsManager Instance { get; private set; }
    public static System.Action onSettingsChanged;

    public float MasterVolume { get; private set; } = 1f;
    public bool VibrationEnabled { get; private set; } = true;
    public bool ShowHudTimer { get; private set; } = true;
    public bool ShowPerfOverlay { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Load();
        ApplyAudio();
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        ApplyAudio();
        Save();
        onSettingsChanged?.Invoke();
    }

    public void ToggleVibration()
    {
        VibrationEnabled = !VibrationEnabled;
        Save();
        onSettingsChanged?.Invoke();
    }

    public void ToggleHudTimer()
    {
        ShowHudTimer = !ShowHudTimer;
        Save();
        onSettingsChanged?.Invoke();
    }

    public void TogglePerfOverlay()
    {
        ShowPerfOverlay = !ShowPerfOverlay;
        Save();
        onSettingsChanged?.Invoke();
    }

    private void ApplyAudio()
    {
        AudioListener.volume = MasterVolume;
    }

    private void Load()
    {
        MasterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        VibrationEnabled = PlayerPrefs.GetInt(VibrationKey, 1) == 1;
        ShowHudTimer = PlayerPrefs.GetInt(ShowTimerKey, 1) == 1;
        ShowPerfOverlay = PlayerPrefs.GetInt(ShowPerfOverlayKey, 0) == 1;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
        PlayerPrefs.SetInt(VibrationKey, VibrationEnabled ? 1 : 0);
        PlayerPrefs.SetInt(ShowTimerKey, ShowHudTimer ? 1 : 0);
        PlayerPrefs.SetInt(ShowPerfOverlayKey, ShowPerfOverlay ? 1 : 0);
        PlayerPrefs.Save();
    }
}
