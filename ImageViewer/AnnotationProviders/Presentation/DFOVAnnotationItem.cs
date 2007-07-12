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