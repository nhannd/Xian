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

using System;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Iod;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal sealed class DFOVAnnotationItem : AnnotationItem
	{
		public DFOVAnnotationItem()
			: base("Presentation.DFOV", new AnnotationResourceResolver(typeof(DFOVAnnotationItem).Assembly))
		{ 
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return String.Empty;
				
			IImageSopProvider imageSopProvider = presentationImage as IImageSopProvider;
			if (imageSopProvider  == null)
				return String.Empty;
				
			ISpatialTransformProvider spatialTransformProvider = presentationImage as ISpatialTransformProvider;
			if (spatialTransformProvider == null)
				return String.Empty;

			ImageSpatialTransform transform = spatialTransformProvider.SpatialTransform as ImageSpatialTransform;
			if (transform == null)
				return String.Empty;

			if (transform.RotationXY % 90 != 0)
				return SR.ValueNotApplicable;

			Frame frame = imageSopProvider.Frame;
			PixelSpacing normalizedPixelSpacing = frame.NormalizedPixelSpacing;
			if (normalizedPixelSpacing.IsNull)
				return String.Empty;

			RectangleF sourceRectangle = new RectangleF(0, 0, frame.Columns, frame.Rows);
			RectangleF destinationRectangle = transform.ConvertToDestination(sourceRectangle);
			destinationRectangle = RectangleUtilities.Intersect(destinationRectangle, presentationImage.ClientRectangle);

			float effectivePixelSizeX = (float)frame.NormalizedPixelSpacing.Column / transform.Scale;
			float effectivePixelSizeY = (float)frame.NormalizedPixelSpacing.Row / transform.Scale;

			double displayedFieldOfViewX;
			double displayedFieldOfViewY;

			if (transform.RotationXY == 90 || transform.RotationXY == 270)
			{
				displayedFieldOfViewX = Math.Abs(destinationRectangle.Height * effectivePixelSizeX / 10);
				displayedFieldOfViewY = Math.Abs(destinationRectangle.Width * effectivePixelSizeY / 10);
			}
			else
			{
				displayedFieldOfViewX = Math.Abs(destinationRectangle.Width * effectivePixelSizeX / 10);
				displayedFieldOfViewY = Math.Abs(destinationRectangle.Height * effectivePixelSizeY / 10);
			}

			return String.Format("{0:F1} x {1:F1} cm", displayedFieldOfViewX, displayedFieldOfViewY);
		}
	}
}
