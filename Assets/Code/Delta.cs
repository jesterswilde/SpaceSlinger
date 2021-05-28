using MiscUtil;
using System;
using System.Collections.Generic;

[Serializable]
public class Delta<T>
{
    public T Previous;
    public T Value;
    public bool Changed => !EqualityComparer<T>.Default.Equals(Previous, Value);
    public event Action<T, T> OnChange;
    public T Diff => Operator.Subtract(Value, Previous);
    public void Update(T val)
    {
        Previous = Value;
        Value = val;
        if (Changed)
            OnChange?.Invoke(Value, Previous);
    }
}
