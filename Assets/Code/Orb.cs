using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Orb : Hoverable
{
    public void StartVibrating()
    {
    }
    public void StopVibrating()
    {

    }
    internal void EnableCollision()
    {
        GetComponentInChildren<Collider>().enabled = true;
    }
}
