using UnityEngine;

namespace Shooter
{
    [CreateAssetMenu(fileName = "ShootingParameters", menuName = "Shooter/Shooting Parameters")]
    public class ShootingParameters : ScriptableObject
    {
        [Header("Fire Settings")]
        public float fireRate = 0.5f;
        public float bulletDamage = 10f;
        public float bulletSpeed = 20f;
        public int numOfBulletsPerShot = 1;
        [Header("Bullet Properties")]
        public GameObject bulletPrefab;
        public float bulletLifetime = 3f;
        
    }
}