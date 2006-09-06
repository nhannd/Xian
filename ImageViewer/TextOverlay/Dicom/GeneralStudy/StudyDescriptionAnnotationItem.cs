using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralStudy
{
	internal class StudyDescriptionAnnotationItem : DicomStringAnnotationItem
	{
		public StudyDescriptionAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralStudy.StudyDescription", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.StudyDescription;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.StudyDescription; }
		}
	}
}
