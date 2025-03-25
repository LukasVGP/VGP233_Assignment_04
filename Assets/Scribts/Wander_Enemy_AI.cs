using UnityEngine;

public class WandererAI : BaseAI
{
    [Header("Wanderer AI Settings")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float minWanderTime = 3f;
    [SerializeField] private float maxWanderTime = 8f;
    [SerializeField] private Vector3 centerPoint;

    private float wanderTimer;

    protected override void Awake()
    {
        base.Awake();
        centerPoint = transform.position;
        SetNewWanderTarget();
    }

    protected override void UpdateState()
    {
        currentState = AIState.Wander;
    }

    protected override void UpdateBehavior()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            SetNewWanderTarget();
        }

        MoveTowards(targetPosition);
    }

    private void SetNewWanderTarget()
    {
        // Get a random point within the wander radius
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = centerPoint + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Make sure the target is on the navmesh
        if (Physics.Raycast(targetPosition + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20, obstacleLayer))
        {
            targetPosition = hit.point;
        }

        // Set a new random timer
        wanderTimer = Random.Range(minWanderTime, maxWanderTime);
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(centerPoint, wanderRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.5f);
        }
    }
}
