using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : SerializedMonoBehaviour
{
    static UI t;
    [SerializeField]
    Dictionary<ItemType, ItemIcon> icons = new Dictionary<ItemType, ItemIcon>();
    [SerializeField]
    Canvas canvas;
    static Canvas defaultCanvas;
    public static Canvas Canvas { get {
            if (t != null)
                return t.canvas;
            defaultCanvas ??= FindObjectOfType<Canvas>();
            return defaultCanvas;
        } }

    void ItemPickedUp(Item item)
    {
        if (icons.ContainsKey(item.Type))
            icons[item.Type].SetActive(true);
    }
    private void Start()
    {
        Player.Inventory.OnItemAdded += ItemPickedUp;
    }
    private void Awake()
    {
        t = this;
    }
}
