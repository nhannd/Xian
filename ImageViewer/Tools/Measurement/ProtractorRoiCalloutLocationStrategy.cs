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

using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using Matrix=System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[Cloneable(true)]
	internal class ProtractorRoiCalloutLocationStrategy : AnnotationCalloutLocationStrategy
	{
		private bool _firstCalculation;
		private PointF _lastVertexLocationSource;
		private bool _userMovedCallout;

		public ProtractorRoiCalloutLocationStrategy()
		{
			_firstCalculation = true;
			_userMovedCallout = false;
		}

		public new IPointsGraphic Roi
		{
			get { return (IPointsGraphic) base.Roi; }
		}

		public override void SetAnnotationGraphic(AnnotationGraphic roiGraphic)
		{
			base.SetAnnotationGraphic(roiGraphic);
			if (_firstCalculation)
				base.Callout.Visible = false;
		}

		public override void OnCalloutLocationChangedExternally()
		{
			_userMovedCallout = true;
		}

		public override void CalculateCalloutEndPoint(out PointF endPoint, out CoordinateSystem coordinateSystem)
		{
			if (_userMovedCallout)
			{
				base.CalculateCalloutEndPoint(out endPoint, out coordinateSystem);
			}
			else
			{
				coordinateSystem = base.Roi.CoordinateSystem;

				if (this.Roi.Points.Count < 3)
					endPoint = this.Roi.Points[0];
				else
					endPoint = this.Roi.Points[1];
			}
		}

		public override bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			if (this.Roi.Points.Count < 3 || string.IsNullOrEmpty(this.Callout.Text))
				base.Callout.Visible = false;
			else
				base.Callout.Visible = true;

			if (!base.Callout.Visible || _userMovedCallout)
				return base.CalculateCalloutLocation(out location, out coordinateSystem);

			SizeF calloutOffsetDestination = GetCalloutOffsetDestination();

			coordinateSystem = CoordinateSystem.Destination;
			base.AnnotationGraphic.CoordinateSystem = coordinateSystem;

			// first, move the callout by the same amount the vertex moved (if it moved at all).
			location = base.Callout.Location + calloutOffsetDestination;

			PointF start = this.Roi.Points[0];
			PointF vertex = this.Roi.Points[1];
			PointF end = this.Roi.Points[2];

			base.AnnotationGraphic.ResetCoordinateSystem();

			double vectorAngle = -Vector.SubtendedAngle(start, vertex, end) / 2 + 180;

			PointF[] points = new PointF[] { start, end };

			using (Matrix rotation = new Matrix())
			{
				rotation.Rotate((float) vectorAngle);
				rotation.Translate(-vertex.X, -vertex.Y);
				rotation.TransformPoints(points);
			}

			float calloutMagnitude = new Vector3D(location.X - vertex.X, location.Y - vertex.Y, 0).Magnitude;

			Vector3D startVector = new Vector3D(points[0].X, points[0].Y, 0);
			if (FloatComparer.AreEqual(startVector.Magnitude, 0F, 0.01F))
				startVector = new Vector3D(-1, 0, 0);

			startVector = startVector / startVector.Magnitude * calloutMagnitude;

			location = new PointF(startVector.X + vertex.X, startVector.Y + vertex.Y);

			return true;
		}

		private SizeF GetCalloutOffsetDestination()
		{
			base.AnnotationGraphic.CoordinateSystem = CoordinateSystem.Source;

			SizeF calloutOffsetDestination = SizeF.Empty;

			if (_firstCalculation)
			{
				//on first calculation, move the callout to the vertex.
				_firstCalculation = false;
				_lastVertexLocationSource = base.Callout.EndPoint;
			}

			PointF currentVertexLocationSource = this.Roi.Points[1];

			if (_lastVertexLocationSource != currentVertexLocationSource)
			{
				PointF currentVertexLocationDestination = Roi.SpatialTransform.ConvertToDestination(currentVertexLocationSource);
				PointF lastVertexLocationDestination = Roi.SpatialTransform.ConvertToDestination(_lastVertexLocationSource);

				calloutOffsetDestination = new SizeF(
					currentVertexLocationDestination.X - lastVertexLocationDestination.X,
					currentVertexLocationDestination.Y - lastVertexLocationDestination.Y);
			}

			_lastVertexLocationSource = currentVertexLocationSource;

			base.AnnotationGraphic.ResetCoordinateSystem();

			return calloutOffsetDestination;
		}
	}
}
