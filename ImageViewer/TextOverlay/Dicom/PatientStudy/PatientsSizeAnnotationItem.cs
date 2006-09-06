using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.PatientStudy
{
	internal class PatientsSizeAnnotationItem : DicomStringAnnotationItem
	{
		public PatientsSizeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("PatientStudy.PatientsSize", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientsSize; }
		}
	}
}
