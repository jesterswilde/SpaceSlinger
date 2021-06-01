using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{

    private void Update()
    {
        Shader.SetGlobalVector("_PlayerPos", Player.Transform.position);
    }
}
