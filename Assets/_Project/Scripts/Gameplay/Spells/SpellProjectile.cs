using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    private SpellData spell;
    private GameObject source;
    private Vector3 direction;
    private float speed;
    private float lifetime;
    private LayerMask hitLayers;
    private float age;
    private bool hasHit;

    public void Initialize(
        SpellData spellData,
        GameObject spellSource,
        Vector3 travelDirection,
        float projectileSpeed,
        float projectileLifetime,
        LayerMask projectileHitLayers)
    {
        spell = spellData;
        source = spellSource;
        direction = travelDirection.sqrMagnitude > 0.0001f ? travelDirection.normalized : transform.forward;
        speed = projectileSpeed;
        lifetime = projectileLifetime;
        hitLayers = projectileHitLayers;

        transform.forward = direction;
    }

    private void Update()
    {
        if (hasHit)
        {
            return;
        }

        age += Time.deltaTime;

        if (age >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 start = transform.position;
        Vector3 movement = direction * speed * Time.deltaTime;
        float distance = movement.magnitude;

        if (distance > 0f && Physics.Raycast(start, direction, out RaycastHit hit, distance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            Hit(hit);
            return;
        }

        transform.position += movement;
    }

    private void Hit(RaycastHit hit)
    {
        if (source != null && hit.collider.transform.root == source.transform.root)
        {
            transform.position += direction * 0.05f;
            return;
        }

        hasHit = true;

        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

        if (damageable != null && spell != null)
        {
            DamageInfo damageInfo = new DamageInfo(
                spell.Damage,
                source,
                hit.point,
                direction,
                spell.KnockbackForce,
                spell.DamageType
            );

            damageable.TakeDamage(damageInfo);
            Debug.Log($"{spell.DisplayName} projectile dealt {spell.Damage} damage to {hit.collider.name}.");
        }

        Destroy(gameObject);
    }
}
