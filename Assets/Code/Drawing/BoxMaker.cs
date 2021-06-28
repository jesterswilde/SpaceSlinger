using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drawing
{
    public class BoxMaker : MonoBehaviour, ISketchMode
    {
        [SerializeField]
        float gridSize;
        [SerializeField]
        float depth;
        [SerializeField]
        Transform boxPrefab;
        public bool DrawingShouldBeClosed => true; 
        public void UseFinishedDrawing(List<LinePoint> linePoints)
        {
            var points = linePoints.Select(l => l.Point).ToList();
            if (IsBox(points))
            {
                var bounds = GetBoxBounds(points);
                MakeBoxInBounds(bounds);
            }
        }
        bool IsBox(List<Vector3> points)
        {
            if (points.Count < 4)
                return false;
            Direction curDir = Direction.None;
            var origin = points[0];
            var lastPoint = VecToGridSizedInt(Vector3.zero);
            var lastCorner = VecToGridSizedInt(Vector3.zero);
            HashSet<Direction> completedDirs = new HashSet<Direction>();
            for (int i = 1; i < points.Count; i++)
            {
                Int3 point = VecToGridSizedInt(points[i] - origin);
                var diff = point - lastPoint;
                var pointDir = DirFromDiff(diff);
                if (curDir == Direction.None || (pointDir != Direction.None && curDir != pointDir))
                {
                    if (AreOpposite(curDir, pointDir))
                    {
                        var dirFromConer = DirFromDiff(point - lastCorner);
                        if (dirFromConer == pointDir)
                            curDir = pointDir;
                    }
                    else
                    {
                        if (curDir != Direction.None)
                            completedDirs.Add(curDir);
                        lastCorner = point;
                        curDir = pointDir;
                    }
                }
                lastPoint = point;
            }
            completedDirs.Add(curDir);
            return completedDirs.Count == 4;
        }
        Bounds GetBoxBounds(List<Vector3> vecs)
        {
            var center = vecs.Aggregate((total, vec) => total + vec) / vecs.Count;
            Bounds bounds = new Bounds();
            bounds.center = center;
            var maxX = vecs.Max((vec) => vec.x);
            var maxY = vecs.Max((vec) => vec.y);
            var minX = vecs.Min((vec) => vec.x);
            var minY = vecs.Min((vec) => vec.y);
            bounds.size = new Vector3(maxX - minX, maxY - minY, depth);
            return bounds;
        }
        Transform MakeBoxInBounds(Bounds bounds)
        {
            var cube = Instantiate(boxPrefab);
            cube.position = bounds.center + Vector3.forward * depth;
            cube.forward = CameraController.Forward;
            cube.localScale = bounds.size;
            return cube;
        }
        Int3 VecToGridSizedInt(Vector3 vec) => Int3.FromInts(Mathf.RoundToInt(vec.x / gridSize), Mathf.RoundToInt(vec.y / gridSize), Mathf.RoundToInt(vec.z / gridSize));
        Direction DirFromDiff(Int3 diff) => diff switch
        {
            var val when val.X >= 2 => Direction.Right,
            var val when val.X <= -2 => Direction.Left,
            var val when val.Y >= 2 => Direction.Up,
            var val when val.Y <= -2 => Direction.Down,
            _ => Direction.None
        };

        bool AreOpposite(Direction a, Direction b) => (a, b) switch
        {
            (Direction.Up, Direction.Down) => true,
            (Direction.Down, Direction.Up) => true,
            (Direction.Left, Direction.Right) => true,
            (Direction.Right, Direction.Left) => true,
            _ => false
        };

        public void StreamLinePoints(LinePoint point)
        {
        }

        public void BeginStream() { }

        public void EndStream() { }

        enum Direction
        {
            Up,
            Right,
            Down,
            Left,
            None
        }
    }
}
