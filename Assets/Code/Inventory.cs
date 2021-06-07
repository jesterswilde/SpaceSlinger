using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event Action<Item> OnItemAdded;
    public List<Item> HeldItems { get; } = new List<Item>();


    internal void AddItem(Item item)
    {
        HeldItems.Add(item);
        OnItemAdded.Invoke(item);
    }
}
