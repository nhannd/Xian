#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive line graphic.
	/// </summary>
	[Cloneable(true)]
	public class LinePrimitive : VectorGraphic, ILineSegmentGraphic
	{
		#region Private fields
		
		private PointF _point1;
		private PointF _point2;
		private event EventHandler<PointChangedEventArgs> _point1ChangedEvent;
		private event EventHandler<PointChangedEventArgs> _point2ChangedEvent;
		
		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="LinePrimitive"/>.
		/// </summary>
		public LinePrimitive()
		{
		}

		/// <summary>
		/// The start point of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF Pt1
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point1;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_point1);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Pt1, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point1 = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_point1 = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_point1ChangedEvent, this, new PointChangedEventArgs(this.Pt1));
				base.NotifyVisualStateChanged("Pt1");
			}
		}

		/// <summary>
		/// The end point of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public PointF Pt2
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point2;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_point2);
				}
			}
			set
			{
				if (FloatComparer.AreEqual(this.Pt2, value))
					return;

				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point2 = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_point2 = base.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_point2ChangedEvent, this, new PointChangedEventArgs(this.Pt2));
				base.NotifyVisualStateChanged("Pt2");
			}
		}

		/// <summary>
		/// Gets the tightest bounding box that encloses the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		public override RectangleF BoundingBox
		{
			get { return RectangleUtilities.ComputeBoundingRectangle(this.Pt1, this.Pt2); }
		}

		/// <summary>
		/// Occurs when <see cref="Pt1"/> has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> Pt1Changed
		{
			add { _point1ChangedEvent += value; }
			remove { _point1ChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when <see cref="Pt2"/> has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> Pt2Changed
		{
			add { _point2ChangedEvent += value; }
			remove { _point2ChangedEvent -= value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="LinePrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="LinePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the line.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			double distance;
			PointF ptNearest = new PointF(0, 0);

			// Always do the hit test in destination coordinates since we want the
			// "activation distance" to be the same irrespective of the zoom
			this.CoordinateSystem = CoordinateSystem.Destination;
			distance = Vector.DistanceFromPointToLine(point, this.Pt1, this.Pt2, ref ptNearest);
			this.ResetCoordinateSystem();

			if (distance < HitTestDistance)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the point on the <see cref="Graphic"/> closest to the specified point.
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
			PointF result = PointF.Empty;
			Vector.DistanceFromPointToLine(point, this.Pt1, this.Pt2, ref result);
			return result;
		}

		/// <summary>
		/// Moves the <see cref="LinePrimitive"/> by a specified delta.
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
			this.Pt1 += del;
			this.Pt2 += del;
#else
			this.Pt1 += delta;
			this.Pt2 += delta;
#endif
		}

		#region ILineSegmentGraphic Members

		PointF ILineSegmentGraphic.Point1
		{
			get { return this.Pt1; }
			set { this.Pt1 = value; }
		}

		PointF ILineSegmentGraphic.Point2
		{
			get { return this.Pt2; }
			set { this.Pt2 = value; }
		}

		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Point1Changed
		{
			add { _point1ChangedEvent += value; }
			remove { _point1ChangedEvent -= value; }
		}

		event EventHandler<PointChangedEventArgs> ILineSegmentGraphic.Point2Changed
		{
			add { _point2ChangedEvent += value; }
			remove { _point2ChangedEvent -= value; }
		}

		#endregion
	}
}
