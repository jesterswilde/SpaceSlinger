using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObjLauncher : MonoBehaviour
{
    [SerializeField]
    LaunchedObj prefab;
    [SerializeField]
    Transform startingPoint;
    public Transform StartingPoint => startingPoint;
    [SerializeField]
    Transform raisedPoint;
    [SerializeField]
    float launchVelocity = 10f;
    [SerializeField]
    float resetTimer;
    [SerializeField]
    float timeToLaunch = 3f;
    [SerializeField]
    ColorChanger platformColor;
    [SerializeField]
    Color activeColor;
    public event Action DidLaunchObj;
    int launchTicket = -1; 

    public LaunchedObj ObjToLaunch { get; private set; }
    bool readyToLaunch = false;
    bool isOnCooldown = false;


    void MakeLaunchObj() {
        if(ObjToLaunch != null)
        {
            ObjToLaunch.Interactable.OnConnect -= PlayerConnected;
            ObjToLaunch.Interactable.OnDisconnect -= PlayerDisconnected;
        }

        ObjToLaunch = Instantiate(prefab);
        ObjToLaunch.Interactable.OnConnect += PlayerConnected;
        ObjToLaunch.Interactable.OnDisconnect += PlayerDisconnected;

        ObjToLaunch.transform.position = startingPoint.position;
        ObjToLaunch.transform.up = startingPoint.up;
        ObjToLaunch.LaunchVelocity = launchVelocity;
        isOnCooldown = false;
    }
    void PlayerConnected()
    {
        if (isOnCooldown)
            return;
        platformColor.ChangeColor(activeColor);
        ReadyObjToLaunch();
    }
    public void PlayerDisconnected() {
        if(launchTicket != -1)
        {
            Callback.Remove(launchTicket);
            launchTicket = -1; 
        }
        platformColor.ChangeToBaseColor();
        UnreadyObj();
        readyToLaunch = false;
    }
    void ReadyObjToLaunch() {
        if(ObjToLaunch != null)
            Lerper.MoveToAbsolute(ObjToLaunch.gameObject, raisedPoint.position, ()=> {
                readyToLaunch = true;
                ObjToLaunch.PrimeForLaunch();
            });
        launchTicket = Callback.Create(LaunchObj, timeToLaunch);
    } 
    void UnreadyObj()
    {
        var lerper = ObjToLaunch.GetComponent<Lerper>();
        if (lerper)
            Destroy(lerper);
        Lerper.MoveToAbsolute(ObjToLaunch.gameObject, startingPoint.position);
        ObjToLaunch.Deactivate();
    }
    void LaunchObj()
    {
        isOnCooldown = true;
        Callback.Create(MakeLaunchObj, resetTimer);
        ObjToLaunch.Interactable.OnDisconnect -= PlayerDisconnected;
        ObjToLaunch.Launch();
        DidLaunchObj?.Invoke();
    }
    private void Start()
    {
        MakeLaunchObj();
    }
}
