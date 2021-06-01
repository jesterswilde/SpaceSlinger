using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GridManager
{
    public void CreateCellsFromPoints(List<PointData> points)
    {
        FillCells(points);
        CategorizeCellsByAmount();
        CalculateFalloff(cellsByAmount[largestCellDensity]);
        CategorizeCellsByFalloff();
        PlayerCell.Update(UpsertCellWithCoord(PositionToCoord(Player.Transform.position)));
    }
    void FillCells(List<PointData> points)
    {
        foreach(var point in points)
        {
            Int3 cellCoord = point.Coord / cellSize;
            if(knownCells.TryGetValue(cellCoord, out Cell cell))
            {
                cell.Amount++;
                cell.Falloff++;
            }
            else {
                cell = new Cell() { Coord = cellCoord, Amount = 1, Falloff = 1, Remoteness = 1 };
                knownCells[cellCoord] = cell;
            }
        }
    }
    void CategorizeCellsByAmount()
    {
        knownCells.Values.ForEach(cell =>
        {
            cellsByAmount.InsertAt(cell.Amount, cell);
            largestCellDensity = Mathf.Max(cell.Amount, largestCellDensity);
        });
    }
    void CategorizeCellsByFalloff()=>
        knownCells.Values.ForEach(cell => cellsByFalloff.InsertAt(cell.Falloff, cell));
    void CalculateFalloff(List<Cell> largestCells)
    {
        Heap<Cell> heap = new Heap<Cell>((curHeap, otherHeap)=> curHeap.Falloff > otherHeap.Falloff);
        HashSet<Cell> visited = new HashSet<Cell>();
        heap.AddRange(largestCells);
        visited.AddRange(largestCells);
        while (heap.HasElements)
        {
            var curCell = heap.Pop();
            int neighborFalloff = FalloffFunc(curCell.Falloff);
            Debug.Log($"Making falloff {curCell.Falloff} | Neighbor: {neighborFalloff}");
            var (occupied, unoccupied) = curCell.Coord.GetNeighbors().Split(coord => knownCells.ContainsKey(coord));
            if(curCell.Falloff > minPopulation)
            {
                var emptyCells = unoccupied.Select(coord => new Cell() { Amount = 0, Falloff = neighborFalloff, Coord = coord });
                emptyCells.ForEach(cell => {
                    knownCells[cell.Coord] = cell;
                    cellsByFalloff.InsertAt(neighborFalloff, cell);
                    visited.Add(cell);
                });
                heap.AddRange(emptyCells);
                visited.AddRange(emptyCells);
            }

            var cellsToUpdate = occupied.Select(coord => knownCells[coord]).Where(cell => !visited.Contains(cell) || cell.Falloff < neighborFalloff);
            cellsToUpdate.ForEach(cell => cell.Falloff = Mathf.Max(neighborFalloff, cell.Falloff));
            heap.AddRange(cellsToUpdate);
            visited.AddRange(cellsToUpdate);
        }
    }
}
