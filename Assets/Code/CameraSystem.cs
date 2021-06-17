using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField]
    Transform camParent;
    public Transform CamParent => camParent;
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
