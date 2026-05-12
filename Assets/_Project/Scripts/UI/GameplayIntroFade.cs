using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GhostBeam.Managers;

namespace GhostBeam.UI
{
    /// <summary>
    /// Revelação do cenário (fade do preto) + contagem regressiva antes de liberar o gameplay.
    /// </summary>
    public class GameplayIntroFade : MonoBehaviour
    {
        [SerializeField] [Tooltip("Duração do preto esmaecendo para revelar o mapa.")]
        private float fadeDuration = 1.15f;

        [SerializeField] [Tooltip("Pausa curta após o fade, antes do primeiro número.")]
        private float pauseAfterFade = 0.22f;

        [SerializeField] private int countdownFrom = 3;

        [SerializeField] [Tooltip("Duração de cada número (3, 2, 1).")]
        private float secondsPerCount = 0.78f;

        [SerializeField] [Tooltip("Exibe \"GO!\" ao final da contagem.")]
        private bool showGoFlash = true;

        [SerializeField] private float goFlashDuration = 0.38f;

        [SerializeField] private float countFontSize = 132f;

        private void Awake()
        {
            GameplayIntroState.BeginIntro();
        }

        private void Start()
        {
            StartCoroutine(RunIntroSequence());
        }

        private IEnumerator RunIntroSequence()
        {
            var canvasRt = GetComponent<RectTransform>();
            if (canvasRt == null)
            {
                GameplayIntroState.EndIntro();
                yield break;
            }

            var root = new GameObject("IntroSequenceRoot", typeof(RectTransform));
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.SetParent(canvasRt, false);
            rootRt.SetAsLastSibling();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.offsetMin = Vector2.zero;
            rootRt.offsetMax = Vector2.zero;

            var backdrop = new GameObject("IntroBackdrop", typeof(RectTransform), typeof(Image));
            var backdropRt = backdrop.GetComponent<RectTransform>();
            backdropRt.SetParent(rootRt, false);
            backdropRt.anchorMin = Vector2.zero;
            backdropRt.anchorMax = Vector2.one;
            backdropRt.offsetMin = Vector2.zero;
            backdropRt.offsetMax = Vector2.zero;
            var backdropImg = backdrop.GetComponent<Image>();
            backdropImg.color = Color.black;
            backdropImg.raycastTarget = true;

            var countGo = new GameObject("IntroCountdown", typeof(RectTransform), typeof(TextMeshProUGUI));
            var countRt = countGo.GetComponent<RectTransform>();
            countRt.SetParent(rootRt, false);
            countRt.anchorMin = new Vector2(0.5f, 0.5f);
            countRt.anchorMax = new Vector2(0.5f, 0.5f);
            countRt.pivot = new Vector2(0.5f, 0.5f);
            countRt.anchoredPosition = Vector2.zero;
            countRt.sizeDelta = new Vector2(480f, 280f);
            var countTmp = countGo.GetComponent<TextMeshProUGUI>();
            countTmp.alignment = TextAlignmentOptions.Center;
            countTmp.fontSize = countFontSize;
            countTmp.color = new Color(0.95f, 0.96f, 1f, 0f);
            countTmp.text = "";
            countTmp.raycastTarget = false;
            var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font != null)
                countTmp.font = font;

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / fadeDuration));
                var c = backdropImg.color;
                c.a = 1f - u;
                backdropImg.color = c;
                yield return null;
            }

            backdropImg.color = new Color(0f, 0f, 0f, 0f);

            if (pauseAfterFade > 0f)
            {
                float w = 0f;
                while (w < pauseAfterFade)
                {
                    w += Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            int from = Mathf.Max(1, countdownFrom);
            for (int n = from; n >= 1; n--)
            {
                countTmp.text = n.ToString();
                countTmp.color = new Color(countTmp.color.r, countTmp.color.g, countTmp.color.b, 1f);
                float elapsed = 0f;
                while (elapsed < secondsPerCount)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float u = Mathf.Clamp01(elapsed / secondsPerCount);
                    float pop = Mathf.SmoothStep(0.35f, 1f, u);
                    countRt.localScale = Vector3.one * pop;
                    float alpha = Mathf.SmoothStep(0f, 1f, Mathf.Min(1f, u * 3f)) *
                                    Mathf.SmoothStep(1f, 0.25f, Mathf.Clamp01((u - 0.55f) / 0.45f));
                    var col = countTmp.color;
                    col.a = alpha;
                    countTmp.color = col;
                    yield return null;
                }

                countRt.localScale = Vector3.one;
            }

            countTmp.text = "";

            if (showGoFlash && goFlashDuration > 0.05f)
            {
                countTmp.text = "GO!";
                countTmp.color = new Color(0.85f, 0.92f, 1f, 1f);
                countRt.localScale = Vector3.one * 0.4f;
                float g = 0f;
                while (g < goFlashDuration)
                {
                    g += Time.unscaledDeltaTime;
                    float u = Mathf.Clamp01(g / goFlashDuration);
                    countRt.localScale = Vector3.one * Mathf.SmoothStep(0.4f, 1.05f, u);
                    var col = countTmp.color;
                    col.a = 1f - u * u;
                    countTmp.color = col;
                    yield return null;
                }
            }

            Destroy(root);
            GameplayIntroState.EndIntro();
        }
    }
}
