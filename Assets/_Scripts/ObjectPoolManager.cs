using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    /// <summary>
    /// A MonoBehaviour-based object pooling system for efficient object reuse.
    /// Attach this to a GameObject to create and manage a pool of prefab instances.
    /// Implemented as a Singleton for easy access from other scripts.
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        [System.Serializable]
        public class PooledObjectInfo
        {
            public GameObject prefab;
            public int initialPoolSize = 10;
            public bool canExpand = true;
        }

        [Tooltip("Define the objects to be pooled with their initial quantities")]
        public List<PooledObjectInfo> pooledObjects = new List<PooledObjectInfo>();

        // Dictionary to store inactive pool objects by prefab
        private Dictionary<GameObject, List<GameObject>> inactivePoolDictionary = new Dictionary<GameObject, List<GameObject>>();

        private void Awake()
        {
            InitializePools();
        }

       

        /// <summary>
        /// Initialize all the pools based on the configuration in `pooledObjects`.
        /// </summary>
        private void InitializePools()
        {
            foreach (PooledObjectInfo poolInfo in pooledObjects)
            {
                if (poolInfo.prefab != null)
                {
                    CreatePool(poolInfo.prefab, poolInfo.initialPoolSize);
                }
                else
                {
                    Debug.LogWarning("Null prefab in Object Pool configuration!");
                }
            }
        }

        /// <summary>
        /// Creates a new pool of game objects for a given prefab.
        /// </summary>
        /// <param name="prefab">The prefab to pool.</param>
        /// <param name="initialPoolSize">The initial number of instances to pool.</param>
        public void CreatePool(GameObject prefab, int initialPoolSize)
        {
            if (prefab == null)
            {
                Debug.LogError("Cannot create a pool with a null prefab!");
                return;
            }

            if (inactivePoolDictionary.ContainsKey(prefab))
            {
                Debug.LogWarning($"A pool for prefab {prefab.name} already exists!");
                return;
            }

            // Create a parent container for pool objects (for better hierarchy organization in the editor)
            GameObject poolContainer = new GameObject($"Pool_{prefab.name}");
            poolContainer.transform.SetParent(transform);

            // Initialize the inactive pool list for this prefab
            List<GameObject> objectPool = new List<GameObject>();

            // Create initial instances
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = CreateNewInstance(prefab, poolContainer.transform);
                objectPool.Add(obj);
            }

            // Store the pool in the dictionary
            inactivePoolDictionary[prefab] = objectPool;
        }

        /// <summary>
        /// Creates a new inactive instance of the prefab and adds it to the parent container.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="parent">The parent Transform to assign to the new object.</param>
        private GameObject CreateNewInstance(GameObject prefab, Transform parent)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(false);
            return obj;
        }

        /// <summary>
        /// Retrieves an object from the pool for the specified prefab.
        /// </summary>
        /// <param name="prefab">The prefab whose pooled instance is required.</param>
        /// <param name="position">The position to set for the object.</param>
        /// <param name="rotation">The rotation to set for the object.</param>
        /// <returns>An instance of the prefab from the pool.</returns>
        
        public GameObject GetPooledObject(string prefabName)
        {
            foreach (var poolInfo in pooledObjects)
            {
                if (poolInfo.prefab != null && poolInfo.prefab.name == prefabName)
                {
                    return GetPooledObject(poolInfo.prefab);
                }
            }

            Debug.LogWarning($"No pool found for prefab with name: {prefabName}");
            return null;
        }
        
        public GameObject GetPooledObject(GameObject prefab)
        {
            if (!inactivePoolDictionary.ContainsKey(prefab))
            {
                Debug.LogWarning($"Pool for prefab {prefab.name} doesn't exist! Creating a new pool with default size.");
                CreatePool(prefab, 10);
            }

            List<GameObject> inactiveObjects = inactivePoolDictionary[prefab];

            GameObject obj = null;
            
            // Check if we have any inactive objects available
            if (inactiveObjects.Count > 0)
            {
                // Get the first inactive object
                obj = inactiveObjects[0];
                // Remove it from the inactive list
                inactiveObjects.RemoveAt(0);
            }
            else
            {
                // If no inactive objects are available, check if we can expand the pool
                PooledObjectInfo poolInfo = pooledObjects.Find(info => info.prefab == prefab);
                bool canExpand = poolInfo != null ? poolInfo.canExpand : true;

                if (canExpand)
                {
                    // Create a new instance since none are available
                    Transform poolParent = inactivePoolDictionary[prefab].Count > 0 ? 
                        inactivePoolDictionary[prefab][0].transform.parent : transform;
                        
                    obj = CreateNewInstance(prefab, poolParent);
                }
                else
                {
                    // If the pool cannot be expanded, return null
                    Debug.LogWarning($"Could not get an object from the pool for {prefab.name} - pool at maximum capacity!");
                    return null;
                }
            }

            // Set the object properties
            obj.SetActive(true);
            
            return obj;
        }

        /// <summary>
        /// Returns an object to the pool by deactivating it.
        /// </summary>
        /// <param name="obj">The object to be returned to the pool.</param>
        public void ReturnToPool(GameObject obj)
        {
            if (obj == null)
                return;
            
            // Deactivate the object
            obj.SetActive(false);

            // Find which prefab this object belongs to
            foreach (var prefab in inactivePoolDictionary.Keys)
            {
                // Check if this object is an instance of this prefab
                if (obj.name.Contains(prefab.name))
                {
                    // Add back to the inactive list for this prefab
                    inactivePoolDictionary[prefab].Add(obj);
                    return;
                }
            }
            
            // If we reach here, the object didn't match any known prefab
            Debug.LogWarning($"Couldn't find matching prefab for returned object: {obj.name}");
        }
    }
}
