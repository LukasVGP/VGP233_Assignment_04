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
    [SerializeField] protected bool useCharacterController = false;

    protected AIState currentState = AIState.Idle;
    protected Vector3 targetPosition;
    protected CharacterController characterController;
    protected Rigidbody rigidBody;
    protected CapsuleCollider capsuleCollider;
    protected float verticalVelocity = 0f;
    protected bool isGrounded = true;

    protected virtual void Awake()
    {
        // Try to get components
        characterController = GetComponent<CharacterController>();
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // If we don't have a CharacterController and we're set to use one, add it
        if (useCharacterController && characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 2.0f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 1.0f, 0);
        }
        // If we're not using CharacterController, make sure we have a collider
        else if (!useCharacterController && capsuleCollider == null)
        {
            capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 2.0f;
            capsuleCollider.radius = 0.5f;
            capsuleCollider.center = new Vector3(0, 1.0f, 0);

            // Add a rigidbody if we don't have one
            if (rigidBody == null)
            {
                rigidBody = gameObject.AddComponent<Rigidbody>();
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
                rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
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

        // Only use Update for movement if using CharacterController
        if (useCharacterController)
        {
            UpdateBehavior();
        }
    }

    protected virtual void FixedUpdate()
    {
        // Use FixedUpdate for physics-based movement
        if (!useCharacterController && rigidBody != null)
        {
            UpdateBehavior();
        }
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
            // Check if there's an obstacle between AI and player
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
            if (useCharacterController && characterController != null)
            {
                // Use CharacterController for movement
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
            else if (rigidBody != null)
            {
                // Use Rigidbody for movement
                Vector3 movement = transform.forward * moveSpeed;

                // Check if grounded
                isGrounded = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.height * 0.5f + 0.1f);

                // Apply gravity
                if (!isGrounded)
                {
                    verticalVelocity -= gravity * Time.deltaTime;
                }
                else
                {
                    verticalVelocity = -0.5f;
                }

                // Set velocity directly
                Vector3 velocity = movement;
                velocity.y = verticalVelocity;
                rigidBody.linearVelocity = velocity;
            }
            else
            {
                // Fallback to simple transform movement
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
            else
            {
                Debug.LogError("GameManager.Instance is null. Make sure you have a GameManager in your scene.");
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
            else
            {
                Debug.LogError("GameManager.Instance is null. Make sure you have a GameManager in your scene.");
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
