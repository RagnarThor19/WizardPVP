using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthComponent : MonoBehaviour, ICombatTarget
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool startAtFullHealth = true;
    [SerializeField] private bool invulnerable;

    [Header("Target")]
    [SerializeField] private TeamId team = TeamId.Neutral;
    [SerializeField] private Transform targetTransform;

    [SerializeField] private float currentHealth;

    public event Action<DamageInfo> Damaged;
    public event Action<float, GameObject> Healed;
    public event Action<float, float> HealthChanged;
    public event Action<GameObject> Died;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;
    public bool IsInvulnerable => invulnerable;
    public TeamId Team => team;
    public GameObject TargetGameObject => gameObject;
    public Transform TargetTransform => targetTransform != null ? targetTransform : transform;

    private void Awake()
    {
        if (startAtFullHealth || currentHealth <= 0f)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (!IsAlive || invulnerable || damageInfo.Amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damageInfo.Amount, 0f);

        Damaged?.Invoke(damageInfo);
        HealthChanged?.Invoke(currentHealth, maxHealth);
        ApplyKnockbackFromDamage(damageInfo);

        if (currentHealth <= 0f)
        {
            Died?.Invoke(damageInfo.Source);
        }
    }

    public void Heal(float amount, GameObject source = null)
    {
        if (!IsAlive || amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        Healed?.Invoke(amount, source);
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetInvulnerable(bool value)
    {
        invulnerable = value;
    }

    private void ApplyKnockbackFromDamage(DamageInfo damageInfo)
    {
        if (damageInfo.KnockbackForce <= 0f || damageInfo.HitDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        IKnockbackable knockbackable = GetComponent<IKnockbackable>();

        if (knockbackable == null)
        {
            knockbackable = GetComponentInParent<IKnockbackable>();
        }

        if (knockbackable == null)
        {
            knockbackable = GetComponentInChildren<IKnockbackable>();
        }

        knockbackable?.ApplyKnockback(damageInfo.HitDirection, damageInfo.KnockbackForce);
    }
}
