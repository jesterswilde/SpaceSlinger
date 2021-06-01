using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Placement", menuName = "Scriptables/Placement", order = 1)]
public class Placement : ScriptableObject
{
    public Placeable prefab;
    [FoldoutGroup("Initial Placement")]
    public SearchOn PrimaryAxis;
    [ShowIf("PrimaryAxis", SearchOn.Amount), FoldoutGroup("Initial Placement")]
    public int PreferredAmount;
    [ShowIf("PrimaryAxis",  SearchOn.Falloff), FoldoutGroup("Initial Placement")]
    public int PreferredFalloff;

    [FoldoutGroup("Initial Placement")]
    public bool useCelestialDist = false;
    [ShowIf("useCelestialDist"), FoldoutGroup("Initial Placement")]
    public int MinDistToCelestial = 0;
    [ShowIf("useCelestialDist"), FoldoutGroup("Initial Placement")]
    public int MaxDistToCelestial = 200;

    [FoldoutGroup("Initial Placement")]
    public bool UseAmount;
    [ShowIf("UseAmount"), FoldoutGroup("Initial Placement")]
    public float MinAmount = 0;
    [ShowIf("UseAmount"), FoldoutGroup("Initial Placement")]
    public float MaxAmount = 1000;

    [FoldoutGroup("Initial Placement")]
    public bool UseFalloff;
    [ShowIf("UseFalloff"), FoldoutGroup("Initial Placement")]
    public float MinFalloff = -100;
    [ShowIf("UseFalloff"), FoldoutGroup("Initial Placement")]
    public float MaxFalloff = 1000;

    [FoldoutGroup("After Created")]
    public float DistanceAway = 0;
    protected virtual bool CellIsValid(Cell cell)
    {
        if (useCelestialDist && (cell.DistToCelestial < MinDistToCelestial || cell.DistToCelestial > MaxDistToCelestial))
            return false;
        if (UseAmount && (cell.Amount < MinAmount || cell.Amount > MaxAmount))
            return false;
        if (UseFalloff && (cell.Falloff < MinFalloff || cell.Falloff > MaxFalloff))
            return false;
        return true;
    }
    public virtual IEnumerable<Cell> FilterCells(IEnumerable<Cell> cells) => cells.Where(CellIsValid);
    public virtual void FinishedPlacing(Transform placedObj, Cell cell)
    {

    }
}
public enum SearchOn
{
    Amount,
    Falloff
}
