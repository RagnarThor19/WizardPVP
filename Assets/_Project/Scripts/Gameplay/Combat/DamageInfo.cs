using UnityEngine;

public readonly struct DamageInfo
{
    public float Amount { get; }
    public GameObject Source { get; }
    public Vector3 HitPoint { get; }
    public Vector3 HitDirection { get; }
    public float KnockbackForce { get; }
    public DamageType DamageType { get; }

    public DamageInfo(
        float amount,
        GameObject source = null,
        Vector3 hitPoint = default(Vector3),
        Vector3 hitDirection = default(Vector3),
        float knockbackForce = 0f,
        DamageType damageType = DamageType.Generic)
    {
        Amount = Mathf.Max(0f, amount);
        Source = source;
        HitPoint = hitPoint;
        HitDirection = hitDirection.sqrMagnitude > 0.0001f ? hitDirection.normalized : Vector3.zero;
        KnockbackForce = Mathf.Max(0f, knockbackForce);
        DamageType = damageType;
    }
}
