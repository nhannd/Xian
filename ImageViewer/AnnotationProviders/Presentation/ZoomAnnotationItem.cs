using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal class ZoomAnnotationItem : AnnotationItem
	{
		public ZoomAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Presentation.Zoom", ownerProvider)
		{ 
		
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			string strAnnotationText = String.Empty;

			try
			{
				SpatialTransform transform = presentationImage.LayerManager.SelectedLayerGroup.SpatialTransform;
				strAnnotationText = transform.Scale.ToString("F2");
				strAnnotationText += "x";
			}
			catch
			{
			}

			return strAnnotationText;
		}
	}
}
