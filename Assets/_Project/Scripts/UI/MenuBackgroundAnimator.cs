using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Collections;
namespace GhostBeam.UI
{
    /// <summary>
    /// Fundo do menu com pulsação sombria em tons de azul profundo.
    /// </summary>
    public class MenuBackgroundAnimator : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image menuBackdropImage;
        [SerializeField] private Color[] gradientColors = new Color[]
        {
            new Color(0.02f, 0.04f, 0.09f, 1f),
            new Color(0.05f, 0.1f, 0.18f, 1f),
            new Color(0.03f, 0.06f, 0.12f, 1f)
        };
        [SerializeField] private float animationSpeed = 0.35f;
        [SerializeField] private Light2D[] dynamicLights;

        private Color _baseBackdrop = new Color(0.025f, 0.04f, 0.075f, 0.97f);

        private void Start()
        {
            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();

            if (menuBackdropImage == null)
            {
                var bgGo = transform.Find("MenuBackground");
                if (bgGo != null)
                    menuBackdropImage = bgGo.GetComponent<Image>();
            }

            if (backgroundImage != null)
                backgroundImage.color = gradientColors[0];

            if (menuBackdropImage != null)
                menuBackdropImage.color = _baseBackdrop;

            StartCoroutine(PulseMenuBackdrop());
            if (dynamicLights != null && dynamicLights.Length > 0)
                StartCoroutine(AnimateLights());
        }

        private IEnumerator PulseMenuBackdrop()
        {
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime * animationSpeed;
                float wave = Mathf.Sin(t) * 0.5f + 0.5f;

                if (backgroundImage != null)
                {
                    Color a = gradientColors[0];
                    Color b = gradientColors[Mathf.Min(1, gradientColors.Length - 1)];
                    Color c = gradientColors[gradientColors.Length - 1];
                    Color mix = Color.Lerp(a, b, wave);
                    mix = Color.Lerp(mix, c, Mathf.PingPong(t * 0.27f, 1f));
                    backgroundImage.color = mix;
                }

                if (menuBackdropImage != null)
                {
                    float bleed = 0.035f * Mathf.Sin(t * 1.7f);
                    menuBackdropImage.color = new Color(
                        _baseBackdrop.r + bleed * 0.4f,
                        _baseBackdrop.g + bleed * 0.25f,
                        _baseBackdrop.b + bleed * 0.55f,
                        _baseBackdrop.a);
                }

                yield return null;
            }
        }

        private IEnumerator AnimateLights()
        {
            float time = 0f;

            while (true)
            {
                time += Time.deltaTime * animationSpeed;

                for (int i = 0; i < dynamicLights.Length; i++)
                {
                    if (dynamicLights[i] != null)
                    {
                        float intensity = Mathf.Sin(time + i * Mathf.PI / dynamicLights.Length) * 0.5f + 0.5f;
                        dynamicLights[i].intensity = Mathf.Lerp(0.15f, 0.55f, intensity);
                    }
                }

                yield return null;
            }
        }

        public void SetBackgroundColor(Color newColor)
        {
            if (backgroundImage != null)
                backgroundImage.color = newColor;
        }
    }
}
