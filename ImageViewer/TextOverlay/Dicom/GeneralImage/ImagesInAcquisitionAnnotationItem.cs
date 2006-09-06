using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class ImagesInAcquisitionAnnotationItem : DicomStringAnnotationItem
	{
		public ImagesInAcquisitionAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.ImagesInAcquisition", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.ImagesInAcquisition;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ImagesInAcquisition; }
		}
	}
}
