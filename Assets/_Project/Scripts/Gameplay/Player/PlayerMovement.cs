using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float smallJumpHeight = 1.5f;
    [SerializeField] private float bigJumpHeight = 2.25f;
    [SerializeField] private float maxJumpHoldTime = 0.16f;
    [SerializeField] private float jumpCutMultiplier = 0.45f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.08f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float coyoteTime = 0.08f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float groundedGravity = -2f;

    private CharacterController characterController;
    private PlayerInputHandler input;

    private Vector3 verticalVelocity;

    private bool isJumping;
    private float jumpHoldTimer;
    private float jumpBufferCounter;
    private float coyoteCounter;

    private float smallJumpVelocity;
    private float bigJumpVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();

        smallJumpVelocity = Mathf.Sqrt(smallJumpHeight * -2f * gravity);
        bigJumpVelocity = Mathf.Sqrt(bigJumpHeight * -2f * gravity);
    }

    private void Update()
    {
        HandleJumpBuffer();
        HandleMovementAndGravity();

        input.ClearOneFrameInputs();
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

        if (grounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (grounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = groundedGravity;
            isJumping = false;
            jumpHoldTimer = 0f;
        }

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
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

        verticalVelocity.y += gravity * Time.deltaTime;

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
        jumpHoldTimer = 0f;

        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
    }

    private void ContinueHeldJump()
    {
        jumpHoldTimer += Time.deltaTime;

        float holdPercent = jumpHoldTimer / maxJumpHoldTime;
        float targetVelocity = Mathf.Lerp(smallJumpVelocity, bigJumpVelocity, holdPercent);

        if (verticalVelocity.y < targetVelocity)
        {
            verticalVelocity.y = targetVelocity;
        }
    }

    private bool IsGrounded()
    {
        if (characterController.isGrounded)
        {
            return true;
        }

        float sphereRadius = characterController.radius * 0.9f;

        Vector3 spherePosition =
            transform.position +
            characterController.center -
            Vector3.up * ((characterController.height / 2f) - sphereRadius + groundCheckDistance);

        return Physics.CheckSphere(
            spherePosition,
            sphereRadius,
            ~0,
            QueryTriggerInteraction.Ignore
        );
    }
}