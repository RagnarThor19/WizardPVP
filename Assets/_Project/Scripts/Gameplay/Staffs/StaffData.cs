using UnityEngine;

[CreateAssetMenu(menuName = "WizardPVP/Staffs/Staff Data", fileName = "NewStaff")]
public class StaffData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string displayName = "New Staff";
    [SerializeField, TextArea] private string description;
    [SerializeField] private StaffTier tier = StaffTier.Beginner;

    [Header("Spell")]
    [SerializeField] private SpellData attachedSpell;

    public string DisplayName => displayName;
    public string Description => description;
    public StaffTier Tier => tier;
    public SpellData AttachedSpell => attachedSpell;

    public bool HasSpell => attachedSpell != null;

    private void OnValidate()
    {
        if (attachedSpell != null && attachedSpell.Tier != tier)
        {
            Debug.LogWarning($"{name} has a {tier} staff tier, but its spell is {attachedSpell.Tier}. This is allowed for testing, but should usually match.");
        }
    }
}
