using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drawing
{

    public class SmoothDrawTool : MonoBehaviour
    {
        [SerializeField]
        float safeRadius = 1f;
        Delta<bool> isDrawing = new Delta<bool>();
        [SerializeField]
        LineRenderer line;
        [SerializeField]
        LineRenderer freeSegment;
        [SerializeField]
        List<LinePoint> drawnPoints;
        Vector3 lastPoint => drawnPoints?[drawnPoints.Count - 1].Point ?? Vector3.zero;
        public event Action<List<LinePoint>> OnFinishDrawing;
        public event Action<LinePoint> OnAddedPoint;
        float drawTime;
        public bool ShouldCloseDrawing { get; set; } = true;

        public void Use(bool isActive)
        {
            isDrawing.Update(isActive);
            if (isDrawing.Changed)
            {
                if (isDrawing.Value)
                    BeginDrawing();
                else if(drawnPoints.Count != 0)
                    FinishDrawing();
            }
            if (isDrawing.Value)
                Draw();
        }

        void Draw()
        {
            var linePoint = MakeLinePoint();
            freeSegment.SetPosition(2, linePoint.Point);
            if (Vector3.Distance(linePoint.Point, lastPoint) > safeRadius)
            {
                drawnPoints.Add(linePoint);
                OnAddedPoint?.Invoke(linePoint);
                UpdateDrawing();
            }
            drawTime += Time.deltaTime;
        }
        void BeginDrawing()
        {
            drawTime = 0f;
            drawnPoints = new List<LinePoint>();
            drawnPoints.Add(MakeLinePoint());
            freeSegment.enabled = true;
            freeSegment.positionCount = 3;
            freeSegment.SetPositions(new Vector3[] { lastPoint, lastPoint, lastPoint });
        }
        void FinishDrawing()
        {
            if (ShouldCloseDrawing)
            {
                drawnPoints.Add(drawnPoints[0]);
                OnAddedPoint?.Invoke(drawnPoints[0]);
            }
            UpdateDrawing();
            freeSegment.enabled = false;
            OnFinishDrawing?.Invoke(drawnPoints);

        }
        void UpdateDrawing()
        {
            line.enabled = true;
            line.positionCount = drawnPoints.Count;
            line.SetPositions(drawnPoints.Select(p => p.Point).ToArray());
            var secondToLast = drawnPoints.Count >= 2 ? drawnPoints[drawnPoints.Count - 2].Point : drawnPoints[0].Point;
            freeSegment.SetPosition(0, secondToLast);
            freeSegment.SetPosition(1, lastPoint);
        }
        public void CancelDrawing()
        {
            drawnPoints.Clear();
            line.enabled = false;
            freeSegment.enabled = false;
        }
        LinePoint MakeLinePoint() => new LinePoint(GetCursorPoint(), drawTime);
        Vector3 GetCursorPoint() => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 30));
    }

    [Serializable]
    public class LinePoint
    {
        public Vector3 Point;
        public float CreatedAt;
        public LinePoint(Vector3 point, float curTime)
        {
            Point = point;
            CreatedAt = curTime;
        }
    }

    // FINISH LATER
    //This class should animate the lines that surround a box to be the outline
    public class LineToBoxAnim : MonoBehaviour
    {
        LineRenderer line;
        int upperLeft;
        int upperRight;
        int lowerLeft;
        int lowerRight;
        public void Setup(List<Vector3> points)
        {
            var pointsWithI = points.Select((point, i) => (point, i));
            Vector3 center = points.Aggregate((total, point) => point + total) / points.Count;
            upperLeft = pointsWithI.Where(item => item.point.x < center.x && item.point.y > center.y).MaxBy(item => Vector3.SqrMagnitude(center - item.point)).i;
            upperRight = pointsWithI.Where(item => item.point.x > center.x && item.point.y > center.y).MaxBy(item => Vector3.SqrMagnitude(center - item.point)).i;
            lowerLeft = pointsWithI.Where(item => item.point.x < center.x && item.point.y < center.y).MaxBy(item => Vector3.SqrMagnitude(center - item.point)).i;
            lowerRight = pointsWithI.Where(item => item.point.x > center.x && item.point.y < center.y).MaxBy(item => Vector3.SqrMagnitude(center - item.point)).i;
        }
        void MakeStrips(int ul, int ur, int ll, int lr, List<Vector3> points)
        {

        }
        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }
    }
}
