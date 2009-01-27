using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class RoiGraphicAnnotationSerializer : GraphicAnnotationSerializer<RoiGraphic>
	{
		protected override void Serialize(RoiGraphic roiGraphic, GraphicAnnotationSequenceItem serializationState) {
			Platform.CheckForNullReference(roiGraphic, "roiGraphic");
			Platform.CheckForNullReference(serializationState, "serializationState");

			SerializeGraphic(roiGraphic.Callout, serializationState);
			SerializeGraphic(roiGraphic.Roi, serializationState);
		}
	}
}