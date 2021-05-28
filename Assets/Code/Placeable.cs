using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public ThingType Type;
    public Cell PlacedInCell;
}

public enum ThingType
{
    Planet,
    Checkpoint
}
