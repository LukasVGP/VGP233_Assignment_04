using UnityEngine;
using System.Collections;

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

    [Header("Damage Settings")]
    [SerializeField] private float invulnerabilityTime = 2f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private Material damagedMaterial;
    [SerializeField] private Material normalMaterial;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalVelocity = 0f;
    private bool isGrounded = false;
    private bool isInvulnerable = false;
    private Renderer playerRenderer;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerRenderer = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver() || GameManager.Instance.IsGamePaused())
            return;

        CheckGrounded();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * moveSpeed;
        }
        else
        {
            moveDirection = new Vector3(0f, moveDirection.y, 0f);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = jumpForce;
        }

        moveDirection.y = verticalVelocity;
    }

    private void ApplyGravity()
    {
        verticalVelocity -= gravity * Time.deltaTime;
    }

    public void TakeDamage(Vector3 damageSource)
    {
        if (isInvulnerable) return;

        // Apply knockback
        Vector3 knockbackDirection = (transform.position - damageSource).normalized;
        knockbackDirection.y = 0;
        characterController.Move(knockbackDirection * knockbackForce);

        // Lose a life
        GameManager.Instance.LoseLife();

        // Start invulnerability
        StartCoroutine(InvulnerabilityCoroutine());
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        // Visual feedback
        if (playerRenderer != null && damagedMaterial != null)
        {
            Material originalMaterial = playerRenderer.material;

            // Flash between materials
            float flashInterval = 0.2f;
            int flashCount = Mathf.FloorToInt(invulnerabilityTime / flashInterval);

            for (int i = 0; i < flashCount; i++)
            {
                playerRenderer.material = (i % 2 == 0) ? damagedMaterial : originalMaterial;
                yield return new WaitForSeconds(flashInterval);
            }

            playerRenderer.material = originalMaterial;
        }
        else
        {
            yield return new WaitForSeconds(invulnerabilityTime);
        }

        isInvulnerable = false;
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
}
