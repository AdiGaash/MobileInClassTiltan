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
        public Transform bulletSpawnPoint;

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
            if (gameArea != null)
            {
                Bounds bounds = gameArea.GetComponent<Collider>().bounds;
                minBounds = new Vector2(bounds.min.x, bounds.min.z);
                maxBounds = new Vector2(bounds.max.x, bounds.max.z);
                boundsInitialized = true;
            }
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
            if (ShootingParameters.bulletPrefab == null || bulletSpawnPoint == null || ObjectPoolManager == null)
            {
                Debug.LogError("Missing references for shooting!");
                return;
            }

            GameObject bullet = ObjectPoolManager.GetPooledObject(ShootingParameters.bulletPrefab);
            if (bullet != null)
            {
                bullet.transform.position = bulletSpawnPoint.position;
                bullet.transform.rotation = bulletSpawnPoint.rotation;
                bullet.SetActive(true);
                
                var projectile = bullet.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Initialize(ShootingParameters.bulletDamage,
                        ShootingParameters.bulletSpeed,
                        ShootingParameters.bulletLifetime, ObjectPoolManager);
                }
            }
        }
    }
}
