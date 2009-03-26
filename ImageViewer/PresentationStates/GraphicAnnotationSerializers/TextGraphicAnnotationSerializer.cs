using System.Drawing;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.PresentationStates.GraphicAnnotationSerializers
{
	internal class TextGraphicAnnotationSerializer : GraphicAnnotationSerializer<ITextGraphic>
	{
		protected override void Serialize(ITextGraphic textGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			// if the callout is not visible, don't serialize it!
			if (!textGraphic.Visible)
				return;

			if (string.IsNullOrEmpty(textGraphic.Text))
				return;

			GraphicAnnotationSequenceItem.TextObjectSequenceItem text = new GraphicAnnotationSequenceItem.TextObjectSequenceItem();

			textGraphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF boundingBox = RectangleUtilities.ConvertToPositiveRectangle(textGraphic.BoundingBox);
				text.BoundingBoxAnnotationUnits = GraphicAnnotationSequenceItem.BoundingBoxAnnotationUnits.Pixel;
				text.BoundingBoxTextHorizontalJustification = GraphicAnnotationSequenceItem.BoundingBoxTextHorizontalJustification.Left;
				text.BoundingBoxTopLeftHandCorner = boundingBox.Location;
				text.BoundingBoxBottomRightHandCorner = boundingBox.Location + boundingBox.Size;
				text.UnformattedTextValue = textGraphic.Text;
			}
			finally
			{
				textGraphic.ResetCoordinateSystem();
			}

			serializationState.AppendTextObjectSequence(text);
		}
	}
}