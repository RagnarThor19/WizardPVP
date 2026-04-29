using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerStaffSlash : MonoBehaviour
{
    [Header("Slash")]
    [SerializeField] private Transform slashOrigin;
    [SerializeField] private float range = 2.2f;
    [SerializeField] private float radius = 0.7f;
    [SerializeField] private LayerMask hitLayers = ~0;
    [SerializeField] private float cooldown = 0.75f;

    [Header("Impact")]
    [SerializeField] private float damage = 6f;
    [SerializeField] private float knockbackForce = 9f;
    [SerializeField] private float upwardKnockback = 0.12f;

    [Header("Debug Visual")]
    [SerializeField] private bool drawSlash = true;
    [SerializeField] private float visualDuration = 0.12f;
    [SerializeField] private Color slashColor = new Color(1f, 0.82f, 0.45f, 0.75f);

    private PlayerInputHandler input;
    private float cooldownRemaining;
    private Material slashMaterial;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();

        if (slashOrigin == null)
        {
            Camera mainCamera = Camera.main;
            slashOrigin = mainCamera != null ? mainCamera.transform : transform;
        }
    }

    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining = Mathf.Max(0f, cooldownRemaining - Time.deltaTime);
        }

        if (input.SwingPressed)
        {
            TrySlash();
        }
    }

    private void TrySlash()
    {
        if (cooldownRemaining > 0f)
        {
            Debug.Log($"Staff slash is on cooldown for {cooldownRemaining:0.0}s.");
            return;
        }

        cooldownRemaining = cooldown;

        Vector3 start = slashOrigin.position;
        Vector3 direction = slashOrigin.forward;
        Vector3 end = start + direction * range;

        RaycastHit[] hits = Physics.SphereCastAll(
            start,
            radius,
            direction,
            range,
            hitLayers,
            QueryTriggerInteraction.Ignore
        );

        DamageTargets(hits, direction);

        if (drawSlash)
        {
            StartCoroutine(ShowSlash(start, end));
        }
    }

    private void DamageTargets(RaycastHit[] hits, Vector3 direction)
    {
        HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hitCollider = hits[i].collider;

            if (hitCollider.transform.root == transform.root)
            {
                continue;
            }

            IDamageable damageable = hitCollider.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                continue;
            }

            GameObject targetObject = GetTargetGameObject(hitCollider, damageable);

            if (targetObject == null || !damagedObjects.Add(targetObject))
            {
                continue;
            }

            Vector3 knockbackDirection = (direction + Vector3.up * upwardKnockback).normalized;
            Vector3 hitPoint = hits[i].point != Vector3.zero ? hits[i].point : hitCollider.transform.position;

            DamageInfo damageInfo = new DamageInfo(
                damage,
                gameObject,
                hitPoint,
                knockbackDirection,
                knockbackForce,
                DamageType.Melee
            );

            damageable.TakeDamage(damageInfo);
            Debug.Log($"Staff slash dealt {damage} damage to {hitCollider.name}.");
        }
    }

    private GameObject GetTargetGameObject(Collider hitCollider, IDamageable damageable)
    {
        ICombatTarget combatTarget = damageable as ICombatTarget;

        if (combatTarget != null)
        {
            return combatTarget.TargetGameObject;
        }

        Component damageableComponent = damageable as Component;

        if (damageableComponent != null)
        {
            return damageableComponent.gameObject;
        }

        return hitCollider.gameObject;
    }

    private IEnumerator ShowSlash(Vector3 start, Vector3 end)
    {
        GameObject slashObject = new GameObject("Debug Staff Slash");
        LineRenderer lineRenderer = slashObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = radius * 1.4f;
        lineRenderer.endWidth = radius * 0.45f;
        lineRenderer.material = GetSlashMaterial();
        lineRenderer.startColor = slashColor;
        lineRenderer.endColor = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);
        lineRenderer.alignment = LineAlignment.View;

        yield return new WaitForSeconds(visualDuration);

        Destroy(slashObject);
    }

    private Material GetSlashMaterial()
    {
        if (slashMaterial != null)
        {
            return slashMaterial;
        }

        Shader shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        }

        slashMaterial = new Material(shader);
        slashMaterial.color = slashColor;
        return slashMaterial;
    }
}
