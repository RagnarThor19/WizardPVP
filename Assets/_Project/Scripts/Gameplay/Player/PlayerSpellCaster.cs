using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerLoadout))]
public class PlayerSpellCaster : MonoBehaviour
{
    [Header("Casting")]
    [SerializeField] private Transform castOrigin;
    [SerializeField] private Transform visualOrigin;
    [SerializeField] private Vector3 visualOffsetFromCastOrigin = new Vector3(0.32f, -0.22f, 0.45f);
    [SerializeField] private float range = 35f;
    [SerializeField] private LayerMask hitLayers = ~0;

    [Header("Debug Visual")]
    [SerializeField] private bool drawBolt = true;
    [SerializeField] private float boltDuration = 0.4f;
    [SerializeField] private float boltWidth = 0.08f;
    [SerializeField] private Color boltColor = new Color(0.45f, 0.85f, 1f, 0.7f);

    private PlayerInputHandler input;
    private PlayerLoadout loadout;
    private Material boltMaterial;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        loadout = GetComponent<PlayerLoadout>();

        if (castOrigin == null)
        {
            Camera mainCamera = Camera.main;
            castOrigin = mainCamera != null ? mainCamera.transform : transform;
        }
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
        SpellData spell = loadout.SelectedSpell;

        if (spell == null)
        {
            Debug.Log("No spell equipped in the selected staff slot.");
            return;
        }

        Vector3 start = castOrigin.position;
        Vector3 direction = castOrigin.forward;
        Vector3 end = start + direction * range;

        if (Physics.Raycast(start, direction, out RaycastHit hit, range, hitLayers, QueryTriggerInteraction.Ignore))
        {
            end = hit.point;
            DamageHitTarget(spell, hit);
        }
        else
        {
            Debug.Log($"{spell.DisplayName} did not hit anything.");
        }

        if (drawBolt)
        {
            StartCoroutine(ShowBolt(GetBoltStartPosition(), end));
        }
    }

    private void DamageHitTarget(SpellData spell, RaycastHit hit)
    {
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            Debug.Log($"{spell.DisplayName} hit {hit.collider.name}, but it has no HealthComponent.");
            return;
        }

        Vector3 hitDirection = (hit.point - castOrigin.position).normalized;

        DamageInfo damageInfo = new DamageInfo(
            spell.Damage,
            gameObject,
            hit.point,
            hitDirection,
            spell.KnockbackForce,
            spell.DamageType
        );

        damageable.TakeDamage(damageInfo);
        Debug.Log($"{spell.DisplayName} dealt {spell.Damage} damage to {hit.collider.name}.");
    }

    private IEnumerator ShowBolt(Vector3 start, Vector3 end)
    {
        GameObject boltObject = new GameObject("Debug Test Bolt");
        LineRenderer lineRenderer = boltObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = boltWidth;
        lineRenderer.endWidth = boltWidth;
        lineRenderer.material = GetBoltMaterial();
        lineRenderer.startColor = boltColor;
        lineRenderer.endColor = boltColor;
        lineRenderer.alignment = LineAlignment.View;

        yield return new WaitForSeconds(boltDuration);

        Destroy(boltObject);
    }

    private Vector3 GetBoltStartPosition()
    {
        if (visualOrigin != null)
        {
            return visualOrigin.position;
        }

        return castOrigin.TransformPoint(visualOffsetFromCastOrigin);
    }

    private Material GetBoltMaterial()
    {
        if (boltMaterial != null)
        {
            return boltMaterial;
        }

        Shader shader = Shader.Find("Sprites/Default");
        boltMaterial = new Material(shader);
        boltMaterial.color = boltColor;
        return boltMaterial;
    }
}
