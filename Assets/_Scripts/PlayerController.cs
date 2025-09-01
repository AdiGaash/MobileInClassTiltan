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
        }
        
        private void OnDisable()
        {
            // Unsubscribe from the movement input event
            if (inputHandler != null)
            {
                inputHandler.OnMovementInput -= Move;
            }
        }
        
        private void Start()
        {
            // Find the camera bounds controller if not set in inspector
            if (cameraBoundsController == null)
            {
                cameraBoundsController = Camera.main.GetComponent<CameraBoundsController>();
            }
            
            if (cameraBoundsController != null)
            {
                // Set the calculation height to match the player's height
                cameraBoundsController.boundaryCalculationHeight = transform.position.y;
                
                // Calculate boundaries for this object
                cameraBoundsController.CalculateBoundariesForObject(gameObject);
                boundsInitialized = true;
            }
            else
            {
                Debug.LogError("CameraBoundsController not found! Please assign it in the inspector or add it to your main camera.");
            }
        }
        
        // Public move function that respects camera boundaries
        public void Move(Vector2 direction)
        {
            if (!boundsInitialized || cameraBoundsController == null)
                return;
                
            // Normalize direction if it exceeds length of 1
            if (direction.magnitude > 1f)
                direction.Normalize();
                
            // Calculate the movement vector (direction.x moves along X, direction.y moves along Z)
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            
            // Calculate new position
            Vector3 newPosition = transform.position + movement;
            
            // Clamp the position within boundaries
            newPosition = cameraBoundsController.ClampPositionToBoundaries(newPosition);
            
            // Apply the movement
            transform.position = newPosition;
        }
        
     
        
       
    }
}