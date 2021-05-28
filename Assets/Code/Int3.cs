using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Int3
{
    [HorizontalGroup("vals", LabelWidth = 50)]
    public int X;
    [HorizontalGroup("vals")]
    public int Y;
    [HorizontalGroup("vals")]
    public int Z;
    public static Int3 operator -(Int3 a, Int3 b) => new Int3() { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };
    public static Int3 operator +(Int3 a, Int3 b) => new Int3() { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
    public static Int3 operator /(Int3 a, int b) => new Int3() { X = a.X / b, Y = a.Y / b, Z = a.Z / b };
    public static Int3 operator *(Int3 a, int b) => new Int3() { X = a.X * b, Y = a.Y * b, Z = a.Z * b };
    public Int3 AddX(int val) => new Int3() { X = X + val, Y = Y, Z = Z };
    public Int3 AddY(int val) => new Int3() { X = X, Y = Y + val, Z = Z };
    public Int3 AddZ(int val) => new Int3() { X = X, Y = Y, Z = Z + val };
    public override int GetHashCode() => X ^ Y.ShiftAndWrap(2) ^ Z.ShiftAndWrap(4);
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Int3))
            return false;
        Int3 other = (Int3) obj;
        return other.X == X && other.Y == Y && other.Z == Z;
    }
    public override string ToString()=>
        $"[{X} {Y} {Z}]";
    public Vector3 ToVector() => new Vector3((float)X, (float)Y, (float)Z);
    public static Int3 FromVector(Vector3 pos) => new Int3() { X = (int)pos.x, Y = (int)pos.y, Z = (int)pos.z };
    public List<Int3> GetNeighbors()
    {
        List<Int3> result = new List<Int3>();
        result.Add(AddX(1));
        result.Add(AddX(-1));
        result.Add(AddY(1));
        result.Add(AddY(-1));
        result.Add(AddZ(-1));
        result.Add(AddZ(1));
        return result;
    }
}
