using System.Collections.Generic;
using UnityEngine;

// Scatters islands around the player (and removes far ones), and drops
// health pickups and ancient artifacts.
public class WorldSpawner : MonoBehaviour
{
    [Header("Islands")]
    public GameObject[] islandPrefabs;
    public int islandTarget = 14;
    public float minDist = 14f;
    public float maxDist = 28f;
    public float despawnDist = 40f;

    [Header("Health pickups")]
    public GameObject pickupPrefab;
    public float pickupInterval = 10f;
    public int maxPickups = 3;

    [Header("Artifacts")]
    public GameObject artifactPrefab;
    public float artifactInterval = 8f;
    public int maxArtifacts = 3;

    Transform player;
    readonly List<GameObject> islands = new List<GameObject>();
    readonly List<GameObject> pickups = new List<GameObject>();
    float nextPickup, nextArtifact;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        if (!player) return;
        for (int i = 0; i < islandTarget; i++) TrySpawnIsland(7f, maxDist);
        nextPickup = Time.time + 4f;
        nextArtifact = Time.time + 2f;
    }

    void Update()
    {
        if (!player) return;

        for (int i = islands.Count - 1; i >= 0; i--)
        {
            if (islands[i] == null) { islands.RemoveAt(i); continue; }
            if (Vector2.Distance(islands[i].transform.position, player.position) > despawnDist)
            {
                Destroy(islands[i]);
                islands.RemoveAt(i);
            }
        }
        while (islands.Count < islandTarget && TrySpawnIsland(minDist, maxDist)) { }

        if (pickupPrefab != null && Time.time >= nextPickup)
        {
            nextPickup = Time.time + pickupInterval;
            pickups.RemoveAll(x => x == null);
            if (pickups.Count < maxPickups) TrySpawnAtFreeSpot(pickupPrefab, 8f, 18f, pickups);
        }

        if (artifactPrefab != null && Time.time >= nextArtifact)
        {
            nextArtifact = Time.time + artifactInterval;
            if (CountNearbyArtifacts() < maxArtifacts) SpawnArtifact();
        }
    }

    // Half the time an artifact appears near a random enemy, so enemies actually get to steal them.
    void SpawnArtifact()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 pos;
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length > 0 && Random.value < 0.5f)
            {
                var e = enemies[Random.Range(0, enemies.Length)];
                pos = (Vector2)e.transform.position + Random.insideUnitCircle.normalized * Random.Range(5f, 9f);
            }
            else pos = RandomRingPoint(10f, 22f);

            if (NearIsland(pos, 4.5f)) continue;
            Instantiate(artifactPrefab, pos, Quaternion.identity);
            return;
        }
    }

    int CountNearbyArtifacts()
    {
        int n = 0;
        foreach (var a in Artifact.All)
            if (a != null && Vector2.Distance(a.transform.position, player.position) < despawnDist) n++;
        return n;
    }

    Vector2 RandomRingPoint(float min, float max) =>
        (Vector2)player.position + Random.insideUnitCircle.normalized * Random.Range(min, max);

    bool NearIsland(Vector2 pos, float dist)
    {
        foreach (var isl in islands)
            if (isl != null && Vector2.Distance(isl.transform.position, pos) < dist) return true;
        return false;
    }

    bool TrySpawnIsland(float min, float max)
    {
        if (islandPrefabs == null || islandPrefabs.Length == 0) return false;
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 pos = RandomRingPoint(min, max);
            if (NearIsland(pos, 7f)) continue;
            var prefab = islandPrefabs[Random.Range(0, islandPrefabs.Length)];
            islands.Add(Instantiate(prefab, pos, Quaternion.Euler(0, 0, Random.Range(0f, 360f))));
            return true;
        }
        return false;
    }

    void TrySpawnAtFreeSpot(GameObject prefab, float min, float max, List<GameObject> track)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 pos = RandomRingPoint(min, max);
            if (NearIsland(pos, 4.5f)) continue;
            var go = Instantiate(prefab, pos, Quaternion.identity);
            if (track != null) track.Add(go);
            return;
        }
    }
}
