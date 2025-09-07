using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    /// <summary>
    /// A MonoBehaviour-based object pooling system for efficient object reuse.
    /// Attach this to a GameObject to create and manage a pool of prefab instances.
    /// </summary>
    public class ObjectPool : MonoBehaviour
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
        
        // Optional dictionary to store and access pool collections by tag
        private Dictionary<string, List<GameObject>> poolDictionaryByTag = new Dictionary<string, List<GameObject>>();

        
        

        private void Awake()
        {
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
            
            // If a tag was provided, also add it to the tag dictionary
            if (!string.IsNullOrEmpty(poolTag) && !poolDictionaryByTag.ContainsKey(poolTag))
            {
                poolDictionaryByTag.Add(poolTag, objectPool);
            }
        }

        /// <summary>
        /// Creates a new instance of the prefab for the pool
        /// </summary>
        private GameObject CreateNewInstance(GameObject prefab, Transform parent)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(false);
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

            List<GameObject> objectPool = poolDictionaryByTag[poolTag];
            
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
            PooledObjectInfo poolInfo = pooledObjects.Find(info => info.poolTag == poolTag);
            bool canExpand = poolInfo != null ? poolInfo.canExpand : true;

            if (canExpand && objectPool.Count > 0)
            {
                // Create a new instance and add it to the pool
                GameObject prefab = poolInfo.prefab;
                Transform poolParent = objectPool[0].transform.parent;
                GameObject newObj = CreateNewInstance(prefab, poolParent);
                newObj.transform.position = position;
                newObj.transform.rotation = rotation;
                newObj.SetActive(true);
                objectPool.Add(newObj);
                return newObj;
            }

            // If we can't expand the pool, return null
            Debug.LogWarning($"Could not get an object from the pool with tag {poolTag} - pool at maximum capacity!");
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
