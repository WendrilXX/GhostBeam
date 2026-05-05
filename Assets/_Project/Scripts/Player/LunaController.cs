using UnityEngine;

namespace GhostBeam.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class LunaController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float horizontalDeadzone = 0.3f;

        private Rigidbody2D rb;
        private Vector2 moveInput = Vector2.zero;
        private Vector2 screenBounds;
        private int movementFingerId = -1;
        private float screenHalfWidth;
        private Vector2 movementCenter;

        public Vector2 MoveInput => moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // Calcular limites da tela
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            screenHalfWidth = Screen.width * 0.5f;
            movementCenter = new Vector2(Screen.width * 0.2f, Screen.height * 0.2f);
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void HandleInput()
        {
            moveInput = Vector2.zero;

            // Teclado (debug)
            if (Input.GetKey(KeyCode.W)) moveInput.y = 1;
            if (Input.GetKey(KeyCode.S)) moveInput.y = -1;
            if (Input.GetKey(KeyCode.D)) moveInput.x = 1;
            if (Input.GetKey(KeyCode.A)) moveInput.x = -1;

            // Touch mobile
            if (Input.touchCount > 0)
            {
                Touch activeTouch = default;
                bool found = false;

                if (movementFingerId != -1)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.fingerId == movementFingerId)
                        {
                            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                            {
                                movementFingerId = -1;
                            }
                            else
                            {
                                activeTouch = touch;
                                found = true;
                            }
                            break;
                        }
                    }
                }

                if (!found)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.position.x < screenHalfWidth && touch.phase == TouchPhase.Began)
                        {
                            movementFingerId = touch.fingerId;
                            activeTouch = touch;
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.position.x < screenHalfWidth && touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                        {
                            movementFingerId = touch.fingerId;
                            activeTouch = touch;
                            found = true;
                            break;
                        }
                    }
                }

                if (found && movementFingerId != -1)
                {
                    Vector2 joystickInput = (activeTouch.position - movementCenter);
                    if (joystickInput.magnitude > horizontalDeadzone)
                    {
                        moveInput = joystickInput.normalized;
                    }
                }
            }

            moveInput = Vector2.ClampMagnitude(moveInput, 1f);
        }

        private void Move()
        {
            Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
            
            // Clamp dentro dos bounds
            newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x, screenBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y, screenBounds.y);
            
            rb.position = newPosition;
        }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 20f);
            #endif
        }
    }
}
