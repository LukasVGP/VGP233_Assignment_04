using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private HashSet<string> collectedItems = new HashSet<string>();

    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float pickupSoundVolume = 0.5f;

    public bool HasItem(string itemTag)
    {
        return collectedItems.Contains(itemTag);
    }

    public void AddItem(string itemTag)
    {
        collectedItems.Add(itemTag);
        Debug.Log($"Collected: {itemTag}");

        // Play pickup sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupSoundVolume);
        }
    }

    public int GetItemCount()
    {
        return collectedItems.Count;
    }

    public void ClearInventory()
    {
        collectedItems.Clear();
    }
}
