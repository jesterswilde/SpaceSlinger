using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class Procedural 
{
    public static Cell Place(Placement placeInfo, CellFilter filter = null)
    {
        var targetCell = PickCell(placeInfo, filter);
        var pos = GridManager.GetPositionInCell(targetCell);
        if (placeInfo.DistanceAway > 0)
            pos += GetDirAway(targetCell) * placeInfo.DistanceAway;
        GridManager.AddCelestialToCell(targetCell);
        var placedThing = GameObject.Instantiate(placeInfo.prefab);
        placedThing.transform.position = pos;
        placedThing.PlacedInCell = targetCell;
        PlaceRelated(placeInfo, targetCell, placedThing.transform);
        return targetCell;
    }
    static void PlaceRelated(Placement placeInfo, Cell cell, Transform placedThing)
    {
        _ = placeInfo switch
        {
            PlacePlanet planet => planet.childPlacements.All(child => PlaceChildren(child, cell, placedThing.position)),
            _ => false
        };
        placeInfo.FinishedPlacing(placedThing, cell);
    }
    static bool PlaceChildren(PlaceChild childToPlace, Cell parentCell, Vector3 parentPos)
    {
        Place(childToPlace, childToPlace.GetFilter(parentCell, parentPos));
        return true;
    }
    static Vector3 GetDirAway(Cell cell)=>
        GridManager.GetNeighboringCells(cell).Aggregate(Vector3.zero, (dir, neighbor) =>{
            if (neighbor.Falloff > cell.Falloff)
                dir -= (neighbor.Coord - cell.Coord).ToVector();
            if(neighbor.Falloff < cell.Falloff)
                dir += (neighbor.Coord - cell.Coord).ToVector();
            return dir;
        }).normalized;
    static Cell PickCell(Placement info, CellFilter filter = null) => info.PrimaryAxis switch
    {
        SearchOn.Amount => PickCellBy(info.PreferredAmount, filter ?? info.FilterCells, GridManager.CellsByAmount),
        SearchOn.Falloff => PickCellBy(info.PreferredFalloff, filter ?? info.FilterCells, GridManager.CellsByFalloff),
        _ => throw new NotImplementedException()
    };
    static Cell PickCellBy(int targetValue, CellFilter filter, SparseList2D<Cell> list)
    {
        var (cells, search) = BeginSearch(list, targetValue);
        var usableCells = filter(cells);
        while(usableCells.Count() == 0 && !search.IsFinished)
            usableCells = filter(search.GetNext());
        if (usableCells == null || usableCells.Count() == 0)
            throw new Exception("Search requirements are too restrictive here");
        return usableCells.ToList().PickRandom();
    }
    static (List<Cell> cells, SearchContext2D<Cell>) BeginSearch(SparseList2D<Cell> list, int target)
    {
        var result = list.GetClosestValue(target);
        return (result.Value, new SearchContext2D<Cell>()
        {
            Target = target,
            LargePointer = result.Copy(),
            SmallPointer = result.Copy(),
            List = list
        });
    }
}

public delegate IEnumerable<Cell> CellFilter(IEnumerable<Cell> cells);
