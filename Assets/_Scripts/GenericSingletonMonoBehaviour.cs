using UnityEngine;

namespace Shooter
{
    /// <summary>
    /// A generic Singleton implementation that can be inherited by MonoBehaviour classes.
    /// Provides centralized control over instance persistence between scene loads.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Singleton instance
        private static T _instance;
        
        // Flag to determine if this singleton should persist between scenes
        [Tooltip("If true, this singleton will not be destroyed when loading a new scene")]
        [SerializeField] protected bool dontDestroyOnLoad = false;
        
        /// <summary>
        /// Global access point to the singleton instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                    }
                    
                    // Call the initialization method if it's the first time creating the instance
                    Singleton<T> singleton = _instance as Singleton<T>;
                    if (singleton != null && singleton.dontDestroyOnLoad)
                    {
                        DontDestroyOnLoad(singleton.gameObject);
                    }
                }
                
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // Ensure we have only one instance of this class
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this as T;
            
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
