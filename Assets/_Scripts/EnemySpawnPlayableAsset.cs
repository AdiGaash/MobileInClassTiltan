using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Shooter
{
    [CreateAssetMenu(fileName = "EnemySpawnClip", menuName = "Shooter/Enemy Spawn Clip")]
    public class EnemySpawnPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [Header("Enemy Settings")]
        [Tooltip("Enemy prefab to spawn")]
        public GameObject enemyPrefab;

        [Tooltip("Number of enemies to spawn")]
        public int spawnCount = 1;

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EnemySpawnPlayable>.Create(graph);
            EnemySpawnPlayable behaviour = playable.GetBehaviour();

            behaviour.enemyPrefab = enemyPrefab;
            behaviour.spawnCount = spawnCount;
            behaviour.poolManager = Object.FindFirstObjectByType<ObjectPoolManager>();

            if (behaviour.poolManager == null)
                Debug.LogWarning("EnemySpawnPlayableAsset: No ObjectPoolManager found in the scene!");

            return playable;
        }
    }
}