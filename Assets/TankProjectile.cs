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
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if ((transform.position - target).sqrMagnitude < 0.0001f) Destroy(gameObject);
    }
}
