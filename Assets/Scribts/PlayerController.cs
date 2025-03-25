using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 20f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invulnerabilityTime = 2f;
    [SerializeField] private float blinkRate = 0.1f;
    [SerializeField] private float knockbackForce = 10f;

    [Header("Visual Feedback")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material hitMaterial;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;

    // Component references
    private CharacterController characterController;
    private Animator animator;
    private AudioSource audioSource;
    private Renderer playerRenderer;

    // Movement variables
    private Vector3 moveDirection = Vector3.zero;
    private float verticalVelocity = 0f;
    private bool isGrounded = false;

    // Health variables
    private int currentHealth;
    private bool isInvulnerable = false;

    private void Awake()
    {
        // Get component references
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerRenderer = GetComponentInChildren<Renderer>();

        // Initialize health
        currentHealth = maxHealth;

        // Store default material if not set
        if (defaultMaterial == null && playerRenderer != null)
        {
            defaultMaterial = playerRenderer.material;
        }
    }

    private void Update()
    {
        // Check if grounded
        CheckGrounded();

        // Handle player input
        HandleMovement();
        HandleJump();

        // Apply gravity
        ApplyGravity();

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);

        // Update animations
        UpdateAnimations();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset vertical velocity when grounded
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small negative value to keep the player grounded
        }
    }

    private void HandleMovement()
    {
        // Get input axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude > 0.1f)
        {
            // Calculate target rotation
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

            // Smoothly rotate towards the input direction
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move in the facing direction
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * moveSpeed;
        }
        else
        {
            // No input, stop horizontal movement
            moveDirection = new Vector3(0f, moveDirection.y, 0f);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = jumpForce;

            // Play jump sound
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }

        // Update vertical component of move direction
        moveDirection.y = verticalVelocity;
    }

    private void ApplyGravity()
    {
        // Apply gravity
        verticalVelocity -= gravity * Time.deltaTime;
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            // Set animation parameters
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("Speed", new Vector2(moveDirection.x, moveDirection.z).magnitude);

            // Set jump/fall animation
            if (!isGrounded)
            {
                animator.SetFloat("VerticalVelocity", verticalVelocity);
            }
        }
    }

    public void TakeDamage(Vector3 damageSource)
    {
        // Check if player is invulnerable
        if (isInvulnerable)
            return;

        // Reduce health
        currentHealth--;

        // Play hit sound
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Apply knockback
        Vector3 knockbackDirection = (transform.position - damageSource).normalized;
        knockbackDirection.y = 0; // Keep knockback horizontal

        // Apply the knockback force
        StartCoroutine(ApplyKnockback(knockbackDirection));

        // Start invulnerability period
        StartCoroutine(InvulnerabilityPeriod());

        // Check if player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Notify GameManager about hit
            if (GameManager.Instance != null)
            {
                // Use LoseLife instead of PlayerHit
                GameManager.Instance.LoseLife();
            }
        }
    }

    private IEnumerator ApplyKnockback(Vector3 direction)
    {
        float knockbackTime = 0.2f;
        float startTime = Time.time;

        while (Time.time < startTime + knockbackTime)
        {
            characterController.Move(direction * knockbackForce * Time.deltaTime);
            yield return null;
        }
    }

    private void Die()
    {
        // Play death sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Notify GameManager about death
        if (GameManager.Instance != null)
        {
            // Use GameOver instead of PlayerDied
            GameManager.Instance.GameOver();
        }
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    private IEnumerator InvulnerabilityPeriod()
    {
        isInvulnerable = true;

        // Blink the player model
        if (playerRenderer != null)
        {
            float endTime = Time.time + invulnerabilityTime;
            bool isVisible = true;

            while (Time.time < endTime)
            {
                isVisible = !isVisible;

                if (isVisible)
                {
                    playerRenderer.material = defaultMaterial;
                }
                else
                {
                    if (hitMaterial != null)
                        playerRenderer.material = hitMaterial;
                    else
                        playerRenderer.enabled = false;
                }

                yield return new WaitForSeconds(blinkRate);
            }

            // Ensure player is visible at the end
            playerRenderer.material = defaultMaterial;
            playerRenderer.enabled = true;
        }
        else
        {
            // Just wait for invulnerability time if no renderer
            yield return new WaitForSeconds(invulnerabilityTime);
        }

        isInvulnerable = false;
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
}
