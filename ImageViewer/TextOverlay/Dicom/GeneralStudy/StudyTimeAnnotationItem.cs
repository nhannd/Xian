using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralStudy
{
	internal class StudyTimeAnnotationItem : DicomTimeAnnotationItem
	{
		public StudyTimeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralStudy.StudyTime", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.StudyTime;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.StudyTime; }
		}
	}
}
