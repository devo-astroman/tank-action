using UnityEngine;

[DisallowMultipleComponent]
public sealed class TankHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField] private AudioClip damageSound;
    public int CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0;
    private DamageFlash damageFlash;
    private void Awake()
    {
        CurrentHealth = maxHealth;
        damageFlash = GetComponent<DamageFlash>();
        if (damageFlash == null) damageFlash = gameObject.AddComponent<DamageFlash>();
    }
    public void TakeDamage(int amount)
    {
        if (!IsAlive || amount <= 0) return;
        CurrentHealth = Mathf.Max(0, CurrentHealth - Mathf.Max(0, amount));
        GameSound.Play(damageSound);
        damageFlash.Flash();
        if (!IsAlive)
        {
            GetComponent<CubeMovement>().enabled = false;
            GetComponent<TankFire>().enabled = false;
            Debug.Log("Tank destroyed. Restart Play mode to try again.");
        }
    }
}
