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
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A primitive curve graphic.
	/// </summary>
	/// <remarks>
	/// <para>Unlike the <see cref="ArcPrimitive"/>, this graphic is defined as a series of points with
	/// an cubic-spline-interpolated curve between the data points. The resulting curve will pass through
	/// all the specified points.</para>
	/// </remarks>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof(CurveGraphicAnnotationSerializer))]
	public class CurvePrimitive : VectorGraphic, IPointsGraphic
	{
		[CloneIgnore]
		private readonly PointsList _points;

		/// <summary>
		/// Constructs a curve graphic.
		/// </summary>
		public CurvePrimitive()
		{
			_points = new PointsList(this);
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected CurvePrimitive(CurvePrimitive source, ICloningContext context)
		{
			context.CloneFields(source, this);
			_points = new PointsList(source._points, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		public IPointsList Points
		{
			get { return _points; }
		}

		public override RectangleF BoundingBox
		{
			get { return RectangleUtilities.ComputeBoundingRectangle(_points); }
		}

		private void Initialize()
		{
			_points.PointAdded += OnPointsChanged;
			_points.PointChanged += OnPointsChanged;
			_points.PointRemoved += OnPointsChanged;
			_points.PointsCleared += OnPointsChanged;
		}

		private void OnPointsChanged(object sender, EventArgs e)
		{
			base.NotifyPropertyChanged("Points");
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
				PointF[] pathPoints = GetCurvePoints(_points);
				GraphicsPath gp = new GraphicsPath();
				if (_points.IsClosed)
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
			foreach (PointF pt in _points)
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
			for (int n = 0; n < _points.Count; n++)
			{
				_points[n] = _points[n] + delta;
			}
		}

		private static PointF[] GetCurvePoints(IPointsList points)
		{
			PointF[] result = new PointF[points.Count - (points.IsClosed ? 1 : 0)];
			for (int n = 0; n < result.Length; n++)
				result[n] = points[n];
			return result;
		}
	}
}