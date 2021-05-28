using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class Procedural 
{
    public static Cell Place(Placement placeInfo)
    {
        var targetCell = PickCell(placeInfo);
        var pos = GridManager.GetPositionInCell(targetCell);
        if (placeInfo.DistanceAway > 0)
            pos += GetDirAway(targetCell) * placeInfo.DistanceAway;
        GridManager.AddCelestialToCell(targetCell);
        var placedThing = GameObject.Instantiate(placeInfo.prefab);
        placedThing.transform.position = pos;
        placedThing.PlacedInCell = targetCell;
        return targetCell;
    }
    static Vector3 GetDirAway(Cell cell)=>
        GridManager.GetNeighboringCells(cell).Aggregate(Vector3.zero, (dir, neighbor) =>{
            if (neighbor.Falloff > cell.Falloff)
                dir -= (neighbor.Coord - cell.Coord).ToVector();
            if(neighbor.Falloff < cell.Falloff)
                dir += (neighbor.Coord - cell.Coord).ToVector();
            return dir;
        }).normalized;
    static Cell PickCell(Placement info) => info.PrimaryAxis switch
    {
        SearchOn.Amount => PickCellBy(info.PreferredAmount, info, GridManager.CellsByAmount),
        SearchOn.Falloff => PickCellBy(info.PreferredFalloff, info, GridManager.CellsByFalloff),
        _ => throw new NotImplementedException()
    };
    static Cell PickCellBy(int targetValue, Placement info, SparseList2D<Cell> list)
    {
        var (cells, search) = BeginSearch(list, targetValue);
        var usableCells = info.FilterCells(cells);
        while(usableCells.Count() == 0 && !search.IsFinished)
            usableCells = info.FilterCells(search.GetNext());
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
