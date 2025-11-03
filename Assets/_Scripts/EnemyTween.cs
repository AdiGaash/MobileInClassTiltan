using UnityEngine;
using DG.Tweening; // Needed for DOTween animations

namespace Shooter
{
    /// <summary>
    /// Alternative Enemy behavior demonstrating movement using Lerp, Slerp, and DOTween.
    /// This class is meant for teaching purposes to show different animation-by-code methods.
    /// </summary>
    public class EnemyTween : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Target player Transform (auto-assigned if not set)")]
        public Transform playerTransform;

        [Header("Enemy Settings")]
        public float moveSpeed = 5f;

        [Header("Behavior Mode")]
        [Tooltip("Choose movement mode for demonstration")]
        public MovementMode movementMode = MovementMode.DOTween;

        private bool canMove = false;

        // Enumeration for demo modes
        public enum MovementMode
        {
            Lerp,
            Slerp,
            DOTween
        }

        private void Awake()
        {
            // Find the player in the scene by tag
            if (playerTransform == null)
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (playerTransform == null)
                Debug.LogWarning("EnemyTween: Player not found! Please tag the player 'Player'.");
        }

        private void FixedUpdate()
        {
            if (!canMove || playerTransform == null)
                return;

            switch (movementMode)
            {
                case MovementMode.Lerp:
                    MoveWithLerp();
                    break;
                case MovementMode.Slerp:
                    MoveWithSlerp();
                    break;
                case MovementMode.DOTween:
                    // DOTween mode handles movement automatically — no FixedUpdate logic needed
                    break;
            }
        }

        /// <summary>
        /// Smooth movement using Vector3.Lerp (linear interpolation).
        /// </summary>
        private void MoveWithLerp()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                playerTransform.position,
                Time.deltaTime * 1.5f // interpolation factor for smoothness
            );
        }

        /// <summary>
        /// Smooth rotation and forward movement using Quaternion.Slerp.
        /// </summary>
        private void MoveWithSlerp()
        {
            // Calculate target rotation to face player
            Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - transform.position);

            // Smoothly rotate toward player
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 2f
            );

            // Move forward after turning
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
        }

        /// <summary>
        /// Initializes attack (movement start) depending on chosen mode.
        /// </summary>
        public void InitAttack()
        {
            if (playerTransform == null)
            {
                Debug.LogWarning("EnemyTween: Player Transform not assigned!");
                return;
            }

            switch (movementMode)
            {
                case MovementMode.Lerp:
                case MovementMode.Slerp:
                    // Continuous movement handled in FixedUpdate
                    canMove = true;
                    break;

                case MovementMode.DOTween:
                    StartDOTweenAttack();
                    break;
            }
        }

        /// <summary>
        /// Demonstrates DOTween-based smooth movement and rotation toward the player.
        /// </summary>
        private void StartDOTweenAttack()
        {
            // Stop any previous tweens on this object
            transform.DOKill();

            // Rotate smoothly to face player over 0.5 seconds
            transform.DOLookAt(playerTransform.position, 0.5f)
                     .SetEase(Ease.OutSine);

            // Move toward player over 2 seconds with a smooth ease
            transform.DOMove(playerTransform.position, 2f)
                     .SetEase(Ease.InOutQuad)
                     .OnComplete(() =>
                     {
                         Debug.Log("EnemyTween: Reached the player!");
                         canMove = false;
                     });

            // No need for continuous movement during tween
            canMove = false;
        }
    }
}
