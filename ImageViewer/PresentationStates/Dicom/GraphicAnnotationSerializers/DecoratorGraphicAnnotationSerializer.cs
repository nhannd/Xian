#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.GraphicAnnotationSerializers
{
	internal class DecoratorGraphicAnnotationSerializer : GraphicAnnotationSerializer<IDecoratorGraphic>
	{
		protected override void Serialize(IDecoratorGraphic controlGraphic, GraphicAnnotationSequenceItem serializationState)
		{
			SerializeGraphic(controlGraphic.DecoratedGraphic, serializationState);
		}
	}
}