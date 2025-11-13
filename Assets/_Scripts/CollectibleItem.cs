using System;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // Static event — all collectibles share this event
    public static event Action<CollectibleItem> OnCollected;

    // Optional: a name or ID for this item
    public string itemID;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collided
        if (other.CompareTag("Player"))
        {
            // Raise the event
            OnCollected?.Invoke(this);

            // Destroy the collectible (simulate being collected)
            Destroy(gameObject);
        }
    }
}