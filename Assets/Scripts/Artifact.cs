using System.Collections.Generic;
using UnityEngine;

// An ancient artifact lying on the water. The player scores it; enemy ships carry it.
public class Artifact : MonoBehaviour
{
    // Every artifact currently in the world (enemies look through this list).
    public static readonly List<Artifact> All = new List<Artifact>();

    bool taken;

    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (taken) return;

        if (other.CompareTag("Player"))
        {
            taken = true;
            if (GameManager.Instance != null) GameManager.Instance.AddArtifact();
            Destroy(gameObject);
        }
        else if (other.TryGetComponent(out ArtifactCarrier carrier))
        {
            taken = true;
            carrier.Pick();
            Destroy(gameObject);
        }
    }
}
