using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField]
    List<MotionInfo> Movements;
    [SerializeField]
    Transform target;

    Vector3 basePos;

    internal void UpdateMotion(MotionInfo motion)
    {
        Movements = new List<MotionInfo>() { motion };
    }
    internal void UpdateMotion(List<MotionInfo> motions)
    {
        Movements = motions;
    }

    private void Update()
    {
        Vector3 offset = basePos;
        if (target != null)
            offset = target.position;
        transform.position = Movements.Aggregate(offset, (total,move) =>
        {
            move.Phase += Time.deltaTime * move.Frequency;
            return total + move.MoveType switch {
                MoveType.Linear => Mathf.Abs((move.Phase % 2) - 1f) * move.Direction,
                MoveType.Sine => Mathf.Sin(move.Phase * Mathf.PI) * move.Direction,
                _ => Vector3.zero
            };
        });
    }
    private void Start()
    {
        basePos = transform.position;
    }
    [Serializable]
    public class MotionInfo
    {
        public MoveType MoveType;
        public Vector3 Direction = Vector3.up;
        public float Frequency = 1;
        public float Phase;
    }
    public enum MoveType
    {
        Linear,
        Sine
    }
}
