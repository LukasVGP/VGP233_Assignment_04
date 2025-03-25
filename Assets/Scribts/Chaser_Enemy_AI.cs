using UnityEngine;

public class ChaserAI : BaseAI
{
    [Header("Chaser AI Settings")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float speedPercentage = 0.75f; // 75% of player speed
    [SerializeField] private float loseInterestDistance = 15f;

    private PlayerController playerController;
    private bool isChasing = false;

    protected override void Awake()
    {
        base.Awake();

        // Find player controller to get speed
        if (playerTransform != null)
        {
            playerController = playerTransform.GetComponent<PlayerController>();
        }
    }

    protected override void UpdateState()
    {
        if (playerTransform == null || playerController == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Start chasing if player is within detection range
        if (distanceToPlayer <= detectionRange && CanSeePlayer())
        {
            isChasing = true;
            currentState = AIState.Chase;

            // Set speed to 75% of player's speed
            moveSpeed = playerController.GetMoveSpeed() * speedPercentage;
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
        else
        {
            // Just idle in place
            // You could add some idle animation or small movements here
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

        // Draw detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw lose interest range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseInterestDistance);
    }
}
