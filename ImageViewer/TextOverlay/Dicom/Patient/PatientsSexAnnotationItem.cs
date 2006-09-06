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

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.PatientsSex;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsSex; }
		}
	}
}
