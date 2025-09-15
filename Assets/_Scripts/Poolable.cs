using UnityEngine;

namespace Shooter
{
   
    public class Poolable: MonoBehaviour
    {
      
        public float MoveSpeed;
        public Vector3 MoveDirection;
        void Update()
        {
            transform.Translate(MoveDirection * MoveSpeed * Time.deltaTime);
        }
    }
}