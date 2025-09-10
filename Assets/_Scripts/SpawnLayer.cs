using UnityEngine;

namespace Shooter
{
    [System.Serializable]
    public class SpawnLayer
    {
        // The Idea of having layers is to be able to have different spawn areas for different objects That might cover one another
        // like background elements and enemies and such like.
        // And as they have different distance from the camera when the camera is "perspective" we need to calculate them seperately 
        
        
        public string name = "Default";
        public float distanceToCamera = 10f;
        public float padding = 2f;
        // cached values
        [HideInInspector] public Vector2 minBounds;
        [HideInInspector] public Vector2 maxBounds;
        [HideInInspector] public float visualizationDistance; // Used for visualization in perspective mode
        public Color debugColor = Color.white;

    }
}