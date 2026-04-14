using UnityEngine;
using TMPro;

public class PickupFloatingText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private RectTransform rectTransform;
    private float lifeTimer;
    private float fadeDuration = 0.8f;
    private float floatDuration = 0.8f;
    private Vector3 startPosition;
    private Color startColor;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            tmp = gameObject.AddComponent<TextMeshProUGUI>();
        }

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }
    }

    private void OnEnable()
    {
        lifeTimer = 0f;
        startPosition = transform.position;
        if (tmp != null)
        {
            startColor = tmp.color;
        }
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer < floatDuration)
        {
            float progress = lifeTimer / floatDuration;
            Vector3 newPos = startPosition + Vector3.up * progress * 1.5f;
            transform.position = Vector3.Lerp(startPosition, newPos, progress);
        }

        if (tmp != null)
        {
            float fadeProgress = Mathf.Clamp01(lifeTimer / fadeDuration);
            Color fadeColor = startColor;
            fadeColor.a = Mathf.Lerp(startColor.a, 0f, fadeProgress);
            tmp.color = fadeColor;
        }

        if (lifeTimer >= floatDuration)
        {
            Destroy(gameObject);
        }
    }

    public static void ShowCoinText(Vector3 worldPosition, int amount)
    {
        ShowFloatingText(worldPosition, "+" + amount, new Color(1f, 0.84f, 0f, 1f)); // Ouro
    }

    public static void ShowBatteryText(Vector3 worldPosition, float amount)
    {
        string text = "+" + Mathf.RoundToInt(amount) + "%";
        ShowFloatingText(worldPosition, text, new Color(0f, 1f, 0f, 1f)); // Verde
    }

    private static void ShowFloatingText(Vector3 worldPosition, string text, Color color)
    {
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.position = worldPosition;

        Canvas worldCanvas = FindAnyObjectByType<Canvas>();
        if (worldCanvas != null)
        {
            textObj.transform.SetParent(worldCanvas.transform, false);
        }

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        tmp.fontSize = 48;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 50);

        PickupFloatingText floatingText = textObj.AddComponent<PickupFloatingText>();
        floatingText.enabled = true;
    }
}
