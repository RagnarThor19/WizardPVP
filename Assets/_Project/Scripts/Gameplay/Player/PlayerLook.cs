using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float mouseSensitivity = 0.12f;

    private PlayerInputHandler input;
    private float xRotation;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        Vector2 lookInput = input.LookInput * mouseSensitivity;

        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x);
    }
}