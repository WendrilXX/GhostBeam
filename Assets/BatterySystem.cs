using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BatterySystem : MonoBehaviour
{
    public static System.Action<float, float, bool> onBatteryChanged;

    public Transform flashlight;
    public Light2D flashlightLight;
    public float maxBattery = 150f;
    public float minDrainPerSecond = 1f;
    public float maxDrainPerSecond = 5f;
    public float blackoutDuration = 3f;
    public float resumeBatteryAmount = 25f;
    public float checkInterval = 0.1f;
    public float lightDistance = 15f;
    public float lightAngle = 35f;
    public bool gameOverOnBatteryDepleted = true;

    public float CurrentBattery { get; private set; }
    public bool IsDisabled { get; private set; }

    private float blackoutTimer;
    private float checkTimer;
    private bool isIlluminatingEnemy;
    private bool flashlightAudioActive;
    private float currentDrainRate; // Taxa de drenagem variável

    private void Awake()
    {
        CurrentBattery = GetEffectiveMaxBattery();
        currentDrainRate = Random.Range(minDrainPerSecond, maxDrainPerSecond);
        if (flashlight == null)
        {
            flashlight = transform;
        }

        if (flashlightLight == null)
        {
            flashlightLight = GetComponent<Light2D>();
        }

        NotifyBatteryChanged();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            SetFlashlightAudio(false);
            return;
        }

        if (IsDisabled)
        {
            SetFlashlightAudio(false);
            UpdateBlackout();
            return;
        }

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            isIlluminatingEnemy = IsAnyEnemyIlluminated();
            currentDrainRate = Random.Range(minDrainPerSecond, maxDrainPerSecond);
            SetFlashlightAudio(isIlluminatingEnemy);
        }

        // Bateria SEMPRE desce, independente de estar iluminando ou não
        float delta = -currentDrainRate;
        float previousBattery = CurrentBattery;
        float effectiveMaxBattery = GetEffectiveMaxBattery();
        CurrentBattery = Mathf.Clamp(CurrentBattery + delta * Time.deltaTime, 0f, effectiveMaxBattery);

        if (!Mathf.Approximately(previousBattery, CurrentBattery))
        {
            NotifyBatteryChanged();
        }

        if (CurrentBattery <= 0f)
        {
            if (gameOverOnBatteryDepleted)
            {
                CurrentBattery = 0f;
                SetFlashlightEnabled(false);
                SetFlashlightAudio(false);
                NotifyBatteryChanged();
                if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
                {
                    GameManager.Instance.TriggerGameOver();
                }

                return;
            }

            TriggerBlackout();
        }
    }

    public void RestoreBattery(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentBattery = Mathf.Clamp(CurrentBattery + amount, 0f, GetEffectiveMaxBattery());
        NotifyBatteryChanged();

        if (IsDisabled && CurrentBattery > 0f)
        {
            Debug.Log("Flashlight is back on!");
            IsDisabled = false;
            blackoutTimer = 0f;
            SetFlashlightEnabled(true);
            NotifyBatteryChanged();
        }
    }

    private void UpdateBlackout()
    {
        blackoutTimer -= Time.deltaTime;
        if (blackoutTimer > 0f)
        {
            return;
        }

        IsDisabled = false;
        CurrentBattery = Mathf.Max(CurrentBattery, Mathf.Min(resumeBatteryAmount, GetEffectiveMaxBattery()));
        SetFlashlightEnabled(true);
        NotifyBatteryChanged();
    }

    private void TriggerBlackout()
    {
        IsDisabled = true;
        blackoutTimer = blackoutDuration;
        SetFlashlightEnabled(false);
        SetFlashlightAudio(false);
        NotifyBatteryChanged();
    }

    private void SetFlashlightAudio(bool active)
    {
        if (flashlightAudioActive == active)
        {
            return;
        }

        flashlightAudioActive = active;
        AudioManager.Instance?.SetFlashlightActive(active);
    }

    private bool IsAnyEnemyIlluminated()
    {
        if (flashlight == null)
        {
            return false;
        }

        System.Collections.Generic.IReadOnlyList<EnemyController> enemies = EnemyController.ActiveEnemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                continue;
            }

            Vector2 toEnemy = (Vector2)(enemies[i].transform.position - flashlight.position);
            float distance = toEnemy.magnitude;
            if (distance > lightDistance)
            {
                continue;
            }

            float angle = Vector2.Angle(flashlight.up, toEnemy);
            if (angle <= lightAngle)
            {
                return true;
            }
        }

        return false;
    }

    private void SetFlashlightEnabled(bool value)
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = value;
        }
    }

    private void NotifyBatteryChanged()
    {
        onBatteryChanged?.Invoke(CurrentBattery, GetEffectiveMaxBattery(), IsDisabled);
    }

    private float GetEffectiveMaxBattery()
    {
        float multiplier = 1f;
        if (UpgradeManager.Instance != null)
        {
            multiplier = UpgradeManager.Instance.GetBatteryCapacityMultiplier();
        }

        return Mathf.Max(10f, maxBattery * multiplier);
    }
}
