using UnityEngine;

public class Orientation : MonoBehaviour
{
    static Orientation t;

    public static Transform Transform => t.transform;
    public static Vector3 Forward => t.transform.forward;
    public static Vector3 Up => t.transform.up;
    public static Vector3 Right => t.transform.right;
    private void FixedUpdate()
    {
        var forward = Vector3.ProjectOnPlane(CameraController.Forward, Gravity.Orientation * -1);
        transform.rotation = Quaternion.LookRotation(forward, Gravity.Orientation * -1);
    }
    private void Awake()
    {
        t = this;
    }

}
