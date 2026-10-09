using UnityEngine;

public sealed class TankProjectile : MonoBehaviour
{
    private Vector3 target;
    private float speed;

    public void Initialize(Vector3 destination, float moveSpeed)
    {
        target = destination;
        speed = moveSpeed;
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        Vector3 start = transform.position;
        Vector3 end = Vector3.MoveTowards(start, target, speed * Time.deltaTime);
        Vector3 travel = end - start;
        // Sweep the whole frame's travel so fast shots cannot skip an enemy.
        RaycastHit[] hits = Physics.SphereCastAll(start, 0.1f, travel.normalized, travel.magnitude,
            ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            EnemyAttack enemy = hit.collider.GetComponentInParent<EnemyAttack>();
            if (enemy == null || enemy.HitsRemaining <= 0) continue;
            enemy.ReceiveShot();
            Destroy(gameObject);
            return;
        }
        foreach (Collider overlap in Physics.OverlapSphere(end, 0.1f))
        {
            EnemyAttack enemy = overlap.GetComponentInParent<EnemyAttack>();
            if (enemy == null || enemy.HitsRemaining <= 0) continue;
            enemy.ReceiveShot();
            Destroy(gameObject);
            return;
        }
        transform.position = end;
        if ((transform.position - target).sqrMagnitude < 0.0001f) Destroy(gameObject);
    }
}
