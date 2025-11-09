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

            // Configure the follower
            follower.splineContainer = splineToFollow;
            follower.followRotation = true;
            follower.followForward = true;
            follower.startPosition = 0f;

            // Get clip duration and initialize the follower
            double clipDuration = ((PlayableDirector)playable.GetGraph().GetResolver()).duration;
            follower.Initialize((float)clipDuration);

            follower.enabled = true;
            return enemy;
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