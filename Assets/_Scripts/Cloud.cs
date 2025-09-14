using UnityEngine;

namespace Shooter
{
    public class Cloud : Poolable
    {
        void Update()
        {
            transform.Translate(MoveDirection * MoveSpeed * Time.deltaTime);
        }
    }

    public abstract class Poolable: MonoBehaviour
    {
      
        public float MoveSpeed;
        public Vector3 MoveDirection;
    }
}