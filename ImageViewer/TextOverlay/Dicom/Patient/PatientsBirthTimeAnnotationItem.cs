using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.Patient
{
	internal class PatientsBirthTimeAnnotationItem : DicomTimeAnnotationItem
	{
		public PatientsBirthTimeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Patient.PatientsBirthTime", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsBirthTime; }
		}
	}
}
