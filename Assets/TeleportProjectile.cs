using UnityEngine;

public sealed class TeleportProjectile : MonoBehaviour
{
    private Vector3 target;
    private Vector3 destination;
    private float speed;
    private Collider floor;
    private bool finished;

    public void Initialize(Vector3 aim, float moveSpeed, Vector3 markedPoint, Collider ground)
    {
        target = aim;
        speed = moveSpeed;
        destination = markedPoint;
        floor = ground;
        Destroy(gameObject, 10f);
    }

    private bool Impact(Collider collider)
    {
        EnemyAttack enemy = collider.GetComponentInParent<EnemyAttack>();
        if (enemy != null && enemy.HitsRemaining > 0)
        {
            // Preserve the prefab's floor clearance and teleport the whole enemy hierarchy.
            float clearance = enemy.transform.position.y - floor.bounds.max.y;
            enemy.transform.position = new Vector3(destination.x, destination.y + clearance, destination.z);
            Physics.SyncTransforms();
            finished = true;
            Destroy(gameObject);
            return true;
        }
        if (collider != floor) return false;
        finished = true;
        Destroy(gameObject);
        return true;
    }

    private void Update()
    {
        if (finished) return;
        Vector3 start = transform.position;
        Vector3 end = Vector3.MoveTowards(start, target, speed * Time.deltaTime);
        Vector3 travel = end - start;
        RaycastHit[] hits = Physics.SphereCastAll(start, 0.1f, travel.normalized, travel.magnitude,
            ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) if (Impact(hit.collider)) return;
        foreach (Collider collider in Physics.OverlapSphere(end, 0.1f)) if (Impact(collider)) return;
        transform.position = end;
        if ((end - target).sqrMagnitude < 0.0001f) Destroy(gameObject);
    }
}
