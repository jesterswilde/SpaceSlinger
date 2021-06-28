using UnityEngine;

public class Item
{
    public Sprite Sprite;
    public ItemType Type;
}

public enum ItemType
{
    Blue = 0,
    Green = 1,
    Orange = 2,
    BasisVector = 3,
}
