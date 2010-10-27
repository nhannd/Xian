#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class CalloutGraphicAnnotationSerializer : GraphicAnnotationSerializer<ICalloutGraphic>
	{
		protected override void Serialize(ICalloutGraphic calloutGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			// if the callout is not visible, don't serialize it!
			if (!calloutGraphic.Visible)
				return;

			GraphicAnnotationSequenceItem.TextObjectSequenceItem text = new GraphicAnnotationSequenceItem.TextObjectSequenceItem();

			calloutGraphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF boundingBox = RectangleUtilities.ConvertToPositiveRectangle(calloutGraphic.TextBoundingBox);
				text.BoundingBoxAnnotationUnits = GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Pixel;
				text.BoundingBoxTextHorizontalJustification = GraphicAnnotationSequenceItem.BoundingBoxTextHorizontalJustification.Left;
				text.BoundingBoxTopLeftHandCorner = boundingBox.Location;
				text.BoundingBoxBottomRightHandCorner = boundingBox.Location + boundingBox.Size;

				text.AnchorPoint = calloutGraphic.AnchorPoint;
				text.AnchorPointAnnotationUnits = GraphicAnnotationSequenceItem.AnchorPointAnnotationUnits.Pixel;
				text.AnchorPointVisibility = GraphicAnnotationSequenceItem.AnchorPointVisibility.Y;

				if (string.IsNullOrEmpty(calloutGraphic.Text))
					text.UnformattedTextValue = " ";
				else
					text.UnformattedTextValue = calloutGraphic.Text;
			}
			finally
			{
				calloutGraphic.ResetCoordinateSystem();
			}

			serializationState.AppendTextObjectSequence(text);
		}
	}
}