using UnityEngine;

namespace Shooter
{
    public class Cloud : Poolable
    {
        public float MoveSpeed;

        void Update()
        {
            transform.Translate(Vector3.back * MoveSpeed * Time.deltaTime);
        }
    }

    public abstract class Poolable: MonoBehaviour
    {
        public ObjectPoolManager ObjectPoolManager;
    }
}