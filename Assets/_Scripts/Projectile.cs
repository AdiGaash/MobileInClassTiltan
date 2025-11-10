using UnityEngine;

namespace Shooter
{
    public class Projectile : MonoBehaviour
    {
        private float damage;
        private float speed;
        private float lifetime;
        private float spawnTime;
        private ObjectPoolManager objectPoolManager;

        

        private void OnEnable()
        {
            spawnTime = Time.time;
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (Time.time - spawnTime >= lifetime)
            {
                ReturnToPool();
            }
        }

        public void Initialize(float damage, float speed, float lifetime, ObjectPoolManager objectPoolManager)
        {
            this.damage = damage;
            this.speed = speed;
            this.lifetime = lifetime;
            this.objectPoolManager = objectPoolManager;
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (objectPoolManager != null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public interface IDamageable
    {
        void TakeDamage(float damage);
    }
}