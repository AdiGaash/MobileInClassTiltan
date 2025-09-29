using UnityEngine;

namespace Shooter
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "EnemyType", order = 0)]
    public class EnemyType : ScriptableObject
    {
        public float Speed;
        public float Health;
        public float Damage;
        public GameObject Prefab;
        public Weapon weapon;
    }
}