using UnityEngine;

namespace Shooter
{
   
    public class AutonomousMove: MonoBehaviour
    {
      
        public float MoveSpeed;
        public Vector3 MoveDirection;
        void Update()
        {
            transform.Translate(MoveDirection * MoveSpeed * Time.deltaTime);
        }
    }
}