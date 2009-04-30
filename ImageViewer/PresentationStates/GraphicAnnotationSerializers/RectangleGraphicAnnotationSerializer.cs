#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Drawing;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.PresentationStates.GraphicAnnotationSerializers
{
	public class RectangleGraphicAnnotationSerializer : GraphicAnnotationSerializer<IBoundableGraphic>
	{
		protected override void Serialize(IBoundableGraphic graphic, GraphicAnnotationSequenceItem serializationState)
		{
			if (!graphic.Visible)
				return; // if the graphic is not visible, don't serialize it!

			GraphicAnnotationSequenceItem.GraphicObjectSequenceItem annotationElement = new GraphicAnnotationSequenceItem.GraphicObjectSequenceItem();

			graphic.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				annotationElement.GraphicAnnotationUnits = GraphicAnnotationSequenceItem.GraphicAnnotationUnits.Pixel;
				annotationElement.GraphicDimensions = 2;
				annotationElement.GraphicType = GraphicAnnotationSequenceItem.GraphicType.Polyline;
				annotationElement.NumberOfGraphicPoints = 5;

				RectangleF bounds = graphic.BoundingBox;

				// add shape vertices
				PointF[] list = new PointF[5];
				list[0] = bounds.Location;
				list[1] = bounds.Location + new SizeF(bounds.Width, 0);
				list[2] = bounds.Location + bounds.Size;
				list[3] = bounds.Location + new SizeF(0, bounds.Height);
				list[4] = bounds.Location;
				annotationElement.GraphicData = list;

				annotationElement.GraphicFilled = GraphicAnnotationSequenceItem.GraphicFilled.N;
			}
			finally
			{
				graphic.ResetCoordinateSystem();
			}

			serializationState.AppendGraphicObjectSequence(annotationElement);
		}
	}
}