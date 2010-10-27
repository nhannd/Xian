#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class StandardAnnotationGraphicSerializer : GraphicAnnotationSerializer<AnnotationGraphic>
	{
		protected override void Serialize(AnnotationGraphic annotationGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			Platform.CheckForNullReference(annotationGraphic, "annotationGraphic");
			Platform.CheckForNullReference(serializationState, "serializationState");

			SerializeGraphic(annotationGraphic.Callout, serializationState);
			SerializeGraphic(annotationGraphic.Subject, serializationState);
		}
	}
}