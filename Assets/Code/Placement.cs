using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Placement", menuName = "Scriptables/Placement", order = 1)]
public class Placement : SerializedScriptableObject
{
    [SerializeField]
    Placeable prefab;
    public Placeable Prefab => prefab;
    [FoldoutGroup("Initial Placement")]
    public SearchOn PrimaryAxis;
    [FoldoutGroup("Initial Placement")]
    public int PreferredAxisVal;
    [SerializeField, FoldoutGroup("Initial Placement")]
    float distAway = 0;
    public float DistanceAway => distAway;

    [SerializeField, PropertyOrder(0), FoldoutGroup("Criteria")]
    Criteria criteria;


    [SerializeField, PropertyOrder(1), FoldoutGroup("Criteria")]
    List<IValidateCell> cellReqs = new List<IValidateCell>();
    [Button(ButtonSizes.Large), GUIColor(0, .3f, 1f), PropertyOrder(2), FoldoutGroup("Criteria")]
    void AddCriteria()
    {
        IValidateCell cellReq = criteria switch
        {
            Criteria.Amount => new CellWithinAmount(),
            Criteria.Falloff => new CellWithinFalloff(),
            Criteria.DistFromStart => new CellWithinDistFromStart(),
            Criteria.CelestialDist => new CellWithinCelestialDist(),
            Criteria.Remoteness => new CellWithRemoteness(),
            _ => throw new Exception("Don't have that enum covered"),
        };
        cellReqs.Add(cellReq);
    }
    public virtual bool CellIsValid(Cell cell) => cellReqs.All(req => req.IsCellValid(cell));
    public virtual IEnumerable<Cell> FilterCells(IEnumerable<Cell> cells) => cells.Where(CellIsValid);
    public virtual void FinishedPlacing(Transform placedObj, Cell cell)
    {

    }
    enum Criteria
    {
        Falloff,
        Amount,
        CelestialDist,
        DistFromStart,
        Remoteness,
    }
}
public enum SearchOn
{
    Amount,
    Falloff,
    DistFromOrigin
}
