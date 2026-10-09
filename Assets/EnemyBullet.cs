using UnityEngine;

public sealed class EnemyBullet : MonoBehaviour
{
    private Vector3 direction;
    private Transform owner;

    public static void Spawn(Vector3 position, Vector3 heading, Transform shooter)
    {
        GameObject bullet = new GameObject("Tall Tank Fireball");
        bullet.transform.position = position;
        bullet.transform.rotation = Quaternion.LookRotation(heading);
        bullet.AddComponent<FireballVfx>();
        EnemyBullet flight = bullet.AddComponent<EnemyBullet>();
        flight.direction = heading;
        flight.owner = shooter;
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

}
