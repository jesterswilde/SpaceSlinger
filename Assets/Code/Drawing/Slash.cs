using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drawing
{
    public class Slash : MonoBehaviour, ISketchMode
    {
        [SerializeField]
        LayerMask mask;
        [SerializeField]
        float castDist = 100f;
        [SerializeField]
        Transform rayOrigin;
        [SerializeField]
        Transform castOrigin;
        [SerializeField]
        float raycastSpacing = 1f;
        [SerializeField]
        bool shouldDebug = true;
        public bool DrawingShouldBeClosed => false;
        float dist = 0f;
        LinePoint prevPoint;
        LinePoint nextPoint;
        HashSet<Transform> prevSeen = new HashSet<Transform>();

        public void StreamLinePoints(LinePoint streamedPoint)
        {
            Debug.Log("Streaming point");
            var strikes = CollectStrikesFromStream(streamedPoint);
            Debug.Log($"Strike count: {strikes.Count}");
            foreach(var strike in strikes)
                strike.ThingHit.GetComponent<FloatingPlatform>()?
                    .GotHit(strike.Force);
        }
        List<Strike> CollectStrikesFromStream(LinePoint streamedPoint)
        {
            Debug.Log($"SP: {streamedPoint} NP: {nextPoint} PP: {prevPoint}");
            prevPoint = nextPoint;
            nextPoint = streamedPoint;
            List<Strike> strikes = new List<Strike>();
            if (prevPoint == null)
                return strikes;
            Vector3 dir = (nextPoint.Point - prevPoint.Point).normalized;
            Vector3 force = dir / (prevPoint.CreatedAt - nextPoint.CreatedAt) * -1;
            float totalSegmentDist = Vector3.Distance(nextPoint.Point, prevPoint.Point);
            Debug.Log($"Checking if should cast \n {dist} {totalSegmentDist}");
            while (dist < totalSegmentDist)
            {
                var point = prevPoint.Point + (dist * dir);
                var objs = RaycastFromPoint(point);
                if(objs.Count > 0)
                {
                    var newObjs = objs.Where(obj => !prevSeen.Contains(obj)).ToList();
                    foreach(var thingHit in newObjs) { 
                        var strike = new Strike() { Force = force, ThingHit = thingHit };
                        strikes.Add(strike);
                    };
                }
                prevSeen = objs.ToHashSet();
                dist += raycastSpacing;
            }
            dist -= totalSegmentDist;
            return strikes;
        }

        public void UseFinishedDrawing(List<LinePoint> points)
        {
        }

        List<Transform> RaycastFromPoint(Vector3 point)
        {
            Debug.Log("Raycasting");
            Ray ray = new Ray(castOrigin.position, point - rayOrigin.position);
            var thingsHit = Physics.RaycastAll(ray, castDist, mask);
            if (shouldDebug)
            {
                //var screenSpace = Camera.main.WorldToScreenPoint(point);
                //ray = Camera.main.ScreenPointToRay(screenSpace);
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.position = ray.origin + ray.direction * 30f;
                Callback.Create(() => Destroy(obj), 2f);
            }
            return thingsHit.Select(hit => hit.collider.gameObject.transform).ToList();
        }

        public void BeginStream()
        {
            Debug.Log("Beggining stream");
            prevPoint = null;
            nextPoint = null;
            dist = 0f;
        }

        public void EndStream() { }

        struct Strike
        {
            public Transform ThingHit;
            public Vector3 Force;
        }
    }
}
