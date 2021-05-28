using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static CameraController t;
    Delta<float> camDist = new Delta<float>();
    public static Delta<float> CamDist => t.camDist;
    Camera cam;
    public static Camera Cam => t.cam;
    public Camera Camera => t.cam;
    [SerializeField]
    Transform yPivot;
    public Transform YPivot => yPivot;
    [SerializeField]
    Transform camOffset;
    public Transform CamOffset => camOffset;
    [SerializeField]
    CameraSystem curSystem;
    CameraSystem defaultSystem; 
    public static Vector3 NoYForward => t.transform.forward.NoY().normalized;
    public static Vector3 NoYRight => t.transform.right.NoY().normalized;
    public static Vector3 Forward => t.transform.forward;
    public static Vector3 Right => t.transform.right;

    public static void LoadSystem(CameraSystem system)
    {
        t.curSystem?.Unmount(t);
        t.curSystem = system == null ? t.defaultSystem : system;
        t.curSystem?.Mount(t);
    }
    private void LateUpdate()
    {
        curSystem?.ControlCamera(this);
    }
    private void Awake()
    {
        t = this;
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultSystem = GetComponentInChildren<BasicCamera>();
        LoadSystem(defaultSystem);
    }
}
