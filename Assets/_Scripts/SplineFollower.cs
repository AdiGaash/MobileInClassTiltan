using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace Shooter
{
    // this is just a refrence for possible spline movement implementation that can be use possibly with the playable clip that spawn the enemies...
    public class SplineFollower : MonoBehaviour
    {
        [Header("Spline Settings")]
        [Tooltip("Reference to the SplineContainer in the scene")]
        public SplineContainer splinePath;
        
        [Tooltip("Movement speed along the spline")]
        public float speed = 5f;
        
        [Tooltip("Should the object loop through the spline")]
        public bool loop = false;

        private float currentDistance = 0f;
        private float splineLength;

        private void Start()
        {
            if (splinePath == null)
            {
                Debug.LogError("SplineFollower: No spline assigned!");
                enabled = false;
                return;
            }

            splineLength = splinePath.CalculateLength();
        }

        private void Update()
        {
            // Update distance based on speed
            currentDistance += speed * Time.deltaTime;

            // Loop or clamp the distance
            if (loop)
            {
                currentDistance %= splineLength;
            }
            else if (currentDistance >= splineLength)
            {
                currentDistance = splineLength;
                enabled = false;
                return;
            }

            // Get normalized position (0 to 1) along the spline
            float normalizedDistance = currentDistance / splineLength;

            // Get position and tangent at the current point
            float3 position = splinePath.EvaluatePosition(normalizedDistance);
            float3 tangent = splinePath.EvaluateTangent(normalizedDistance);

            // Update position and rotation
            transform.position = position;
            if (tangent.x != 0 || tangent.y != 0 || tangent.z != 0)
            {
                transform.rotation = Quaternion.LookRotation(tangent);
            }
        }
    }
}