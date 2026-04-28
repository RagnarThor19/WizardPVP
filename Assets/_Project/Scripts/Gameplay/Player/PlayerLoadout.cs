using UnityEngine;

[DisallowMultipleComponent]
public class PlayerLoadout : MonoBehaviour
{
    private const int SlotCount = 6;

    [SerializeField] private StaffSlot[] staffSlots = new StaffSlot[SlotCount];
    [SerializeField, Range(1, SlotCount)] private int selectedSlotNumber = 1;

    private PlayerInputHandler input;

    public int SelectedSlotNumber => selectedSlotNumber;
    public int SelectedSlotIndex => selectedSlotNumber - 1;
    public StaffSlot[] StaffSlots => staffSlots;
    public StaffSlot SelectedSlot => GetSlotByNumber(selectedSlotNumber);
    public StaffData SelectedStaff => SelectedSlot != null ? SelectedSlot.Staff : null;
    public SpellData SelectedSpell => SelectedSlot != null ? SelectedSlot.Spell : null;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        EnsureSlotLayout();
    }

    private void OnValidate()
    {
        EnsureSlotLayout();
        selectedSlotNumber = Mathf.Clamp(selectedSlotNumber, 1, SlotCount);
    }

    private void Update()
    {
        if (input != null)
        {
            SelectSlot(input.SelectedStaffSlot);
        }
    }

    public bool SelectSlot(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > SlotCount)
        {
            return false;
        }

        selectedSlotNumber = slotNumber;
        return true;
    }

    public StaffSlot GetSlotByNumber(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > SlotCount || staffSlots == null || staffSlots.Length != SlotCount)
        {
            return null;
        }

        return staffSlots[slotNumber - 1];
    }

    public bool TryEquipStaff(int slotNumber, StaffData staff)
    {
        StaffSlot slot = GetSlotByNumber(slotNumber);

        if (slot == null)
        {
            return false;
        }

        return slot.TryEquip(staff);
    }

    private void EnsureSlotLayout()
    {
        if (staffSlots == null || staffSlots.Length != SlotCount)
        {
            StaffSlot[] oldSlots = staffSlots;
            staffSlots = new StaffSlot[SlotCount];

            if (oldSlots != null)
            {
                int copyCount = Mathf.Min(oldSlots.Length, staffSlots.Length);

                for (int i = 0; i < copyCount; i++)
                {
                    staffSlots[i] = oldSlots[i];
                }
            }
        }

        EnsureSlot(0, "Slot 1 - Master", StaffTier.Master);
        EnsureSlot(1, "Slot 2 - Advanced", StaffTier.Advanced);
        EnsureSlot(2, "Slot 3 - Advanced", StaffTier.Advanced);
        EnsureSlot(3, "Slot 4 - Beginner", StaffTier.Beginner);
        EnsureSlot(4, "Slot 5 - Beginner", StaffTier.Beginner);
        EnsureSlot(5, "Slot 6 - Beginner", StaffTier.Beginner);
    }

    private void EnsureSlot(int index, string slotName, StaffTier requiredTier)
    {
        if (staffSlots[index] == null)
        {
            staffSlots[index] = new StaffSlot(slotName, requiredTier);
        }

        staffSlots[index].EditorSetSlotRules(slotName, requiredTier);
        staffSlots[index].EditorClearInvalidStaff(this);
    }
}
