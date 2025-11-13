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

        [HideInInspector] public float clipDuration; // 👈 Duration passed from PlayableAsset

       
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // Spawn enemies when this playable starts
            for (int i = 0; i < spawnCount; i++)
            {
                GameObject enemy = SpawnEnemy(playable);
                if (enemy != null)
                {
                    spawnedEnemies.Add(enemy);
                }
            }
        }


        private GameObject SpawnEnemy(Playable playable)
        {
            if (enemyPrefab == null || splineToFollow == null)
            {
                Debug.LogWarning("EnemySpawnPlayable: Missing enemy prefab or spline!");
                return null;
            }

            GameObject enemy = poolManager != null ? poolManager.GetPooledObject(enemyPrefab) : null;
            if (enemy == null) return null;

            SplineFollower follower = enemy.GetComponent<SplineFollower>();
            if (follower == null)
            {
                follower = enemy.AddComponent<SplineFollower>();
            }
            follower.enabled = false;
            // Configure the follower
            // Get clip duration and initialize the follower
           
            
            follower.Initialize(clipDuration, splineToFollow);

            follower.enabled = true;
            return enemy;
        }


        
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            
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
            
        }
    }
}