using UnityEngine;
using DG.Tweening; // Needed for DOTween animations

namespace Shooter
{
    /// <summary>
    /// Alternative Enemy behavior demonstrating movement using Lerp, Slerp, and DOTween.
    /// This class is meant for teaching purposes to show different animation-by-code methods.
    /// </summary>
    public class EnemyTween0 : MonoBehaviour
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
            
        }

        /// <summary>
        /// Smooth rotation and forward movement using Quaternion.Slerp.
        /// </summary>
        private void MoveWithSlerp()
        {
           
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
            
        }
    }
}
