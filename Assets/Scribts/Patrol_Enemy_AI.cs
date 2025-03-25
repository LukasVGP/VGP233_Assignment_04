using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : BaseAI
{
    [Header("Patrol AI Settings")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseRadius = 8f;
    [SerializeField] private float loseInterestDistance = 15f;
    [SerializeField] private float waypointReachedDistance = 0.5f;

    private int currentPatrolIndex = 0;

    protected override void UpdateState()
    {
        if (currentState == AIState.Chase)
        {
            // Check if we should stop chasing
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                if (distanceToPlayer > loseInterestDistance || !CanSeePlayer())
                {
                    currentState = AIState.Patrol;
                    moveSpeed = patrolSpeed;
                }
            }
        }
        else
        {
            // Check if we should start chasing
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                if (distanceToPlayer <= chaseRadius && CanSeePlayer())
                {
                    currentState = AIState.Chase;
                    moveSpeed = chaseSpeed;
                }
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
        else
        {
            // Patrol between waypoints
            if (patrolPoints.Count == 0) return;

            Transform currentPatrolPoint = patrolPoints[currentPatrolIndex];
            if (currentPatrolPoint != null)
            {
                MoveTowards(currentPatrolPoint.position);

                // Check if we've reached the waypoint
                float distanceToWaypoint = Vector3.Distance(transform.position, currentPatrolPoint.position);
                if (distanceToWaypoint < waypointReachedDistance)
                {
                    // Move to the next patrol point
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                }
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && !player.IsInvulnerable())
            {
                player.TakeDamage(transform.position);
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && !player.IsInvulnerable())
            {
                player.TakeDamage(transform.position);
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

        // Draw patrol path
        if (patrolPoints.Count > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Vector3 currentPoint = patrolPoints[i].position;
                    Vector3 nextPoint = patrolPoints[(i + 1) % patrolPoints.Count].position;

                    Gizmos.DrawSphere(currentPoint, 0.3f);
                    Gizmos.DrawLine(currentPoint, nextPoint);
                }
            }
        }
    }
}
