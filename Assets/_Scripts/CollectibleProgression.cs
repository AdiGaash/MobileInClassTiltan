using UnityEngine;

namespace Shooter
{
    

    public class CollectibleProgression : MonoBehaviour
    {
        private int totalCollected = 0;

        private void OnEnable()
        {
            // Subscribe to the collectible event
            CollectibleItem.OnCollected += HandleCollectibleCollected;
        }

        private void OnDisable()
        {
            // Unsubscribe when disabled to avoid memory leaks
            CollectibleItem.OnCollected -= HandleCollectibleCollected;
        }

        private void HandleCollectibleCollected(CollectibleItem item)
        {
            totalCollected++;

            // Print or update UI
            Debug.Log($"Collected: {item.itemID} | Total: {totalCollected}");

            // You could trigger UI updates, save data, etc. here
        }
    }

    
}