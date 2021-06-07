using Sirenix.OdinInspector;
using System;
using UnityEngine;

public interface IValidateCell
{
    bool IsCellValid(Cell cell);
}

[Serializable]
public class CellWithinFalloff : IValidateCell
{
    [Title("Falloff")]
    [SerializeField]
    float minFaloff = -30;
    [SerializeField]
    float maxFalloff = 30;
    public bool IsCellValid(Cell cell) => cell.Falloff <= maxFalloff && cell.Falloff >= minFaloff;
}
[Serializable]
public class CellWithinAmount : IValidateCell
{
    [Title("Ammount")]
    [SerializeField]
    public float minAmount = 0f;
    [SerializeField]
    public float maxAmount = 100f;
    public bool IsCellValid(Cell cell) => cell.Amount <= maxAmount && cell.Amount >= minAmount;
}

[Serializable]
public class CellWithinCelestialDist : IValidateCell
{
    [Title("Celestial Dist")]
    [SerializeField]
    float minDist = 0;
    [SerializeField]
    float maxDist = 20f;
    public bool IsCellValid(Cell cell) => cell.DistToCelestial <= maxDist && cell.DistToCelestial >= minDist;
}

[Serializable]
public class CellWithRemoteness : IValidateCell
{
    [Title("Remotness")]
    [SerializeField]
    float minRemote = 0;
    [SerializeField]
    float maxRemote = 100f;
    public bool IsCellValid(Cell cell) => cell.Remoteness <= maxRemote && cell.Remoteness >= minRemote;
}
[Serializable]
public class CellWithinDistFromStart : IValidateCell
{
    [Title("Dist From Origin")]
    [SerializeField]
    float minDist = 0;
    [SerializeField]
    float maxDist = 100f;
    public bool IsCellValid(Cell cell)
    {
        var diff = GridManager.StartingCoord - cell.Coord;
        var diffSqrd = diff.X * diff.X + diff.Y * diff.Y + diff.Z * diff.Z;
        float minSqrd = minDist * minDist;
        float maxSqrd = maxDist * maxDist;
            return diffSqrd <= maxSqrd && diffSqrd >= minSqrd;
    }
}
