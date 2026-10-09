using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class TankFire : MonoBehaviour
{
    [SerializeField] private Transform cannon;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Collider floor;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 15f;
    [SerializeField, Min(0.05f)] private float fireInterval = 0.2f;
    private Vector3 pivot;
    private float barrelOffset;
    private float nextFire;
    private float fireRateBoostUntil;
    public float CurrentFireInterval => fireInterval / (Time.time < fireRateBoostUntil ? 2f : 1f);
    public void ApplyDoubleFireRate() => fireRateBoostUntil = Time.time + 10f;
    private Material projectileMaterial;
    private Material missileMaterial;
    public int SelectedWeapon { get; private set; } = 1;
    public bool BlueMissileSelected => SelectedWeapon == 2;
    private Material acidMaterial;
    private Material teleportMaterial;
    private GameObject teleportMarker;
    public bool HasTeleportMark { get; private set; }
    public Vector3 TeleportPoint { get; private set; }
    [SerializeField, Min(0.1f)] private float missileBlastRadius = 3f;

    private void Awake()
    {
        if (cannon == null) { enabled = false; return; }
        pivot = cannon.localPosition;
        if (muzzle == null)
        {
        barrelOffset = pivot.z;
        pivot.z = 0f;
        }
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        projectileMaterial = new Material(shader) { color = Color.red };
        missileMaterial = new Material(shader) { color = Color.blue };
        acidMaterial = new Material(shader) { color = Color.yellow };
        teleportMaterial = new Material(shader) { color = Color.green };
    }

    private void LateUpdate()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.digit1Key.wasPressedThisFrame) SelectedWeapon = 1;
            if (keyboard.digit2Key.wasPressedThisFrame) SelectedWeapon = 2;
            if (keyboard.digit3Key.wasPressedThisFrame) SelectedWeapon = 3;
            if (keyboard.digit4Key.wasPressedThisFrame) SelectedWeapon = 4;
        }
        Mouse mouse = Mouse.current;
        Camera camera = Camera.main;
        if (mouse == null || camera == null || floor == null) return;
        Ray ray = camera.ScreenPointToRay(mouse.position.ReadValue());
        if (!floor.Raycast(ray, out RaycastHit hit, 1000f)) return;
        if (SelectedWeapon == 4 && mouse.rightButton.wasPressedThisFrame) PlaceTeleportMark(hit.point);
        Vector3 origin = cannon.parent.TransformPoint(pivot);
        Vector3 direction = hit.point - origin;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;
        cannon.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        if (muzzle == null) cannon.position = origin + cannon.forward * barrelOffset * transform.lossyScale.z;
        if (mouse.leftButton.isPressed && Time.time >= nextFire && (SelectedWeapon != 4 || HasTeleportMark))
        {
            nextFire = Time.time + CurrentFireInterval;
            FireAt(hit.point);
        }
    }

    public void FireAt(Vector3 target)
    {
        if (SelectedWeapon == 4 && !HasTeleportMark) return;
        Vector3 shotOrigin = muzzle != null ? muzzle.position : cannon.position + cannon.forward * (Mathf.Abs(cannon.lossyScale.z) * 0.5f + 0.2f);
        GameObject shot = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shot.name = SelectedWeapon == 4 ? "Green Teleport Bullet" : SelectedWeapon == 3 ? "Yellow Acid Bullet" : BlueMissileSelected ? "Blue Missile" : "Red Projectile";
        shot.transform.position = shotOrigin;
        shot.transform.localScale = Vector3.one * 0.2f;
        shot.GetComponent<Renderer>().sharedMaterial = SelectedWeapon == 4 ? teleportMaterial : SelectedWeapon == 3 ? acidMaterial : BlueMissileSelected ? missileMaterial : projectileMaterial;
        Destroy(shot.GetComponent<Collider>());
        if (SelectedWeapon == 4)
            shot.AddComponent<TeleportProjectile>().Initialize(target + Vector3.up * 0.1f, projectileSpeed, TeleportPoint, floor);
        else if (SelectedWeapon != 1)
            shot.AddComponent<BlueMissile>().Initialize(target + Vector3.up * 0.1f, projectileSpeed, floor, missileBlastRadius, SelectedWeapon == 3);
        else shot.AddComponent<TankProjectile>().Initialize(target + Vector3.up * 0.1f, projectileSpeed);
    }

    public void PlaceTeleportMark(Vector3 point)
    {
        HasTeleportMark = true;
        TeleportPoint = point;
        if (teleportMarker == null)
        {
            teleportMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            teleportMarker.name = "Teleport Mark";
            teleportMarker.GetComponent<Collider>().enabled = false;
            Destroy(teleportMarker.GetComponent<Collider>());
            teleportMarker.GetComponent<Renderer>().sharedMaterial = teleportMaterial;
            teleportMarker.transform.localScale = new Vector3(1.2f, 0.025f, 1.2f);
        }
        teleportMarker.transform.position = point + Vector3.up * 0.03f;
    }

    private void OnDestroy()
    {
        if (projectileMaterial != null) Destroy(projectileMaterial);
        if (missileMaterial != null) Destroy(missileMaterial);
        if (acidMaterial != null) Destroy(acidMaterial);
        if (teleportMarker != null) Destroy(teleportMarker);
        if (teleportMaterial != null) Destroy(teleportMaterial);
    }
}
