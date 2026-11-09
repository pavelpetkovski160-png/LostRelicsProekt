using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int Current { get; private set; }
    bool dead;

    void Awake() => Current = maxHealth;

    public void Heal(int amount) => Current = Mathf.Min(maxHealth, Current + amount);

    public void TakeDamage(int amount)
    {
        if (dead) return;
        Current -= amount;
        if (Current > 0) return;

        dead = true;
        if (CompareTag("Enemy"))
        {
            if (TryGetComponent(out ArtifactCarrier carrier)) carrier.DropAll();
            if (GameManager.Instance != null) GameManager.Instance.AddScore(100);
        }
        if (CompareTag("Player") && GameManager.Instance != null) GameManager.Instance.GameOver();
        Destroy(gameObject);
    }
}
