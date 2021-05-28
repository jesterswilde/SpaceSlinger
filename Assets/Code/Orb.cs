using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : Hoverable
{
    [SerializeField]
    Material baseMat;
    [SerializeField]
    Material higlightMat;
    Renderer _rend;
    Renderer rend { get {
            _rend ??= GetComponent<Renderer>();
            return _rend;
        } }

    internal override void HoverEnter()
    {
        rend.material = higlightMat;
    }
    internal override void HoverExit()
    {
        rend.material = baseMat;
    }
}
