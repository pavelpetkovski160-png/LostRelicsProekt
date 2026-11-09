using UnityEngine;

// Put on ENEMY ships. They pick up artifacts and drop them all when sunk.
public class ArtifactCarrier : MonoBehaviour
{
    public GameObject artifactPrefab;
    public int Carried { get; private set; }

    SpriteRenderer marker;

    void Awake()
    {
        // Small gold dot at the stern, visible only while carrying something.
        var go = new GameObject("CarryMarker");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f, -0.55f, 0f);
        go.transform.localScale = Vector3.one * 0.4f;
        marker = go.AddComponent<SpriteRenderer>();
        marker.sprite = DotSprite.Get();
        marker.color = new Color(1f, 0.84f, 0.1f);
        marker.sortingOrder = 11;
        marker.enabled = false;
    }

    public void Pick()
    {
        Carried++;
        marker.enabled = true;
    }

    public void DropAll()
    {
        if (artifactPrefab == null) return;
        for (int i = 0; i < Carried; i++)
        {
            Vector3 offset = (Vector3)(Random.insideUnitCircle * 0.8f);
            Instantiate(artifactPrefab, transform.position + offset, Quaternion.identity);
        }
        Carried = 0;
    }
}
