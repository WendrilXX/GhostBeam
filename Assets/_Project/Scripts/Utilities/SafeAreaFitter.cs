using UnityEngine;

namespace GhostBeam.Utilities
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Rect previousSafeArea;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            previousSafeArea = Screen.safeArea;
            ApplySafeArea();
        }

        private void Update()
        {
            // Atualizar se safe area mudar (device orientation)
            if (Screen.safeArea != previousSafeArea)
            {
                previousSafeArea = Screen.safeArea;
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;

            // Converter screen space para canvas space
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}
