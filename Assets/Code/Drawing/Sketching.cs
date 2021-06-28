using Obi;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drawing
{
    public class Sketching : Equipment
    {
        [SerializeField]
        bool alwaysActive = false;
        [SerializeField]
        bool isActive = false;
        [SerializeField]
        ISketchMode mode;
        [SerializeField]
        SmoothDrawTool drawTool;
        Delta<bool> isDrawing = new Delta<bool>();
        void FinishedDrawing(List<LinePoint> drawing)
        {
            mode?.UseFinishedDrawing(drawing);
        }
        void StreamPointInDrawing(LinePoint point)
        {
            mode?.StreamLinePoints(point);
        }
        public override void Equip()
        {
            if (mode == null)
                throw new Exception("No drawing mode");
            isActive = true;
            drawTool.ShouldCloseDrawing = mode.DrawingShouldBeClosed;
        }
        public override void Unequip()
        {
            isActive = false;
            drawTool.CancelDrawing();
            if (isDrawing.Value)
                mode?.EndStream();
        }
        public override void Activate()=> isDrawing.Update(true);
        public override void Deactivate()=> isDrawing.Update(false);
        private void Update()
        {
            if (isActive)
                drawTool.Use(isDrawing.Value);
        }
        private void Awake()
        {
            isDrawing.Update(false);
        }
        void OnDrawingChange(bool state, bool oldState)
        {
            if (state)
                mode?.BeginStream();
            else
                mode?.EndStream();
        }
        protected override void Start()
        {
            base.Start();
            if (alwaysActive)
                Equip();
            drawTool.OnFinishDrawing += FinishedDrawing;
            drawTool.OnAddedPoint += StreamPointInDrawing;
            isDrawing.OnChange += OnDrawingChange;
        }
    }
}
