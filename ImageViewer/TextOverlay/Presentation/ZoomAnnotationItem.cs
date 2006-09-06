using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.TextOverlay.Presentation
{
	internal class ZoomAnnotationItem : AnnotationItem
	{
		public ZoomAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Presentation.Zoom", ownerProvider)
		{ 
		
		}

		public override string GetAnnotationText(PresentationImage presentationImage)
		{
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
