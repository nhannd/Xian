using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class StandardAnnotationGraphicSerializer : GraphicAnnotationSerializer<StandardAnnotationGraphic>
	{
		protected override void Serialize(StandardAnnotationGraphic annotationGraphic, GraphicAnnotationSequenceItem serializationState) {
			Platform.CheckForNullReference(annotationGraphic, "annotationGraphic");
			Platform.CheckForNullReference(serializationState, "serializationState");

			SerializeGraphic(annotationGraphic.Callout, serializationState);
			SerializeGraphic(annotationGraphic.Subject, serializationState);
		}
	}
}