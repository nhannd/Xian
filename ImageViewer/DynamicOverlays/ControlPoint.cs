using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for RulerOverlay.
	/// </summary>
	public class ControlPoint : Graphic
	{
		private int _index;
		private PointF _location;
		private InvariantRectanglePrimitive _rectElement;
		private event EventHandler<ControlPointEventArgs> _controlPointChangedEvent;

		public ControlPoint(int index)
		{
			_index = index;
			_rectElement = new InvariantRectanglePrimitive();
			_rectElement.InvariantTopLeft = new PointF(-4, -4);
			_rectElement.InvariantBottomRight = new PointF(4, 4);
			base.Graphics.Add(_rectElement);
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

					_rectElement.AnchorPoint = this.Location;
					EventsHelper.Fire(_controlPointChangedEvent, this, new ControlPointEventArgs(_index, this.Location));
				}
			}
		}

		public event EventHandler<ControlPointEventArgs> ControlPointChanged
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
		}

		public override bool HitTest(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			PointF ptMouse = this.SpatialTransform.ConvertToSource(new PointF(e.X,e.Y));

			_rectElement.CoordinateSystem = CoordinateSystem.Source;

			RectF srcRect = new RectF();
			srcRect.TopLeft = _rectElement.TopLeft;
			srcRect.BottomRight = _rectElement.BottomRight;

			_rectElement.ResetCoordinateSystem();

			return srcRect.Contains(ptMouse);
		}
	}
}
