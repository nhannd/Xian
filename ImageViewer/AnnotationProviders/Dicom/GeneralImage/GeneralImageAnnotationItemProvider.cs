using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralImage
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralImageAnnotationItemProvider : AnnotationItemProvider
	{
		public GeneralImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralImage")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add((IAnnotationItem)new AcquisitionDateAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new AcquisitionDateTimeAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new AcquisitionNumberAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new AcquisitionTimeAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ContentDateAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ContentTimeAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new DerivationDescriptionAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ImageCommentsAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ImagesInAcquisitionAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ImageTypeAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new InstanceNumberAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new LossyImageCompressionAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new LossyImageCompressionRatioAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientOrientationAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new QualityControlImageAnnotationItem(this));

				return annotationItems;
			}
		}
	}
}
