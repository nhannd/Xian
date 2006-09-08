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

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = true;
			dicomValue = dicomPresentationImage.ImageSop.PatientsName;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsName; }
		}
	}
}
