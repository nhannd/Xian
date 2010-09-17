#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// An implementation of <see cref="IAnnotationCalloutLocationStrategy"/>
	/// suitable for linear measurements.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This implementation uses the ROI graphic's end points to
	/// compute a callout position that tries to minimize callout obstruction of the
	/// underlying anatomy while keeping the callout within the image tile.
	/// </para>
	/// <para>
	/// The auto computation is disabled if the user manually positions the callout.
	/// </para>
	/// </remarks>
	public class RulerCalloutLocationStrategy : AnnotationCalloutLocationStrategy
	{
		private bool _manuallyPositionedCallout = false;

		/// <summary>
		/// Gets the <see cref="AnnotationGraphic"/>'s Subject.
		/// </summary>
		protected new IPointsGraphic Roi
		{
			get { return ((IPointsGraphic) base.Roi); }
		}

		public override void OnCalloutLocationChangedExternally()
		{
			base.OnCalloutLocationChangedExternally();

			_manuallyPositionedCallout = true;
		}

		public override bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			// if the user has manually positioned the callout, we won't override it
			if (_manuallyPositionedCallout || Roi.Points.Count == 0)
			{
				location = PointF.Empty;
				coordinateSystem = CoordinateSystem.Destination;
				return false;
			}

			Callout.CoordinateSystem = CoordinateSystem.Destination;
			Roi.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				var roiPoint1 = Roi.Points[0];
				var roiPoint2 = Roi.Points[Roi.Points.Count - 1];
				var clientRectangle = Roi.ParentPresentationImage.ClientRectangle;

				var textSize = Callout.TextBoundingBox.Size;
				if (textSize.IsEmpty)
					textSize = new SizeF(100, 50);

				var calloutLocation = new PointF();
				calloutLocation.Y = ComputeCalloutLocationY(textSize, clientRectangle, roiPoint1, roiPoint2);
				calloutLocation.X = ComputeCalloutLocationX(textSize, clientRectangle, roiPoint1, roiPoint2, calloutLocation.Y);

				coordinateSystem = CoordinateSystem.Destination;
				location = calloutLocation;
			}
			finally
			{
				Callout.ResetCoordinateSystem();
				Roi.ResetCoordinateSystem();
			}
			return true;
		}

		private static float ComputeCalloutLocationY(SizeF textSize, RectangleF clientRectangle, PointF roiPoint1, PointF roiPoint2)
		{
			const float roiVOffset = 15;

			var roiY = (roiPoint1.Y + roiPoint2.Y)/2;
			var roiHalfHeight = Math.Abs(roiPoint1.Y - roiPoint2.Y)/2;
			var textHalfHeight = textSize.Height/2;

			//TODO (CR Sept 2010): can this be written more descriptively?
			// e.g. if (IsBeyondTopEdge(roiY)) MoveInsideTopEdge(RoiY);
			if (roiY < textSize.Height + roiHalfHeight + roiVOffset)
				return roiY + textHalfHeight + roiHalfHeight + roiVOffset;
			else if (roiY < clientRectangle.Height/2)
				return roiY - textHalfHeight - roiHalfHeight - roiVOffset;
			else if (roiY < clientRectangle.Height - textSize.Height - roiHalfHeight - roiVOffset)
				return roiY + textHalfHeight + roiHalfHeight + roiVOffset;
			else
				return roiY - textHalfHeight - roiHalfHeight - roiVOffset;
		}

		private static float ComputeCalloutLocationX(SizeF textSize, RectangleF clientRectangle, PointF roiPoint1, PointF roiPoint2, float calloutY)
		{
			var roiX = Math.Abs(calloutY - roiPoint1.Y) < Math.Abs(calloutY - roiPoint2.Y) ? roiPoint1.X : roiPoint2.X;
			var roiHalfWidth = Math.Abs(roiPoint1.X - roiPoint2.X)/2;
			var textHalfWidth = textSize.Width/2;

			//TODO (CR Sept 2010): can this be written more descriptively?
			// e.g. if (IsBeyondLeftEdge(roiX)) MoveInsideLeftEdge(RoiX);
			if (roiX < -roiHalfWidth)
				return roiX + roiHalfWidth + textHalfWidth;
			else if (roiX < textHalfWidth)
				return textHalfWidth;
			else if (roiX < clientRectangle.Width - textHalfWidth)
				return roiX;
			else if (roiX < clientRectangle.Width + roiHalfWidth)
				return clientRectangle.Width - textHalfWidth;
			else
				return roiX - roiHalfWidth - textHalfWidth;
		}
	}
}