using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class FlashlightController : MonoBehaviour
{
    public bool useTouchAimOnMobile = true;
    [Range(0.3f, 0.7f)] public float aimZoneSplit = 0.5f;
    public float aimJoystickRadiusPixels = 130f;
    public bool showTouchAimOverlay = true;

    private int aimFingerId = -1;
    private Vector2 aimStartScreen;
    private Vector2 aimInput;
    private Vector3? lastTouchAimWorld;

    private Texture2D circleTexture;
    private Texture2D whiteTexture;
    private GUIStyle zoneLabelStyle;

    private struct TouchData
    {
        public int fingerId;
        public Vector2 position;
        public TouchPhase phase;
    }

    private void Update()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        Vector3 aimPoint;

        if (Application.isMobilePlatform && useTouchAimOnMobile)
        {
            if (TryGetRightTouchAimWorld(cam, out Vector3 touchAimWorld))
            {
                aimPoint = touchAimWorld;
                lastTouchAimWorld = aimPoint;
            }
            else if (lastTouchAimWorld.HasValue)
            {
                aimPoint = lastTouchAimWorld.Value;
            }
            else
            {
                return;
            }
        }
        else
        {
            Vector2 screenAim = ReadPointerPosition();
            aimPoint = cam.ScreenToWorldPoint(screenAim);
        }

        aimPoint.z = 0f;

        Vector2 direction = aimPoint - transform.position;
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private bool TryGetRightTouchAimWorld(Camera cam, out Vector3 aimWorld)
    {
        aimWorld = default;
        float aimZoneMinX = Screen.width * aimZoneSplit;

        if (aimFingerId != -1)
        {
            int touchCount = GetTouchCount();
            for (int i = 0; i < touchCount; i++)
            {
                TouchData activeTouch = GetTouchAt(i);
                if (activeTouch.fingerId != aimFingerId)
                {
                    continue;
                }

                if (activeTouch.phase == TouchPhase.Ended || activeTouch.phase == TouchPhase.Canceled)
                {
                    aimFingerId = -1;
                    aimInput = Vector2.zero;
                    return false;
                }

                Vector2 delta = activeTouch.position - aimStartScreen;
                delta = Vector2.ClampMagnitude(delta, aimJoystickRadiusPixels);
                aimInput = delta / Mathf.Max(1f, aimJoystickRadiusPixels);

                if (aimInput.sqrMagnitude < 0.0004f)
                {
                    return false;
                }

                aimWorld = transform.position + new Vector3(aimInput.x, aimInput.y, 0f) * 6f;
                return true;
            }

            aimFingerId = -1;
            aimInput = Vector2.zero;
        }

        int beginTouchCount = GetTouchCount();
        for (int i = 0; i < beginTouchCount; i++)
        {
            TouchData touch = GetTouchAt(i);
            if (touch.phase != TouchPhase.Began)
            {
                continue;
            }

            if (touch.position.x < aimZoneMinX)
            {
                continue;
            }

            aimFingerId = touch.fingerId;
            aimStartScreen = touch.position;
            aimInput = Vector2.zero;
            return false;
        }

        aimInput = Vector2.zero;
        return false;
    }

    private Vector2 ReadPointerPosition()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Pointer pointer = Pointer.current;
        if (pointer != null)
        {
            return pointer.position.ReadValue();
        }

        return Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    private int GetTouchCount()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Touchscreen screen = Touchscreen.current;
        if (screen == null)
        {
            return 0;
        }

        int count = 0;
        ReadOnlyArray<TouchControl> touches = screen.touches;
        for (int i = 0; i < touches.Count; i++)
        {
            TouchControl t = touches[i];
            if (t.press.isPressed || t.phase.ReadValue() != UnityEngine.InputSystem.TouchPhase.None)
            {
                count++;
            }
        }

        return count;
#else
        return Input.touchCount;
#endif
    }

    private TouchData GetTouchAt(int index)
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Touchscreen screen = Touchscreen.current;
        int current = 0;
        if (screen != null)
        {
            ReadOnlyArray<TouchControl> touches = screen.touches;
            for (int i = 0; i < touches.Count; i++)
            {
                TouchControl t = touches[i];
                if (!(t.press.isPressed || t.phase.ReadValue() != UnityEngine.InputSystem.TouchPhase.None))
                {
                    continue;
                }

                if (current == index)
                {
                    return new TouchData
                    {
                        fingerId = (int)t.touchId.ReadValue(),
                        position = t.position.ReadValue(),
                        phase = ConvertTouchPhase(t.phase.ReadValue())
                    };
                }

                current++;
            }
        }

        return default;
