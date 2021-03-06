using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : SerializedMonoBehaviour
{
    [SerializeField]
    LayerMask mask;
    [SerializeField]
    HashSet<Collider> colls = new HashSet<Collider>();
    [SerializeField, FoldoutGroup("Events")]
    UnityEvent OnEnter;
    [SerializeField, FoldoutGroup("Events")]
    UnityEvent OnExit;
    [ShowInInspector, FoldoutGroup("Events")]
    public bool IsBlocked => colls.Count > 0;

    private void OnTriggerEnter(Collider other)
    {
        if (mask.ContainsGameObject(other.gameObject))
            colls.Add(other);
        if(colls.Count == 1)
            OnEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (mask.ContainsGameObject(other.gameObject))
            colls.Remove(other);
        if (colls.Count == 0)
            OnExit?.Invoke();
    }

}
