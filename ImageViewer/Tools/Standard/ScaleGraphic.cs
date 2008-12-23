using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Generic scale graphic class.
	/// </summary>
	internal class ScaleGraphic : CompositeGraphic
	{
		private event EventHandler _changed;
		private readonly LinePrimitive _baseLine;
		private readonly List<LinePrimitive> _ticklines = new List<LinePrimitive>();
		private PointF _point1, _point2;
		private bool _isMirrored;
		private bool _visible;

		/// <summary>
		/// Constructs a <see cref="ScaleGraphic"/>.
		/// </summary>
		public ScaleGraphic()
		{
			base.Graphics.Add(_baseLine = new LinePrimitive());
		}

		/// <summary>
		/// Gets the minor tick spacing in millimetres.
		/// </summary>
		/// <remarks>
		/// The base implementation of <see cref="ScaleGraphic"/> has a fixed minor tick spacing of 1.0 cm.
		/// </remarks>
		public virtual float MinorTick
		{
			get { return 10; }
		}

		/// <summary>
		/// Gets the major tick spacing in millimetres.
		/// </summary>
		/// <remarks>
		/// The base implementation of <see cref="ScaleGraphic"/> has a fixed major tick spacing of 5.0 cm.
		/// </remarks>
		public virtual float MajorTick
		{
			get { return 50; }
		}

		/// <summary>
		/// Gets the minor tick length in client-space pixels.
		/// </summary>
		/// <remarks>
		/// The base implementation of <see cref="ScaleGraphic"/> has a fixed minor tick length of 15 pixels.
		/// </remarks>
		public virtual float MinorTickLength
		{
			get { return 15; }
		}

		/// <summary>
		/// Gets the major tick length in client-space pixels.
		/// </summary>
		/// <remarks>
		/// The base implementation of <see cref="ScaleGraphic"/> has a fixed major tick length of 25 pixels.
		/// </remarks>
		public virtual float MajorTickLength
		{
			get { return 25; }
		}

		/// <summary>
		/// Gets or sets a value indicating that the ticks are drawn in the opposite direction.
		/// </summary>
		public bool IsMirrored
		{
			get { return _isMirrored; }
			set
			{
				if (_isMirrored != value)
				{
					_isMirrored = value;
					OnChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets an endpoint of the scale's base line segment in the current coordinate space.
		/// </summary>
		public virtual PointF Point1
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point1;
				return base.SpatialTransform.ConvertToDestination(_point1);
			}
			set
			{
				if (base.CoordinateSystem != CoordinateSystem.Source)
					value = base.SpatialTransform.ConvertToSource(value);

				if (value != _point1)
				{
					_point1 = value;
					OnChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the other endpoint of the scale's base line segment in the current coordinate space.
		/// </summary>
		public virtual PointF Point2
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point2;
				return base.SpatialTransform.ConvertToDestination(_point2);
			}
			set
			{
				if (base.CoordinateSystem != CoordinateSystem.Source)
					value = base.SpatialTransform.ConvertToSource(value);

				if (value != _point2)
				{
					_point2 = value;
					OnChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating the visibility of the <see cref="IGraphic"/>.
		/// </summary>
		public override bool Visible
		{
			// use the private variable to allow the tick computation algorithm to hide itself independently of what client code specifies
			get { return _visible && base.Visible; }
			set
			{
				if (base.Visible != value || _visible != value)
				{
					base.Visible = value;
					_visible = value;
					OnChanged();
				}
			}
		}

		/// <summary>
		/// Indicates that the ScaleGraphic has changed.
		/// </summary>
		public event EventHandler Changed
		{
			add { _changed += value; }
			remove { _changed -= value; }
		}

		/// <summary>
		/// Sets both endpoints of the scale's base line segment in one atomic operation, cause only one update and draw.
		/// </summary>
		/// <param name="point1">One endpoint of the base line segment.</param>
		/// <param name="point2">The other endpoint of the base line segment.</param>
		public void SetEndPoints(PointF point1, PointF point2)
		{
			if (base.CoordinateSystem != CoordinateSystem.Source)
			{
				point1 = base.SpatialTransform.ConvertToSource(point1);
				point2 = base.SpatialTransform.ConvertToSource(point2);
			}

			if (point1 != _point1 || point2 != _point2)
			{
				_point1 = point1;
				_point2 = point2;
				OnChanged();
			}
		}

		/// <summary>
		/// Sets both endpoints of the scale's base line segment in one atomic operation, cause only one update and draw.
		/// </summary>
		/// <param name="location">One endpoint of the base line segment.</param>
		/// <param name="offset">The offset of the other endpoint relative to the first endpoint.</param>
		public void SetEndPoints(PointF location, SizeF offset)
		{
			this.SetEndPoints(location, location + offset);
		}

		/// <summary>
		/// Called when the <see cref="Changed"/> event is fired.
		/// </summary>
		protected virtual void OnChanged()
		{
			UpdateScale();
			EventsHelper.Fire(_changed, this, EventArgs.Empty);
		}

		/// <summary>
		/// Formats the scale's base line segment.
		/// </summary>
		/// <param name="baseLine">A reference to the <see cref="LinePrimitive"/> that is the base line segment.</param>
		/// <param name="endpoint1">One endpoint where the base line segment should be drawn.</param>
		/// <param name="endpoint2">The other endpoint where the base line segment should be drawn.</param>
		protected virtual void FormatBaseLine(LinePrimitive baseLine, PointF endpoint1, PointF endpoint2)
		{
			baseLine.Pt1 = endpoint1;
			baseLine.Pt2 = endpoint2;
		}

		/// <summary>
		/// Formats the scale's major ticks.
		/// </summary>
		/// <param name="tickLine">A reference to the <see cref="LinePrimitive"/> that is the tick.</param>
		/// <param name="point">The point along the base line segment where the tick begins.</param>
		/// <param name="unitNormal">A unit normal vector that is perpendicular to the base line segment.</param>
		protected virtual void FormatMajorTick(LinePrimitive tickLine, PointF point, SizeF unitNormal)
		{
			float length = this.MajorTickLength;
			if (this.IsMirrored)
				length = -length;
			tickLine.Pt1 = point;
			tickLine.Pt2 = new PointF(point.X + length*unitNormal.Width, point.Y + length*unitNormal.Height);
		}

		/// <summary>
		/// Formats the scale's minor ticks.
		/// </summary>
		/// <param name="tickLine">A reference to the <see cref="LinePrimitive"/> that is the tick.</param>
		/// <param name="point">The point along the base line segment where the tick begins.</param>
		/// <param name="unitNormal">A unit normal vector that is perpendicular to the base line segment.</param>
		protected virtual void FormatMinorTick(LinePrimitive tickLine, PointF point, SizeF unitNormal)
		{
			float length = this.MinorTickLength;
			if (this.IsMirrored)
				length = -length;
			tickLine.Pt1 = point;
			tickLine.Pt2 = new PointF(point.X + length*unitNormal.Width, point.Y + length*unitNormal.Height);
		}

		/// <summary>
		/// Recomputes and reformats the graphics that comprise the scale.
		/// </summary>
		/// <exception cref="UncalibratedImageException">Thrown if the parent image does not provide pixel spacing information.</exception>
		protected virtual void UpdateScale()
		{
			if (!_visible) // no point recomputing the scale if client code has made us invisible
				return;

			try
			{
				base.CoordinateSystem = CoordinateSystem.Destination;
				PointF pt0 = this.Point1;
				PointF pt1 = this.Point2;

				// draw base line
				FormatBaseLine(_baseLine, pt0, pt1);

				// compute normal to the base line
				TickOffset unitNormal = new TickOffset(pt0.Y - pt1.Y, pt1.X - pt0.X).GetUnitOffset();

				// compute tick marks
				IList<PointF> majorTicks;
				IList<PointF> minorTicks;
				ComputeTickMarks(out majorTicks, out minorTicks, pt0, pt1, false);

				// draw tick marks
				if (majorTicks.Count + minorTicks.Count > 1) // must be at least 2 ticks for this to be useful
				{
					IList<LinePrimitive> ticks = AllocateTicks(majorTicks.Count + minorTicks.Count);
					for (int n = 0; n < minorTicks.Count; n++)
					{
						FormatMinorTick(ticks[n], minorTicks[n], unitNormal);
					}
					for (int n = 0; n < majorTicks.Count; n++)
					{
						FormatMajorTick(ticks[minorTicks.Count + n], majorTicks[n], unitNormal);
					}

					if (!base.Visible)
						base.Visible = true;
				}
				else
				{
					base.Visible = false;
				}
			}
			finally
			{
				base.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Allocates sufficient tick <see cref="LinePrimitive"/>s, adding new objects and disposing extra objects as necessary.
		/// </summary>
		/// <param name="tickCount">The number of tick <see cref="LinePrimitive"/>s to allocate.</param>
		/// <returns></returns>
		protected IList<LinePrimitive> AllocateTicks(int tickCount)
		{
			LinePrimitive tick;
			while (_ticklines.Count < tickCount)
			{
				tick = new LinePrimitive();
				_ticklines.Add(tick);
				base.Graphics.Add(tick);
			}
			while (_ticklines.Count > tickCount)
			{
				tick = _ticklines[0];
				_ticklines.RemoveAt(0);
				base.Graphics.Remove(tick);
				tick.Dispose();
			}
			return _ticklines.AsReadOnly();
		}

		/// <summary>
		/// Computes positions of major and minor tick marks along the specified line segment.
		/// </summary>
		/// <remarks>
		/// For performance reasons the arguments are <b>not</b> validated. In particular, the line segment specified by the two points
		/// must be valid and non-trivial - specifying the same point (or close to the same point) for both end points produces indeterminate
		/// results.
		/// </remarks>
		/// <param name="majorTicks">Output variable to receive a list of major tick positions.</param>
		/// <param name="minorTicks">Output variable to receive a list of minor tick positions.</param>
		/// <param name="linePoint1">One endpoint of the line segment along which to compute tick positions.</param>
		/// <param name="linePoint2">The other endpoint of the line segment along which to compute tick positions.</param>
		/// <param name="allowOverlap">Specifies if minor ticks coincident with an existing major tick should be included in the results.</param>
		/// <exception cref="UncalibratedImageException">Thrown if the parent image does not provide pixel spacing information.</exception>
		protected void ComputeTickMarks(out IList<PointF> majorTicks, out IList<PointF> minorTicks, PointF linePoint1, PointF linePoint2, bool allowOverlap)
		{
			PointF p0 = base.SpatialTransform.ConvertToSource(linePoint1);
			PointF p1 = base.SpatialTransform.ConvertToSource(linePoint2);
			PointF pM = Vector.Midpoint(p0, p1);

			bool xyW;
			double len;
			double pxR, pxS;
			double pxW, pxH;
			GetPixelDimensions(out pxW, out pxH);
			if (!FloatComparer.AreEqual(p0.X, p1.X, 0.001f))
			{
				pxR = Math.Abs((p0.Y - p1.Y)/(p0.X - p1.X));
				pxS = 1/Math.Sqrt(pxW*pxW + pxH*pxH*pxR*pxR);
				xyW = false;
				len = Math.Abs(p0.X - p1.X) / pxS;
			}
			else
			{
				pxR = 0f;
				pxS = 1/Math.Sqrt(pxH*pxH + pxW*pxW*pxR*pxR);
				xyW = true;
				len = Math.Abs(p0.Y - p1.Y) * pxH;
			}

			List<PointF> listMajorTicks = new List<PointF>();
			List<PointF> listMinorTicks = new List<PointF>();

			if (this.MinorTick < len)
			{
				Dictionary<string, object> map = new Dictionary<string, object>();
				PointF ptPos, ptNeg;

				// compute major tick positions and index their positions
				if (this.MajorTick < len)
				{
					TickOffset majorOffset = TickOffset.CreateTickOffset(this.MajorTick*pxS, pxR, xyW);
					ptPos = ptNeg = pM;
					while (Math.Abs(Vector.SubtendedAngle(p0, ptPos, p1)) > 90)
					{
						listMajorTicks.Add(base.SpatialTransform.ConvertToDestination(ptPos));
						map.Add(string.Format("{0:f2},{1:f2}", ptPos.X, ptPos.Y), null);

						if (!ptPos.Equals(ptNeg))
						{
							listMajorTicks.Insert(0, base.SpatialTransform.ConvertToDestination(ptNeg));
							map.Add(string.Format("{0:f2},{1:f2}", ptNeg.X, ptNeg.Y), null);
						}

						ptPos = ptPos + majorOffset;
						ptNeg = ptNeg - majorOffset;
					}
				}

				// compute minor tick positions, checking the index for existence of a major tick at the same position
				TickOffset minorOffset = TickOffset.CreateTickOffset(this.MinorTick*pxS, pxR, xyW);
				ptPos = ptNeg = pM;
				while (Math.Abs(Vector.SubtendedAngle(p0, ptPos, p1)) > 90)
				{
					if (allowOverlap || !map.ContainsKey(string.Format("{0:f2},{1:f2}", ptPos.X, ptPos.Y)))
					{
						listMinorTicks.Add(base.SpatialTransform.ConvertToDestination(ptPos));
					}

					if (!ptPos.Equals(ptNeg) && (allowOverlap || !map.ContainsKey(string.Format("{0:f2},{1:f2}", ptNeg.X, ptNeg.Y))))
					{
						listMinorTicks.Insert(0, base.SpatialTransform.ConvertToDestination(ptNeg));
					}

					ptPos = ptPos + minorOffset;
					ptNeg = ptNeg - minorOffset;
				}

				// if we don't have room for 2 ticks, try recomputing from one end instead of the middle
				if (listMajorTicks.Count + listMinorTicks.Count < 2)
				{
					listMajorTicks.Clear();
					listMinorTicks.Clear();

					ptPos = p0 + minorOffset;
					ptNeg = p0 - minorOffset;

					if (Math.Abs(Vector.SubtendedAngle(p0, ptPos, p1)) > 90)
					{
						listMinorTicks.Add(linePoint1);
						listMinorTicks.Add(base.SpatialTransform.ConvertToDestination(ptPos));
					}
					else if (Math.Abs(Vector.SubtendedAngle(p0, ptNeg, p1)) > 90)
					{
						listMinorTicks.Add(linePoint1);
						listMinorTicks.Add(base.SpatialTransform.ConvertToDestination(ptNeg));
					}
				}
			}

			majorTicks = listMajorTicks.AsReadOnly();
			minorTicks = listMinorTicks.AsReadOnly();
		}

		/// <summary>
		/// Gets the source image's pixel dimensions in millimetres.
		/// </summary>
		/// <param name="width">Output variable to receive the pixel millimetre width.</param>
		/// <param name="height">Output variable to receive the pixel millimetre height.</param>
		/// <exception cref="UncalibratedImageException">Thrown if the parent image does not provide pixel spacing information.</exception>
		private void GetPixelDimensions(out double width, out double height)
		{
			if (base.ParentPresentationImage is IImageSopProvider)
			{
				NormalizedPixelSpacing spacing = ((IImageSopProvider) base.ParentPresentationImage).Frame.NormalizedPixelSpacing;
				if (!spacing.IsNull)
				{
					width = spacing.Column;
					height = spacing.Row;
					return;
				}
			}
			throw new UncalibratedImageException();
		}

		/// <summary>
		/// Exception indicating that the parent image does not provide pixel spacing information.
		/// </summary>
		public class UncalibratedImageException : Exception
		{
			internal UncalibratedImageException() : base("The image does not provide pixel spacing information, nor has it been calibrated.") {}
		}

		private class TickOffset
		{
			public readonly float X;
			public readonly float Y;

			public TickOffset(double x, double y)
			{
				this.X = (float) x;
				this.Y = (float) y;
			}

			public TickOffset GetUnitOffset()
			{
				double magnitude = Math.Sqrt(X*X + Y*Y);
				return new TickOffset(Math.Abs(this.X)/magnitude, Math.Abs(this.Y)/magnitude);
			}

			public static TickOffset CreateTickOffset(double x, double yxRatio, bool xySwapped)
			{
				if (xySwapped)
					return new TickOffset(x*yxRatio, x);
				return new TickOffset(x, x*yxRatio);
			}

			public static TickOffset operator *(int multiplier, TickOffset offset)
			{
				return new TickOffset(multiplier*offset.X, multiplier*offset.Y);
			}

			public static implicit operator SizeF(TickOffset offset)
			{
				return new SizeF(offset.X, offset.Y);
			}
		}
	}
}