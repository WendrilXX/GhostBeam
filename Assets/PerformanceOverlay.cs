using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class PerformanceOverlay : MonoBehaviour
{
    public TMP_Text overlayText;
    public float refreshInterval = 0.5f;

    private float timer;
    private int frames;

    private void OnEnable()
    {
        SettingsManager.onSettingsChanged += ApplyVisibility;
        ApplyVisibility();
    }

    private void OnDisable()
    {
        SettingsManager.onSettingsChanged -= ApplyVisibility;
    }

    private void Update()
    {
        if (overlayText == null || !overlayText.gameObject.activeSelf)
        {
            return;
        }

        frames++;
        timer += Time.unscaledDeltaTime;
        if (timer < refreshInterval)
        {
            return;
        }

        float fps = frames / Mathf.Max(0.0001f, timer);
        float frameMs = 1000f / Mathf.Max(1f, fps);
        long totalMemoryBytes = Profiler.GetTotalAllocatedMemoryLong();
        float totalMemoryMb = totalMemoryBytes / (1024f * 1024f);

        int enemies = EnemyController.ActiveCount;
        int pickups = BatteryPickup.ActiveCount;
        SpawnManager spawnManager = FindAnyObjectByType<SpawnManager>();
        float pressure = spawnManager != null ? spawnManager.CurrentPerformancePressure * 100f : 0f;

        // Cores dinâmicas baseado em FPS
        string fpsColor = fps >= 50f ? "#00FF00" : (fps >= 30f ? "#FFD700" : "#FF3333");
        
        overlayText.text = $"<color={fpsColor}><b>FPS {Mathf.RoundToInt(fps)}</b></color> | {frameMs:0.0}ms\n"
            + $"MEM {totalMemoryMb:0.0}MB | LOAD {pressure:0}%\n"
            + $"* Enemies {enemies} | * Pickups {pickups}";
        
        overlayText.fontSize = 16;
        overlayText.alignment = TextAlignmentOptions.BottomRight;

        frames = 0;
        timer = 0f;
    }

    private void ApplyVisibility()
    {
        if (overlayText == null)
        {
            return;
        }

        bool visible = SettingsManager.Instance != null && SettingsManager.Instance.ShowPerfOverlay;
        overlayText.gameObject.SetActive(visible);
    }
}
