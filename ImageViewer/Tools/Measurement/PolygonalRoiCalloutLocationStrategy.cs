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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[Cloneable(true)]
	internal class PolygonalRoiCalloutLocationStrategy : AnnotationCalloutLocationStrategy
	{
		private bool _initialLocationSet;

		public override void SetAnnotationGraphic(AnnotationGraphic annotationGraphic)
		{
			base.SetAnnotationGraphic(annotationGraphic);

			IPointsGraphic pointsGraphic = annotationGraphic.Subject as IPointsGraphic;
			if(pointsGraphic != null)
			{
				pointsGraphic.Points.PointAdded += OnPolygonGraphicClosed;
				pointsGraphic.Points.PointChanged += OnPolygonGraphicClosed;
				pointsGraphic.Points.PointRemoved += OnPolygonGraphicClosed;
				pointsGraphic.Points.PointsCleared += OnPolygonGraphicClosed;
			}
		}

		public override bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			base.Callout.Visible = !string.IsNullOrEmpty(base.Callout.Text);
			location = PointF.Empty;
			coordinateSystem = CoordinateSystem.Destination;

			if (!_initialLocationSet && IsClosed(this.Roi))
			{
				_initialLocationSet = true;

				//TODO: make the offset less hard-coded (use case Roi analyzers with many results).
				SizeF offset = new SizeF(0, 50);

				// Setup the callout
				this.Roi.CoordinateSystem = CoordinateSystem.Destination;
				location = this.Roi.BoundingBox.Location - offset;
				this.Roi.ResetCoordinateSystem();
				return true;
			}

			return false;
		}

		protected new IPointsGraphic Roi
		{
			get { return ((IPointsGraphic)base.Roi); }
		}

		private void OnPolygonGraphicClosed(object sender, EventArgs e)
		{
			// sometimes the coordinate systems are mismatched, so force fix it now
			this.Roi.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				PointF location;
				CoordinateSystem coordinateSystem;

				// compute a nice location for the callout
				if (this.CalculateCalloutLocation(out location, out coordinateSystem))
				{
					this.Callout.CoordinateSystem = coordinateSystem;
					this.Callout.TextLocation = location;
					this.Callout.ResetCoordinateSystem();
				}
			}
			finally
			{
				this.Roi.ResetCoordinateSystem();
			}
		}

		private static bool IsClosed(IPointsGraphic g)
		{
			if (g.Points.Count > 2)
				return FloatComparer.AreEqual(g.Points[0], g.Points[g.Points.Count - 1]);
			return false;
		}
	}
}