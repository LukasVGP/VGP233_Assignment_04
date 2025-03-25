using UnityEngine;

public class PatrolAI : BaseAI
{
    [Header("Patrol AI Settings")]
    [SerializeField] private AIWaypointSystem waypointSystem;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseRadius = 8f;
    [SerializeField] private float loseInterestDistance = 15f;
    [SerializeField] private float waypointReachedDistance = 0.5f;

    private int currentWaypointIndex = 0;
    private bool isChasing = false;

    protected override void Awake()
    {
        base.Awake();

        // Find waypoint system if not assigned
        if (waypointSystem == null)
        {
            waypointSystem = GetComponentInParent<AIWaypointSystem>();
        }

        // Set initial speed
        moveSpeed = patrolSpeed;
    }

    protected override void UpdateState()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (currentState == AIState.Chase)
        {
            // Check if we should stop chasing
            if (distanceToPlayer > loseInterestDistance || !CanSeePlayer())
            {
                currentState = AIState.Patrol;
                moveSpeed = patrolSpeed;
                isChasing = false;
            }
        }
        else
        {
            // Check if we should start chasing
            if (distanceToPlayer <= chaseRadius && CanSeePlayer())
            {
                currentState = AIState.Chase;
                moveSpeed = chaseSpeed;
                isChasing = true;
            }
        }
    }

    protected override void UpdateBehavior()
    {
        if (currentState == AIState.Chase && playerTransform != null)
        {
            // Chase the player
            MoveTowards(playerTransform.position);
        }
        else if (currentState == AIState.Patrol)
        {
            // Patrol between waypoints
            if (waypointSystem == null || waypointSystem.GetWaypointCount() == 0)
            {
                currentState = AIState.Idle;
                return;
            }

            Transform currentWaypoint = waypointSystem.GetWaypoint(currentWaypointIndex);
            if (currentWaypoint != null)
            {
                MoveTowards(currentWaypoint.position);

                // Check if we've reached the waypoint
                float distanceToWaypoint = Vector3.Distance(transform.position, currentWaypoint.position);
                if (distanceToWaypoint < waypointReachedDistance)
                {
                    // Move to the next waypoint
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypointSystem.GetWaypointCount();
                }
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                GameManager.Instance.LoseLife();
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Draw chase radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Draw lose interest distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, loseInterestDistance);
    }
}
