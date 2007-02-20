using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal sealed class ZoomAnnotationItem : ResourceResolvingAnnotationItem
	{
		public ZoomAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Presentation.Zoom", ownerProvider)
		{ 
		
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			ISpatialTransformProvider image = presentationImage as ISpatialTransformProvider;
			if (image  == null)
				return string.Empty;

			return String.Format("{0}{1}", new DoubleFormatter().Format(image.SpatialTransform.Scale), SR.Presentation_Zoom_Indicator);
		}
	}
}
