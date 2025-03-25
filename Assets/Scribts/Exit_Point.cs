using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private int requiredItemCount = 1;
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
            if (GameManager.Instance.GetCollectedItems() >= requiredItemCount)
            {
                GameManager.Instance.Victory();
            }
            else
            {
                Debug.Log($"You need to collect {requiredItemCount} items to exit!");
            }
        }
    }

    private void UpdateVisualState()
    {
        if (exitRenderer != null)
        {
            bool isUnlocked = GameManager.Instance.GetCollectedItems() >= requiredItemCount;

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
        UpdateVisualState();
    }
}
