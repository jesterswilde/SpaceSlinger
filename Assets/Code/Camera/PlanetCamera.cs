using Sirenix.OdinInspector;
using UnityEngine;
public class PlanetCamera : CameraSystem
{
    [SerializeField]
    Transform target;
    Transform Target { get {
            target ??= Player.Transform;
            return target;
        } }
    [SerializeField]
    float rotSpeed = 10f;
    [SerializeField]
    float maxCamDist = 20;
    [SerializeField]
    float curDist;
    [SerializeField]
    float upCompensator = 1;
    bool shouldLerp = true;
    [SerializeField]
    LayerMask clipMask;
    [SerializeField]
    float relaxMaxdist;
    Delta<float> distToPlanet = new Delta<float>();
    public override void ControlCamera(CameraController controller)
    {
        ControlCameraDist(controller);
        PlayerControl(controller);
        SetCamPos();
    }
    void SetCamPos()
    {
        var dir = (transform.position - Player.Transform.position).normalized;
        transform.position = Player.Transform.position + dir * curDist;
        transform.LookAt(Player.Transform.position, Orientation.RawUp);
    }
    void PlayerControl(CameraController controller)
    {
        var xChange = Input.GetAxisRaw("Mouse X");
        var yChange = Input.GetAxisRaw("Mouse Y");
        UpdateHeight();
        transform.position += (transform.right * xChange * rotSpeed * -1 + transform.up * yChange * rotSpeed * -1) * Time.deltaTime;
    }
    void ControlCameraDist(CameraController controller)
    {
        var ray = new Ray(Player.Transform.position, transform.position - Player.Transform.position);
        float curMax;
        if (Physics.Raycast(ray, out RaycastHit hit, maxCamDist + 2, clipMask))
            curMax = Vector3.Distance(hit.point, Player.Transform.position);
        else
            curMax = maxCamDist;
        curDist = Mathf.Min(curMax, curDist + relaxMaxdist * Time.deltaTime);
    }
    void UpdateHeight()
    {
        var planet = Gravity.GetNearestPlanet().transform.position;
        if (planet != null)
            distToPlanet.Update(Vector3.Distance(transform.position, planet));
    }
    public override void Mount(CameraController controller)
    {
        base.Mount(controller);
        distToPlanet.Update(10);
        distToPlanet.Update(10);
        curDist = maxCamDist;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            shouldLerp = !shouldLerp;
    }
}
