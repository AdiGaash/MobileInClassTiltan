using System.Collections;
using UnityEngine;

namespace Shooter
{
    public class Spawner : MonoBehaviour
    {
        [Tooltip("Reference to the GameArea script")]
        public GameArea gameArea;
        
        [Tooltip("The layer from which to spawn objects")]
        public string targetLayerName = "Default";
        
        [Tooltip("Tag of objects to spawn from the object pool")]
        public string objectPoolTag;
        
        [Tooltip("Time between spawns in seconds")]
        public float spawnInterval = 2f;
        
        [Tooltip("Whether spawning is active")]
        public bool isSpawning = true;
        
        private SpawnLayer targetLayer;
        private bool isInitialized = false;
        private WaitForSeconds waitTime;

        private void Start()
        {
            
            
            // Find the target layer by name
            InitializeTargetLayer();
            
            // Initialize wait time
            waitTime = new WaitForSeconds(spawnInterval);
            
            // Start spawning
            if (isSpawning)
                StartCoroutine(SpawnRoutine());
        }
        
        private void InitializeTargetLayer()
        {
            foreach (SpawnLayer layer in gameArea.layers)
            {
                if (layer.name == targetLayerName)
                {
                    targetLayer = layer;
                    isInitialized = true;
                    break;
                }
            }
            
            if (!isInitialized)
            {
                Debug.LogWarning($"Spawner could not find layer named '{targetLayerName}'. Spawning is disabled.");
                isSpawning = false;
            }
        }
        
        private IEnumerator SpawnRoutine()
        {
            while (isSpawning)
            {
                SpawnObject();
                yield return waitTime;
            }
        }
        
        public void SpawnObject()
        {
            if (!isInitialized || string.IsNullOrEmpty(objectPoolTag))
                return;
                
            // Get spawn position from the GameArea
            Vector3 spawnPosition = gameArea.GetSpawnPositionFromTop(targetLayer);
            
            
            // Get an object from the pool
            GameObject spawnedObject = ObjectPoolManager.Instance.GetPooledObject(objectPoolTag);
            
            if (spawnedObject != null)
            {
                spawnedObject.transform.position = spawnPosition;
                spawnedObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"No pooled object with tag '{objectPoolTag}' available!");
            }
        }
        
        public void StartSpawning()
        {
            if (!isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SpawnRoutine());
            }
        }
        
        public void StopSpawning()
        {
            isSpawning = false;
        }
        
        // For editor usage - allows updating the target layer if it changes
        public void SetTargetLayer(string layerName)
        {
            targetLayerName = layerName;
            isInitialized = false;
            InitializeTargetLayer();
        }
    }
}
