using MiscUtil.Linq.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orientation : MonoBehaviour
{
    static Orientation t;
    Queue<Vector3> gravVecs = new Queue<Vector3>();
    int maxGrav = 40;

    public static Transform Transform => t.transform;
    Vector3 forward;
    public static Vector3 Forward => t.forward;
    public static Vector3 Up => t.transform.up;
    Vector3 rawUp;
    public static Vector3 RawUp => t.rawUp;
    Vector3 right;
    public static Vector3 Right => t.right;
    private void FixedUpdate()
    {
        rawUp = Gravity.Orientation *-1;
        right = Vector3.Cross(rawUp, CameraController.Forward);
        forward = Vector3.Cross(right, rawUp);

        //gravVecs.Enqueue(Physics.gravity * -1);
        //if (gravVecs.Count > maxGrav)
        //    gravVecs.Dequeue();
        //rawUp = gravVecs.Sum().normalized;
        transform.up = rawUp;
        //var forward = Vector3.ProjectOnPlane(CameraController.Forward, rawUp);
        //transform.rotation = Quaternion.LookRotation(CameraController.Forward, rawUp);
    }
    private void Awake()
    {
        t = this;
    }

}
