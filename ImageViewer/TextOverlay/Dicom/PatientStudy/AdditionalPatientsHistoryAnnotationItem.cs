using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.PatientStudy
{
	internal class AdditionalPatientsHistoryAnnotationItem : DicomStringAnnotationItem
	{
		public AdditionalPatientsHistoryAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("PatientStudy.AdditionalPatientsHistory", ownerProvider)
		{
		}
		
		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.AdditionalPatientsHistory;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.AdditionalPatientHistory; }
		}
	}
}
