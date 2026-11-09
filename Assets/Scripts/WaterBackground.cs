using UnityEngine;

// Builds a grid of water tiles and keeps it under the camera, snapped to the
// tile size, so the water looks fixed in the world while you sail over it.
public class WaterBackground : MonoBehaviour
{
    public Sprite waterSprite;
    public int rangeX = 3;
    public int rangeY = 2;

    Transform cam;
    float tw, th;

    void Start()
    {
        cam = Camera.main.transform;
        tw = waterSprite.bounds.size.x;
        th = waterSprite.bounds.size.y;

        for (int x = -rangeX; x <= rangeX; x++)
        for (int y = -rangeY; y <= rangeY; y++)
        {
            var go = new GameObject("Water");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(x * tw, y * th, 0);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = waterSprite;
            sr.sortingOrder = -100;
        }
    }

    void LateUpdate()
    {
        transform.position = new Vector3(
            Mathf.Round(cam.position.x / tw) * tw,
            Mathf.Round(cam.position.y / th) * th, 0);
    }
}
