using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public class LatticeMaker : MonoBehaviour
{
    static LatticeMaker t; 
    [SerializeField, Range(0f, 1f)]
    float percentStay;
    [SerializeField]
    Transform prefab;
    [SerializeField]
    List<Int3> basisVectors;
    [SerializeField]
    int debugDrawNum = 100;
    [SerializeField]
    int numPoints = 200;
    [SerializeField]
    bool shouldCreateOrbs = true;
    [SerializeField]
    List<PointData> points = new List<PointData>();
    public static List<PointData> Points => t.points;
    bool updateQueued = false;

    private void OnDrawGizmosSelected()
    {
        basisVectors?.ForEach(vec => Gizmos.DrawLine(transform.position, transform.position + vec.ToVector()));
        points?.ForEach(point => Gizmos.DrawWireSphere(point.Position + transform.position, 1));
    }
    [Button(ButtonSizes.Large), GUIColor(0f, .3f, .8f)]
    void GenerateDebugPoints()
    {
        points = MakePoints(debugDrawNum);
    }
    List<PointData> MakePoints(int leftToMake)
    {
        HashSet<Int3> usedCoord = new HashSet<Int3>();
        List<PointData> result = new List<PointData>();
        Queue<PointData> queue = new Queue<PointData>();
        var seed = new PointData(basisVectors.Select(_ => 0).ToList(), basisVectors);
        queue.Enqueue(seed);
        usedCoord.Add(seed.Coord); 
        while(leftToMake > 0 && queue.Count > 0) { 
            var point = queue.Dequeue();
            var weighting = (Mathf.Max(Mathf.Log(result.Count + 1), 1) / Mathf.Max(queue.Count * queue.Count, 1));
            if (Random.Range(0f, 1f) - weighting < percentStay)
            {
                result.Add(point);
                leftToMake--;
                point.GenerateNeighbors(basisVectors).ForEach(neighbor => {
                    if(usedCoord.WasAbleToAdd(neighbor.Coord))
                        queue.Enqueue(neighbor);
                });
            }
        }
        return result;
    }
    [Button]
    void ManuallyUpdateBasis()
    {
        UpdatePoints(basisVectors);
    }
    public static Int3 GetBasisVector(int index) => t.basisVectors[index];

    internal static void UpdateBasis(int associatedVector, Vector3 normalized, float magnitude)
    {
        if (t == null)
            return;
        t.basisVectors[associatedVector] = Int3.FromVector(normalized * magnitude);
        if (!t.updateQueued)
        {
            t.updateQueued = true;
            Callback.Create(() =>
            {
                t.UpdatePoints(t.basisVectors);
                t.updateQueued = false;
            }, 1f);
        }
    }

    void UpdatePoints(List<Int3> basisVectors)=>
        points?.ForEach(point => point.UpdateBasis(basisVectors));

    private void Start()
    {
        if (shouldCreateOrbs)
        {
            points.ForEach(point =>
            {
                var trans = Instantiate(prefab);
                trans.position = point.Coord.ToVector() + transform.position;
                trans.SetParent(transform, true);
                point.Transform = trans;
            });
        }
        GridManager.Setup(points);
        Generator.PlaceThings();
    }
    private void Awake()
    {
        t = this;
        points = MakePoints(numPoints);
    }
}
