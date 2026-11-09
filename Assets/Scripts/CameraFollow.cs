using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x, target.position.y, -10f);
    }
}
