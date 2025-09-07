using System;
using UnityEngine;

namespace Shooter
{
    public class BackgorundManager : MonoBehaviour
    {
        public LevelManager levelManager;
        public float CloudSpeedModifier = 1f;
        public float LandSpeedModifier = 1f;
        public float outOfScreenBoundX = 10f;
        public float outOfScreenBoundZ = 10f;
        public CameraBoundsController cameraBoundsController;
        [HideInInspector]
        public Rect activeAreaBounds;
        [HideInInspector]
        public Rect spawnAreaBounds;
       
        

        private void OnEnable()
        {
            cameraBoundsController.OnBoundariesChanged += UpdatePoolArea;
        }
        private void OnDisable()
        {
            cameraBoundsController.OnBoundariesChanged -= UpdatePoolArea;
        }

        void UpdatePoolArea(Rect newBoundaries)
        {
            activeAreaBounds = cameraBoundsController.CalculateBoundaries(outOfScreenBoundX,outOfScreenBoundZ);
            spawnAreaBounds = new Rect(newBoundaries.xMin - outOfScreenBoundX, newBoundaries.height,
                newBoundaries.width + outOfScreenBoundX, newBoundaries.height + outOfScreenBoundZ);
        }

        private void Start()
        {
            Invoke("SpawnCloud",CloudSpeedModifier+levelManager.GameSpeed);
            Invoke("SpawnOnLand",LandSpeedModifier+levelManager.GameSpeed);
        }

        private void Update()
        {
            SpawnCloud();
            SpawnOnLand();
            
        }

        private void SpawnOnLand()
        {
            
        }

        private void SpawnCloud()
        {
            
        }
    }
}