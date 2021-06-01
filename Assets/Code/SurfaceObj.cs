using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Rand = UnityEngine.Random;

[Serializable]
class SurfaceObj
{
    public Transform Prefab;
    public PlaceOptions placeFacing;
    [SerializeField, HideIf("placeFacing", PlaceOptions.Anything)]
    float angleFrom = 15f;
    public Vector3 GetAngle(Vector3 baseDir) => placeFacing == PlaceOptions.Anything ? 
            new Vector3(Rand.Range(-1f, 1f), Rand.Range(-1f, 1f), Rand.Range(-1f, 1f)).normalized :
            (new Vector3(baseDir.x + Rand.Range(-angleFrom, angleFrom), baseDir.y + Rand.Range(-angleFrom, angleFrom), baseDir.z + Rand.Range(-angleFrom, angleFrom))).normalized;
}
public enum PlaceOptions
{
    Dense,
    Void,
    Anything,
}
