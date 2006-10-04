using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralStudy
{
	internal class AccessionNumberAnnotationItem : DicomStringAnnotationItem
	{
		public AccessionNumberAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralStudy.AccessionNumber", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = true;
			dicomValue = dicomPresentationImage.ImageSop.AccessionNumber;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.AccessionNumber; }
		}
	}
}
