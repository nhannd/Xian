#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive arc graphic.
	/// </summary>
	/// <remarks>
	/// An arc is defined by a portion of the perimeter of an ellipse.
	/// The ellipse is defined by a bounding rectangle defined by the
	/// base class, <see cref="BoundableGraphic"/>.  The portion of the
	/// ellipse is defined by the <see cref="ArcPrimitive.StartAngle"/>
	/// and <see cref="ArcPrimitive.SweepAngle"/>.
	/// </remarks>
	[Cloneable(true)]
	public class ArcPrimitive : BoundableGraphic, IArcGraphic
	{
		private float _startAngle;
		private float _sweepAngle;

		/// <summary>
		/// Initializes a new instance of <see cref="ArcPrimitive"/>.
		/// </summary>
		public ArcPrimitive()
		{
			
		}

		/// <summary>
		/// Gets or sets the angle in degrees at which the arc begins.
		/// </summary>
		/// <remarks>
		/// It is good practice to set the <see cref="IArcGraphic.StartAngle"/> before the <see cref="IArcGraphic.SweepAngle"/>
		/// because in the case where a graphic is scaled differently in x than in y, the conversion
		/// of the <see cref="IArcGraphic.SweepAngle"/> from <see cref="CoordinateSystem.Source"/> to
		/// <see cref="CoordinateSystem.Destination"/> coordinates is dependent upon the <see cref="IArcGraphic.StartAngle"/>.
		/// However, under normal circumstances, where the scale in x and y are the same, the <see cref="IArcGraphic.StartAngle"/>
		/// and <see cref="IArcGraphic.SweepAngle"/> can be set independently.
		/// </remarks>
		public float StartAngle
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					return _startAngle;
				}
				else
				{
					return ArcPrimitive.ConvertStartAngle(_startAngle, this.SpatialTransform, CoordinateSystem.Destination);
				}
			}
			set
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					_startAngle = value;
				}
				else
				{
					_startAngle = ArcPrimitive.ConvertStartAngle(value, this.SpatialTransform, CoordinateSystem.Source);
				}
			}
		}

		/// <summary>
		/// Gets or sets the angle in degrees that the arc sweeps out.
		/// </summary>
		/// <remarks>
		/// See <see cref="IArcGraphic.StartAngle"/> for information on setting the <see cref="IArcGraphic.SweepAngle"/>.
		/// </remarks>
		public float SweepAngle
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					return _sweepAngle;
				}
				else
				{
					return ArcPrimitive.ConvertSweepAngle(_sweepAngle, _startAngle, this.SpatialTransform, CoordinateSystem.Destination);
				}
			}
			set
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					_sweepAngle = value;
				}
				else
				{
					this.CoordinateSystem = CoordinateSystem.Destination;
					_sweepAngle = ArcPrimitive.ConvertSweepAngle(value, this.StartAngle, this.SpatialTransform, CoordinateSystem.Source);
					this.ResetCoordinateSystem();
				}
			}
		}

		/// <summary>
		/// Performs a hit test on the <see cref="ArcPrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="ArcPrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the arc.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;

			bool result = ArcPrimitive.HitTest(
				SpatialTransform.ConvertToSource(point), this.Rectangle,
				this.StartAngle, this.SweepAngle,
				this.SpatialTransform);
			
			this.ResetCoordinateSystem();

			return result;
		}

		/// <summary>
		/// Gets the point on the <see cref="ArcPrimitive"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			return GetClosestPoint(point, this.Rectangle, this.StartAngle, this.SweepAngle);
		}

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns>Always returns <b>false</b>, since an arc cannot
		/// contain a point.</returns>
		public override bool Contains(PointF point)
		{
			return false;
		}

		internal static PointF GetClosestPoint(
			PointF point,
			RectangleF boundingBox,
			float startAngle,
			float sweepAngle)
		{
			// Semi major/minor axes
			float a = boundingBox.Width/2;
			float b = boundingBox.Height/2;

			// Center of ellipse
			float x1 = boundingBox.Left + a;
			float y1 = boundingBox.Top + b;

			// normalize the angles
			float normSweepAngle = Math.Sign(sweepAngle)*Math.Min(360, Math.Abs(sweepAngle));
			float normStartAngle = startAngle % 360;
			float normEndAngle = (normStartAngle + normSweepAngle);
			if(sweepAngle < 0)
			{
				float t = normStartAngle;
				normStartAngle = normEndAngle;
				normEndAngle = t;
			}

			PointF center = new PointF(x1, y1);
			PointF result = EllipsePrimitive.IntersectEllipseAndLine(a, b, center, point);
			double angle = Vector.SubtendedAngle(center + new SizeF(1, 0), center, result);
			if(angle > normStartAngle && angle < normEndAngle)
				return result;
			angle = (angle + 180)%360;
			if (angle > normStartAngle && angle < normEndAngle)
				return result;

			//TODO check end points
			return center;
		}

		internal static bool HitTest(
			PointF point, 
			RectangleF boundingBox, 
			float startAngle,
			float sweepAngle,
			SpatialTransform transform)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddArc(RectangleUtilities.ConvertToPositiveRectangle(boundingBox), startAngle, sweepAngle);

			Pen pen = new Pen(Brushes.White, HitTestDistance / transform.CumulativeScale);
			bool result = path.IsOutlineVisible(point, pen);

			path.Dispose();
			pen.Dispose();

			return result;
		}

		internal static float ConvertStartAngle(float angle, SpatialTransform transform, CoordinateSystem targetSystem)
		{
			PointF xVector = new PointF(100, 0);

			Matrix rotation = new Matrix();
			PointF[] angleVector = new PointF[] { xVector };
			rotation.Rotate(angle);
			rotation.TransformVectors(angleVector);
			rotation.Dispose();

			SizeF xVectorTransformed, angleVectorTransformed;
			if (targetSystem == Graphics.CoordinateSystem.Destination)
			{
				xVectorTransformed = transform.ConvertToDestination(new SizeF(xVector));
				angleVectorTransformed = transform.ConvertToDestination(new SizeF(angleVector[0]));
			}
			else
			{
				xVectorTransformed = transform.ConvertToSource(new SizeF(xVector));
				angleVectorTransformed = transform.ConvertToSource(new SizeF(angleVector[0]));
			}

			float xRotationOffset =
				(int)Math.Round(Vector.SubtendedAngle(xVectorTransformed.ToPointF(), PointF.Empty, xVector));

			float angleTransformed =
				(int)Math.Round(Vector.SubtendedAngle(angleVectorTransformed.ToPointF(), PointF.Empty, xVectorTransformed.ToPointF()));

			// have to figure out where x-axis moved to and then return the difference between the angle
			// and the x-axis, where both are in 'target' coordinates.
			float returnAngle = angleTransformed + xRotationOffset;
			if (returnAngle < 0)
				returnAngle += 360;

			return returnAngle;
		}

		internal static float ConvertSweepAngle(float sweepAngle, float startAngle, SpatialTransform transform, CoordinateSystem targetSystem)
		{
			PointF x = new PointF(100, 0);

			PointF[] startVector = new PointF[] { x };
			Matrix rotation = new Matrix();
			rotation.Rotate(startAngle);
			rotation.TransformVectors(startVector);

			PointF[] sweepVector = (PointF[])startVector.Clone();
			rotation.Reset();
			rotation.Rotate(sweepAngle);
			rotation.TransformVectors(sweepVector);
			rotation.Dispose();

			SizeF startVectorTransformed, sweepVectorTransformed;
			if (targetSystem == Graphics.CoordinateSystem.Destination)
			{
				startVectorTransformed = transform.ConvertToDestination(new SizeF(startVector[0]));
				sweepVectorTransformed = transform.ConvertToDestination(new SizeF(sweepVector[0]));
			}
			else
			{
				startVectorTransformed = transform.ConvertToSource(new SizeF(startVector[0]));
				sweepVectorTransformed = transform.ConvertToSource(new SizeF(sweepVector[0]));
			}

			// simply return the angle between the start and sweep angle, in the target system.
			return (int)Math.Round(Vector.SubtendedAngle(sweepVectorTransformed.ToPointF(), PointF.Empty, startVectorTransformed.ToPointF()));
		}
	}
}
