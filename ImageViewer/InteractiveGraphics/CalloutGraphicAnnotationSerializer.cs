using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class CalloutGraphicAnnotationSerializer : GraphicAnnotationSerializer<CalloutGraphic>
	{
		protected override void Serialize(CalloutGraphic calloutGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			if (!calloutGraphic.Visible)
				return; // if the callout is not visible, don't serialize it!

			GraphicAnnotationSequenceItem.TextObjectSequenceItem text = new GraphicAnnotationSequenceItem.TextObjectSequenceItem();

			calloutGraphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				//InvariantTextPrimitive textGraphic = (InvariantTextPrimitive)CollectionUtils.SelectFirst(calloutGraphic.Graphics, delegate(IGraphic graphic) { return graphic is InvariantTextPrimitive; });
				//RectangleF boundingBox = RectangleUtilities.ConvertToPositiveRectangle(textGraphic.BoundingBox);
				//text.BoundingBoxAnnotationUnits = GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Pixel;
				//text.BoundingBoxTextHorizontalJustification = GraphicAnnotationSequenceItem.BoundingBoxTextHorizontalJustification.Left;
				//text.BoundingBoxTopLeftHandCorner = boundingBox.Location;
				//text.BoundingBoxBottomRightHandCorner = boundingBox.Location + boundingBox.Size;

				text.AnchorPoint = calloutGraphic.EndPoint;
				text.AnchorPointAnnotationUnits = GraphicAnnotationSequenceItem.AnchorPointAnnotationUnits.Pixel;
				text.AnchorPointVisibility = GraphicAnnotationSequenceItem.AnchorPointVisibility.Y;
				text.UnformattedTextValue = calloutGraphic.Text.Trim();
			}
			finally
			{
				calloutGraphic.ResetCoordinateSystem();
			}

			serializationState.AppendTextObjectSequence(text);
		}
	}
}