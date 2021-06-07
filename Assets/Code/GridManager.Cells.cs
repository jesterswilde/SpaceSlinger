using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GridManager
{
    public static Vector3 GetPositionInCell(Cell cell)=> (cell.Coord * t.cellSize).ToVector();
    void UpdatePlayerCell()
    {
        var playerCoord = PositionToCoord(Player.Transform.position);
        if(playerCoord != playerCell.Value.Coord)
        {
            var nextCell = GetOrMakeNeighborCells(t.playerCell.Value, playerCoord);
            playerCell.Update(nextCell);
        }
    }
    static Cell GetOrMakeNeighborCells(Cell curCell, Int3 neighborCoord)
    {
        if (!t.knownCells.TryGetValue(neighborCoord, out var neighborCell))
        {
            neighborCell = new Cell() { 
                Coord = neighborCoord ,
                DistToOriginSqrd = Int3.DistanceSqrd(neighborCoord, StartingCoord),
            };
            neighborCell.Remoteness = curCell.Remoteness + 1;
            neighborCell.Falloff = FalloffFunc(curCell.Falloff);
            neighborCell.DistToCelestial = curCell.DistToCelestial + 1;
            CellsByAmount.InsertAt(0, neighborCell);
            CellsByDist.InsertAt(neighborCell.DistToOriginSqrd, neighborCell);
            t.knownCells[neighborCoord] = neighborCell;
        }
        else
        {
            CellsByFalloff.RemoveAt(neighborCell.Amount, neighborCell);
            neighborCell.Falloff = Mathf.Max(neighborCell.Amount, FalloffFunc(curCell.Falloff));
            neighborCell.DistToCelestial = Mathf.Min(neighborCell.DistToCelestial, curCell.DistToCelestial + 1);
            neighborCell.Remoteness = Mathf.Min(neighborCell.Remoteness, curCell.Remoteness + 1);
        }
        CellsByFalloff.InsertAt(neighborCell.Falloff, neighborCell);
        return neighborCell;
    }
    static Cell GetOrMakeCellWithCoord(Int3 coord, int amount = 0, int falloff = -10, int celestial = 100, bool shouldInsert = true)
    {
        if(!t.knownCells.TryGetValue(coord, out var cell))
        {
            cell = new Cell() { Coord = coord, DistToOriginSqrd = Int3.DistanceSqrd(StartingCoord, coord), Amount = amount, Falloff = falloff, DistToCelestial = celestial };
            t.knownCells[coord] = cell;
            if (shouldInsert)
            {
                CellsByAmount.InsertAt(amount, cell);
                CellsByFalloff.InsertAt(falloff, cell);
            }
            CellsByDist.InsertAt(cell.DistToOriginSqrd, cell);
        }
        return cell;
    }

    public static Vector3 ToDensestCell(Cell cell) => ToCellsOfType(cell, (neighbor) => neighbor.Falloff);
    public static Vector3 ToEmptiestCell(Cell cell) => ToCellsOfType(cell, (neighbor) => neighbor.Falloff * -1);
    public static Vector3 ToCellsOfType(Cell startingCell, Func<Cell, int> evaluator)
    {
        var neighbors = GetNeighborhood(startingCell, 2).Select(cell => new { value = evaluator(cell), cell });
        var targetCell = neighbors.MaxBy(vals => vals.value).cell;
        Debug.Log($"direction : {targetCell.Falloff}");
        return (GetPositionInCell(targetCell) - GetPositionInCell(startingCell));
    }
    static List<Cell> GetNeighborhood(Cell center, int range = 1) =>
        Helpers.CollectNeighborhood((x, y, z) =>
            GetOrMakeNeighborCells(center, new Int3() { X = center.Coord.X + x, Y = center.Coord.Y + y, Z = center.Coord.Z + z }),
            range);
    public static Cell Raycast(Vector3 position, Vector3 dir, Func<Cell,bool> meetsReqs, float stepSize = 0)
    {
        int maxSteps = 1000;
        if (stepSize == 0)
            stepSize = t.cellSize / 2f;
        var coord = new Delta<Int3>();
        coord.Update(PositionToCoord(position));
        var cell = GetOrMakeCellWithCoord(coord.Value);
        while(maxSteps > 0)
        {
            if (meetsReqs(cell))
                return cell;
            position += dir * stepSize;
            coord.Update(PositionToCoord(position));
            if (coord.Changed)
                cell = GetOrMakeNeighborCells(cell, coord.Value);
            maxSteps--;
        }
        throw new Exception("Raycast and never found anything. Consider less restrictive reqs");
    }
    public static List<Cell> GetNeighboringCells(Cell cell)
    {
        var neighborCoords = cell.Coord.GetNeighbors();
        neighborCoords.Where(neighbor => !t.knownCells.ContainsKey(neighbor))
            .ForEach(unknownCoord => GetOrMakeCellWithCoord(unknownCoord, falloff: FalloffFunc(cell.Falloff)));
        return neighborCoords.Select(coord => t.knownCells[coord]).ToList();
    }
}
