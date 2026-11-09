using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 2;
    public float lifetime = 30f;

    void Start() => Destroy(gameObject, lifetime);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent(out Health hp)) return;
        if (hp.Current >= hp.maxHealth) return;   // keep it for later if you're at full health
        hp.Heal(healAmount);
        Destroy(gameObject);
    }
}
