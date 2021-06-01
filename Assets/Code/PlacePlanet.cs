using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PlacePlanet", menuName = "Scriptables/Place Planet", order = 2)]
public class PlacePlanet : Placement
{
    [SerializeField, FoldoutGroup("Planet")]
    List<SurfaceObj> surfaceObjs = new List<SurfaceObj>();
    [SerializeField, FoldoutGroup("Planet")]
    public List<PlaceChild> childPlacements =  new List<PlaceChild>();
    [SerializeField, FoldoutGroup("Planet")]
    float distFromCenter = 20f;

    public override void FinishedPlacing(Transform planetTrans, Cell cell)
    {
        PlaceSurfaceObjects(planetTrans, cell);
    }

    private void PlaceSurfaceObjects(Transform planetTrans, Cell cell)
    {
        surfaceObjs.ForEach(obj => {
            var baseDir = obj.placeFacing switch {
                PlaceOptions.Dense => GridManager.ToDensestCell(cell),
                PlaceOptions.Void => GridManager.ToEmptiestCell(cell),
                _ => Vector3.up
            };
            PlaceSurfaceObject(planetTrans, obj, baseDir);
        });
    }
    void PlaceSurfaceObject(Transform planetTrans, SurfaceObj obj, Vector3 baseDir)
    {
        var dir = obj.GetAngle(baseDir);
        var trans = Instantiate(obj.Prefab);
        trans.position = planetTrans.position + dir * distFromCenter;
        trans.up = dir; 
    }
}
