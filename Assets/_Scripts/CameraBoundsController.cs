using UnityEngine;

namespace Shooter
{
    public class CameraBoundsController : MonoBehaviour
    {
        // Camera boundaries in world space
        private float minX, maxX, minZ, maxZ;
        private bool boundariesCalculated = false;
        
        // Height at which to calculate the boundaries (typically player's y position)
        [Tooltip("The Y height at which to calculate camera boundaries (usually player's height)")]
        public float boundaryCalculationHeight = 0f;
        
        // Calculate general boundaries with optional object extents
        public void CalculateBoundaries(float objectExtentX = 0f, float objectExtentZ = 0f)
        {
            // Find the camera boundaries in world space
            Camera mainCamera = GetComponent<Camera>();
            
            // For orthographic camera, the calculation is different
            if (mainCamera.orthographic)
            {
                // In orthographic mode, we don't need to consider distance
                float orthoHeight = mainCamera.orthographicSize;
                float orthoWidth = orthoHeight * mainCamera.aspect;
                
                // Calculate world boundaries at camera position
                Vector3 cameraPos = mainCamera.transform.position;
                
                minX = cameraPos.x - orthoWidth + objectExtentX;
                maxX = cameraPos.x + orthoWidth - objectExtentX;
                minZ = cameraPos.z - orthoHeight + objectExtentZ;
                maxZ = cameraPos.z + orthoHeight - objectExtentZ;
            }
            else
            {
                // Original perspective camera calculation
                float distanceToCamera = Mathf.Abs(boundaryCalculationHeight - mainCamera.transform.position.y);
                
                // Calculate viewport boundaries at the specified height
                Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera));
                Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera));
                
                // Set the boundaries, accounting for object size
                minX = bottomLeft.x + objectExtentX;
                maxX = topRight.x - objectExtentX;
                minZ = bottomLeft.z + objectExtentZ;
                maxZ = topRight.z - objectExtentZ;
            }
            
            boundariesCalculated = true;
            
            Debug.Log($"Camera boundaries calculated: X({minX} to {maxX}), Z({minZ} to {maxZ})");
        }
        
        // Calculate boundaries for a specific object - now uses the general method
        public void CalculateBoundariesForObject(GameObject targetObject)
        {
            // Get object size
            Bounds objectBounds;
            Renderer meshRenderer = targetObject.GetComponent<Renderer>();
            
            if (meshRenderer == null && targetObject.transform.childCount > 0)
                meshRenderer = targetObject.GetComponentInChildren<Renderer>();
                
            if (meshRenderer != null)
            {
                objectBounds = meshRenderer.bounds;
            }
            else
            {
                // Fallback if no renderer found
                Collider collider = targetObject.GetComponent<Collider>();
                if (collider != null)
                {
                    objectBounds = collider.bounds;
                }
                else
                {
                    // Default values if no renderer or collider found
                    objectBounds = new Bounds(targetObject.transform.position, new Vector3(1f, 1f, 1f));
                }
            }
            
            // Calculate object extents (half-size)
            float objectExtentX = objectBounds.extents.x;
            float objectExtentZ = objectBounds.extents.z;
            
            // Use the general method with object-specific extents
            CalculateBoundaries(objectExtentX, objectExtentZ);
            
            Debug.Log($"Camera boundaries calculated for {targetObject.name}: X({minX} to {maxX}), Z({minZ} to {maxZ})");
        }
        
        // Clamp a position to stay within boundaries
        public Vector3 ClampPositionToBoundaries(Vector3 position)
        {
            if (!boundariesCalculated)
            {
                Debug.LogWarning("Attempting to clamp position before boundaries are calculated!");
                return position;
            }
            
            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.z = Mathf.Clamp(position.z, minZ, maxZ);
            
            return position;
        }
        
        // Check if a position is within boundaries
        public bool IsPositionWithinBoundaries(Vector3 position)
        {
            if (!boundariesCalculated)
                return true; // Default to true if not calculated yet
                
            return position.x >= minX && position.x <= maxX && 
                   position.z >= minZ && position.z <= maxZ;
        }
        
        // Get the current boundaries
        public Rect GetBoundariesRect()
        {
            if (!boundariesCalculated)
                return new Rect(0, 0, 0, 0);
                
            return new Rect(minX, minZ, maxX - minX, maxZ - minZ);
        }
        
        // Returns the maximum distance the object can move in a given direction
        public float GetMaxDistanceInDirection(Vector3 position, Vector3 direction)
        {
            if (!boundariesCalculated)
                return Mathf.Infinity;
                
            direction.Normalize();
            
            // Initialize with a large value
            float maxDistance = Mathf.Infinity;
            
            // Check X boundaries
            if (Mathf.Abs(direction.x) > 0.0001f)
            {
                if (direction.x > 0)
                {
                    float distanceToX = (maxX - position.x) / direction.x;
                    maxDistance = Mathf.Min(maxDistance, distanceToX);
                }
                else
                {
                    float distanceToX = (minX - position.x) / direction.x;
                    maxDistance = Mathf.Min(maxDistance, distanceToX);
                }
            }
            
            // Check Z boundaries
            if (Mathf.Abs(direction.z) > 0.0001f)
            {
                if (direction.z > 0)
                {
                    float distanceToZ = (maxZ - position.z) / direction.z;
                    maxDistance = Mathf.Min(maxDistance, distanceToZ);
                }
                else
                {
                    float distanceToZ = (minZ - position.z) / direction.z;
                    maxDistance = Mathf.Min(maxDistance, distanceToZ);
                }
            }
            
            return maxDistance;
        }
    }
}