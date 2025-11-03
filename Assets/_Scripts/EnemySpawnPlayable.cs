using UnityEngine;
using UnityEngine.Playables;

namespace Shooter
{
    /// <summary>
    /// Playable behaviour for spawning an enemy at a specific moment in a Timeline.
    /// </summary>
    public class EnemySpawnPlayable : PlayableBehaviour
    {
        public GameObject enemyPrefab;
        public Transform spawnPoint;
        public ObjectPoolManager poolManager;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (enemyPrefab == null || poolManager == null || spawnPoint == null)
            {
                Debug.LogWarning("EnemySpawnPlayable: Missing references!");
                return;
            }

            // Pull enemy from pool
            GameObject enemyObj = poolManager.GetPooledObject(enemyPrefab);

            if (enemyObj == null)
            {
                Debug.LogWarning("EnemySpawnPlayable: Pool returned null object!");
                return;
            }

            // Reset position/rotation
            enemyObj.transform.position = spawnPoint.position;
            enemyObj.transform.rotation = Quaternion.identity;
            enemyObj.SetActive(true);
            
        }
    }
}