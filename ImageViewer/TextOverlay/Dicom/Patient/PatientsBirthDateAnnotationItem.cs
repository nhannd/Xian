using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.Patient
{
	internal class PatientsBirthDateAnnotationItem : DicomDateAnnotationItem
	{
		public PatientsBirthDateAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Patient.PatientsBirthDate", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.PatientsBirthDate;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsBirthDate; }
		}
	}
}
