using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.ImagePlane
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class ImagePlaneAnnotationItemProvider : AnnotationItemProvider
	{
		public ImagePlaneAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.ImagePlane")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add((IAnnotationItem)new ImageOrientationPatientAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ImagePositionPatientAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PixelSpacingAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new SliceLocationAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new SliceThicknessAnnotationItem(this));

				return annotationItems;
			}
		}
	}
}
