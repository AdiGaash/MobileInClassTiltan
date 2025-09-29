using UnityEngine;

namespace Shooter.Sensors
{
    public class CompassDemo : MonoBehaviour
    {
        void Start()
        {
            // Enable compass if supported
            if (SystemInfo.supportsGyroscope) // magnetometer is usually tied to gyro support
            {
                Input.compass.enabled = true;
            }
            else
            {
                Debug.Log("Compass not supported on this device.");
            }
        }

        void Update()
        {
            if (Input.compass.enabled)
            {
                // Heading relative to magnetic north
                float heading = Input.compass.trueHeading;
                Debug.Log("Compass heading: " + heading);
            }
        }
    }
}