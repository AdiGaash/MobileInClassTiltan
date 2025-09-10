using UnityEngine;

namespace Shooter
{
    /// <summary>
    /// Utility class for drawing debug gizmos in the Unity Editor.
    /// </summary>
    public static class GizmoUtils
    {
        /// <summary>
        /// Draws a rectangle using Gizmos.
        /// </summary>
        /// <param name="min">The minimum point (bottom-left corner) of the rectangle.</param>
        /// <param name="max">The maximum point (top-right corner) of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawRectangle(Vector3 min, Vector3 max, Color color)
        {
            // Save the current Gizmos color
            Color previousColor = Gizmos.color;
            
            // Set the color for this rectangle
            Gizmos.color = color;
            
            // Create the four corners of the rectangle
            Vector3 bottomLeft = min;
            Vector3 bottomRight = new Vector3(max.x, min.y, min.z);
            Vector3 topLeft = new Vector3(min.x, min.y, max.z);
            Vector3 topRight = new Vector3(max.x, min.y, max.z);
            
            // Draw the four lines of the rectangle
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
            
            // Restore the previous Gizmos color
            Gizmos.color = previousColor;
        }
        
        /// <summary>
        /// Draws a solid rectangle using Gizmos.
        /// </summary>
        /// <param name="min">The minimum point (bottom-left corner) of the rectangle.</param>
        /// <param name="max">The maximum point (top-right corner) of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawSolidRectangle(Vector3 min, Vector3 max, Color color)
        {
            // Save the current Gizmos color
            Color previousColor = Gizmos.color;
            
            // Set the color for this rectangle
            Gizmos.color = color;
            
            // Calculate the center and size of the cube
            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;
            
            // Draw a solid cube
            Gizmos.DrawCube(center, size);
            
            // Restore the previous Gizmos color
            Gizmos.color = previousColor;
        }
        
        /// <summary>
        /// Draws a wire rectangle using Gizmos.
        /// </summary>
        /// <param name="min">The minimum point (bottom-left corner) of the rectangle.</param>
        /// <param name="max">The maximum point (top-right corner) of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawWireRectangle(Vector3 min, Vector3 max, Color color)
        {
            // Save the current Gizmos color
            Color previousColor = Gizmos.color;
            
            // Set the color for this rectangle
            Gizmos.color = color;
            
            // Calculate the center and size of the cube
            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;
            
            // Draw a wire cube
            Gizmos.DrawWireCube(center, size);
            
            // Restore the previous Gizmos color
            Gizmos.color = previousColor;
        }
    }
}
