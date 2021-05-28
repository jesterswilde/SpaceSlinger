using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class Procedural 
{
    class SearchContext<V>
    {
        public int Target;
        public SparseResult<V> SmallPointer;
        public SparseResult<V> LargePointer;
        public SparseList<V> List;
        public bool IsFinished => SmallPointer.KeyIndex == 0 && LargePointer.KeyIndex == List.KeyCount - 1;
        public V GetNext()
        {
            if (IsFinished)
                throw new System.Exception("Tried to search after exhausting list. Use 'IsFinished' to avoid this");
            else if (SmallPointer.KeyIndex == 0)
            {
                LargePointer = List.Next(LargePointer);
                return LargePointer.Value;
            }
            else if (LargePointer.KeyIndex == List.KeyCount - 1)
            {
                SmallPointer = List.Prev(SmallPointer);
                return SmallPointer.Value;
            }
            var smallPoint = List.Prev(SmallPointer);
            var largePoint = List.Next(LargePointer);
            //next small pointer is closer
            if(Mathf.Abs(SmallPointer.ValueIndex - Target) < Mathf.Abs(LargePointer.ValueIndex - Target))
            {
                SmallPointer = smallPoint;
                return SmallPointer.Value;
            }
            //large pointer is closer
            else
            {
                LargePointer = largePoint;
                return LargePointer.Value;
            }
        }
    }
    class SearchContext2D<V> : SearchContext<List<V>> { }
}
