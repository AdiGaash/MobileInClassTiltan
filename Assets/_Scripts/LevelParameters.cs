using UnityEngine;

namespace Shooter
{
    [CreateAssetMenu(fileName = "LevelParameters", menuName = "LevelParametersSO", order = 0)]
    public class LevelParameters : ScriptableObject
    {
        public float PlayerSpeed = 3f;
        
    }
}