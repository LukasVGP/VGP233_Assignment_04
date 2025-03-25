using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string requiredKeyTag = "Key";
    [SerializeField] private int requiredItemCount = 1;
    [SerializeField] private GameObject visualIndicator;
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Material unlockedMaterial;

    private Renderer exitRenderer;

    private void Start()
    {
        exitRenderer = GetComponent<Renderer>();
        UpdateVisualState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool canExit = true;

            // Check if key is required
            if (requiresKey)
            {
                PlayerInventory inventory = other.GetComponent<PlayerInventory>();
                if (inventory == null || !inventory.HasItem(requiredKeyTag))
                {
                    Debug.Log("You need a key to exit!");
                    canExit = false;
                }
            }

            // Check if specific number of items is required
            if (requiredItemCount > 0 && GameManager.Instance != null)
            {
                if (GameManager.Instance.GetCollectedItems() < requiredItemCount)
                {
                    Debug.Log($"You need to collect {requiredItemCount} items to exit!");
                    canExit = false;
                }
            }

            // Check if player has lives remaining
            if (canExit && GameManager.Instance != null && GameManager.Instance.GetCurrentLives() > 0)
            {
                GameManager.Instance.Victory();
            }
        }
    }

    private void UpdateVisualState()
    {
        if (exitRenderer != null && visualIndicator != null)
        {
            bool isUnlocked = true;

            // Check if key requirement is met
            if (requiresKey && GameManager.Instance != null)
            {
                PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
                isUnlocked = (inventory != null && inventory.HasItem(requiredKeyTag));
            }

            // Check if item count requirement is met
            if (requiredItemCount > 0 && GameManager.Instance != null)
            {
                isUnlocked = isUnlocked && (GameManager.Instance.GetCollectedItems() >= requiredItemCount);
            }

            // Update visual state
            if (isUnlocked && unlockedMaterial != null)
            {
                exitRenderer.material = unlockedMaterial;
            }
            else if (!isUnlocked && lockedMaterial != null)
            {
                exitRenderer.material = lockedMaterial;
            }
        }
    }

    private void Update()
    {
        // Update visual state every frame to reflect current game state
        UpdateVisualState();
    }
}
