using System.Collections.Generic;
using UnityEngine;

public sealed class BlueMissile : MonoBehaviour
{
    private Vector3 target;
    private float speed;
    private float blastRadius;
    private Collider floor;
    private bool exploded;

    public void Initialize(Vector3 destination, float moveSpeed, Collider ground, float radius)
    {
        target = destination;
        speed = moveSpeed;
        floor = ground;
        blastRadius = radius;
        Destroy(gameObject, 10f);
    }

    private bool IsImpact(Collider collider)
    {
        if (collider == floor) return true;
        EnemyAttack enemy = collider.GetComponentInParent<EnemyAttack>();
        return enemy != null && enemy.HitsRemaining > 0;
    }

    private void Update()
    {
        if (exploded) return;
        Vector3 start = transform.position;
        Vector3 end = Vector3.MoveTowards(start, target, speed * Time.deltaTime);
        Vector3 travel = end - start;
        RaycastHit[] hits = Physics.SphereCastAll(start, 0.1f, travel.normalized, travel.magnitude,
            ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            if (!IsImpact(hit.collider)) continue;
            Explode(hit.point);
            return;
        }
        foreach (Collider overlap in Physics.OverlapSphere(end, 0.1f, ~0, QueryTriggerInteraction.Collide))
        {
            if (!IsImpact(overlap)) continue;
            Explode(end);
            return;
        }
        transform.position = end;
        if ((end - target).sqrMagnitude < 0.0001f) Explode(end);
    }

    private void Explode(Vector3 center)
    {
        if (exploded) return;
        exploded = true;
        var damaged = new HashSet<EnemyAttack>();
        foreach (Collider collider in Physics.OverlapSphere(center, blastRadius, ~0, QueryTriggerInteraction.Collide))
        {
            EnemyAttack enemy = collider.GetComponentInParent<EnemyAttack>();
            if (enemy != null && enemy.HitsRemaining > 0 && damaged.Add(enemy)) enemy.ReceiveShot();
        }
        // A brief expanding blue ring shows the spherical blast's floor footprint.
        GameObject effect = new GameObject("Missile Blast");
        effect.transform.position = center;
        var line = effect.AddComponent<LineRenderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
        Material material = new Material(shader) { color = Color.cyan };
        line.sharedMaterial = material;
        line.loop = true;
        line.useWorldSpace = false;
        line.widthMultiplier = 0.06f;
        line.positionCount = 64;
        for (int i = 0; i < 64; i++)
        {
            float angle = i * Mathf.PI * 2f / 64;
            line.SetPosition(i, new Vector3(Mathf.Cos(angle) * blastRadius, 0.12f, Mathf.Sin(angle) * blastRadius));
        }
        Destroy(effect, 0.25f);
        Destroy(material, 0.3f);
        Destroy(gameObject);
    }
}
