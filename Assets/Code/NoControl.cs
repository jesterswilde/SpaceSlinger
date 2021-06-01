using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoControl : PlayerMotion
{

    internal override void End()
    {
        player.Rigid.velocity = Vector3.zero;
        player.transform.position += Physics.gravity * -1; 
    }
}
