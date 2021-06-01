using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers
{
    public static List<T> CollectNeighborhood<T>(Func<int, int, T> evaluator, int range = 1)
    {
        var result = new List<T>();
        for(int x = -range; x <= range; x++)
            for(int y = -range; y <= range; y++)
                if (x != 0 || y != 0)
                    result.Add(evaluator(x, y));
        return result;
    }public static List<T> CollectNeighborhood<T>(Func<int, int, int, T> evaluator, int range = 1)
    {
        var result = new List<T>();
        for(int x = -range; x <= range; x++)
            for(int y = -range; y <= range; y++)
                for(int z = -range; z <= range; z++)
                if (x != 0 || y != 0 || z != 0)
                        result.Add(evaluator(x, y, z));
        return result;
    }
    public static void ForNeighborhood(Action<int, int> evaluator, int range = 1)
    {
        for(int x = -range; x <= range; x++)
            for(int y = -range; y <= range; y++)
                if (x != 0 || y != 0)
                    evaluator(x, y);
    }
    public static void ForNeighborhood(Action<int, int, int> evaluator, int range = 1)
    {
        for(int x = -range; x <= range; x++)
            for(int y = -range; y <= range; y++)
                for(int z = -range; z <= range; z++)
                    if (x != 0 || y != 0 || z != 0)
                        evaluator(x, y, z);
    }
    public static (List<T> matching, List<T> unmatching) Split<T>(this List<T> list, Func<T,int,bool> IsMatching)
    {
        var matching = new List<T>();
        var unmatching = new List<T>();
        for(int i = 0; i < list.Count; i++)
        {
            if (IsMatching(list[i], i))
                matching.Add(list[i]);
            else
                unmatching.Add(list[i]);
        }
        return (matching, unmatching);
    }
    public static (List<T> matching, List<T> unmatching) Split<T>(this List<T> list, Func<T,bool> IsMatching)
    {
        var matching = new List<T>();
        var unmatching = new List<T>();
        for(int i = 0; i < list.Count; i++)
        {
            if (IsMatching(list[i]))
                matching.Add(list[i]);
            else
                unmatching.Add(list[i]);
        }
        return (matching, unmatching);
    }
    public static T PickRandom<T>(this List<T> list)
    {
        if (list.Count == 0)
            return default(T);
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    public static int ShiftAndWrap(this int value, int positions)
    {
        positions = positions & 0x1F;

        // Save the existing bit pattern, but interpret it as an unsigned integer.
        uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        // Preserve the bits to be discarded.
        uint wrapped = number >> (32 - positions);
        // Shift and wrap the discarded bits.
        return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
    }
    public static float Map (this float value, float from1, float to1, float from2, float to2, bool shouldClamp = false) {
        var val = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        if (shouldClamp)
        {
            float larger, smaller;
            if(to2 > from2)
            {
                larger = to2;
                smaller = from2;
            }
            else
            {
                larger = from2;
                smaller = to2;
            }
            val = Mathf.Clamp(val, smaller, larger);
        }
        return val; 
    }
    public static bool ContainsGameObject(this LayerMask mask, GameObject go)=> (1 << go.layer & mask) > 0;
    public static Vector3 NoY(this Vector3 vec) => new Vector3(vec.x, 0, vec.z);
    public static bool WasAbleToAdd<T>(this HashSet<T> set, T val)
    {
        if (set.Contains(val))
            return false;
        set.Add(val);
        return true;
    }
    public static T MinBy<T,V>(this IEnumerable<T> list, Func<T,V> comparer) where V : IComparable<V>
    {
        if (list.Count() == 0)
            return default;
        if (list.Count() == 1)
            return list.First();
        T item = list.First();
        V val = comparer(item);
        foreach(var el in list)
        {
            var result = comparer(el);
            if(result.CompareTo(val) < 0)
            {
                val = result;
                item = el;
            }
        }
        return item;
    }
    public static T MaxBy<T,V>(this IEnumerable<T> list, Func<T,V> comparer) where V : IComparable<V>
    {
        if (list.Count() == 0)
            return default;
        if (list.Count() == 1)
            return list.First();
        T item = list.First();
        V val = comparer(item);
        foreach(var el in list)
        {
            var result = comparer(el);
            if(result.CompareTo(val) > 0)
            {
                val = result;
                item = el;
            }
        }
        return item;
    }
}
