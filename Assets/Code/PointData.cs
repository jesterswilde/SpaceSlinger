using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public class PointData
{
    public Vector3 Position => Coord.ToVector();
    public List<int> VectorCount;
    public Int3 Coord;
    public Transform Transform;
    public PointData(List<int> counts, List<Int3> basisVectors)
    {
        int X = 0, Y = 0, Z = 0;
        for(int i = 0; i < counts.Count; i++)
        {
            X += counts[i] * basisVectors[i].X;
            Y += counts[i] * basisVectors[i].Y;
            Z += counts[i] * basisVectors[i].Z;
        }
        VectorCount = counts;
        Coord = new Int3() { X = X, Y = Y, Z = Z };
    }
    public List<PointData> GenerateNeighbors(List<Int3> basisVectors) {
        var result = new List<PointData>();
        basisVectors.ForEach((vec, i) =>
        {
            var add = VectorCount.ToList();
            add[i]++;
            var sub = VectorCount.ToList();
            sub[i]--;
            result.Add(new PointData(add, basisVectors));
            result.Add(new PointData(sub, basisVectors));
        });
        return result;
    }
    public List<Int3> GetNeighborCoords(List<Int3> vectors)
    {
        List<Int3> result = new List<Int3>();
        foreach(var vec in vectors)
        {
            result.Add(Coord + vec);
            result.Add(Coord - vec);
        }
        return result;
    }
    static int HashComponents(List<int> vals)
    {
        int result = 0;
        for (int i = 0; i < vals.Count; i++)
            result ^= vals[i].ShiftAndWrap(i*2);
        return result;
    }
    public override int GetHashCode() => HashComponents(VectorCount);
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is PointData))
            return false;
        var other = (PointData)obj;
        for (int i = 0; i < other.VectorCount.Count; i++)
            if (other.VectorCount[i] != VectorCount[i])
                return false;
        return true; 
    }

    internal void UpdateBasis(List<Int3> basisVectors)
    {
        Int3 newCoord = new Int3();
        for (int i = 0; i < basisVectors.Count; i++)
            newCoord += basisVectors[i] * VectorCount[i];
        Coord = newCoord;
        Callback.Create(() => Lerper.MoveToAbsolute(Transform.gameObject, Coord.ToVector()), 1f);
    }
}
