using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.PatientStudy
{
	internal class PatientsWeightAnnotationItem : DicomStringAnnotationItem
	{
		public PatientsWeightAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("PatientStudy.PatientsWeight", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsWeight; }
		}
	}
}
