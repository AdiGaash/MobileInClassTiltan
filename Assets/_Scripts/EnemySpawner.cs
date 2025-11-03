using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    /// <summary>
    /// Spawns and despawns enemies using GameArea boundaries and ObjectPoolManager.
    /// Does NOT handle enemy movement logic.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the GameArea that defines spawning boundaries")]
        [SerializeField] private GameArea gameArea;

        [Tooltip("Name of the layer within the GameArea used for spawning")]
        [SerializeField] private string spawnLayerName = "Default";

        [Tooltip("Prefab to spawn (should be an enemy prefab)")]
        [SerializeField] private GameObject enemyPrefab;

        [Tooltip("Reference to the ObjectPoolManager for pooling")]
        [SerializeField] private ObjectPoolManager poolManager;

        [Tooltip("Spawn interval in seconds")]
        [SerializeField] private float spawnInterval = 3f;

        [SerializeField] private int maxSpawnCount = 100;
        
        // Internal state
        private List<Enemy> activeEnemies = new List<Enemy>();
        private SpawnLayer spawnLayer;

        private void Awake()
        {
            // Ensure references
            if (gameArea == null)
                gameArea = FindObjectOfType<GameArea>();

            if (gameArea == null)
                Debug.LogError("EnemySpawner requires a GameArea in the scene!");

            if (poolManager == null)
                poolManager = FindObjectOfType<ObjectPoolManager>();

            if (poolManager == null)
                Debug.LogError("EnemySpawner requires an ObjectPoolManager in the scene!");

            // Get layer from GameArea
            spawnLayer = gameArea.GetSpawnLayerByName(spawnLayerName);
        }

        private void Start()
        {
            if (spawnInterval <= 0)
                Debug.LogWarning("EnemySpawner: spawnInterval is <= 0, enemies may spawn too quickly or not at all!");

            // Start spawning repeatedly
            StartRepeatedSpawning(enemyPrefab, spawnInterval);
        }

        private void Update()
        {
            // Regularly check which enemies left the screen or died
            CheckEnemiesOutOfBounds();
        }

        /// <summary>
        /// Checks if any active enemies are outside the GameArea or dead, and despawns them.
        /// </summary>
        private void CheckEnemiesOutOfBounds()
        {
            List<Enemy> toDespawn = new List<Enemy>();

            foreach (var enemy in activeEnemies)
            {
                if (enemy == null) continue;

                // Check if out of bounds or dead
                if (gameArea.IsOutOfBounds(enemy.transform.position, spawnLayer) || enemy.health <= 0)
                {
                    toDespawn.Add(enemy);
                }
            }

            foreach (var enemy in toDespawn)
            {
                Despawn(enemy);
            }
        }

        /// <summary>
        /// Spawns an enemy at a specific position and rotation.
        /// </summary>
        public Enemy Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                Debug.LogError("EnemySpawner: Cannot spawn a null prefab!");
                return null;
            }

            GameObject pooledObject = poolManager.GetPooledObject(prefab);
            if (pooledObject == null)
            {
                Debug.LogWarning("EnemySpawner: Pool returned null for prefab: " + prefab.name);
                return null;
            }

            Enemy enemy = pooledObject.GetComponent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("EnemySpawner: Spawned object does not have an Enemy component!");
                return null;
            }

            enemy.transform.position = position;
            enemy.transform.rotation = rotation;

            // Add to active tracking
            activeEnemies.Add(enemy);

            return enemy;
        }

        /// <summary>
        /// Spawns an enemy from the top of the GameArea spawn layer (no movement control).
        /// </summary>
        public Enemy SpawnFromTop(GameObject prefab)
        {
            if (gameArea == null)
            {
                Debug.LogError("EnemySpawner: No GameArea assigned!");
                return null;
            }

            if (prefab == null)
            {
                Debug.LogError("EnemySpawner: Cannot spawn from top with a null prefab!");
                return null;
            }

            // Get a spawn position from the top boundary of the layer
            Vector3 spawnPos = gameArea.GetSpawnPositionFromTop(spawnLayer);

            // Spawn from pool at that position
            Enemy enemy = Spawn(prefab, spawnPos, Quaternion.identity);
            
            
            
            // No movement setup — movement handled by the Enemy itself (AI, physics, etc.)
            return enemy;
        }

        /// <summary>
        /// Starts repeatedly spawning enemies from the top at fixed intervals.
        /// </summary>
        public Coroutine StartRepeatedSpawning(GameObject prefab, float interval, int maxSpawnCount = 0)
        {
            if (prefab == null)
            {
                Debug.LogError("EnemySpawner: Cannot start repeated spawning with null prefab!");
                return null;
            }

            return StartCoroutine(SpawnRepeatedly(prefab, interval, maxSpawnCount));
        }

        private IEnumerator SpawnRepeatedly(GameObject prefab, float interval, int maxSpawnCount)
        {
            int spawned = 0;
            bool hasLimit = maxSpawnCount > 0;

            while (!hasLimit || spawned < maxSpawnCount)
            {
                Enemy newEnemy = SpawnFromTop(prefab);
                newEnemy.InitAttack();
                
                spawned++;
                yield return new WaitForSeconds(interval);
            }
        }

        /// <summary>
        /// Despawns an enemy and returns it to the pool.
        /// </summary>
        public void Despawn(Enemy enemy)
        {
            if (enemy == null) return;

            activeEnemies.Remove(enemy);
            enemy.canMoveTowardsPlayer = false;
            poolManager.ReturnToPool(enemy.gameObject);
            
        }

        /// <summary>
        /// Stops all active spawn coroutines.
        /// </summary>
        public void StopAllRepeatedSpawning()
        {
            StopAllCoroutines();
        }

        private void OnDisable()
        {
            StopAllRepeatedSpawning();
        }
    }
}
