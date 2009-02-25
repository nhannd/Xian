using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class StandardAnnotationGraphicSerializer : GraphicAnnotationSerializer<AnnotationGraphic>
	{
		protected override void Serialize(AnnotationGraphic annotationGraphic, GraphicAnnotationSequenceItem serializationState) {
			Platform.CheckForNullReference(annotationGraphic, "annotationGraphic");
			Platform.CheckForNullReference(serializationState, "serializationState");

			SerializeGraphic(annotationGraphic.Callout, serializationState);
			SerializeGraphic(annotationGraphic.Subject, serializationState);
		}
	}
}