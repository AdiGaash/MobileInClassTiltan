using UnityEngine;

namespace Shooter.Sensors
{
    public class AccelerometerDemo : MonoBehaviour
    {
        void Update()
        {
            // Read the accelerometer values (x, y, z)
            Vector3 accel = Input.acceleration;

            // Log the raw accelerometer data
            Debug.Log("Accelerometer: X=" + accel.x + 
                      " Y=" + accel.y + 
                      " Z=" + accel.z);
        }
    }
}