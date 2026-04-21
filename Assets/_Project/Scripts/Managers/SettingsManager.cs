using UnityEngine;
using System;

namespace GhostBeam.Managers
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        private float masterVolume = 1f;
        private bool vibrationEnabled = true;
        private bool performanceOverlayEnabled = false;

        public float MasterVolume 
        { 
            get => masterVolume;
            set 
            { 
                masterVolume = Mathf.Clamp01(value);
                SaveSettings();
            }
        }

        public bool VibrationEnabled 
        { 
            get => vibrationEnabled;
            set 
            { 
                vibrationEnabled = value;
                SaveSettings();
            }
        }

        public bool PerformanceOverlayEnabled 
        { 
            get => performanceOverlayEnabled;
            set 
            { 
                performanceOverlayEnabled = value;
                SaveSettings();
            }
        }

        public static event Action onSettingsChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }

        private void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            vibrationEnabled = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
            performanceOverlayEnabled = PlayerPrefs.GetInt("PerformanceOverlay", 0) == 1;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetInt("VibrationEnabled", vibrationEnabled ? 1 : 0);
            PlayerPrefs.SetInt("PerformanceOverlay", performanceOverlayEnabled ? 1 : 0);
            PlayerPrefs.Save();
            
            onSettingsChanged?.Invoke();
        }

        public void Vibrate()
        {
            if (vibrationEnabled && Application.isEditor == false)
            {
                Handheld.Vibrate();
            }
        }
    }
}
