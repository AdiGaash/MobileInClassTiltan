using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    /// <summary>
    /// A MonoBehaviour-based object pooling system for efficient object reuse.
    /// Attach this to a GameObject to create and manage a pool of prefab instances.
    /// Implemented as a Singleton for easy access from other scripts.
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        [System.Serializable]
        public class PooledObjectInfo
        {
            public GameObject prefab;
            public int initialPoolSize = 10;
            public bool canExpand = true;
            [Tooltip("Optional tag to identify objects in this pool")]
            public string poolTag;
        }

        [Tooltip("Define the objects to be pooled with their initial quantities")]
        public List<PooledObjectInfo> pooledObjects = new List<PooledObjectInfo>();

        // Dictionary to store and access pool collections by prefab
        private Dictionary<GameObject, List<GameObject>> poolDictionary = new Dictionary<GameObject, List<GameObject>>();
        
        // Dictionary to store and access pool collections by tag
        private Dictionary<string, List<List<GameObject>>> poolDictionaryByTag = new Dictionary<string, List<List<GameObject>>>();
        
        // Dictionary to map prefabs to their corresponding tags for reverse lookup
        private Dictionary<GameObject, string> prefabTagMap = new Dictionary<GameObject, string>();
        
        // Dictionary to map a tag to its list of prefabs
        private Dictionary<string, List<GameObject>> tagPrefabsMap = new Dictionary<string, List<GameObject>>();

        protected override void Awake()
        {
            base.Awake();
            
            InitializePools();
        }

        /// <summary>
        /// Create all the pools based on the configuration
        /// </summary>
        private void InitializePools()
        {
            foreach (PooledObjectInfo poolInfo in pooledObjects)
            {
                if (poolInfo.prefab != null)
                {
                    CreatePool(poolInfo.prefab, poolInfo.initialPoolSize, poolInfo.poolTag);
                }
                else
                {
                    Debug.LogWarning("Null prefab in Object Pool configuration!");
                }
            }
        }

        /// <summary>
        /// Creates a new pool of game objects
        /// </summary>
        /// <param name="prefab">The prefab to pool</param>
        /// <param name="initialPoolSize">Initial number of instances</param>
        /// <param name="poolTag">Optional tag to identify this pool</param>
        public void CreatePool(GameObject prefab, int initialPoolSize, string poolTag = "")
        {
            if (prefab == null)
            {
                Debug.LogError("Cannot create a pool with a null prefab!");
                return;
            }

            // Create parent container for pool objects
            GameObject poolContainer = new GameObject($"Pool_{prefab.name}");
            poolContainer.transform.SetParent(transform);

            // Initialize list for this prefab
            List<GameObject> objectPool = new List<GameObject>();

            // Create initial instances
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = CreateNewInstance(prefab, poolContainer.transform);
                objectPool.Add(obj);
            }

            // Store the pool in our dictionary
            poolDictionary.Add(prefab, objectPool);
            
            // If a tag was provided, map prefab to tag and manage tag-based pools
            if (!string.IsNullOrEmpty(poolTag))
            {
                // Map prefab to its tag for reverse lookup
                prefabTagMap[prefab] = poolTag;
                
                // Add prefab to the list of prefabs with this tag
                if (!tagPrefabsMap.ContainsKey(poolTag))
                {
                    tagPrefabsMap[poolTag] = new List<GameObject>();
                }
                tagPrefabsMap[poolTag].Add(prefab);
                
                // Add the object pool to tag-based dictionary
                if (!poolDictionaryByTag.ContainsKey(poolTag))
                {
                    poolDictionaryByTag[poolTag] = new List<List<GameObject>>();
                }
                poolDictionaryByTag[poolTag].Add(objectPool);
            }
        }

        /// <summary>
        /// Creates a new instance of the prefab for the pool
        /// </summary>
        private GameObject CreateNewInstance(GameObject prefab, Transform parent)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(false);
            Poolable poolable = obj.GetComponent<Poolable>();
            if (poolable != null)
            {
                poolable.ObjectPoolManager = this;
            }
            return obj;
        }

        /// <summary>
        /// Gets an object from the specified pool
        /// </summary>
        /// <param name="prefab">The prefab whose instance you want</param>
        /// <param name="position">Position to set the object at</param>
        /// <param name="rotation">Rotation to set the object at</param>
        /// <returns>An instance of the prefab from the pool</returns>
        public GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(prefab))
            {
                Debug.LogWarning($"Pool for prefab {prefab.name} doesn't exist! Creating a new pool with default size.");
                CreatePool(prefab, 10);
            }

            List<GameObject> objectPool = poolDictionary[prefab];

            // Find an inactive object in the pool
            for (int i = 0; i < objectPool.Count; i++)
            {
                if (objectPool[i] != null && !objectPool[i].activeInHierarchy)
                {
                    GameObject obj = objectPool[i];
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                    obj.SetActive(true);
                    return obj;
                }
            }

            // No inactive object found in the pool, check if we can expand
            PooledObjectInfo poolInfo = pooledObjects.Find(info => info.prefab == prefab);
            bool canExpand = poolInfo != null ? poolInfo.canExpand : true;

            if (canExpand)
            {
                // Create a new instance and add it to the pool
                Transform poolParent = objectPool[0].transform.parent;
                GameObject newObj = CreateNewInstance(prefab, poolParent);
                newObj.transform.position = position;
                newObj.transform.rotation = rotation;
                newObj.SetActive(true);
                objectPool.Add(newObj);
                return newObj;
            }

            // If we can't expand the pool, return null
            Debug.LogWarning($"Could not get an object from the pool for {prefab.name} - pool at maximum capacity!");
            return null;
        }
        
        /// <summary>
        /// Gets an object from the pool by tag without specifying position or rotation
        /// </summary>
        /// <param name="poolTag">The tag of the pool</param>
        /// <returns>An instance from the tagged pool with default position/rotation</returns>
        public GameObject GetPooledObject(string poolTag)
        {
            return GetPooledObjectByTag(poolTag, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Gets an object from a pool identified by tag
        /// </summary>
        /// <param name="poolTag">The tag of the pool</param>
        /// <param name="position">Position to set the object at</param>
        /// <param name="rotation">Rotation to set the object at</param>
        /// <returns>An instance from the tagged pool</returns>
        public GameObject GetPooledObjectByTag(string poolTag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionaryByTag.ContainsKey(poolTag))
            {
                Debug.LogError($"No pool found with tag: {poolTag}");
                return null;
            }
            
            return GetRandomPooledObjectByTag(poolTag, position, rotation);
        }
        
        /// <summary>
        /// Gets a random object from pools with the specified tag
        /// </summary>
        /// <param name="poolTag">The tag of the pools to select from</param>
        /// <param name="position">Position to set the object at</param>
        /// <param name="rotation">Rotation to set the object at</param>
        /// <returns>A random instance from the tagged pools</returns>
        public GameObject GetRandomPooledObjectByTag(string poolTag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionaryByTag.ContainsKey(poolTag))
            {
                Debug.LogError($"No pool found with tag: {poolTag}");
                return null;
            }

            List<List<GameObject>> objectPools = poolDictionaryByTag[poolTag];
            
            // Randomize the order of pools to check
            int randomPoolIndex = Random.Range(0, objectPools.Count);
            
            // Try to get an inactive object from each pool in random order
            for (int poolIdx = 0; poolIdx < objectPools.Count; poolIdx++)
            {
                int currentPoolIndex = (randomPoolIndex + poolIdx) % objectPools.Count;
                List<GameObject> currentPool = objectPools[currentPoolIndex];
                
                // Find an inactive object in the current pool
                for (int i = 0; i < currentPool.Count; i++)
                {
                    if (currentPool[i] != null && !currentPool[i].activeInHierarchy)
                    {
                        GameObject obj = currentPool[i];
                        obj.transform.position = position;
                        obj.transform.rotation = rotation;
                        obj.SetActive(true);
                        return obj;
                    }
                }
            }

            // No inactive object found in any pool, try to expand one of them
            if (tagPrefabsMap.ContainsKey(poolTag) && tagPrefabsMap[poolTag].Count > 0)
            {
                // Choose a random prefab from this tag's prefabs
                int randomPrefabIndex = Random.Range(0, tagPrefabsMap[poolTag].Count);
                GameObject prefabToExpand = tagPrefabsMap[poolTag][randomPrefabIndex];
                
                // Find the corresponding pool info to check if it can expand
                PooledObjectInfo poolInfo = pooledObjects.Find(info => info.prefab == prefabToExpand);
                bool canExpand = poolInfo != null ? poolInfo.canExpand : true;

                if (canExpand)
                {
                    // Get the pool for this prefab
                    List<GameObject> objectPool = poolDictionary[prefabToExpand];
                    Transform poolParent = objectPool[0].transform.parent;
                    
                    // Create a new instance and add it to the pool
                    GameObject newObj = CreateNewInstance(prefabToExpand, poolParent);
                    newObj.transform.position = position;
                    newObj.transform.rotation = rotation;
                    newObj.SetActive(true);
                    objectPool.Add(newObj);
                    return newObj;
                }
            }

            // If we can't expand any pool, return null
            Debug.LogWarning($"Could not get an object from the pool with tag {poolTag} - all pools at maximum capacity!");
            return null;
        }

        /// <summary>
        /// Returns an object to the pool (deactivates it)
        /// </summary>
        /// <param name="obj">The object to return to the pool</param>
        public void ReturnToPool(GameObject obj)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
