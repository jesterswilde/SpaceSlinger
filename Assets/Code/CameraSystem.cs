using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField]
    Transform camParent;
    public Transform CamParent => camParent;
    public virtual void ControlCamera(CameraController controller)
    {

    }
    public virtual void Mount(CameraController controller)
    {

    }
    public virtual void Unmount(CameraController controller)
    {

    }
}

public class LauncherCam : CameraSystem
{
    [HideInInspector]
    public Transform TargetObj;
    [HideInInspector]
    public Transform PlayerTrans;
    [SerializeField]
    Transform camRoot;
    [SerializeField]
    LayerMask mask;
    [SerializeField]
    float minDist = 5;
    [SerializeField]
    float speedMult = 0.5f;
    [SerializeField]
    float rotSpeed = 10f;
    float extraDist => minDist + speedMult * Player.T.Position.Diff.magnitude * Time.deltaTime;
    public Vector3 up;

    public override void ControlCamera(CameraController controller)
    {
        base.ControlCamera(controller);
        PositionCam();
        RotateCam();
    }
    void PositionCam()
    {
        var dir = (PlayerTrans.position - TargetObj.position).normalized;
        var distToPlayer = Vector3.Distance(TargetObj.position, Player.T.Position.Value);
        Ray ray = new Ray(TargetObj.position, CamParent.position - TargetObj.position);
        if (Physics.Raycast(ray, out RaycastHit hit, distToPlayer + extraDist, mask))
        {
            var distToHit = Vector3.Distance(TargetObj.position, hit.point);
            camRoot.position = TargetObj.position + distToHit * dir;
        }
        else
            camRoot.position = TargetObj.position + dir * (distToPlayer + extraDist);
        CamParent.LookAt(TargetObj);
    }
    void RotateCam()
    {
        var xChange = Input.GetAxisRaw("Mouse X");
        transform.Rotate(Vector3.up * rotSpeed * xChange);
        //var yChange = Input.GetAxisRaw("Mouse Y");
        //yPivot.Rotate(Vector3.right * rotSpeed * yChange * -1);
    }
}
