using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    bool destroyOnPickup = true;
    [SerializeField]
    Sprite inventoryIcon;
    [SerializeField]
    ItemType ItemType;

    public void GotPickedUp()
    {
        Player.Inventory.AddItem(ToItem());
        if (destroyOnPickup)
            Destroy(gameObject);
    }
    Item ToItem() => new Item() { Sprite = inventoryIcon, Type = ItemType };
}
