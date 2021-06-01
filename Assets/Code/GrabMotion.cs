using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabMotion : PlayerMotion
{
    IGrabbable heldThing;
    Vector3 movement;
    internal void HoldOnto(Grab grab)
    {
        heldThing = grab.GrabbedThing;
        heldThing.Connect();
    }
    internal override void GetInputs()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }
    internal override void Run(float fixedDeltaTime)
    {
        heldThing.Move(fixedDeltaTime, movement);
    }
    internal override void End()
    {
        heldThing.Disconnect();
    }
}
