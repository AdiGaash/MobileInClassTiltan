using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    SplineContainer splineContainer;
    float startPosition = 0f;

    private float duration;
    private float elapsedTime;
    private bool isInitialized;

    public void Initialize(float clipDuration, SplineContainer splineContainer)
    {
        this.splineContainer = splineContainer;
        startPosition = 0;
        duration = clipDuration;
        Debug.Log(clipDuration);
        elapsedTime = 0f;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || splineContainer == null || splineContainer.Spline == null) return;

        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);
        Debug.Log("progress: " + progress);
        // Update position along spline
        Vector3 position = splineContainer.EvaluatePosition(progress);
        transform.position = position;

        Vector3 tangent = splineContainer.EvaluateTangent(progress);
        if (tangent != Vector3.zero)
        {
            transform.forward = tangent.normalized;
        }

        if (progress >= 1f)
        {
            isInitialized = false;
        }
    }
}