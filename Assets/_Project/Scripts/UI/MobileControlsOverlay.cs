using UnityEngine;
using UnityEngine.UI;

namespace GhostBeam.UI
{
    public class MobileControlsOverlay : MonoBehaviour
    {
        [SerializeField] private bool onlyOnMobile = true;
        [SerializeField] private bool showInEditor = false;
        [SerializeField] private Vector2 leftPosition = new Vector2(170f, 170f);
        [SerializeField] private Vector2 rightPosition = new Vector2(-170f, 170f);
        [SerializeField] private Vector2 joystickSize = new Vector2(300f, 300f);
        [SerializeField] private float baseAlpha = 0.2f;
        [SerializeField] private float knobAlpha = 0.35f;
        [SerializeField] private int sortingOrder = 2;

        private void Awake()
        {
            if (!ShouldShow())
                return;

            EnsureOverlay();
        }

        private bool ShouldShow()
        {
            if (onlyOnMobile && !Application.isMobilePlatform)
                return showInEditor && Application.isEditor;

            return true;
        }

        private void EnsureOverlay()
        {
            GameObject mobileUIObj = GameObject.Find("MobileUI");
            if (mobileUIObj == null)
            {
                mobileUIObj = new GameObject("MobileUI");
            }

            ConfigureCanvas(mobileUIObj);
            EnsureJoystick(mobileUIObj.transform, "JoystickLeft", new Vector2(0f, 0f), leftPosition);
            EnsureJoystick(mobileUIObj.transform, "JoystickRight", new Vector2(1f, 0f), rightPosition);
        }

        private void ConfigureCanvas(GameObject mobileUIObj)
        {
            var canvas = mobileUIObj.GetComponent<Canvas>();
            if (canvas == null)
                canvas = mobileUIObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            var scaler = mobileUIObj.GetComponent<CanvasScaler>();
            if (scaler == null)
                scaler = mobileUIObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            var group = mobileUIObj.GetComponent<CanvasGroup>();
            if (group == null)
                group = mobileUIObj.AddComponent<CanvasGroup>();
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        private void EnsureJoystick(Transform parent, string name, Vector2 anchor, Vector2 anchoredPosition)
        {
            Transform existing = parent.Find(name);
            GameObject joystickObj = existing != null ? existing.gameObject : new GameObject(name);
            joystickObj.transform.SetParent(parent, false);

            var rect = joystickObj.GetComponent<RectTransform>();
            if (rect == null)
                rect = joystickObj.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = joystickSize;

            var image = joystickObj.GetComponent<Image>();
            if (image == null)
                image = joystickObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, baseAlpha);
            image.raycastTarget = false;

            EnsureKnob(joystickObj.transform, "Knob");
        }

        private void EnsureKnob(Transform parent, string name)
        {
            Transform existing = parent.Find(name);
            GameObject knobObj = existing != null ? existing.gameObject : new GameObject(name);
            knobObj.transform.SetParent(parent, false);

            var rect = knobObj.GetComponent<RectTransform>();
            if (rect == null)
                rect = knobObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = joystickSize * 0.4f;

            var image = knobObj.GetComponent<Image>();
            if (image == null)
                image = knobObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, knobAlpha);
            image.raycastTarget = false;
        }
    }
}
