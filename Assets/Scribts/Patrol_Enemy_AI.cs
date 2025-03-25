using UnityEngine;

public class PatrolAI : BaseAI
{
    public enum PatrolMode
    {
        Waypoints,
        LocationMap
    }

    [Header("Patrol AI Settings")]
    [SerializeField] private PatrolMode patrolMode = PatrolMode.Waypoints;

    [Header("Waypoint Settings")]
    [SerializeField] private AIWaypointSystem waypointSystem;
    [SerializeField] private float waypointReachedDistance = 0.5f;

    [Header("Location Map Settings")]
    [SerializeField] private AILocationMap locationMap;
    [SerializeField] private string[] patrolAreaSequence; // Names of areas to patrol in sequence
    [SerializeField] private float areaStayTime = 5f; // How long to stay in each area

    private int currentWaypointIndex = 0;
    private int currentAreaIndex = 0;
    private float areaTimer = 0f;
    private bool isMovingToNextArea = true;

    protected override void Awake()
    {
        base.Awake();

        // Find systems if not assigned
        if (patrolMode == PatrolMode.Waypoints && waypointSystem == null)
        {
            waypointSystem = GetComponentInParent<AIWaypointSystem>();
            if (waypointSystem == null)
            {
                waypointSystem = FindObjectOfType<AIWaypointSystem>();
            }
        }

        if (patrolMode == PatrolMode.LocationMap && locationMap == null)
        {
            locationMap = GetComponentInParent<AILocationMap>();
            if (locationMap == null)
            {
                locationMap = FindObjectOfType<AILocationMap>();
            }
        }

        // Set initial target
        if (patrolMode == PatrolMode.LocationMap && locationMap != null && patrolAreaSequence.Length > 0)
        {
            SetNewAreaTarget();
        }
    }

    protected override void UpdateState()
    {
        base.UpdateState(); // Handle chase detection

        if (!isChasing)
        {
            currentState = AIState.Patrol;
        }
    }

    protected override void UpdateBehavior()
    {
        if (currentState == AIState.Chase)
        {
            base.UpdateBehavior(); // Use base chase behavior
        }
        else if (currentState == AIState.Patrol)
        {
            if (patrolMode == PatrolMode.Waypoints)
            {
                PatrolWaypoints();
            }
            else
            {
                PatrolLocationMap();
            }
        }
    }

    private void PatrolWaypoints()
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

    private void PatrolLocationMap()
    {
        if (locationMap == null || patrolAreaSequence.Length == 0)
        {
            currentState = AIState.Idle;
            return;
        }

        if (isMovingToNextArea)
        {
            // Moving to the next area's center
            string currentAreaName = patrolAreaSequence[currentAreaIndex];
            AILocationMap.PatrolArea area = locationMap.GetAreaByName(currentAreaName);

            if (area != null)
            {
                MoveTowards(area.centerPoint);

                // Check if we've reached the area
                float distanceToArea = Vector3.Distance(transform.position, area.centerPoint);
                if (distanceToArea < waypointReachedDistance)
                {
                    isMovingToNextArea = false;
                    areaTimer = areaStayTime;
                    // Set a random point in this area to patrol to
                    targetPosition = locationMap.GetRandomPointInArea(currentAreaName);
                }
            }
            else
            {
                // Area not found, move to next one
                currentAreaIndex = (currentAreaIndex + 1) % patrolAreaSequence.Length;
            }
        }
        else
        {
            // Staying within the current area
            MoveTowards(targetPosition);

            // Check if we've reached the target point or timer expired
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            areaTimer -= Time.deltaTime;

            if (distanceToTarget < waypointReachedDistance || areaTimer <= 0)
            {
                // Either we reached the point or time's up, move to next area
                currentAreaIndex = (currentAreaIndex + 1) % patrolAreaSequence.Length;
                isMovingToNextArea = true;
            }
        }
    }

    private void SetNewAreaTarget()
    {
        if (locationMap != null && patrolAreaSequence.Length > 0)
        {
            string currentAreaName = patrolAreaSequence[currentAreaIndex];
            targetPosition = locationMap.GetRandomPointInArea(currentAreaName);
        }
    }

    protected override void OnChaseEnd()
    {
        // Return to patrol state
        currentState = AIState.Patrol;
    }
}
