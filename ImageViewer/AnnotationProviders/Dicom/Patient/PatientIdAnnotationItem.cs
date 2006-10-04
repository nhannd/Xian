using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Patient
{
	internal class PatientIdAnnotationItem : DicomStringAnnotationItem
	{
		public PatientIdAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Patient.PatientId", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = true;
			dicomValue = dicomPresentationImage.ImageSop.PatientId;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientId; }
		}
	}
}
