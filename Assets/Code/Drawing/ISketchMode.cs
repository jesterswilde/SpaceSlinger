using System.Collections.Generic;

namespace Drawing
{
    public interface ISketchMode
    {
        void UseFinishedDrawing(List<LinePoint> points);
        void StreamLinePoints(LinePoint point);
        void BeginStream();
        void EndStream();
        bool DrawingShouldBeClosed { get; }
    }
}
