using UnityEngine;

public class Mine : MonoBehaviour
{
    public float lifetime = 40f;

    void Start() => Destroy(gameObject, lifetime);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.TryGetComponent(out Health hp)) hp.TakeDamage(9999); // instant loss
        Destroy(gameObject);
    }
}
