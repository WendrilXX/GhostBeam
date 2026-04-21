using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GhostBeam
{
    /// <summary>
    /// Controls the flashlight light rendering and direction
    /// </summary>
    public class LanternLight : MonoBehaviour
    {
        [SerializeField] private Light2D light2D;
        [SerializeField] private float maxIntensity = 2f;
        [SerializeField] private float minIntensity = 0.5f;
        [SerializeField] private float pulsationSpeed = 2f;

        private void Start()
        {
            // Get or create Light2D component
            if (light2D == null)
            {
                light2D = GetComponent<Light2D>();
            }

            if (light2D == null)
            {
                light2D = gameObject.AddComponent<Light2D>();
                Debug.Log("[LanternLight] Created Light2D component");
            }

            // Configure light
            light2D.lightType = Light2D.LightType.Parametric;  // Use Parametric instead of Spot
            light2D.intensity = maxIntensity;
            light2D.falloffIntensity = 0.5f;
            light2D.color = new Color(1f, 0.9f, 0.7f, 1f);  // Warm yellow-ish
            light2D.shadowsEnabled = false;
            
            Debug.Log("[LanternLight] Light configured");
        }

        private void Update()
        {
            if (light2D == null) return;

            // Add pulsation effect
            float pulsation = Mathf.Sin(Time.time * pulsationSpeed) * 0.3f;
            light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, 0.7f + pulsation);

            // The rotation is handled by FlashlightController's aim system
            // This script just handles the light visual properties
        }

        /// <summary>
        /// Set the light's outer radius (range)
        /// </summary>
        public void SetLightRange(float range)
        {
            if (light2D != null)
            {
                light2D.pointLightOuterRadius = range;
            }
        }

        /// <summary>
        /// Set the light's inner radius (focus)
        /// </summary>
        public void SetLightFocus(float focus)
        {
            if (light2D != null)
            {
                light2D.pointLightInnerRadius = focus;
            }
        }

        /// <summary>
        /// Pulse the light (for visual feedback)
        /// </summary>
        public void Pulse(float intensity = 1f)
        {
            if (light2D != null)
            {
                light2D.intensity = Mathf.Max(light2D.intensity, maxIntensity * intensity);
            }
        }

        /// <summary>
        /// Set light color
        /// </summary>
        public void SetLightColor(Color color)
        {
            if (light2D != null)
            {
                light2D.color = color;
            }
        }
    }
}
