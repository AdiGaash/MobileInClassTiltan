using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Shooter
{
    /// <summary>
    /// Manages spawning and despawning of objects based on prefab type or name.
    /// Works with GameArea to determine spawn positions and off-screen detection.
    /// Uses ObjectPoolManager for efficient object reuse.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the GameArea that defines spawning boundaries")]
        [SerializeField] private GameArea gameArea;
        [SerializeField] private string SpawnLayerName = "Default";
        
        public GameObject prefab;
        
        [SerializeField] float speedModifier = 1f;
        // List to track all active spawned objects
        private List<GameObject> activeObjects = new List<GameObject>();
        
        // Cache ObjectPoolManager reference
        [SerializeField] private ObjectPoolManager poolManager;
        SpawnLayer spawnLayer;
        private void Awake()
        {
            // Get references
            if (gameArea == null)
                gameArea = FindObjectOfType<GameArea>();
                
            
            
            if (gameArea == null)
                Debug.LogError("No GameArea found in scene! ObjectSpawner requires a GameArea to function properly.");
                
            if (poolManager == null)
                Debug.LogError("ObjectPoolManager singleton instance not found! Make sure it exists in the scene.");

            spawnLayer = gameArea.GetSpawnLayerByName(SpawnLayerName);
        }

        private void Start()
        {
            StartRepeatedSpawning(prefab, 2.5f);
        }

        private void Update()
        {
            CheckObjectsOutOfBounds();
        }
        
        /// <summary>
        /// Checks if any active objects are out of bounds and despawns them.
        /// </summary>
        private void CheckObjectsOutOfBounds()
        {
            // Create a temporary list to store objects that need to be despawned
            List<GameObject> objectsToDespawn = new List<GameObject>();
            
            // Check each active object of this prefab type
            foreach (var obj in activeObjects)
            {
                if (obj == null) continue;
                // Check if the object is out of bounds
                if (gameArea.IsOutOfBounds(obj.transform.position, spawnLayer))
                {
                    objectsToDespawn.Add(obj);
                }
            }
            
            // Despawn all objects that are out of bounds
            foreach (var obj in objectsToDespawn)
            {
                Despawn(obj);
            }
        }
        
        /// <summary>
        /// Spawns an object from the pool at a specific position and rotation.
        /// </summary>
        /// <param name="prefab">The prefab to spawn</param>
        /// <param name="position">Position to spawn at</param>
        /// <param name="rotation">Rotation to spawn with</param>
        /// <param name="spawnLayerName">Optional name of spawn layer to use</param>
        /// <returns>The spawned GameObject</returns>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                Debug.LogError("Cannot spawn a null prefab!");
                return null;
            }
            
            // Get object from pool
            GameObject obj = poolManager.GetPooledObject(prefab);
            if (obj == null) return null;
            
            // Set position and rotation
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            
            // Set movement properties
            AutonomousMove autonomousMove = obj.GetComponent<AutonomousMove>();
            if (autonomousMove != null)
            {
                autonomousMove.MoveDirection = Vector3.back;
                autonomousMove.MoveSpeed = speedModifier;
            }

            // Add to active objects list
            activeObjects.Add(obj);
            return obj;
        }
        
        /// <summary>
        /// Spawns an object from the top of the specified spawn layer.
        /// </summary>
        /// <param name="prefab">The prefab to spawn</param>
        /// <param name="spawnLayerName">Optional name of spawn layer to use</param>
        /// <returns>The spawned GameObject</returns>
        public GameObject SpawnFromTop(GameObject prefab)
        {
            if (gameArea == null)
            {
                Debug.LogError("GameArea not assigned! Cannot spawn from top.");
                return null;
            }
            
            // Get spawn position from the top of the layer
            Vector3 position = gameArea.GetSpawnPositionFromTop(spawnLayer);
            // Spawn the object at the calculated position
            return Spawn(prefab, position, Quaternion.identity);
        }
        
        /// <summary>
        /// Starts repeatedly spawning objects from the top at a given interval
        /// </summary>
        /// <param name="prefab">The prefab to spawn</param>
        /// <param name="spawnInterval">Time between spawns in seconds</param>
        /// <param name="maxSpawnCount">Maximum number of objects to spawn, 0 for infinite</param>
        /// <returns>Coroutine handle that can be used to stop spawning</returns>
        public Coroutine StartRepeatedSpawning(GameObject prefab, float spawnInterval, int maxSpawnCount = 0)
        {
            if (prefab == null)
            {
                Debug.LogError("Cannot start spawning with a null prefab!");
                return null;
            }
            
            return StartCoroutine(SpawnRepeatedlyCoroutine(prefab, spawnInterval, maxSpawnCount));
        }
        
        /// <summary>
        /// Coroutine that repeatedly spawns objects from the top
        /// </summary>
        private IEnumerator SpawnRepeatedlyCoroutine(GameObject prefab, float spawnInterval, int maxSpawnCount)
        {
            int spawnCount = 0;
            bool hasLimit = maxSpawnCount > 0;
            
            while (!hasLimit || spawnCount < maxSpawnCount)
            {
                SpawnFromTop(prefab);
                spawnCount++;
                
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        /// <summary>
        /// Stops all repeated spawning coroutines
        /// </summary>
        public void StopAllRepeatedSpawning()
        {
            StopAllCoroutines();
        }

        private void OnDisable()
        {
            StopAllRepeatedSpawning();
        }

        /// <summary>
        /// Despawns an object, returning it to the pool.
        /// </summary>
        /// <param name="obj">The object to despawn</param>
        public void Despawn(GameObject obj)
        {
            if (obj == null) return;
            
            // Remove from active objects list
            activeObjects.Remove(obj);
            
            // Return to pool
            poolManager.ReturnToPool(obj);
        }
    }
}
