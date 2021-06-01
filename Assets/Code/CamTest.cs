using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CamTest : MonoBehaviour
{
    [SerializeField]
    Transform center;
    [SerializeField]
    Transform target;
    [SerializeField]
    Vector3 right;


    [SerializeField]
    Transform controlled;
    [SerializeField]
    Transform follow;


    private void Update()
    {
        var forward = (target.position - center.position).normalized;
        Debug.DrawRay(center.position, forward * 5, Color.red);
        var upCross = Vector3.Cross(forward, right).normalized;
        Debug.DrawRay( center.position, upCross * 5);
        var rightCross = Vector3.Cross(upCross, forward).normalized;
        Debug.DrawRay( center.position, rightCross * 5, Color.black);
        right = rightCross == Vector3.zero ? Vector3.right : rightCross;

        controlled.position = center.position + forward * 5f;
        follow.position = controlled.position + upCross * 3;
        follow.LookAt(controlled, forward);
    }
    private void Start()
    {
        right = Vector3.right;
    }

}
