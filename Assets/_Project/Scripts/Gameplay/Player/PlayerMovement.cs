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

    [Header("Gravity")]
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float groundedGravity = -2f;

    private CharacterController characterController;
    private PlayerInputHandler input;

    private Vector3 verticalVelocity;

    private bool isJumping;
    private bool hasReleasedJump;
    private float jumpHoldTimer;

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
        HandleMovement();
        HandleJumpAndGravity();

        input.ClearOneFrameInputs();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = input.MoveInput;

        Vector3 moveDirection =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void HandleJumpAndGravity()
    {
        bool grounded = characterController.isGrounded;

        if (grounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = groundedGravity;
            isJumping = false;
            hasReleasedJump = false;
            jumpHoldTimer = 0f;
        }

        if (grounded && input.JumpPressedThisFrame)
        {
            verticalVelocity.y = smallJumpVelocity;
            isJumping = true;
            hasReleasedJump = false;
            jumpHoldTimer = 0f;
        }

        if (isJumping)
        {
            if (!input.JumpHeld)
            {
                hasReleasedJump = true;
            }

            if (input.JumpHeld && !hasReleasedJump && jumpHoldTimer < maxJumpHoldTime)
            {
                jumpHoldTimer += Time.deltaTime;

                float holdPercent = jumpHoldTimer / maxJumpHoldTime;
                verticalVelocity.y = Mathf.Lerp(smallJumpVelocity, bigJumpVelocity, holdPercent);
            }

            if (hasReleasedJump && verticalVelocity.y > 0f)
            {
                verticalVelocity.y *= jumpCutMultiplier;
                isJumping = false;
            }
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }
}