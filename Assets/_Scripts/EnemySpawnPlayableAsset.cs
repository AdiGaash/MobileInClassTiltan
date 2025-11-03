using UnityEngine;
using UnityEngine.Playables;

namespace Shooter
{
    [CreateAssetMenu(fileName = "EnemySpawnClip", menuName = "Shooter/Enemy Spawn Clip")]
    public class EnemySpawnPlayableAsset : PlayableAsset
    {
        public GameObject enemyPrefab;
        public Transform spawnPoint;
       
        public ObjectPoolManager poolManager;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EnemySpawnPlayable>.Create(graph);

            EnemySpawnPlayable behaviour = playable.GetBehaviour();
            behaviour.enemyPrefab = enemyPrefab;
            behaviour.spawnPoint = spawnPoint;
            behaviour.poolManager = poolManager;

            return playable;
        }
    }
}