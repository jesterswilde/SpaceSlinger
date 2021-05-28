using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Generator: MonoBehaviour
{
    static Generator t;

    [SerializeField]
    List<Placement> thingsToPlace;
    [SerializeField]
    List<Cell> cells = new List<Cell>();
    public static void PlaceThings()
    {
        t.cells = t.thingsToPlace.Select(thing => Procedural.Place(thing)).ToList();
    }
    private void Awake()
    {
        t = this;
    }
}
