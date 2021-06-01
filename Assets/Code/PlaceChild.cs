using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "PlaceChild", menuName = "Scriptables/Place Child", order = 3)]
public class PlaceChild : Placement
{
    [SerializeField]
    float minDistFromParent;
    [SerializeField]
    float maxDistFromParent;
    internal CellFilter GetFilter(Cell parentCell, Vector3 parentPos) =>
        (IEnumerable<Cell> cells) => cells.Where(cell => CellIsValid(cell) && CellIsNearParent(cell, parentCell, parentPos));
    bool CellIsNearParent(Cell cell, Cell parentCell, Vector3 parentPos)
    {
        var dist = Vector3.Distance(GridManager.CoordToPosition(cell.Coord), parentPos);
        return dist < maxDistFromParent && dist > minDistFromParent;
    }
}
