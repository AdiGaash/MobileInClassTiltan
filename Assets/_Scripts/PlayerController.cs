using UnityEngine;

namespace Shooter
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        
        [Header("References")]
        [Tooltip("Reference to the CameraBoundsController (usually on the main camera)")]
        public CameraBoundsController cameraBoundsController;
        
        private InputHandler inputHandler;
        private bool boundsInitialized = false;
        
        // Local storage for movement boundaries
        private Rect movementBoundaries;
        
        private void Awake()
        {
            // Get the InputHandler component
            inputHandler = GetComponent<InputHandler>();
        }
        
        private void OnEnable()
        {
            // Subscribe to the movement input event
            if (inputHandler != null)
            {
                inputHandler.OnMovementInput += Move;
            }
            
            // Subscribe to camera boundaries changed event
            if (cameraBoundsController != null)
            {
                cameraBoundsController.OnBoundariesChanged += UpdateMovementBoundaries;
            }
        }
        
        private void OnDisable()
        {
            // Unsubscribe from the movement input event
            if (inputHandler != null)
            {
                inputHandler.OnMovementInput -= Move;
            }
            
            // Unsubscribe from camera boundaries changed event
            if (cameraBoundsController != null)
            {
                cameraBoundsController.OnBoundariesChanged -= UpdateMovementBoundaries;
            }
        }
        
        private void Start()
        {
            // Find the camera bounds controller if not set in inspector
            if (cameraBoundsController == null)
            {
                cameraBoundsController = Camera.main.GetComponent<CameraBoundsController>();
                
                // Subscribe to its events if we just found it
                if (cameraBoundsController != null)
                {
                    cameraBoundsController.OnBoundariesChanged += UpdateMovementBoundaries;
                }
            }
            
            if (cameraBoundsController != null)
            {
                // Set the calculation height to match the player's height
                cameraBoundsController.boundaryCalculationHeight = transform.position.y;
                
                // Calculate and store boundaries for this object
                movementBoundaries = cameraBoundsController.CalculateBoundariesForObject(gameObject);
                boundsInitialized = true;
                
                Debug.Log($"Initial movement boundaries set: X({movementBoundaries.xMin} to {movementBoundaries.xMax}), Z({movementBoundaries.yMin} to {movementBoundaries.yMax})");
            }
            else
            {
                Debug.LogError("CameraBoundsController not found! Please assign it in the inspector or add it to your main camera.");
            }
        }
        
        // Event handler for when camera boundaries change
        private void UpdateMovementBoundaries(Rect newBoundaries)
        {
            movementBoundaries = newBoundaries;
            boundsInitialized = true;
            Debug.Log($"Movement boundaries updated: X({movementBoundaries.xMin} to {movementBoundaries.xMax}), Z({movementBoundaries.yMin} to {movementBoundaries.yMax})");
        }
        
        // Clamp a position to stay within the local movement boundaries
        private Vector3 ClampPositionToBoundaries(Vector3 position)
        {
            if (!boundsInitialized)
            {
                Debug.LogWarning("Attempting to clamp position before boundaries are calculated!");
                return position;
            }
            
            position.x = Mathf.Clamp(position.x, movementBoundaries.xMin, movementBoundaries.xMax);
            position.z = Mathf.Clamp(position.z, movementBoundaries.yMin, movementBoundaries.yMax);
            
            return position;
        }
        
        // Check if a position is within boundaries
        private bool IsPositionWithinBoundaries(Vector3 position)
        {
            if (!boundsInitialized)
                return true; // Default to true if not calculated yet
                
            return position.x >= movementBoundaries.xMin && position.x <= movementBoundaries.xMax && 
                   position.z >= movementBoundaries.yMin && position.z <= movementBoundaries.yMax;
        }
        
        // Public move function that respects camera boundaries
        public void Move(Vector2 direction)
        {
            if (!boundsInitialized)
                return;
                
            // Normalize direction if it exceeds length of 1
            if (direction.magnitude > 1f)
                direction.Normalize();
                
            // Calculate the movement vector (direction.x moves along X, direction.y moves along Z)
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            
            // Calculate new position
            Vector3 newPosition = transform.position + movement;
            
            // Clamp the position within boundaries using our local method
            newPosition = ClampPositionToBoundaries(newPosition);
            
            // Apply the movement
            transform.position = newPosition;
        }
    }
}