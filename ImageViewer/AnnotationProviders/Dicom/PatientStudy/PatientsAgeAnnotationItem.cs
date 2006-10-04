using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.PatientStudy
{
	internal class PatientsAgeAnnotationItem : DicomStringAnnotationItem
	{
		public PatientsAgeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("PatientStudy.PatientsAge", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			dicomValue = string.Empty;
			storedValueExists = false;

			//!! Uncomment once this item has been implemented in ImageSop class(es).
			//storedValueExists = true; 
			//dicomValue = dicomPresentationImage.ImageSop.PatientsAge;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsAge; }
		}
	}
}
