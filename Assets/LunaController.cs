using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class LunaController : MonoBehaviour
{
    public float speed = 5f;
    public bool useTouchMoveOnMobile = true;
    public float joystickRadiusPixels = 130f;
    [Range(0.3f, 0.7f)] public float movementZoneSplit = 0.5f;
    public bool showTouchMoveOverlay = true;

    private int movementFingerId = -1;
    private Vector2 joystickStartScreen;
    private Vector2 joystickInput;

    private Texture2D circleTexture;
    private Texture2D whiteTexture;
    private GUIStyle zoneLabelStyle;

    private struct TouchData
    {
        public int fingerId;
        public Vector2 position;
        public TouchPhase phase;
    }

    private void Awake()
    {
        // Garantir que Luna tem Rigidbody2D para colisões funcionarem
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Garantir que Luna tem Collider2D para permitir colisões
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;
        }
    }

    private void Update()
    {
        if (Application.isMobilePlatform && useTouchMoveOnMobile)
        {
            HandleTouchJoystickMovement();
            return;
        }

        Vector2 moveInput = ReadMoveInput();
        float moveX = moveInput.x;
        float moveY = moveInput.y;

        transform.position += new Vector3(moveX, moveY, 0f) * speed * Time.deltaTime;
    }

    private void HandleTouchJoystickMovement()
    {
        AcquireMovementFinger();

        if (movementFingerId == -1)
        {
            joystickInput = Vector2.zero;
            return;
        }

        int touchCount = GetTouchCount();
        for (int i = 0; i < touchCount; i++)
        {
            TouchData touch = GetTouchAt(i);
            if (touch.fingerId != movementFingerId)
            {
                continue;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                movementFingerId = -1;
                joystickInput = Vector2.zero;
                return;
            }

            Vector2 delta = touch.position - joystickStartScreen;
            delta = Vector2.ClampMagnitude(delta, joystickRadiusPixels);
            joystickInput = delta / Mathf.Max(1f, joystickRadiusPixels);

            Vector3 movement = new Vector3(joystickInput.x, joystickInput.y, 0f) * speed * Time.deltaTime;
            transform.position += movement;
            return;
        }

        movementFingerId = -1;
        joystickInput = Vector2.zero;
    }

    private void AcquireMovementFinger()
    {
        if (movementFingerId != -1)
        {
            return;
        }

        float movementZoneMaxX = Screen.width * movementZoneSplit;
        int touchCount = GetTouchCount();
        for (int i = 0; i < touchCount; i++)
        {
            TouchData touch = GetTouchAt(i);
            if (touch.phase != TouchPhase.Began)
            {
                continue;
            }

            if (touch.position.x > movementZoneMaxX)
            {
                continue;
            }

            movementFingerId = touch.fingerId;
            joystickStartScreen = touch.position;
            joystickInput = Vector2.zero;
            return;
        }
    }

    private Vector2 ReadMoveInput()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Vector2 move = Vector2.zero;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) move.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) move.x += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) move.y -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) move.y += 1f;
        }

        Gamepad gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Vector2 stick = gamepad.leftStick.ReadValue();
            if (stick.sqrMagnitude > move.sqrMagnitude)
            {
                move = stick;
            }
        }

        return Vector2.ClampMagnitude(move, 1f);
#else
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
        if (!Application.isMobilePlatform || !useTouchMoveOnMobile || !showTouchMoveOverlay)
        {
            return;
        }

        EnsureOverlayResources();

        float leftWidth = Screen.width * movementZoneSplit;
        DrawRect(new Rect(0f, 0f, leftWidth, Screen.height), new Color(0f, 0f, 0f, 0.08f));
        GUI.Label(new Rect(18f, Screen.height - 54f, 220f, 36f), "MOVER", zoneLabelStyle);

        if (movementFingerId == -1)
        {
            return;
        }

        Vector2 startGui = ToGuiPoint(joystickStartScreen);
        Vector2 knobGui = ToGuiPoint(joystickStartScreen + joystickInput * joystickRadiusPixels);
        float baseSize = joystickRadiusPixels * 2f;
        float knobSize = joystickRadiusPixels * 0.9f;

        DrawCircle(startGui, baseSize, new Color(0.8f, 0.9f, 1f, 0.2f));
        DrawCircle(startGui, baseSize * 0.72f, new Color(0.7f, 0.86f, 1f, 0.16f));
        DrawCircle(knobGui, knobSize, new Color(0.95f, 0.98f, 1f, 0.52f));
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
                alignment = TextAnchor.MiddleLeft
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