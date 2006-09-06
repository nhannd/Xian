using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class AcquisitionDateTimeAnnotationItem : DicomDateTimeAnnotationItem
	{
		public AcquisitionDateTimeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.AcquisitionDateTime", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.AcquisitionDateTime;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.AcquisitionDatetime; }
		}
	}
}
