using UnityEngine;

namespace Shooter
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerController : MonoBehaviour
    {
        public LevelParameters LevelParameters;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;

        [Header("Shooting Settings")]
        [Tooltip("Time between shots in seconds")]
        public float fireRate = 0.5f;
        private float nextFireTime;

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
            inputHandler = GetComponent<InputHandler>();
            if (inputHandler != null)
            {
                Debug.Log("Successfully subscribed to input events");
                inputHandler.OnMovementInput += Move;
                inputHandler.OnShootInput += HandleShooting;
            }
        }

        private void OnDisable()
        {
            if (inputHandler != null)
            {
                inputHandler.OnMovementInput -= Move;
                inputHandler.OnShootInput -= HandleShooting;
            }
        }

        private void Start()
        {
            SetInputHandler();
            InitializeBoundaries();
        }

        private void InitializeBoundaries()
        {
            if (gameArea == null)
            {
                Debug.LogError("GameArea not assigned to PlayerController! Please assign it in the inspector.");
                return;
            }

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

        private Vector3 ClampPositionToBoundaries(Vector3 position)
        {
            if (!boundsInitialized)
            {
                Debug.LogWarning("Attempting to clamp position before boundaries are initialized!");
                return position;
            }

            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
            return position;
        }

        public void Move(Vector2 direction)
        {
            moveSpeed = LevelParameters.PlayerSpeed;

            if (!boundsInitialized)
                return;

            if (direction.magnitude > 1f)
                direction.Normalize();

            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;
            newPosition = ClampPositionToBoundaries(newPosition);
            transform.position = newPosition;
        }

        private void HandleShooting()
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        private void Shoot()
        {
            Debug.Log("Player fired weapon");
            // Implement actual shooting logic here
            // For example: Instantiate projectiles, raycasts, etc.
        }
    }
}
