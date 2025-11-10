using UnityEngine;

namespace Shooter
{
    public class InputHandler : MonoBehaviour
    {
        // Movement events
        public delegate void MovementInputEvent(Vector2 movementInput);
        public event MovementInputEvent OnMovementInput;

        // Shooting events
        public delegate void ShootInputEvent();
        public event ShootInputEvent OnShootInput;

        // Input settings
        [Header("Input Settings")]
        [Tooltip("Use legacy input system instead of new Input System")]
        public bool useLegacyInput = true;
        
        [Tooltip("Name of the fire button in Input settings")]
        public string fireButtonName = "Fire1";

        private void Update()
        {
            // Handle movement input
            Vector2 movementInput = GetMovementInput();
            if (movementInput != Vector2.zero)
            {
                Debug.Log($"Movement input detected: {movementInput}");
                OnMovementInput?.Invoke(movementInput);
            }

            // Handle shooting input
            if (GetShootInput())
            {
                Debug.Log("Shoot input detected");
                OnShootInput?.Invoke();
            }
        }

        private Vector2 GetMovementInput()
        {
            float horizontalInput = 0f;
            float verticalInput = 0f;

            if (useLegacyInput)
            {
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");
            }
            else
            {
                // New Input System implementation placeholder
            }

            return new Vector2(horizontalInput, verticalInput);
        }

        private bool GetShootInput()
        {
            if (useLegacyInput)
            {
                return Input.GetButtonDown(fireButtonName);
            }
            else
            {
                // New Input System implementation placeholder
                // Return false for now until implemented
                return false;
            }
        }

        public bool IsMoving()
        {
            Vector2 input = GetMovementInput();
            return Mathf.Abs(input.x) > 0.1f || Mathf.Abs(input.y) > 0.1f;
        }
    }
}
