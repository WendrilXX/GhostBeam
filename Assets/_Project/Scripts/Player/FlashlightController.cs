using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GhostBeam.Player
{
    public class FlashlightController : MonoBehaviour
    {
        private const string BeamUpgradeTierKey = "Upgrade_Beam_Tier";
        private const string PowerUpgradeTierKey = "Upgrade_Power_Tier";
        private const int MaxPowerTier = 3;

        [SerializeField] private Light2D flashlight;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float beamAnglePerTier = 8f;
        [SerializeField] private float beamRangePerTier = 2.5f;
        [SerializeField] private float intensityPerTier = 0.35f;

        private float currentRotation = 0f;
        private Gameplay.BatterySystem batterySystem;
        private bool wantsFlashlightActive = true;
        private float baseOuterAngle;
        private float baseOuterRadius;
        private float baseIntensity;
        private int aimFingerId = -1;
        private float screenHalfWidth;
        private Vector2 aimCenter;

        private void Awake()
        {
            if (flashlight == null)
                flashlight = GetComponentInChildren<Light2D>();

            batterySystem = GetComponent<Gameplay.BatterySystem>();
            if (batterySystem == null)
                batterySystem = FindAnyObjectByType<Gameplay.BatterySystem>();

            if (flashlight != null)
            {
                baseOuterAngle = flashlight.pointLightOuterAngle;
                baseOuterRadius = flashlight.pointLightOuterRadius;
                baseIntensity = flashlight.intensity;
                ApplySavedUpgrades();
            }

            screenHalfWidth = Screen.width * 0.5f;
            aimCenter = new Vector2(Screen.width * 0.8f, Screen.height * 0.2f);
        }

        private void OnEnable()
        {
            Gameplay.BatterySystem.onBatteryDepleted += OnBatteryDepleted;
        }

        private void OnDisable()
        {
            Gameplay.BatterySystem.onBatteryDepleted -= OnBatteryDepleted;
            if (batterySystem != null)
                batterySystem.SetLighting(false);
        }

        private void Update()
        {
            HandleAim();
            SyncBatteryUsage();
        }

        private void HandleAim()
        {
            Vector2 aimInput = Vector2.zero;
            bool usedTouch = false;

            // Touch mobile
            if (Input.touchCount > 0)
            {
                Touch activeTouch = default;
                bool found = false;

                if (aimFingerId != -1)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.fingerId == aimFingerId)
                        {
                            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                            {
                                aimFingerId = -1;
                            }
                            else
                            {
                                activeTouch = touch;
                                found = true;
                            }
                            break;
                        }
                    }
                }

                if (!found)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.position.x > screenHalfWidth && touch.phase == TouchPhase.Began)
                        {
                            aimFingerId = touch.fingerId;
                            activeTouch = touch;
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.position.x > screenHalfWidth && touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                        {
                            aimFingerId = touch.fingerId;
                            activeTouch = touch;
                            found = true;
                            break;
                        }
                    }
                }

                if (found && aimFingerId != -1)
                {
                    aimInput = (activeTouch.position - aimCenter).normalized;
                    usedTouch = true;
                }
            }

            if (!usedTouch)
            {
                // Mouse (debug)
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10f;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                aimInput = (worldPos - transform.position).normalized;
            }

            // Calcular rotação em graus
            if (aimInput.magnitude > 0.1f)
            {
                currentRotation = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg - 90f;
            }

            // Aplicar rotação suave
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.AngleAxis(currentRotation, Vector3.forward),
                Time.deltaTime * rotationSpeed
            );
        }

        public bool IsActive => flashlight != null && flashlight.enabled;

        public void SetActive(bool active)
        {
            wantsFlashlightActive = active;

            if (flashlight != null)
            {
                bool hasBattery = batterySystem == null || batterySystem.CurrentBattery > 0.01f;
                flashlight.enabled = active && hasBattery;
            }

            if (batterySystem != null)
                batterySystem.SetLighting(IsActive);
        }

        private void SyncBatteryUsage()
        {
            if (flashlight == null || batterySystem == null)
                return;

            bool hasBattery = batterySystem.CurrentBattery > 0.01f;
            flashlight.enabled = wantsFlashlightActive && hasBattery;
            batterySystem.SetLighting(flashlight.enabled);
        }

        private void OnBatteryDepleted()
        {
            if (flashlight != null)
                flashlight.enabled = false;
        }

        private void ApplySavedUpgrades()
        {
            if (flashlight == null)
                return;

            int beamTier = Mathf.Clamp(PlayerPrefs.GetInt(BeamUpgradeTierKey, 0), 0, 3);
            int powerTier = Mathf.Clamp(PlayerPrefs.GetInt(PowerUpgradeTierKey, 0), 0, 3);

            flashlight.pointLightOuterAngle = baseOuterAngle + (beamAnglePerTier * beamTier);
            flashlight.pointLightOuterRadius = baseOuterRadius + (beamRangePerTier * beamTier);
            float powerMultiplier = Mathf.Lerp(1f, 2f, powerTier / (float)MaxPowerTier);
            flashlight.intensity = baseIntensity * powerMultiplier;
        }
    }
}
