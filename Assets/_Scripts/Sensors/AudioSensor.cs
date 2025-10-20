using UnityEngine;

namespace Shooter.Sensors
{
    public class AudioSensor : MonoBehaviour
    {
        private AudioClip microphoneInput;
        
        void Start()
        {
            // Start capturing audio from the microphone
            microphoneInput = Microphone.Start(null, true, 10, 44100);
        }
    }
}