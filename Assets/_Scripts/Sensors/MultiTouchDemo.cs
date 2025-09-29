using UnityEngine;

namespace Shooter.Sensors
{
    public class MultiTouchDemo : MonoBehaviour
    {
        void Update()
        {
            // Get how many fingers are currently on the screen
            int touchCount = Input.touchCount;

            if (touchCount > 0)
            {
                Debug.Log("Number of touches: " + touchCount);

                // Loop through all active touches
                foreach (Touch t in Input.touches)
                {
                    // Log useful touch data
                    Debug.Log("Finger ID: " + t.fingerId +
                              " | Position: " + t.position +
                              " | Phase: " + t.phase);
                }
            }
        }
    }
}