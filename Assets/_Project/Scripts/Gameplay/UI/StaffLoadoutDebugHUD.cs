using UnityEngine;

public class StaffLoadoutDebugHUD : MonoBehaviour
{
    [SerializeField] private PlayerLoadout loadout;
    [SerializeField] private StaffCooldowns cooldowns;
    [SerializeField] private bool showHud = true;
    [SerializeField] private Vector2 position = new Vector2(16f, 16f);
    [SerializeField] private float width = 440f;

    private GUIStyle boxStyle;
    private GUIStyle selectedStyle;
    private GUIStyle normalStyle;

    private void Awake()
    {
        if (loadout == null)
        {
            loadout = GetComponent<PlayerLoadout>();
        }

        if (cooldowns == null)
        {
            cooldowns = GetComponent<StaffCooldowns>();
        }
    }

    private void OnGUI()
    {
        if (!showHud || loadout == null)
        {
            return;
        }

        EnsureStyles();

        StaffSlot[] slots = loadout.StaffSlots;
        float height = 34f + slots.Length * 24f;
        Rect boxRect = new Rect(position.x, position.y, width, height);

        GUI.Box(boxRect, "Staff Loadout", boxStyle);

        float y = position.y + 28f;

        for (int i = 0; i < slots.Length; i++)
        {
            StaffSlot slot = slots[i];
            bool selected = loadout.SelectedSlotIndex == i;

            string staffName = slot.HasStaff ? slot.Staff.DisplayName : "Empty";
            string spellName = slot.Spell != null ? slot.Spell.DisplayName : "No Spell";
            string cooldownText = GetCooldownText(slot.RequiredTier);
            string line = $"{i + 1}. {slot.RequiredTier}: {staffName} -> {spellName}  {cooldownText}";

            GUI.Label(new Rect(position.x + 12f, y, width - 24f, 22f), line, selected ? selectedStyle : normalStyle);
            y += 24f;
        }
    }

    private void EnsureStyles()
    {
        if (boxStyle != null)
        {
            return;
        }

        boxStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.UpperLeft,
            padding = new RectOffset(10, 10, 6, 8)
        };

        normalStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            normal = { textColor = Color.white }
        };

        selectedStyle = new GUIStyle(normalStyle)
        {
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(1f, 0.82f, 0.35f) }
        };
    }

    private string GetCooldownText(StaffTier tier)
    {
        if (cooldowns == null)
        {
            return "";
        }

        float remaining = cooldowns.GetRemaining(tier);

        if (remaining <= 0f)
        {
            return "[Ready]";
        }

        return $"[CD {remaining:0.0}s]";
    }
}
