using System;
using System.Collections;
using System.Collections.Generic;


class Heap<T>
{
    List<T> storage = new List<T>();
    Func<T, T, bool> HasPriority; 

    public void AddRange(IEnumerable<T> elements)
    {
        foreach (var el in elements)
            Add(el);
    }
    public void Add(T element)
    {
        var childIndex = storage.Count;
        var parentIndex = GetParentIndex(childIndex);
        storage.Add(element);
        bool shouldBubble = true;
        while(childIndex > 0 && shouldBubble)
        {
            var parentVal = storage[parentIndex];
            shouldBubble = HasPriority(element, parentVal);
            if (shouldBubble)
            {
                storage[childIndex] = parentVal;
                storage[parentIndex] = element;
                childIndex = parentIndex;
                parentIndex = GetParentIndex(childIndex);
            }
        }
    }
    public T Pop()
    {
        if (storage.Count == 0)
            throw new Exception("There was nothing to remove in heap");
        var result = storage[0];
        var parentIndex = 0;
        var bubblingVal = storage[storage.Count - 1];
        storage[0] = bubblingVal;
        storage.RemoveAt(storage.Count - 1);
        var childIndex = GetChildIndex(parentIndex);
        while(childIndex >= 0){
            var childVal = storage[childIndex];
            var shouldBubble = !HasPriority(bubblingVal, childVal);
            if (shouldBubble)
            {
                storage[parentIndex] = childVal;
                storage[childIndex] = bubblingVal;
                parentIndex = childIndex;
                childIndex = GetChildIndex(parentIndex);
            }
            else
                break;
        }

        return result; 
    }

    public int Count => storage.Count;
    public bool HasElements => storage.Count > 0;
    int GetParentIndex(int index) => (index - 1) / 2;
    int GetChildIndex(int index)
    {
        int firstChild = index * 2 + 1;
        int secondChild = index * 2 + 2;
        if (firstChild >= storage.Count)
            return -1;
        if (firstChild == storage.Count - 1)
            return firstChild;
        return HasPriority(storage[firstChild], storage[secondChild]) ? firstChild : secondChild;
    }
    public Heap(Func<T, T, bool> _hasPriority) => HasPriority = _hasPriority;
}
