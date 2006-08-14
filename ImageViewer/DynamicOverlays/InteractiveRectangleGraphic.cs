using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for RulerOverlay.
	/// </summary>
	public class InteractiveRectangleGraphic : InteractiveGraphic
	{
		private const int _topLeft = 0;
		private const int _topRight = 1;
		private const int _bottomLeft = 2;
		private const int _bottomRight = 3;

		private RectanglePrimitive _rectangleGraphic;

		public InteractiveRectangleGraphic(bool userCreated)
			: base(userCreated)
		{
			BuildGraphic();
		}

		public PointF TopLeft
		{
			get { return _rectangleGraphic.TopLeft; }
			set { _rectangleGraphic.TopLeft = value; }
		}

		public PointF BottomRight
		{
			get { return _rectangleGraphic.BottomRight; }
			set { _rectangleGraphic.BottomRight = value; }
		}

		#region IMemorable Members

		public override IMemento CreateMemento()
		{
			RectangleMemento memento = new RectangleMemento();

			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;
			memento.TopLeft = this.TopLeft;
			memento.BottomRight = this.BottomRight;
			this.ResetCoordinateSystem();

			return memento;
		}

		public override void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			RectangleMemento graphicMemento = memento as RectangleMemento;
			Platform.CheckForInvalidCast(graphicMemento, "memento", "RectangleMemento");

			this.CoordinateSystem = CoordinateSystem.Source;
			this.TopLeft = graphicMemento.TopLeft;
			this.BottomRight = graphicMemento.BottomRight;
			this.ResetCoordinateSystem();
		}

		#endregion

		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			this.TopLeft += del;
			this.BottomRight += del;
#else
			this.TopLeft += delta;
			this.BottomRight += delta;
#endif
		}

		public override GraphicState CreateCreateState()
		{
			return new CreateRectangleGraphicState(this);
		}

		public override bool HitTest(XMouseEventArgs e)
		{
			return _rectangleGraphic.HitTest(e);
		}

		private void BuildGraphic()
		{
			AddRectangle();
			AddControlPoints();
		}

		private void AddRectangle()
		{
			_rectangleGraphic = new RectanglePrimitive();
			base.Graphics.Add(_rectangleGraphic);
			_rectangleGraphic.TopLeft = new PointF(0, 0);
			_rectangleGraphic.BottomRight = new PointF(0, 0);
			_rectangleGraphic.TopLeftChanged += new EventHandler<PointChangedEventArgs>(OnTopLeftChanged);
			_rectangleGraphic.BottomRightChanged += new EventHandler<PointChangedEventArgs>(OnBottomRightChanged);
		}

		private void AddControlPoints()
		{
			for (int i = 0; i < 4; i++)
				base.ControlPoints.Add(new PointF(0, 0));
		}

		// This acts as a mediator.  It listens for changes in the anchor points
		// and make corresponding changes in the position of the control points.
		protected void OnTopLeftChanged(object sender, PointChangedEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnTopLeftChanged: {0}\n", e.Point.ToString()));

			base.ControlPoints[_topLeft] = e.Point;
			base.ControlPoints[_topRight] = new PointF(this.BottomRight.X, this.TopLeft.Y);
			base.ControlPoints[_bottomLeft] = new PointF(this.TopLeft.X, this.BottomRight.Y);
		}

		protected void OnBottomRightChanged(object sender, PointChangedEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnBottomRightChanged: {0}\n", e.Point.ToString()));

			base.ControlPoints[_topRight] = new PointF(this.BottomRight.X, this.TopLeft.Y);
			base.ControlPoints[_bottomLeft] = new PointF(this.TopLeft.X, this.BottomRight.Y);
			base.ControlPoints[_bottomRight] = e.Point;
		}

		protected override void OnControlPointChanged(object sender, ControlPointEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnControlPointChanged: index = {0}, {1}\n", e.ControlPointIndex, e.ControlPoint.ToString()));

			switch (e.ControlPointIndex)
			{
				case _topLeft:
					this.TopLeft = e.ControlPoint;
					break;
				case _topRight:
					this.TopLeft = new PointF(this.TopLeft.X, e.ControlPoint.Y);
					this.BottomRight = new PointF(e.ControlPoint.X, this.BottomRight.Y);
				    break;
				case _bottomLeft:
					this.TopLeft = new PointF(e.ControlPoint.X, this.TopLeft.Y);
					this.BottomRight = new PointF(this.BottomRight.X, e.ControlPoint.Y);
				    break;
				case _bottomRight:
					this.BottomRight = e.ControlPoint;
					break;
			}
		}
	}
}
