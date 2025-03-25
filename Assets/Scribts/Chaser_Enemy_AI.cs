using UnityEngine;

public class ChaserAI : BaseAI
{
    [Header("Chaser AI Settings")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float loseInterestDistance = 15f;

    private bool isChasing = false;

    protected override void UpdateState()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Start chasing if player is within detection range
        if (distanceToPlayer <= detectionRange && CanSeePlayer())
        {
            isChasing = true;
            currentState = AIState.Chase;
        }
        // Stop chasing if player is too far away
        else if (isChasing && distanceToPlayer > loseInterestDistance)
        {
            isChasing = false;
            currentState = AIState.Idle;
        }
    }

    protected override void UpdateBehavior()
    {
        if (currentState == AIState.Chase && playerTransform != null)
        {
            // Chase the player
            MoveTowards(playerTransform.position);
        }
    }
}
