using Sirenix.OdinInspector;
using UnityEngine;

public class AttachedCam : CameraSystem
{
    Interactable target;
    Vector3 CenterPos => (Player.Transform.position + target.transform.position) / 2f;
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
    [SerializeField]
    float baseDist = 20;
    [SerializeField]
    float maxLerpSpeed = 1f;
    [SerializeField]
    float speedDistMult;
    [SerializeField]
    float minMove = 1f;
    float distBetween => Vector3.Distance(Player.Transform.position, target.transform.position);
    float maxCamDist => baseDist + Player.T.Velocity.magnitude * speedDistMult;
    public override void ControlCamera(CameraController controller)
    {
        if(Gear.T?.ConnectedEquipment?.ConnectedTo != target)
            CameraController.UnloadSystem(this);
        else {
            PlaceCamRoot();
            PlayerControl(controller);
            ControlCameraDist(controller);
        }
    }
    void PlaceCamRoot()
    {
        var dir = (CenterPos - transform.position).normalized;
        var mag = Vector3.Distance(CenterPos, transform.position);
        var clamps = Mathf.Max(maxLerpSpeed * Time.deltaTime, (mag * Time.deltaTime) );
        mag = Mathf.Min(mag, clamps);
        var moveTarget = transform.position + mag * dir; 
        //if(Vector3.Distance(moveTarget, CenterPos) > minMove)
            transform.position += mag * dir;
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
            dist += distBetween;
        }
        camOffset.position = camOffset.transform.forward * dist * -1 + transform.position;
        CameraController.CamDist.Update(dist);
    }
    public override void Mount(CameraController controller)
    {
        base.Mount(controller);
        target = Gear.T.ConnectedEquipment.ConnectedTo;
        transform.position = CenterPos;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            shouldLerp = !shouldLerp;
    }
}
