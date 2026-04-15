using UnityEngine;

/// <summary>
/// Controlador principal de Luna.
/// Gerencia movimento, rotação e game logic.
/// </summary>
public class LunaController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public bool useTouchMoveOnMobile = true;
    public float joystickRadiusPixels = 130f;
    [Range(0.3f, 0.7f)] public float movementZoneSplit = 0.5f;
    public bool showTouchMoveOverlay = true;

    private Rigidbody2D rb2d;
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
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInMainMenu)
        {
            GameManager.Instance.StartGameplayFromMenu();
        }

        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }
        
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.simulated = true;
        rb2d.gravityScale = 0f;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb2d.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        bool isRealMobile = Application.platform == RuntimePlatform.Android || 
                           Application.platform == RuntimePlatform.IPhonePlayer;

        if (isRealMobile && useTouchMoveOnMobile)
        {
            HandleTouchJoystickMovement();
        }
        else
        {
            Vector2 moveInput = ReadMoveInput();
            
            if (moveInput.sqrMagnitude > 0)
            {
                Vector2 movement = moveInput * speed;
                rb2d.linearVelocity = movement;
                Debug.Log($"🚀 LUNA MOVEU! Velocity: {movement}");
            }
            else
            {
                rb2d.linearVelocity = Vector2.zero;
            }
        }

        RotateTowardFlashlight();
    }

    private void HandleTouchJoystickMovement()
    {
        AcquireMovementFinger();

        if (movementFingerId == -1)
        {
            joystickInput = Vector2.zero;
            rb2d.linearVelocity = Vector2.zero;
            return;
        }

        int touchCount = Input.touchCount;
        for (int i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.fingerId != movementFingerId)
            {
                continue;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                movementFingerId = -1;
                joystickInput = Vector2.zero;
                rb2d.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 delta = touch.position - joystickStartScreen;
            delta = Vector2.ClampMagnitude(delta, joystickRadiusPixels);
            joystickInput = delta / Mathf.Max(1f, joystickRadiusPixels);

            Vector2 movement = joystickInput * speed;
            rb2d.linearVelocity = movement;
            return;
        }

        movementFingerId = -1;
        joystickInput = Vector2.zero;
        rb2d.linearVelocity = Vector2.zero;
    }

    private void AcquireMovementFinger()
    {
        if (movementFingerId != -1)
        {
            return;
        }

        float movementZoneMaxX = Screen.width * movementZoneSplit;
        int touchCount = Input.touchCount;
        for (int i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
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
        Vector2 move = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) move.y += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) move.y -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) move.x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) move.x += 1f;

        return Vector2.ClampMagnitude(move, 1f);
    }

    private void RotateTowardFlashlight()
    {
        Transform flashlight = transform.Find("Flashlight");
        if (flashlight == null)
        {
            return;
        }

        Vector3 flashlightDir = flashlight.position - transform.position;
        
        if (flashlightDir.sqrMagnitude < 0.01f)
        {
            return;
        }

        float angle = Mathf.Atan2(flashlightDir.y, flashlightDir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

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