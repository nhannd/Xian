using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public class InteractiveMultiLineGraphic : InteractiveGraphic
	{
		private MultiLineAnchorPointsGraphic _anchorPointsGraphic = new MultiLineAnchorPointsGraphic();
		private int _maxAnchorPoints;
		
		private CursorToken _moveToken;

		public InteractiveMultiLineGraphic(bool userCreated, int numberOfPoints)
			: base(userCreated)
		{
			_maxAnchorPoints = numberOfPoints;
			BuildGraphic();
		}

		public int MaximumAnchorPoints
		{
			get { return _maxAnchorPoints; }
			protected set { _maxAnchorPoints = value; }
		}

		public MultiLineAnchorPointsGraphic AnchorPoints
		{
			get { return _anchorPointsGraphic; }
		}

		public CursorToken MoveToken
		{
			get { return _moveToken; }
			set { _moveToken = value; }
		}

		#region IMemorable Members

		public override IMemento CreateMemento()
		{
			return this.AnchorPoints.CreateMemento();
		}

		public override void SetMemento(IMemento memento)
		{
			this.AnchorPoints.SetMemento(memento);
		}

		#endregion

		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			
			for (int i = 0; i < this.AnchorPoints.Count; i++)
				this.AnchorPoints[i] += del;
#else
			for (int i = 0; i < this.AnchorPoints.Count; i++)
				this.AnchorPoints[i] += delta;
#endif
		}

		public override GraphicState CreateCreateState()
		{
			return new CreateMultiLineGraphicState(this);
		}

		public override bool HitTest(XMouseEventArgs e)
		{
			return this.AnchorPoints.HitTest(e);
		}

		public override bool SetCursorToken(XMouseEventArgs e)
		{
			if (base.SetCursorToken(e))
				return true;

			if (this.HitTest(e))
			{
				e.SelectedTile.CursorToken = this.MoveToken;
				return true;
			}

			return false;
		}

		// This acts as a mediator.  It listens for changes in the anchor points
		// and make corresponding changes in the position of the control points.
		protected void OnAnchorPointChanged(object sender, AnchorPointEventArgs e)
		{
			base.ControlPoints[e.AnchorPointIndex] = e.AnchorPoint;
			Trace.Write(String.Format("OnAnchorPointChanged: {0}, {1}\n", e.AnchorPointIndex, e.AnchorPoint.ToString()));
		}

		protected override void OnControlPointChanged(object sender, ControlPointEventArgs e)
		{
			this.AnchorPoints[e.ControlPointIndex] = e.ControlPoint;
			Trace.Write(String.Format("OnControlPointChanged: {0}, {1}\n", e.ControlPointIndex, e.ControlPoint.ToString()));
		}

		private void BuildGraphic()
		{
			base.Graphics.Add(_anchorPointsGraphic);
			_anchorPointsGraphic.AnchorPointChangedEvent += new EventHandler<AnchorPointEventArgs>(OnAnchorPointChanged);

			// Add two points to begin with
			this.AnchorPoints.Add(new PointF(0, 0));
			base.ControlPoints.Add(new PointF(0, 0));
			this.AnchorPoints.Add(new PointF(0, 0));
			base.ControlPoints.Add(new PointF(0, 0));
		}
	}
}
