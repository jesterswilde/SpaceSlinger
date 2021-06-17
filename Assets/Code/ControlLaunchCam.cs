using UnityEngine;

class ControlLaunchCam : MonoBehaviour
{

    /* When we enter the area, we start watching with this camera system
     * When we leave
     *    If they are connected, we keep watching
     *      Until they disconnect, then we stop watching
     *    if they are not connected
     *      We stop watching with this system   
     */

    [SerializeField]
    ObjLauncher launcher;
    [SerializeField]
    LauncherCam cam;
    Interactable _launchObj;
    Interactable curLaunchInter { get {
            _launchObj ??= launcher.GetComponent<Interactable>();
            return _launchObj;
        } }
    Interactable playerConnectedTo => Player.T.ConnectedEquipment?.ConnectedTo;
    bool isFollowing = false;
    public void PlayerEnteredArea()
    {
        cam.PlayerTrans = Player.Transform;
        cam.TargetObj = launcher.ObjToLaunch.transform;
        CameraController.LoadSystem(cam);
    }
    public void PlayerLeftArea()
    {
        if (playerConnectedTo == null || curLaunchInter != playerConnectedTo)
            RemoveFromStackAndReset();
        else
            isFollowing = true;
    }
    void CheckForNewLaunchObj()
    {
        if (launcher.ObjToLaunch != null)
            cam.TargetObj = launcher.ObjToLaunch.transform;
        else
            Callback.Create(CheckForNewLaunchObj, 1f);
    }
    void LaunchedObj()
    {
        if (playerConnectedTo == null || curLaunchInter != playerConnectedTo)
        {
            CheckForNewLaunchObj();
        }
    }
    void RemoveFromStackAndReset()
    {
        isFollowing = false;
        CameraController.UnloadSystem(cam);
    }
    void EventListener(PlayerEvents e)
    {
        if (isFollowing && e == PlayerEvents.Disconnected)
            RemoveFromStackAndReset();
    }
    private void Start()
    {
        Player.PEvent += EventListener;
        launcher.DidLaunchObj += LaunchedObj;
    }
}
