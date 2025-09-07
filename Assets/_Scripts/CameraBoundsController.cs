using UnityEngine;
using System;

namespace Shooter
{
    public class CameraBoundsController : MonoBehaviour
    {
        // Event that will be raised when camera boundaries change
        public event Action<Rect> OnBoundariesChanged;
        
        // Cache the camera transform and properties to check for changes
        private Transform cameraTransform;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private Camera mainCamera;
        private float lastFieldOfView;
        private float lastOrthographicSize;
        
        // Height at which to calculate the boundaries (typically player's y position)
        [Tooltip("The Y height at which to calculate camera boundaries")]
        public float boundaryCalculationHeight = 0f;
        
        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            cameraTransform = transform;
            lastPosition = cameraTransform.position;
            lastRotation = cameraTransform.rotation;
            lastFieldOfView = mainCamera.fieldOfView;
            lastOrthographicSize = mainCamera.orthographicSize;
        }
        
        private void LateUpdate()
        {
            // Check if camera has moved, rotated, or if FOV/orthographic size has changed
            bool hasPositionChanged = lastPosition != cameraTransform.position;
            bool hasRotationChanged = lastRotation != cameraTransform.rotation;
            bool hasFOVChanged = !mainCamera.orthographic && lastFieldOfView != mainCamera.fieldOfView;
            bool hasOrthoSizeChanged = mainCamera.orthographic && lastOrthographicSize != mainCamera.orthographicSize;
            
            if (hasPositionChanged || hasRotationChanged || hasFOVChanged || hasOrthoSizeChanged)
            {
                // Recalculate boundaries and notify listeners
                Rect newBoundaries = CalculateBoundaries();
                
                // Update cached values
                lastPosition = cameraTransform.position;
                lastRotation = cameraTransform.rotation;
                lastFieldOfView = mainCamera.fieldOfView;
                lastOrthographicSize = mainCamera.orthographicSize;
            }
        }
        
        // Calculate general boundaries with optional object extents
        public Rect CalculateBoundaries(float objectExtentX = 0f, float objectExtentZ = 0f)
        {
            float minX, maxX, minZ, maxZ;
            
            // Find the camera boundaries in world space
            
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
            
            // Create the boundaries rect
            Rect boundaries = new Rect(minX, minZ, maxX - minX, maxZ - minZ);
            
            Debug.Log($"Camera boundaries calculated: X({minX} to {maxX}), Z({minZ} to {maxZ})");
            
            // Notify listeners that boundaries have changed
            OnBoundariesChanged?.Invoke(boundaries);
            
            return boundaries;
        }
        
        // Calculate boundaries for a specific object - now uses the general method
        public Rect CalculateBoundariesForObject(GameObject targetObject)
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
            Rect boundaries = CalculateBoundaries(objectExtentX, objectExtentZ);
            
            Debug.Log($"Camera boundaries calculated for {targetObject.name}: X({boundaries.xMin} to {boundaries.xMax}), Z({boundaries.yMin} to {boundaries.yMax})");
            
            return boundaries;
        }
        
        // Get the current boundaries (calculated fresh each time)
        public Rect GetBoundariesRect()
        {
            return CalculateBoundaries();
        }
    }
}