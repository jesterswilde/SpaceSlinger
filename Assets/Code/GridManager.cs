using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public partial class GridManager : SerializedMonoBehaviour
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
    protected Dictionary<Int3, Cell> knownCells = new Dictionary<Int3, Cell>();
    [SerializeField]
    SparseList2D<Cell> cellsByAmount = new SparseList2D<Cell>();
    public static SparseList2D<Cell> CellsByAmount => t.cellsByAmount;
    [SerializeField]
    SparseList2D<Cell> cellsByFalloff = new SparseList2D<Cell>();
    public static SparseList2D<Cell> CellsByFalloff => t.cellsByFalloff;
    [SerializeField]
    SparseList2D<Cell> cellsByDist = new SparseList2D<Cell>();
    public static SparseList2D<Cell> CellsByDist => t.cellsByDist;
    Int3 startingPoint = new Int3() { X = 0, Y = 0, Z = 0 };
    public static Int3 StartingCoord => t.startingPoint;
    int largestCellDensity = 0;

    #region DebugProperties
    [SerializeField, FoldoutGroup("Debug")]
    bool drawFalloff = false;
    [SerializeField, FoldoutGroup("Debug")]
    Gradient positiveDebugGradient;
    [SerializeField, FoldoutGroup("Debug")]
    Gradient negativeDebugGradient;
    [SerializeField, FoldoutGroup("Debug")]
    float debugCubeSize;
    [SerializeField, FoldoutGroup("Debug")]
    Int3 debugCoord;
    int numShades => largestCellDensity - minPopulation;
   
    [Button]
    void PrintCellAtCoord()
    {
        Debug.Log($"{knownCells[debugCoord]}");
    }
    #endregion

    public static void Setup(List<PointData> points)
    {
        t.CreateCellsFromPoints(points);
        t.DebugDraw();
    }

    public static Int3 PositionToCoord(Vector3 pos) => Int3.FromVector(pos / t.cellSize);
    public static Vector3 CoordToPosition(Int3 coord) => coord.ToVector() * t.cellSize;
    static int FalloffFunc(int neighborFalloff) => neighborFalloff > 0 ? (neighborFalloff / 2) : neighborFalloff - 1; 
    static int RemotenessFunc(Cell cell, Cell Neighbor = null)
    {
        var remoteness = (cell.Amount > 0 || cell.DistToCelestial > 0) ? 1 : 0;
        if (Neighbor != null)
            remoteness = Mathf.Min(remoteness, RemotenessFunc(Neighbor) - 1);
        return remoteness;
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
    public int DistToOriginSqrd;
    public int DistToCelestial = 100;
    public int Remoteness = 0;
    public Int3 Coord;
    public override string ToString() => $"Cell {{ Amount: {Amount} Falloff:{Falloff} Coord: {Coord} Remoteness: {Remoteness} DistToCelest:{DistToCelestial} }}";
}
