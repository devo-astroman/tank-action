using UnityEngine;

public sealed class EnemyBullet : MonoBehaviour
{
    private Vector3 direction;
    private Transform owner;
    private Material material;

    public static void Spawn(Vector3 position, Vector3 heading, Transform shooter)
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bullet.name = "Enemy Bullet";
        bullet.transform.position = position;
        bullet.transform.localScale = Vector3.one * 0.2f;
        bullet.GetComponent<Collider>().enabled = false;
        Renderer renderer = bullet.GetComponent<Renderer>();
        Material material = new Material(renderer.sharedMaterial);
        material.color = new Color(1f, 0.3f, 0f);
        renderer.sharedMaterial = material;
        EnemyBullet flight = bullet.AddComponent<EnemyBullet>();
        flight.direction = heading;
        flight.owner = shooter;
        flight.material = material;
        Destroy(bullet, 8f);
    }

    private void Update()
    {
        Vector3 step = direction * (10f * Time.deltaTime);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.1f, direction, step.magnitude, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            if (owner != null && hit.transform.IsChildOf(owner)) continue;
            TankHealth tank = hit.collider.GetComponentInParent<TankHealth>();
            if (tank != null) tank.TakeDamage(10);
            Destroy(gameObject);
            return;
        }
        transform.position += step;
    }

    private void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
