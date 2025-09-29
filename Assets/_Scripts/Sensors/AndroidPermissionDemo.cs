using UnityEngine;
using UnityEngine.Android; // Needed for Permission API


namespace Shooter.Sensors
{

    public class AndroidPermissionDemo : MonoBehaviour
    {
        void Start()
        {
            // Example: Ask for "Fine Location" permission (needed for GPS)
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                // Request permission at runtime
                Permission.RequestUserPermission(Permission.FineLocation);
            }

            // You can check again later if permission was granted
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.Log("Fine Location permission granted!");
            }
            else
            {
                Debug.Log("Fine Location permission NOT granted.");
            }
        }
    }
}