using UnityEngine;

namespace Shooter.Sensors
{
    public class GyroscopeDemo : MonoBehaviour
    {
        void Start()
        {
            // Enable gyroscope if supported
            if (SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = true;
            }
            else
            {
                Debug.Log("Gyroscope not supported on this device.");
            }
        }

        void Update()
        {
            if (Input.gyro.enabled)
            {
                // Get device rotation
                Quaternion rotation = Input.gyro.attitude;
                Debug.Log("Gyroscope rotation: " + rotation.eulerAngles);
            }
        }
    }
}