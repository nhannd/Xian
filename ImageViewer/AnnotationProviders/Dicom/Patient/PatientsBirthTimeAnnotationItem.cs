using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Patient
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
