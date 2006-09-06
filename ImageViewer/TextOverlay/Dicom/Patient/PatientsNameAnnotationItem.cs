using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.Patient
{
	internal class PatientsNameAnnotationItem : DicomPersonNameAnnotationItem
	{
		public PatientsNameAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Patient.PatientsName", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.PatientsName;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsName; }
		}
	}
}
