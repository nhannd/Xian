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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	//TODO: change this to expose a collection of points.

	/// <summary>
	/// A primitive curve graphic.
	/// </summary>
	/// <remarks>
	/// <para>Unlike the <see cref="ArcPrimitive"/>, this graphic is defined as a series of points with
	/// an cubic-spline-interpolated curve between the data points. The resulting curve will pass through
	/// all the specified points.</para>
	/// </remarks>
	[Cloneable]
	public class CurvePrimitive : VectorGraphic
	{
		[CloneIgnore]
		private readonly List<PointF> _srcPts;

		private event EventHandler<ListEventArgs<PointF>> _pointChanged;

		/// <summary>
		/// Constructs a curve graphic.
		/// </summary>
		public CurvePrimitive()
		{
			_srcPts = new List<PointF>();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected CurvePrimitive(CurvePrimitive source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_srcPts = new List<PointF>();
			foreach (PointF point in source._srcPts)
			{
				_srcPts.Add(point);
			}
		}

		/// <summary>
		/// Gets or sets the data points of the curve.
		/// </summary>
		/// <remarks>
		/// <para>The resulting interpolated curve will pass through these points.</para>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.</para>
		/// </remarks>
		/// <param name="index">The index of the data point.</param>
		public PointF this[int index]
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Destination)
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_srcPts[index]);
				}
				return _srcPts[index];
			}
			set
			{
				if (base.CoordinateSystem == CoordinateSystem.Destination)
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					value = base.SpatialTransform.ConvertToSource(value);
				}

				if (!FloatComparer.AreEqual(_srcPts[index], value))
				{
					_srcPts[index] = value;
					EventsHelper.Fire(_pointChanged, this, new ListEventArgs<PointF>(value, index));
					base.NotifyPropertyChanged("Points");
				}
			}
		}

		/// <summary>
		/// Gets the number of data points in the curve.
		/// </summary>
		public int CountPoints
		{
			get { return _srcPts.Count; }
		}

		public override RectangleF BoundingBox
		{
			get { return RectangleUtilities.ComputeBoundingRectangle(this.AsArray()); }
		}

		/// <summary>
		/// Adds a new data point to the end of the curve.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether the
		/// point is interpreted as source or destination coordinates.
		/// </remarks>
		/// <param name="point">The data point to add.</param>
		public void Add(PointF point)
		{
			if (base.CoordinateSystem == CoordinateSystem.Destination)
			{
				Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
				point = base.SpatialTransform.ConvertToSource(point);
			}

			_srcPts.Add(point);
			base.NotifyPropertyChanged("Points");
		}

		/// <summary>
		/// Inserts a new data point at the specified index.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether the
		/// point is interpreted as source or destination coordinates.
		/// </remarks>
		/// <param name="index">The index of the data point.</param>
		/// <param name="point">The data point to add.</param>
		public void Insert(int index, PointF point)
		{
			if (base.CoordinateSystem == CoordinateSystem.Destination)
			{
				Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
				point = base.SpatialTransform.ConvertToSource(point);
			}

			_srcPts.Insert(index, point);
			base.NotifyPropertyChanged("Points");
		}

		/// <summary>
		/// Removes the data point at the specified index.
		/// </summary>
		/// <param name="index">The index of the data point.</param>
		public void RemoveAt(int index)
		{
			_srcPts.RemoveAt(index);
			base.NotifyPropertyChanged("Points");
		}

		/// <summary>
		/// Gets a value indicating if the curve is closed - that is, its last data point is equal to the first data point.
		/// </summary>
		public bool IsClosed
		{
			get { return (_srcPts.Count > 0) && FloatComparer.AreEqual(_srcPts[0], _srcPts[_srcPts.Count - 1]); }
		}

		/// <summary>
		/// Event fired when the coordinates of a data point have changed.
		/// </summary>
		public event EventHandler<ListEventArgs<PointF>> PointChanged
		{
			add { _pointChanged += value; }
			remove { _pointChanged -= value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="Graphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="Graphic"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		public override bool HitTest(Point point)
		{
			base.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				PointF[] pathPoints = this.AsArray(true);
				GraphicsPath gp = new GraphicsPath();
				if (this.IsClosed)
					gp.AddClosedCurve(pathPoints);
				else
					gp.AddCurve(pathPoints);
				return gp.IsVisible(point);
			}
			finally
			{
				base.ResetCoordinateSystem();
			}
		}

		/// <summary>
		/// Gets the point on the <see cref="CurvePrimitive"/> closest to the specified point.
		/// </summary>
		/// <param name="point">A point in either source or destination coordinates.</param>
		/// <returns>The point on the graphic closest to the given <paramref name="point"/>.</returns>
		/// <remarks>
		/// <para>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// the computation will be carried out in either source
		/// or destination coordinates.</para>
		/// <para>Since the interpolation between nodes of the curve is not explicitly
		/// defined, this method returns the closest node to the specified point, and
		/// ignores the individual curve segments for the purposes of this calculation.</para>
		/// </remarks>
		public override PointF GetClosestPoint(PointF point)
		{
			PointF result = PointF.Empty;
			double min = double.MaxValue;
			foreach (PointF pt in this.AsArray())
			{
				double d = Vector.Distance(point, pt);
				if (min > d)
				{
					min = d;
					result = pt;
				}
			}
			return result;
		}

		/// <summary>
		/// Moves the <see cref="Graphic"/> by a specified delta.
		/// </summary>
		/// <param name="delta">The distance to move.</param>
		/// <remarks>
		/// Depending on the value of <see cref="Graphic.CoordinateSystem"/>,
		/// <paramref name="delta"/> will be interpreted in either source
		/// or destination coordinates.
		/// </remarks>
		public override void Move(SizeF delta)
		{
			for (int n = 0; n < _srcPts.Count; n++)
			{
				_srcPts[n] = _srcPts[n] + delta;
			}
			base.NotifyPropertyChanged("Points");
		}

		/// <summary>
		/// Gets the data points of the curve as an array.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether the
		/// points are given as source or destination coordinates.
		/// </remarks>
		/// <returns>An array of points.</returns>
		public PointF[] AsArray()
		{
			// this implementation is much more efficient than calling the general overload
			PointF[] array;
			if (base.CoordinateSystem == CoordinateSystem.Destination)
			{
				Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
				array = new PointF[_srcPts.Count];
				for (int n = 0; n < _srcPts.Count; n++)
					array[n] = base.SpatialTransform.ConvertToDestination(_srcPts[n]);
			}
			else
			{
				array = _srcPts.ToArray();
			}
			return array;
		}

		/// <summary>
		/// Gets the data points of the curve as an array.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether the
		/// points are given as source or destination coordinates.
		/// </remarks>
		/// <param name="excludeClosedPathPoint">True if a closed path should have its last point excluded from the results; False otherwise. No effect if the curve is not closed.</param>
		/// <returns>An array of points.</returns>
		public PointF[] AsArray(bool excludeClosedPathPoint)
		{
			return AsArray(SizeF.Empty, excludeClosedPathPoint);
		}

		/// <summary>
		/// Gets the data points of the curve as an array.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether the
		/// points are given as source or destination coordinates.
		/// </remarks>
		/// <param name="offset">An offset to apply to each data point.</param>
		/// <returns>An array of points.</returns>
		public PointF[] AsArray(SizeF offset)
		{
			return AsArray(offset, false);
		}

		/// <summary>
		/// Gets the data points of the curve as an array.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether the
		/// points are given as source or destination coordinates.
		/// </remarks>
		/// <param name="offset">An offset to apply to each data point.</param>
		/// <param name="excludeClosedPathPoint">True if a closed path should have its last point excluded from the results; False otherwise. No effect if the curve is not closed.</param>
		/// <returns>An array of points.</returns>
		public PointF[] AsArray(SizeF offset, bool excludeClosedPathPoint)
		{
			PointF[] array;
			if (this.IsClosed && excludeClosedPathPoint)
				array = new PointF[_srcPts.Count - 1];
			else
				array = new PointF[_srcPts.Count];

			if (base.CoordinateSystem == CoordinateSystem.Destination)
			{
				Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
				for (int n = 0; n < array.Length; n++)
					array[n] = base.SpatialTransform.ConvertToDestination(_srcPts[n]) + offset;
			}
			else
			{
				for (int n = 0; n < array.Length; n++)
					array[n] = _srcPts[n] + offset;
			}
			return array;
		}
	}
}