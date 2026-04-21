using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GhostBeam.Player
{
    public class FlashlightController : MonoBehaviour
    {
        [SerializeField] private Light2D flashlight;
        [SerializeField] private float rotationSpeed = 10f;

        private float currentRotation = 0f;

        private void Update()
        {
            HandleAim();
        }

        private void HandleAim()
        {
            Vector2 aimInput = Vector2.zero;

            // Mouse (debug)
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            aimInput = (worldPos - transform.position).normalized;

            // Touch mobile
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                
                // Joystick direito (mira) - bottom-right
                if (touch.position.x > Screen.width * 0.5f)
                {
                    Vector2 joystickCenter = new Vector2(Screen.width * 0.8f, Screen.height * 0.2f);
                    aimInput = (touch.position - joystickCenter).normalized;
                }
            }

            // Calcular rotação em graus
            if (aimInput.magnitude > 0.1f)
            {
                currentRotation = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg - 90f;
            }

            // Aplicar rotação suave
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.AngleAxis(currentRotation, Vector3.forward),
                Time.deltaTime * rotationSpeed
            );
        }

        public bool IsActive => flashlight != null && flashlight.enabled;

        public void SetActive(bool active)
        {
            if (flashlight != null)
                flashlight.enabled = active;
        }
    }
}
