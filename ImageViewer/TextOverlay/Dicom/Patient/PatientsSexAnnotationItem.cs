using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.Patient
{
	internal class PatientsSexAnnotationItem : DicomStringAnnotationItem
	{
		public PatientsSexAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Patient.PatientsSex", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = true;
			dicomValue = dicomPresentationImage.ImageSop.PatientsSex;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsSex; }
		}
	}
}
