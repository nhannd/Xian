using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralImage
{
	internal class ContentTimeAnnotationItem : DicomTimeAnnotationItem
	{
		public ContentTimeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.ContentTime", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ContentTime; }
		}
	}
}
