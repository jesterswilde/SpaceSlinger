using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkCamera : CameraSystem
{
    [SerializeField]
    float rotSpeed;

    public override void ControlCamera(CameraController controller)
    {
        base.ControlCamera(controller);
        var xChange = Input.GetAxisRaw("Mouse X");
        transform.Rotate(Vector3.up * rotSpeed * xChange);
    }
}
