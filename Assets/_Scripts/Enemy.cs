using System;
using UnityEngine;

namespace Shooter
{
    public class Enemy : MonoBehaviour
    {
        public Transform playerTransform;
        public int health;
        public float MoveSpeed = 5;
        public bool canMoveTowardsPlayer = false;
        private void Awake()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            health = 10;
        }

        private void FixedUpdate()
        {
            if (canMoveTowardsPlayer)
                transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
            
        }

        public void InitAttack()
        {
            transform.LookAt(playerTransform);
            canMoveTowardsPlayer = true;
        }
    }
}