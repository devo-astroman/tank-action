using UnityEngine;

public sealed class FirerEnemy : MonoBehaviour
{
    private TankHealth tank;
    private EnemyAttack attack;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform cannon;
    [SerializeField] private Transform muzzle;
    [SerializeField] private AudioClip fireSound;
    private float nextFire;

    public void Initialize(TankHealth target, EnemyAttack health, Transform barrel)
    {
        tank = target;
        attack = health;
        if (cannon == null) cannon = barrel;
        nextFire = Time.time + 3f;
    }

    private void LateUpdate()
    {
        if (tank == null || !tank.IsAlive || attack == null || attack.HitsRemaining <= 0 || cannon == null) return;
        Vector3 direction = tank.transform.position - cannon.position;
        if (direction.sqrMagnitude < 0.001f) return;
        if (turret != null)
        {
            Vector3 heading = tank.transform.position - turret.position;
            heading.y = 0f;
            if (heading.sqrMagnitude > 0.001f) turret.rotation = Quaternion.LookRotation(heading);
        }
        direction = tank.transform.position - cannon.position;
        if (direction.sqrMagnitude > 0.001f) cannon.rotation = Quaternion.LookRotation(direction);
        if (Time.time < nextFire) return;
        nextFire = Time.time + 3f;
        Renderer barrel = cannon.GetComponent<Renderer>();
        float barrelLength = barrel != null ? barrel.bounds.extents.magnitude : 1f;
        Vector3 shotOrigin = muzzle != null ? muzzle.position : cannon.position + cannon.forward * (barrelLength + 0.2f);
        EnemyBullet.Spawn(shotOrigin, (tank.transform.position - shotOrigin).normalized, transform);
        GameSound.Play(fireSound);
    }
}
