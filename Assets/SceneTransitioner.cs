using UnityEngine;
using System.Collections;

/// <summary>
/// Script para gerenciar transições suaves entre telas com fade in/out
/// </summary>
public class SceneTransitioner : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float fadeDuration = 0.5f;
    public static SceneTransitioner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Fade in da tela (entrada)
    /// </summary>
    public void FadeIn(float duration = 0.5f)
    {
        fadeDuration = duration;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 1f, duration));
    }

    /// <summary>
    /// Fade out da tela (saída)
    /// </summary>
    public void FadeOut(float duration = 0.5f)
    {
        fadeDuration = duration;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0f, duration));
    }

    private IEnumerator FadeRoutine(float fromAlpha, float toAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = toAlpha;
    }
}
