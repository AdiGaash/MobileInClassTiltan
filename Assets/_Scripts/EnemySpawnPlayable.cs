using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;

namespace Shooter
{
    // Handles enemy spawning only (no animation or movement)
    public class EnemySpawnPlayable : PlayableBehaviour
    {
        [Header("Enemy Settings")]
        public GameObject enemyPrefab;
        public int spawnCount = 1;
        public SplineContainer splineToFollow;

        [Header("Pooling")]
        public ObjectPoolManager poolManager;

        // Keep track of spawned enemies for later cleanup
        private readonly List<GameObject> spawnedEnemies = new List<GameObject>();


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // Spawn enemies when this playable starts
            for (int i = 0; i < spawnCount; i++)
            {
                GameObject enemy = SpawnEnemy();
                if (enemy != null)
                {
                    spawnedEnemies.Add(enemy);
                }
            }
        }


        private GameObject SpawnEnemy()
        {
            if (enemyPrefab == null)
            {
                Debug.LogWarning("EnemySpawnPlayable: No enemy prefab assigned!");
                return null;
            }

            // Get from pool if possible, otherwise instantiate
            if (poolManager != null)
                return poolManager.GetPooledObject(enemyPrefab);
            else
                return null;
        }
        
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //if (info.effectiveParentSpeed > 0f)
            //{
                // When the timeline clip ends, return enemies to pool (or destroy them)
                foreach (var enemy in spawnedEnemies)
                {
                    if (enemy != null)
                    {
                        if (poolManager != null)
                            poolManager.ReturnToPool(enemy);
                        else
                            Object.Destroy(enemy);
                    }
                }

                spawnedEnemies.Clear();
            //}
        }
    }
}