using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using UnityEngine.Timeline;

namespace Shooter
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "EnemySpawnClip", menuName = "Shooter/Enemy Spawn Clip")]
    public class EnemySpawnPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [Header("Enemy Settings")]
        [Tooltip("Enemy prefab to spawn")]
        public GameObject enemyPrefab;

        [Tooltip("Number of enemies to spawn")]
        public int spawnCount = 1;

        // Change to SplineContainer instead of GameObject
        public ExposedReference<SplineContainer> splineToFollow;

        public ClipCaps clipCaps => ClipCaps.All;
        
        [HideInInspector] public double clipDuration = 1.0;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EnemySpawnPlayable>.Create(graph);
            EnemySpawnPlayable behaviour = playable.GetBehaviour();

            behaviour.enemyPrefab = enemyPrefab;
            behaviour.spawnCount = spawnCount;

            // Resolve the SplineContainer directly
            var director = owner != null ? owner.GetComponent<PlayableDirector>() : null;
            if (director != null)
                behaviour.splineToFollow = splineToFollow.Resolve(director);
            else
            {
                behaviour.splineToFollow = null;
                Debug.LogWarning("EnemySpawnPlayableAsset: splineToFollow not resolved. Bind the SplineContainer to the exposed reference on the Timeline.");
            }

            behaviour.clipDuration = (float)clipDuration;
            behaviour.poolManager = Object.FindFirstObjectByType<ObjectPoolManager>();

            if (behaviour.poolManager == null)
                Debug.LogWarning("EnemySpawnPlayableAsset: No ObjectPoolManager found in the scene!");

            return playable;
        }
    }
}