using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hangtime : MonoBehaviour
{
    [SerializeField]
    AnimationCurve curve;
    Delta<Vector3> Pos => Player.T.Position;
    [SerializeField, HorizontalGroup("Speed")]
    float minSpeed;
    [SerializeField, HorizontalGroup("Speed")]
    float maxSpeed;
    [SerializeField]
    float multiplier = 1f;
    Rigidbody rigid;

    void ModifySpeed()
    {
        float dot = Vector3.Dot(Pos.Diff, Orientation.Up) * -1;
        dot = Mathf.Abs(dot);
        float curSpeed = Helpers.Map((Pos.Diff.magnitude / Time.fixedDeltaTime) * dot, minSpeed, maxSpeed, 0, 1, true);
        var modMag = curve.Evaluate(curSpeed) * multiplier;
        rigid.AddForce(Orientation.Up * modMag, ForceMode.Acceleration);
    }

    private void FixedUpdate()
    {
        ModifySpeed();
    }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
}
