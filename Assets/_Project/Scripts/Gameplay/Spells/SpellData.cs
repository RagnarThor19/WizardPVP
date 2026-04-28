using UnityEngine;

[CreateAssetMenu(menuName = "WizardPVP/Spells/Spell Data", fileName = "NewSpell")]
public class SpellData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string displayName = "New Spell";
    [SerializeField, TextArea] private string description;
    [SerializeField] private StaffTier tier = StaffTier.Beginner;

    [Header("Timing")]
    [SerializeField] private float cooldown = 2f;

    [Header("Combat")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float knockbackForce = 0f;
    [SerializeField] private DamageType damageType = DamageType.Spell;

    public string DisplayName => displayName;
    public string Description => description;
    public StaffTier Tier => tier;
    public float Cooldown => cooldown;
    public float Damage => damage;
    public float KnockbackForce => knockbackForce;
    public DamageType DamageType => damageType;

    private void OnValidate()
    {
        cooldown = Mathf.Max(0f, cooldown);
        damage = Mathf.Max(0f, damage);
        knockbackForce = Mathf.Max(0f, knockbackForce);
    }
}
