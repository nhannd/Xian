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

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.StudyDate;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.StudyDate; }
		}
	}
}
