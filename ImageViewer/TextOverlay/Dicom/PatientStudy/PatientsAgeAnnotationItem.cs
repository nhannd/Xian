using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.PatientStudy
{
	internal class PatientsAgeAnnotationItem : DicomStringAnnotationItem
	{
		public PatientsAgeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("PatientStudy.PatientsAge", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.PatientsAge;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsAge; }
		}
	}
}
