using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gravity : SerializedMonoBehaviour
{
    static Gravity t;
    Planet orientingPlanet;
    public static Planet OrientingPlanet => t.orientingPlanet;
    [SerializeField]
    Vector3 orientation => Orientation;
    [SerializeField]
    Vector3 ambientGravity;
    public static Vector3 AmbientGravity => t.ambientGravity;
    [SerializeField]
    List<float> influence;

    public static Vector3 Orientation { get {
            if (t.orientingPlanet != null && t.orientingPlanet.Influence >= 1f)
                return (t.orientingPlanet.transform.position - Player.Transform.position).normalized;
            return AmbientGravity.normalized;
        } }
    [SerializeField]
    List<Planet> planets = new List<Planet>();
    bool shouldUpdate = false;
    public static Planet GetNearestPlanet()=>
        t.planets.Count == 0 ? null : t.planets.MinBy((planet) => Vector3.Distance(planet.transform.position, Player.Transform.position));
    internal static void AddPlanet(Planet planet)
    {
        t.planets.Add(planet);
    }

    internal static void RemovePlanet(Planet planet)
    {
        t.planets.Remove(planet);
    }

    internal static void SetDirty()
    {
        t.shouldUpdate = true; 
    }


    void UpdateGravity()
    {
        shouldUpdate = false;
        var planetInfluence = Mathf.Min(1, planets.Aggregate(0f, (total, planet) => planet.Influence + total));
        var baseGravity = (1 - planetInfluence) * ambientGravity;
        var planetaryGravity = planets.Aggregate(Vector3.zero, (force, planet) => planet.GravityForce + force);
        Physics.gravity = planetaryGravity + baseGravity;
        orientingPlanet = GetNearestPlanet();
    }
    private void FixedUpdate()
    {
        if (shouldUpdate)
            UpdateGravity();
    }
    private void Start()
    {
        Player.T.Position.OnChange += (a,b) => SetDirty();
    }
    private void Awake()
    {
        t = this;
    }
}
