using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 3f;
    public int damage = 1;
    [HideInInspector] public string ownerTag;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.up * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ownerTag)) return;

        if (other.TryGetComponent(out Health hp))
        {
            hp.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)   // islands and other solid things stop cannonballs
        {
            Destroy(gameObject);
        }
    }
}
