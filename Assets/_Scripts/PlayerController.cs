using UnityEngine;

namespace Shooter
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerController : MonoBehaviour
    {
        
        public LevelParameters LevelParameters;
        
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        
        [Header("References")]
        [Tooltip("Reference to the GameArea that defines the play area")]
        public GameArea gameArea;
        
        [Tooltip("Name of the layer to use for player boundaries")]
        public string boundaryLayerName = "PlayerBoundary";
        
        private InputHandler inputHandler;
        private bool boundsInitialized = false;


        private Vector2 minBounds;
        private Vector2 maxBounds;
        
        
        private void SetInputHandler()
        { 
            // Subscribe to the movement input event
            inputHandler = GetComponent<InputHandler>();
            if (inputHandler != null)
            { 
                Debug.Log("Successfully subscribed to OnMovementInput event");
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
            SetInputHandler();
            InitializeBoundaries();
        }
        
        // Initialize the movement boundaries using GameArea's SpawnLayer
        private void InitializeBoundaries()
        {
            if (gameArea == null)
            {
                Debug.LogError("GameArea not assigned to PlayerController! Please assign it in the inspector.");
                return;
            }
            
            // Find the appropriate layer for player boundaries
            foreach (var layer in gameArea.layers)
            {
                if (layer.name == boundaryLayerName)
                {
                    minBounds = layer.minBounds;
                    maxBounds = layer.maxBounds;
                    
                    Debug.Log($"Player boundary layer '{boundaryLayerName}' found and initialized");
                    boundsInitialized = true;
                    return;
                }
            }
            
            Debug.LogError($"Layer '{boundaryLayerName}' not found in GameArea. Please create this layer in the GameArea component.");
        }
        
        // Clamp a position to stay within the player boundary layer
        private Vector3 ClampPositionToBoundaries(Vector3 position)
        {
            if (!boundsInitialized)
            {
                Debug.LogWarning("Attempting to clamp position before boundaries are initialized!");
                return position;
            }
            
            // Clamp X and Z (we assume Y doesn't change for typical top-down or side-scrolling games)
            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
            
            return position;
        }
        
       
        
        // Public move function that respects the game area boundaries
        public void Move(Vector2 direction)
        {
            
            moveSpeed = LevelParameters.PlayerSpeed;
            
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