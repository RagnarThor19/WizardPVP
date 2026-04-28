using System;
using UnityEngine;

[Serializable]
public class StaffSlot
{
    [SerializeField] private string slotName;
    [SerializeField] private StaffTier requiredTier;
    [SerializeField] private StaffData staff;

    public string SlotName => slotName;
    public StaffTier RequiredTier => requiredTier;
    public StaffData Staff => staff;
    public bool HasStaff => staff != null;
    public bool HasInvalidStaff => staff != null && staff.Tier != requiredTier;
    public SpellData Spell => staff != null ? staff.AttachedSpell : null;

    public StaffSlot(string slotName, StaffTier requiredTier)
    {
        this.slotName = slotName;
        this.requiredTier = requiredTier;
    }

    public bool CanEquip(StaffData staffData)
    {
        return staffData == null || staffData.Tier == requiredTier;
    }

    public bool TryEquip(StaffData staffData)
    {
        if (!CanEquip(staffData))
        {
            return false;
        }

        staff = staffData;
        return true;
    }

    public void EditorSetSlotRules(string newSlotName, StaffTier newRequiredTier)
    {
        slotName = newSlotName;
        requiredTier = newRequiredTier;
    }

    public void EditorClearInvalidStaff(UnityEngine.Object owner)
    {
        if (!HasInvalidStaff)
        {
            return;
        }

        Debug.LogWarning($"{owner.name} removed {staff.DisplayName} from {slotName} because that slot requires {requiredTier} staffs.");
        staff = null;
    }
}
