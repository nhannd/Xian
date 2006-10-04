using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Patient
{
	internal class PatientsBirthDateAnnotationItem : DicomDateAnnotationItem
	{
		public PatientsBirthDateAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Patient.PatientsBirthDate", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = true;
			dicomValue = dicomPresentationImage.ImageSop.PatientsBirthDate;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsBirthDate; }
		}
	}
}
