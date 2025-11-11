using UnityEngine;

namespace Shooter
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerController : MonoBehaviour
    {
        public LevelParameters LevelParameters;
        public ShootingParameters ShootingParameters;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;

        [Header("References")]
        [Tooltip("Reference to the GameArea that defines the play area")]
        public GameArea gameArea;

        [Tooltip("Name of the layer to use for player boundaries")]
        public string boundaryLayerName = "PlayerBoundary";

        [Tooltip("Spawn point for bullets")]
        public Transform[] bulletSpawnPoints;

        private InputHandler inputHandler;
        private bool boundsInitialized = false;
        private Vector2 minBounds;
        private Vector2 maxBounds;
        private float nextFireTime;
        public ObjectPoolManager ObjectPoolManager;

        private void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            InitializeBoundaries();
            
        }

        public void SetInputHandler(InputHandler handler)
        {
            inputHandler = handler;
        }

        private void OnDisable()
        {
            boundsInitialized = false;
        }

        private void Update()
        {
            if (!boundsInitialized)
            {
                InitializeBoundaries();
                return;
            }

            Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Move(movement);

            if (Input.GetButton("Fire1"))
            {
                HandleShooting();
            }
        }

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

        private void Move(Vector2 input)
        {
            input = input.normalized;
            Vector3 movement = new Vector3(input.x, 0f, input.y) * (moveSpeed * Time.deltaTime);
            Vector3 newPosition = transform.position + movement;
            transform.position = ClampPositionToBoundaries(newPosition);
        }

        private Vector3 ClampPositionToBoundaries(Vector3 position)
        {
            if (!boundsInitialized) return position;

            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
            return position;
        }

        private void HandleShooting()
        {
            if (Time.time >= nextFireTime && ShootingParameters != null)
            {
                Shoot();
                nextFireTime = Time.time + ShootingParameters.fireRate;
            }
        }

        private void Shoot()
        {
            // Validate required references
            if (ShootingParameters == null)
            {
                Debug.LogError("ShootingParameters is null!");
                return;
            }
            if (ShootingParameters.bulletPrefab == null)
            {
                Debug.LogError("ShootingParameters.bulletPrefab is null!");
                return;
            }
            if (ObjectPoolManager == null)
            {
                Debug.LogError("ObjectPoolManager is null!");
                return;
            }
            if (bulletSpawnPoints == null || bulletSpawnPoints.Length == 0)
            {
                Debug.LogError("No bullet spawn points assigned!");
                return;
            }

            int numBullets = ShootingParameters.numOfBulletsPerShot;

            // Decide which spawn point indices to use based on requested bullet count
            int[] spawnIndices;
            switch (numBullets)
            {
                case 1:
                    spawnIndices = new int[] { 0 };          // use index 0
                    break;
                case 2:
                    spawnIndices = new int[] { 1, 2 };      // use indices 1 and 2 (not 0)
                    break;
                case 3:
                    spawnIndices = new int[] { 0, 1, 2 };   // use 0,1,2
                    break;
                default:
                    Debug.LogError($"Unsupported number of bullets per shot: {numBullets}. Supported: 1, 2, 3.");
                    return;
            }

            // Validate that required spawn indices exist in the array
            foreach (int idx in spawnIndices)
            {
                if (idx < 0 || idx >= bulletSpawnPoints.Length)
                {
                    Debug.LogError($"Bullet spawn point index {idx} is out of range. bulletSpawnPoints.Length = {bulletSpawnPoints.Length}");
                    return;
                }
                if (bulletSpawnPoints[idx] == null)
                {
                    Debug.LogError($"bulletSpawnPoints[{idx}] is null!");
                    return;
                }
            }

            // Pull bullets from pool and initialize them at the chosen spawn points
            foreach (int idx in spawnIndices)
            {
                Transform spawnPoint = bulletSpawnPoints[idx];

                GameObject bullet = ObjectPoolManager.GetPooledObject(ShootingParameters.bulletPrefab);
                if (bullet == null)
                {
                    Debug.LogWarning("ObjectPoolManager returned null (no available bullet in pool).");
                    continue;
                }

                bullet.transform.position = spawnPoint.position;
                bullet.transform.rotation = spawnPoint.rotation;
                bullet.SetActive(true);

                Projectile projectile = bullet.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Initialize(
                        ShootingParameters.bulletDamage,
                        ShootingParameters.bulletSpeed,
                        ShootingParameters.bulletLifetime,
                        ObjectPoolManager
                    );
                }
                else
                {
                    Debug.LogWarning("Pooled bullet object is missing a Projectile component.");
                }
            }
        }
        
    }
}
