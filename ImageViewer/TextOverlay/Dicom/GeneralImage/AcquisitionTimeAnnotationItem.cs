using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class AcquisitionTimeAnnotationItem : DicomTimeAnnotationItem
	{
		public AcquisitionTimeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.AcquisitionTime", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.AcquisitionTime;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.AcquisitionTime; }
		}
	}
}
