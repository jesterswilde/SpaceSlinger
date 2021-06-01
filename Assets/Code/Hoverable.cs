using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.Utilities;

public class Hoverable : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnEnter;
    [SerializeField]
    UnityEvent OnOver;
    [SerializeField]
    UnityEvent OnExit;
    [SerializeField]
    Material hoverMat;
    List<Renderer> rends;
    List<Material> baseMats;
    internal virtual void HoverExit()
    {
        OnExit?.Invoke();
        if (hoverMat != null && Player.GetEquipment().Any(equip => equip.ShouldHighlight(this)))
            rends.ForEach((rend, i) => rend.material = baseMats[i]);
    }

    internal virtual void HoverEnter()
    {
        OnEnter?.Invoke();
        if (hoverMat != null)
            rends.ForEach((rend, i) => rend.material = hoverMat);
    }

    internal virtual void HoverOver()
    {
        OnOver?.Invoke();
    }
    private void Awake()
    {
        rends = GetComponentsInChildren<Renderer>().ToList();
        baseMats = rends.Select(rend => rend.material).ToList();
    }
}
