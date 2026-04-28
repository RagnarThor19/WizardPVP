using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerKnockback : MonoBehaviour, IKnockbackable
{
    [SerializeField] private float knockbackDrag = 18f;
    [SerializeField] private float stopSpeed = 0.05f;

    private CharacterController characterController;
    private Vector3 knockbackVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (knockbackVelocity.sqrMagnitude <= stopSpeed * stopSpeed)
        {
            knockbackVelocity = Vector3.zero;
            return;
        }

        characterController.Move(knockbackVelocity * Time.deltaTime);
        knockbackVelocity = Vector3.MoveTowards(knockbackVelocity, Vector3.zero, knockbackDrag * Time.deltaTime);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (force <= 0f || direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        knockbackVelocity += direction.normalized * force;
    }
}
