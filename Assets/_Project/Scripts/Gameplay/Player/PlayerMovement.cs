using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float smallJumpHeight = 1.4f;
    [SerializeField] private float bigJumpHeight = 2.1f;
    [SerializeField] private float maxJumpHoldTime = 0.14f;
    [SerializeField] private float jumpCutMultiplier = 0.45f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float coyoteTime = 0.08f;
    [SerializeField] private LayerMask groundLayers = ~0;

    [Header("Gravity")]
    [SerializeField] private float gravity = -34f;
    [SerializeField] private float fallGravityMultiplier = 1.45f;
    [SerializeField] private float groundedGravity = -3f;

    private CharacterController characterController;
    private PlayerInputHandler input;

    private Vector3 verticalVelocity;

    private bool isJumping;
    private bool hasJumpedSinceGrounded;

    private float jumpHoldTimer;
    private float jumpBufferCounter;
    private float coyoteCounter;

    private float smallJumpVelocity;
    private float bigJumpVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();

        RecalculateJumpVelocities();
    }

    private void OnValidate()
    {
        RecalculateJumpVelocities();
    }

    private void Update()
    {
        HandleJumpBuffer();
        HandleMovementAndGravity();

        input.ClearOneFrameInputs();
    }

    private void RecalculateJumpVelocities()
    {
        smallJumpVelocity = Mathf.Sqrt(smallJumpHeight * -2f * gravity);
        bigJumpVelocity = Mathf.Sqrt(bigJumpHeight * -2f * gravity);
    }

    private void HandleJumpBuffer()
    {
        if (input.JumpPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void HandleMovementAndGravity()
    {
        bool grounded = IsGrounded();

        if (grounded && verticalVelocity.y <= 0f)
        {
            coyoteCounter = coyoteTime;
            hasJumpedSinceGrounded = false;
            isJumping = false;
            jumpHoldTimer = 0f;
            verticalVelocity.y = groundedGravity;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteCounter > 0f && !hasJumpedSinceGrounded)
        {
            StartJump();
        }

        if (isJumping && input.JumpHeld && jumpHoldTimer < maxJumpHoldTime)
        {
            ContinueHeldJump();
        }

        if (input.JumpReleasedThisFrame && verticalVelocity.y > 0f)
        {
            verticalVelocity.y *= jumpCutMultiplier;
            isJumping = false;
        }

        ApplyGravity();

        Vector2 moveInput = input.MoveInput;

        Vector3 horizontalVelocity =
            (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;

        Vector3 finalVelocity = horizontalVelocity + verticalVelocity;

        characterController.Move(finalVelocity * Time.deltaTime);
    }

    private void StartJump()
    {
        verticalVelocity.y = smallJumpVelocity;

        isJumping = true;
        hasJumpedSinceGrounded = true;
        jumpHoldTimer = 0f;

        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
    }

    private void ContinueHeldJump()
    {
        jumpHoldTimer += Time.deltaTime;

        float holdPercent = jumpHoldTimer / maxJumpHoldTime;
        float extraVelocity = Mathf.Lerp(0f, bigJumpVelocity - smallJumpVelocity, holdPercent);

        verticalVelocity.y += extraVelocity * Time.deltaTime * 8f;

        if (verticalVelocity.y > bigJumpVelocity)
        {
            verticalVelocity.y = bigJumpVelocity;
        }
    }

    private void ApplyGravity()
    {
        float currentGravity = gravity;

        if (verticalVelocity.y < 0f)
        {
            currentGravity *= fallGravityMultiplier;
        }

        verticalVelocity.y += currentGravity * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        if (verticalVelocity.y > 0.1f)
        {
            return false;
        }

        Vector3 rayStart = transform.position + characterController.center;
        float rayDistance = (characterController.height / 2f) + groundCheckDistance;

        return Physics.Raycast(
            rayStart,
            Vector3.down,
            rayDistance,
            groundLayers,
            QueryTriggerInteraction.Ignore
        );
    }
}