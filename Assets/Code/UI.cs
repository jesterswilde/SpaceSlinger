using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : SerializedMonoBehaviour
{
    [SerializeField]
    Dictionary<ItemType, ItemIcon> icons = new Dictionary<ItemType, ItemIcon>();

    void ItemPickedUp(Item item)
    {
        if (icons.ContainsKey(item.Type))
            icons[item.Type].SetActive(true);
    }
    private void Start()
    {
        Player.Inventory.OnItemAdded += ItemPickedUp;
    }
}
