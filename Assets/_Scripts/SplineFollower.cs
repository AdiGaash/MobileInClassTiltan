using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    public SplineContainer splineContainer;
    public bool followRotation = true;
    public bool followForward = true;
    public float startPosition = 0f;
    
    private float duration;
    private float elapsedTime;
    private bool isInitialized;

    public void Initialize(float clipDuration)
    {
        duration = clipDuration;
        elapsedTime = 0f;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || splineContainer == null || splineContainer.Spline == null) return;

        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);

        // Update position along spline
        Vector3 position = splineContainer.EvaluatePosition(progress);
        transform.position = position;

        if (followRotation || followForward)
        {
            Vector3 tangent = splineContainer.EvaluateTangent(progress);
            if (tangent != Vector3.zero)
            {
                transform.forward = tangent.normalized;
            }
        }
    }
}
