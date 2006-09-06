using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralStudy
{
	internal class AccessionNumberAnnotationItem : DicomStringAnnotationItem
	{
		public AccessionNumberAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralStudy.AccessionNumber", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.AccessionNumber;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.AccessionNumber; }
		}
	}
}
