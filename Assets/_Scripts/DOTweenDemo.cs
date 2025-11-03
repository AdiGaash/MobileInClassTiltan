using UnityEngine;
using DG.Tweening;

/// <summary>
/// Demonstrates DOTween features: Sequence and OnComplete.
/// Attach this script to a GameObject in the scene (cube, sphere, or enemy prefab).
/// </summary>
public class DOTweenDemo : MonoBehaviour
{
    [Header("Demo Settings")]
    public Transform target;       // Optional target position for movement
    public float moveDuration = 1f;
    public float scaleDuration = 0.5f;
    public float rotateDuration = 1f;

    private void Start()
    {
        // If no target is assigned, move relative to current position
        Vector3 moveTarget = target != null ? target.position : transform.position + Vector3.forward * 3;

        // Create a DOTween Sequence
        Sequence demoSequence = DOTween.Sequence();

        // Step 1: Move the object forward
        demoSequence.Append(transform.DOMove(moveTarget, moveDuration)
            .SetEase(Ease.InOutQuad));

        // Step 2: Scale up the object (happens after move completes)
        demoSequence.Append(transform.DOScale(Vector3.one * 1.5f, scaleDuration)
            .SetEase(Ease.OutBounce));

        // Step 3: Rotate 360 degrees (happens after scale)
        demoSequence.Append(transform.DORotate(Vector3.up * 360, rotateDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutSine));

        // Step 4: Add OnComplete callback (called after the entire sequence finishes)
        demoSequence.OnComplete(() =>
        {
            Debug.Log("DOTweenDemo: Sequence completed!");
            // Example: reset scale to original
            transform.localScale = Vector3.one;
        });

        // Optional: Make the sequence loop 2 times
        demoSequence.SetLoops(2, LoopType.Restart);
    }
}