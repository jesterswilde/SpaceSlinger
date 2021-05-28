using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SparseResult<V>
{
    public int ValueIndex;
    public V Value;
    public int KeyIndex;
    public SparseResult<V> Copy() => new SparseResult<V>() {
        ValueIndex = ValueIndex,
        Value = Value,
        KeyIndex = KeyIndex
    };
}
[Serializable]
public class SparseList<V> 
{
    public SparseList()=> values = new Dictionary<int, V>();
    public SparseList(Dictionary<int, V> dict)=> values = dict;
    protected bool isDirty = true;
    [SerializeField]
    protected Dictionary<int, V> values;
    protected List<int> _keys;
    protected List<int> Keys { get {
            if(isDirty || _keys == null)
            {
                isDirty = false;
                _keys = values.Keys.ToList();
            }
            return _keys;
        } }
    public int KeyCount => Keys.Count;
    public int ValueCount => values.Count;
    public V this[int index]
    {
        get => values[index];
        set  {
            isDirty = true;
            values[index] = value;
        }
    }
    public SparseResult<V> First()
    {
        var valueIndex = Keys[0];
        return new SparseResult<V>()
        {
            KeyIndex = 0,
            ValueIndex = valueIndex,
            Value = values[valueIndex]
        };
    }
    public SparseResult<V> Last() { 
        var valueIndex = Keys[Keys.Count -1];
        return new SparseResult<V>()
        {
            KeyIndex = Keys.Count - 1,
            ValueIndex = valueIndex,
            Value = values[valueIndex]
        };
    }
    public SparseResult<V> Prev(SparseResult<V> result)
    {
        if (result.KeyIndex == 0)
            return null;
        var keyindex = result.KeyIndex - 1;
        var valueIndex = Keys[keyindex];
        return new SparseResult<V>
        {
            KeyIndex = keyindex,
            ValueIndex = valueIndex,
            Value = values[valueIndex]
        };
    }
    public SparseResult<V> Next(SparseResult<V> result)
    {
        if (result.KeyIndex == Keys.Count - 1)
            return null;
        var keyindex = result.KeyIndex + 1;
        var valueIndex = Keys[keyindex];
        return new SparseResult<V>
        {
            KeyIndex = keyindex,
            ValueIndex = valueIndex,
            Value = values[valueIndex]
        };
    }
    public SparseResult<V> GetClosestValue(int targetValue)
    {
        int valueIndex = -1;
        int diff = int.MaxValue;
        int keyIndex = -1;
        for (int i = 0; i < Keys.Count; i++)
        {
            var val = Keys[i];
            if (Math.Abs(val - targetValue) < diff)
            {
                diff = Math.Abs(val - targetValue);
                valueIndex = val;
                keyIndex = i;
            }
        }
        if (keyIndex == -1)
            return null;
        return new SparseResult<V>()
        {
            KeyIndex = keyIndex,
            ValueIndex = valueIndex,
            Value = values[valueIndex]
        };
    }
}
public class SparseList2D<V> : SparseList<List<V>>
{
    public void InsertAt(int index, V value)
    {
        isDirty = true;
        if (!values.ContainsKey(index))
            values[index] = new List<V>();
        values[index].Add(value);
    }
    public void RemoveAt(int index, V value)
    {
        if (values.ContainsKey(index))
        {
            isDirty = true;
            values[index].Remove(value);
        }
    }
}
