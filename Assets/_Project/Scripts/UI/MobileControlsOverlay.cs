using UnityEngine;
using UnityEngine.UI;

namespace GhostBeam.UI
{
    public class MobileControlsOverlay : MonoBehaviour
    {
        private static Sprite _circleSprite;

        [SerializeField] private bool onlyOnMobile = true;
        [SerializeField] private bool showInEditor = false;
        [SerializeField] private Vector2 leftPosition = new Vector2(170f, 170f);
        [SerializeField] private Vector2 rightPosition = new Vector2(-170f, 170f);
        [SerializeField] private Vector2 joystickSize = new Vector2(300f, 300f);
        [SerializeField] private float baseAlpha = 0.2f;
        [SerializeField] private float knobAlpha = 0.35f;
        [SerializeField] private int sortingOrder = 2;

        private Canvas canvas;
        private RectTransform canvasRect;
        private RectTransform leftBase;
        private RectTransform rightBase;
        private RectTransform leftKnob;
        private RectTransform rightKnob;
        private Vector2 leftDefaultPos;
        private Vector2 rightDefaultPos;
        private int leftFingerId = -1;
        private int rightFingerId = -1;

        private void Awake()
        {
            if (!ShouldShow())
                return;

            EnsureOverlay();
            CacheDefaults();
        }

        private void Update()
        {
            if (!ShouldShow() || !Application.isMobilePlatform)
                return;

            HandleTouches();
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
            leftBase = EnsureJoystick(mobileUIObj.transform, "JoystickLeft");
            rightBase = EnsureJoystick(mobileUIObj.transform, "JoystickRight");
            leftKnob = leftBase != null ? leftBase.Find("Knob")?.GetComponent<RectTransform>() : null;
            rightKnob = rightBase != null ? rightBase.Find("Knob")?.GetComponent<RectTransform>() : null;
        }

        private void ConfigureCanvas(GameObject mobileUIObj)
        {
            canvas = mobileUIObj.GetComponent<Canvas>();
            if (canvas == null)
                canvas = mobileUIObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            canvasRect = canvas.GetComponent<RectTransform>();

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

        private RectTransform EnsureJoystick(Transform parent, string name)
        {
            Transform existing = parent.Find(name);
            GameObject joystickObj = existing != null ? existing.gameObject : new GameObject(name);
            joystickObj.transform.SetParent(parent, false);

            var rect = joystickObj.GetComponent<RectTransform>();
            if (rect == null)
                rect = joystickObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = joystickSize;

            var image = joystickObj.GetComponent<Image>();
            if (image == null)
                image = joystickObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, baseAlpha);
            image.raycastTarget = false;
            ApplyCircularSprite(image);

            EnsureKnob(joystickObj.transform, "Knob");
            return rect;
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
            ApplyCircularSprite(image);
        }

        private static void ApplyCircularSprite(Image image)
        {
            if (image == null)
                return;

            image.sprite = GetOrCreateCircleSprite();
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        }

        private static Sprite GetOrCreateCircleSprite()
        {
            if (_circleSprite != null)
                return _circleSprite;

            const int size = 128;
            const float radius = size * 0.5f - 1f;
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.hideFlags = HideFlags.HideAndDontSave;

            float cx = size * 0.5f;
            float cy = size * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01((radius - d + 0.8f) / 1.6f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }

            tex.Apply(false, true);
            _circleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            _circleSprite.hideFlags = HideFlags.HideAndDontSave;
            return _circleSprite;
        }

        private void CacheDefaults()
        {
            if (canvasRect == null)
                return;

            leftDefaultPos = GetCornerPosition(leftPosition, false);
            rightDefaultPos = GetCornerPosition(rightPosition, true);

            if (leftBase != null)
                leftBase.anchoredPosition = leftDefaultPos;
            if (rightBase != null)
                rightBase.anchoredPosition = rightDefaultPos;
        }

        private Vector2 GetCornerPosition(Vector2 offset, bool right)
        {
            float halfWidth = canvasRect.rect.width * 0.5f;
            float halfHeight = canvasRect.rect.height * 0.5f;
            float x = right ? halfWidth + offset.x : -halfWidth + offset.x;
            float y = -halfHeight + offset.y;
            return new Vector2(x, y);
        }

        private void HandleTouches()
        {
            bool leftActive = false;
            bool rightActive = false;
            float screenHalf = Screen.width * 0.5f;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.position.x < screenHalf)
                {
                    leftActive = true;
                    UpdateJoystick(ref leftFingerId, touch, leftBase, leftKnob, leftDefaultPos);
                }
                else
                {
                    rightActive = true;
                    UpdateJoystick(ref rightFingerId, touch, rightBase, rightKnob, rightDefaultPos);
                }
            }

            if (!leftActive)
                ResetJoystick(ref leftFingerId, leftBase, leftKnob, leftDefaultPos);
            if (!rightActive)
                ResetJoystick(ref rightFingerId, rightBase, rightKnob, rightDefaultPos);
        }

        private void UpdateJoystick(ref int fingerId, Touch touch, RectTransform baseRect, RectTransform knobRect, Vector2 fallbackPos)
        {
            if (baseRect == null || knobRect == null || canvasRect == null)
                return;

            if (touch.phase == TouchPhase.Began)
            {
                fingerId = touch.fingerId;
                SetBasePosition(baseRect, touch.position);
                knobRect.anchoredPosition = Vector2.zero;
                return;
            }

            if (fingerId != touch.fingerId)
                return;

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                ResetJoystick(ref fingerId, baseRect, knobRect, fallbackPos);
                return;
            }

            Vector2 localPoint = GetLocalPoint(touch.position);
            Vector2 delta = localPoint - baseRect.anchoredPosition;
            float maxOffset = joystickSize.x * 0.35f;
            knobRect.anchoredPosition = Vector2.ClampMagnitude(delta, maxOffset);
        }

        private void ResetJoystick(ref int fingerId, RectTransform baseRect, RectTransform knobRect, Vector2 fallbackPos)
        {
            fingerId = -1;
            if (baseRect != null)
                baseRect.anchoredPosition = fallbackPos;
            if (knobRect != null)
                knobRect.anchoredPosition = Vector2.zero;
        }

        private void SetBasePosition(RectTransform baseRect, Vector2 screenPosition)
        {
            Vector2 localPoint = GetLocalPoint(screenPosition);
            baseRect.anchoredPosition = localPoint;
        }

        private Vector2 GetLocalPoint(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out Vector2 localPoint);
            return localPoint;
        }
    }
}
