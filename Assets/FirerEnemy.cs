using UnityEngine;

public sealed class FirerEnemy : MonoBehaviour
{
    private TankHealth tank;
    private EnemyAttack attack;
    private Transform cannon;
    private float nextFire;

    public void Initialize(TankHealth target, EnemyAttack health, Transform barrel)
    {
        tank = target;
        attack = health;
        cannon = barrel;
        nextFire = Time.time + 3f;
    }

    private void LateUpdate()
    {
        if (tank == null || !tank.IsAlive || attack == null || attack.HitsRemaining <= 0 || cannon == null) return;
        Vector3 direction = tank.transform.position - cannon.position;
        if (direction.sqrMagnitude < 0.001f) return;
        cannon.rotation = Quaternion.LookRotation(direction);
        if (Time.time < nextFire) return;
        nextFire = Time.time + 3f;
        Renderer barrel = cannon.GetComponent<Renderer>();
        float barrelLength = barrel != null ? barrel.bounds.extents.magnitude : 1f;
        Vector3 muzzle = cannon.position + cannon.forward * (barrelLength + 0.2f);
        EnemyBullet.Spawn(muzzle, (tank.transform.position - muzzle).normalized, transform);
    }
}
