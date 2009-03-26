using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.GraphicAnnotationSerializers
{
	internal class DecoratorGraphicAnnotationSerializer : GraphicAnnotationSerializer<IDecoratorGraphic>
	{
		protected override void Serialize(IDecoratorGraphic controlGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			SerializeGraphic(controlGraphic.DecoratedGraphic, serializationState);
		}
	}
}