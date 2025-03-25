using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemTag = "Key";
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float hoverSpeed = 1f;
    [SerializeField] private GameObject pickupEffect;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Rotate the item for visual effect
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Add hovering effect
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddItem(itemTag);

                // Notify GameManager about pickup
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddPickup();
                }

                // Spawn pickup effect if available
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                // Destroy the collectible
                Destroy(gameObject);
            }
        }
    }
}
