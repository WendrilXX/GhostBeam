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

        public Vector2 MoveInput => moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // Calcular limites da tela
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
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
                Touch touch = Input.GetTouch(0);
                
                // Joystick esquerdo (movimento) - bottom-left
                if (touch.position.x < Screen.width * 0.5f)
                {
                    Vector2 joystickCenter = new Vector2(Screen.width * 0.2f, Screen.height * 0.2f);
                    Vector2 joystickInput = (touch.position - joystickCenter).normalized;
                    
                    if (joystickInput.magnitude > horizontalDeadzone)
                    {
                        moveInput = joystickInput;
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
