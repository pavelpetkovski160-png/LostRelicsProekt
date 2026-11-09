using UnityEngine;

// Put on the PLAYER ship. Ramming an enemy ship destroys both and ends the game.
public class ShipCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D c)
    {
        if (!c.collider.CompareTag("Enemy")) return;

        Destroy(c.collider.gameObject);
        if (GameManager.Instance != null) GameManager.Instance.GameOver();
        Destroy(gameObject);
    }
}
