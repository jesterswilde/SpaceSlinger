using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static CameraController t;
    public static bool IsSetup => t != null;
    Delta<float> camDist = new Delta<float>();
    public static Delta<float> CamDist => t.camDist;
    Camera cam;
    public static Camera Cam => t.cam;
    public Camera Camera => t.cam;
    List<CameraSystem> activeSystems = new List<CameraSystem>();
    [SerializeField]
    CameraSystem curSystem;
    CameraSystem prevSystem;
    float transition = 0;
    float transitionSpeed = 1f;
    bool isTransitioning = false;
    [SerializeField]
    CameraSystem defaultSystem;
    [SerializeField]
    CameraSystem planetCam;
    public static CameraSystem PlanetCam => t.planetCam;
    [SerializeField]
    CameraSystem attachedCam;
    bool isActive = true;
    public static CameraSystem AttachedCam => t.attachedCam;
    public static Vector3 NoYForward => Cam?.transform.forward.NoY().normalized ?? Vector3.forward;
    public static Vector3 NoYRight => Cam?.transform.right.NoY().normalized ?? Vector3.right;
    public static Vector3 Forward => Cam?.transform.forward ?? Vector3.forward;
    public static Vector3 Right => Cam?.transform.right ?? Vector3.right;

    void StartLerp(float lerpSpeed = 1f)
    {
        t.isTransitioning = true;
        t.transitionSpeed = lerpSpeed;
        t.transition = 0f;
    }
    public static void LoadSystem(CameraSystem system, float lerpSpeed = 1f)
    {
        Debug.Log($"{system.name}");
        if (t.curSystem == system)
            return; 
        t.activeSystems.Add(system);
        t.prevSystem = t.curSystem;
        t.curSystem = system;
        t.curSystem?.Mount(t);
        if (t.prevSystem != null)
            t.StartLerp(lerpSpeed);
    }
    public static void UnloadSystem(CameraSystem system)
    {
        t.activeSystems.Remove(system);
        if(t.curSystem == system)
        {
            t.prevSystem = system;
            t.curSystem = t.activeSystems.Last();
            t.StartLerp();
        }
    }
    void Lerp()
    {
        transition = Mathf.Min(1, transition + Time.deltaTime * transitionSpeed);
        Cam.transform.position = Vector3.Lerp(prevSystem.CamParent.position, curSystem.CamParent.position, transition);
        Cam.transform.rotation = Quaternion.Slerp(prevSystem.CamParent.rotation, curSystem.CamParent.rotation, transition);
        if(transition == 1)
        {
            isTransitioning = false;
            prevSystem.Unmount(this);
            prevSystem = null;
        }
    }
    void SystemControlsCam()
    {
        Cam.transform.position = curSystem.CamParent.transform.position;
        Cam.transform.rotation = curSystem.CamParent.rotation; 
    }
    public void SetIsActive(bool value)=> isActive = value;
    private void LateUpdate()
    {
        if (!isActive)
            return;
        curSystem?.ControlCamera(this);
        if (isTransitioning)
            Lerp();
        else
            SystemControlsCam();
        if(Player.DoesExist)
            camDist.Update(Vector3.Distance(Cam.transform.position, Player.Transform.position));
    }
    private void Awake()
    {
        t = this;
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        LoadSystem(defaultSystem);
    }
}
