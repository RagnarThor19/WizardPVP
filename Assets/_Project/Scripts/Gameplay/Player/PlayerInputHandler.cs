using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool JumpPressedThisFrame { get; private set; }
    public bool JumpHeld { get; private set; }

    public bool DashPressed { get; private set; }
    public bool CastPressed { get; private set; }
    public bool SwingPressed { get; private set; }

    public int SelectedStaffSlot { get; private set; } = 1;

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        LookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        bool isPressed = value.isPressed;

        if (isPressed && !JumpHeld)
        {
            JumpPressedThisFrame = true;
        }

        JumpHeld = isPressed;
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed) DashPressed = true;
    }

    public void OnCast(InputValue value)
    {
        if (value.isPressed) CastPressed = true;
    }

    public void OnSwing(InputValue value)
    {
        if (value.isPressed) SwingPressed = true;
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

    public void ClearOneFrameInputs()
    {
        JumpPressedThisFrame = false;
        DashPressed = false;
        CastPressed = false;
        SwingPressed = false;
    }
}