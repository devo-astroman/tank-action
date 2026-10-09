using UnityEngine;

[DisallowMultipleComponent]
public sealed class TankHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0;
    private void Awake() => CurrentHealth = maxHealth;
    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - Mathf.Max(0, amount));
        if (!IsAlive)
        {
            GetComponent<CubeMovement>().enabled = false;
            GetComponent<TankFire>().enabled = false;
            Debug.Log("Tank destroyed. Restart Play mode to try again.");
        }
    }
}
