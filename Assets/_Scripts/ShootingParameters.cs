using UnityEngine;

namespace Shooter
{
    [CreateAssetMenu(fileName = "ShootingParameters", menuName = "Shooter/Shooting Parameters")]
    public class ShootingParameters : ScriptableObject
    {
        [Header("Fire Settings")]
        [Range(0.2f, 5f)]
        public float fireRate = 0.5f;
        [Range(0.1f, 4f)]
        public float bulletDamage = 10f;
        [Range(5f, 30f)]
        public float bulletSpeed = 20f;
        [Range(1, 3)]
        public int numOfBulletsPerShot = 1;
        [Header("Bullet Properties")]
        public GameObject bulletPrefab;
        [Range(0.1f, 10f)]
        public float bulletLifetime = 3f;
        
    }
}