using UnityEngine;

// Shared movement + firing, used by both the player and AI ships.
[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float turnSpeed = 120f;
    public float fireCooldown = 0.6f;
    public Projectile projectilePrefab;
    public Transform muzzle;

    Rigidbody2D rb;
    float nextFire;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        // Ships must be solid so they bump into islands and each other.
        foreach (var c in GetComponents<Collider2D>()) c.isTrigger = false;
    }

    public void Drive(float throttle, float turn)
    {
        rb.MoveRotation(rb.rotation + turn * turnSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = transform.up * (throttle * moveSpeed);
    }

    public void Fire()
    {
        if (Time.time < nextFire) return;
        nextFire = Time.time + fireCooldown;
        var p = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        p.ownerTag = tag;
    }
}
