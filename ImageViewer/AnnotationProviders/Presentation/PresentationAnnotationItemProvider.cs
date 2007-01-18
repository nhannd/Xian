using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PresentationAnnotationItemProvider : AnnotationItemProvider
	{
		public PresentationAnnotationItemProvider()
			: base("AnnotationItemProviders.Presentation")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add((IAnnotationItem)new ZoomAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new AppliedLutAnnotationItem(this));
				
				annotationItems.Add((IAnnotationItem)new DirectionalMarkerAnnotationItem(this, DirectionalMarkerAnnotationItem.ImageEdge.Bottom));
				annotationItems.Add((IAnnotationItem)new DirectionalMarkerAnnotationItem(this, DirectionalMarkerAnnotationItem.ImageEdge.Left));
				annotationItems.Add((IAnnotationItem)new DirectionalMarkerAnnotationItem(this, DirectionalMarkerAnnotationItem.ImageEdge.Right));
				annotationItems.Add((IAnnotationItem)new DirectionalMarkerAnnotationItem(this, DirectionalMarkerAnnotationItem.ImageEdge.Top));

				return annotationItems;
			}
		}
	}
}
