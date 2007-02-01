using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class ControlPoint : CompositeGraphic
	{
		private int _index;
		private PointF _location;
		private InvariantRectanglePrimitive _rectangle;
		private event EventHandler<ControlPointEventArgs> _controlPointChangedEvent;

		public ControlPoint(int index)
		{
			_index = index;
			_rectangle = new InvariantRectanglePrimitive();
			_rectangle.InvariantTopLeft = new PointF(-4, -4);
			_rectangle.InvariantBottomRight = new PointF(4, 4);
			this.Graphics.Add(_rectangle);
		}

		public PointF Location
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _location;
				else
					return base.SpatialTransform.ConvertToDestination(_location);
			}
			set
			{
				if (!FloatComparer.AreEqual(this.Location, value))
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					if (base.CoordinateSystem == CoordinateSystem.Source)
						_location = value;
					else
						_location = base.SpatialTransform.ConvertToSource(value);

					Trace.Write(String.Format("Control Point: {0}\n", _location.ToString()));

					_rectangle.AnchorPoint = this.Location;
					EventsHelper.Fire(_controlPointChangedEvent, this, new ControlPointEventArgs(_index, this.Location));
				}
			}
		}

		public Color Color
		{
			get { return _rectangle.Color; }
			set { _rectangle.Color = value; }
		}
	

		public event EventHandler<ControlPointEventArgs> ControlPointChanged
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
		}

		public override bool HitTest(Point point)
		{
			PointF ptMouse = this.SpatialTransform.ConvertToSource(point);

			_rectangle.CoordinateSystem = CoordinateSystem.Source;

			RectF srcRect = new RectF();
			srcRect.TopLeft = _rectangle.TopLeft;
			srcRect.BottomRight = _rectangle.BottomRight;

			_rectangle.ResetCoordinateSystem();

			return srcRect.Contains(ptMouse);
		}
	}
}