#else
        Touch touch = Input.GetTouch(index);
        return new TouchData
        {
            fingerId = touch.fingerId,
            position = touch.position,
            phase = touch.phase
        };
#endif
    }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    private static TouchPhase ConvertTouchPhase(UnityEngine.InputSystem.TouchPhase phase)
    {
        switch (phase)
        {
            case UnityEngine.InputSystem.TouchPhase.Began:
                return TouchPhase.Began;
            case UnityEngine.InputSystem.TouchPhase.Moved:
                return TouchPhase.Moved;
            case UnityEngine.InputSystem.TouchPhase.Stationary:
                return TouchPhase.Stationary;
            case UnityEngine.InputSystem.TouchPhase.Ended:
                return TouchPhase.Ended;
            case UnityEngine.InputSystem.TouchPhase.Canceled:
                return TouchPhase.Canceled;
            default:
                return TouchPhase.Canceled;
        }
    }
#endif

    private void OnGUI()
    {
        if (!Application.isMobilePlatform || !useTouchAimOnMobile || !showTouchAimOverlay)
        {
            return;
        }

        EnsureOverlayResources();

        float rightX = Screen.width * aimZoneSplit;
        float rightWidth = Screen.width - rightX;
        DrawRect(new Rect(rightX, 0f, rightWidth, Screen.height), new Color(0f, 0f, 0f, 0.08f));
        GUI.Label(new Rect(Screen.width - 190f, Screen.height - 54f, 170f, 36f), "MIRAR", zoneLabelStyle);

        if (aimFingerId == -1)
        {
            return;
        }

        Vector2 startGui = ToGuiPoint(aimStartScreen);
        Vector2 knobGui = ToGuiPoint(aimStartScreen + aimInput * aimJoystickRadiusPixels);
        float baseSize = aimJoystickRadiusPixels * 2f;
        float knobSize = aimJoystickRadiusPixels * 0.9f;

        DrawCircle(startGui, baseSize, new Color(1f, 0.72f, 0.58f, 0.24f));
        DrawCircle(startGui, baseSize * 0.72f, new Color(1f, 0.74f, 0.6f, 0.18f));
        DrawCircle(knobGui, knobSize, new Color(1f, 0.9f, 0.82f, 0.56f));
    }

    private void EnsureOverlayResources()
    {
        if (whiteTexture == null)
        {
            whiteTexture = Texture2D.whiteTexture;
        }

        if (circleTexture == null)
        {
            circleTexture = CreateCircleTexture(128);
        }

        if (zoneLabelStyle == null)
        {
            zoneLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight
            };
            zoneLabelStyle.normal.textColor = new Color(1f, 1f, 1f, 0.55f);
        }
    }

    private void DrawCircle(Vector2 center, float size, Color color)
    {
        Rect rect = new Rect(center.x - size * 0.5f, center.y - size * 0.5f, size, size);
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, circleTexture);
        GUI.color = previous;
    }

    private void DrawRect(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = previous;
    }

    private static Vector2 ToGuiPoint(Vector2 screenPosition)
    {
        return new Vector2(screenPosition.x, Screen.height - screenPosition.y);
    }

    private static Texture2D CreateCircleTexture(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = (size - 1) * 0.5f;
        float feather = Mathf.Max(1f, size * 0.07f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01((radius - dist) / feather);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        return tex;
    }
}