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
        if(Player.DoesExist)
            PlayerCell.Update(GetOrMakeCellWithCoord(PositionToCoord(Player.Transform.position)));
    }
    void FillCells(List<PointData> points)
    {
        foreach(var point in points)
        {
            Int3 cellCoord = point.Coord / cellSize;
            var cell = GetOrMakeCellWithCoord(cellCoord, amount: 0, falloff: 0, shouldInsert: false);
            cell.Amount++;
            cell.Falloff++;
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
            var (occupied, unoccupied) = curCell.Coord.GetNeighbors().Split(coord => knownCells.ContainsKey(coord));
            if(curCell.Falloff > minPopulation)
            {
                var emptyCells = unoccupied.Select(coord => GetOrMakeCellWithCoord(coord, falloff: neighborFalloff, amount: 0));
                emptyCells.ForEach(cell => {
                    cell.Remoteness = curCell.Remoteness + 1;
                    visited.Add(cell);
                });
                heap.AddRange(emptyCells);
                visited.AddRange(emptyCells);
            }

            var cellsToUpdate = occupied.Select(coord => knownCells[coord]).Where(cell => !visited.Contains(cell) || cell.Falloff < neighborFalloff);
            cellsToUpdate.ForEach(cell => {
                cell.Falloff = Mathf.Max(neighborFalloff, cell.Falloff);
                cell.Remoteness = Mathf.Min(curCell.Remoteness + 1, cell.Remoteness);
            });
            heap.AddRange(cellsToUpdate);
            visited.AddRange(cellsToUpdate);
        }
    }
}
