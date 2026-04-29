using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyKnockback : MonoBehaviour, IKnockbackable
{
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;

    private Rigidbody targetRigidbody;

    private void Awake()
    {
        targetRigidbody = GetComponent<Rigidbody>();
        targetRigidbody.isKinematic = false;
        targetRigidbody.useGravity = true;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (force <= 0f || direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        targetRigidbody.AddForce(direction.normalized * force, forceMode);
    }
}
