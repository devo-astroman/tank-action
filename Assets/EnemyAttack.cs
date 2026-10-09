using UnityEngine;

public sealed class EnemyAttack : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 2f;
    [SerializeField, Min(1)] private int contactDamage = 10;
    [SerializeField, Min(0.1f)] private float damageInterval = 1f;
    public int HitsRemaining { get; private set; } = 3;
    private TankHealth tank;
    private Collider tankCollider;
    private Collider body;
    private float nextDamage;
    private DamageFlash damageFlash;

    public void Initialize(TankHealth target)
    {
        tank = target;
        tankCollider = target.GetComponent<Collider>();
        body = GetComponent<Collider>();
        damageFlash = GetComponent<DamageFlash>();
        if (damageFlash == null) damageFlash = gameObject.AddComponent<DamageFlash>();
    }

    private void Update()
    {
        if (tank == null || !tank.IsAlive || HitsRemaining <= 0) return;
        Vector3 destination = tank.transform.position;
        destination.y = transform.position.y;
        Vector3 direction = destination - transform.position;
        if (direction.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(direction);
        // Stop at the tank's bounds instead of passing through it.
        Bounds ownBounds = body.bounds;
        Bounds targetBounds = tankCollider.bounds;
        ownBounds.Expand(0.05f);
        if (ownBounds.Intersects(targetBounds))
        {
            if (Time.time >= nextDamage)
            {
                nextDamage = Time.time + damageInterval;
                tank.TakeDamage(contactDamage);
            }
        }
        else transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    public void ReceiveShot()
    {
        if (HitsRemaining <= 0) return;
        HitsRemaining--;
        damageFlash.Flash();
        if (HitsRemaining == 0)
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
            Destroy(gameObject, 0.18f);
        }
    }
}
