using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
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
			return dicomPresentationImage.ImageSop.ImageType.Split('\\');
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ImageType; }
		}
	}
}
