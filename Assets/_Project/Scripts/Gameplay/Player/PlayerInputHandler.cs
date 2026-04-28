using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool JumpPressedThisFrame { get; private set; }
    public bool JumpReleasedThisFrame { get; private set; }
    public bool JumpHeld { get; private set; }

    public bool DashPressed { get; private set; }
    public bool CastPressed { get; private set; }
    public bool SwingPressed { get; private set; }

    public int SelectedStaffSlot { get; private set; } = 1;

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction castAction;
    private InputAction swingAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        InputActionMap playerMap = playerInput.actions.FindActionMap("Player", true);

        moveAction = playerMap.FindAction("Move", true);
        lookAction = playerMap.FindAction("Look", true);
        jumpAction = playerMap.FindAction("Jump", true);
        dashAction = playerMap.FindAction("Dash", true);
        castAction = playerMap.FindAction("Cast", true);
        swingAction = playerMap.FindAction("Swing", true);
    }

    private void Update()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        LookInput = lookAction.ReadValue<Vector2>();

        JumpPressedThisFrame = jumpAction.WasPressedThisFrame();
        JumpReleasedThisFrame = jumpAction.WasReleasedThisFrame();
        JumpHeld = jumpAction.IsPressed();

        if (dashAction.WasPressedThisFrame()) DashPressed = true;
        if (castAction.WasPressedThisFrame()) CastPressed = true;
        if (swingAction.WasPressedThisFrame()) SwingPressed = true;
    }

    private void LateUpdate()
    {
        ClearOneFrameInputs();
    }

    public void OnStaff1(InputValue value)
    {
        if (value.isPressed) SelectedStaffSlot = 1;
    }

    public void OnStaff2(InputValue value)
    {
        if (value.isPressed) SelectedStaffSlot = 2;
    }

    public void OnStaff3(InputValue value)
    {
        if (value.isPressed) SelectedStaffSlot = 3;
    }

    public void OnStaff4(InputValue value)
    {
        if (value.isPressed) SelectedStaffSlot = 4;
    }

    public void OnStaff5(InputValue value)
    {
        if (value.isPressed) SelectedStaffSlot = 5;
    }

    public void OnStaff6(InputValue value)
    {
        if (value.isPressed) SelectedStaffSlot = 6;
    }

    private void ClearOneFrameInputs()
    {
        DashPressed = false;
        CastPressed = false;
        SwingPressed = false;
    }
}
