using UnityEngine;

namespace Shooter
{
    public class Unoptimized3 : MonoBehaviour
    {
        private float timer = 0f;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 2f)
            {
                Debug.Log("Spawning enemy...");
                SpawnEnemy();
                timer = 0f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space pressed!");
            }
        }

        private void SpawnEnemy()
        {
            Debug.Log("Spawning enemy...");
        }
    }

}