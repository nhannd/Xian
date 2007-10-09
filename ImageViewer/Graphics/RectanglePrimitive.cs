using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive rectangle graphic.
	/// </summary>
	public class RectanglePrimitive : VectorGraphic
	{
		private RectangleF _rectangle = new RectangleF(0,0,0,0);
		private event EventHandler<PointChangedEventArgs> _topLeftChangedEvent;
		private event EventHandler<PointChangedEventArgs> _bottomRightChangedEvent;

		/// <summary>
		/// Initializes a new instance of <see cref="RectanglePrimitive"/>.
		/// </summary>
		public RectanglePrimitive() 
		{
		}

		/// <summary>
		/// Gets or sets the top-left corner of the rectangle
		/// in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF TopLeft
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _rectangle.Location;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_rectangle.Location);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.TopLeft, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_rectangle.Width = _rectangle.Right - value.X;
					_rectangle.Height = _rectangle.Bottom - value.Y;
					_rectangle.Location = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					_rectangle.Width = _rectangle.Right - base.SpatialTransform.ConvertToSource(value).X;
					_rectangle.Height = _rectangle.Bottom - base.SpatialTransform.ConvertToSource(value).Y;
					_rectangle.Location = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_topLeftChangedEvent, this, new PointChangedEventArgs(this.TopLeft));
			}
		}

		/// <summary>
		/// Gets or sets the bottom-right corner of the rectangle
		/// in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF BottomRight
		{
			get
			{
				PointF bottomRight = new PointF(_rectangle.Right, _rectangle.Bottom);

				if (base.CoordinateSystem == CoordinateSystem.Source)
					return bottomRight;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(bottomRight);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.BottomRight, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_rectangle.Width = value.X - _rectangle.X;
					_rectangle.Height = value.Y - _rectangle.Y;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					PointF pointSrc = base.SpatialTransform.ConvertToSource(value);

					_rectangle.Width = pointSrc.X - _rectangle.X;
					_rectangle.Height = pointSrc.Y - _rectangle.Y;
				}

				EventsHelper.Fire(_bottomRightChangedEvent, this, new PointChangedEventArgs(this.BottomRight));
			}
		}

		/// <summary>
		/// Gets the width of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Width
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _rectangle.Width;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(_rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(_rectangle.Right, _rectangle.Bottom));

					return bottomRight.X - topLeft.X;
				}
			}
		}

		/// <summary>
		/// Gets the height of the rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		public float Height
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _rectangle.Height;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

					PointF topLeft = base.SpatialTransform.ConvertToDestination(_rectangle.Location);
					PointF bottomRight = base.SpatialTransform.ConvertToDestination(new PointF(_rectangle.Right, _rectangle.Bottom));

					return bottomRight.Y - topLeft.Y;
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="TopLeft"/> property changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> TopLeftChanged
		{
			add { _topLeftChangedEvent += value; }
			remove { _topLeftChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="BottomRight"/> property changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> BottomRightChanged
		{
			add { _bottomRightChangedEvent += value; }
			remove { _bottomRightChangedEvent -= value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="RectanglePrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="RectanglePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the rectangle.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			double distance;
			PointF ptNearest = new PointF(0, 0);
			PointF ptMouse = point;

			// Always do the hit test in destination coordinates since we want the
			// "activation distance" to be the same irrespective of the zoom
			this.CoordinateSystem = CoordinateSystem.Destination;

			PointF ptTopLeft = this.TopLeft;
			PointF ptBottomRight = this.BottomRight;
			PointF ptTopRight = new PointF(ptBottomRight.X, ptTopLeft.Y);
			PointF ptBottomLeft = new PointF(ptTopLeft.X, ptBottomRight.Y);

			this.ResetCoordinateSystem();

			distance = Vector.DistanceFromPointToLine(point, ptTopLeft, ptTopRight, ref ptNearest);

			if (distance < HitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(point, ptTopRight, ptBottomRight, ref ptNearest);

			if (distance < HitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(point, ptBottomRight, ptBottomLeft, ref ptNearest);

			if (distance < HitTestDistance)
				return true;

			distance = Vector.DistanceFromPointToLine(point, ptBottomLeft, ptTopLeft, ref ptNearest);

			if (distance < HitTestDistance)
				return true;

			return false;
		}

		/// <summary>
		/// Moves the <see cref="RectanglePrimitive"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
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

		/// <summary>
		/// Returns a value indicating whether the specified point is contained
		/// in the rectangle.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool Contains(PointF point)
		{
			if (base.CoordinateSystem == CoordinateSystem.Source)
				return _rectangle.Contains(point);
			else
			{
				Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
				return _rectangle.Contains(base.SpatialTransform.ConvertToSource(point));
			}
		}
	}
}
