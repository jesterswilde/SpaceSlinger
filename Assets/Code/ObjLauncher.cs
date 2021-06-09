using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ObjLauncher : MonoBehaviour
{
    [SerializeField]
    LaunchedObj prefab;
    [SerializeField]
    Transform startingPoint;
    [SerializeField]
    Transform raisedPoint;
    [SerializeField]
    float launchVelocity = 10f;
    [SerializeField]
    float resetTimer;
    [SerializeField]
    ColorChanger platformColor;
    [SerializeField]
    Color activeColor;

    LaunchedObj objToLaunch;
    bool readyToLaunch = false;
    bool isOnCooldown = false;


    void MakeLaunchObj() {
        objToLaunch = Instantiate(prefab);
        objToLaunch.transform.position = startingPoint.position;
        objToLaunch.transform.up = startingPoint.up;
        objToLaunch.LaunchVelocity = launchVelocity;
        isOnCooldown = false;
    }
    public void PlayerSteppedOnPad()
    {
        if (isOnCooldown)
            return;
        platformColor.ChangeColor(activeColor);
        ReadyObjToLaunch();
    }
    public void PlayerLeftPad() {
        platformColor.ChangeToBaseColor();
        if (readyToLaunch)
            LaunchObj();
        else
            UnreadyObj();
        readyToLaunch = false;
    }
    void ReadyObjToLaunch() {
        if(objToLaunch != null)
            Lerper.MoveToAbsolute(objToLaunch.gameObject, raisedPoint.position, ()=> {
                readyToLaunch = true;
                objToLaunch.PrimeForLaunch();
            });
    } 
    void UnreadyObj()
    {
        var lerper = objToLaunch.GetComponent<Lerper>();
        if (lerper)
            Destroy(lerper);
        Lerper.MoveToAbsolute(objToLaunch.gameObject, startingPoint.position);
        objToLaunch.Deactivate();
    }
    void LaunchObj()
    {
        isOnCooldown = true;
        Callback.Create(MakeLaunchObj, resetTimer);
        objToLaunch.Launch();
    }
    private void Start()
    {
        MakeLaunchObj();
    }
}
