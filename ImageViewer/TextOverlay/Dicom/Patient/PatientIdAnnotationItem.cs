using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.Patient
{
	internal class PatientIdAnnotationItem : DicomStringAnnotationItem
	{
		public PatientIdAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Patient.PatientId", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.PatientId;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientId; }
		}
	}
}
