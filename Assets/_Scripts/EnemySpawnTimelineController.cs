using UnityEngine;
using UnityEngine.Playables;

namespace Shooter
{
    /// <summary>
    /// Controls enemy spawning using a Timeline (PlayableDirector) instead of coroutines.
    /// </summary>
    public class EnemySpawnTimelineController : MonoBehaviour
    {
        public PlayableDirector playableDirector;

        private void Start()
        {
            if (playableDirector == null)
            {
                playableDirector = GetComponent<PlayableDirector>();
            }

            if (playableDirector != null)
            {
                playableDirector.Play();
            }
            else
            {
                Debug.LogWarning("EnemySpawnTimelineController: No PlayableDirector assigned!");
            }
        }
    }
}