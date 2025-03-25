using UnityEngine;

public class WandererAI : BaseAI
{
    [Header("Wanderer AI Settings")]
    [SerializeField] private AILocationMap locationMap;
    [SerializeField] private string areaName; // The name of the area to wander in
    [SerializeField] private int areaIndex = 0; // Alternative to using area name
    [SerializeField] private bool useAreaName = true; // Whether to use name or index
    [SerializeField] private float minWanderTime = 3f;
    [SerializeField] private float maxWanderTime = 8f;

    private float wanderTimer;

    protected override void Awake()
    {
        base.Awake();

        // Find location map if not assigned
        if (locationMap == null)
        {
            locationMap = GetComponentInParent<AILocationMap>();
            if (locationMap == null)
            {
                locationMap = FindObjectOfType<AILocationMap>();
            }

            if (locationMap == null)
            {
                Debug.LogWarning("No AILocationMap found for WandererAI. The AI will wander around its starting position.");
            }
        }

        SetNewWanderTarget();
    }

    protected override void UpdateState()
    {
        base.UpdateState(); // Handle chase detection

        if (!isChasing)
        {
            currentState = AIState.Wander;
        }
    }

    protected override void UpdateBehavior()
    {
        if (currentState == AIState.Chase)
        {
            base.UpdateBehavior(); // Use base chase behavior
        }
        else if (currentState == AIState.Wander)
        {
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0)
            {
                SetNewWanderTarget();
            }

            MoveTowards(targetPosition);
        }
    }

    private void SetNewWanderTarget()
    {
        if (locationMap != null)
        {
            if (useAreaName)
            {
                targetPosition = locationMap.GetRandomPointInArea(areaName);
            }
            else
            {
                targetPosition = locationMap.GetRandomPointInArea(areaIndex);
            }

            // Make sure the target is on the ground
            if (Physics.Raycast(targetPosition + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20, obstacleLayer))
            {
                targetPosition = hit.point;
            }
        }
        else
        {
            // Fallback if no location map - wander around starting position
            Vector2 randomCircle = Random.insideUnitCircle * 10f;
            targetPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Make sure the target is on the ground
            if (Physics.Raycast(targetPosition + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20, obstacleLayer))
            {
                targetPosition = hit.point;
            }
        }

        // Set a new random timer
        wanderTimer = Random.Range(minWanderTime, maxWanderTime);
    }

    protected override void OnChaseEnd()
    {
        // Return to wander state and get a new target
        currentState = AIState.Wander;
        SetNewWanderTarget();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Draw current target position if in play mode
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.5f);
        }
    }
}
