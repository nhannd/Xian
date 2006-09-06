using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;

namespace ClearCanvas.ImageViewer.TextOverlay.Presentation
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

				return annotationItems;
			}
		}
	}
}
