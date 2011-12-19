#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// A default implementation of <see cref="IAnnotationCalloutLocationStrategy"/>
	/// suitable for ROI measurements with a tight bounding box (i.e. not a line).
	/// </summary>
	/// <remarks>
	/// <para>
	/// This implementation uses the ROI graphic's <see cref="IGraphic.BoundingBox"/>
	/// compute a callout position that tries to minimize callout obstruction of the
	/// underlying anatomy while keeping the callout within the image tile.
	/// </para>
	/// <para>
	/// The auto computation is disabled if the user manually positions the callout.
	/// </para>
	/// </remarks>
	public class DefaultRoiCalloutLocationStrategy : AnnotationCalloutLocationStrategy
	{
		private bool _manuallyPositionedCallout = false;

		public override void OnCalloutLocationChangedExternally()
		{
			base.OnCalloutLocationChangedExternally();

			_manuallyPositionedCallout = true;
		}

		public override bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			// if the user has manually positioned the callout, we won't override it
			if (_manuallyPositionedCallout)
			{
				location = PointF.Empty;
				coordinateSystem = CoordinateSystem.Destination;
				return false;
			}

			Callout.CoordinateSystem = CoordinateSystem.Destination;
            AnnotationSubject.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
                var roiBoundingBox = AnnotationSubject.BoundingBox;
                var clientRectangle = AnnotationSubject.ParentPresentationImage.ClientRectangle;

				var textSize = Callout.TextBoundingBox.Size;
				if (textSize.IsEmpty)
					textSize = new SizeF(100, 50);

				coordinateSystem = CoordinateSystem.Destination;
				location = new PointF(ComputeCalloutLocationX(textSize, clientRectangle, roiBoundingBox), ComputeCalloutLocationY(textSize, clientRectangle, roiBoundingBox));
			}
			finally
			{
				Callout.ResetCoordinateSystem();
                AnnotationSubject.ResetCoordinateSystem();
			}
			return true;
		}

		private static float ComputeCalloutLocationY(SizeF textSize, RectangleF clientRectangle, RectangleF roiBoundingBox)
		{
			const float roiVOffset = 15;

			var roiY = roiBoundingBox.Top + roiBoundingBox.Height/2;
			var roiHalfHeight = roiBoundingBox.Height/2;
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

		private static float ComputeCalloutLocationX(SizeF textSize, RectangleF clientRectangle, RectangleF roiBoundingBox)
		{
			var roiX = roiBoundingBox.Left + roiBoundingBox.Width/2;
			var roiHalfWidth = roiBoundingBox.Width/2;
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