using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{

    [SerializeField]
    TrackingEnum tracking;
    private void Update()
    {
        var val = tracking switch
        {
            TrackingEnum.Gravity => Physics.gravity,
            TrackingEnum.Velocity => Player.T?.Velocity ?? Vector3.zero
        };
        if (val != Vector3.zero)
            transform.forward = val;
    }
}
enum TrackingEnum
{
    Gravity, 
    Velocity
}
