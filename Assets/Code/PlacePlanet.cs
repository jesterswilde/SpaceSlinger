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
    List<SurfaceObjData> surfaceObjs = new List<SurfaceObjData>();
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
            List<SurfaceObj> placedObjs = new List<SurfaceObj>();
            PlaceSurfaceObject(planetTrans, obj, baseDir, placedObjs);
        });
    }
    void PlaceSurfaceObject(Transform planetTrans, SurfaceObjData obj, Vector3 baseDir, List<SurfaceObj> placedObj)
    {
        var surfaceObj = Instantiate(obj.Prefab);
        float extraAngle = 0;
        bool needsNewPlacement = true;
        Vector3 dir = Vector3.up; 
        while (needsNewPlacement)
        {
            dir = obj.GetAngle(baseDir, extraAngle);
            surfaceObj.transform.position = planetTrans.position + dir * distFromCenter;
            if (placedObj.Count == 0)
                needsNewPlacement = false;
            else
                needsNewPlacement = placedObj.All(other => !surfaceObj.CollidesWith(other));
            extraAngle += 1;
        }
        surfaceObj.transform.up = dir; 
    }
}
