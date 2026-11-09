using UnityEngine;

// Spawns enemy ships in a ring around the PLAYER and removes ones left far behind,
// so there are always enemies near you no matter how far you sail.
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float interval = 4f;
    public int maxNearby = 6;          // max enemies close to the player
    public float minSpawnDist = 14f;
    public float maxSpawnDist = 20f;
    public float despawnDist = 45f;    // enemies farther than this are removed

    Transform player;
    float next;

    void Update()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) return;
            player = p.transform;
        }

        if (Time.time < next) return;
        next = Time.time + interval;

        // Remove enemies left far behind, count the ones nearby
        int nearby = 0;
        foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Vector2.Distance(e.transform.position, player.position) > despawnDist) Destroy(e);
            else nearby++;
        }
        if (nearby >= maxNearby) return;

        // Pick a free spot (not inside an island)
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 pos = (Vector2)player.position +
                          Random.insideUnitCircle.normalized * Random.Range(minSpawnDist, maxSpawnDist);
            if (IsBlocked(pos)) continue;
            Instantiate(enemyPrefab, pos, Quaternion.identity);
            return;
        }
    }

    bool IsBlocked(Vector2 pos)
    {
        foreach (var hit in Physics2D.OverlapCircleAll(pos, 1.5f))
            if (!hit.isTrigger) return true;
        return false;
    }
}
