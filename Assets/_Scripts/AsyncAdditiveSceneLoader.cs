using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Shooter
{
/// <summary>
/// Demonstrates how to load and unload scenes asynchronously and additively in Unity.
/// Attach this script to an empty GameObject in your main scene.
/// Make sure the scenes you want to load are added to Build Settings.
/// </summary>
    public class AsyncAdditiveSceneLoader : MonoBehaviour
    {
        [Header("Scene Names (Must Match Build Settings)")]
        public string sceneToLoad = "EnvironmentScene";
        public string sceneToUnload = "OldScene";

        private AsyncOperation asyncLoadOperation;

        private void Start()
        {
            // Optionally start loading immediately at scene start
            // StartCoroutine(LoadSceneAdditiveAsync());
        }

        private void Update()
        {
            // Example input keys for demonstration
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(LoadSceneAdditiveAsync());
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                StartCoroutine(UnloadSceneAsync());
            }
        }

        /// <summary>
        /// Loads another scene asynchronously and additively,
        /// meaning it will load alongside the current scene.
        /// </summary>
        private IEnumerator LoadSceneAdditiveAsync()
        {
            if (SceneManager.GetSceneByName(sceneToLoad).isLoaded)
            {
                Debug.LogWarning($"Scene '{sceneToLoad}' is already loaded.");
                yield break;
            }

            Debug.Log($"Starting to load scene '{sceneToLoad}' additively...");
            asyncLoadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            asyncLoadOperation.allowSceneActivation = true; // allow activation once done

            // While still loading, report progress
            while (!asyncLoadOperation.isDone)
            {
                Debug.Log($"Loading progress: {asyncLoadOperation.progress * 100f:0}%");
                yield return null;
            }

            Debug.Log($"Scene '{sceneToLoad}' successfully loaded!");
        }

        /// <summary>
        /// Unloads a scene asynchronously.
        /// </summary>
        private IEnumerator UnloadSceneAsync()
        {
            if (!SceneManager.GetSceneByName(sceneToUnload).isLoaded)
            {
                Debug.LogWarning($"Scene '{sceneToUnload}' is not loaded, cannot unload.");
                yield break;
            }

            Debug.Log($"Unloading scene '{sceneToUnload}'...");
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneToUnload);

            while (!asyncUnload.isDone)
            {
                Debug.Log($"Unloading progress: {asyncUnload.progress * 100f:0}%");
                yield return null;
            }

            Debug.Log($"Scene '{sceneToUnload}' successfully unloaded!");
        }
    }
}