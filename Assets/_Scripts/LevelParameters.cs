using UnityEngine;

namespace Shooter
{
    [CreateAssetMenu(fileName = "LevelParameters", menuName = "LevelParametersSO", order = 0)]
    public class LevelParameters : ScriptableObject
    {
        public float GameSpeed = 3f;
        public AudioClip backgroundMusic;
        
    }
}