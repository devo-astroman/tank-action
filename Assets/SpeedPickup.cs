using UnityEngine;

public sealed class SpeedPickup : MonoBehaviour
{
    private bool consumed;
    private Material material;

    public static SpeedPickup Spawn(Collider floor)
    {
        Bounds bounds = floor.bounds;
        for (int attempt = 0; attempt < 30; attempt++)
        {
            Vector3 sample = new Vector3(Random.Range(bounds.min.x + 0.5f, bounds.max.x - 0.5f),
                bounds.max.y + 10f, Random.Range(bounds.min.z + 0.5f, bounds.max.z - 0.5f));
            if (!floor.Raycast(new Ray(sample, Vector3.down), out RaycastHit hit, 30f)) continue;
            Vector3 position = hit.point + Vector3.up * 0.35f;
            bool occupied = false;
            foreach (Collider overlap in Physics.OverlapBox(position, Vector3.one * 0.4f))
                if (overlap != floor) { occupied = true; break; }
            if (occupied) continue;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Double Speed Pickup";
            cube.transform.position = position;
            cube.transform.localScale = Vector3.one * 0.7f;
            var pickup = cube.AddComponent<SpeedPickup>();
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            pickup.material = new Material(shader) { color = Color.white };
            cube.GetComponent<Renderer>().sharedMaterial = pickup.material;
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
                tank.GetComponent<CubeMovement>().ApplyDoubleSpeed();
                Consume();
                return;
            }
            EnemyAttack enemy = collider.GetComponentInParent<EnemyAttack>();
            if (enemy != null && enemy.HitsRemaining > 0)
            {
                enemy.ApplyDoubleSpeed();
                Consume();
                return;
            }
        }
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
