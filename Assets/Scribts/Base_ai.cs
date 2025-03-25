using UnityEngine;

public enum AIState
{
    Idle,
    Patrol,
    Chase,
    Wander
}

public class BaseAI : MonoBehaviour
{
    [Header("Base AI Settings")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float rotationSpeed = 120f;
    [SerializeField] protected float detectionRadius = 5f;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected LayerMask obstacleLayer;
    [SerializeField] protected float gravity = 20f;

    protected AIState currentState = AIState.Idle;
    protected Vector3 targetPosition;
    protected CharacterController characterController;
    protected float verticalVelocity = 0f;

    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 2.0f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 1.0f, 0);
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    protected virtual void Update()
    {
        UpdateState();
        UpdateBehavior();
    }

    protected virtual void UpdateState()
    {
        // Base implementation - override in child classes
    }

    protected virtual void UpdateBehavior()
    {
        // Base implementation - override in child classes
    }

    protected bool CanSeePlayer()
    {
        if (playerTransform == null) return false;

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRadius)
        {
            Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer.normalized);
            if (!Physics.Raycast(ray, distanceToPlayer, obstacleLayer))
            {
                return true;
            }
        }
        return false;
    }

    protected void MoveTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            // Rotate towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move towards the target
            Vector3 movement = transform.forward * moveSpeed;

            // Apply gravity
            if (characterController.isGrounded)
            {
                verticalVelocity = -0.5f;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            movement.y = verticalVelocity;
            characterController.Move(movement * Time.deltaTime);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
