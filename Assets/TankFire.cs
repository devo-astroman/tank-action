using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class TankFire : MonoBehaviour
{
    [SerializeField] private Transform cannon;
    [SerializeField] private Collider floor;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 15f;
    [SerializeField, Min(0.05f)] private float fireInterval = 0.2f;
    private Vector3 pivot;
    private float barrelOffset;
    private float nextFire;
    private Material projectileMaterial;
    private Material missileMaterial;
    public bool BlueMissileSelected { get; private set; }
    [SerializeField, Min(0.1f)] private float missileBlastRadius = 3f;

    private void Awake()
    {
        if (cannon == null) { enabled = false; return; }
        pivot = cannon.localPosition;
        barrelOffset = pivot.z;
        pivot.z = 0f;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        projectileMaterial = new Material(shader) { color = Color.red };
        missileMaterial = new Material(shader) { color = Color.blue };
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.digit1Key.wasPressedThisFrame) BlueMissileSelected = false;
            if (keyboard.digit2Key.wasPressedThisFrame) BlueMissileSelected = true;
        }
        Mouse mouse = Mouse.current;
        Camera camera = Camera.main;
        if (mouse == null || camera == null || floor == null) return;
        Ray ray = camera.ScreenPointToRay(mouse.position.ReadValue());
        if (!floor.Raycast(ray, out RaycastHit hit, 1000f)) return;
        Vector3 origin = transform.TransformPoint(pivot);
        Vector3 direction = hit.point - origin;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;
        cannon.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        cannon.position = origin + cannon.forward * barrelOffset * transform.lossyScale.z;
        if (mouse.leftButton.wasPressedThisFrame && Time.time >= nextFire)
        {
            nextFire = Time.time + fireInterval;
            FireAt(hit.point);
        }
    }

    public void FireAt(Vector3 target)
    {
        Vector3 muzzle = cannon.position + cannon.forward * (Mathf.Abs(cannon.lossyScale.z) * 0.5f + 0.2f);
        GameObject shot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shot.name = BlueMissileSelected ? "Blue Missile" : "Red Projectile";
        shot.transform.position = muzzle;
        shot.transform.localScale = Vector3.one * 0.2f;
        shot.GetComponent<Renderer>().sharedMaterial = BlueMissileSelected ? missileMaterial : projectileMaterial;
        Destroy(shot.GetComponent<Collider>());
        if (BlueMissileSelected)
            shot.AddComponent<BlueMissile>().Initialize(target + Vector3.up * 0.1f, projectileSpeed, floor, missileBlastRadius);
        else shot.AddComponent<TankProjectile>().Initialize(target + Vector3.up * 0.1f, projectileSpeed);
    }

    private void OnDestroy()
    {
        if (projectileMaterial != null) Destroy(projectileMaterial);
        if (missileMaterial != null) Destroy(missileMaterial);
    }
}
