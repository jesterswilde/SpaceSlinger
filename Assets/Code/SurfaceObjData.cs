using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Rand = UnityEngine.Random;

[Serializable]
class SurfaceObjData
{
    [SerializeField]
    SurfaceObj prefab;
    public SurfaceObj Prefab => prefab;
    public PlaceOptions placeFacing;
    [SerializeField, HideIf("placeFacing", PlaceOptions.Anything)]
    float angleFrom = 15f;

    public Vector3 GetAngle(Vector3 baseDir, float extraAngle = 0) => placeFacing == PlaceOptions.Anything ? 
            new Vector3(Rand.Range(-1f, 1f), Rand.Range(-1f, 1f), Rand.Range(-1f, 1f)).normalized :
            (new Vector3(
                baseDir.x + Rand.Range(-(extraAngle + angleFrom), (extraAngle + angleFrom)),
                baseDir.y + Rand.Range(-(extraAngle + angleFrom), (extraAngle + angleFrom)),
                baseDir.z + Rand.Range(-(extraAngle + angleFrom), (extraAngle + angleFrom))
            )).normalized;
}
public enum PlaceOptions
{
    Dense,
    Void,
    Anything,
}
