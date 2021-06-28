using Sirenix.OdinInspector;
using UnityEngine;

public class BasicCamera : CameraSystem
{
    [SerializeField]
    Transform target;
    Vector3 TargetPos { get {
            if(target == null && Player.DoesExist)
                target = Player.Transform;
            return target == null ? Vector3.zero : target.position;
        } }
    [SerializeField]
    float rotSpeed = 10f;
    [SerializeField]
    float ySentivity = 0.5f;
    [SerializeField]
    float maxY = 0.8f;
    [SerializeField]
    float minY = -0.3f;
    bool shouldLerp = true;
    [SerializeField]
    LayerMask clipMask;
    [SerializeField]
    Transform camOffset;
    [SerializeField]
    Transform yPivot;
    [SerializeField, FoldoutGroup("Lerp")]
    float lerpVelocity;
    [SerializeField, FoldoutGroup("Lerp")]
    float relaxMaxdist;
    float maxCamDist = 20;
    public override void ControlCamera(CameraController controller)
    {
        transform.position = TargetPos;
        PlayerControl(controller);
        ControlCameraDist(controller);
        LerpToOrientation(controller);
    }
    void PlayerControl(CameraController controller)
    {

        var xChange = Input.GetAxisRaw("Mouse X");
        var yChange = Input.GetAxisRaw("Mouse Y");
        transform.Rotate(Vector3.up * rotSpeed * xChange);
        yPivot.Rotate(Vector3.right * rotSpeed * yChange * -1);
    }
    void ControlCameraDist(CameraController controller)
    {
        var ray = new Ray(transform.position, camOffset.position - transform.position);
        float dist;
        if (Physics.Raycast(ray, out RaycastHit hit, maxCamDist + 2, clipMask))
        {
            dist = Vector3.Distance(hit.point, transform.position);
        }
        else
        {
            dist = Vector3.Distance(transform.position, camOffset.position);
            dist = Mathf.Min(maxCamDist, dist + relaxMaxdist * Time.deltaTime);
        }
        camOffset.position = camOffset.transform.forward * dist * -1 + transform.position;
        CameraController.CamDist.Update(dist);
    }
    void LerpToOrientation(CameraController controller)
    {
        if (!shouldLerp)
            return;
    }
    public override void Mount(CameraController controller)
    {
        base.Mount(controller);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            shouldLerp = !shouldLerp;
    }
}
