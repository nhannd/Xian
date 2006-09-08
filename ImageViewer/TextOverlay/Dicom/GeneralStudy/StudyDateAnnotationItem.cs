using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralStudy
{
	internal class StudyDateAnnotationItem : DicomDateAnnotationItem
	{
		public StudyDateAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralStudy.StudyDate", ownerProvider)
		{ 
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = true;
			dicomValue = dicomPresentationImage.ImageSop.StudyDate;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.StudyDate; }
		}
	}
}
