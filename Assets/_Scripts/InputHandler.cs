using UnityEngine;

namespace Shooter
{
    public class InputHandler : MonoBehaviour
    {
        // Event for movement input
        public delegate void MovementInputEvent(Vector2 movementInput);
        public event MovementInputEvent OnMovementInput;

        // Input settings
        [Header("Input Settings")]
        [Tooltip("Use legacy input system instead of new Input System")]
        public bool useLegacyInput = true;

        private void Update()
        {
            // Get movement input
            Vector2 movementInput = GetMovementInput();
            
            // If there's any input, trigger the event
            if (movementInput != Vector2.zero)
            {
                Debug.Log($"Movement input detected: {movementInput}");
                OnMovementInput?.Invoke(movementInput);
            }
        }

        // Get player movement input from axes
        private Vector2 GetMovementInput()
        {
            float horizontalInput = 0f;
            float verticalInput = 0f;

            if (useLegacyInput)
            {
                // Use the legacy Input system
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");
            }
            else
            {
                // Use the new Input System
                // Note: This requires the Input System package and proper setup
                // This is a placeholder - implement according to your Input System setup
                // If you're using InputSystem_Actions.inputactions, you'll need to implement
                // the specific input reading based on your action configuration
            }

            return new Vector2(horizontalInput, verticalInput);
        }

      

        // Check if any movement key is pressed
        public bool IsMoving()
        {
            Vector2 input = GetMovementInput();
            return Mathf.Abs(input.x) > 0.1f || Mathf.Abs(input.y) > 0.1f;
        }
    }
}