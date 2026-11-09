using UnityEngine;

// Put on the ENEMY ship. Drops a mine behind it every few seconds.
public class MineDropper : MonoBehaviour
{
    public GameObject minePrefab;
    public Vector2 intervalRange = new Vector2(4f, 8f);
    public float dropDistance = 1.4f;

    float next;

    void Start() => ScheduleNext();

    void Update()
    {
        if (Time.time < next) return;
        ScheduleNext();
        Instantiate(minePrefab, transform.position - transform.up * dropDistance, Quaternion.identity);
    }

    void ScheduleNext() => next = Time.time + Random.Range(intervalRange.x, intervalRange.y);
}
