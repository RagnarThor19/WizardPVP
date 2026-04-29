using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStaffVisual : MonoBehaviour
{
    [Header("Parent")]
    [SerializeField] private Transform viewRoot;

    [Header("View Position")]
    [SerializeField] private Vector3 localPosition = new Vector3(0.42f, -0.34f, 0.75f);
    [SerializeField] private Vector3 localRotation = new Vector3(58f, -8f, -12f);

    [Header("Placeholder Shape")]
    [SerializeField] private float staffLength = 0.95f;
    [SerializeField] private float staffThickness = 0.045f;
    [SerializeField] private float focusOrbSize = 0.16f;
    [SerializeField] private float castPointForwardOffset = 0.04f;
    [SerializeField] private Color woodColor = new Color(0.23f, 0.13f, 0.08f, 1f);
    [SerializeField] private Color focusColor = new Color(0.45f, 0.85f, 1f, 0.85f);

    private StaffCastPoint castPoint;
    private Material woodMaterial;
    private Material focusMaterial;

    public Transform CastPoint => castPoint != null ? castPoint.transform : null;

    private void Awake()
    {
        if (viewRoot == null)
        {
            Camera mainCamera = Camera.main;
            viewRoot = mainCamera != null ? mainCamera.transform : transform;
        }

        CreatePlaceholderStaff();
    }

    private void CreatePlaceholderStaff()
    {
        GameObject root = new GameObject("Placeholder Staff");
        root.transform.SetParent(viewRoot, false);
        root.transform.localPosition = localPosition;
        root.transform.localRotation = Quaternion.Euler(localRotation);

        GameObject rod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rod.name = "Staff Rod";
        rod.transform.SetParent(root.transform, false);
        rod.transform.localPosition = Vector3.zero;
        rod.transform.localRotation = Quaternion.identity;
        rod.transform.localScale = new Vector3(staffThickness, staffLength * 0.5f, staffThickness);
        rod.GetComponent<Renderer>().material = GetWoodMaterial();
        DisableCollider(rod);

        GameObject focus = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        focus.name = "Staff Focus";
        focus.transform.SetParent(root.transform, false);
        focus.transform.localPosition = GetStaffTipPosition() + Vector3.up * (focusOrbSize * 0.5f);
        focus.transform.localScale = Vector3.one * focusOrbSize;
        focus.GetComponent<Renderer>().material = GetFocusMaterial();
        DisableCollider(focus);

        GameObject castPointObject = new GameObject("StaffCastPoint");
        castPointObject.transform.SetParent(root.transform, false);
        castPointObject.transform.localPosition = focus.transform.localPosition + Vector3.forward * castPointForwardOffset;
        castPointObject.transform.localRotation = Quaternion.identity;
        castPoint = castPointObject.AddComponent<StaffCastPoint>();
    }

    private Vector3 GetStaffTipPosition()
    {
        return Vector3.up * (staffLength * 0.5f);
    }

    private void DisableCollider(GameObject target)
    {
        Collider targetCollider = target.GetComponent<Collider>();

        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }
    }

    private Material GetWoodMaterial()
    {
        if (woodMaterial != null)
        {
            return woodMaterial;
        }

        woodMaterial = new Material(GetDefaultShader());
        woodMaterial.color = woodColor;
        return woodMaterial;
    }

    private Material GetFocusMaterial()
    {
        if (focusMaterial != null)
        {
            return focusMaterial;
        }

        focusMaterial = new Material(GetDefaultShader());
        focusMaterial.color = focusColor;
        return focusMaterial;
    }

    private Shader GetDefaultShader()
    {
        Shader shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        }

        return shader;
    }
}
