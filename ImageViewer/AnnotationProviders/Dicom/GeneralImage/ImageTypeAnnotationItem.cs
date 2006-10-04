using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralImage
{
	//!! VM = 1..N

	internal class ImageTypeAnnotationItem : DicomStringArrayAnnotationItem
	{
		public ImageTypeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.ImageType", ownerProvider)
		{
		}

		protected override string[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			return null;

			//!! Uncomment once this item has been implemented in ImageSop class(es).
			//return dicomPresentationImage.ImageSop.ImageType.Split('\\');
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ImageType; }
		}
	}
}
