using System.Collections.Generic;
using UnityEngine;

public sealed class AcidPuddle : MonoBehaviour
{
    private const float Radius = 2f;
    private readonly Dictionary<EnemyAttack, float> enemyTicks = new Dictionary<EnemyAttack, float>();
    private readonly Dictionary<TankHealth, float> tankTicks = new Dictionary<TankHealth, float>();
    private Material material;

    public static AcidPuddle Create(Vector3 floorPoint)
    {
        GameObject puddle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        puddle.name = "Acid Puddle";
        puddle.transform.position = floorPoint + Vector3.up * 0.025f;
        puddle.transform.localScale = new Vector3(Radius * 2f, 0.025f, Radius * 2f);
        puddle.GetComponent<Collider>().enabled = false;
        Destroy(puddle.GetComponent<Collider>());
        AcidPuddle acid = puddle.AddComponent<AcidPuddle>();
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
        acid.material = new Material(shader) { color = new Color(0.8f, 1f, 0.05f) };
        puddle.GetComponent<Renderer>().sharedMaterial = acid.material;
        Destroy(puddle, 5f);
        return acid;
    }

    private void Update()
    {
        var enemies = new HashSet<EnemyAttack>();
        var tanks = new HashSet<TankHealth>();
        foreach (Collider collider in Physics.OverlapSphere(transform.position, Radius + 1f))
        {
            EnemyAttack enemy = collider.GetComponentInParent<EnemyAttack>();
            TankHealth tank = collider.GetComponentInParent<TankHealth>();
            if (enemy == null && tank == null) continue;
            // Test contact with the thin circular floor area, rather than a tall sphere.
            Vector3 closest = collider.ClosestPoint(transform.position);
            Vector3 offset = closest - transform.position;
            if (Mathf.Abs(offset.y) > 0.15f) continue;
            offset.y = 0f;
            if (offset.sqrMagnitude > Radius * Radius) continue;
            if (enemy != null) enemies.Add(enemy);
            if (tank != null) tanks.Add(tank);
        }
        foreach (EnemyAttack enemy in enemies)
        {
            if (enemy.HitsRemaining <= 0) continue;
            if (enemyTicks.TryGetValue(enemy, out float next) && Time.time < next) continue;
            enemyTicks[enemy] = Time.time + 1f;
            enemy.ReceiveShot();
        }
        foreach (TankHealth tank in tanks)
        {
            if (!tank.IsAlive) continue;
            if (tankTicks.TryGetValue(tank, out float next) && Time.time < next) continue;
            tankTicks[tank] = Time.time + 1f;
            tank.TakeDamage(10);
        }
    }

    private void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
