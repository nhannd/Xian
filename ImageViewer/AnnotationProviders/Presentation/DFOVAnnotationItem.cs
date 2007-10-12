#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

// Implement this later

/*
namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal sealed class DFOVAnnotationItem : ResourceResolvingAnnotationItem
	{
		public DFOVAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Presentation.DFOV", ownerProvider)
		{ 
		
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			IImageSopProvider imageSopProvider = presentationImage as IImageSopProvider;

			if (imageSopProvider  == null)
				return string.Empty;

			ImageSop imageSop = imageSopProvider.ImageSop;

			ISpatialTransformProvider spatialTransformProvider = presentationImage as ISpatialTransformProvider;

			if (spatialTransformProvider == null)
				return string.Empty;

			IImageSpatialTransform spatialTransform = spatialTransformProvider.SpatialTransform as IImageSpatialTransform;
			spatialTransform.

			double pixelSpacingX, pixelSpacingY;
			ImageSopHelper.GetModalityPixelSpacing(image.ImageSop, out pixelSpacingX, out pixelSpacingY);

			bool pixelSpacingInvalid =  pixelSpacingX <= float.Epsilon ||
										pixelSpacingY <= float.Epsilon ||
										double.IsNaN(pixelSpacingX) ||
										double.IsNaN(pixelSpacingY);

			if (pixelSpacingInvalid)
 				return String.Empty;
  
			// DFOV in cm
			double displayedFieldOfViewX = imageSop.Columns * pixelSpacingX / 10;
			double displayedFieldOfViewY = imageSop.Rows * pixelSpacingY / 10;

			string str = String.Format("{0:F1} x {1:F1} cm", displayedFieldOfViewX, displayedFieldOfViewY);

			return str;
		}
	}
}
*/