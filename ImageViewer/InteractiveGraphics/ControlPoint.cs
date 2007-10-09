using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A graphical representation of the "handles" that allow 
	/// the user to move and resize <see cref="InteractiveGraphic"/>s.
	/// </summary>
	public class ControlPoint : CompositeGraphic
	{
		#region Private fields

		private int _index;
		private PointF _location;
		private InvariantRectanglePrimitive _rectangle;
		private event EventHandler<ControlPointEventArgs> _locationChangedEvent;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ControlPoint"/>.
		/// </summary>
		/// <param name="index">The index at which a <see cref="ControlPoint"/>
		/// is inserted into a <see cref="ControlPointGroup"/>.
		/// </param>
		internal ControlPoint(int index)
		{
			Platform.CheckNonNegative(index, "index");

			_index = index;
			_rectangle = new InvariantRectanglePrimitive();
			_rectangle.InvariantTopLeft = new PointF(-4, -4);
			_rectangle.InvariantBottomRight = new PointF(4, 4);
			this.Graphics.Add(_rectangle);
		}

		/// <summary>
		/// Gets or sets the location of the control point.
		/// </summary>
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
					EventsHelper.Fire(_locationChangedEvent, this, new ControlPointEventArgs(_index, this.Location));
				}
			}
		}

		/// <summary>
		/// Gets or sets the colour of the control point.
		/// </summary>
		public Color Color
		{
			get { return _rectangle.Color; }
			set { _rectangle.Color = value; }
		}
	
		/// <summary>
		/// Occurs when the location of the control point has changed.
		/// </summary>
		public event EventHandler<ControlPointEventArgs> LocationChanged
		{
			add { _locationChangedEvent += value; }
			remove { _locationChangedEvent -= value; }
		}

		/// <summary>
		/// This method overrides <see cref="Graphic.HitTest"/>.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			PointF ptMouse = this.SpatialTransform.ConvertToSource(point);

			// TODO: Should call _rectangle.HitTest instead
			_rectangle.CoordinateSystem = CoordinateSystem.Source;

			RectF srcRect = new RectF();
			srcRect.TopLeft = _rectangle.TopLeft;
			srcRect.BottomRight = _rectangle.BottomRight;

			_rectangle.ResetCoordinateSystem();

			return srcRect.Contains(ptMouse);
		}
	}
}
