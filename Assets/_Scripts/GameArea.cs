using UnityEngine;
using UnityEngine.Serialization;

namespace Shooter
{
    public class GameArea : MonoBehaviour
    {
        public Camera camera;
        public SpawnLayer[] layers;
        public bool showDebugBounds = true; // Option to toggle debug visualization

        void Start()
        {
            if (camera == null)
                camera = Camera.main;

            RecalculateAllLayers();
        }

        /// <summary>
        /// Recalculate bounds for all layers (call when camera size/zoom changes).
        /// </summary>
        public void RecalculateAllLayers()
        {
            foreach (var layer in layers)
            {
                RecalculateLayerBounds(layer);
            }
        }

        void RecalculateLayerBounds(SpawnLayer layer)
        {
            if (camera.orthographic)
            {
                float height = camera.orthographicSize * 2f;
                float width = height * camera.aspect;
                Vector3 camPos = camera.transform.position;

                // Store bounds in X and Z axes instead of X and Y for top-down view
                layer.minBounds = new Vector2(camPos.x - width / 2, camPos.z - height / 2);
                layer.maxBounds = new Vector2(camPos.x + width / 2, camPos.z + height / 2);
            }
            else
            {
                Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, layer.distanceToCamera));
                Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, layer.distanceToCamera));

                // Store bounds in X and Z axes instead of X and Y for top-down view
                layer.minBounds = new Vector2(bottomLeft.x, bottomLeft.z);
                layer.maxBounds = new Vector2(topRight.x, topRight.z);
            }
        }

        /// <summary>
        /// Spawn only from the top edge.
        /// </summary>
        public Vector3 GetSpawnPositionFromTop(SpawnLayer layer)
        {
           
            float x = Random.Range(layer.minBounds.x, layer.maxBounds.x);
            // Use Z instead of Y for the top edge in a top-down view
            float z = layer.maxBounds.y + layer.padding;

            // Y coordinate should be consistent with the camera's view direction
            float y = camera.orthographic ? 0f : -layer.distanceToCamera;
            return new Vector3(x, y, z);
        }

        public SpawnLayer GetSpawnLayerByName(string layerName)
        {
            foreach (var layer in layers)
            {
                if (layer.name == layerName)
                {
                    return layer;
                }
            }
            Debug.LogWarning($"Spawn layer '{layerName}' not found in GameArea!");
            return null;
        }
        
        /// <summary>
        /// Check if position is outside cached bounds.
        /// </summary>
        public bool IsOutOfBounds(Vector3 pos, SpawnLayer layer)
        {
            return (pos.x < layer.minBounds.x - layer.padding ||
                    pos.x > layer.maxBounds.x + layer.padding ||
                    pos.z < layer.minBounds.y - layer.padding ||
                    pos.z > layer.maxBounds.y + layer.padding);
        }

        private void OnDrawGizmos()
        {
            if (!showDebugBounds || layers == null || layers.Length == 0 || camera == null)
                return;

            // Ensure bounds are calculated
            if (Application.isPlaying == false)
            {
                if (camera == null)
                    camera = Camera.main;

                if (camera != null)
                    RecalculateAllLayers();
            }

            // Draw debug rectangles for each layer
            foreach (var layer in layers)
            {
                if (!camera.orthographic)
                {
                    // Calculate the correct position for the Gizmo based on camera's forward direction
                    // This ensures it's at the right distance from the camera
                    Vector3 cameraPosition = camera.transform.position;
                    Vector3 cameraForward = camera.transform.forward;
                    
                    // Position at the specified distance from the camera along its forward direction
                    Vector3 planePosition = cameraPosition + cameraForward * layer.distanceToCamera;
                    
                    // Use the calculated y-coordinate for proper height
                    float y = planePosition.y;
                    
                    // Get viewport boundaries at this distance
                    Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, layer.distanceToCamera));
                    Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, layer.distanceToCamera));
                    
                    // Draw the main bounds (using X and Z for the rectangle)
                    GizmoUtils.DrawRectangle(
                        new Vector3(bottomLeft.x, y, bottomLeft.z),
                        new Vector3(topRight.x, y, topRight.z),
                        layer.debugColor);

                    // Draw the padded bounds with transparency
                    Color transparentColor = new Color(layer.debugColor.r, layer.debugColor.g, layer.debugColor.b, 0.3f);
                    
                    // Calculate proper world-space padding at this distance
                    float worldSpacePadding = layer.padding;
                    
                    GizmoUtils.DrawRectangle(
                        new Vector3(bottomLeft.x - worldSpacePadding, y, bottomLeft.z - worldSpacePadding),
                        new Vector3(topRight.x + worldSpacePadding, y, topRight.z + worldSpacePadding),
                        transparentColor);
                }
                else
                {
                    // For orthographic camera, use a fixed y value (usually 0)
                    float y = 0f;
                    
                    // Draw the main bounds (using X and Z for the rectangle)
                    GizmoUtils.DrawRectangle(
                        new Vector3(layer.minBounds.x, y, layer.minBounds.y),
                        new Vector3(layer.maxBounds.x, y, layer.maxBounds.y),
                        layer.debugColor);

                    // Draw the padded bounds with transparency
                    Color transparentColor = new Color(layer.debugColor.r, layer.debugColor.g, layer.debugColor.b, 0.3f);
                    GizmoUtils.DrawRectangle(
                        new Vector3(layer.minBounds.x - layer.padding, y, layer.minBounds.y - layer.padding),
                        new Vector3(layer.maxBounds.x + layer.padding, y, layer.maxBounds.y + layer.padding),
                        transparentColor);
                }
            }
        }
    }
}