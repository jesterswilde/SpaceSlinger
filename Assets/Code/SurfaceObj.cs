using UnityEngine;

public class SurfaceObj: MonoBehaviour
{
    [SerializeField]
    float radius;
    public float Radius => radius;
    public bool CollidesWith(SurfaceObj other) =>
        Vector3.Distance(other.transform.position, transform.position) < radius + other.radius;
}
