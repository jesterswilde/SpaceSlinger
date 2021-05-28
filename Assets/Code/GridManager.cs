using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GridManager : SerializedMonoBehaviour
{
    static GridManager t;
    Delta<Cell> playerCell = new Delta<Cell>();
    [SerializeField]
    Cell curPlayerCell;
    public static Delta<Cell> PlayerCell => t.playerCell;
    [SerializeField]
    int cellSize = 10;
    [SerializeField]
    int minPopulation = -5;
    [SerializeField]
    int celestialCascade = 3;
    Dictionary<Int3, Cell> knownCells = new Dictionary<Int3, Cell>();
    [SerializeField]
    SparseList2D<Cell> cellsByAmount = new SparseList2D<Cell>();
    public static SparseList2D<Cell> CellsByAmount => t.cellsByAmount;
    [SerializeField]
    SparseList2D<Cell> cellsByFalloff = new SparseList2D<Cell>();
    public static SparseList2D<Cell> CellsByFalloff => t.cellsByFalloff;
    int largestCellDensity = 0;

    [SerializeField, FoldoutGroup("Debug")]
    bool drawFalloff = false;
    [SerializeField, FoldoutGroup("Debug")]
    Gradient positiveDebugGradient;
    [SerializeField, FoldoutGroup("Debug")]
    Gradient negativeDebugGradient;
    [SerializeField, FoldoutGroup("Debug")]
    float debugCubeSize;
    int numShades => largestCellDensity - minPopulation;
    

    void UpdatePlayerCell()
    {
        var playerCoord = PositionToCoord(Player.Transform.position);
        var nextCell = UpsertNeighborCell(t.playerCell.Value, playerCoord);
        playerCell.Update(nextCell);
    }
    static Cell UpsertNeighborCell(Cell curCell, Int3 neighborCoord)
    {
        if (!t.knownCells.TryGetValue(neighborCoord, out var neighborCell))
        {
            t.knownCells[neighborCoord] = neighborCell;
            neighborCell = new Cell() { Coord = neighborCoord };
            neighborCell.Remoteness = curCell.Remoteness + 1;
            neighborCell.Falloff = t.FalloffFunc(curCell.Falloff);
            neighborCell.DistToCelestial = curCell.DistToCelestial + 1;
            CellsByAmount.InsertAt(0, neighborCell);
            t.knownCells[neighborCoord] = neighborCell;
        }
        else
        {
            CellsByFalloff.RemoveAt(neighborCell.Amount, neighborCell);
            neighborCell.Falloff = Mathf.Max(neighborCell.Amount, curCell.Falloff);
            neighborCell.DistToCelestial = Mathf.Min(neighborCell.DistToCelestial, curCell.DistToCelestial + 1);
        }
        CellsByFalloff.InsertAt(neighborCell.Falloff, neighborCell);
        return neighborCell;
    }
    static Cell UpsertCellWithCoord(Int3 coord, int amount = 0, int falloff = -10, int celestial = 100)
    {
        if(!t.knownCells.TryGetValue(coord, out var cell))
        {
            cell = new Cell() { Coord = coord, Amount = amount, Falloff = falloff, DistToCelestial = celestial };
            t.knownCells[coord] = cell;
            t.cellsByAmount.InsertAt(amount, cell);
            t.cellsByFalloff.InsertAt(falloff, cell);
        }
        return cell;
    }
    public static Cell Raycast(Vector3 position, Vector3 dir, Func<Cell,bool> meetsReqs, float stepSize = 0)
    {
        int maxSteps = 1000;
        if (stepSize == 0)
            stepSize = t.cellSize / 2f;
        var coord = new Delta<Int3>();
        coord.Update(PositionToCoord(position));
        var cell = UpsertCellWithCoord(coord.Value);
        while(maxSteps > 0)
        {
            if (meetsReqs(cell))
                return cell;
            position += dir * stepSize;
            coord.Update(PositionToCoord(position));
            if (coord.Changed)
                cell = UpsertNeighborCell(cell, coord.Value);
            maxSteps--;
        }
        throw new Exception("Raycast and never found anything. Consider less restrictive reqs");
    }
    public static Int3 PositionToCoord(Vector3 pos) => Int3.FromVector(pos / t.cellSize);
    public static List<Cell> GetNeighboringCells(Cell cell)
    {
        var neighborCoords = cell.Coord.GetNeighbors();
        neighborCoords.Where(neighbor => !t.knownCells.ContainsKey(neighbor))
            .ForEach(unknownCoord => UpsertCellWithCoord(unknownCoord, falloff: cell.Falloff-1));
        return neighborCoords.Select(coord => t.knownCells[coord]).ToList();
    }
    public static Vector3 GetPositionInCell(Cell cell)=>
        (cell.Coord * t.cellSize).ToVector();
    public static void Setup(List<PointData> points)
    {
        t.CreateCellsFromPoints(points);
        t.DebugDraw();
    }
    void CreateCellsFromPoints(List<PointData> points)
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
    int FalloffFunc(int neighborFalloff) => neighborFalloff > 0 ? (neighborFalloff / 2) : neighborFalloff - 1; 
    int RemotenessFunc(Cell cell, Cell Neighbor = null)
    {
        var remoteness = (cell.Amount > 0 || cell.DistToCelestial > 0) ? 1 : 0;
        if (Neighbor != null)
            remoteness = Mathf.Min(remoteness, RemotenessFunc(Neighbor) - 1);
        return remoteness;
    }
    void CategorizeCellsByFalloff()=>
        knownCells.Values.ForEach(cell => cellsByFalloff.InsertAt(cell.Falloff, cell));
    void CalculateFalloff(List<Cell> largestCells)
    {
        cellsByAmount[0] = new List<Cell>();
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
                var emptyCells = unoccupied.Select(coord => new Cell() { Amount = 0, Falloff = neighborFalloff, Coord = coord });
                emptyCells.ForEach(cell => {
                    knownCells[cell.Coord] = cell;
                    cellsByAmount[0].Add(cell);
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
    public static void AddCelestialToCell(Cell cell)
    {
        int count = 0;
        List<Cell> cells = new List<Cell>() { cell };
        HashSet<Cell> visited = new HashSet<Cell>();
        while(count < t.celestialCascade)
        {
            cells.ForEach(cell => cell.DistToCelestial = Mathf.Min(count, cell.DistToCelestial));
            visited.AddRange(cells);
            cells = cells.Aggregate(new List<Cell>(), (list, cell) => {
                list.AddRange(GetNeighboringCells(cell));
                return list;
            }).Where(cell => !visited.Contains(cell)).ToList();
            count++;
        }
    }
    private void Update()
    {
        UpdatePlayerCell();
        curPlayerCell = PlayerCell.Value;
    }
    private void Awake()
    {
        t = this;
    }
    void DebugDraw()
    {
        if (drawFalloff)
            DebugDrawFalloff();
    }
    void DebugDrawFalloff()
    {
        var cellSearch = CellsByAmount.First();
        while(cellSearch != null)
        {
            cellSearch.Value.ForEach(cell =>
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = (cell.Coord * cellSize).ToVector();
                cube.transform.localScale = Vector3.one * debugCubeSize;
                var mat = cube.GetComponent<Renderer>().material;
                if(cell.Falloff > 0)
                    mat.color = positiveDebugGradient.Evaluate(((float)cellSearch.ValueIndex).Map(largestCellDensity, 0, 0f, 1f));
                else
                    mat.color = negativeDebugGradient.Evaluate(((float)cellSearch.ValueIndex).Map(0f, minPopulation, 0f, 1f));
            });
            cellSearch = CellsByAmount.Next(cellSearch);
        }
    }

}
[Serializable]
public class Cell
{
    public int Amount;
    public int Falloff;
    public Int3 Coord;
    public int DistToCelestial = 100;
    public int Remoteness = 0;
}
