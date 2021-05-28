using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
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

public class LandmarkCamera : CameraSystem
{
    [SerializeField]
    Transform target;
    float preferredDist = 30f; 
}

