using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerLoadout))]
[RequireComponent(typeof(StaffCooldowns))]
public class PlayerSpellCaster : MonoBehaviour
{
    [Header("Casting")]
    [SerializeField] private Transform castOrigin;
    [SerializeField] private Transform visualOrigin;
    [SerializeField] private Vector3 visualOffsetFromCastOrigin = new Vector3(0.32f, -0.22f, 0.45f);
    [SerializeField] private float range = 35f;
    [SerializeField] private LayerMask hitLayers = ~0;

    [Header("Projectile Visual")]
    [SerializeField] private float projectileRadius = 0.25f;
    [SerializeField] private Color boltColor = new Color(0.45f, 0.85f, 1f, 0.7f);

    private PlayerInputHandler input;
    private PlayerLoadout loadout;
    private StaffCooldowns cooldowns;
    private Material projectileMaterial;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        loadout = GetComponent<PlayerLoadout>();
        cooldowns = GetComponent<StaffCooldowns>();

        if (castOrigin == null)
        {
            Camera mainCamera = Camera.main;
            castOrigin = mainCamera != null ? mainCamera.transform : transform;
        }

        TryFindStaffCastPoint();
    }

    private void Update()
    {
        if (input.CastPressed)
        {
            TryCastSelectedSpell();
        }
    }

    private void TryCastSelectedSpell()
    {
        StaffSlot selectedSlot = loadout.SelectedSlot;
        SpellData spell = loadout.SelectedSpell;

        if (spell == null)
        {
            Debug.Log("No spell equipped in the selected staff slot.");
            return;
        }

        StaffTier cooldownTier = selectedSlot.RequiredTier;

        if (cooldowns.IsOnCooldown(cooldownTier))
        {
            Debug.Log($"{cooldownTier} staffs are on cooldown for {cooldowns.GetRemaining(cooldownTier):0.0}s.");
            return;
        }

        Vector3 start = castOrigin.position;
        Vector3 direction = castOrigin.forward;
        Vector3 end = start + direction * range;

        if (Physics.Raycast(start, direction, out RaycastHit hit, range, hitLayers, QueryTriggerInteraction.Ignore))
        {
            end = hit.point;
        }

        LaunchProjectile(spell, end);

        cooldowns.StartCooldown(cooldownTier, spell.Cooldown);
    }

    private void LaunchProjectile(SpellData spell, Vector3 aimPoint)
    {
        Vector3 projectileStart = GetBoltStartPosition();
        Vector3 projectileDirection = (aimPoint - projectileStart).normalized;

        if (projectileDirection.sqrMagnitude <= 0.0001f)
        {
            projectileDirection = castOrigin.forward;
        }

        GameObject projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileObject.name = $"{spell.DisplayName} Projectile";
        projectileObject.transform.position = projectileStart;
        projectileObject.transform.localScale = Vector3.one * projectileRadius;

        Collider projectileCollider = projectileObject.GetComponent<Collider>();
        projectileCollider.enabled = false;

        Renderer projectileRenderer = projectileObject.GetComponent<Renderer>();
        projectileRenderer.material = GetProjectileMaterial();

        SpellProjectile projectile = projectileObject.AddComponent<SpellProjectile>();
        projectile.Initialize(
            spell,
            gameObject,
            projectileDirection,
            spell.ProjectileSpeed,
            spell.ProjectileLifetime,
            hitLayers
        );

        Debug.Log($"{spell.DisplayName} projectile cast.");
    }

    private Vector3 GetBoltStartPosition()
    {
        if (visualOrigin == null)
        {
            TryFindStaffCastPoint();
        }

        if (visualOrigin != null)
        {
            return visualOrigin.position;
        }

        return castOrigin.TransformPoint(visualOffsetFromCastOrigin);
    }

    private void TryFindStaffCastPoint()
    {
        StaffCastPoint castPoint = GetComponentInChildren<StaffCastPoint>();

        if (castPoint != null)
        {
            visualOrigin = castPoint.transform;
        }
    }

    private Material GetProjectileMaterial()
    {
        if (projectileMaterial != null)
        {
            return projectileMaterial;
        }

        Shader shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        }

        projectileMaterial = new Material(shader);
        projectileMaterial.color = boltColor;
        return projectileMaterial;
    }
}
