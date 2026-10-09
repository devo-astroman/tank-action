using UnityEngine;

public sealed class SpeedPickup : MonoBehaviour
{
    private bool consumed;
    private Material material;
    public enum PowerUpType { DoubleSpeed, DoubleFireRate }
    public PowerUpType Type { get; private set; }
    private Transform billboard;

    public static SpeedPickup Spawn(Collider floor, PowerUpType? forcedType = null)
    {
        GameObject regionObject = GameObject.Find("Regions/Powerups");
        if (regionObject == null)
        {
            Debug.LogWarning("Power-up spawning requires Regions/Powerups region cubes.");
            return null;
        }
        Transform regionRoot = regionObject.transform;
        var regions = new System.Collections.Generic.List<BoxCollider>();
        foreach (Transform child in regionRoot)
        {
            BoxCollider box = child.GetComponent<BoxCollider>();
            if (box != null) regions.Add(box);
        }
        if (regions.Count == 0) return null;
        for (int attempt = 0; attempt < 100; attempt++)
        {
            BoxCollider region = regions[Random.Range(0, regions.Count)];
            Vector3 half = region.size * 0.5f;
            Vector3 scale = region.transform.lossyScale;
            float marginX = 0.5f / Mathf.Max(Mathf.Abs(scale.x), 0.001f);
            float marginZ = 0.5f / Mathf.Max(Mathf.Abs(scale.z), 0.001f);
            if (half.x <= marginX || half.z <= marginZ) continue;
            Vector3 local = region.center + new Vector3(Random.Range(-half.x + marginX, half.x - marginX),
                0f, Random.Range(-half.z + marginZ, half.z - marginZ));
            Vector3 sample = region.transform.TransformPoint(local);
            sample.y = floor.bounds.max.y + 10f;
            if (!floor.Raycast(new Ray(sample, Vector3.down), out RaycastHit hit, 30f)) continue;
            Vector3 position = hit.point + Vector3.up * 0.35f;
            // Check all four floor corners against the oriented region footprint.
            bool inside = true;
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 corner = region.transform.InverseTransformPoint(hit.point + new Vector3(x * 0.35f, 0f, z * 0.35f)) - region.center;
                    if (Mathf.Abs(corner.x) > half.x || Mathf.Abs(corner.z) > half.z) inside = false;
                }
            if (!inside) continue;
            bool occupied = false;
            foreach (Collider overlap in Physics.OverlapBox(position, Vector3.one * 0.4f))
                if (overlap != floor && !overlap.transform.IsChildOf(regionRoot)) { occupied = true; break; }
            if (occupied) continue;
            GameObject cube = new GameObject("Power Up");
            cube.AddComponent<BoxCollider>();
            cube.name = "Double Speed Pickup";
            cube.transform.position = position;
            cube.transform.localScale = Vector3.one * 0.7f;
            var pickup = cube.AddComponent<SpeedPickup>();
            pickup.Type = forcedType ?? (Random.value < 0.5f ? PowerUpType.DoubleSpeed : PowerUpType.DoubleFireRate);
            cube.name = pickup.Type == PowerUpType.DoubleSpeed ? "Double Speed Pickup" : "Double Fire Rate Pickup";
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            GameObject model = Resources.Load<GameObject>("Powerups/WoodenBoxPickupModel");
            if (model != null) Instantiate(model, cube.transform, false);
            var labelObject = new GameObject("Power Up Label");
            labelObject.transform.SetParent(cube.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            var text = labelObject.AddComponent<TMPro.TextMeshPro>();
            text.font = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            text.text = pickup.Type == PowerUpType.DoubleSpeed ? "speed x2" : "fire rate x2";
            text.fontSize = 6f;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            text.color = Color.white;
            text.outlineWidth = 0.2f;
            text.outlineColor = Color.black;
            text.rectTransform.sizeDelta = new Vector2(4.8f, 1.2f);
            GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
            background.name = "Label Background";
            background.transform.SetParent(labelObject.transform, false);
            background.transform.localPosition = new Vector3(0f, 0f, 0.02f);
            background.transform.localScale = new Vector3(4.8f, 0.9f, 1f);
            background.GetComponent<Collider>().enabled = false;
            Destroy(background.GetComponent<Collider>());
            pickup.material = new Material(shader) { color = Color.black };
            background.GetComponent<Renderer>().sharedMaterial = pickup.material;
            pickup.billboard = labelObject.transform;
            Destroy(cube, 3f);
            return pickup;
        }
        return null;
    }

    private void Update()
    {
        if (consumed) return;
        foreach (Collider collider in Physics.OverlapBox(transform.position, Vector3.one * 0.36f))
        {
            TankHealth tank = collider.GetComponentInParent<TankHealth>();
            if (tank != null && tank.IsAlive)
            {
                if (Type == PowerUpType.DoubleSpeed) tank.GetComponent<CubeMovement>().ApplyDoubleSpeed();
                else tank.GetComponent<TankFire>().ApplyDoubleFireRate();
                Consume();
                return;
            }
            EnemyAttack enemy = collider.GetComponentInParent<EnemyAttack>();
            if (enemy != null && enemy.HitsRemaining > 0)
            {
                if (Type != PowerUpType.DoubleSpeed) continue;
                enemy.ApplyDoubleSpeed();
                Consume();
                return;
            }
        }
    }

    private void LateUpdate()
    {
        if (billboard != null && Camera.main != null) billboard.rotation = Camera.main.transform.rotation;
    }

    public static bool DestroyFromShot(Collider collider)
    {
        SpeedPickup pickup = collider.GetComponent<SpeedPickup>();
        if (pickup == null || pickup.consumed) return false;
        pickup.Consume();
        return true;
    }

    private void Consume()
    {
        if (consumed) return;
        consumed = true;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
