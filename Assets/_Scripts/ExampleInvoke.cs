using UnityEngine;

namespace Shooter
{
    public class ExampleInvoke : MonoBehaviour
    {
        void Start()
        {
            // Dynamically call "HelloOnce" after 2 seconds
            CallFunctionByName("HelloOnce", 2f);

            // Start repeating "RepeatMessage" after 1 second, every 3 seconds
            StartRepeating("RepeatMessage", 1f, 3f);

            // Stop all invokes after 10 seconds
            Invoke("StopAllInvokes", 10f);
        }

        // -------------------------
        // 1. Call a function once dynamically
        void CallFunctionByName(string functionName, float delay)
        {
            Invoke(functionName, delay);
        }

        void HelloOnce()
        {
            Debug.Log("HelloOnce called after delay!");
        }

        // -------------------------
        // 2. Start repeating a function dynamically
        void StartRepeating(string functionName, float delay, float repeatRate)
        {
            InvokeRepeating(functionName, delay, repeatRate);
        }

        void RepeatMessage()
        {
            Debug.Log("RepeatMessage is being called repeatedly!");
        }

        // -------------------------
        // 3. Stop all invokes in this MonoBehaviour
        void StopAllInvokes()
        {
            CancelInvoke(); // Cancels all scheduled invokes in this script
            Debug.Log("All invokes stopped!");
        }
    }
    
}