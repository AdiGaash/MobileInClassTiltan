using UnityEngine;
using System.Collections;

namespace Shooter
{
    public class Optimized3 : MonoBehaviour
    {
        private Coroutine spawnRoutine;

        private void Start()
        {
            // Start a coroutine that handles timing automatically.
            spawnRoutine = StartCoroutine(SpawnEnemyRoutine());
        }

        private IEnumerator SpawnEnemyRoutine()
        {
            while (true)
            {
                Debug.Log("Spawning enemy...");
                SpawnEnemy();
                yield return new WaitForSeconds(2f); // Pause for 2 seconds without using CPU
            }
        }

        private void SpawnEnemy()
        {
            // Simulate an action like spawning or logging.
        }

        private void Update()
        {
            // Input still handled here (fast, per-frame logic only)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space pressed!");
            }
        }
    }
}