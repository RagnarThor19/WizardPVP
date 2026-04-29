using UnityEngine;
using UnityEngine.InputSystem;

public class DebugDamageCaster : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private Key damageKey = Key.F;
    [SerializeField] private bool allowLeftMouseButton = false;

    [Header("Raycast")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float range = 25f;
    [SerializeField] private LayerMask hitLayers = ~0;

    [Header("Damage")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private DamageType damageType = DamageType.Spell;

    [Header("Debug")]
    [SerializeField] private bool logReadyMessage = true;

    private void Awake()
    {
        if (rayOrigin == null)
        {
            Camera mainCamera = Camera.main;
            rayOrigin = mainCamera != null ? mainCamera.transform : transform;
        }
    }

    private void OnEnable()
    {
        if (logReadyMessage)
        {
            Debug.Log($"DebugDamageCaster is active on {name}. Press {damageKey} to test damage.");
        }
    }

    private void Update()
    {
        bool keyboardPressed = Keyboard.current != null && Keyboard.current[damageKey].wasPressedThisFrame;
        bool mousePressed = allowLeftMouseButton && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (keyboardPressed || mousePressed)
        {
            TryDamageTarget();
        }
    }

    private void TryDamageTarget()
    {
        if (!Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, range, hitLayers, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("DebugDamageCaster did not hit anything.");
            return;
        }

        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            Debug.Log($"DebugDamageCaster hit {hit.collider.name}, but it has no IDamageable component.");
            return;
        }

        Vector3 hitDirection = (hit.collider.transform.position - rayOrigin.position).normalized;

        DamageInfo damageInfo = new DamageInfo(
            damageAmount,
            gameObject,
            hit.point,
            hitDirection,
            knockbackForce,
            damageType
        );

        damageable.TakeDamage(damageInfo);

        Debug.Log($"DebugDamageCaster dealt {damageAmount} damage to {hit.collider.name}.");
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = rayOrigin != null ? rayOrigin : transform;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin.position, origin.forward * range);
    }
}
