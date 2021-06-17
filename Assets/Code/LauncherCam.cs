using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LauncherCam : CameraSystem
{
    [HideInInspector]
    public Transform TargetObj;
    [HideInInspector]
    public Transform PlayerTrans;
    [SerializeField]
    Transform camRoot;
    [SerializeField]
    Transform camPos;
    [SerializeField]
    Transform yPivot; 
    [SerializeField]
    Transform leftPoint;
    [SerializeField]
    Transform rightPoint;
    [SerializeField]
    LayerMask mask;
    [SerializeField]
    float minDist = 5;
    [SerializeField]
    float speedMult = 0.5f;
    [SerializeField]
    float rotSpeed = 10f;
    [SerializeField]
    float xPos = .7f; 
    float extraDist => minDist + speedMult * Player.T.Position.Diff.magnitude * Time.deltaTime;
    public Vector3 up;

    public override void ControlCamera(CameraController controller)
    {
        base.ControlCamera(controller);
        PositionCam();
        RotateCam();
        if(TargetObj != null)
            camPos.LookAt(TargetObj.position);
    }
    void PositionCam()
    {
        if (PlayerTrans == null || TargetObj == null)
            return;
        var dir = (PlayerTrans.position - TargetObj.position).normalized;
        var distToPlayer = Vector3.Distance(TargetObj.position, Player.T.Position.Value);
        //Ray ray = new Ray(TargetObj.position, CamParent.position - TargetObj.position);
        //if (Physics.Raycast(ray, out RaycastHit hit, distToPlayer + extraDist, mask))
        //{
        //    var distToHit = Vector3.Distance(TargetObj.position, hit.point);
        //    camRoot.position = TargetObj.position + distToHit * dir;
        //}
        //else
        //    camRoot.position = TargetObj.position + dir * (distToPlayer + extraDist);
        camRoot.position = TargetObj.position + dir * (distToPlayer + extraDist);
        camRoot.forward = Vector3.ProjectOnPlane(dir * -1, Orientation.Up);
    }
    void RotateCam()
    {
        var xChange = Input.GetAxisRaw("Mouse X");
        xPos = Mathf.Clamp(xPos + xChange * Time.deltaTime * rotSpeed, 0, 1);
        CamParent.position = Vector3.Lerp(leftPoint.position, rightPoint.position, xPos);
        var yChange = Input.GetAxisRaw("Mouse Y");
        yPivot.Rotate(Vector3.right * rotSpeed * yChange * -1);
    }
    private void OnDrawGizmos()
    {
        HashSet<Transform> transHash = GetComponentsInChildren<Transform>().ToHashSet();
        if (!transHash.Contains(Selection.activeTransform))
            return;
        Gizmos.color = Color.white;
        if(leftPoint != null && rightPoint != null)
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        Gizmos.DrawWireSphere(camRoot.position, 0.5f);
    }
}
