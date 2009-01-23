using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.ImageViewer.PresentationStates {
	public interface IPresentationStateSerializableGraphic {
		void SerializeRoiGraphic(GraphicAnnotationSequenceItem serializationState);
	}
}
