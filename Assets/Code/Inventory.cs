using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event Action<Item> OnItemAdded;
    List<Item> heldItems = new List<Item>();
    public List<Item> HeldItems => heldItems;


    internal void AddItem(Item item)
    {
        heldItems.Add(item);
        OnItemAdded.Invoke(item);
    }
}
