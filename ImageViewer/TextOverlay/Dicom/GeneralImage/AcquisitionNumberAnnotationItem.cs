using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class AcquisitionNumberAnnotationItem : DicomStringAnnotationItem
	{
		public AcquisitionNumberAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.AcquisitionNumber", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.AcquisitionNumber;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.AcquisitionNumber; }
		}
	}
}
