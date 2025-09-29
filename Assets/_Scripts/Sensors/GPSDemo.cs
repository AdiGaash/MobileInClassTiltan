using UnityEngine;
using System.Collections;


namespace Shooter.Sensors
{

    public class GPSDemo : MonoBehaviour
    {
        IEnumerator Start()
        {
            // Check if user has location service enabled
            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("Location service not enabled by user.");
                yield break;
            }

            // Start location service
            Input.location.Start();

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service failed
            if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to determine device location.");
                yield break;
            }
            else
            {
                // Success: get location
                Debug.Log("GPS Location: " +
                          Input.location.lastData.latitude + ", " +
                          Input.location.lastData.longitude);
            }
        }
    }
}